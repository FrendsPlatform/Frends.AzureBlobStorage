using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.ListContainers.Definitions;

namespace Frends.AzureBlobStorage.ListContainers.Helpers;

// Internal class doesn't have to be documented
#pragma warning disable SA1600
internal static class ConnectionHandler
{
	internal static BlobServiceClient GetBlobServiceClient(Connection connection, CancellationToken cancellationToken)
#pragma warning restore SA1611
    {
        try
        {
            return connection.AuthenticationMethod switch
            {
                AuthenticationMethod.ConnectionString => GetClientWithConnectionString(connection),
                AuthenticationMethod.SASToken => GetClientWithSasToken(connection),
                AuthenticationMethod.OAuth2 => GetClientWithOAuth2(connection),
                AuthenticationMethod.ArcManagedIdentity => GetClientWithArcManagedIdentity(connection),
                AuthenticationMethod.ArcManagedIdentityCrossTenant => GetClientWithArcManagedIdentityCrossTenant(connection, cancellationToken),
				AuthenticationMethod.DefaultManagedIdentity => GetClientWithDefaultManagedIdentity(connection),
				_ => throw new NotSupportedException(),
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobServiceClient error: ", ex);
        }
    }

    private static BlobServiceClient GetClientWithConnectionString(Connection connection)
    {
        return new BlobServiceClient(connection.ConnectionString);
    }

    private static BlobServiceClient GetClientWithSasToken(Connection connection)
    {
        return new BlobServiceClient(new Uri($"{connection.Uri.TrimEnd('/')}?{connection.SasToken}"));
    }

    private static BlobServiceClient GetClientWithOAuth2(Connection connection)
    {
        return new BlobServiceClient(
            new Uri(connection.Uri),
            new ClientSecretCredential(
                connection.TenantId,
                connection.ApplicationId,
                connection.ClientSecret,
                new ClientSecretCredentialOptions()));
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobServiceClient GetClientWithArcManagedIdentity(Connection connection)
    {
		var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
		return new BlobServiceClient(new Uri($"{connection.Uri}"), credentials);
    }

	[ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
	private static BlobServiceClient GetClientWithArcManagedIdentityCrossTenant(
		Connection connection,
		CancellationToken cancellationToken)
	{
		var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
		ClientAssertionCredential assertion = new(
			connection.TargetTenantId,
			connection.TargetClientId,
			async _ =>
			{
				var tokenRequestContext = new TokenRequestContext(connection.Scopes);
				var accessToken = await credentials
					.GetTokenAsync(tokenRequestContext, cancellationToken).ConfigureAwait(false);

				return accessToken.Token;
			});

		return new BlobServiceClient(new Uri(connection.Uri), assertion);

	}

	private static BlobServiceClient GetClientWithDefaultManagedIdentity(Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.Uri))
		{
			throw new Exception("URI is required.");
		}

		return new BlobServiceClient(new Uri(connection.Uri.TrimEnd('/')), new DefaultAzureCredential());
	}
}
#pragma warning restore SA1600
