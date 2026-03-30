using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests;

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
	public async Task ListBlob_ConnectionStringMissing()
	{
		_connection.ConnectionString = "";

		var options = new Options
		{
			Prefix = "t",
			ListingStructure = ListingStructure.Hierarchical
		};

		var ex = await Assert.ThrowsExactlyAsync<ArgumentException>(async () => await AzureBlobStorage.ListBlobsInContainer(_connection, options, default));
		Assert.AreEqual("Connection string required.", ex.InnerException.Message);
	}
}