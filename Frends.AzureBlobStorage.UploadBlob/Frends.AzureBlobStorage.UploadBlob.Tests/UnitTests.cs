using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.UploadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.UploadBlob.Tests;

[TestClass]
public class UnitTests
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");
    private string _containerName;
    private readonly string _appID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_AppID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ClientSecret");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_TenantID");
    private readonly string _storageAccount = "testsorage01";
    private readonly string _testFileDir = Path.Combine(Environment.CurrentDirectory, "TestFiles");
    private readonly string _testfile = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile.txt");
    private readonly string _testfile2 = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile2.txt");
    private readonly Tag[] _tags = new[] { new Tag { Name = "TagName", Value = "TagValue" } };
    private readonly List<AzureBlobType> _blobtypes = new() { AzureBlobType.Block, AzureBlobType.Page, AzureBlobType.Append };
    private Destination _destinationCS;
    private Destination _destinationOA;
    private Source _source;
    private Options _options;

    [TestInitialize]
    public void TestSetup()
    {
        CreateFiles();
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

        _source = new Source
        {
            SourceType = UploadSourceType.File,
            SourceFile = _testfile,
            Tags = _tags,
            Compress = default,
            ContentsOnly = default,
            SearchPattern = default,
            SourceDirectory = default
        };

        _destinationCS = new Destination
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName,
            BlobType = default,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = null,
            ApplicationID = null,
            StorageAccountName = null,
            ClientSecret = null,
            ContentType = null,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
            ResizeFile = default,
        };

        _destinationOA = new Destination
        {
            ConnectionMethod = ConnectionMethod.OAuth2,
            ContainerName = _containerName,
            BlobType = default,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = _tenantID,
            ApplicationID = _appID,
            StorageAccountName = _storageAccount,
            ClientSecret = _clientSecret,
            ContentType = null,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
            ResizeFile = default,
        };

        _options = new Options() { ThrowErrorOnFailure = true };

    }

    [TestCleanup]
    public async Task CleanUp()
    {
        var container = GetBlobContainer(_connectionString, _containerName);
        await container.DeleteIfExistsAsync();
        DeleteFiles();
    }

    [TestMethod]
    public async Task UploadFile_WithAndWithoutTags()
    {
        var container = GetBlobContainer(_connectionString, _containerName);

        var _sourceWithoutTags = _source;
        _sourceWithoutTags.Tags = null;

        foreach (var blobtype in _blobtypes)
        {
            _destinationCS.BlobType = blobtype;
            _destinationOA.BlobType = blobtype;

            // connection string
            var resultWithTags = await AzureBlobStorage.UploadBlob(_source, _destinationCS, _options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            var resultWithoutTags = await AzureBlobStorage.UploadBlob(_sourceWithoutTags, _destinationCS, _options, default);
            Assert.IsTrue(resultWithoutTags.Success);
            Assert.IsTrue(resultWithoutTags.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            // OAuth
            var resultWithTags2 = await AzureBlobStorage.UploadBlob(_source, _destinationOA, _options, default);
            Assert.IsTrue(resultWithTags2.Success);
            Assert.IsTrue(resultWithTags2.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            var resultWithoutTags2 = await AzureBlobStorage.UploadBlob(_sourceWithoutTags, _destinationOA, _options, default);
            Assert.IsTrue(resultWithoutTags2.Success);
            Assert.IsTrue(resultWithoutTags2.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
        }
    }

    [TestMethod]
    public async Task UploadFile_SetBlobName_ForceEncoding_ForceContentType()
    {
        _source.BlobName = "SomeBlob";
        _destinationCS.FileEncoding = "foo/bar";
        _destinationCS.ContentType = "text/xml";
        _destinationOA.FileEncoding = "foo/bar";
        _destinationOA.ContentType = "text/xml";

        var container = GetBlobContainer(_connectionString, _containerName);

        foreach (var blobtype in _blobtypes)
        {
            _destinationCS.BlobType = blobtype;
            _destinationOA.BlobType = blobtype;

            // Connection string
            var result = await AzureBlobStorage.UploadBlob(_source, _destinationCS, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/SomeBlob"));
            Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");

            //OAuth
            var result2 = await AzureBlobStorage.UploadBlob(_source, _destinationOA, _options, default);
            Assert.IsTrue(result2.Success);
            Assert.IsTrue(result2.Data.ContainsValue($"{container.Uri}/SomeBlob"));
            Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");
        }
    }

    [TestMethod]
    public async Task UploadFile_Compress()
    {
        _source.BlobName = "compress.gz";
        _source.Compress = true;

        var container = GetBlobContainer(_connectionString, _containerName);

        foreach (var blobtype in _blobtypes)
        {
            _destinationCS.BlobType = blobtype;
            _destinationOA.BlobType = blobtype;

            // connection string
            var result = await AzureBlobStorage.UploadBlob(_source, _destinationCS, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/compress.gz"));
            Assert.IsTrue(await container.GetBlobClient("compress.gz").ExistsAsync(), "Uploaded SomeBlob blob should exist");

            // OAuth
            var result2 = await AzureBlobStorage.UploadBlob(_source, _destinationOA, _options, default);
            Assert.IsTrue(result2.Success);
            Assert.IsTrue(result2.Data.ContainsValue($"{container.Uri}/compress.gz"));
            Assert.IsTrue(await container.GetBlobClient("compress.gz").ExistsAsync(), "Uploaded SomeBlob blob should exist");
        }
    }

    [TestMethod]
    public async Task UploadFile_HandleExistingFile()
    {
        var errorHandlers = new List<HandleExistingFile>() { HandleExistingFile.Append, HandleExistingFile.Overwrite, HandleExistingFile.Error };
        var _source2 = _source;
        _source2.BlobName = "testfile.txt";
        _source2.SourceFile = _testfile2;

        foreach (var blobtype in _blobtypes)
        {
            _destinationCS.BlobType = blobtype;
            _destinationOA.BlobType = blobtype;

            if (blobtype is AzureBlobType.Page)
            {
                _destinationCS.ResizeFile = true;
                _destinationOA.ResizeFile = true;
            }

            foreach (var handler in errorHandlers)
            {
                _destinationCS.HandleExistingFile = handler;

                var container = GetBlobContainer(_connectionString, _containerName);
                var result = await AzureBlobStorage.UploadBlob(_source, _destinationCS, _options, default);
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

                if (handler is HandleExistingFile.Append)
                {
                    var result2 = await AzureBlobStorage.UploadBlob(_source2, _destinationCS, _options, default);
                    // You can use Azure Portal to check if the blob contains 2x "Etiam dui".
                    Assert.IsTrue(result2.Success);
                    Assert.IsTrue(result2.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else if (handler is HandleExistingFile.Overwrite)
                {
                    var result2 = await AzureBlobStorage.UploadBlob(_source, _destinationCS, _options, default);
                    Assert.IsTrue(result2.Success);
                    Assert.IsTrue(result2.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else
                {
                    var ex = await Assert.ThrowsExceptionAsync<Exception>(() => AzureBlobStorage.UploadBlob(_source, _destinationCS, _options, default));
                    Assert.IsTrue(ex.InnerException.InnerException.Message.Contains("already exists"));
                }

                await CleanUp();
                TestSetup();
            }
        }
    }

    [TestMethod]
    public async Task UploadDirectory_WithAndWithoutTags()
    {
        _source.SourceType = UploadSourceType.Directory;
        _source.SourceDirectory = _testFileDir;
        _source.SourceFile = default;

        var container = GetBlobContainer(_connectionString, _containerName);

        foreach (var blobtype in _blobtypes)
        {
            _destinationCS.BlobType = blobtype;
            _destinationOA.BlobType = blobtype;

            if (blobtype is AzureBlobType.Page)
            {
                _destinationCS.ResizeFile = true;
                _destinationOA.ResizeFile = true;
            }

            var resultWithTags = await AzureBlobStorage.UploadBlob(_source, _destinationCS, _options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");
        }
    }

    [TestMethod]
    public async Task UploadDirectory_RenameDir()
    {
        _source.SourceType = UploadSourceType.Directory;
        _source.SourceDirectory = _testFileDir;
        _source.SourceFile = default;
        _source.BlobFolderName = "RenameDir";

        var container = GetBlobContainer(_connectionString, _containerName);

        foreach (var blobtype in _blobtypes)
        {
            _destinationCS.BlobType = blobtype;
            _destinationOA.BlobType = blobtype;

            if (blobtype is AzureBlobType.Page)
            {
                _destinationCS.ResizeFile = true;
                _destinationOA.ResizeFile = true;
            }

            var resultWithTags = await AzureBlobStorage.UploadBlob(_source, _destinationCS, _options, default);
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