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
    private const string StorageAccount = "stataskdevelopment";
    private string _containerName;

    [TestInitialize]
    public void TestSetup()
    {
        TestHelper.LoadEnvironmentVariables();

        // Generate unique container name to avoid conflicts when running multiple tests
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        // delete whole container after running tests
        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);
        var container = blobServiceClient.GetBlobContainerClient(_containerName);
        await container.DeleteIfExistsAsync();
    }

    [TestMethod]
    public async Task TestCreateContainer()
    {
        var input = new Input
        {
            ContainerName = _containerName
        };
        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = TestHelper.ConnectionString
        };
        var options = new Options
        {
            ThrowErrorOnFailure = true
        };
        var result = await AzureBlobStorage.CreateContainer(input, connection, options, CancellationToken.None);
        Assert.IsNotNull(result);
        Assert.AreEqual(new BlobClient(TestHelper.ConnectionString, _containerName, "").Uri.ToString(), result.Uri);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [DataTestMethod]
    [DataRow("Not valid parameter")]
    [DataRow("name=value")]
    [ExpectedException(typeof(Exception))]
    public async Task TestCreateContainer_throws_ParameterNotValid(string conString)
    {
        var input = new Input
        {
            ContainerName = "valid"
        };
        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = conString
        };
        var options = new Options
        {
            ThrowErrorOnFailure = true
        };
        await AzureBlobStorage.CreateContainer(input, connection, options, CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestCreateContainer_throws_ClientNotFound()
    {
        var input = new Input
        {
            ContainerName = _containerName
        };
        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString =
                "DefaultEndpointsProtocol=https;AccountName=unitTestStorage;AccountKey=abcdefghijklmnopqrstuyxz123456789;EndpointSuffix=core.windows.net"
        };
        var options = new Options
        {
            ThrowErrorOnFailure = true
        };
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
            ApplicationId = TestHelper.ClientId,
            TenantId = TestHelper.TenantId,
            ClientSecret = TestHelper.ClientSecret,
        };

        var options = new Options
        {
            ThrowErrorOnFailure = true
        };
        var result = await AzureBlobStorage.CreateContainer(_input, _connection, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);

        // Cleanup the container created for this test
        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        await container.DeleteIfExistsAsync();
    }

    [TestMethod]
    public async Task TestCreateContainer_ThrowErrorOnFailure_False()
    {
        var input = new Input
        {
            ContainerName = "Valid name"
        };
        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = "Not valid parameter"
        };
        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "Custom error message"
        };

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
        var input = new Input
        {
            ContainerName = "Valid name"
        };
        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = "Not valid parameter"
        };
        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "Custom error message"
        };

        await AzureBlobStorage.CreateContainer(input, connection, options, CancellationToken.None);
    }
}
