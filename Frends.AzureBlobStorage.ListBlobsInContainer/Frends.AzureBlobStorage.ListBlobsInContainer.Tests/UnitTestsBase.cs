using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests;
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

	protected async Task InitBase()
	{
		await CreateContainer(_containerName);
		CreateWorkingDirectory();
		await CreateAndUploadTestFiles(true);
	}

	[TestCleanup]
	public async Task CleanUp()
	{
		await DeleteContainer(_containerName);
		DeleteWorkingDirectory();
	}

	[TestMethod]
	public async Task ListBlob_ListingStructures()
	{
		var listing = new List<ListingStructure>() { ListingStructure.Flat, ListingStructure.Hierarchical };

		foreach (var structure in listing)
		{
			var options = new Options
			{
				Prefix = null,
				ListingStructure = structure
			};

			var result = await AzureBlobStorage.ListBlobsInContainer(_connection, options, default);

			if (structure is ListingStructure.Flat)
			{
				Assert.Contains(x => x.Name == "Temp/SubFolderFile1.txt", result.BlobList);
				Assert.Contains(x => x.Name == "Temp/SubFolderFile2.txt", result.BlobList);
				Assert.Contains(x => x.URL.Contains("/Temp/SubFolderFile1.txt"), result.BlobList);
				Assert.Contains(x => x.URL.Contains("/Temp/SubFolderFile2.txt"), result.BlobList);
			}
			else
			{
				Assert.Contains(x => x.Name == "Temp/", result.BlobList);
				Assert.Contains(x => x.URL.Contains("/Temp/"), result.BlobList);
			}

			Assert.Contains(x => x.Name == "TestFile1.txt", result.BlobList);
			Assert.Contains(x => x.Name == "TestFile2.txt", result.BlobList);
			Assert.Contains(x => x.ETag != null, result.BlobList);
			Assert.Contains(x => x.Type != null, result.BlobList);
			Assert.Contains(x => x.CreatedOn != null, result.BlobList);
			Assert.Contains(x => x.LastModified != null, result.BlobList);
		}
	}

	[TestMethod]
	public async Task ListBlob_Prefix()
	{
		var listing = new List<ListingStructure>() { ListingStructure.Flat, ListingStructure.Hierarchical };

		foreach (var structure in listing)
		{
			var options = new Options
			{
				Prefix = "Tes",
				ListingStructure = structure
			};

			var result = await AzureBlobStorage.ListBlobsInContainer(_connection, options, default);

			Assert.DoesNotContain(x => x.Name == "Temp/SubFolderFile1.txt", result.BlobList);
			Assert.DoesNotContain(x => x.Name == "Temp/SubFolderFile2.txt", result.BlobList);
			Assert.DoesNotContain(x => x.URL.Contains("/Temp/SubFolderFile1.txt"), result.BlobList);
			Assert.DoesNotContain(x => x.URL.Contains("/Temp/SubFolderFile2.txt"), result.BlobList);
			Assert.DoesNotContain(x => x.Name == "Temp/", result.BlobList);
			Assert.DoesNotContain(x => x.URL.Contains("/Temp/"), result.BlobList);

			Assert.Contains(x => x.Name == "TestFile1.txt", result.BlobList);
			Assert.Contains(x => x.Name == "TestFile2.txt", result.BlobList);
			Assert.Contains(x => x.ETag != null, result.BlobList);
			Assert.Contains(x => x.Type != null, result.BlobList);
			Assert.Contains(x => x.CreatedOn != null, result.BlobList);
			Assert.Contains(x => x.LastModified != null, result.BlobList);
		}
	}

	[TestMethod]
	public async Task ListBlob_ListByTags()
	{
		// Wait for a while to ensure that the tags are indexed
		Thread.Sleep(2000);

		var listing = new List<ListingStructure>() { ListingStructure.Flat };

		foreach (var structure in listing)
		{
			var options = new Options
			{
				ListingStructure = structure,
				QueryType = QueryType.Tags,
				//TagQuery = $"createdUtc = '{DateTime.UtcNow:d}'"
				TagQuery = $"folder = 'temp'"
			};

			var result = await AzureBlobStorage.ListBlobsInContainer(_connection, options, default);

			Assert.Contains(x => x.Name == "Temp/SubFolderFile1.txt", result.BlobList);
			Assert.Contains(x => x.Name == "Temp/SubFolderFile2.txt", result.BlobList);
			Assert.Contains(x => x.URL.Contains("/Temp/SubFolderFile1.txt"), result.BlobList);
			Assert.Contains(x => x.URL.Contains("/Temp/SubFolderFile2.txt"), result.BlobList);
			Assert.DoesNotContain(x => x.Name == "Temp/", result.BlobList);
			Assert.Contains(x => x.URL.Contains("/Temp/"), result.BlobList);
			Assert.HasCount(2, result.BlobList);
			Assert.DoesNotContain(x => x.Name == "TestFile.txt", result.BlobList);
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
			},TestContext.CancellationToken);
	}


	public TestContext TestContext { get; set; }

	#endregion
}
