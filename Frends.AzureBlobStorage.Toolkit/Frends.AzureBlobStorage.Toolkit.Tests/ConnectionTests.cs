using Frends.AzureBlobStorage.Toolkit.Definitions;
using Frends.AzureBlobStorage.Toolkit.Handlers;

namespace Frends.AzureBlobStorage.Toolkit.Tests;

public class ConnectionTests
{
    Connection connection;

    [OneTimeSetUp]
    public void Initialize()
    {
        TestHandler.LoadEnvironmentVariables();
    }

    [SetUp]
    public void Setup()
    {
        connection = new Connection
        {
            ConnectionString = TestHandler.ConnectionString,
            ApplicationId = TestHandler.ClientId,
            TenantId = TestHandler.TenantId,
            ClientSecret = TestHandler.ClientSecret,
            SasToken = TestHandler.SasToken,
            StorageAccountName = TestHandler.StorageAccountName,
        };
    }

    [TestCase(ConnectionMethod.ConnectionString)]
    [TestCase(ConnectionMethod.OAuth2)]
    [TestCase(ConnectionMethod.SasToken)]
    public void Should_Get_BlobServiceClient_Successfully(ConnectionMethod connectionMethod)
    {
        connection.AuthenticationMethod = connectionMethod;
        Assert.DoesNotThrow(() => ConnectionHandler.GetBlobServiceClient(connection, CancellationToken.None));
    }

    [TestCase(ConnectionMethod.ConnectionString)]
    [TestCase(ConnectionMethod.OAuth2)]
    [TestCase(ConnectionMethod.SasToken)]
    public void Should_Get_BlobContainerClient_Successfully(ConnectionMethod connectionMethod)
    {
        connection.AuthenticationMethod = connectionMethod;
        Assert.DoesNotThrow(() => ConnectionHandler.GetBlobContainerClient(connection, "foo", CancellationToken.None));
    }

    [TestCase(ConnectionMethod.ConnectionString)]
    [TestCase(ConnectionMethod.OAuth2)]
    [TestCase(ConnectionMethod.SasToken)]
    public void Should_Get_BlobClient_Successfully(ConnectionMethod connectionMethod)
    {
        connection.AuthenticationMethod = connectionMethod;
        Assert.DoesNotThrow(() => ConnectionHandler.GetBlobClient(connection, "foo", "bar", CancellationToken.None));
    }

    [TestCase(ConnectionMethod.ConnectionString, AzureBlobType.Append)]
    [TestCase(ConnectionMethod.ConnectionString, AzureBlobType.Block)]
    [TestCase(ConnectionMethod.ConnectionString, AzureBlobType.Page)]
    [TestCase(ConnectionMethod.OAuth2, AzureBlobType.Append)]
    [TestCase(ConnectionMethod.OAuth2, AzureBlobType.Block)]
    [TestCase(ConnectionMethod.OAuth2, AzureBlobType.Page)]
    [TestCase(ConnectionMethod.SasToken, AzureBlobType.Append)]
    [TestCase(ConnectionMethod.SasToken, AzureBlobType.Block)]
    [TestCase(ConnectionMethod.SasToken, AzureBlobType.Page)]
    public void Should_Get_BlobBaseClient_Successfully(ConnectionMethod connectionMethod, AzureBlobType blobType)
    {
        connection.AuthenticationMethod = connectionMethod;
        Assert.DoesNotThrow(() =>
            ConnectionHandler.GetBlobBaseClient(connection, "foo", "bar", blobType,
                CancellationToken.None));
    }

    [TestCase(ConnectionMethod.ConnectionString)]
    [TestCase(ConnectionMethod.OAuth2)]
    [TestCase(ConnectionMethod.SasToken)]
    public void Should_Throw_When_Invalid_AzureBlobType(ConnectionMethod connectionMethod)
    {
        connection.AuthenticationMethod = connectionMethod;
        var ex = Assert.Throws<NotSupportedException>(() =>
            ConnectionHandler.GetBlobBaseClient(connection, "foo", "bar", (AzureBlobType)99,
                CancellationToken.None));
        Assert.That(ex.Message, Does.Contain("Specified method is not supported."), ex.Message);
    }

    [TestCase(ConnectionMethod.ConnectionString)]
    [TestCase(ConnectionMethod.OAuth2)]
    [TestCase(ConnectionMethod.SasToken)]
    public void Should_Throw_When_Invalid_ConnectionData(ConnectionMethod method)
    {
        connection = new Connection
        {
            AuthenticationMethod = method,
        };
        var ex = Assert.Throws<ArgumentException>(() =>
            ConnectionHandler.GetBlobServiceClient(connection, CancellationToken.None));
        Assert.That(ex.Message, Does.Contain("GetBlobServiceClient error: "), ex.Message);
    }
}
