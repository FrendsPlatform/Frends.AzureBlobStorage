using Azure.Storage.Blobs;

#pragma warning disable CS1591

namespace Frends.AzureBlobStorage.DownloadBlob
{
    public class Utils
    {
        public static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
        {
            // Initialize azure account.
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Fetch the container client.
            return blobServiceClient.GetBlobContainerClient(containerName);
        }
    }
}
