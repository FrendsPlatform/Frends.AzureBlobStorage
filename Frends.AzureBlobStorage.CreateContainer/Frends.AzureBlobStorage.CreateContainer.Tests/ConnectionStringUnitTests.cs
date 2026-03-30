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
			ConnectionString = _connstring
		};
	}



	[TestMethod]
	public async Task TestCreateContainer_throws_ClientNotFound()
	{
		var input = new Input { ContainerName = _containerName };
		var connection = new Connection
		{
			AuthenticationMethod = AuthenticationMethod.ConnectionString,
			ConnectionString =
				"DefaultEndpointsProtocol=https;AccountName=unitTestStorage;AccountKey=abcdefghijklmnopqrstuyxz123456789;EndpointSuffix=core.windows.net"
		};
		var options = new Options { ThrowErrorOnFailure = true };
		await Assert.ThrowsExactlyAsync<Exception>(async () =>
		{
			await AzureBlobStorage.CreateContainer(input, connection, options, CancellationToken.None);
		});
	}
}
