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
    private readonly string _connstring = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
    private readonly string _appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private readonly string _storageaccount = "frendstaskstestcontainer";
    private static string _containerName;
    private readonly string _blobName = "test.txt";
    private readonly string _testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestFiles", "TestFile.xml");

    [SetUp]
    public async Task SetUp()
    {
        var test = _testFilePath;
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
            ConnectionString = _connstring,
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
            ConnectionString = _connstring,
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
        var source = new Source
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

        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.ReadBlob(source, options, default));
        Assert.That(ex.Message.Equals("SAS Token and URI required."));
    }

    /// <summary>
    /// Error handling, missing connection string.
    /// </summary>
    [Test]
    public void ReadBlobConnectionStringMissing()
    {
        var source = new Source
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

        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.ReadBlob(source, options, default));
        Assert.That(ex.Message.Equals("Connection string required."));
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
