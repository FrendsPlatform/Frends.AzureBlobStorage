using System;
using System.ComponentModel;
using System.Threading;
using System.Text;
using Azure.Storage.Blobs;
using Azure;
using Frends.AzureBlobStorage.ReadBlob.Definitions;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ReadBlob
{
    /// <summary>
    /// Azure Storage task.
    /// </summary>
    public class AzureBlobStorage
    {
        /// <summary>
        /// Encode and read a single file from Azure Storage using connection string or SAS Token authentication.
        /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.AzureBlobStorage.ReadBlob)
        /// </summary>
        /// <param name="source">Source connection parameters.</param>
        /// <param name="options">Options for the task</param>
        /// <param name="cancellationToken">Token to cancel DownloadContent. This is generated by Frends.</param>
        /// <returns>object { string Content }</returns>

        public static async Task<Result> ReadBlob([PropertyTab] Source source, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            var blob = CreateBlobClient(source);
            var result = (await blob.DownloadContentAsync(cancellationToken)).Value;
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
                    if (string.IsNullOrWhiteSpace(source.ConnectionString))
                        throw new Exception("Connection string required.");
                    blob = new BlobClient(source.ConnectionString, source.ContainerName, source.BlobName);
                    break;

                case AuthenticationMethod.SASToken:
                    if (string.IsNullOrWhiteSpace(source.SASToken) || string.IsNullOrWhiteSpace(source.URI))
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
