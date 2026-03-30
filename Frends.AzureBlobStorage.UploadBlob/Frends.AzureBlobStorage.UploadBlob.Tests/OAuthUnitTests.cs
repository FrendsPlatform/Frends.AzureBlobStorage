using Frends.AzureBlobStorage.UploadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.UploadBlob.Tests;
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
			ApplicationId = _appID,
			TenantId = _tenantID,
			ClientSecret = _clientSecret,
			Uri = _uri,
			ContainerName = _containerName
		};
	}

	[TestMethod]
	public async Task UploadBlob_ErrorOAuth2EmptyCredentials()
	{
		// Setup
		_connection.AuthenticationMethod = AuthenticationMethod.OAuth2;
		_connection.ApplicationId = "";
		_connection.ClientSecret = "";
		_connection.TenantId = "";
		_connection.Uri = "";
		
		var input = new Input
		{
			SourceType = UploadSourceType.File,
			SourceFile = _testFiles[0],
			ActionOnExistingFile = OnExistingFile.Overwrite
		};

		// Act
		var ex = await Assert.ThrowsExactlyAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, _connection, _options, default));

		// Assert
		Assert.Contains("An exception occured.", ex.Message);
	}
}