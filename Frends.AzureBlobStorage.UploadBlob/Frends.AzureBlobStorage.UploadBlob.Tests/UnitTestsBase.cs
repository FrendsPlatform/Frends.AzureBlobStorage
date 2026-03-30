using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Frends.AzureBlobStorage.UploadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.UploadBlob.Tests;

public abstract class UnitTestsBase
{
	internal readonly string _connstring = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
	internal readonly string _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";


	internal readonly string _workingDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
	internal readonly List<string> _testFiles =
	[
		"TestFile1.txt",
		"TestFile2.txt",
		"Temp\\SubFolderFile1.txt",
		"Temp\\SubFolderFile2.txt",
		"512byte\\TestFile1.txt",
		"512byte\\TestFile2.txt"
	];

	internal string _testFileDir;
	internal string _destinationDirectory;

	internal readonly Tag[] _tags = [new Tag { Name = "TagName", Value = "TagValue" }];
	internal readonly List<AzureBlobType> _blobtypes = [AzureBlobType.Block, AzureBlobType.Page, AzureBlobType.Append];

	internal Connection _connection;
	internal Options _options;

	protected async Task InitBase()
	{
		await CreateContainer(_containerName);
		CreateWorkingDirectory();
		await CreateAndUploadTestFiles(true);

		_options = new Options
		{
			ThrowErrorOnFailure = true,
			BlobType = default,
			ResizeFile = default,
			PageMaxSize = default,
			PageOffset = default,
			ContentType = null,
			Encoding = FileEncoding.UTF8,
			EnableBom = false,
			ParallelOperations = default
		};
	}

	[TestCleanup]
	public async Task CleanUp()
	{
		await DeleteContainer(_containerName);
		DeleteWorkingDirectory();
	}

	[TestMethod]
	[DataRow(AzureBlobType.Block)]
	[DataRow(AzureBlobType.Append)]
	[DataRow(AzureBlobType.Page)]
	public async Task UploadFile_SimpleUpload(AzureBlobType blobType)
	{
		// Setup
		var container = GetBlobContainer(_connstring, _containerName);
		var blobName = $"{Guid.NewGuid()}/{blobType}/TestFile1.txt";
		var input = new Input
		{
			BlobName = blobName,
			SourceType = UploadSourceType.File,
			SourceFile = GetSourceFile(_testFiles[4]),
			Tags = null,
			Compress = default,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = default,
			ActionOnExistingFile = OnExistingFile.Overwrite,
		};

		_options.BlobType = blobType;
		
		// Acte
		var result = await AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken);

		// Assert
		Assert.IsTrue(result.Success);
		Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{blobName}"));
		Assert.IsTrue(await container.GetBlobClient(blobName).ExistsAsync(default), $"Uploaded {blobName} blob should exist");
	}

	[TestMethod]
	[DataRow(AzureBlobType.Block)]
	[DataRow(AzureBlobType.Append)]
	[DataRow(AzureBlobType.Page)]
	public async Task UploadBlob_ContentsOnly(AzureBlobType blobType)
	{
		// Setup
		var container = GetBlobContainer(_connstring, _containerName);
		var blobName = $"{Guid.NewGuid()}/{blobType}/TestFile1.txt";
		var input = new Input
		{
			BlobName = blobName,
			SourceType = UploadSourceType.File,
			SourceFile = GetSourceFile(_testFiles[4]),
			Tags = null,
			Compress = default,
			ContentsOnly = true,
			SearchPattern = default,
			SourceDirectory = default,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		_options.BlobType = blobType;

		// Act
		var result = await AzureBlobStorage.UploadBlob(input, _connection, _options, default);

		// Assert
		Assert.IsTrue(result.Success);
		Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{blobName}"));
		Assert.IsTrue(await container.GetBlobClient(blobName).ExistsAsync(), $"Uploaded {blobName} blob should exist");
	}

	[TestMethod]
	[DataRow(AzureBlobType.Block)]
	[DataRow(AzureBlobType.Append)]
	[DataRow(AzureBlobType.Page)]
	public async Task UploadFile_WithTags(AzureBlobType blobType)
	{
		// Setup
		var container = GetBlobContainer(_connstring, _containerName);
		var blobName = $"{Guid.NewGuid()}/{blobType}/TestFile1.txt";
		var input = new Input
		{
			BlobName = blobName,
			SourceType = UploadSourceType.File,
			SourceFile = GetSourceFile(_testFiles[4]),
			Tags = _tags,
			Compress = default,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = default,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		_options.BlobType = blobType;

		// Act
		var result = await AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken);

		// Assert
		Assert.IsTrue(result.Success);
		Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{blobName}"));

		var blobClient = container.GetBlobClient(blobName);
		Assert.IsTrue(await blobClient.ExistsAsync(TestContext.CancellationToken), $"Uploaded {blobName} blob should exist");
		blobClient.GetTagsAsync(cancellationToken: TestContext.CancellationToken).Result.Value.Tags.TryGetValue("TagName", out var tagValue);
		Assert.AreEqual("TagValue", tagValue, "Blob should have the correct tag value");
	}

	[TestMethod]
	[DataRow(AzureBlobType.Block)]
	[DataRow(AzureBlobType.Append)]
	[DataRow(AzureBlobType.Page)]
	public async Task UploadFile_SetBlobName_ForceEncoding_ForceContentType(AzureBlobType blobType)
	{
		// Setup
		var container = GetBlobContainer(_connstring, _containerName);
		var blobName = $"{Guid.NewGuid()}/{blobType}/TestFile1.txt";
		var input = new Input
		{
			BlobName = blobName,
			SourceType = UploadSourceType.File,
			SourceFile = GetSourceFile(_testFiles[4]),
			Tags = null,
			Compress = default,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = default,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		_options.BlobType = blobType;
		_options.ContentType = "text/xml";

		// Act
		var result = await AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken);

		// Assert
		Assert.IsTrue(result.Success);
		Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{blobName}"));
		var blobClient = container.GetBlobClient(blobName);
		Assert.IsTrue(await blobClient.ExistsAsync(TestContext.CancellationToken), "Uploaded blob should exist");
		var properties = await blobClient.GetPropertiesAsync(cancellationToken: TestContext.CancellationToken);
		Assert.AreEqual("text/xml", properties.Value.ContentType, "Uploaded blob should have content type text/xml");
	}

	[TestMethod]
	[DataRow(AzureBlobType.Block)]
	[DataRow(AzureBlobType.Append)]
	[DataRow(AzureBlobType.Page)]
	public async Task UploadFile_Compress(AzureBlobType blobType)
	{
		// Setup
		var container = GetBlobContainer(_connstring, _containerName);
		var blobName = $"{Guid.NewGuid()}/{blobType}/compress.gz";
		var input = new Input
		{
			BlobName = blobName,
			SourceType = UploadSourceType.File,
			SourceFile = GetSourceFile(_testFiles[4]),
			Tags = null,
			Compress = true,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = default,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		_options.BlobType = blobType;

		// Act
		var result = await AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken);

		// Assert
		Assert.IsTrue(result.Success);
		Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{blobName}"));
		var blobClient = container.GetBlobClient(blobName);
		Assert.IsTrue(await blobClient.ExistsAsync(TestContext.CancellationToken), $"Uploaded {blobName} blob should exist");

		if (blobType == AzureBlobType.Block)
		{
			var downloadedFilePath = Path.GetTempFileName();
			try
			{
				await blobClient.DownloadToAsync(downloadedFilePath, TestContext.CancellationToken);

				using var downloadedStream = File.OpenRead(downloadedFilePath);
				using var gzipStream = new GZipStream(downloadedStream, CompressionMode.Decompress);
				using var reader = new StreamReader(gzipStream);
				var decompressedContent = await reader.ReadToEndAsync(TestContext.CancellationToken);
				var originalContent = await File.ReadAllTextAsync(input.SourceFile, TestContext.CancellationToken);
				Assert.AreEqual(originalContent, decompressedContent, "Decompressed content should match original");
			}
			finally
			{
				if (File.Exists(downloadedFilePath)) File.Delete(downloadedFilePath);
			}
		}
	}

	[TestMethod]
	[DataRow(AzureBlobType.Block)]
	[DataRow(AzureBlobType.Append)]
	[DataRow(AzureBlobType.Page)]
	public async Task UploadFile_HandleExistingFile(AzureBlobType blobType)
	{
		// Setup
		var container = GetBlobContainer(_connstring, _connection.ContainerName);
		var errorHandlers = new List<OnExistingFile>() { OnExistingFile.Append, OnExistingFile.Overwrite, OnExistingFile.Throw };
		var blobName = $"{Guid.NewGuid()}/{blobType}/TestFile1.txt";
		var testInput = new Input
		{
			BlobName = blobName,
			SourceType = UploadSourceType.File,
			SourceFile = GetSourceFile(_testFiles[4]),
			Tags = null,
			Compress = default,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = default,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};
		_options.BlobType = blobType;
		var result = await AzureBlobStorage.UploadBlob(testInput, _connection, _options, default);
		Assert.IsTrue(result.Success);

		if (blobType is AzureBlobType.Page)
			_options.ResizeFile = true;

		// Assert & Act
		foreach (var handler in errorHandlers)
		{
			var input = new Input
			{
				BlobName = blobName,
				SourceType = UploadSourceType.File,
				SourceFile = handler is OnExistingFile.Append ? GetSourceFile(_testFiles[1]) : GetSourceFile(_testFiles[0]),
				Tags = null,
				Compress = default,
				ContentsOnly = default,
				SearchPattern = default,
				SourceDirectory = default,
				ActionOnExistingFile = handler
			};

			if (handler is OnExistingFile.Append)
			{
				result = await AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken);
				Assert.IsTrue(result.Success);
				Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{blobName}"));
				Assert.IsTrue(await container.GetBlobClient(blobName).ExistsAsync(), $"Uploaded {blobName} blob should exist");
			}
			else if (handler is OnExistingFile.Overwrite)
			{
				result = await AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken);
				Assert.IsTrue(result.Success);
				Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{blobName}"));
				Assert.IsTrue(await container.GetBlobClient(blobName).ExistsAsync(), $"Uploaded {blobName} blob should exist");
			}
			else
			{
				var ex = await Assert.ThrowsExactlyAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken));

				if (blobType != AzureBlobType.Page)
					Assert.IsTrue(ex.InnerException?.InnerException?.Message.Contains("already exists"));
				else
					Assert.IsTrue(ex.InnerException?.Message.Contains("already exists"));
			}
		}
	}

	[TestMethod]
	[DataRow(AzureBlobType.Block)]
	[DataRow(AzureBlobType.Append)]
	[DataRow(AzureBlobType.Page)]
	public async Task UploadDirectory_WithTags(AzureBlobType blobType)
	{
		// Setup
		var input = new Input
		{
			BlobName = default,
			SourceType = UploadSourceType.Directory,
			SourceFile = default,
			Tags = _tags,
			Compress = true,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = _testFileDir,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		if (blobType is AzureBlobType.Page)
			_options.ResizeFile = true;

		_options.BlobType = blobType;

		var resultWithTags = await AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken);
		Assert.IsTrue(resultWithTags.Success);
		await AssertAllFilesUploaded(resultWithTags.Data, _testFileDir);
	}

	[TestMethod]
	[DataRow(AzureBlobType.Block)]
	[DataRow(AzureBlobType.Append)]
	[DataRow(AzureBlobType.Page)]
	public async Task UploadDirectory_RenameDir(AzureBlobType blobType)
	{
		// Setup
		var container = GetBlobContainer(_connstring, _containerName);
		var input = new Input
		{
			BlobName = default,
			BlobFolderName = "RenameDir",
			SourceType = UploadSourceType.Directory,
			SourceFile = default,
			Tags = _tags,
			Compress = true,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = _testFileDir,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		_options.BlobType = blobType;
		if (blobType is AzureBlobType.Page)
			_options.ResizeFile = true;

		// Act
		var result = await AzureBlobStorage.UploadBlob(input, _connection, _options, default);
		
		// Assert
		Assert.IsTrue(result.Success);
		Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{input.BlobFolderName}/TestFile1.txt"));
		Assert.IsTrue(await container.GetBlobClient($"{input.BlobFolderName}/TestFile1.txt").ExistsAsync(TestContext.CancellationToken), "Uploaded TestFile1.txt blob should exist");
		Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/{input.BlobFolderName}/TestFile2.txt"));
		Assert.IsTrue(await container.GetBlobClient($"{input.BlobFolderName}/TestFile2.txt").ExistsAsync(TestContext.CancellationToken), "Uploaded TestFile2.txt blob should exist");
	}

	[TestMethod]
	public async Task UploadBlob_TestEncoding()
	{
		// Setup
		var container = GetBlobContainer(_connstring, _containerName);
		var blobName = $"{Guid.NewGuid()}/TestFile1.txt";
		var input = new Input
		{
			BlobName = blobName,
			SourceType = UploadSourceType.File,
			SourceFile = GetSourceFile(_testFiles[0]),
			Tags = null,
			Compress = default,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = default,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};
		var encodings = new List<FileEncoding>()
		{
			FileEncoding.UTF8,
			FileEncoding.Default,
			FileEncoding.ASCII,
			FileEncoding.Windows1252,
			FileEncoding.Other
		};

		var expected = File.ReadAllText(input.SourceFile);
		_options.FileEncodingString = "windows-1251";

		// Act & Assert
		foreach (var encoding in encodings)
		{
			_options.Encoding = encoding;
			var result = await AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken);
			Assert.IsTrue(result.Success, $"Encoding: {encoding}");
			Assert.IsTrue(await BlobExists(_connection.ContainerName, blobName, expected));
		}
	}

	[TestMethod]
	public async Task UploadBlob_ErrorUploadTypeDirectoryNoDirectory()
	{
		// Setup
		var input = new Input
		{
			SourceType = UploadSourceType.Directory,
			SourceDirectory = "",
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		// Act
		var ex = await Assert.ThrowsExactlyAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken));

		// Assert
		Assert.Contains("An exception occured while uploading directory", ex.Message);
	}

	[TestMethod]
	public async Task UploadBlob_ErrorUploadTypeDirectoryDirectoryDoesNotExist()
	{
		// Setup
		var input = new Input
		{
			SourceType = UploadSourceType.Directory,
			SourceDirectory = @"C:\\doesnt\\exist",
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		// Act
		var ex = await Assert.ThrowsExactlyAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken));

		// Assert
		Assert.Contains("An exception occured while uploading directory", ex.Message);
	}

	[TestMethod]
	public async Task UploadBlob_ErrorNoSourceFile()
	{
		// Setup
		var input = new Input
		{
			SourceType = UploadSourceType.File,
			SourceFile = "",
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		// Act
		var ex = await Assert.ThrowsExactlyAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken));

		// Assert
		Assert.Contains("An exception occured.", ex.Message);
	}

	[TestMethod]
	public async Task UploadBlob_ErrorSourceFileNotExist()
	{
		// Setup
		var input = new Input
		{
			SourceType = UploadSourceType.File,
			SourceFile = "doesntexists.txt",
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		// Act
		var ex = await Assert.ThrowsExactlyAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken));

		// Assert
		Assert.Contains("An exception occured.", ex.Message);
	}

	[TestMethod]
	public async Task UploadBlob_ErrorEmptyContainerName()
	{
		// Setup
		_connection.ContainerName = "";

		var input = new Input
		{
			SourceType = UploadSourceType.File,
			SourceFile = "",
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		// Act
		var ex = await Assert.ThrowsExactlyAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, _connection, _options, TestContext.CancellationToken));

		// Assert
		Assert.Contains("An exception occured.", ex.Message);
	}


	[TestMethod]
	public async Task UploadBlob_WontThrowWithOption()
	{
		// Setup
		_options.ThrowErrorOnFailure = false;
		_connection.ContainerName = "";
		var input = new Input
		{
			SourceType = UploadSourceType.File,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		var result = await AzureBlobStorage.UploadBlob(input, _connection, _options, default);
		Assert.IsFalse(result.Success);

		input.SourceType = UploadSourceType.Directory;
		result = await AzureBlobStorage.UploadBlob(input, _connection, _options, default);
		Assert.IsFalse(result.Success);
	}

	[TestMethod]
	[DataRow(10 * 1024L, "small_no_compress.dat")]
	[DataRow(200L * 1024 * 1024, "large_no_compress.dat")]
	public async Task UploadDownload_NoCompression_VerifyIntegrity(long fileSize, string fileName)
	{
		// Setup
		var tempDir = Path.GetTempPath();
		var originalFile = Path.Combine(tempDir, "original_" + fileName);
		var downloadedFile = Path.Combine(tempDir, "downloaded_" + fileName);

		try
		{
			await GenerateRandomFile(originalFile, fileSize);
			var originalHash = await CalculateSHA256(originalFile);
			var originalSize = new FileInfo(originalFile).Length;

			var input = new Input
			{
				SourceType = UploadSourceType.File,
				SourceFile = originalFile,
				BlobName = fileName,
				Compress = false,
				ContentsOnly = false,
				ActionOnExistingFile = OnExistingFile.Overwrite
			};

			var options = new Options
			{
				ThrowErrorOnFailure = true,
				BlobType = AzureBlobType.Block,
				ResizeFile = default,
				PageMaxSize = default,
				PageOffset = default,
				ContentType = null,
				Encoding = FileEncoding.UTF8,
				EnableBom = false,
				ParallelOperations = default
			};

			// Act
			var uploadResult = await AzureBlobStorage.UploadBlob(input, _connection, options, TestContext.CancellationToken);

			// Assert
			Assert.IsTrue(uploadResult.Success, "Upload failed");

			await DownloadBlobSimple(_connstring, _containerName, fileName, downloadedFile, TestContext.CancellationToken);
			var downloadedSize = new FileInfo(downloadedFile).Length;
			var downloadedHash = await CalculateSHA256(downloadedFile);

			Assert.AreEqual(originalSize, downloadedSize, $"File size changed: {originalSize} -> {downloadedSize}");
			Assert.AreEqual(BitConverter.ToString(originalHash), BitConverter.ToString(downloadedHash), "File content corrupted");
		}
		finally
		{
			try { if (File.Exists(originalFile)) File.Delete(originalFile); } catch { }
			try { if (File.Exists(downloadedFile)) File.Delete(downloadedFile); } catch { }
		}
	}

	[TestMethod]
	[DataRow(10 * 1024L, "small_with_compress.dat", true)]
	[DataRow(10 * 1024L, "small_with_compress.dat", false)]
	[DataRow(200L * 1024 * 1024, "large_with_compress.dat", true)]
	//[DataRow(200L * 1024 * 1024, "large_with_compress.dat", false)]
	//[DataRow(3L * 1024 * 1024 * 1024, "very_large_3gb_file.dat", false)]
	public async Task UploadDownload_WithCompression_VerifyIntegrity(long fileSize, string fileName, bool contentsOnly)
	{
		var tempDir = Path.GetTempPath();
		var originalFile = Path.Combine(tempDir, "original_" + fileName);
		var downloadedFile = Path.Combine(tempDir, "downloaded_" + fileName);
		var decompressedFile = downloadedFile + ".decompressed";

		try
		{
			await GenerateRandomFile(originalFile, fileSize);
			var originalHash = await CalculateSHA256(originalFile);

			var input = new Input
			{
				SourceType = UploadSourceType.File,
				SourceFile = originalFile,
				BlobName = fileName,
				Compress = true,
				ContentsOnly = contentsOnly,
				ActionOnExistingFile = OnExistingFile.Overwrite
			};

			var options = new Options
			{
				ThrowErrorOnFailure = true,
				BlobType = AzureBlobType.Block,
				ResizeFile = default,
				PageMaxSize = default,
				PageOffset = default,
				ContentType = "",
				Encoding = FileEncoding.UTF8,
				EnableBom = true,
				ParallelOperations = 4
			};

			var uploadResult = await AzureBlobStorage.UploadBlob(input, _connection, options, default);
			Assert.IsTrue(uploadResult.Success, "Upload failed");

			await DownloadBlobSimple(_connstring, _containerName, fileName, downloadedFile, TestContext.CancellationToken);

			using (var inputStream = File.OpenRead(downloadedFile))
			using (var gzip = new GZipStream(inputStream, CompressionMode.Decompress))
			using (var outFile = File.Create(decompressedFile))
			{
				await gzip.CopyToAsync(outFile);
			}

			var downloadedHash = await CalculateSHA256(decompressedFile);
			Assert.AreEqual(BitConverter.ToString(originalHash), BitConverter.ToString(downloadedHash), "Downloaded content doesn't match original");
		}
		finally
		{
			try { if (File.Exists(originalFile)) File.Delete(originalFile); } catch { }
			try { if (File.Exists(downloadedFile)) File.Delete(downloadedFile); } catch { }
			try { if (File.Exists(decompressedFile)) File.Delete(decompressedFile); } catch { }
		}
	}


	#region Helper methods

	internal static Uri GetUri(string uri, string containerName) => new($"{uri}/{containerName}");

	internal async Task<BlobContainerClient> GetContainerClient(string containerName)
	{
		var blobServiceClient = new BlobServiceClient(_connstring);
		var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: TestContext.CancellationToken);
		return blobContainerClient;
	}

	internal async Task CreateContainer(string containerName)
	{
		var blobServiceClient = new BlobServiceClient(_connstring);
		var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: TestContext.CancellationToken);
	}

	internal async Task DeleteContainer(string containerName)
	{
		var blobServiceClient = new BlobServiceClient(_connstring);
		var container = blobServiceClient.GetBlobContainerClient(containerName);
		await container.DeleteIfExistsAsync(cancellationToken: TestContext.CancellationToken);
	}

	internal async Task<bool> ContainerExists(string containerName)
	{
		var blobServiceClient = new BlobServiceClient(_connstring);
		var container = blobServiceClient.GetBlobContainerClient(containerName);
		return await container.ExistsAsync(cancellationToken: TestContext.CancellationToken);
	}

	private void CreateWorkingDirectory()
	{
		_testFileDir = Path.Combine(_workingDir, "TestFiles");
		_destinationDirectory = Path.Combine(_workingDir, "Destination");

		if (!Directory.Exists(_workingDir))
		{
			Directory.CreateDirectory(_workingDir);
		}

		if (!Directory.Exists(_testFileDir))
		{
			Directory.CreateDirectory(_testFileDir);
		}

		if (!Directory.Exists(_destinationDirectory))
		{
			Directory.CreateDirectory(_destinationDirectory);
		}
	}

	private void DeleteWorkingDirectory()
	{
		if (Directory.Exists(_workingDir))
		{
			Directory.Delete(_workingDir, true);
		}
	}

	private async Task CreateAndUploadTestFiles(bool upload512ByteFiles = false)
	{
		CreateFiles(upload512ByteFiles);

		var filesToUpload = Directory.GetFiles(_testFileDir, "*", SearchOption.AllDirectories);
		foreach (var file in filesToUpload)
		{
			var relativePath = Path.GetRelativePath(_testFileDir, file);
			await UploadTestFiles(new FileInfo(file), relativePath);
		}
	}

	private void CreateFiles(bool create512ByteFiles = false)
	{
		foreach (var file in _testFiles.SkipLast(2))
		{
			FileInfo fileInfo = new(Path.Combine(_testFileDir, file));
			fileInfo.Directory.Create();

			using (var streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write($"This is {file}");
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}

		#region 512 byte files

		if (create512ByteFiles)
		{
			FileInfo fileInfo = new(Path.Combine(_testFileDir, "512byte", "TestFile1.txt"));
			fileInfo.Directory.Create();

			using (var streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque non sem quis orci rutrum hendrerit. Fusce ultricies cursus ante nec bibendum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam libero massa, viverra id suscipit in, tincidunt sit amet urna. Vestibulum gravida a massa eu molestie. Phasellus volutpat neque vitae enim molestie, vitae pharetra massa varius. Phasellus ante nulla, faucibus nec tristique eu, dignissim quis magna. Sed vitae sodales ipsum. Ut et maximus nibh. Etiam dui.");
				streamWriter.Close();
				streamWriter.Dispose();
			}

			fileInfo = new(Path.Combine(_testFileDir, "512byte", "TestFile2.txt"));
			fileInfo.Directory.Create();

			using (var streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque non sem quis orci rutrum hendrerit. Fusce ultricies cursus ante nec bibendum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam libero massa, viverra id suscipit in, tincidunt sit amet urna. Vestibulum gravida a massa eu molestie. Phasellus volutpat neque vitae enim molestie, vitae pharetra massa varius. Phasellus ante nulla, faucibus nec tristique eu, dignissim quis magna. Sed vitae sodales ipsum. Ut et maximus nibh. Etiam dui.");
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}

		#endregion 512 byte files
	}

	private async Task UploadTestFiles(FileInfo fileInfo, string blobName)
	{
		var createdDate = DateTimeOffset.UtcNow.AddDays(+1);
		createdDate = createdDate.AddDays(-1);

		// Example metadata
		var metadata = new Dictionary<string, string>
		{
			{ "uploadedBy", "unit-test" },
			{ "fileType", fileInfo.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase) ? "text" : "binary" }
		};

		// Example tags	
		var tags = new Dictionary<string, string>
		{
			{ "createdUtc",createdDate.ToString("d")}
		};

		if (blobName.Contains("Temp\\"))
		{
			tags["folder"] = "temp";
		}

		var blobClient = new BlobClient(_connstring, _containerName, blobName);
		using var fileStream = File.OpenRead(fileInfo.FullName);

		// Upload with metadata
		await blobClient.UploadAsync(fileStream,
			new BlobUploadOptions
			{
				Metadata = metadata,
				Tags = tags
			}, TestContext.CancellationToken);
	}

	private static async Task GenerateRandomFile(string filePath, long fileSize)
	{
		var rnd = new Random();
		await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
		var buffer = new byte[81920];
		long written = 0;

		while (written < fileSize)
		{
			rnd.NextBytes(buffer);
			int toWrite = (int)Math.Min(buffer.Length, fileSize - written);
			await fs.WriteAsync(buffer.AsMemory(0, toWrite));
			written += toWrite;
		}
	}

	private static Task<byte[]> CalculateSHA256(string filePath)
	{
		return Task.Run(() =>
		{
			using var stream = File.OpenRead(filePath);
			using var sha = SHA256.Create();
			return sha.ComputeHash(stream);
		});
	}

	public static async Task DownloadBlobSimple(string connectionString, string containerName, string blobName, string destinationPath, CancellationToken cancellationToken = default)
	{
		var blobClient = new BlobContainerClient(connectionString, containerName).GetBlobClient(blobName);
		await blobClient.DownloadToAsync(destinationPath, cancellationToken);
	}

	private static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
	{
		var blobServiceClient = new BlobServiceClient(connectionString);
		return blobServiceClient.GetBlobContainerClient(containerName);
	}

	private async Task<bool> BlobExists(string containerName, string blobName, string expected)
	{
		var blobServiceClient = new BlobServiceClient(_connstring);
		var container = blobServiceClient.GetBlobContainerClient(containerName);
		var blob = container.GetBlobClient(blobName);
		if (!blob.Exists())
			return false;

		var blobClient = new BlobClient(_connstring, _connection.ContainerName, blobName);
		var blobDownload = await blobClient.DownloadAsync();

		using var reader = new StreamReader(blobDownload.Value.Content);
		var content = await reader.ReadToEndAsync();
		return content == expected;
	}

	private string GetSourceFile(string fileName)
	{
		return Path.Combine(_testFileDir, fileName);
	}

	private string GetBlobName(string filePath)
	{
		return filePath.Substring(filePath.IndexOf("TestFiles", StringComparison.OrdinalIgnoreCase) + "TestFiles".Length)
			.TrimStart(Path.DirectorySeparatorChar);
	}

	public TestContext TestContext { get; set; }

	#endregion

	internal async Task AssertAllFilesUploaded(Dictionary<string, string> uploadedData, string sourceDirectory)
	{
		var localFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
		var uploadedLocalPaths = new HashSet<string>(uploadedData.Keys, StringComparer.OrdinalIgnoreCase);

		var missingFiles = localFiles.Where(f => !uploadedLocalPaths.Contains(f)).ToList();
		Assert.AreEqual(0, missingFiles.Count, $"The following files were not uploaded:{Environment.NewLine}{string.Join(Environment.NewLine, missingFiles)}");
		Assert.AreEqual(localFiles.Length, uploadedData.Count, $"Expected {localFiles.Length} uploaded files but got {uploadedData.Count}");

		var container = GetBlobContainer(_connstring, _containerName);
		foreach (var (localPath, blobUrl) in uploadedData)
		{
			var blobName = new Uri(blobUrl).AbsolutePath.TrimStart('/');
			blobName = blobName.Substring(blobName.IndexOf('/') + 1);
			var blobClient = container.GetBlobClient(blobName);
			Assert.IsTrue(
				await blobClient.ExistsAsync(TestContext.CancellationToken),
				$"Blob for '{localPath}' should exist in storage at '{blobUrl}'");

			blobClient.GetTagsAsync(cancellationToken: TestContext.CancellationToken).Result.Value.Tags.TryGetValue("TagName", out string tagValue);
			Assert.AreEqual("TagValue", tagValue, "Blob should have the correct tag value");
		}
	}
}
