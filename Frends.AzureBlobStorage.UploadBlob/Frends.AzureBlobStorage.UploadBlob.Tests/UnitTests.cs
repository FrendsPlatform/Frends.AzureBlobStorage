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
    private Input _input;
    private Connection _connection;
    private Options _options;

    [SetUp]
    public void TestSetup()
    {
        CreateFiles();
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

        _input = new Input
        {
            SourceType = UploadSourceType.File,
            SourceFile = _testfile,
            Tags = null,
            Compress = default,
            ContentsOnly = default,
            SearchPattern = default,
            SourceDirectory = default,
            HandleExistingFile = HandleExistingFile.Overwrite
        };

        _connection = new Connection
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            TenantID = _tenantID,
            ApplicationID = _appID,
            Uri = _uri,
            SASToken = _sasToken,
            ClientSecret = _clientSecret
        };

        _options = new Options
        {
            ThrowErrorOnFailure = true,
            BlobType = default,
            ResizeFile = default,
            PageMaxSize = default,
            PageOffset = default,
            ContentType = null,
            Encoding = FileEncoding.UTF8,
            EnableBOM = false,
            ParallelOperations = default
        };

    }

    [TearDown]
    public async Task CleanUp()
    {
        var container = GetBlobContainer(_connectionString, _containerName);
        await container.DeleteIfExistsAsync();
        // Empties the const container.
        await DeleteBlobsInContainer(_connectionString, _container, _input.BlobName);
        DeleteFiles();
    }

    [Test]
    public async Task UploadFile_SimpleUpload()
    {
        var container = GetBlobContainer(_connectionString, _containerName);
        foreach (var blobtype in _blobtypes)
        {
            _options.BlobType = blobtype;

            // Connection string
            var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            // OAuth
            _connection.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success, "OAuth");
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"), "OAuth");
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            // SAS token
            _connection.ConnectionMethod = ConnectionMethod.SASToken;
            _connection.ContainerName = _container;
            container = GetBlobContainer(_connectionString, _container);
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
        }
    }

    [Test]
    public async Task UploadBlob_RenameBlob()
    {
        var container = GetBlobContainer(_connectionString, _containerName);
        _input.BlobName = "RenamedBlob";

        foreach (var blobtype in _blobtypes)
        {
            _options.BlobType = blobtype;

            // Connection string
            var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{_input.BlobName}"));
            Assert.IsTrue(await container.GetBlobClient(_input.BlobName).ExistsAsync());

            // OAuth
            _connection.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success, "OAuth");
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{_input.BlobName}"), "OAuth");
            Assert.IsTrue(await container.GetBlobClient(_input.BlobName).ExistsAsync());

            // SAS token
            _connection.ConnectionMethod = ConnectionMethod.SASToken;
            _connection.ContainerName = _container;
            container = GetBlobContainer(_connectionString, _container);
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{_input.BlobName}"));
            Assert.IsTrue(await container.GetBlobClient(_input.BlobName).ExistsAsync());
        }
    }

    [Test]
    public async Task UploadBlob_ContentsOnly()
    {
        _input.ContentsOnly = true;

        var container = GetBlobContainer(_connectionString, _containerName);
        foreach (var blobtype in _blobtypes)
        {
            _options.BlobType = blobtype;

            // Connection string
            var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            // OAuth
            _connection.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success, "OAuth");
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"), "OAuth");
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            // SAS token
            _connection.ConnectionMethod = ConnectionMethod.SASToken;
            _connection.ContainerName = _container;
            container = GetBlobContainer(_connectionString, _container);
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
        }
    }

    [Test]
    public async Task UploadFile_WithTags()
    {
        _input.Tags = _tags;

        foreach (var blobtype in _blobtypes)
        {
            _options.BlobType = blobtype;

            // connection string
            var container = GetBlobContainer(_connectionString, _containerName);
            _connection.ConnectionMethod = ConnectionMethod.ConnectionString;
            var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            await CleanUp();
            TestSetup();

            // OAuth
            container = GetBlobContainer(_connectionString, _containerName);
            _connection.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            foreach (var item in result.Data)
            {
                Console.WriteLine(item.Value);
            }
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            await CleanUp();
            TestSetup();

            // SAS token
            container = GetBlobContainer(_connectionString, _container);
            _connection.ConnectionMethod = ConnectionMethod.SASToken;
            _connection.ContainerName = _container;
            container = GetBlobContainer(_connectionString, _container);
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            await CleanUp();
            TestSetup();
        }
    }

    [Test]
    public async Task UploadFile_SetBlobName_ForceEncoding_ForceContentType()
    {
        _input.BlobName = "SomeBlob";
        _options.ContentType = "text/xml";

        foreach (var blobtype in _blobtypes)
        {
            _options.BlobType = blobtype;
            _connection.ContainerName = _containerName;

            var container = GetBlobContainer(_connectionString, _containerName);

            // Connection string
            _connection.ConnectionMethod = ConnectionMethod.ConnectionString;
            var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/SomeBlob"));
            Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");

            //OAuth
            _connection.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/SomeBlob"));
            Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");

            // SAS token
            _connection.ConnectionMethod = ConnectionMethod.SASToken;
            _connection.ContainerName = _container;
            container = GetBlobContainer(_connectionString, _container);
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/SomeBlob"));
            Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");
        }
    }

    [Test]
    public async Task UploadFile_Compress()
    {
        _input.BlobName = "compress.gz";
        _input.Compress = true;

        foreach (var blobtype in _blobtypes)
        {
            _options.BlobType = blobtype;
            _connection.ContainerName = _containerName;
            var container = GetBlobContainer(_connectionString, _containerName);

            // connection string
            _connection.ConnectionMethod = ConnectionMethod.ConnectionString;
            var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
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
                        var originalContent = await File.ReadAllTextAsync(_input.SourceFile);
                        Assert.AreEqual(originalContent, decompressedContent, "Decompressed content should match original");
                    }
                }
                finally
                {
                    if (File.Exists(downloadedFilePath)) File.Delete(downloadedFilePath);
                }
            }

            // OAuth
            _connection.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/compress.gz"));
            Assert.IsTrue(await container.GetBlobClient("compress.gz").ExistsAsync(), "Uploaded SomeBlob blob should exist");

            // SAS token
            _connection.ConnectionMethod = ConnectionMethod.SASToken;
            _connection.ContainerName = _container;
            container = GetBlobContainer(_connectionString, _container);
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/compress.gz"));
            Assert.IsTrue(await container.GetBlobClient("compress.gz").ExistsAsync(), "Uploaded SomeBlob blob should exist");
        }
    }

    [Test]
    public async Task UploadFile_HandleExistingFile()
    {
        var errorHandlers = new List<HandleExistingFile>() { HandleExistingFile.Append, HandleExistingFile.Overwrite, HandleExistingFile.Error };
        var _source2 = _input;
        _source2.BlobName = "testfile.txt";
        _source2.SourceFile = _testfile2;

        foreach (var blobtype in _blobtypes)
        {
            _options.BlobType = blobtype;

            if (blobtype is AzureBlobType.Page)
                _options.ResizeFile = true;

            foreach (var handler in errorHandlers)
            {
                // Connection string
                _input.HandleExistingFile = handler;
                var container = GetBlobContainer(_connectionString, _connection.ContainerName);
                _connection.ConnectionMethod = ConnectionMethod.ConnectionString;
                var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

                if (handler is HandleExistingFile.Append)
                {
                    result = await AzureBlobStorage.UploadBlob(_source2, _connection, _options, default);
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else if (handler is HandleExistingFile.Overwrite)
                {
                    result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else
                {
                    var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
                    Assert.IsTrue(ex.InnerException.InnerException.Message.Contains("already exists"));
                }

                await CleanUp();
                TestSetup();

                // OAuth
                container = GetBlobContainer(_connectionString, _connection.ContainerName);
                _connection.ConnectionMethod = ConnectionMethod.OAuth2;
                _input.HandleExistingFile = handler;

                result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

                if (handler is HandleExistingFile.Append)
                {
                    result = await AzureBlobStorage.UploadBlob(_source2, _connection, _options, default);
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else if (handler is HandleExistingFile.Overwrite)
                {
                    result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else
                {
                    var test = _input.HandleExistingFile;
                    var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
                    Assert.IsTrue(ex.InnerException.InnerException.Message.Contains("already exists"));
                }

                await CleanUp();
                TestSetup();

                // SAS Token
                _connection.ConnectionMethod = ConnectionMethod.SASToken;
                _input.HandleExistingFile = handler;
                _connection.ContainerName = _container;
                container = GetBlobContainer(_connectionString, _connection.ContainerName);

                result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

                if (handler is HandleExistingFile.Append)
                {
                    result = await AzureBlobStorage.UploadBlob(_source2, _connection, _options, default);
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else if (handler is HandleExistingFile.Overwrite)
                {
                    result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
                    Assert.IsTrue(result.Success);
                    Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                    Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
                }
                else
                {
                    var test = _input.HandleExistingFile;
                    var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
                    Assert.IsTrue(ex.InnerException.InnerException.Message.Contains("already exists"));
                }

                await CleanUp();
                TestSetup();
            }
        }
    }

    [Test]
    public async Task UploadDirectory_WithTags()
    {
        foreach (var blobtype in _blobtypes)
        {
            if (blobtype is AzureBlobType.Page)
                _options.ResizeFile = true;

            // Connection string
            _options.BlobType = blobtype;
            _input.SourceType = UploadSourceType.Directory;
            _input.SourceDirectory = _testFileDir;
            _input.SourceFile = default;
            var container = GetBlobContainer(_connectionString, _containerName);
            var resultWithTags = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

            await CleanUp();
            TestSetup();

            if (blobtype is AzureBlobType.Page)
                _options.ResizeFile = true;

            // OAuth
            _options.BlobType = blobtype;
            _input.SourceType = UploadSourceType.Directory;
            _input.SourceDirectory = _testFileDir;
            _input.SourceFile = default;
            container = GetBlobContainer(_connectionString, _containerName);
            _connection.ConnectionMethod = ConnectionMethod.OAuth2;
            resultWithTags = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

            await CleanUp();
            TestSetup();

            if (blobtype is AzureBlobType.Page)
                _options.ResizeFile = true;

            // SAS token
            _options.BlobType = blobtype;
            _input.SourceType = UploadSourceType.Directory;
            _input.SourceDirectory = _testFileDir;
            _input.SourceFile = default;
            container = GetBlobContainer(_connectionString, _container);
            _connection.ConnectionMethod = ConnectionMethod.SASToken;
            _connection.ContainerName = _container;
            resultWithTags = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

            await CleanUp();
            TestSetup();
        }
    }

    [Test]
    public async Task UploadDirectory_RenameDir()
    {
        foreach (var blobtype in _blobtypes)
        {
            _options.BlobType = blobtype;

            if (blobtype is AzureBlobType.Page)
                _options.ResizeFile = true;

            // Connection string
            _input.SourceType = UploadSourceType.Directory;
            _input.SourceDirectory = _testFileDir;
            _input.SourceFile = default;
            _input.BlobFolderName = "RenameDir";
            var container = GetBlobContainer(_connectionString, _containerName);
            var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/RenameDir/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/RenameDir/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

            await CleanUp();
            TestSetup();

            // OAuth
            _input.SourceType = UploadSourceType.Directory;
            _input.SourceDirectory = _testFileDir;
            _input.SourceFile = default;
            _input.BlobFolderName = "RenameDir";
            container = GetBlobContainer(_connectionString, _containerName);
            _connection.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/RenameDir/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/RenameDir/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

            await CleanUp();
            TestSetup();

            // SAS token
            _input.SourceType = UploadSourceType.Directory;
            _input.SourceDirectory = _testFileDir;
            _input.SourceFile = default;
            _input.BlobFolderName = "RenameDir";
            container = GetBlobContainer(_connectionString, _container);
            _connection.ConnectionMethod = ConnectionMethod.SASToken;
            _connection.ContainerName = _container;
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/RenameDir/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/RenameDir/testfile2.txt"));
            Assert.IsTrue(await container.GetBlobClient("RenameDir/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

            await CleanUp();
            TestSetup();
        }
    }

    [Test]
    public async Task UploadBlob_TestEncoding()
    {
        var encodings = new List<FileEncoding>()
        {
            FileEncoding.UTF8,
            FileEncoding.Default,
            FileEncoding.ASCII,
            FileEncoding.WINDOWS1252,
            FileEncoding.Other
        };

        var expected = File.ReadAllText(_input.SourceFile);

        _options.FileEncodingString = "windows-1251";

        foreach (var encoding in encodings)
        {
            _options.Encoding = encoding;

            // Connection string
            var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success, $"Encoding: {encoding}");
            Assert.IsTrue(await BlobExists(_connection.ContainerName, Path.GetFileName(_input.SourceFile), expected));

            _options.EnableBOM = true;

            // OAuth
            _input.BlobName = $"testblob_{Guid.NewGuid()}";
            _connection.ConnectionMethod = ConnectionMethod.OAuth2;
            result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
            Assert.IsTrue(result.Success, $"Encoding: {encoding}");
            Assert.IsTrue(await BlobExists(_connection.ContainerName, Path.GetFileName(_input.SourceFile), expected));
        }
    }

    [Test]
    public void UploadBlob_ErrorUploadTypeDirectoryNoDirectory()
    {
        _input.SourceFile = "";
        _input.SourceDirectory = "";
        _input.SourceType = UploadSourceType.Directory;

        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
        Assert.AreEqual("Source.SourceDirectory value is empty.", ex.InnerException.Message);
    }

    [Test]
    public void UploadBlob_ErrorUploadTypeDirectoryDirectoryDoesNotExist()
    {
        _input.SourceFile = "";
        _input.SourceDirectory = @"C:\\doesnt\\exist";
        _input.SourceType = UploadSourceType.Directory;

        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
        Assert.AreEqual(@"Source directory C:\\doesnt\\exist doesn't exists.", ex.InnerException.Message);
    }

    [Test]
    public void UploadBlob_ErrorNoSourceFile()
    {
        _input.SourceFile = "";
        _input.SourceType = UploadSourceType.File;

        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
        Assert.AreEqual("Source.SourceFile value is empty.", ex.InnerException.Message);
    }

    [Test]
    public void UploadBlob_ErrorSourceFileNotExist()
    {
        _input.SourceFile = "doesntexists.txt";
        _input.SourceType = UploadSourceType.File;

        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
        Assert.AreEqual("Source.SourceFile not found.", ex.InnerException.Message);
    }

    [Test]
    public void UploadBlob_ErrorOAuth2EmptyCredentials()
    {
        _connection.ConnectionMethod = ConnectionMethod.OAuth2;
        _connection.ApplicationID = "";
        _connection.ClientSecret = "";

        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
        Assert.AreEqual("Destination.StorageAccountName, Destination.ClientSecret, Destination.ApplicationID and Destination.TenantID parameters can't be empty when Destination.ConnectionMethod = OAuth.", ex.InnerException.Message);
    }

    [Test]
    public void UploadBlob_ErrorEmptyConnectionString()
    {
        _connection.ConnectionMethod = ConnectionMethod.ConnectionString;
        _connection.ConnectionString = "";
        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
        Assert.AreEqual("Destination.ConnectionString parameter can't be empty when Destination.ConnectionMethod = ConnectionString.", ex.InnerException.Message);
    }

    [Test]
    public void UploadBlob_ErrorEmptyUri()
    {
        _connection.ConnectionMethod = ConnectionMethod.SASToken;
        _connection.Uri = "";
        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
        Assert.AreEqual("Destination.SASToken and Destination.URI parameters can't be empty when Destination.ConnectionMethod = SASToken.", ex.InnerException.Message);
    }

    [Test]
    public void UploadBlob_ErrorEmptyContainerName()
    {
        _connection.ConnectionMethod = ConnectionMethod.ConnectionString;
        _connection.ContainerName = "";
        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
        Assert.AreEqual("Destination.ContainerName parameter can't be empty.", ex.InnerException.Message);
    }

    [Test]
    public void UploadBlob_ErrorEmptySASToken()
    {
        _connection.ConnectionMethod = ConnectionMethod.SASToken;
        _connection.SASToken = "";
        var ex = Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.UploadBlob(_input, _connection, _options, default));
        Assert.AreEqual("Destination.SASToken and Destination.URI parameters can't be empty when Destination.ConnectionMethod = SASToken.", ex.InnerException.Message);
    }

    [Test]
    public async Task UploadBlob_WontThrowWithOption()
    {
        _options.ThrowErrorOnFailure = false;
        _connection.ConnectionMethod = ConnectionMethod.ConnectionString;
        _connection.ContainerName = "";
        var result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
        Assert.IsFalse(result.Success);

        _input.SourceType = UploadSourceType.Directory;
        result = await AzureBlobStorage.UploadBlob(_input, _connection, _options, default);
        Assert.IsFalse(result.Success);
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

    private async Task DeleteBlobsInContainer(string connectionString, string containerName, string blobName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobContainer = blobServiceClient.GetBlobContainerClient(containerName);
        var blobList = blobContainer.GetBlobsAsync();
        await foreach (var item in blobList)
            await blobContainer.DeleteBlobIfExistsAsync(item.Name);
    }

    private async Task<bool> BlobExists(string containerName, string blobName, string expected)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        if (!blob.Exists())
            return false;

        var blobClient = new BlobClient(_connectionString, _connection.ContainerName, blobName);
        var blobDownload = await blobClient.DownloadAsync();

        using var reader = new StreamReader(blobDownload.Value.Content);
        var content = await reader.ReadToEndAsync();
        return content == expected;
    }
}