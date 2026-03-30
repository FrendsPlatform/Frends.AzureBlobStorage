using Frends.AzureBlobStorage.WriteBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.WriteBlob.Tests;

[TestClass]
public class ConnectionStringUnitTests : UnitTestsBase
{
	[TestInitialize]
	public async Task Init()
	{
		await InitBase();

		_connection = new Connection
		{
			AuthenticationMethod = AuthenticationMethod.ConnectionString,
			ContainerName = _containerName,
			CreateContainerIfItDoesNotExist = false,
			BlobName = $"testblob_{Guid.NewGuid()}",
			Tags = null,
			HandleExistingFile = HandleExistingFile.Overwrite,
			Compress = false,
			ConnectionString = _connString,
		};
	}

	[TestMethod]
	public async Task WriteBlob_InvalidConnectionString_ShouldThrowException()
	{
		// Setup
		_connection.ConnectionString =
			"DefaultEndpointsProtocol=https;AccountName=invalid;AccountKey=InvalidAccountKey;EndpointSuffix=core.windows.net"; // Simulate an invalid connection string

		var input = new Input
		{
			SourceType = SourceType.String,
			ContentString = _testContent,
			Encoding = FileEncoding.UTF8
		};

		var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
			AzureBlobStorage.WriteBlob(input, _connection, _options, default));
		Assert.Contains("GetBlobClient error:", ex.Message, ex.Message);
	}
}
