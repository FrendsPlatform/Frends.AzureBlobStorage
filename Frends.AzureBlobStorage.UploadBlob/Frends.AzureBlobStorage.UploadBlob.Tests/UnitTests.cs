using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.UploadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.AzureBlobStorage.UploadBlob.Tests;

[TestClass]
public class UploadTest
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");
    private string _containerName;
    private readonly string _testFileDir = Path.Combine(Environment.CurrentDirectory, "TestFiles");
    private readonly string _downloadDir = Path.Combine(Environment.CurrentDirectory, "TestFiles", "Downloads");
    private readonly string _firstTestFilePath = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile.txt");
    private readonly string _secondTestFilePath = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile2.txt");

    [TestInitialize]
    public void TestSetup()
    {
        CreateFiles();
        // Generate unique container name to avoid conflicts when running multiple tests.
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        var container = GetBlobContainer(_connectionString, _containerName);
        await container.DeleteIfExistsAsync();
        DeleteFiles();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task UploadFileAsync_ShouldThrowArgumentExceptionIfFileWasNotFound()
    {
        await AzureBlobStorage.UploadBlob( new Source { SourceFile = "NonExistingFile" }, new Destination(), new CancellationToken());
    }

    [TestMethod]
    public async Task UploadFileAsync_ShouldUploadFileAsBlockBlob()
    {
        var input = new Source { SourceFile = _firstTestFilePath };

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
        };
        var container = GetBlobContainer(_connectionString, _containerName);
        var result = await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
        var blobResult = container.GetBlobClient("testfile.txt");

        StringAssert.EndsWith(result.Uri, $"{_containerName}/testfile.txt");
        Assert.IsTrue(blobResult.Exists(), "Uploaded testfile.txt blob should exist");
    }

    [TestMethod]
    public async Task UploadFileAsync_ShouldUploadFileAsAppendBlob()
    {
        var input = new Source { SourceFile = _firstTestFilePath };

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Append,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            HandleExistingFile = HandleExistingFile.Overwrite,
            FileEncoding = "utf-8"
        };
        var container = GetBlobContainer(_connectionString, _containerName);
        var result = await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
        var blobResult = container.GetBlobClient("testfile.txt");

        StringAssert.EndsWith(result.Uri, $"{_containerName}/testfile.txt");
        Assert.IsTrue(blobResult.Exists(), "Uploaded testfile.txt blob should exist");
    }

    // Calculate page's size and upload page blob. More accurate file check in append test(s).
    [TestMethod]
    public async Task UploadFileAsync_ShouldUploadFileAsPageCalculatePageSizeBlob()
    {
        var filename = Path.GetFileName(_firstTestFilePath);

        var input = new Source { SourceFile = _firstTestFilePath };

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Page,
            ParallelOperations = 24,
            ConnectionString = _connectionString,
            HandleExistingFile = HandleExistingFile.Error,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            BlobName = filename,
            PageMaxSize = 0,
            PageOffset = 0
        };
        var container = GetBlobContainer(_connectionString, _containerName);
        var result = await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
        var blobResult = container.GetBlobClient(filename);

        StringAssert.EndsWith(result.Uri, $"{_containerName}/{filename}");
        Assert.IsTrue(blobResult.Exists(), $"Uploaded {filename} blob should exist");
    }

    // Upload page blob. Calculate file's size and offset to end. More accurate file check in append test(s).
    [TestMethod]
    public async Task UploadFileAsync_ShouldUploadFileAsPageOffsetToEndBlob()
    {
        var filename = Path.GetFileName(_firstTestFilePath);

        var input = new Source { SourceFile = _firstTestFilePath };

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Page,
            ParallelOperations = 24,
            ConnectionString = _connectionString,
            HandleExistingFile = HandleExistingFile.Error,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            BlobName = filename,
            PageMaxSize = 2048,
            PageOffset = -1,
        };
        var container = GetBlobContainer(_connectionString, _containerName);
        var result = await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
        var blobResult = container.GetBlobClient(filename);

        StringAssert.EndsWith(result.Uri, $"{_containerName}/{filename}");
        Assert.IsTrue(blobResult.Exists(), $"Uploaded {filename} blob should exist");
    }

    [TestMethod]
    public async Task UploadFileAsync_ShouldRenameFileToBlob()
    {
        var input = new Source { SourceFile = _firstTestFilePath };
        var options = new Destination
        {
            RenameTo = "RenamedFile.txt",
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ParallelOperations = 24,
            ConnectionString = _connectionString,
            HandleExistingFile = HandleExistingFile.Overwrite,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8"
        };

        var result = await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
        StringAssert.EndsWith(result.Uri, $"{_containerName}/RenamedFile.txt");
    }

    [TestMethod]
    public async Task UploadFileAsync_ShouldUploadCompressedFile()
    {
        var input = new Source
        {
            SourceFile = _firstTestFilePath,
            Compress = true,
            ContentsOnly = true
        };

        var guid = Guid.NewGuid().ToString();
        var renameTo = guid + ".gz";

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ParallelOperations = 24,
            ConnectionString = _connectionString,
            HandleExistingFile = HandleExistingFile.Error,
            CreateContainerIfItDoesNotExist = true,
            ContentType = "text/xml",
            FileEncoding = "utf8",
            RenameTo = renameTo
        };
        var container = GetBlobContainer(_connectionString, _containerName);

        await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
        var blobResult = container.GetBlobClient(renameTo);

        Assert.IsTrue(blobResult.Exists(), $"Uploaded {Path.GetFileName(_firstTestFilePath)} blob should exist");
    }

    [TestMethod]
    public async Task UploadFileAsync_ContentTypeIsForcedProperly()
    {
        var input = new Source
        {
            SourceFile = _firstTestFilePath,
            Compress = false,
            ContentsOnly = false
        };

        var guid = Guid.NewGuid().ToString();
        var renameTo = guid + ".gz";

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ParallelOperations = 24,
            ConnectionString = _connectionString,
            HandleExistingFile = HandleExistingFile.Error,
            CreateContainerIfItDoesNotExist = true,
            ContentType = "foo/bar",
            FileEncoding = "utf8",
            RenameTo = renameTo
        };
        var container = GetBlobContainer(_connectionString, _containerName);

        await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
        var blobResult = container.GetBlobClient(renameTo);
        var properties = await blobResult.GetPropertiesAsync(null, new CancellationToken());

        Assert.IsTrue(properties.Value.ContentType == "foo/bar");
    }

    [TestMethod]
    public async Task UploadFileAsync_ContentEncodingIsGzipWhenCompressed()
    {
        var input = new Source
        {
            SourceFile = _firstTestFilePath,
            Compress = true,
            ContentsOnly = true
        };

        var guid = Guid.NewGuid().ToString();
        var renameTo = guid + ".gz";

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ParallelOperations = 24,
            ConnectionString = _connectionString,
            HandleExistingFile = HandleExistingFile.Error,
            CreateContainerIfItDoesNotExist = true,
            ContentType = "foo/bar",
            FileEncoding = "utf8",
            RenameTo = renameTo
        };
        var container = GetBlobContainer(_connectionString, _containerName);

        await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
        var blobResult = container.GetBlobClient(renameTo);
        var properties = await blobResult.GetPropertiesAsync(null, new CancellationToken());

        Assert.IsTrue(properties.Value.ContentEncoding == "gzip");
    }

    /// <summary>
    /// Actions: Upload test file to container (create Block blob) -> Download test file (Block blob) to Downloads-folder -> Append Block blob with Source file -> Upload Block blob "back to" container -> Download Block blob to make sure append was done.
    /// </summary>
    [TestMethod]
    public async Task UploadFileAsync_AppendBlockBlob()
    {
        //Upload parameters for first file.
        var input = new Source { SourceFile = _firstTestFilePath };

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf8",
            HandleExistingFile = HandleExistingFile.Error,
        };

        //Upload parameters for appending.
        var input2 = new Source { SourceFile = _secondTestFilePath };

        var options2 = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = false,
            FileEncoding = "utf8",
            HandleExistingFile = HandleExistingFile.Append,
            BlobName = Path.GetFileName(_firstTestFilePath)
        };

        var reader = File.ReadAllText(_firstTestFilePath).ToLower();
        if (reader.Contains("Ut et maximus nibh. Etiam dui") && !reader.Contains("Mauris quis sapien non ligula maximus eget"))
        {
            //Upload first file
            await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());

            //Handle appending
            await AzureBlobStorage.UploadBlob(input2, options2, new CancellationToken());

            //Download appended file (blob) and check if it was appended with file2
            await DownloadBlob(_connectionString, _containerName, Path.GetFileName(_firstTestFilePath), _downloadDir);

            var reader2 = File.ReadAllText(Path.Combine(_downloadDir, Path.GetFileName(_firstTestFilePath)));
            Assert.IsTrue(reader2.Contains("Ut et maximus nibh. Etiam dui") && reader2.Contains("Mauris quis sapien non ligula maximus eget"));
        }
    }

    /// <summary>
    /// Actions: Upload test file to container (create Page blob) -> Download test file (Page blob) to Downloads-folder -> Append Page blob with Source file -> Upload Page blob "back to" container -> Delete appended file from Downloads-folder -> Download Page blob to make sure append was done.
    /// </summary>
    [TestMethod]
    public async Task UploadFileAsync_AppendPageBlob()
    {
        //Upload parameters for first file.
        var input = new Source { SourceFile = _firstTestFilePath };

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Page,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf8",
            PageMaxSize = 512,
            PageOffset = 0,
            HandleExistingFile = HandleExistingFile.Error,
        };

        //Upload parameters for appending.
        var input2 = new Source { SourceFile = _secondTestFilePath };

        var options2 = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Page,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = false,
            FileEncoding = "utf8",
            HandleExistingFile = HandleExistingFile.Append,
            BlobName = Path.GetFileName(_firstTestFilePath),
            PageMaxSize = 0,
            PageOffset = -1,
        };

        var reader = File.ReadAllText(_firstTestFilePath);
        if (reader.Contains("Ut et maximus nibh. Etiam dui") && !reader.Contains("Mauris quis sapien non ligula maximus eget"))
        {
            //Upload first file
            await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());

            //Handle appending
            await AzureBlobStorage.UploadBlob(input2, options2, new CancellationToken());

            //Download appended file (blob) and check if it was appended with file2
            await DownloadBlob(_connectionString, _containerName, Path.GetFileName(_firstTestFilePath), _downloadDir);
            var reader2 = File.ReadAllText(Path.Combine(_downloadDir, Path.GetFileName(_firstTestFilePath)));

            Assert.IsTrue(reader2.Contains("Ut et maximus nibh. Etiam dui") && reader2.Contains("Mauris quis sapien non ligula maximus eget"));
        }
    }

    /// <summary>
    /// Actions: Append given blob without downloading -> Download Append blob to make sure append was done.
    /// </summary>
    [TestMethod]
    public async Task UploadFileAsync_AppendAppendBlob()
    {
        //Upload parameters for first file.
        var input = new Source { SourceFile = _firstTestFilePath };

        var options = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Append,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf8",
            PageMaxSize = 512,
            PageOffset = 0,
            HandleExistingFile = HandleExistingFile.Error,
        };

        //Upload parameters for appending.
        var input2 = new Source { SourceFile = _secondTestFilePath };

        var options2 = new Destination
        {
            ContainerName = _containerName,
            BlobType = AzureBlobType.Append,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = false,
            FileEncoding = "utf8",
            HandleExistingFile = HandleExistingFile.Append,
            BlobName = Path.GetFileName(_firstTestFilePath),
        };

        var reader = File.ReadAllText(_firstTestFilePath);
        if (reader.Contains("Ut et maximus nibh. Etiam dui") && !reader.Contains("Mauris quis sapien non ligula maximus eget"))
        {
            //Upload first file
            await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());

            //Handle appending
            await AzureBlobStorage.UploadBlob(input2, options2, new CancellationToken());

            //Download appended file (blob) and check if it was appended with file2
            Directory.CreateDirectory(_downloadDir);
            await DownloadBlob(_connectionString, _containerName, Path.GetFileName(_firstTestFilePath), _downloadDir);
            var reader2 = File.ReadAllText(Path.Combine(_downloadDir, Path.GetFileName(_firstTestFilePath)));

            Assert.IsTrue(reader2.Contains("Ut et maximus nibh. Etiam dui") && reader2.Contains("Mauris quis sapien non ligula maximus eget"));
        }
    }

    private static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
    {
        // Initialize azure account.
        var blobServiceClient = new BlobServiceClient(connectionString);

        // Fetch the container client.
        return blobServiceClient.GetBlobContainerClient(containerName);
    }

    private static async Task<bool> DownloadBlob(string connectionString, string containerName, string blobName, string downloadPath)
    {
        try
        {
            var blob = new BlobClient(connectionString, containerName, blobName);
            await blob.DownloadToAsync(Path.Combine(downloadPath, blobName));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Some tests requires 1-2 files that are at least size of 512 bytes.
    /// </summary>
    /// <returns></returns>
    private bool CreateFiles()
    {
        Directory.CreateDirectory(_downloadDir);

        #region 512 byte files
        File.WriteAllText(_firstTestFilePath, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque non sem quis orci rutrum hendrerit. Fusce ultricies cursus ante nec bibendum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam libero massa, viverra id suscipit in, tincidunt sit amet urna. Vestibulum gravida a massa eu molestie. Phasellus volutpat neque vitae enim molestie, vitae pharetra massa varius. Phasellus ante nulla, faucibus nec tristique eu, dignissim quis magna. Sed vitae sodales ipsum. Ut et maximus nibh. Etiam dui.");

        File.WriteAllText(_secondTestFilePath, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla mollis neque nibh, molestie consequat lacus euismod in. Phasellus in libero id velit sollicitudin rhoncus. Sed a nunc non lacus hendrerit iaculis. Suspendisse quis dui id enim sollicitudin rhoncus. Phasellus convallis lacinia finibus. Sed quis purus vitae felis finibus facilisis at quis nisi. Integer pharetra, ex egestas iaculis ultricies, tortor neque hendrerit justo, eu vulputate odio eros sed augue. Mauris quis sapien non ligula maximus eget.");
        #endregion 512 byte files

        return true;
    }

    private void DeleteFiles()
    {
        try
        {
            if (File.Exists(_firstTestFilePath))
                File.Delete(_firstTestFilePath);

            if (File.Exists(_secondTestFilePath))
                File.Delete(_secondTestFilePath);

            if (File.Exists(Path.Combine(_downloadDir, Path.GetFileName(_firstTestFilePath))))
                File.Delete(Path.Combine(_downloadDir, Path.GetFileName(_firstTestFilePath)));

            Directory.Delete(_testFileDir, true);
        }
        catch (Exception)
        {

            throw;
        }
    }
}