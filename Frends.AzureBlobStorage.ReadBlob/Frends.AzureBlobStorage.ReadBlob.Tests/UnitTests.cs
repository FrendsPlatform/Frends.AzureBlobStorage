using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Frends.AzureBlobStorage.ReadBlob.Definitions;
using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ReadBlob.Tests;

[TestFixture]
public class ReadTest
{
    Source source;
    Options options;

    private readonly string _connectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
    private readonly string _accessKey = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_frendstaskstestcontainerAccessKey");
    private readonly string _appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private readonly string _storageaccount = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_StorageAccount");
    private string _containerName;
    private readonly string _blobName = "test.txt";
    private readonly string _testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestFiles", "TestFile.xml");

    [SetUp]
    public async Task SetUp()
    {
        // Generate unique container name to avoid conflicts when running multiple tests
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

        var blobServiceClient = new BlobServiceClient(_connectionString);
        var container = blobServiceClient.GetBlobContainerClient(_containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null);
        var blockBlob = container.GetBlobClient(_blobName);
        await blockBlob.UploadAsync(_testFilePath, default);
    }

    [TearDown]
    public async Task TearDown()
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var container = blobServiceClient.GetBlobContainerClient(_containerName);
        await container.DeleteIfExistsAsync(null);
    }

    [Test]
    public void ReadBlobSAS()
    {
        source = new Source
        {
            AuthenticationMethod = AuthenticationMethod.SASToken,
            URI = $"https://{_storageaccount}.blob.core.windows.net/{_containerName}/{_blobName}?",
            SASToken = GetServiceSasUriForBlob(),
            ContainerName = _containerName,
            BlobName = _blobName
        };

        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var result = AzureBlobStorage.ReadBlob(source, options, default);
        Assert.IsNotEmpty(result.Result.Content);
    }

    [Test]
    public void ReadBlobConnectionString()
    {
        source = new Source
        {
            AuthenticationMethod = AuthenticationMethod.ConnectionString,
            ConnectionString = _connectionString,
            ContainerName = _containerName,
            BlobName = _blobName
        };

        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var result = AzureBlobStorage.ReadBlob(source, options, default);
        Assert.IsNotEmpty(result.Result.Content);
    }

    [Test]
    public void ReadBlobOAuth()
    {
        source = new Source
        {
            AuthenticationMethod = AuthenticationMethod.OAuth2,
            ConnectionString = _connectionString,
            ContainerName = _containerName,
            BlobName = _blobName,
            ApplicationID = _appID,
            StorageAccountName = _storageaccount,
            ClientSecret = _clientSecret,
            TenantID = _tenantID,
        };

        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var result = AzureBlobStorage.ReadBlob(source, options, default);
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
            AuthenticationMethod = AuthenticationMethod.SASToken,
            URI = $"https://{_storageaccount}.blob.core.windows.net/{_containerName}/{_blobName}?",
            SASToken = "",
            ContainerName = _containerName,
            BlobName = _blobName
        };

        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(() => AzureBlobStorage.ReadBlob(source, options, default));
        Assert.That(ex.Message.Contains("SAS Token and URI required."), ex.Message);
    }

    /// <summary>
    /// Error handling, missing connection string.
    /// </summary>
    [Test]
    public void ReadBlobConnectionStringMissing()
    {
        source = new Source
        {
            AuthenticationMethod = AuthenticationMethod.ConnectionString,
            ConnectionString = "",
            ContainerName = _containerName,
            BlobName = _blobName
        };
        options = new Options
        {
            Encoding = Encode.ASCII
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(() => AzureBlobStorage.ReadBlob(source, options, default));
        Assert.That(ex.Message.Contains("Connection string required."), ex.Message);
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
        var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_storageaccount, _accessKey)).ToString();
        return sasToken;
    }
}
