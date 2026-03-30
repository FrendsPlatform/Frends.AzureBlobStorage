using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DownloadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.DownloadBlob.Tests;
public abstract class UnitTestsBase
{
	internal readonly string _connstring = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
	internal readonly string _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

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

	private string _testFileDir;
	internal string _destinationDirectory;
	internal Connection _connection;
	internal Destination _destination;

	protected async Task InitBase()
	{
		await CreateContainer(_containerName);
		CreateWorkingDirectory();
		await CreateAndUploadTestFiles(true);

		_destination = new Destination
		{
			Directory = _destinationDirectory,
			FileExistsOperation = FileExistsAction.Overwrite
		};
	}

	[TestCleanup]
	public async Task CleanUp()
	{
		await DeleteContainer(_containerName);
		DeleteWorkingDirectory();
	}

	[TestMethod]
	public async Task DownloadBlob_WritesBlobToFile()
	{
		// Setup
		var input = new Input
		{
			BlobName = "TestFile1.txt",
			Encoding = FileEncoding.UTF8,
			EnableBOM = false
		};

		// Act
		var result = await AzureBlobStorage.DownloadBlob(input, _connection, _destination, default);

		// Assert
		Assert.IsTrue(File.Exists(result.FullPath));
		var fileContent = File.ReadAllText(result.FullPath);
		Assert.Contains($"This is {input.BlobName}", fileContent);
	}

	[TestMethod]
	public async Task DownloadBlob_ThrowsExceptionIfDestinationFileExists()
	{
		// Setup
		File.Create(Path.Combine(_destinationDirectory, "TestFile1.txt")).Dispose();

		var input = new Input
		{
			BlobName = "TestFile1.txt",
			Encoding = FileEncoding.UTF8,
			EnableBOM = false
		};

		_destination.FileExistsOperation = FileExistsAction.Error;

		// Act
		await Assert.ThrowsAsync<Exception>(() => AzureBlobStorage.DownloadBlob(input, _connection, _destination, default));
	}

	[TestMethod]
	public async Task DownloadBlob_RenamesFileIfExists()
	{
		// Setup
		File.Create(Path.Combine(_destinationDirectory, "TestFile1.txt")).Dispose();

		var input = new Input
		{
			BlobName = "TestFile1.txt",
			Encoding = FileEncoding.UTF8,
			EnableBOM = false
		};

		_destination.FileExistsOperation = FileExistsAction.Rename;

		// Act
		var result = await AzureBlobStorage.DownloadBlob(input, _connection, _destination, default);

		// Assert
		Assert.AreEqual("TestFile1(1).txt", result.FileName);
	}

	[TestMethod]
	public async Task DownloadBlob_OverwritesFileIfExists()
	{
		// Setup
		File.Create(Path.Combine(_destinationDirectory, "TestFile1.txt")).Dispose();

		var input = new Input
		{
			BlobName = "TestFile1.txt",
			Encoding = FileEncoding.UTF8,
			EnableBOM = false
		};

		_destination.FileExistsOperation = FileExistsAction.Overwrite;

		// Act
		await AzureBlobStorage.DownloadBlob(input, _connection, _destination, default);

		// Assert
		Assert.HasCount(1, Directory.GetFiles(_destinationDirectory));
	}

	[TestMethod]
	public async Task DownloadBlob_DifferentEncoding()
	{
		// Setup
		var encodings = new FileEncoding[]
		{
			FileEncoding.UTF8,
			FileEncoding.WINDOWS1252,
			FileEncoding.Other,
			FileEncoding.ASCII,
			FileEncoding.Default
		};

		var input = new Input
		{
			BlobName = "TestFile1.txt",
			FileEncodingString = "windows-1251",
			EnableBOM = false
		};

		// Act & Assert
		foreach (var encoding in encodings)
		{
			input.Encoding = encoding;
			await AzureBlobStorage.DownloadBlob(input, _connection, _destination, default);
			Assert.HasCount(1, Directory.GetFiles(_destinationDirectory));
		}
	}

	[TestMethod]
	public async Task DownloadBlob_UsesTargetFileName_WhenSpecified()
	{
		// Setup
		var input = new Input
		{
			BlobName = "TestFile1.txt",
			Encoding = FileEncoding.UTF8,
			EnableBOM = false
		};

		_destination.TargetFileName = "custom-name.txt";

		// Act
		var result = await AzureBlobStorage.DownloadBlob(input, _connection, _destination, default);

		// Assert
		Assert.AreEqual(_destination.TargetFileName, result.FileName);
		Assert.IsTrue(File.Exists(result.FullPath));
		var fileContent = File.ReadAllText(result.FullPath);
		Assert.Contains($"This is {input.BlobName}", fileContent);
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
		using var fileStream = File.OpenRead(fileInfo.FullName);
		var blobClient = new BlobClient(_connstring, _containerName, blobName);
		await blobClient.UploadAsync(fileStream, TestContext.CancellationToken);
	}


	public TestContext TestContext { get; set; }

	#endregion
}

