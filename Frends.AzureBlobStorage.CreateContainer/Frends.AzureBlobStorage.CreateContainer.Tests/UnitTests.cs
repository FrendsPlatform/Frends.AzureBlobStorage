using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.CreateContainer.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;

namespace Frends.AzureBlobStorage.CreateContainer.Tests
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
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var container = blobServiceClient.GetBlobContainerClient(_containerName);

            await container.DeleteIfExistsAsync();
        }

        [TestMethod]
        public async Task TestCreateContainer() { 
            var result = await AzureBlobStorage.CreateContainer(new Input { ConnectionString = _connectionString, ContainerName = _containerName }, new CancellationToken());
            
            Assert.IsNotNull(result);
            Assert.AreEqual(new BlobClient(_connectionString, _containerName, "").Uri.ToString(), result.Uri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task TestCreateContainer_throws_ParameterEmpty()
        {
            var nameEmpty = await AzureBlobStorage.CreateContainer(new Input { ConnectionString = _connectionString, ContainerName = null }, new CancellationToken());
            var connectionEmpty = await AzureBlobStorage.CreateContainer(new Input { ConnectionString = null, ContainerName = _containerName }, new CancellationToken());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task TestCreateContainer_throws_ParameterNotValid()
        {
            var wrongFormat = await AzureBlobStorage.CreateContainer(new Input { ConnectionString = "Not valid parameter", ContainerName = "Valid name" }, new CancellationToken());
            var noAccount = await AzureBlobStorage.CreateContainer(new Input { ConnectionString = "name=value", ContainerName = "Valid name" }, new CancellationToken());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task TestCreateContainer_throws_ClientNotFound()
        {
            var noAccount = await AzureBlobStorage.CreateContainer(new Input { ConnectionString = "DefaultEndpointsProtocol=https;AccountName=unitTestStorage;AccountKey=abcdefghijklmnopqrstuyxz123456789;EndpointSuffix=core.windows.net", ContainerName = _containerName }, new System.Threading.CancellationToken());
        }
    }
}
