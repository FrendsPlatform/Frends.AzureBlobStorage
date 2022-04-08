using System;
using System.ComponentModel;
using System.Threading;
using System.Text;
using Azure.Storage.Blobs;
using Azure;
using Frends.AzureBlobStorage.ReadBlob.Definitions;
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
            var uri = $"{source.Uri}/{source.ContainerName}/{source.BlobName}?";

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
            var encoding = SetStringEncoding(result.Value.Content.ToString(), source);

            return encoding.ToString();
        }

        private static string SetStringEncoding(string text, Source source)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            switch (source.Encoding)
            {
                case Encode.UTF8:
                    return Encoding.UTF8.GetString(bytes);
                case Encode.UTF32:
                    return Encoding.UTF32.GetString(bytes);
                case Encode.Unicode:
                    return Encoding.Unicode.GetString(bytes);
                case Encode.ASCII:
                    return Encoding.ASCII.GetString(bytes);
                default: 
                    return null;
            }
        }
    }
}
