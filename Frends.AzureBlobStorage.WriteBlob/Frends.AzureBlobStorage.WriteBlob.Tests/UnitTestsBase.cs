using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Frends.AzureBlobStorage.WriteBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.WriteBlob.Tests;

public abstract class UnitTestsBase
{
	internal readonly string _connString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
	internal readonly string _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
	internal readonly string _containerName2 = $"test-container2{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

	internal readonly string _testContent = "This is a test content for Azure Blob Storage WriteBlob unit tests.";
	internal readonly Tag[] _tags = [new Tag { Name = "TagName", Value = "TagValue" }];
	
	private readonly string _workingDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
	private readonly List<string> _testFiles =
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

	internal Connection _connection;
	internal Options _options;

	protected async Task InitBase()
	{
		await CreateContainer(_containerName);

		_options = new Options
		{
			ThrowErrorOnFailure = true
		};
	}

	[TestCleanup]
	public async Task CleanUp()
	{
		await DeleteContainer(_containerName);
		await DeleteContainer(_containerName2);
	}

	[TestMethod]
	public async Task WriteBlob_TestWriteFromString()
	{
		// Setup
		_connection.BlobName = $"testblob_{Guid.NewGuid()}";
		var input = new Input
		{
			SourceType = SourceType.String,
			ContentString = _testContent,
			ContentBytes = Encoding.UTF8.GetBytes(_testContent),
			Encoding = FileEncoding.UTF8
		};

		// Act
		var result = await AzureBlobStorage.WriteBlob(input, _connection, _options, TestContext.CancellationToken);
		Assert.IsTrue(result.Success);
		Assert.IsTrue(await BlobExists(_connection.ContainerName, _connection.BlobName, _testContent));
	}

	[TestMethod]
	public async Task WriteBlob_TestWriteFromByteArray()
	{
		// Setup
		_connection.BlobName = $"testblob_{Guid.NewGuid()}";
		var input = new Input
		{
			SourceType = SourceType.Bytes,
			ContentBytes = Encoding.UTF8.GetBytes(_testContent),
			Encoding = FileEncoding.UTF8
		};

		// Act
		var result = await AzureBlobStorage.WriteBlob(input, _connection, _options, TestContext.CancellationToken);

		// Assert
		Assert.IsTrue(result.Success);
		Assert.IsTrue(await BlobExists(_connection.ContainerName, _connection.BlobName, _testContent));
	}

	[TestMethod]
	public async Task WriteBlob_TestFolderBlobName()
	{
		// Setup
		_connection.BlobName = $"testblob_{Guid.NewGuid()}";
		var input = new Input
		{
			SourceType = SourceType.Bytes,
			ContentBytes = Encoding.UTF8.GetBytes(_testContent),
			Encoding = FileEncoding.UTF8
		};

		// Act
		var result = await AzureBlobStorage.WriteBlob(input, _connection, _options, default);

		// Assert
		Assert.IsTrue(result.Success);
		Assert.IsTrue(await BlobExists(_connection.ContainerName, _connection.BlobName, _testContent));
	}

	[TestMethod]
	public async Task WriteBlob_TestEncoding()
	{
		// Setup
		var encodings = new List<FileEncoding>()
		{
			FileEncoding.UTF8,
			FileEncoding.Default,
			FileEncoding.ASCII,
			FileEncoding.WINDOWS1252,
			FileEncoding.Other
		};

		var input = new Input
		{
			SourceType = SourceType.Bytes,
			ContentBytes = Encoding.UTF8.GetBytes(_testContent),
			FileEncodingString = "windows-1251",
		};

		foreach (var encoding in encodings)
		{
			input.Encoding = encoding;

			// Connection string
			var result = await AzureBlobStorage.WriteBlob(input, _connection, _options, default);
			Assert.IsTrue(result.Success, $"Encoding: {encoding}");
			Assert.IsTrue(await BlobExists(_connection.ContainerName, _connection.BlobName, _testContent));
		}
	}

	[TestMethod]
	public async Task WriteBlob_TestCreateContainer()
	{
		// Setup
		_connection.CreateContainerIfItDoesNotExist = true;
		_connection.ContainerName = _containerName2;

		var input = new Input
		{
			SourceType = SourceType.String,
			ContentString = _testContent,
			Encoding = FileEncoding.UTF8
		};

		// Act
		var result = await AzureBlobStorage.WriteBlob(input, _connection, _options, TestContext.CancellationToken);

		// Assert
		Assert.IsTrue(result.Success);

		var blobServiceClient = new BlobServiceClient(_connString);
		var containerClient = blobServiceClient.GetBlobContainerClient(_containerName2);
		Assert.IsTrue(containerClient.Exists(TestContext.CancellationToken));
	}

	[TestMethod]
	public async Task WriteBlob_Tags()
	{
		// Setup
		_connection.BlobName = $"testblob_{Guid.NewGuid()}";
		var input = new Input
		{
			SourceType = SourceType.String,
			ContentString = _testContent,
			ContentBytes = Encoding.UTF8.GetBytes(_testContent),
			Encoding = FileEncoding.UTF8
		};

		_connection.Tags = _tags;

		// Act
		var result = await AzureBlobStorage.WriteBlob(input, _connection, _options, default);

		// Assert
		Assert.IsTrue(result.Success);
		Assert.IsTrue(await BlobExists(_connection.ContainerName, _connection.BlobName, _testContent));

		var container = await GetContainerClient(_containerName);
		var blobClient = container.GetBlobClient(_connection.BlobName);
		blobClient.GetTagsAsync(cancellationToken: TestContext.CancellationToken).Result.Value.Tags.TryGetValue("TagName", out var tagValue);
		Assert.AreEqual("TagValue", tagValue, "Blob should have the correct tag value");
	}


	[TestMethod]
	public async Task WriteBlob_Compress()
	{
		// Setup
		var input = new Input
		{
			SourceType = SourceType.String,
			ContentString = _testContent,
			ContentBytes = Encoding.UTF8.GetBytes(_testContent),
			Encoding = FileEncoding.UTF8
		};

		_connection.Compress = true;

		// Act
		var result = await AzureBlobStorage.WriteBlob(input, _connection, _options, default);

		// Assert
		Assert.IsTrue(result.Success);
		Assert.Contains(_connection.ContainerName + "/" + _connection.BlobName, result.Uri);
	}

	#region Helper methods

	internal static Uri GetUri(string uri, string containerName) => new($"{uri}/{containerName}");

	internal async Task<BlobContainerClient> GetContainerClient(string containerName)
	{
		var blobServiceClient = new BlobServiceClient(_connString);
		var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: TestContext.CancellationToken);
		return blobContainerClient;
	}

	internal async Task CreateContainer(string containerName)
	{
		var blobServiceClient = new BlobServiceClient(_connString);
		var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: TestContext.CancellationToken);
	}

	internal async Task DeleteContainer(string containerName)
	{
		var blobServiceClient = new BlobServiceClient(_connString);
		var container = blobServiceClient.GetBlobContainerClient(containerName);
		await container.DeleteIfExistsAsync(cancellationToken: TestContext.CancellationToken);
	}

	internal async Task<bool> ContainerExists(string containerName)
	{
		var blobServiceClient = new BlobServiceClient(_connString);
		var container = blobServiceClient.GetBlobContainerClient(containerName);
		return await container.ExistsAsync(cancellationToken: TestContext.CancellationToken);
	}

	internal async Task<bool> BlobExists(string containerName, string blobName, string expectedContent = null)
	{
		var container = await GetContainerClient(containerName);
		var blobClient = container.GetBlobClient(blobName);
		if (await blobClient.ExistsAsync(cancellationToken: TestContext.CancellationToken))
		{
			if (expectedContent != null)
			{
				var downloadInfo = await blobClient.DownloadAsync(cancellationToken: TestContext.CancellationToken);
				using var reader = new StreamReader(downloadInfo.Value.Content);
				var content = await reader.ReadToEndAsync();
				return content == expectedContent;
			}
			return true;
		}
		return false;
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

		var blobClient = new BlobClient(_connString, _containerName, blobName);
		using var fileStream = File.OpenRead(fileInfo.FullName);

		// Upload with metadata
		await blobClient.UploadAsync(fileStream,
			new BlobUploadOptions
			{
				Metadata = metadata,
				Tags = tags
			}, TestContext.CancellationToken);
	}


	public TestContext TestContext { get; set; }

	#endregion
}
