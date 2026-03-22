using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.ListContainers.Definitions;
using Frends.AzureBlobStorage.Toolkit.Definitions;
using NUnit.Framework;

namespace Frends.AzureBlobStorage.ListContainers.Tests;

[TestFixture]
public class ListContainersTests
{
    private readonly string connectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
    private readonly string appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private const string storageAccount = "stataskdevelopment";
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
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = connectionString,
            TenantId = tenantID,
            ApplicationId = appID,
            StorageAccountName = storageAccount,
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
