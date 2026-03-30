using Frends.AzureBlobStorage.WriteBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.WriteBlob.Tests;

[TestClass]
public class SASUnitTests : UnitTestsBase
{
	private readonly string _uri = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_Uri");
	private readonly string _sasToken = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_SASToken");

	[TestInitialize]
	public async Task Init()
	{
		await InitBase();

		_connection = new Connection
		{
			AuthenticationMethod = AuthenticationMethod.SASToken,
			ContainerName = _containerName,
			CreateContainerIfItDoesNotExist = false,
			BlobName = $"testblob_{Guid.NewGuid()}",
			Tags = null,
			HandleExistingFile = HandleExistingFile.Overwrite,
			Compress = false,
			SasToken = _sasToken,
			Uri = _uri,
		};
	}

	[TestMethod]
	public async Task UploadBlob_ErrorEmptyUri()
	{
		// Setup
		var input = new Input
		{
			SourceType = SourceType.String,
			ContentString = _testContent,
			ContentBytes = Encoding.UTF8.GetBytes(_testContent),
			Encoding = FileEncoding.UTF8
		};
		_connection.Uri = "";

		var ex = await Assert.ThrowsExactlyAsync<Exception>(() =>
			AzureBlobStorage.WriteBlob(input, _connection, _options, default));
		Assert.Contains("Connection.SasToken and Connection.Uri parameters can't be empty ", ex.Message);
	}

	[TestMethod]
	public async Task UploadBlob_ErrorEmptySasToken()
	{
		// Setup
		var input = new Input
		{
			SourceType = SourceType.String,
			ContentString = _testContent,
			ContentBytes = Encoding.UTF8.GetBytes(_testContent),
			Encoding = FileEncoding.UTF8
		};
		_connection.SasToken = "";

		var ex = await Assert.ThrowsExactlyAsync<Exception>(() => 
			AzureBlobStorage.WriteBlob(input, _connection, _options, TestContext.CancellationToken));
		Assert.Contains("Connection.SasToken and Connection.Uri parameters can't be empty ", ex.Message);
	}
}
