using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Helpers;

internal static class ConnectionHandler
{
	private static Uri GetUri(string uri, string containerName) => new($"{uri}/{containerName}");

	internal static BlobContainerClient GetBlobContainerClient(Connection connection, CancellationToken cancellationToken)
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
				_ => throw new NotSupportedException()
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobContainerClient error: ", ex);
        }
    }

    private static BlobContainerClient GetClientWithConnectionString(Connection connection)
    {
        return string.IsNullOrWhiteSpace(connection.ConnectionString)
            ? throw new Exception("Connection string required.")
            : new BlobContainerClient(connection.ConnectionString, connection.ContainerName);
    }

    private static BlobContainerClient GetClientWithSasToken(Connection connection)
    {
        if (string.IsNullOrWhiteSpace(connection.SasToken) || string.IsNullOrWhiteSpace(connection.Uri))
            throw new Exception("SAS Token and URI required.");

        return new BlobContainerClient(new Uri($"{connection.Uri}/{connection.ContainerName}?"),
            new AzureSasCredential(connection.SasToken));
    }

    private static BlobContainerClient GetClientWithOAuth2(Connection connection)
    {
        var credentials = new ClientSecretCredential(connection.TenantId, connection.ApplicationId,
            connection.ClientSecret, new ClientSecretCredentialOptions());
        var blobServiceClient = new BlobServiceClient(new Uri($"{connection.Uri}"), credentials);

        return blobServiceClient.GetBlobContainerClient(connection.ContainerName);
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobContainerClient GetClientWithArcManagedIdentity(Connection connection)
    {
        var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
        var blobServiceClient = new BlobServiceClient(new Uri($"{connection.Uri}"), credentials);

        return blobServiceClient.GetBlobContainerClient(connection.ContainerName);
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobContainerClient GetClientWithArcManagedIdentityCrossTenant(Connection connection,
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

        var blobServiceClient = new BlobServiceClient(new Uri(connection.Uri), assertion);

        return blobServiceClient.GetBlobContainerClient(connection.ContainerName);
    }

	private static BlobContainerClient GetClientWithDefaultManagedIdentity(Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.Uri))
		{
			throw new Exception("URI is required.");
		}

		return new BlobContainerClient(GetUri(connection.Uri, connection.ContainerName), new DefaultAzureCredential());
	}

}
