using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Frends.AzureBlobStorage.ReadBlob.Definitions;
using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ReadBlob.Tests;

[TestFixture]
public class ReadTest
{
    Input input;
    Options options;
    Connection connection;

    private string _containerName;
    private readonly string _blobName = "test.txt";

    private readonly string _testFilePath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestFiles", "TestFile.xml");

    [SetUp]
    public async Task SetUp()
    {
        TestHelper.LoadEnvironmentVariables();
        // Generate unique container name to avoid conflicts when running multiple tests
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);
        var container = blobServiceClient.GetBlobContainerClient(_containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null);
        var blockBlob = container.GetBlobClient(_blobName);
        await blockBlob.UploadAsync(_testFilePath, default);
    }

    [TearDown]
    public async Task TearDown()
    {
        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);
        var container = blobServiceClient.GetBlobContainerClient(_containerName);
        await container.DeleteIfExistsAsync(null);
    }

    [Test]
    public void ReadBlobSAS()
    {
        input = new Input
        {
            ContainerName = _containerName,
            BlobName = _blobName
        };

        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.SasToken,
            StorageAccountName = TestHelper.StorageAccountName,
            SasToken = GetServiceSasUriForBlob(),
        };

        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var result = AzureBlobStorage.ReadBlob(input, connection, options, default);
        Assert.That(result.Result.Success, Is.True);
        Assert.IsNotEmpty(result.Result.Content);
        Assert.IsNull(result.Result.Error);
    }

    [Test]
    public void ReadBlobConnectionString()
    {
        input = new Input
        {
            ContainerName = _containerName,
            BlobName = _blobName,
        };

        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = TestHelper.ConnectionString,
        };

        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var result = AzureBlobStorage.ReadBlob(input, connection, options, default);
        Assert.That(result.Result.Success, Is.True);
        Assert.IsNotEmpty(result.Result.Content);
        Assert.IsNull(result.Result.Error);
    }

    [Test]
    public void ReadBlobOAuth()
    {
        input = new Input
        {
            ContainerName = _containerName,
            BlobName = _blobName,
        };

        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.OAuth2,
            ConnectionString = TestHelper.ConnectionString,
            ApplicationId = TestHelper.ClientId,
            StorageAccountName = TestHelper.StorageAccountName,
            ClientSecret = TestHelper.ClientSecret,
            TenantId = TestHelper.TenantId,
        };

        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var result = AzureBlobStorage.ReadBlob(input, connection, options, default);
        Assert.That(result.Result.Success, Is.True);
        Assert.IsNotEmpty(result.Result.Content);
        Assert.IsNull(result.Result.Error);
    }

    /// <summary>
    /// Error handling, missing SAS Token error.
    /// </summary>
    [Test]
    public void ReadBlobSasMissing()
    {
        input = new Input
        {
            ContainerName = _containerName,
            BlobName = _blobName
        };

        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.SasToken,
            StorageAccountName = TestHelper.StorageAccountName,
            SasToken = string.Empty,
        };

        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            AzureBlobStorage.ReadBlob(input, connection, options, default));
        Assert.That(ex.Message.Contains("SasToken is required."), ex.Message);
    }

    /// <summary>
    /// Error handling, missing connection string.
    /// </summary>
    [Test]
    public void ReadBlobConnectionStringMissing()
    {
        input = new Input
        {
            ContainerName = _containerName,
            BlobName = _blobName
        };
        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = string.Empty,
        };
        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            AzureBlobStorage.ReadBlob(input, connection, options, default));
        Assert.That(ex.Message.Contains("ConnectionString is required."), ex.Message);
    }

    /// <summary>
    /// ThrowErrorOnFailure = false returns error in result instead of throwing.
    /// </summary>
    [Test]
    public async Task ReadBlob_ThrowErrorOnFailure_False_ReturnsErrorInResult()
    {
        input = new Input
        {
            ContainerName = _containerName,
            BlobName = _blobName
        };
        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = string.Empty,
        };
        options = new Options
        {
            Encoding = Encode.ASCII,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = string.Empty,
        };

        var result = await AzureBlobStorage.ReadBlob(input, connection, options, default);
        Assert.That(result.Success, Is.False);
        Assert.IsNotNull(result.Error);
        Assert.IsNotEmpty(result.Error.Message);
        Assert.IsNotNull(result.Error.AdditionalInfo);
    }

    /// <summary>
    /// ThrowErrorOnFailure = false with custom error message prefix uses that prefix.
    /// </summary>
    [Test]
    public async Task ReadBlob_ThrowErrorOnFailure_False_CustomMessage_ReturnsErrorWithPrefix()
    {
        input = new Input
        {
            ContainerName = _containerName,
            BlobName = _blobName
        };
        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = string.Empty,
        };
        options = new Options
        {
            Encoding = Encode.ASCII,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "Custom error prefix",
        };

        var result = await AzureBlobStorage.ReadBlob(input, connection, options, default);
        Assert.That(result.Success, Is.False);
        Assert.IsNotNull(result.Error);
        Assert.That(result.Error.Message, Does.StartWith("Custom error prefix"));
    }

    /// <summary>
    /// ThrowErrorOnFailure = true with custom error message throws exception with that message.
    /// </summary>
    [Test]
    public void ReadBlob_ThrowErrorOnFailure_True_CustomMessage_ThrowsWithCustomMessage()
    {
        input = new Input
        {
            ContainerName = _containerName,
            BlobName = _blobName
        };
        connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = string.Empty,
        };
        options = new Options
        {
            Encoding = Encode.ASCII,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "My custom error message",
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            AzureBlobStorage.ReadBlob(input, connection, options, default));
        Assert.That(ex.Message, Is.EqualTo("My custom error message"));
    }

    /// <summary>
    /// Generate SAS Token for testfile. Token last for 10 minutes.
    /// </summary>
    private string GetServiceSasUriForBlob()
    {
        BlobSasBuilder blobSasBuilder = new()
        {
            BlobContainerName = _containerName,
            BlobName = _blobName,
            ExpiresOn = DateTime.UtcNow.AddMinutes(5)
        };
        blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
        var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(TestHelper.StorageAccountName, TestHelper.AccessKey))
            .ToString();

        return sasToken;
    }
}
