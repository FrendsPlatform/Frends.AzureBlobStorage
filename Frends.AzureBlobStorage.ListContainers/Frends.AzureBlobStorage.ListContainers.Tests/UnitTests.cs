using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.ListContainers.Definitions;
using NUnit.Framework;

namespace Frends.AzureBlobStorage.ListContainers.Tests;

[TestFixture]
public class ListContainersTests
{
    private const string StorageAccount = "stataskdevelopment";

    private Connection connection;
    private Options options;
    private Input input;
    private string testContainerName;

    [SetUp]
    public async Task Setup()
    {
        TestHelper.LoadEnvironmentVariables();
        testContainerName = $"testcontainer{DateTime.Now.ToString("mmssffff", CultureInfo.InvariantCulture)}";

        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = TestHelper.ConnectionString,
            TenantId = TestHelper.TenantId,
            ApplicationId = TestHelper.ClientId,
            StorageAccountName = StorageAccount,
            SasToken = TestHelper.SasToken,
            ClientSecret = TestHelper.ClientSecret,
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
        };

        input = new Input
        {
            Prefix = null,
            States = ContainerStateFilter.None,
        };

        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);
        await blobServiceClient.CreateBlobContainerAsync(testContainerName);
    }

    [TearDown]
    public async Task Cleanup()
    {
        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);
        await blobServiceClient.DeleteBlobContainerAsync(testContainerName);
    }

    [Test]
    public async Task ListContainers_ShouldReturnContainers_WhenUsingConnectionString()
    {
        var result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers, Is.Not.Null);
        Assert.That(
            result.Containers.Exists(c => c.Name == testContainerName),
            Is.True);
    }

    [Test]
    public async Task ListContainers_ShouldWork_WithAllConnectionMethods()
    {
        // Connection String
        connection.AuthenticationMethod = ConnectionMethod.ConnectionString;
        var result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers.Count > 0, Is.True);

        // SAS Token
        connection.AuthenticationMethod = ConnectionMethod.SasToken;
        result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers.Count > 0, Is.True);

        // OAuth2
        connection.AuthenticationMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers.Count > 0, Is.True);
    }

    [Test]
    public async Task ListContainers_ShouldFail_WithInvalidConnectionString()
    {
        connection.AuthenticationMethod = ConnectionMethod.ConnectionString;
        connection.ConnectionString = "InvalidConnectionString";

        options.ThrowErrorOnFailure = false;
        var result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void ListContainers_ShouldThrow_WithMissingSasToken()
    {
        connection.AuthenticationMethod = ConnectionMethod.SasToken;
        connection.SasToken = string.Empty;

        Assert.ThrowsAsync<ValidationException>(
            async () =>
        await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None),
            "SasToken is required.");
    }

    [Test]
    public async Task ListContainers_ShouldFilterByPrefix()
    {
        input.Prefix = testContainerName[..6];

        var result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers.All(c => c.Name.StartsWith(input.Prefix)), Is.True);
    }

    [Test]
    [TestCase(ContainerStateFilter.System)]
    [TestCase(ContainerStateFilter.Deleted)]
    public async Task ListContainers_ShouldWork_ForDifferentStates(ContainerStateFilter state)
    {
        var input = new Input
        {
            States = state,
            Prefix = null,
        };

        var result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers, Is.Not.Null);
        Assert.That(result.Containers.Count, Is.GreaterThan(0));
    }
}
