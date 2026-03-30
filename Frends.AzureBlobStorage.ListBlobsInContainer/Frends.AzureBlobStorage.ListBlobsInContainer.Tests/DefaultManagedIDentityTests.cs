using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests;

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

