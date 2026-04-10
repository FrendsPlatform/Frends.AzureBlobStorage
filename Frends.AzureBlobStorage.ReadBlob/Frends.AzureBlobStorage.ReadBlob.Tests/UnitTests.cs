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
    Source source;
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
        source = new Source
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

        var result = AzureBlobStorage.ReadBlob(source, connection, options, default);
        Assert.IsNotEmpty(result.Result.Content);
    }

    [Test]
    public void ReadBlobConnectionString()
    {
        source = new Source
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

        var result = AzureBlobStorage.ReadBlob(source, connection, options, default);
        Assert.IsNotEmpty(result.Result.Content);
    }

    [Test]
    public void ReadBlobOAuth()
    {
        source = new Source
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

        var result = AzureBlobStorage.ReadBlob(source, connection, options, default);
        Assert.IsNotEmpty(result.Result.Content);
    }

    /// <summary>
    /// Error handling, missing SAS Token error.
    /// </summary>
    [Test]
    public void ReadBlobSasMissing()
    {
        source = new Source
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

        var ex = Assert.ThrowsAsync<ValidationException>(() =>
            AzureBlobStorage.ReadBlob(source, connection, options, default));
        Assert.That(ex.Message.Contains("SasToken is required."), ex.Message);
    }

    /// <summary>
    /// Error handling, missing connection string.
    /// </summary>
    [Test]
    public void ReadBlobConnectionStringMissing()
    {
        source = new Source
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

        var ex = Assert.ThrowsAsync<ValidationException>(() =>
            AzureBlobStorage.ReadBlob(source, connection, options, default));
        Assert.That(ex.Message.Contains("ConnectionString is required."), ex.Message);
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
