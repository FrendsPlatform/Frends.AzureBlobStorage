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
        /// Creates a container to Azure blob storage.
        /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.AzureBlobStorage.CreateContainer)
        /// </summary>
        /// <param name="input">Information about the container destination.</param>
        /// <returns>Object { string Uri }</returns>
        public static async Task<Result> CreateContainer([PropertyTab] Input input, CancellationToken cancellationToken)
        {
            if(input.ConnectionString == null || input.ContainerName == null)
                throw new ArgumentNullException("Given parameter can't be empty."); 

            // get container
            var container = GetBlobContainer(input.ConnectionString,
                input.ContainerName);

            try
            {
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
            try
            {
                // initialize azure account
                var blobServiceClient = new BlobServiceClient(connectionString);

                // Fetch the container client
                return blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (Exception ex) {
                throw new Exception("Fetching an account caused an exception.", ex);
            }
        }
    }
}
