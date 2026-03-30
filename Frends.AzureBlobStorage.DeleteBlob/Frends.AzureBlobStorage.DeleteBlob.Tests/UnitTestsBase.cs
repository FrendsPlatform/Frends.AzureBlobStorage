using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DeleteBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.DeleteBlob.Tests;
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
		await CreateAndUploadTestFiles();
	}

	[TestCleanup]
	public async Task CleanUp()
	{
		await DeleteContainer(_containerName);
		DeleteTestFilesDirectory();
	}

	[TestMethod]
	public async Task DeleteBlobAsync_ContainerDoesNotExistsInfo()
	{
		_connection.ContainerName = "none";
		var input = new Input
		{
			BlobName = Guid.NewGuid().ToString()
		};

		var options = new Options()
		{
			SnapshotDeleteOption = default,
			VerifyETagWhenDeleting = default,
			ThrowErrorIfBlobDoesNotExists = false,
		};

		var result = await AzureBlobStorage.DeleteBlob(input, _connection, options, default);
		Assert.IsFalse(result.Success);
		Assert.Contains("doesn't exist in container", result.Info);
	}

	[TestMethod]
	public async Task DeleteBlobAsync_ThrowError_WithFlagEnabled()
	{
		_connection.ContainerName = "none";
		var input = new Input
		{
			BlobName = Guid.NewGuid().ToString()
		};

		var options = new Options
		{
			SnapshotDeleteOption = default,
			VerifyETagWhenDeleting = default,
			ThrowErrorIfBlobDoesNotExists = true,
		};

		var exception = await Assert.ThrowsExactlyAsync<Exception>(() =>
			AzureBlobStorage.DeleteBlob(input, _connection, options, CancellationToken.None));

		Assert.IsNotNull(exception);
		Assert.IsTrue(exception.Message.Contains("An error occured while trying to delete blob"), exception.Message);
	}

	[TestMethod]
	public async Task DeleteBlobAsync_BlobDoesNotExistsInfo()
	{
		var input = new Input
		{
			BlobName = "none"
		};

		var options = new Options()
		{
			SnapshotDeleteOption = default,
			VerifyETagWhenDeleting = default,
			ThrowErrorIfBlobDoesNotExists = false,
		};

		var result = await AzureBlobStorage.DeleteBlob(input, _connection, options, default);
		Assert.IsFalse(result.Success);
		Assert.IsTrue(result.Info.Contains("doesn't exist in container"));
	}

	[TestMethod]
	public async Task DeleteBlobAsync_DeleteBlob()
	{
		var input = new Input
		{
			BlobName = new FileInfo(_firstTestFile).Name
		};

		var options = new Options()
		{
			SnapshotDeleteOption = default,
			VerifyETagWhenDeleting = default,
			ThrowErrorIfBlobDoesNotExists = true,
		};

		var result = await AzureBlobStorage.DeleteBlob(input, _connection, options, default);
		Assert.IsTrue(result.Success);
		Assert.IsTrue(result.Info.Contains("deleted from container"));
	}



	[TestMethod]
	public async Task DeleteBlobAsync_DeleteBlob_NotFullFileName()
	{
		var input = new Input
		{
			BlobName = "testfile"
		};

		var result = await AzureBlobStorage.DeleteBlob(input, _connection, new Options(), default);
		Assert.IsFalse(result.Success);
		Assert.Contains("doesn't exist in container", result.Info);
	}

	[TestMethod]
	public async Task DeleteBlobAsync_DeleteBlob_NotFullFileNameWithJoker()
	{
		var input = new Input
		{
			BlobName = "testfile*"
		};

		var result = await AzureBlobStorage.DeleteBlob(input, _connection, new Options(), default);
		Assert.IsFalse(result.Success);
		Assert.Contains("doesn't exist in container", result.Info);
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

