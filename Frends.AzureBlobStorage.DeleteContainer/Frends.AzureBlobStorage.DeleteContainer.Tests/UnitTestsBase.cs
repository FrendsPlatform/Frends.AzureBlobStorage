using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DeleteContainer.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.DeleteContainer.Tests;
public abstract class UnitTestsBase
{
	internal readonly string _connstring = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
	internal readonly string _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

	private readonly string _testFileDir = Path.Combine(Environment.CurrentDirectory, "TestFiles");
	private readonly string _firstTestFile = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile.txt");
	private readonly string _secondTestFile = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile2.txt");

	internal Connection _connection;

	protected async Task InitBase()
	{
		await CreateContainer(_containerName);
	}

	[TestCleanup]
	public async Task CleanUp()
	{
		await DeleteContainer(_containerName);
	}

	[TestMethod]
	public async Task DeleteContainer_ContainerDoesNotExist()
	{
		var input = new Input
		{
			ContainerName = $"nonexistingcontainer{Guid.NewGuid()}"
		};

		var options = new Options()
		{ 
			ThrowErrorIfContainerDoesNotExists = false
		};

		var result = await AzureBlobStorage.DeleteContainer(input, _connection, options, TestContext.CancellationToken);
		Assert.IsFalse(result.ContainerWasDeleted);
		Assert.Contains("Container not found", result.Message);
	}

	[TestMethod]
	public async Task DeleteContainer_ContainerDeleted()
	{
		var input = new Input
		{
			ContainerName = _containerName
		};

		var options = new Options()
		{
			ThrowErrorIfContainerDoesNotExists = false
		};

		var result = await AzureBlobStorage.DeleteContainer(input, _connection, options, TestContext.CancellationToken);
		Assert.IsTrue(result.ContainerWasDeleted);
		Assert.Contains("Container deleted successfully.", result.Message);
		Assert.IsFalse(ContainerExists(_containerName).Result);
	}


	#region Helper methods

	internal static Uri GetUri(string uri, string containerName) => new($"{uri}/{containerName}");

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

	private async Task CreateAndUploadTestFiles()
	{
		await CreateFiles();
		await UploadTestFiles(new FileInfo(_firstTestFile));
		await UploadTestFiles(new FileInfo(_secondTestFile));
	}

	private async Task CreateFiles()
	{
		Directory.CreateDirectory(_testFileDir);

		#region 512 byte files

		await File.WriteAllTextAsync(_firstTestFile,
			"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque non sem quis orci rutrum hendrerit. Fusce ultricies cursus ante nec bibendum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam libero massa, viverra id suscipit in, tincidunt sit amet urna. Vestibulum gravida a massa eu molestie. Phasellus volutpat neque vitae enim molestie, vitae pharetra massa varius. Phasellus ante nulla, faucibus nec tristique eu, dignissim quis magna. Sed vitae sodales ipsum. Ut et maximus nibh. Etiam dui.",
			TestContext.CancellationToken);

		await File.WriteAllTextAsync(_secondTestFile,
			"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla mollis neque nibh, molestie consequat lacus euismod in. Phasellus in libero id velit sollicitudin rhoncus. Sed a nunc non lacus hendrerit iaculis. Suspendisse quis dui id enim sollicitudin rhoncus. Phasellus convallis lacinia finibus. Sed quis purus vitae felis finibus facilisis at quis nisi. Integer pharetra, ex egestas iaculis ultricies, tortor neque hendrerit justo, eu vulputate odio eros sed augue. Mauris quis sapien non ligula maximus eget.",
			TestContext.CancellationToken	);

		#endregion 512 byte files
	}

	private async Task UploadTestFiles(FileInfo fileInfo)
	{
		using var fileStream = File.OpenRead(fileInfo.FullName);
		var blobClient = new BlobClient(_connstring, _containerName, fileInfo.Name);
		await blobClient.UploadAsync(fileStream, TestContext.CancellationToken);
	}

	private void DeleteTestFilesDirectory()
	{
		if (Directory.Exists(_testFileDir))
		{
			Directory.Delete(_testFileDir, true);
		}
	}


	public TestContext TestContext { get; set; }

	#endregion
}

