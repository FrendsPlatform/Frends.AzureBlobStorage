using Frends.AzureBlobStorage.ReadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ReadBlob.Tests;

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
			ConnectionString = _connstring,
			ContainerName = _containerName
		};
	}

	[TestMethod]
	public async Task ReadBlob_ConnectionString_Missing()
	{
		// Setup
		var input = new Input
		{
			BlobName = "TestFile1.txt",
		};

		_connection.ConnectionString = "";

		// Act & Assert
		var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
			AzureBlobStorage.ReadBlob(input, _connection, _options, default));

		Assert.Contains("Connection string required.", ex.Message);
	}
}
