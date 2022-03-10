using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.AzureBlobStorage.DeleteBlob.Tests
{
    [TestClass]
    public class DeleteTest
    {
        /// <summary>
        /// Connection string for Azure Storage.
        /// </summary>
        private readonly string _connectionString = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");

        /// <summary>
        /// Container name for tests.
        /// </summary>
        private readonly string _containerName = "test-container";

        [TestCleanup]
        public async Task Cleanup()
        {
            // Delete whole container after running tests.
            var container = GetBlobContainer(_connectionString, _containerName);
            await container.DeleteIfExistsAsync();
        }

        [TestMethod]
        public async Task DeleteBlobAsync_ShouldReturnTrueWithNonexistingBlob()
        {
            var input = new Input
            {
                BlobName = Guid.NewGuid().ToString(),
                ContainerName = _containerName,
                ConnectionString = _connectionString,
            };
            var container = GetBlobContainer(_connectionString, _containerName);
            await container.CreateIfNotExistsAsync();

            var result = await AzureBlobStorage.DeleteBlob(input, new Options(), new CancellationToken());

            Assert.IsTrue(result.Success, "DeleteBlob should've returned true when trying to delete non existing blob");
        }

        [TestMethod]
        public async Task DeleteBlobAsync_ShouldReturnTrueWithNonexistingContainer()
        {
            var input = new Input
            {
                BlobName = Guid.NewGuid().ToString(),
                ConnectionString = _connectionString,
                ContainerName = Guid.NewGuid().ToString()
            };

            var result = await AzureBlobStorage.DeleteBlob(input, new Options(), new CancellationToken());

            Assert.IsTrue(result.Success, "DeleteBlob should've returned true when trying to delete blob in non existing container");
        }

        #region HelperMethods

        private static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
        {
            // Initialize azure account.
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Fetch the container client.
            return blobServiceClient.GetBlobContainerClient(containerName);
        }

        #endregion
    }
}
