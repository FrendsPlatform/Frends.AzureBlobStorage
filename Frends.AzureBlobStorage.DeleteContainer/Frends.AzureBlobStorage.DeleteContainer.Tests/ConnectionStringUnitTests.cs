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
}
