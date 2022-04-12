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
        /// <param name="options">Options regarding the error handling.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { string Success }</returns>
        public static async Task<Result> DeleteContainer([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(input.ConnectionString))
                throw new ArgumentNullException("ConnectionString parameter can't be empty.");
            else if(string.IsNullOrWhiteSpace(input.ContainerName))
                throw new ArgumentNullException("ContainerName parameter can't be empty.");

            // get container
            var container = GetBlobContainer(input.ConnectionString, input.ContainerName);

            // if container not found and set not to throw an error - returns false
            if (!await container.ExistsAsync(cancellationToken) && !options.ThrowErrorIfContainerDoesNotExists) return new Result(false, "Container was not found.");
            // if container not found and set to throw an error - throws an Exception
            else if (!await container.ExistsAsync(cancellationToken) && options.ThrowErrorIfContainerDoesNotExists) throw new Exception("DeleteContainer: The blob container was not found.");

            // delete container
            try
            {
                var result = await container.DeleteIfExistsAsync(null, cancellationToken);
                return new Result(result, "Container deleted successfully.");
            }
            catch (Exception e)
            {
                throw new Exception("DeleteContaine: Error occured while trying to delete blob container.", e);
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
