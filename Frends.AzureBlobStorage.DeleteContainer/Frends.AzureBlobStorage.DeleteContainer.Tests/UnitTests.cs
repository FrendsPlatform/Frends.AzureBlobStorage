using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DeleteContainer.Definitions;
using Azure.Storage.Blobs.Models;

namespace Frends.AzureBlobStorage.DeleteContainer.Tests;

[TestClass]
public class UnitTests
{
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
        var container = GetBlobServiceClient();
        await container.DeleteIfExistsAsync();
    }

    private BlobContainerClient GetBlobServiceClient()
    {
        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);

        return blobServiceClient.GetBlobContainerClient(_containerName);
    }

    [TestMethod]
    public async Task TestDeleteContainer()
    {
        // Test method returns true when container doesn't exist
        var result = await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = _containerName },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { ThrowErrorIfContainerDoesNotExists = false },
            new CancellationToken());

        Assert.IsFalse(result.ContainerWasDeleted);

        // Test method returns true when container that exists is deleted
        var container = GetBlobServiceClient();
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, new CancellationToken());
        var deleted = await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = _containerName },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { ThrowErrorIfContainerDoesNotExists = true },
            new CancellationToken());
        Assert.IsTrue(deleted.ContainerWasDeleted);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_throws_ContainerNotFound()
    {
        await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = _containerName },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { ThrowErrorIfContainerDoesNotExists = true },
            new CancellationToken());
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_throws_ParameterEmpty()
    {
        await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = null },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { ThrowErrorIfContainerDoesNotExists = false },
            new CancellationToken());
    }

    [DataTestMethod]
    [DataRow("Not valid parameter")]
    [DataRow("name=value")]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_throws_ParameterNotValid(string conString)
    {
        await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = "valid" },
            new Connection { ConnectionString = conString, },
            new Options { ThrowErrorIfContainerDoesNotExists = false },
            new CancellationToken());
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_throws_ClientNotFound()
    {
        await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = _containerName },
            new Connection
            {
                ConnectionString =
                    "DefaultEndpointsProtocol=https;AccountName=unitTestStorage;AccountKey=abcdefghijklmnopqrstuyxz123456789;EndpointSuffix=core.windows.net",
            },
            new Options { ThrowErrorIfContainerDoesNotExists = false }, new CancellationToken());
    }

    [TestMethod]
    public async Task DeleteContainerAsync_AccessTokenAuthenticationTest()
    {
        var input = new Input
        {
            ContainerName = _containerName,
        };

        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.OAuth2,
            StorageAccountName = TestHelper.StorageAccountName,
            ApplicationId = TestHelper.ClientId,
            TenantId = TestHelper.TenantId,
            ClientSecret = TestHelper.ClientSecret
        };

        var options = new Options
        {
            ThrowErrorIfContainerDoesNotExists = false
        };

        var client = GetBlobServiceClient();
        await client.CreateIfNotExistsAsync(PublicAccessType.None, null, null, new CancellationToken());
        var result = await AzureBlobStorage.DeleteContainer(input, connection, options, default);
        Assert.IsTrue(result.ContainerWasDeleted);
    }
}
