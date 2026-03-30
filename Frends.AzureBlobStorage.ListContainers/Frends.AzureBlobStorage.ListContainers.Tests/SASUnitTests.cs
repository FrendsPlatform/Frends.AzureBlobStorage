using Frends.AzureBlobStorage.ListContainers.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ListContainers.Tests;

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
			Uri = _uri
		};
	}

	[TestMethod]
	public async Task ListContainers_ShouldThrow_WithMissingSasToken()
	{
		// Setup
		var input = new Input
		{
			Prefix = null,
			States = ContainerStateFilter.None,
		};
		_connection.SasToken = string.Empty;

		// Act & Assert
		await Assert.ThrowsExactlyAsync<Exception>(async () =>
			await AzureBlobStorage.ListContainers(input, _connection, _options, CancellationToken.None), 
			"Expected an exception when SAS token is missing.");
	}
}
