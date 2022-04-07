using System;
using System.ComponentModel;
using System.Threading;
using System.Text;
using Azure.Storage.Blobs;
using Azure;
#pragma warning disable 1591
namespace Frends.AzureBlobStorage.ReadBlob
{
    public class AzureBlobStorage
    {
        /// <summary>
        /// Read a single file to Azure blob storage using connection string or SAS Token.
        /// [Documentation](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/tree/main/Frends.AzureBlobStorage.ReadBlob)
        /// </summary>

        public static Result ReadBlob([PropertyTab] Source source, CancellationToken cancellationToken)
        {
            var data = ReadBlobContentAsyncTest(source, cancellationToken);
            return new Result(data.ToString());
        }

        private static string ReadBlobContentAsyncTest(Source source, CancellationToken cancellationToken)
        {
            BlobClient blob;
            var uri = $"https://teemusbfrdstrg.blob.core.windows.net/{source.ContainerName}/{source.BlobName}?";

            if (String.IsNullOrEmpty(source.ConnectionString))
            {
                blob = new BlobClient(new Uri(uri), new AzureSasCredential(source.SasToken));
            }
            else
            {
                blob = new BlobClient(source.ConnectionString, source.ContainerName, source.BlobName);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var result = blob.DownloadContentAsync(cancellationToken).Result;
            var encoding = SetStringEncoding(result.Value.Content.ToString(), source.Encoding.ToString());

            return encoding.ToString();

            throw new Exception("Problems with connecting to Azure Blob Storage. Please check connection settings.");
        }

        private static string SetStringEncoding(string text, string encoding)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            switch (encoding.ToLower())
            {
                case "utf8":
                    return Encoding.UTF8.GetString(bytes);
                case "utf32":
                    return Encoding.UTF32.GetString(bytes);
                case "unicode":
                    return Encoding.Unicode.GetString(bytes);
                case "ascii":
                    return Encoding.ASCII.GetString(bytes);
                default:
                    throw new Exception("Provided encoding is not supported. Please check supported encodings from Encoding-option.");
            }
        }
    }
}
