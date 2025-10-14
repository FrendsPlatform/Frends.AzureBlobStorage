using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.ListContainers.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Frends.AzureBlobStorage.ListContainers.Tests;

[TestFixture]
public class ListContainersTests
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
    private readonly string _appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private readonly string _uri = "https://stataskdevelopment.blob.core.windows.net";
    private readonly string _sasToken = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_SASToken");

    private Connection _connection;
    private Options _options;
    private string _testContainerName;

    [SetUp]
    public async Task Setup()
    {
        _testContainerName = $"testcontainer{DateTime.Now.ToString("mmssffff", CultureInfo.InvariantCulture)}";

        _connection = new Connection
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ConnectionString = _connectionString,
            TenantId = _tenantID,
            ApplicationId = _appID,
            Uri = _uri,
            SasToken = _sasToken,
            ClientSecret = _clientSecret,
        };

        _options = new Options
        {
            ThrowErrorOnFailure = true,
        };

        var blobServiceClient = new BlobServiceClient(_connectionString);
        await blobServiceClient.CreateBlobContainerAsync(_testContainerName);
    }

    [TearDown]
    public async Task Cleanup()
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        await blobServiceClient.DeleteBlobContainerAsync(_testContainerName);
    }

    [Test]
    public async Task ListContainers_ShouldReturnContainers_WhenUsingConnectionString()
    {
        var result = await AzureBlobStorage.ListContainers(_connection, _options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers, Is.Not.Null);
        Assert.That(
            result.Containers.Exists(c => c.Name == _testContainerName),
            Is.True);
    }

    [Test]
    public async Task ListContainers_ShouldWork_WithAllConnectionMethods()
    {
        // Connection String
        _connection.ConnectionMethod = ConnectionMethod.ConnectionString;
        var result = await AzureBlobStorage.ListContainers(_connection, _options, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers.Count > 0, Is.True);

        // SAS Token
        _connection.ConnectionMethod = ConnectionMethod.SasToken;
        result = await AzureBlobStorage.ListContainers(_connection, _options, CancellationToken.None);
        Assert.That(result.Success, Is.True)
        Assert.That(result.Containers.Count > 0, Is.True);

        // OAuth2
        _connection.ConnectionMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.ListContainers(_connection, _options, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers.Count > 0, Is.True);
    }

    [Test]
    public async Task ListContainers_ShouldFail_WithInvalidConnectionString()
    {
        _connection.ConnectionMethod = ConnectionMethod.ConnectionString;
        _connection.ConnectionString = "InvalidConnectionString";

        _options.ThrowErrorOnFailure = false;
        var result = await AzureBlobStorage.ListContainers(_connection, _options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        StringAssert.Contains("An exception occurred", result.Error.Message);

    }

    [Test]
    public void ListContainers_ShouldThrow_WithMissingSasToken()
    {
        _connection.ConnectionMethod = ConnectionMethod.SasToken;
        _connection.SasToken = string.Empty;

        Assert.ThrowsAsync<Exception>(
            async () =>
        await AzureBlobStorage.ListContainers(_connection, _options, CancellationToken.None),
            "Expected an exception when SAS token is missing.");
    }

    [Test]
    public async Task ListContainers_ShouldReturnEmptyList_WhenNoContainersExist()
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        await blobServiceClient.DeleteBlobContainerAsync(_testContainerName);

        _options.ThrowErrorOnFailure = false;
        var result = await AzureBlobStorage.ListContainers(_connection, _options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Containers, Is.Empty);

    }
}
