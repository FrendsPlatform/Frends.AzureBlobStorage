using Frends.AzureBlobStorage.ReadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ReadBlob.Tests;

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
	public async Task ReadBlob_Sas_Missing()
	{
		// Setup
		var input = new Input
		{
			BlobName = "TestFile1.txt",
		};

		_connection.SasToken = "";

		// Act & Assert
		var ex = await Assert.ThrowsAsync<ArgumentException>(() => 
			AzureBlobStorage.ReadBlob(input, _connection, _options, default));

		Assert.Contains("SAS Token and URI required.", ex.Message);
	}
}
