using NUnit.Framework;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.WriteBlob.Definitions;
using Frends.AzureBlobStorage.WriteBlob.Enums;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using Azure.Identity;

namespace Frends.AzureBlobStorage.WriteBlob.Tests;

[TestFixture]
public class UnitTests
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");

    private string _containerName;
    private readonly string _appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private readonly string _uri = "https://stataskdevelopment.blob.core.windows.net";
    private readonly string _sasToken = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_SASToken");
    private readonly Tag[] _tags = new[] { new Tag { Name = "TagName", Value = "TagValue" } };
    private readonly string _container = "const-test-container";
    private Destination _destination;
    private Source _source;
    private Options _options;
    private readonly string _testContent = "This is test data";

    [SetUp]
    public async Task TestSetup()
    {
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

        await CreateBlobContainer(_connectionString, _containerName);

        _source = new Source
        {
            SourceType = SourceType.String,
            ContentString = _testContent,
            ContentBytes = Encoding.UTF8.GetBytes(_testContent),
            Encoding = FileEncoding.UTF8
        };

        _destination = new Destination
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = false,
            BlobName = $"testblob_{Guid.NewGuid()}",
            Tags = null,
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = _tenantID,
            ApplicationID = _appID,
            Uri = _uri,
            ClientSecret = _clientSecret,
            Compress = false
        };

        _options = new Options() { ThrowErrorOnFailure = true };
    }

    [TearDown]
    public async Task CleanUp()
    {
        await DeleteBlobContainer(_containerName);
    }

    [Test]
    public async Task WriteBlob_TestWriteFromString()
    {
        // Connection string
        var result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);

        // OAuth
        _destination.BlobName = $"testblob_{Guid.NewGuid()}";
        _destination.ConnectionMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_destination.ContainerName, _destination.BlobName, _testContent));
    }

    [Test]
    public async Task WriteBlob_TestWriteFromByteArray()
    {
        _source.SourceType = SourceType.Bytes;

        // Connection string
        var result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);

        // OAuth
        _destination.BlobName = $"testblob_{Guid.NewGuid()}";
        _destination.ConnectionMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_destination.ContainerName, _destination.BlobName, _testContent));
    }

    [Test]
    public async Task WriteBlob_TestFolderBlobName()
    {
        // Connection string
        _destination.BlobName = $"C:\\folder\\testBlob_{Guid.NewGuid()}";
        var result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);

        // OAuth
        _destination.BlobName = $"C:\\folder\\testBlob_{Guid.NewGuid()}";
        _destination.ConnectionMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_destination.ContainerName, _destination.BlobName, _testContent));
    }

    [Test]
    public async Task WriteBlob_TestEncoding()
    {
        var encodings = new List<FileEncoding>()
        {
            FileEncoding.UTF8,
            FileEncoding.Default,
            FileEncoding.ASCII,
            FileEncoding.WINDOWS1252,
            FileEncoding.Other
        };

        _source.FileEncodingString = "windows-1251";

        foreach (var encoding in encodings)
        {
            _source.Encoding = encoding;

            // Connection string
            var result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success, $"Encoding: {encoding}");
            Assert.IsTrue(await BlobExists(_destination.ContainerName, _destination.BlobName, _testContent));

            // OAuth
            _destination.BlobName = $"testblob_{Guid.NewGuid()}";
            _destination.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success, $"Encoding: {encoding}");
            Assert.IsTrue(await BlobExists(_destination.ContainerName, _destination.BlobName, _testContent));
        }
    }

    [Test]
    public async Task WriteBlob_TestCreateContainer()
    {
        _destination.CreateContainerIfItDoesNotExist = true;

        // Connection string
        _destination.ContainerName =
            $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
        var result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);

        var blobServiceClient = new BlobServiceClient(_destination.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_destination.ContainerName);
        Assert.IsTrue(containerClient.Exists());

        await DeleteBlobContainer(_destination.ContainerName);

        // OAuth
        _destination.ConnectionString = null;
        _destination.ContainerName =
            $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
        _destination.BlobName = $"testblob_{Guid.NewGuid()}";
        _destination.ConnectionMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_destination.ContainerName, _destination.BlobName, _testContent));

        containerClient = blobServiceClient.GetBlobContainerClient(_destination.ContainerName);
        Assert.IsTrue(containerClient.Exists());

        await DeleteBlobContainer(_destination.ContainerName);
    }

    [Test]
    public void WriteBlob_InvalidConnectionString_ShouldThrowException()
    {
        _destination.ConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=invalid;AccountKey=InvalidAccountKey;EndpointSuffix=core.windows.net"; // Simulate an invalid connection string

        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await AzureBlobStorage.WriteBlob(_source, _destination, _options, default));
        Assert.That(ex.Message.StartsWith("GetBlobContainerClient error:"), ex.Message);
    }

    [Test]
    public void WriteBlob_InvalidOAuth2_ShouldThrowException()
    {
        _destination.ConnectionMethod = ConnectionMethod.OAuth2;
        _destination.ClientSecret = "InvalidClientSecret";

        var ex = Assert.ThrowsAsync<AuthenticationFailedException>(async () =>
            await AzureBlobStorage.WriteBlob(_source, _destination, _options, default));
        Assert.IsTrue(ex.Message.Contains("ClientSecretCredential authentication failed"));
    }

    [Test]
    public async Task WriteBlob_Tags()
    {
        _destination.Tags = _tags;

        // Connection string
        var result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_destination.ContainerName, _destination.BlobName, _testContent));
    }

    [Test]
    public async Task WriteBlob_SasToken()
    {
        _destination.ConnectionMethod = ConnectionMethod.SASToken;
        _destination.SASToken = _sasToken;
        _destination.ContainerName = _container;

        var result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_destination.ContainerName, _destination.BlobName, _testContent));
    }

    [Test]
    public async Task WriteBlob_Compress()
    {
        _destination.Compress = true;
        var result = await AzureBlobStorage.WriteBlob(_source, _destination, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"https://stataskdevelopment.blob.core.windows.net/{_destination.ContainerName}/{_destination.BlobName}",
            result.Uri);
    }

    private async static Task CreateBlobContainer(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null);
    }

    private async Task DeleteBlobContainer(string containerName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        await container.DeleteIfExistsAsync();
    }

    private async Task<bool> BlobExists(string containerName, string blobName, string expected)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);

        if (!blob.Exists())
            return false;

        var blobClient = new BlobClient(_connectionString, _destination.ContainerName, _destination.BlobName);
        var blobDownload = await blobClient.DownloadAsync();

        using var reader = new StreamReader(blobDownload.Value.Content);
        var content = await reader.ReadToEndAsync();

        return content == expected;
    }
}
