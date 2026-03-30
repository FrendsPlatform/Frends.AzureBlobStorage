using Azure.Identity;
using Frends.AzureBlobStorage.WriteBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.WriteBlob.Tests;
[TestClass]
public class OAuthUnitTests : UnitTestsBase
{
	private readonly string _uri = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_Uri");
	private readonly string _appID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
	private readonly string _tenantID = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
	private readonly string _clientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");

	[TestInitialize]
	public async Task Init()
	{
		await InitBase();

		_connection = new Connection
		{
			AuthenticationMethod = AuthenticationMethod.OAuth2,
			ContainerName = _containerName,
			CreateContainerIfItDoesNotExist = false,
			BlobName = $"testblob_{Guid.NewGuid()}",
			Tags = null,
			HandleExistingFile = HandleExistingFile.Overwrite,
			Compress = false,
			ApplicationId = _appID,
			TenantId = _tenantID,
			ClientSecret = _clientSecret,
			Uri = _uri,
			
		};
	}

	[TestMethod]
	public async Task WriteBlob_InvalidOAuth2_ShouldThrowException()
	{
		// Setup
		_connection.ClientSecret = "InvalidClientSecret";
		var input = new Input
		{
			SourceType = SourceType.String,
			ContentString = _testContent,
			Encoding = FileEncoding.UTF8
		};

		var ex = await Assert.ThrowsAsync<Exception>(async () =>
			await AzureBlobStorage.WriteBlob(input, _connection, _options, TestContext.CancellationToken));
		Assert.Contains("ClientSecretCredential authentication failed", ex.Message);
	}
}