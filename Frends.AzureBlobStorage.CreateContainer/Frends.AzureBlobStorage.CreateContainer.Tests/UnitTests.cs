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
        private readonly string _connectionString = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");  
        private readonly string _appID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_AppID");
        private readonly string _tenantID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_TenantID");
        private readonly string _clientSecret = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ClientSecret");
        private readonly string _storageAccount = "testsorage01";
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
        public async Task TestCreateContainer() 
        { 
            var result = await AzureBlobStorage.CreateContainer(new Input { ConnectionString = _connectionString, ContainerName = _containerName }, new CancellationToken());
            Assert.IsNotNull(result);
            Assert.AreEqual(new BlobClient(_connectionString, _containerName, "").Uri.ToString(), result.Uri);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task TestCreateContainer_throws_ParameterNotValid()
        {
            await AzureBlobStorage.CreateContainer(new Input { ConnectionString = "Not valid parameter", ContainerName = "Valid name" }, new CancellationToken());
            await AzureBlobStorage.CreateContainer(new Input { ConnectionString = "name=value", ContainerName = "Valid name" }, new CancellationToken());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task TestCreateContainer_throws_ClientNotFound()
        {
            await AzureBlobStorage.CreateContainer(new Input { ConnectionString = "DefaultEndpointsProtocol=https;AccountName=unitTestStorage;AccountKey=abcdefghijklmnopqrstuyxz123456789;EndpointSuffix=core.windows.net", ContainerName = _containerName }, new CancellationToken());
        }

        [TestMethod]
        public async Task AccessTokenAuthenticationTest()
        {
            var containerName = "test" + Guid.NewGuid().ToString();
            var _conn = new OAuthConnection()
            {
                StorageAccountName = _storageAccount,
                ApplicationID = _appID,
                TenantID = _tenantID,
                ClientSecret = _clientSecret
            };

            var input = new Input
            {
                ConnectionMethod = ConnectionMethod.OAuth2,
                ContainerName = containerName,
                Connection = new[] {_conn }
            };

            var result = await AzureBlobStorage.CreateContainer(input, default);
            Assert.IsTrue(result.Success);
        }
    }
}
