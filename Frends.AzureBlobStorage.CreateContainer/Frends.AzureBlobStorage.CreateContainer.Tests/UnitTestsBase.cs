using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.CreateContainer.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.CreateContainer.Tests;
public abstract class UnitTestsBase
{
	internal readonly string _connstring = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
	internal readonly string _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

	internal Connection _connection;

	protected async Task InitBase()
	{
	}

	[TestCleanup]
	public async Task CleanUp()
	{
		await DeleteContainer(_containerName);
	}

	[TestMethod]
	public async Task CreateContainer_Success()
	{
		var input = new Input 
		{ 
			ContainerName = _containerName 
		};

		var options = new Options 
		{ 
			ThrowErrorOnFailure = true 
		};

		var result = await AzureBlobStorage.CreateContainer(input, _connection, options, TestContext.CancellationToken);
		
		Assert.IsNotNull(result);
		Assert.IsTrue(result.Success);
		Assert.Contains(_containerName, result.Uri);
		Assert.IsNull(result.Error);
		Assert.IsTrue(ContainerExists(_containerName).Result);
	}

	[TestMethod]
	public async Task CreateContainer_Throws_ParameterNotValid()
	{
		var input = new Input 
		{ 
			ContainerName = "Valid name" 
		};

		var options = new Options 
		{ 
			ThrowErrorOnFailure = true 
		};
		
		await Assert.ThrowsExactlyAsync<Exception>(async () =>
		{
			await AzureBlobStorage.CreateContainer(input, _connection, options, TestContext.CancellationToken);
		});
	}

	[TestMethod]
	public async Task CreateContainer_ThrowErrorOnFailure_False()
	{
		var input = new Input 
		{ 
			ContainerName = "" 
		};
	
		var options = new Options 
		{ 
			ThrowErrorOnFailure = false, 
			ErrorMessageOnFailure = "Custom error message" 
		};

		var result = await AzureBlobStorage.CreateContainer(input, _connection, options, TestContext.CancellationToken);

		Assert.IsFalse(result.Success);
		Assert.AreEqual(string.Empty, result.Uri);
		Assert.IsNotNull(result.Error);
		Assert.Contains("Custom error message", result.Error.Message);
		Assert.IsNotNull(result.Error.AdditionalInfo);
	}

	[TestMethod]
	public async Task CreateContainer_ThrowErrorOnFailure_True()
	{
		var input = new Input 
		{ 
			ContainerName = "Valid name" 
		};

		var options = new Options 
		{ 
			ThrowErrorOnFailure = true, 
			ErrorMessageOnFailure = "Custom error message" 
		};

		await Assert.ThrowsExactlyAsync<Exception>(async () =>
		{
			await AzureBlobStorage.CreateContainer(input, _connection, options, CancellationToken.None);
		});
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


	public TestContext TestContext { get; set; }

	#endregion
}

