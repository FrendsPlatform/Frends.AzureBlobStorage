using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.AzureBlobStorage.UploadBlob
{
    [TestClass]
    public class UploadTest
    {
        /// <summary>
        /// Connection string for Azure Storage Emulator.
        /// </summary>
        private readonly string _connectionString = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");

        /// <summary>
        /// Container name for tests.
        /// </summary>
        private string _containerName;

        /// <summary>
        /// Some random file for test purposes.
        /// </summary>
        private readonly string _testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "TestFile.xml");

        [TestInitialize]
        public void TestSetup()
        {
            // Generate unique container name to avoid conflicts when running multiple tests.
            _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            // Delete whole container after running tests.
            var container = GetBlobContainer(_connectionString, _containerName);
            await container.DeleteIfExistsAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UploadFileAsync_ShouldThrowArgumentExceptionIfFileWasNotFound()
        {
            await AzureBlobStorage.UploadBlob(
                new Source {SourceFile = "NonExistingFile"},
                new Destination(),
                new CancellationToken());
        }

        [TestMethod]
        public async Task UploadFileAsync_ShouldUploadFileAsBlockBlob()
        {
            var input = new Source
            {
                SourceFile = _testFilePath
            };
            var options = new Destination
            {
                ContainerName = _containerName,
                BlobType = AzureBlobType.Block,
                ParallelOperations = 24,
                ConnectionString = _connectionString,
                Overwrite = true,
                CreateContainerIfItDoesNotExist = true,
                FileEncoding = "utf-8"
            };
            var container = GetBlobContainer(_connectionString, _containerName);

            var result = await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
            var blobResult = container.GetBlobClient("TestFile.xml");

            StringAssert.EndsWith(result.Uri, $"{_containerName}/TestFile.xml");
            Assert.IsTrue(blobResult.Exists(), "Uploaded TestFile.xml blob should exist");
        }

        [TestMethod]
        public async Task UploadFileAsync_ShouldRenameFileToBlob()
        {
            var input = new Source
            {
                SourceFile = _testFilePath
            };
            var options = new Destination
            {
                RenameTo = "RenamedFile.xml",
                ContainerName = _containerName,
                BlobType = AzureBlobType.Block,
                ParallelOperations = 24,
                ConnectionString = _connectionString,
                Overwrite = true,
                CreateContainerIfItDoesNotExist = true,
                FileEncoding = "utf-8"
            };

            var result = await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());

            StringAssert.EndsWith(result.Uri, $"{_containerName}/RenamedFile.xml");
        }

        [TestMethod]
        public async Task UploadFileAsync_ShouldUploadCompressedFile()
        {
            var input = new Source
            {
                SourceFile = _testFilePath,
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
                Overwrite = false,
                CreateContainerIfItDoesNotExist = true,
                ContentType = "text/xml",
                FileEncoding = "utf8",
                RenameTo = renameTo
            };
            var container = GetBlobContainer(_connectionString, _containerName);

            await AzureBlobStorage.UploadBlob(input, options, new CancellationToken());
            var blobResult = container.GetBlobClient(renameTo);

            Assert.IsTrue(blobResult.Exists(), "Uploaded TestFile.xml blob should exist");
        }

        [TestMethod]
        public async Task UploadFileAsync_ContentTypeIsForcedProperly()
        {
            var input = new Source
            {
                SourceFile = _testFilePath,
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
                Overwrite = false,
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
                SourceFile = _testFilePath,
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
                Overwrite = false,
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

        #region HelperMethods

        private static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
        {
            // Initialize azure account.
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Fetch the container client.
            return blobServiceClient.GetBlobContainerClient(containerName);
        }

        #endregion
    }
}
