using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.CreateContainer.Definitions;

namespace Frends.AzureBlobStorage.CreateContainer.Helpers;

internal static class ConnectionHandler
{
    internal static BlobContainerClient GetBlobContainerClient(Connection connection, Input input,
        CancellationToken cancellationToken)
    {
        try
        {
            return connection.AuthenticationMethod switch
            {
                ConnectionMethod.ConnectionString => GetClientWithConnectionString(connection, input),
                ConnectionMethod.OAuth2 => GetClientWithOAuth2(connection, input),
                ConnectionMethod.ArcManagedIdentity => GetClientWithArcManagedIdentity(connection, input),
                ConnectionMethod.ArcManagedIdentityCrossTenant => GetClientWithArcManagedIdentityCrossTenant(connection,
                    input, cancellationToken),
                _ => throw new NotSupportedException(),
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobContainerClient error: ", ex);
        }
    }

    private static Uri GetUri(string storageAccountName) => new($"https://{storageAccountName}.blob.core.windows.net");

    private static BlobContainerClient GetClientWithConnectionString(Connection connection, Input input)
    {
        var blobServiceClient = new BlobServiceClient(connection.ConnectionString);

        return blobServiceClient.GetBlobContainerClient(input.ContainerName);
    }

    private static BlobContainerClient GetClientWithOAuth2(Connection connection, Input input)
    {
        var credentials = new ClientSecretCredential(connection.TenantId, connection.ApplicationId,
            connection.ClientSecret, new ClientSecretCredentialOptions());
        var blobServiceClient =
            new BlobServiceClient(GetUri(connection.StorageAccountName),
                credentials);

        return blobServiceClient.GetBlobContainerClient(input.ContainerName);
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobContainerClient GetClientWithArcManagedIdentity(Connection connection, Input input)
    {
        {
            var credentials = new ManagedIdentityCredential();
            var blobServiceClient = new BlobServiceClient(
                GetUri(connection.StorageAccountName),
                credentials);

            return blobServiceClient.GetBlobContainerClient(input.ContainerName);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobContainerClient GetClientWithArcManagedIdentityCrossTenant(Connection connection, Input input,
        CancellationToken cancellationToken)
    {
        {
            var credentials = new ManagedIdentityCredential();
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

            var blobServiceClient = new BlobServiceClient(
                GetUri(connection.StorageAccountName),
                assertion);

            return blobServiceClient.GetBlobContainerClient(input.ContainerName);
        }
    }
}
