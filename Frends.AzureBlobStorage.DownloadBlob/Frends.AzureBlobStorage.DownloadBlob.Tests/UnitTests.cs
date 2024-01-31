using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DownloadBlob.Definitions;

namespace Frends.AzureBlobStorage.DownloadBlob.Tests;

[TestClass]
public class UnitTests
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
    private readonly string _testBlob = "test-blob.txt";
    private string _containerName;
    private Destination _destination;
    private string _destinationDirectory;
    private Source _source;
    private readonly string _testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "TestFile.xml");
    private readonly string _appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string _storageAccount = "frendstaskstestcontainer";

    [TestInitialize]
    public async Task TestSetup()
    {
        _destinationDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_destinationDirectory);

        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

        _source = new Source
        {
            ConnectionString = _connectionString,
            BlobName = _testBlob,
            ContainerName = _containerName,
            Encoding = "utf-8"
        };
        _destination = new Destination
        {
            Directory = _destinationDirectory,
            FileExistsOperation = FileExistsAction.Overwrite
        };

        var blobServiceClient = new BlobServiceClient(_connectionString);
        var container = blobServiceClient.GetBlobContainerClient(_containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null);
        var blockBlob = container.GetBlobClient(_testBlob);
        await blockBlob.UploadAsync(_testFilePath, default);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var container = blobServiceClient.GetBlobContainerClient(_containerName);
        await container.DeleteIfExistsAsync(null);
        if (Directory.Exists(_destinationDirectory)) Directory.Delete(_destinationDirectory, true);
    }

    [TestMethod]
    public async Task DownloadBlobAsync_WritesBlobToFile()
    {
        var result = await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.IsTrue(File.Exists(result.FullPath));
        var fileContent = File.ReadAllText(result.FullPath);
        Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task DownloadBlobAsync_ThrowsExceptionIfDestinationFileExists()
    {
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        _destination.FileExistsOperation = FileExistsAction.Error;
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
    }

    [TestMethod]
    public async Task DownloadBlobAsync_RenamesFileIfExists()
    {
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        _destination.FileExistsOperation = FileExistsAction.Rename;
        var result = await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.AreEqual("test-blob(1).txt", result.FileName);
    }

    [TestMethod]
    public async Task DownloadBlobAsync_OverwritesFileIfExists()
    {
        // Download file with same name couple of time.
        _destination.FileExistsOperation = FileExistsAction.Overwrite;
        
        for (int i = 0; i < 4; i++)
            await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        
        Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);
    }

    [TestMethod]
    public async Task AccessTokenAuthenticationTest()
    {
        _source = new Source
        {
            ConnectionMethod = ConnectionMethod.OAuth2,
            StorageAccountName = _storageAccount,
            ApplicationID = _appID,
            TenantID = _tenantID,
            ClientSecret = _clientSecret,
            BlobName = _testBlob,
            ContainerName = _containerName,
            Encoding = "utf-8"
        };

        var result = await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.IsTrue(File.Exists(result.FullPath));
        var fileContent = File.ReadAllText(result.FullPath);
        Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));
    }

    [TestMethod]
    public async Task DownloadBlobAsync_EmptyEncoding()
    {
        var source = _source;
        source.Encoding = "";
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);
    }
}