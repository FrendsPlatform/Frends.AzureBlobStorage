using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DeleteContainer.Definitions;
using Azure.Storage.Blobs.Models;

namespace Frends.AzureBlobStorage.DeleteContainer.Tests
{
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        ///     Connection string for Azure Storage Emulator
        /// </summary>
        private readonly string _connectionString = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");

        /// <summary>
        ///     Container name for tests
        /// </summary>
        private string _containerName;

        [TestInitialize]
        public void TestSetup()
        {
            // Generate unique container name to avoid conflicts when running multiple tests
            _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            // delete whole container after running tests
            var container = GetBlobServiceClient();
            await container.DeleteIfExistsAsync();
        }

        private BlobContainerClient GetBlobServiceClient() {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            return blobServiceClient.GetBlobContainerClient(_containerName);
        }

        [TestMethod]
        public async Task TestDeleteContainer() {
            // Test method returns true when container doesn't exist
            var result = await AzureBlobStorage.DeleteContainer(new Input { ConnectionString = _connectionString, ContainerName = _containerName}, new CancellationToken());
            Assert.IsTrue(result.Success);

            // Test method returns true when container that exists is deleted
            var container = GetBlobServiceClient();
            await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, new CancellationToken());
            var deleted = await AzureBlobStorage.DeleteContainer(new Input { ConnectionString = _connectionString, ContainerName = _containerName }, new CancellationToken());
            Assert.IsTrue(deleted.Success);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task TestDeleteContainer_throws_ParameterEmpty()
        {
            var nameEmpty = await AzureBlobStorage.DeleteContainer(new Input { ConnectionString = _connectionString, ContainerName = null }, new CancellationToken());
            var connectionEmpty = await AzureBlobStorage.DeleteContainer(new Input { ConnectionString = null, ContainerName = _containerName }, new CancellationToken());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task TestDeleteContainer_throws_ParameterNotValid()
        {
            var wrongFormat = await AzureBlobStorage.DeleteContainer(new Input { ConnectionString = "Not valid parameter", ContainerName = "Valid name" }, new CancellationToken());
            var noAccount = await AzureBlobStorage.DeleteContainer(new Input { ConnectionString = "name=value", ContainerName = "Valid name" }, new CancellationToken());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task TestDeleteContainer_throws_ClientNotFound()
        {
            var noAccount = await AzureBlobStorage.DeleteContainer(new Input { ConnectionString = "DefaultEndpointsProtocol=https;AccountName=unitTestStorage;AccountKey=abcdefghijklmnopqrstuyxz123456789;EndpointSuffix=core.windows.net", ContainerName = _containerName }, new CancellationToken());
        }
    }
}
