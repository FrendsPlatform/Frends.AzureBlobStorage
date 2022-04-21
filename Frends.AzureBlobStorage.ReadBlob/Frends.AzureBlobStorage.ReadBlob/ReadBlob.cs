using System;
using System.ComponentModel;
using System.Threading;
using System.Text;
using Azure.Storage.Blobs;
using Azure;
using Frends.AzureBlobStorage.ReadBlob.Definitions;

namespace Frends.AzureBlobStorage.ReadBlob
{
    public class AzureBlobStorage
    {
        /// <summary>
        /// Read a single file from Azure Storage.
        /// </summary>
        /// <param name="source">Source connection parameters.</param>
        /// <param name="options">Options for the task</param>
        /// <returns>object { string Content }</returns>

        public static Result ReadBlob([PropertyTab] Source source, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            var blob = CreateBlobClient(source);
            var result = blob.DownloadContent(cancellationToken).Value;
            var encoding = SetStringEncoding(result.Content.ToString(), options.Encoding);

            return new Result(encoding);
        }

        private static BlobClient CreateBlobClient(Source source)
        {
            BlobClient blob = null;
            var uri = $"{source.URI}/{source.ContainerName}/{source.BlobName}?";

            switch (source.AuthenticationMethod)
            {
                case AuthenticationMethod.ConnectionString:
                    if (string.IsNullOrEmpty(source.ConnectionString))
                        throw new Exception("Connection string required.");
                    blob = new BlobClient(source.ConnectionString, source.ContainerName, source.BlobName);
                    break;

                case AuthenticationMethod.SASToken:
                    if (string.IsNullOrEmpty(source.SASToken) || string.IsNullOrEmpty(source.URI))
                        throw new Exception("SAS Token and URI required.");
                    blob = new BlobClient(new Uri(uri), new AzureSasCredential(source.SASToken));
                    break;
            }

            return blob;
        }

        private static string SetStringEncoding(string text, Encode encoding)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            switch (encoding)
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
                    return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
