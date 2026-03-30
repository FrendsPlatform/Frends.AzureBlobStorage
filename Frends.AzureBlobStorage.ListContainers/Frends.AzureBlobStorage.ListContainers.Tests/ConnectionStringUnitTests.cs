using Frends.AzureBlobStorage.ListContainers.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ListContainers.Tests;

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
		};
	}

	[TestMethod]
	public async Task ListContainers_ShouldFail_WithInvalidConnectionString()
	{
		// Setup
		var input = new Input
		{
			Prefix = null,
			States = ContainerStateFilter.None,
		};

		_options.ThrowErrorOnFailure = false;
		_connection.ConnectionString = "InvalidConnectionString";

		// Act
		var result = await AzureBlobStorage.ListContainers(input, _connection, _options, default);

		// Assert
		Assert.IsFalse(result.Success);
		Assert.IsNotNull(result.Error);
	}
}
