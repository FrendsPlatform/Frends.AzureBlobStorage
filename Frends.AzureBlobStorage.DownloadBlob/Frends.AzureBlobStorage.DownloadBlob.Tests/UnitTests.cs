using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.Storage.Blobs.Models;

namespace Frends.AzureBlobStorage.DownloadBlob.Tests
{
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        ///     Connection string for Azure Storage.
        /// </summary>
        private readonly string _connectionString = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");

        /// <summary>
        ///     Some random file for test purposes.
        /// </summary>
        private readonly string _testBlob = "test-blob.txt";

        private CancellationToken _cancellationToken;

        /// <summary>
        ///     Container name for tests.
        /// </summary>
        private string _containerName;

        private DestinationFileProperties _destination;

        private string _destinationDirectory;

        private SourceProperties _source;

        /// <summary>
        ///     Some random file for test purposes.
        /// </summary>
        private readonly string _testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "TestFile.xml");

        [TestInitialize]
        public async Task TestSetup()
        {
            _destinationDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_destinationDirectory);

            // Generate unique container name to avoid conflicts when running multiple tests.
            _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

            // Task properties.
            _source = new SourceProperties
            {
                ConnectionString = _connectionString,
                BlobName = _testBlob,
                ContainerName = _containerName,
                Encoding = "utf-8"
            };
            _destination = new DestinationFileProperties
            {
                Directory = _destinationDirectory,
                FileExistsOperation = FileExistsAction.Overwrite
            };
            _cancellationToken = new CancellationToken();


            // Setup test material for download tasks.

            var container = Utils.GetBlobContainer(_connectionString, _containerName);
            var success = await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, _cancellationToken);

            if (success is null) throw new Exception("Could no create blob container");

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlobClient(_testBlob);

            await blockBlob.UploadAsync(_testFilePath, _cancellationToken);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            // Delete whole container after running tests.
            var container = Utils.GetBlobContainer(_connectionString, _containerName);
            await container.DeleteIfExistsAsync(null, _cancellationToken);

            // Delete test files and folders.
            if (Directory.Exists(_destinationDirectory)) Directory.Delete(_destinationDirectory, true);
        }

        [TestMethod]
        public async Task DownloadBlobAsync_WritesBlobToFile()
        {
            var result = await AzureBlobStorage.DownloadBlob(_source, _destination, _cancellationToken);
            Assert.IsTrue(File.Exists(result.FullPath));
            var fileContent = File.ReadAllText(result.FullPath);
            Assert.IsTrue(fileContent.Contains(@"<input>WhatHasBeenSeenCannotBeUnseen</input>"));
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public async Task DownloadBlobAsync_ThrowsExceptionIfDestinationFileExists()
        {
            await AzureBlobStorage.DownloadBlob(_source, _destination, _cancellationToken);
            _destination.FileExistsOperation = FileExistsAction.Error;
            await AzureBlobStorage.DownloadBlob(_source, _destination, _cancellationToken);
        }

        [TestMethod]
        public async Task DownloadBlobAsync_RenamesFileIfExists()
        {
            await AzureBlobStorage.DownloadBlob(_source, _destination, _cancellationToken);
            _destination.FileExistsOperation = FileExistsAction.Rename;
            var result = await AzureBlobStorage.DownloadBlob(_source, _destination, _cancellationToken);
            Assert.AreEqual("test-blob(1).txt", result.FileName);
        }

        [TestMethod]
        public async Task DownloadBlobAsync_OverwritesFileIfExists()
        {
            // Download file with same name couple of time.
            _destination.FileExistsOperation = FileExistsAction.Overwrite;
            await AzureBlobStorage.DownloadBlob(_source, _destination, _cancellationToken);
            await AzureBlobStorage.DownloadBlob(_source, _destination, _cancellationToken);
            await AzureBlobStorage.DownloadBlob(_source, _destination, _cancellationToken);

            // Only one file should exist in destination folder.
            Assert.AreEqual(1, Directory.GetFiles(_destinationDirectory).Length);
        }
    }
}