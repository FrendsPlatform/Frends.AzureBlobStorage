using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.UploadBlob.Definitions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.UploadBlob.Tests;

[TestFixture]
public class UnitTests
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
    private string _containerName;
    private readonly string _appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
    private readonly string _uri = "https://stataskdevelopment.blob.core.windows.net";
    private readonly string _testFileDir = Path.Combine(Environment.CurrentDirectory, "TestFiles");
    private readonly string _testfile = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile.txt");
    private readonly string _testfile2 = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile2.txt");
    private readonly string _sasToken = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_SASToken");
    private readonly Tag[] _tags = new[] { new Tag { Name = "TagName", Value = "TagValue" } };
    private readonly List<AzureBlobType> _blobtypes = new() { AzureBlobType.Block, AzureBlobType.Page, AzureBlobType.Append };
    private readonly string _container = "const-test-container";
    private Destination _destination;
    private Source _source;
    private Options _options;

    [SetUp]
    public void TestSetup()
    {
        CreateFiles();
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

        _source = new Source
        {
            SourceType = UploadSourceType.File,
            SourceFile = _testfile,
            Tags = null,
            Compress = default,
            ContentsOnly = default,
            SearchPattern = default,
            SourceDirectory = default
        };

        _destination = new Destination
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName,
            BlobType = default,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            Encoding = FileEncoding.UTF8,
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = _tenantID,
            ApplicationID = _appID,
            Uri = _uri,
            SASToken = _sasToken,
            ClientSecret = _clientSecret,
            ContentType = null,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
            ResizeFile = default,
        };

        _options = new Options() { ThrowErrorOnFailure = true };

    }

    [TearDown]
    public async Task CleanUp()
    {
        var container = GetBlobContainer(_connectionString, _containerName);
        await container.DeleteIfExistsAsync();
        DeleteFiles();
    }

    [Test]
    public async Task UploadFile_SimpleUpload()
    {
        var container = GetBlobContainer(_connectionString, _containerName);
        foreach (var blobtype in _blobtypes)
        {
            _destination.BlobType = blobtype;

            // Connection string
            var result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            // OAuth
            _destination.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success, "OAuth");
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"), "OAuth");
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            // SAS token
            _destination.ConnectionMethod = ConnectionMethod.SASToken;
            _destination.ContainerName = _container;
            container = GetBlobContainer(_connectionString, _container);
            result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
        }
    }

    [Test]
    public async Task UploadFile_WithTags()
    {
        var container = GetBlobContainer(_connectionString, _containerName);

        _source.Tags = _tags;

        foreach (var blobtype in _blobtypes)
        {
            _destination.BlobType = blobtype;

            // connection string
            var result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            // OAuth
            _destination.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            // SAS token
            _destination.ConnectionMethod = ConnectionMethod.SASToken;
            _destination.ContainerName = _container;
            container = GetBlobContainer(_connectionString, _container);
            result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
        }
    }

    [Test]
    public async Task UploadFile_SetBlobName_ForceEncoding_ForceContentType()
    {
        _source.BlobName = "SomeBlob";
        _destination.ContentType = "text/xml";

        var container = GetBlobContainer(_connectionString, _containerName);

        foreach (var blobtype in _blobtypes)
        {
            _destination.BlobType = blobtype;

            // Connection string
            var result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/SomeBlob"));
            Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");

            //OAuth
            _destination.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/SomeBlob"));
            Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");

            // SAS token
            _destination.ConnectionMethod = ConnectionMethod.SASToken;
            _destination.ContainerName = _container;
            result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/SomeBlob"));
            Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");
        }
    }

    [Test]
    public async Task UploadFile_Compress()
    {
        _source.BlobName = "compress.gz";
        _source.Compress = true;

        var container = GetBlobContainer(_connectionString, _containerName);

        foreach (var blobtype in _blobtypes)
        {
            _destination.BlobType = blobtype;

            // connection string
            var result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/compress.gz"));
            Assert.IsTrue(await container.GetBlobClient("compress.gz").ExistsAsync(), "Uploaded SomeBlob blob should exist");

            if (blobtype == AzureBlobType.Block)
            {
                var downloadedFilePath = Path.GetTempFileName();
                try
                {
                    await container.GetBlobClient("compress.gz").DownloadToAsync(downloadedFilePath);

                    using (var downloadedStream = File.OpenRead(downloadedFilePath))
                    using (var gzipStream = new GZipStream(downloadedStream, CompressionMode.Decompress))
                    using (var reader = new StreamReader(gzipStream))
                    {
                        var decompressedContent = await reader.ReadToEndAsync();
                        var originalContent = await File.ReadAllTextAsync(_source.SourceFile);
                        Assert.AreEqual(originalContent, decompressedContent, "Decompressed content should match original");
                    }
                }
                finally
                {
                    if (File.Exists(downloadedFilePath)) File.Delete(downloadedFilePath);
                }
            }

            // OAuth
            _destination.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/compress.gz"));
            Assert.IsTrue(await container.GetBlobClient("compress.gz").ExistsAsync(), "Uploaded SomeBlob blob should exist");

            // SAS token
            _destination.ConnectionMethod = ConnectionMethod.SASToken;
            _destination.ContainerName = _container;
            result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/compress.gz"));
            Assert.IsTrue(await container.GetBlobClient("compress.gz").ExistsAsync(), "Uploaded SomeBlob blob should exist");
        }
    }

    [Test]
    public async Task UploadFile_HandleExistingFile()
    {
        var errorHandlers = new List<HandleExistingFile>() { HandleExistingFile.Overwrite, HandleExistingFile.Error };
        var _source2 = _source;
        _source2.BlobName = "testfile.txt";
        _source2.SourceFile = _testfile2;

        foreach (var blobtype in _blobtypes)
        {
            _destination.BlobType = blobtype;

            if (blobtype is AzureBlobType.Page)
                _destination.ResizeFile = true;

            foreach (var handler in errorHandlers)
            {
                _destination.HandleExistingFile = handler;

                var container = GetBlobContainer(_connectionString, _containerName);

                /*
                // Connection string
                var result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

                if (handler is HandleExistingFile.Append)
                {
                    result = await AzureBlobStorage.UploadBlob(_source2, _destination, _options, default);
                    // You can use Azure Portal to check if the blob contains 2x "Etiam dui".
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else if (handler is HandleExistingFile.Overwrite)
                {
                    result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else
                {
                    var ex = await Assert.ThrowsExceptionAsync<Exception>(() => AzureBlobStorage.UploadBlob(_source, _destination, _options, default));
                    Assert.IsTrue(ex.InnerException.InnerException.Message.Contains("already exists"));
                }
                await CleanUp();
                TestSetup();
                */

                // OAuth
                _destination.ConnectionMethod = ConnectionMethod.OAuth2;

                var result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

                if (handler is HandleExistingFile.Append)
                {
                    result = await AzureBlobStorage.UploadBlob(_source2, _destination, _options, default);
                    // You can use Azure Portal to check if the blob contains 2x "Etiam dui".
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else if (handler is HandleExistingFile.Overwrite)
                {
                    result = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else
                {
                    var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_source, _destination, _options, default));
                    Assert.IsTrue(ex.InnerException.InnerException.Message.Contains("already exists"));
                }

                await CleanUp();
                TestSetup();
            }
        }
    }

    [Test]
    public async Task UploadDirectory_WithAndWithoutTags()
    {
        _source.SourceType = UploadSourceType.Directory;
        _source.SourceDirectory = _testFileDir;
        _source.SourceFile = default;

        var container = GetBlobContainer(_connectionString, _containerName);

        foreach (var blobtype in _blobtypes)
        {
            _destination.BlobType = blobtype;

            if (blobtype is AzureBlobType.Page)
                _destination.ResizeFile = true;

            var resultWithTags = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

            // OAuth
            _destination.ConnectionMethod = ConnectionMethod.OAuth2;
            resultWithTags = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");
        }
    }

    [Test]
    public async Task UploadDirectory_RenameDir()
    {
        _source.SourceType = UploadSourceType.Directory;
        _source.SourceDirectory = _testFileDir;
        _source.SourceFile = default;
        _source.BlobFolderName = "RenameDir";

        var container = GetBlobContainer(_connectionString, _containerName);

        foreach (var blobtype in _blobtypes)
        {
            _destination.BlobType = blobtype;

            if (blobtype is AzureBlobType.Page)
                _destination.ResizeFile = true;

            var resultWithTags = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/RenameDir/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/RenameDir/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

            // OAuth
            _destination.ConnectionMethod = ConnectionMethod.OAuth2;
            resultWithTags = await AzureBlobStorage.UploadBlob(_source, _destination, _options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/RenameDir/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/RenameDir/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");
        }
    }

    private static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        return blobServiceClient.GetBlobContainerClient(containerName);
    }

    private void CreateFiles()
    {
        Directory.CreateDirectory(_testFileDir);

        // 512 byte file
        File.WriteAllText(_testfile, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque non sem quis orci rutrum hendrerit. Fusce ultricies cursus ante nec bibendum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam libero massa, viverra id suscipit in, tincidunt sit amet urna. Vestibulum gravida a massa eu molestie. Phasellus volutpat neque vitae enim molestie, vitae pharetra massa varius. Phasellus ante nulla, faucibus nec tristique eu, dignissim quis magna. Sed vitae sodales ipsum. Ut et maximus nibh. Etiam dui.");

        File.WriteAllText(_testfile2, "More texts.");
    }

    private void DeleteFiles()
    {
        if (Directory.Exists(_testFileDir))
            Directory.Delete(_testFileDir, true);
    }
}