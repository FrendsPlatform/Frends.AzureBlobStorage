using Frends.AzureBlobStorage.WriteBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.WriteBlob.Tests;
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
			ContainerName = _containerName,
			CreateContainerIfItDoesNotExist = false,
			BlobName = $"testblob_{Guid.NewGuid()}",
			Tags = null,
			HandleExistingFile = HandleExistingFile.Overwrite,
			Compress = false,
		};
	}
}