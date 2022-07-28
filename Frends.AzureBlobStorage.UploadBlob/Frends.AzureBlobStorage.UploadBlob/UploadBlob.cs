﻿using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using MimeMapping;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.IO.Compression;

namespace Frends.AzureBlobStorage.UploadBlob;
/// <summary>
/// Azure Blob Storage Task.
/// </summary>
public class AzureBlobStorage
{
    /// <summary>
    /// Uploads a single file to Azure blob storage.
    /// Will create given container on connection if necessary.
    /// [Documentation](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/tree/main/Frends.AzureBlobStorage.UploadBlob)
    /// </summary>
    /// <returns>Object { string Uri, string SourceFile }</returns>
    public static async Task<Result> UploadBlob([PropertyTab] Source source, [PropertyTab] Destination destination, CancellationToken cancellationToken)
    {
        if (!File.Exists(source.SourceFile))
            throw new ArgumentException($"Source file {source.SourceFile} does not exist");

        var fi = destination.Append && await BlobExists(destination, cancellationToken) ? await AppendAny(source, destination, cancellationToken) : new FileInfo(source.SourceFile);

        var blobServiceClient = new BlobServiceClient(destination.ConnectionString);
        var container = blobServiceClient.GetBlobContainerClient(destination.ContainerName);

        if (fi != null)
        {
            try
            {
                if (destination.CreateContainerIfItDoesNotExist) await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Checking if container exists or creating new container caused an exception.{ex}");
            }

            string fileName;
            if (string.IsNullOrWhiteSpace(destination.RenameTo) && source.Compress) fileName = fi.Name + ".gz";
            else if (string.IsNullOrWhiteSpace(destination.RenameTo)) fileName = fi.Name;
            else fileName = destination.RenameTo;

            // Return URI to uploaded blob and source file path.
            return destination.BlobType switch
            {
                AzureBlobType.Append => await AppendBlob(source, destination, fi, fileName, cancellationToken),
                AzureBlobType.Page => await UploadPageBlob(source, destination, fi, fileName, cancellationToken),
                _ => await UploadBlockBlob(source, destination, fi, fileName, cancellationToken),
            };
        }
        else
            return new Result { SourceFile = source.SourceFile, Uri = source.SourceFile };
    }

    private static async Task<bool> BlobExists(Destination destination, CancellationToken cancellationToken)
    {
        try
        {
            var blob = new BlobClient(destination.ConnectionString, destination.ContainerName, destination.BlobName);
            return await blob.ExistsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"BlobExists: Error while checking if blob exists. {ex}");
        }
    }

    private static async Task<FileInfo> AppendAny(Source source, Destination destination, CancellationToken cancellationToken)
    {
        try
        {
            var blob = new BlobClient(destination.ConnectionString, destination.ContainerName, destination.BlobName);
            var blobProperties = blob.GetPropertiesAsync(cancellationToken: cancellationToken);

            if (blobProperties.Result.Value.BlobType.Equals(BlobType.Append))
            {
                var appendBlob = new AppendBlobClient(destination.ConnectionString, destination.ContainerName, destination.BlobName);
                var appendBlobMaxAppendBlockBytes = appendBlob.AppendBlobMaxAppendBlockBytes;

                using var file = File.OpenRead(source.SourceFile);
                int bytesRead;
                var buffer = new byte[appendBlobMaxAppendBlockBytes];
                while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var newArray = new Span<byte>(buffer, 0, bytesRead).ToArray();
                    Stream stream = new MemoryStream(newArray) { Position = 0 };
                    await appendBlob.AppendBlockAsync(stream, cancellationToken: cancellationToken);
                }

                return null;
            }
            else
            {
                var appendFile = Path.Combine(destination.DownloadFolder, destination.BlobName);
                Directory.CreateDirectory(destination.DownloadFolder);
                await blob.DownloadToAsync(Path.Combine(destination.DownloadFolder, destination.BlobName), cancellationToken);

                using (var sourceData = new StreamReader(source.SourceFile))
                using (var destinationFile = File.AppendText(appendFile))
                {
                    var line = sourceData.ReadLine();
                    destinationFile.Write(line); //WriteLine will mess up Page blob upload.
                };

                return new FileInfo(appendFile);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"AppendAny: Error occured while appending. {ex}");
        }
    }

    private static async Task<Result> UploadBlockBlob(Source source, Destination destinationProperties, FileInfo fi, string fileName, CancellationToken cancellationToken)
    {
        var blob = new BlobClient(destinationProperties.ConnectionString, destinationProperties.ContainerName, fileName);
        var contentType = string.IsNullOrWhiteSpace(destinationProperties.ContentType) ? MimeUtility.GetMimeMapping(fi.Name) : destinationProperties.ContentType;
        var encoding = GetEncoding(destinationProperties.FileEncoding);

        if (destinationProperties.Overwrite) await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);

        var progressHandler = new Progress<long>(progress => { Console.WriteLine("Bytes uploaded: {0}", progress); });

        var uploadOptions = new BlobUploadOptions
        {
            ProgressHandler = progressHandler,
            TransferOptions = new StorageTransferOptions { MaximumConcurrency = destinationProperties.ParallelOperations },
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType, ContentEncoding = source.Compress ? "gzip" : encoding.WebName }
        };

        try
        {
            using var stream = GetStream(source.Compress, source.ContentsOnly, encoding, fi);
            await blob.UploadAsync(stream, uploadOptions, cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception("UploadFileAsync: Error occured while uploading file to blob storage", e);
        }
        return new Result { SourceFile = source.SourceFile, Uri = blob.Uri.ToString() };
    }

    private static async Task<Result> AppendBlob(Source source, Destination destinationProperties, FileInfo fi, string fileName, CancellationToken cancellationToken)
    {
        var blob = new AppendBlobClient(destinationProperties.ConnectionString, destinationProperties.ContainerName, fileName);
        var encoding = GetEncoding(destinationProperties.FileEncoding);

        if (destinationProperties.Overwrite || !await blob.ExistsAsync(cancellationToken))
        {
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);
            var contentType = string.IsNullOrWhiteSpace(destinationProperties.ContentType) ? MimeUtility.GetMimeMapping(fi.Name) : destinationProperties.ContentType;
            var uploadOptions = new AppendBlobCreateOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType, ContentEncoding = source.Compress ? "gzip" : encoding.WebName } };
            await blob.CreateAsync(uploadOptions, cancellationToken);
        }

        var progressHandler = new Progress<long>(progress => { Console.WriteLine("Bytes uploaded: {0}", progress); });

        try
        {
            using var stream = GetStream(false, true, encoding, fi);
            await blob.AppendBlockAsync(stream, null, null, progressHandler, cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception("Error occured while appending a block.", e);
        }

        return new Result { SourceFile = source.SourceFile, Uri = blob.Uri.ToString() };
    }

    private static async Task<Result> UploadPageBlob(Source source, Destination destination, FileInfo fi, string fileName, CancellationToken cancellationToken)
    {
        var blob = new PageBlobClient(destination.ConnectionString, destination.ContainerName, fileName);
        var encoding = GetEncoding(destination.FileEncoding);
        var maxSize = destination.PageMaxSize < 512 ? 512 : destination.PageMaxSize;
        var fiMinLenght = fi.Length;

        //Upload size can't be over or same as Page's full size.
        while (maxSize % 512 != 0) maxSize += (maxSize / 512);
        while (fiMinLenght % 512 != 0) fiMinLenght += (fiMinLenght / 512);
        while (maxSize < fiMinLenght) maxSize += 512;

        if (destination.Overwrite) await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);

        var progressHandler = new Progress<long>(progress => { Console.WriteLine("Bytes uploaded: {0}", progress); });

        try
        {
            if(fiMinLenght > 8796093022208 || maxSize > 8796093022208) //Handle over 8 TB
                throw new Exception($"UploadPageBlob: Required minimum size of Page upload: {fiMinLenght} or Page blob maximum size: {maxSize} is over 8 TB.");
            
            using var stream = GetStream(false, true, encoding, fi);
            await blob.CreateAsync(maxSize, cancellationToken: cancellationToken);
            await blob.UploadPagesAsync(stream, destination.PageOffset == -1 ? maxSize - fiMinLenght : destination.PageOffset, cancellationToken: cancellationToken);

            if(destination.DeleteTempFile)
                File.Delete(fi.FullName);
        }
        catch (Exception e)
        {
            throw new Exception("Error occured while uploading page blob", e);
        }

        return new Result { SourceFile = source.SourceFile, Uri = blob.Uri.ToString() };
    }

    /// <summary>
    /// Gets correct stream object.
    /// Does not always dispose, so use using.
    /// </summary>
    private static Stream GetStream(bool compress, bool fromString, Encoding encoding, FileInfo file)
    {
        var fileStream = File.OpenRead(file.FullName);

        if (!compress && !fromString) return fileStream;

        byte[] bytes;
        if (!compress)
        {
            using (var reader = new StreamReader(fileStream, encoding)) bytes = encoding.GetBytes(reader.ReadToEnd());
            return new MemoryStream(bytes);
        }

        using (var outStream = new MemoryStream())
        {
            using (var gzip = new GZipStream(outStream, CompressionMode.Compress))
            {
                if (!fromString) fileStream.CopyTo(gzip);
                else
                    using (var reader = new StreamReader(fileStream, encoding))
                    {
                        var content = reader.ReadToEnd();
                        using var encodedMemory = new MemoryStream(encoding.GetBytes(content));
                        encodedMemory.CopyTo(gzip);
                    }
            }
            bytes = outStream.ToArray();
        }
        fileStream.Dispose();

        var memStream = new MemoryStream(bytes);
        return memStream;
    }

    private static Encoding GetEncoding(string target)
    {
        return target.ToLower() switch
        {
            "utf-32" => Encoding.UTF32,
            "unicode" => Encoding.Unicode,
            "ascii" => Encoding.ASCII,
            _ => Encoding.UTF8,
        };
    }
}
