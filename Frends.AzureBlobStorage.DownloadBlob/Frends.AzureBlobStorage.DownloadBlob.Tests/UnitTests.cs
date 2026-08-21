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
    private readonly string _testBlob = "test-blob.txt";
    private string _containerName;
    private Input _input;
    private Options _options;
    private string _destinationDirectory;
    private Connection _connection;

    private readonly string _testFilePath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "TestFile.xml");

    private readonly string _container = "const-test-container";
    private readonly string _storageAccountName = "stataskdevelopment";

    [SetUp]
    public async Task TestSetup()
    {
        TestHelper.LoadEnvironmentVariables();
        _destinationDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_destinationDirectory);

        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

        _input = new Input
        {
            BlobName = _testBlob,
            ContainerName = _containerName,
            TargetDirectory = _destinationDirectory,
            ActionOnExistingFile = FileExistsAction.Overwrite,
        };

        _options = new Options
        {
            Encoding = FileEncoding.UTF8,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = string.Empty,
        };

        _connection = new Connection()
        {
            ConnectionString = TestHelper.ConnectionString,
            TenantId = TestHelper.TenantId,
            ApplicationId = TestHelper.ClientId,
            ClientSecret = TestHelper.ClientSecret,
            StorageAccountName = _storageAccountName,
            SasToken = TestHelper.SasToken,
        };

        await UploadTestFiles(_containerName);

        // Uploads test files to const container.
        await UploadTestFiles(_container);
    }

    [TearDown]
    public async Task Cleanup()
    {
        var container = GetBlobContainer(TestHelper.ConnectionString, _containerName);
        await container.DeleteIfExistsAsync();
        // Empties the const container
        await DeleteBlobsInContainer(TestHelper.ConnectionString, _container, _input.BlobName);
    }

    [Test]
    public async Task DownloadBlobAsync_WritesBlobToFile()
    {
        // Connection string
        var result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(File.Exists(result.FilePath));
        var fileContent = File.ReadAllText(result.FilePath);
        Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));

        await Cleanup();
        await TestSetup();

        // OAuth
        _connection.AuthenticationMethod = ConnectionMethod.OAuth2;
        result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(File.Exists(result.FilePath));
        fileContent = File.ReadAllText(result.FilePath);
        Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));

        await Cleanup();
        await TestSetup();

        // SAS Token
        _connection.AuthenticationMethod = ConnectionMethod.SasToken;
        _input.ContainerName = _container;
        result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(File.Exists(result.FilePath));
        fileContent = File.ReadAllText(result.FilePath);
        Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));
    }

    [Test]
    public async Task DownloadBlobAsync_ThrowsExceptionIfDestinationFileExists()
    {
        await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        _input.ActionOnExistingFile = FileExistsAction.Throw;
        Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.DownloadBlob(_input, _connection, _options, default));
    }

    [Test]
    public async Task DownloadBlobAsync_RenamesFileIfExists()
    {
        // Connection string
        await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        _input.ActionOnExistingFile = FileExistsAction.Rename;
        var result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        Assert.AreEqual(Path.GetFileName(result.FilePath), "test-blob(1).txt");

        await Cleanup();
        await TestSetup();

        // OAuth
        _connection.AuthenticationMethod = ConnectionMethod.OAuth2;
        await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        _input.ActionOnExistingFile = FileExistsAction.Rename;
        result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        Assert.AreEqual(Path.GetFileName(result.FilePath), "test-blob(1).txt");

        await Cleanup();
        await TestSetup();

        // SAS Token
        _connection.AuthenticationMethod = ConnectionMethod.SasToken;
        _input.ContainerName = _container;
        await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        _input.ActionOnExistingFile = FileExistsAction.Rename;
        result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        Assert.AreEqual(Path.GetFileName(result.FilePath), "test-blob(1).txt");
    }

    [Test]
    public async Task DownloadBlobAsync_OverwritesFileIfExists()
    {
        await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        _input.ActionOnExistingFile = FileExistsAction.Overwrite;

        // Connection string
        await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);

        // OAuth
        _connection.AuthenticationMethod = ConnectionMethod.OAuth2;
        await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);

        // SAS Token
        _connection.AuthenticationMethod = ConnectionMethod.SasToken;
        _input.ContainerName = _container;
        await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);
    }

    [Test]
    public async Task DownloadBlobAsync_DifferentEncoding()
    {
        var encodings = new FileEncoding[]
        {
            FileEncoding.UTF8WithBOM, FileEncoding.UTF8, FileEncoding.Windows1252,
            FileEncoding.Other, FileEncoding.ASCII, FileEncoding.Default,
        };
        _input.ActionOnExistingFile = FileExistsAction.Overwrite;
        _options.OtherEncoding = "windows-1251";

        foreach (var encoding in encodings)
        {
            _options.Encoding = encoding;
            await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
            Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);
        }
    }

    [Test]
    public async Task DownloadBlobAsync_UsesTargetFileName_WhenSpecified()
    {
        _input.TargetFileName = "custom-name.txt";

        var result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("custom-name.txt", Path.GetFileName(result.FilePath));
        Assert.IsTrue(File.Exists(result.FilePath));
        var fileContent = File.ReadAllText(result.FilePath);
        Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));
    }

    [Test]
    public async Task DownloadBlobAsync_UTF8WithBOM_Encoding()
    {
        _options.Encoding = FileEncoding.UTF8WithBOM;
        var result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);

        Assert.IsTrue(result.Success);
        Assert.IsTrue(File.Exists(result.FilePath));
        // Verify BOM is present in the written file
        var bytes = File.ReadAllBytes(result.FilePath);
        // UTF-8 BOM is EF BB BF
        Assert.IsTrue(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF,
            "Expected UTF-8 BOM at the start of the file.");
    }

    [Test]
    public async Task DownloadBlobAsync_CopiesBlobToRequestedDirectory()
    {
        _options.CopyBlob = true;
        _options.BlobCopyDir = "archive/processed";

        var result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        var copiedBlob = GetBlobContainer(TestHelper.ConnectionString, _containerName)
            .GetBlobClient("archive/processed/test-blob.txt");

        Assert.IsTrue(result.Success);
        Assert.IsTrue(File.Exists(result.FilePath));
        Assert.IsTrue(await copiedBlob.ExistsAsync());

        var download = await copiedBlob.DownloadAsync();
        using var reader = new StreamReader(download.Value.Content);
        var copiedContent = await reader.ReadToEndAsync();
        Assert.IsTrue(copiedContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));
    }

    [Test]
    public async Task DownloadBlobAsync_CopiesBlobAndDeletesOriginal_WhenRequested()
    {
        _options.CopyBlob = true;
        _options.BlobCopyDir = "archive";
        _options.DeleteOriginal = true;

        var result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        var container = GetBlobContainer(TestHelper.ConnectionString, _containerName);
        var originalBlob = container.GetBlobClient(_testBlob);
        var copiedBlob = container.GetBlobClient("archive/test-blob.txt");

        Assert.IsTrue(result.Success);
        Assert.IsFalse(await originalBlob.ExistsAsync());
        Assert.IsTrue(await copiedBlob.ExistsAsync());
    }

    [Test]
    public async Task DownloadBlobAsync_RenamesCopiedBlob_WhenTargetBlobAlreadyExists()
    {
        _options.CopyBlob = true;
        _options.BlobCopyDir = "archive/processed";

        var container = GetBlobContainer(TestHelper.ConnectionString, _containerName);
        await container.GetBlobClient("archive/processed/test-blob.txt").UploadAsync(_testFilePath, overwrite: true);
        await container.GetBlobClient("archive/processed/test-blob(1).txt").UploadAsync(_testFilePath, overwrite: true);

        var result = await AzureBlobStorage.DownloadBlob(_input, _connection, _options, default);
        var existingBlob = container.GetBlobClient("archive/processed/test-blob.txt");
        var firstRenamedBlob = container.GetBlobClient("archive/processed/test-blob(1).txt");
        var secondRenamedBlob = container.GetBlobClient("archive/processed/test-blob(2).txt");

        Assert.IsTrue(result.Success);
        Assert.IsTrue(await existingBlob.ExistsAsync());
        Assert.IsTrue(await firstRenamedBlob.ExistsAsync());
        Assert.IsTrue(await secondRenamedBlob.ExistsAsync());
    }

    private async Task UploadTestFiles(string containerName)
    {
        var blobServiceClient = new BlobServiceClient(TestHelper.ConnectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null);
        var blockBlob = container.GetBlobClient(_testBlob);
        await blockBlob.UploadAsync(_testFilePath, overwrite: true);
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
