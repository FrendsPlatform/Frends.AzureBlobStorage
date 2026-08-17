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
    private string _containerName;
    private readonly string _storageAccount = "stataskdevelopment";

    private readonly Tag[] _tags = new[]
    {
        new Tag
        {
            Name = "TagName",
            Value = "TagValue"
        }
    };

    private readonly string _container = "const-test-container";
    private Input _input;
    private Options _options;
    private Connection _connection;
    private readonly string _testContent = "This is test data";

    [SetUp]
    public async Task TestSetup()
    {
        TestHelper.LoadEnvironmentVariables();
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

        await CreateBlobContainer(TestHelper.ConnectionString, _containerName);

        _input = new Input
        {
            SourceType = SourceType.String,
            ContentString = _testContent,
            ContentBytes = Encoding.UTF8.GetBytes(_testContent),
            Encoding = FileEncoding.UTF8,
            ContainerName = _containerName,
            CreateContainerIfItDoesNotExist = false,
            BlobName = $"testblob_{Guid.NewGuid()}",
            Tags = null,
            HandleExistingFile = HandleExistingFile.Overwrite,
            Compress = false
        };

        _connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = TestHelper.ConnectionString,
            TenantId = TestHelper.TenantId,
            ApplicationId = TestHelper.ClientId,
            StorageAccountName = _storageAccount,
            ClientSecret = TestHelper.ClientSecret,
        };

        _options = new Options()
        {
            ThrowErrorOnFailure = true
        };
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
        var result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);

        // OAuth
        _input.BlobName = $"testblob_{Guid.NewGuid()}";
        _connection.AuthenticationMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_input.ContainerName, _input.BlobName, _testContent));
    }

    [Test]
    public async Task WriteBlob_TestWriteFromByteArray()
    {
        _input.SourceType = SourceType.Bytes;

        // Connection string
        var result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);

        // OAuth
        _input.BlobName = $"testblob_{Guid.NewGuid()}";
        _connection.AuthenticationMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_input.ContainerName, _input.BlobName, _testContent));
    }

    [Test]
    public async Task WriteBlob_TestFolderBlobName()
    {
        // Connection string
        _input.BlobName = $"C:\\folder\\testBlob_{Guid.NewGuid()}";
        var result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);

        // OAuth
        _input.BlobName = $"C:\\folder\\testBlob_{Guid.NewGuid()}";
        _connection.AuthenticationMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_input.ContainerName, _input.BlobName, _testContent));
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

        _input.FileEncodingString = "windows-1251";

        foreach (var encoding in encodings)
        {
            _input.Encoding = encoding;

            // Connection string
            var result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success, $"Encoding: {encoding}");
            Assert.IsTrue(await BlobExists(_input.ContainerName, _input.BlobName, _testContent));

            // OAuth
            _input.BlobName = $"testblob_{Guid.NewGuid()}";
            _connection.AuthenticationMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success, $"Encoding: {encoding}");
            Assert.IsTrue(await BlobExists(_input.ContainerName, _input.BlobName, _testContent));
        }
    }

    [Test]
    public async Task WriteBlob_TestCreateContainer()
    {
        _input.CreateContainerIfItDoesNotExist = true;

        // Connection string
        _input.ContainerName =
            $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
        var result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);

        var blobServiceClient = new BlobServiceClient(_connection.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_input.ContainerName);
        Assert.IsTrue(containerClient.Exists());

        await DeleteBlobContainer(_input.ContainerName);

        // OAuth
        _connection.ConnectionString = null;
        _input.ContainerName =
            $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
        _input.BlobName = $"testblob_{Guid.NewGuid()}";
        _connection.AuthenticationMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_input.ContainerName, _input.BlobName, _testContent));

        containerClient = blobServiceClient.GetBlobContainerClient(_input.ContainerName);
        Assert.IsTrue(containerClient.Exists());

        await DeleteBlobContainer(_input.ContainerName);
    }

    [Test]
    public void WriteBlob_InvalidConnectionString_ShouldThrowException()
    {
        _connection.ConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=invalid;AccountKey=InvalidAccountKey;EndpointSuffix=core.windows.net"; // Simulate an invalid connection string

        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await AzureBlobStorage.WriteBlob(_input, _connection, _options, default));
        Assert.That(ex.Message.Contains("GetBlobServiceClient error:"), ex.Message);
    }

    [Test]
    public void WriteBlob_InvalidOAuth2_ShouldThrowException()
    {
        _connection.AuthenticationMethod = ConnectionMethod.OAuth2;
        _connection.ClientSecret = "InvalidClientSecret";

        var ex = Assert.ThrowsAsync<AuthenticationFailedException>(async () =>
            await AzureBlobStorage.WriteBlob(_input, _connection, _options, default));
        Assert.IsTrue(ex.Message.Contains("ClientSecretCredential authentication failed"));
    }

    [Test]
    public async Task WriteBlob_Tags()
    {
        _input.Tags = _tags;

        // Connection string
        var result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_input.ContainerName, _input.BlobName, _testContent));
    }

    [Test]
    public async Task WriteBlob_SasToken()
    {
        _connection.AuthenticationMethod = ConnectionMethod.SasToken;
        _connection.SasToken = TestHelper.SasToken;
        _input.ContainerName = _container;

        var result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(await BlobExists(_input.ContainerName, _input.BlobName, _testContent));
    }

    [Test]
    public async Task WriteBlob_Compress()
    {
        _input.Compress = true;
        var result = await AzureBlobStorage.WriteBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(
            $"https://stataskdevelopment.blob.core.windows.net/{_input.ContainerName}/{_input.BlobName}",
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
        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        await container.DeleteIfExistsAsync();
    }

    private async Task<bool> BlobExists(string containerName, string blobName, string expected)
    {
        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);

        if (!blob.Exists())
            return false;

        var blobClient = new BlobClient(TestHelper.ConnectionString, _input.ContainerName, _input.BlobName);
        var blobDownload = await blobClient.DownloadAsync();

        using var reader = new StreamReader(blobDownload.Value.Content);
        var content = await reader.ReadToEndAsync();

        return content == expected;
    }
}

