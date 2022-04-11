using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Frends.AzureBlobStorage.CreateContainer.Definitions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Frends.AzureBlobStorage.CreateContainer
{
    public static class AzureBlobStorage
    {
        /// <summary>
        /// Downloads Blob to a file.
        /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.AzureBlobStorage.CreateContainer)
        /// </summary>
        /// <param name="input">Information about the container destination.</param>
        /// <returns>Object { string FileName, string Directory, string FullPath}</returns>
        /// <summary>
        ///     Creates a container to Azure blob storage.
        /// </summary>
        /// <returns>Object { string Uri }</returns>
        public static async Task<Result> CreateContainer([PropertyTab] Input input, CancellationToken cancellationToken)
        {
            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();

            if(input.ConnectionString == null || input.ContainerName == null)
                throw new ArgumentNullException("Given parameter can't be empty."); 

            // get container
            var container = GetBlobContainer(input.ConnectionString,
                input.ContainerName);

            try
            {
                // check for interruptions
                cancellationToken.ThrowIfCancellationRequested();

                await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, cancellationToken);
                return new Result(new BlobClient(input.ConnectionString, input.ContainerName, "").Uri.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Creating a new container caused an exception.", ex);
            }
        }

        private static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
        {
            // initialize azure account
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Fetch the container client
            return blobServiceClient.AccountName != null ? blobServiceClient.GetBlobContainerClient(containerName) : throw new Exception("Account not found.");
        }
    }
}
