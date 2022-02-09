using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using MimeMapping;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

#pragma warning disable CS1591

namespace Frends.AzureBlobStorage.UploadBlob
{
    public class AzureBlobStorage
    {
        /// <summary>
        ///     Uploads a single file to Azure blob storage.
        ///     See https://github.com/FrendsPlatform/Frends.AzureBlobStorage/tree/main/Frends.AzureBlobStorage.UploadBlob
        ///     Will create given container on connection if necessary.
        /// </summary>
        /// <returns>Object { string Uri, string SourceFile }</returns>
        public static async Task<UploadOutput> UploadBlob(
            [PropertyTab] UploadInput input,
            [PropertyTab] DestinationProperties destinationProperties,
            CancellationToken cancellationToken)
        {
            // Check that source file exists.
            var fi = new FileInfo(input.SourceFile);
            if (!fi.Exists) throw new ArgumentException($"Source file {input.SourceFile} does not exist", nameof(input.SourceFile));

            // Get container.
            var container = Utils.GetBlobContainer(destinationProperties.ConnectionString, destinationProperties.ContainerName);

            try
            {
                if (destinationProperties.CreateContainerIfItDoesNotExist) await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Checking if container exists or creating new container caused an exception.", ex);
            }

            string fileName;
            if (string.IsNullOrWhiteSpace(destinationProperties.RenameTo) && input.Compress) fileName = fi.Name + ".gz";
            else if (string.IsNullOrWhiteSpace(destinationProperties.RenameTo)) fileName = fi.Name;
            else fileName = destinationProperties.RenameTo;

            // Return URI to uploaded blob and source file path.
            switch (destinationProperties.BlobType)
            {
                case AzureBlobType.Append:
                    return await AppendBlob(input, destinationProperties, fi, fileName, cancellationToken);
                case AzureBlobType.Page:
                    return await UploadPageBlob(input, destinationProperties, fi, fileName, cancellationToken);
                default:
                    return await UploadBlockBlob(input, destinationProperties, fi, fileName, cancellationToken);
            }
        }

        private static Encoding GetEncoding(string target)
        {
            switch (target.ToLower())
            {
                case "utf-8":
                    return Encoding.UTF8;
                case "utf-32":
                    return Encoding.UTF32;
                case "unicode":
                    return Encoding.Unicode;
                case "ascii":
                    return Encoding.ASCII;
                default:
                    return Encoding.UTF8;
            }
        }

        private static async Task<UploadOutput> UploadBlockBlob(
            UploadInput input,
            DestinationProperties destinationProperties,
            FileInfo fi,
            string fileName,
            CancellationToken cancellationToken)
        {
            // Get the destination blob, rename if necessary.
            var blob = new BlobClient(destinationProperties.ConnectionString, destinationProperties.ContainerName, fileName);

            var contentType = string.IsNullOrWhiteSpace(destinationProperties.ContentType)
                ? MimeUtility.GetMimeMapping(fi.Name)
                : destinationProperties.ContentType;

            var encoding = GetEncoding(destinationProperties.FileEncoding);

            // Delete blob if user requested overwrite.
            if (destinationProperties.Overwrite) await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);

            var progressHandler = new Progress<long>(progress =>
            {
                Console.WriteLine("Bytes uploaded: {0}", progress);
            });

            // Setup the number of the concurrent operations.
            var uploadOptions = new BlobUploadOptions
            {
                ProgressHandler = progressHandler,
                TransferOptions = new StorageTransferOptions { MaximumConcurrency = destinationProperties.ParallelOperations },
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType, ContentEncoding = input.Compress ? "gzip" : encoding.WebName }
            };

            // Begin and await for upload to complete.
            try
            {
                using (var stream = Utils.GetStream(input.Compress, input.ContentsOnly, encoding, fi)) await blob.UploadAsync(stream, uploadOptions, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception("UploadFileAsync: Error occured while uploading file to blob storage", e);
            }
            return new UploadOutput { SourceFile = input.SourceFile, Uri = blob.Uri.ToString() };
        }

        private static async Task<UploadOutput> AppendBlob(
            UploadInput input,
            DestinationProperties destinationProperties,
            FileInfo fi,
            string fileName,
            CancellationToken cancellationToken)
        {
            // Get the destination blob, rename if necessary.
            var blob = new AppendBlobClient(destinationProperties.ConnectionString, destinationProperties.ContainerName, fileName);

            var encoding = GetEncoding(destinationProperties.FileEncoding);

            // Delete blob if user requested overwrite.
            if (destinationProperties.Overwrite)
            {
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);

                var contentType = string.IsNullOrWhiteSpace(destinationProperties.ContentType)
                ? MimeUtility.GetMimeMapping(fi.Name)
                : destinationProperties.ContentType;

                var uploadOptions = new AppendBlobCreateOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = contentType, ContentEncoding = input.Compress ? "gzip" : encoding.WebName }
                };

                await blob.CreateAsync(uploadOptions, cancellationToken);
            }

            var progressHandler = new Progress<long>(progress =>
            {
                Console.WriteLine("Bytes uploaded: {0}", progress);
            });

            // Begin and await for upload to complete.
            try
            {
                using (var stream = Utils.GetStream(false, true, encoding, fi)) await blob.AppendBlockAsync(stream, null, null, progressHandler, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while appending a block.", e);
            }

            return new UploadOutput { SourceFile = input.SourceFile, Uri = blob.Uri.ToString() };
        }

        private static async Task<UploadOutput> UploadPageBlob(
            UploadInput input,
            DestinationProperties destinationProperties,
            FileInfo fi,
            string fileName,
            CancellationToken cancellationToken)
        {
            // Get the destination blob, rename if necessary.
            var blob = new PageBlobClient(destinationProperties.ConnectionString, destinationProperties.ContainerName, fileName);

            var encoding = GetEncoding(destinationProperties.FileEncoding);

            // Delete blob if user requested overwrite.
            if (destinationProperties.Overwrite) await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);

            var progressHandler = new Progress<long>(progress =>
            {
                Console.WriteLine("Bytes uploaded: {0}", progress);
            });

            // Begin and await for upload to complete.
            try
            {
                using (var stream = Utils.GetStream(false, true, encoding, fi)) await blob.UploadPagesAsync(stream, 512L, null, null, progressHandler, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while uploading page blob", e);
            }

            return new UploadOutput { SourceFile = input.SourceFile, Uri = blob.Uri.ToString() };
        }
    }
}