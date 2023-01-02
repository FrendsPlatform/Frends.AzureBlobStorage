using Azure.Storage;
using Azure.Storage.Sas;
using Frends.AzureBlobStorage.ReadBlob.Definitions;
using NUnit.Framework;
using System;

namespace Frends.AzureBlobStorage.ReadBlob.Tests;

[TestFixture]
public class ReadTest
{ 
    Source source;
    Options options;

    private readonly string _accessKey = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_testsorage01AccessKey");
    private readonly string _connstring = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");
    private readonly string _appID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_AppID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ClientSecret");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_TenantID");
    private readonly string _storageaccount = "testsorage01";
    private readonly string _containerName = "test";
    private readonly string _blobName = "test.txt";

    [Test]
    public void ReadBlobSAS()
    {
        source = new Source
        { 
            AuthenticationMethod = AuthenticationMethod.SASToken,
            URI = $"https://testsorage01.blob.core.windows.net/{_containerName}/{_blobName}?",
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
            URI = $"https://testsorage01.blob.core.windows.net/{_containerName}/{_blobName}?",
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
