using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DeleteContainer.Definitions;
using Azure.Storage.Blobs.Models;
using Frends.AzureBlobStorage.Toolkit.Definitions;

namespace Frends.AzureBlobStorage.DeleteContainer.Tests;

[TestClass]
public class UnitTests
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");

    private readonly string _appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");

    private readonly string _storageAccount =
        Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_StorageAccount");

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

    private BlobContainerClient GetBlobServiceClient()
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);

        return blobServiceClient.GetBlobContainerClient(_containerName);
    }

    [TestMethod]
    public async Task TestDeleteContainer()
    {
        // Test method returns true when container doesn't exist
        var result = await AzureBlobStorage.DeleteContainer(
            new Input
            {
                ContainerName = _containerName
            },
            new Connection
            {
                ConnectionString = _connectionString,
            },
            new Options
            {
                ThrowErrorIfContainerDoesNotExists = false
            }, new CancellationToken());

        Assert.IsFalse(result.ContainerWasDeleted);

        // Test method returns true when container that exists is deleted
        var container = GetBlobServiceClient();
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, new CancellationToken());
        var deleted = await AzureBlobStorage.DeleteContainer(new Input
            {
                ContainerName = _containerName,
            },
            new Connection
            {
                ConnectionString = _connectionString,
            },
            new Options
            {
                ThrowErrorIfContainerDoesNotExists = true
            }, new CancellationToken());
        Assert.IsTrue(deleted.ContainerWasDeleted);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_throws_ContainerNotFound()
    {
        await AzureBlobStorage.DeleteContainer(new Input
            {
                ContainerName = _containerName
            },
            new Connection
            {
                ConnectionString = _connectionString,
            },
            new Options
            {
                ThrowErrorIfContainerDoesNotExists = true
            }, new CancellationToken());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task TestDeleteContainer_throws_ParameterEmpty()
    {
        await AzureBlobStorage.DeleteContainer(new Input
            {
                ContainerName = null
            },
            new Connection
            {
                ConnectionString = _connectionString,
            },
            new Options
            {
                ThrowErrorIfContainerDoesNotExists = false
            }, new CancellationToken());
        await AzureBlobStorage.DeleteContainer(new Input
            {
                ContainerName = _containerName
            },
            new Connection
            {
                ConnectionString = _connectionString,
            },
            new Options
            {
                ThrowErrorIfContainerDoesNotExists = false
            }, new CancellationToken());
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_throws_ParameterNotValid()
    {
        await AzureBlobStorage.DeleteContainer(new Input
            {
                ContainerName = "Valid name"
            },
            new Connection
            {
                ConnectionString = "Not valid parameter",
            },
            new Options
            {
                ThrowErrorIfContainerDoesNotExists = false
            }, new CancellationToken());
        await AzureBlobStorage.DeleteContainer(new Input
            {
                ContainerName = "Valid name"
            }, new Connection
            {
                ConnectionString = "name=value",
            },
            new Options
            {
                ThrowErrorIfContainerDoesNotExists = false
            }, new CancellationToken());
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_throws_ClientNotFound()
    {
        await AzureBlobStorage.DeleteContainer(new Input
            {
                ContainerName = _containerName
            },
            new Connection
            {
                ConnectionString =
                    "DefaultEndpointsProtocol=https;AccountName=unitTestStorage;AccountKey=abcdefghijklmnopqrstuyxz123456789;EndpointSuffix=core.windows.net",
            },
            new Options
            {
                ThrowErrorIfContainerDoesNotExists = false
            }, new CancellationToken());
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
            StorageAccountName = _storageAccount,
            ApplicationId = _appID,
            TenantId = _tenantID,
            ClientSecret = _clientSecret
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
