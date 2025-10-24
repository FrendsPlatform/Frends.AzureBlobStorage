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
    private Input _input;
    private Connection _connection;

    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");

    private readonly string _appId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string _tenantId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private const string StorageAccount = "stataskdevelopment";
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
        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = _connectionString
        };
        var options = new Options { ThrowErrorOnFailure = true };
        var result = await AzureBlobStorage.CreateContainer(input, connection, options, CancellationToken.None);
        Assert.IsNotNull(result);
        Assert.AreEqual(new BlobClient(_connectionString, _containerName, "").Uri.ToString(), result.Uri);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestCreateContainer_throws_ParameterNotValid()
    {
        var input = new Input { ContainerName = "Valid name" };
        var connection1 = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = "Not valid parameter"
        };
        var connection2 = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = "name=value"
        };
        var options = new Options { ThrowErrorOnFailure = true };
        await AzureBlobStorage.CreateContainer(input, connection1, options, CancellationToken.None);
        await AzureBlobStorage.CreateContainer(input, connection2, options, CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestCreateContainer_throws_ClientNotFound()
    {
        var input = new Input { ContainerName = _containerName };
        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString =
                "DefaultEndpointsProtocol=https;AccountName=unitTestStorage;AccountKey=abcdefghijklmnopqrstuyxz123456789;EndpointSuffix=core.windows.net"
        };
        var options = new Options { ThrowErrorOnFailure = true };
        await AzureBlobStorage.CreateContainer(input, connection, options, CancellationToken.None);
    }

    [TestMethod]
    public async Task AccessTokenAuthenticationTest()
    {
        var containerName = $"test{Guid.NewGuid()}";

        _input = new Input
        {
            ContainerName = containerName
        };

        _connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.OAuth2,
            StorageAccountName = StorageAccount,
            ApplicationId = _appId,
            TenantId = _tenantId,
            ClientSecret = _clientSecret
        };

        var options = new Options { ThrowErrorOnFailure = true };
        var result = await AzureBlobStorage.CreateContainer(_input, _connection, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);

        // Cleanup the container created for this test
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        await container.DeleteIfExistsAsync();
    }

    [TestMethod]
    public async Task TestCreateContainer_ThrowErrorOnFailure_False()
    {
        var input = new Input { ContainerName = "Valid name" };
        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = "Not valid parameter"
        };
        var options = new Options { ThrowErrorOnFailure = false, ErrorMessageOnFailure = "Custom error message" };

        var result = await AzureBlobStorage.CreateContainer(input, connection, options, CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.AreEqual(string.Empty, result.Uri);
        Assert.IsNotNull(result.Error);
        StringAssert.Contains(result.Error.Message, "Custom error message");
        Assert.IsNotNull(result.Error.AdditionalInfo);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestCreateContainer_ThrowErrorOnFailure_True()
    {
        var input = new Input { ContainerName = "Valid name" };
        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = "Not valid parameter"
        };
        var options = new Options { ThrowErrorOnFailure = true, ErrorMessageOnFailure = "Custom error message" };

        await AzureBlobStorage.CreateContainer(input, connection, options, CancellationToken.None);
    }
}