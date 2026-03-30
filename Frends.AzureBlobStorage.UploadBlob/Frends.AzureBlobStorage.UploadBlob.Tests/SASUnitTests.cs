using Frends.AzureBlobStorage.UploadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.UploadBlob.Tests;

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
			SasToken = _sasToken,
			Uri = _uri,
			ContainerName = _containerName
		};
	}

	[TestMethod]
	public async Task UploadBlob_ErrorEmptyUri()
	{
		// Setup
		_connection.Uri = "";
		var input = new Input
		{
			BlobName = "TestFile1.txt",
			SourceType = UploadSourceType.File,
			SourceFile = _testFiles[0],
			Tags = null,
			Compress = default,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = default,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		// Act & Assert
		var ex = await Assert.ThrowsExactlyAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, _connection, _options, default));
		Assert.Contains("An exception occured.", ex.Message);
	}

	[TestMethod]
	public async Task UploadBlob_ErrorEmptySasToken()
	{
		// Setup
		_connection.SasToken = "";
		var input = new Input
		{
			BlobName = "TestFile1.txt",
			SourceType = UploadSourceType.File,
			SourceFile = _testFiles[0],
			Tags = null,
			Compress = default,
			ContentsOnly = default,
			SearchPattern = default,
			SourceDirectory = default,
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		var ex = await Assert.ThrowsExactlyAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, _connection, _options, default));
		Assert.Contains("An exception occured.", ex.Message);
	}
}
