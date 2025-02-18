using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DownloadBlob.Definitions;

namespace Frends.AzureBlobStorage.DownloadBlob.Tests;

[TestFixture]
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
    private readonly string _sasToken = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_SASToken");
    private readonly string _container = "const-test-container";
    private readonly string _uri = "https://stataskdevelopment.blob.core.windows.net";

    [SetUp]
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
            Encoding = FileEncoding.UTF8,
            EnableBOM = false,
            TenantID = _tenantID,
            ApplicationID = _appID,
            ClientSecret = _clientSecret,
            Uri = _uri,
            SASToken = _sasToken
        };

        _destination = new Destination
        {
            Directory = _destinationDirectory,
            FileExistsOperation = FileExistsAction.Overwrite
        };

        await UploadTestFiles(_containerName);

        // Uploads test files to const container.
        await UploadTestFiles(_container);
    }

    [TearDown]
    public async Task Cleanup()
    {
        var container = GetBlobContainer(_connectionString, _containerName);
        await container.DeleteIfExistsAsync();
        // Empties the const container
        await DeleteBlobsInContainer(_connectionString, _container, _source.BlobName);
    }

    [Test]
    public async Task DownloadBlobAsync_WritesBlobToFile()
    {
        // Connection string
        var result = await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.IsTrue(File.Exists(result.FullPath));
        var fileContent = File.ReadAllText(result.FullPath);
        Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));

        await Cleanup();
        await TestSetup();

        // OAuth
        _source.ConnectionMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.IsTrue(File.Exists(result.FullPath));
        fileContent = File.ReadAllText(result.FullPath);
        Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));

        await Cleanup();
        await TestSetup();

        // SAS Token
        _source.ConnectionMethod = ConnectionMethod.SASToken;
        _source.ContainerName = _container;
        result = await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.IsTrue(File.Exists(result.FullPath));
        fileContent = File.ReadAllText(result.FullPath);
        Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));
    }

    [Test]
    public async Task DownloadBlobAsync_ThrowsExceptionIfDestinationFileExists()
    {
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        _destination.FileExistsOperation = FileExistsAction.Error;
        Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.DownloadBlob(_source, _destination, default));
    }

    [Test]
    public async Task DownloadBlobAsync_RenamesFileIfExists()
    {
        // Connection string
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        _destination.FileExistsOperation = FileExistsAction.Rename;
        var result = await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.AreEqual("test-blob(1).txt", result.FileName);

        await Cleanup();
        await TestSetup();

        // OAuth
        _source.ConnectionMethod = ConnectionMethod.OAuth2;
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        _destination.FileExistsOperation = FileExistsAction.Rename;
        result = await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.AreEqual("test-blob(1).txt", result.FileName);

        await Cleanup();
        await TestSetup();

        // SAS Token
        _source.ConnectionMethod = ConnectionMethod.SASToken;
        _source.ContainerName = _container;
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        _destination.FileExistsOperation = FileExistsAction.Rename;
        result = await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.AreEqual("test-blob(1).txt", result.FileName);
    }

    [Test]
    public async Task DownloadBlobAsync_OverwritesFileIfExists()
    {
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        _destination.FileExistsOperation = FileExistsAction.Overwrite;

        // Connection string
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);

        // OAuth
        _source.ConnectionMethod = ConnectionMethod.OAuth2;
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);

        // SAS Token
        _source.ConnectionMethod = ConnectionMethod.SASToken;
        _source.ContainerName = _container;
        await AzureBlobStorage.DownloadBlob(_source, _destination, default);
        Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);
    }

    [Test]
    public async Task DownloadBlobAsync_DifferentEncoding()
    {
        var encodings = new FileEncoding[] { FileEncoding.UTF8, FileEncoding.WINDOWS1252, FileEncoding.Other, FileEncoding.ASCII, FileEncoding.Default };
        var source = _source;
        _destination.FileExistsOperation = FileExistsAction.Overwrite;
        source.FileEncodingString = "windows-1251";

        foreach (var encoding in encodings)
        {
            source.Encoding = encoding;
            await AzureBlobStorage.DownloadBlob(_source, _destination, default);
            Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);
        }
    }

    private async Task UploadTestFiles(string containerName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null);
        var blockBlob = container.GetBlobClient(_testBlob);
        await blockBlob.UploadAsync(_testFilePath, default);
    }

    private async Task DeleteBlobsInContainer(string connectionString, string containerName, string blobName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobContainer = blobServiceClient.GetBlobContainerClient(containerName);
        var blobList = blobContainer.GetBlobsAsync();
        await foreach (var item in blobList)
            await blobContainer.DeleteBlobIfExistsAsync(item.Name);
    }

    private static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        return blobServiceClient.GetBlobContainerClient(containerName);
    }
}