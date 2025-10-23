using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.CreateContainer.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;

namespace Frends.AzureBlobStorage.CreateContainer.Tests;

[TestClass]
public class UnitTests
{
    Input input;
    Connection connection;

    private readonly string _connectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
    private readonly string _appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string _storageAccount = "frendstaskstestcontainer";
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
        var input = new Input { ContainerName = _containerName };
        var connection = new Connection { AuthenticationMethod = ConnectionMethod.ConnectionString, ConnectionString = _connectionString };
        var result = await AzureBlobStorage.CreateContainer(input, connection, new CancellationToken());
        Assert.IsNotNull(result);
        Assert.AreEqual(new BlobClient(_connectionString, _containerName, "").Uri.ToString(), result.Uri);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestCreateContainer_throws_ParameterNotValid()
    {
        var input = new Input { ContainerName = "Valid name" };
        var connection1 = new Connection { AuthenticationMethod = ConnectionMethod.ConnectionString, ConnectionString = "Not valid parameter" };
        var connection2 = new Connection { AuthenticationMethod = ConnectionMethod.ConnectionString, ConnectionString = "name=value" };
        await AzureBlobStorage.CreateContainer(input, connection1, new CancellationToken());
        await AzureBlobStorage.CreateContainer(input, connection2, new CancellationToken());
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestCreateContainer_throws_ClientNotFound()
    {
        var input = new Input { ContainerName = _containerName };
        var connection = new Connection { AuthenticationMethod = ConnectionMethod.ConnectionString, ConnectionString = "DefaultEndpointsProtocol=https;AccountName=unitTestStorage;AccountKey=abcdefghijklmnopqrstuyxz123456789;EndpointSuffix=core.windows.net" };
        await AzureBlobStorage.CreateContainer(input, connection, new CancellationToken());
    }

    [TestMethod]
    public async Task AccessTokenAuthenticationTest()
    {
        var containerName = $"test{Guid.NewGuid()}";

        input = new Input
        {
            ContainerName = containerName
        };

        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.OAuth2,
            StorageAccountName = _storageAccount,
            ApplicationId = _appID,
            TenantId = _tenantID,
            ClientSecret = _clientSecret
        };

        var result = await AzureBlobStorage.CreateContainer(input, connection, default);
        Assert.IsTrue(result.Success);
    }
}
