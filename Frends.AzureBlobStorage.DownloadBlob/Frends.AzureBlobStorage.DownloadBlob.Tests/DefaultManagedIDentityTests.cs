using Frends.AzureBlobStorage.DownloadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.DownloadBlob.Tests;

[TestClass]
public class DefaultManagedIDentityTests : UnitTestsBase
{
	private readonly string _uri = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_Uri");

	[TestInitialize]
	public async Task Init()
	{
		await InitBase();

		_connection = new Connection
		{
			AuthenticationMethod = AuthenticationMethod.DefaultManagedIdentity,
			Uri = _uri,
			ContainerName = _containerName
		};
	}
}
