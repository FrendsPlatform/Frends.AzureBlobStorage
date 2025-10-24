using System;
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
    private readonly string connectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
    private readonly string appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private readonly string uri = "https://stataskdevelopment.blob.core.windows.net";
    private readonly string sasToken = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_SASToken");

    private Connection connection;
    private Options options;
    private Input input;
    private string testContainerName;

    [SetUp]
    public async Task Setup()
    {
        testContainerName = $"testcontainer{DateTime.Now.ToString("mmssffff", CultureInfo.InvariantCulture)}";

        connection = new Connection
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ConnectionString = connectionString,
            TenantId = tenantID,
            ApplicationId = appID,
            Uri = uri,
            SasToken = sasToken,
            ClientSecret = clientSecret,
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

        var blobServiceClient = new BlobServiceClient(connectionString);
        await blobServiceClient.CreateBlobContainerAsync(testContainerName);
    }

    [TearDown]
    public async Task Cleanup()
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
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
        connection.ConnectionMethod = ConnectionMethod.ConnectionString;
        var result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers.Count > 0, Is.True);

        // SAS Token
        connection.ConnectionMethod = ConnectionMethod.SasToken;
        result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers.Count > 0, Is.True);

        // OAuth2
        connection.ConnectionMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers.Count > 0, Is.True);
    }

    [Test]
    public async Task ListContainers_ShouldFail_WithInvalidConnectionString()
    {
        connection.ConnectionMethod = ConnectionMethod.ConnectionString;
        connection.ConnectionString = "InvalidConnectionString";

        options.ThrowErrorOnFailure = false;
        var result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void ListContainers_ShouldThrow_WithMissingSasToken()
    {
        connection.ConnectionMethod = ConnectionMethod.SasToken;
        connection.SasToken = string.Empty;

        Assert.ThrowsAsync<Exception>(
            async () =>
        await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None),
            "Expected an exception when SAS token is missing.");
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
    public async Task ListContainers_ShouldReturnOnlySystemContainers_WhenSystemStateIsUsed()
    {
        input.Prefix = null;
        input.States = ContainerStateFilter.System;

        var result = await AzureBlobStorage.ListContainers(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers, Is.Not.Null);
        Assert.That(result.Containers.Count > 0);
        Assert.That(
            result.Containers.Any(c => c.Name.StartsWith('$')),
            "Expected at least one system container.");
    }
}
