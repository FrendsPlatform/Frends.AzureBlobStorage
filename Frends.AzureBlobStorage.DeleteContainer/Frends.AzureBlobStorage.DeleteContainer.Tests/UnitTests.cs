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
    public async Task TestDeleteContainer_ContainerNotFound_ReturnsSuccessFalse()
    {
        var result = await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = _containerName },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { FailOnContainerNotFound = false, ThrowErrorOnFailure = true },
            new CancellationToken());

        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task TestDeleteContainer_DeletesExistingContainer_ReturnsSuccessTrue()
    {
        var container = GetBlobServiceClient();
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, new CancellationToken());

        var result = await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = _containerName },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { FailOnContainerNotFound = true, ThrowErrorOnFailure = true },
            new CancellationToken());

        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_FailOnContainerNotFound_Throws()
    {
        await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = _containerName },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { FailOnContainerNotFound = true, ThrowErrorOnFailure = true },
            new CancellationToken());
    }

    [TestMethod]
    public async Task TestDeleteContainer_ThrowErrorOnFailureFalse_ReturnsErrorResult()
    {
        var result = await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = _containerName },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { FailOnContainerNotFound = true, ThrowErrorOnFailure = false },
            new CancellationToken());

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsFalse(string.IsNullOrEmpty(result.Error.Message));
    }

    [TestMethod]
    public async Task TestDeleteContainer_ErrorMessageOnFailure_IncludedInErrorMessage()
    {
        const string customMessage = "Custom error message";

        var result = await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = _containerName },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { FailOnContainerNotFound = true, ThrowErrorOnFailure = false, ErrorMessageOnFailure = customMessage },
            new CancellationToken());

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        StringAssert.Contains(result.Error.Message, customMessage);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_ErrorMessageOnFailure_IncludedInException()
    {
        const string customMessage = "Custom error message";

        try
        {
            await AzureBlobStorage.DeleteContainer(
                new Input { ContainerName = _containerName },
                new Connection { ConnectionString = TestHelper.ConnectionString },
                new Options { FailOnContainerNotFound = true, ThrowErrorOnFailure = true, ErrorMessageOnFailure = customMessage },
                new CancellationToken());
        }
        catch (Exception ex)
        {
            StringAssert.Contains(ex.Message, customMessage);
            throw;
        }
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task TestDeleteContainer_throws_ParameterEmpty()
    {
        await AzureBlobStorage.DeleteContainer(
            new Input { ContainerName = null },
            new Connection { ConnectionString = TestHelper.ConnectionString },
            new Options { FailOnContainerNotFound = false, ThrowErrorOnFailure = true },
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
            new Options { FailOnContainerNotFound = false, ThrowErrorOnFailure = true },
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
            new Options { FailOnContainerNotFound = false, ThrowErrorOnFailure = true }, new CancellationToken());
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
            FailOnContainerNotFound = false,
            ThrowErrorOnFailure = true
        };

        var client = GetBlobServiceClient();
        await client.CreateIfNotExistsAsync(PublicAccessType.None, null, null, new CancellationToken());
        var result = await AzureBlobStorage.DeleteContainer(input, connection, options, default);
        Assert.IsTrue(result.Success);
    }
}
