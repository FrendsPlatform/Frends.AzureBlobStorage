using System;
using System.Threading;
using System.Threading.Tasks;
using Frends.AzureBlobStorage.DeleteContainer.Definitions;
using Azure.Storage.Blobs;
using System.ComponentModel;

namespace Frends.AzureBlobStorage.DeleteContainer
{
    public static class AzureBlobStorage
    {
        /// <summary>
        /// Deletes a container from Azure blob storage.
        /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.AzureBlobStorage.DeleteContainer)
        /// </summary>
        /// <param name="input">Information about the container destination.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { string Success }</returns>
        public static async Task<Result> DeleteContainer([PropertyTab] Input input, CancellationToken cancellationToken)
        {
            if (input.ConnectionString == null || input.ContainerName == null)
                throw new ArgumentNullException("Given parameter can't be empty.");

            // get container
            var container = GetBlobContainer(input.ConnectionString, input.ContainerName);

            if (!await container.ExistsAsync(cancellationToken) && !input.IfThrow) return new Result(true);
            else if (!await container.ExistsAsync(cancellationToken) && input.IfThrow) throw new Exception("Container was not found.");

            // delete container
            try
            {
                var result = await container.DeleteIfExistsAsync(null, cancellationToken);
                return new Result(result);
            }
            catch (Exception e)
            {
                throw new Exception("DeleteContainerAsync: Error occured while trying to delete blob container", e);
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
            catch (Exception ex)
            {
                throw new Exception("Fetching an account caused an exception.", ex);
            }
        }
    }
}
