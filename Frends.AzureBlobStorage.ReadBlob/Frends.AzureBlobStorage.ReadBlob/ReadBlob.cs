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
        /// Read a single file to Azure blob storage using connection string or SAS Token.
        /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.AzureBlobStorage.ReadBlob)
        /// <param name="source">Information about which Blob to read.</param>
        /// <returns>Object { string content }</returns>
        /// </summary>

        public static Result ReadBlob([PropertyTab] Source source, CancellationToken cancellationToken)
        {
            var data = ReadBlobContent(source, cancellationToken);
            return new Result(data.ToString());
        }

        private static string ReadBlobContent(Source source, CancellationToken cancellationToken)
        {
            BlobClient blob = null;
            var uri = $"{source.Uri}/{source.ContainerName}/{source.BlobName}?";

            switch (source.AuthenticationMethod)
            {
                case AuthenticationMethod.Connectionstring:
                    if (string.IsNullOrEmpty(source.ConnectionString))
                        throw new Exception("Connection string required.");
                    blob = new BlobClient(source.ConnectionString, source.ContainerName, source.BlobName);
                    break;

                case AuthenticationMethod.Sastoken:
                    if (string.IsNullOrEmpty(source.SasToken) || string.IsNullOrEmpty(source.Uri))
                        throw new Exception("SAS Token and URI required.");
                    blob = new BlobClient(new Uri(uri), new AzureSasCredential(source.SasToken));
                    break;

                default:
                    break;
            }

            var result = blob.DownloadContent(cancellationToken).Value;
            var encoding = SetStringEncoding(result.Content.ToString(), source);

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
