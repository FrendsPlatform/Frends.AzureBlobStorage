using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Frends.AzureBlobStorage.CreateContainer.Definitions;

namespace Frends.AzureBlobStorage.CreateContainer.Helpers;

/// <summary>
/// Connection handler to connect with Azure Blob Storage.
/// </summary>
public static class ConnectionHandler
{
    /// <summary>
    /// Get Blob Container Client.
    /// </summary>
    /// <param name="connection">Connection task parameters</param>
    /// <param name="containerName">container name from input parameter</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>BlobContainerClient object</returns>
    public static BlobContainerClient GetBlobContainerClient(
        Connection connection,
        string containerName,
        CancellationToken cancellationToken)
    {
        var serviceClient = GetBlobServiceClient(connection, cancellationToken);

        return serviceClient.GetBlobContainerClient(containerName);
    }

    /// <summary>
    /// Get Blob Service Client.
    /// </summary>
    /// <param name="connection">Connection task parameters</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>BlobServiceClient object</returns>
    private static BlobServiceClient GetBlobServiceClient(Connection connection, CancellationToken cancellationToken)
    {
        try
        {
            return connection.AuthenticationMethod switch
            {
                ConnectionMethod.ConnectionString => GetBlobServiceClientWithConnectionString(connection),
                ConnectionMethod.SasToken => GetBlobServiceClientWithSasToken(connection),
                ConnectionMethod.OAuth2 => GetBlobServiceClientWithOAuth2(connection),
                ConnectionMethod.ArcManagedIdentity => GetBlobServiceClientWithArcManagedIdentity(connection),
                ConnectionMethod.ArcManagedIdentityCrossTenant => GetBlobServiceClientWithArcManagedIdentityCrossTenant(
                    connection,
                    cancellationToken),
                _ => throw new NotSupportedException(),
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"GetBlobServiceClient error: {ex.Message}", ex);
        }
    }

    private static BlobServiceClient GetBlobServiceClientWithConnectionString(Connection connection)
    {
        return new BlobServiceClient(connection.ConnectionString);
    }

    private static BlobServiceClient GetBlobServiceClientWithSasToken(Connection connection)
    {
        return new BlobServiceClient(GetUri(connection.StorageAccountName, connection.SasToken));
    }

    private static BlobServiceClient GetBlobServiceClientWithOAuth2(Connection connection)
    {
        return new BlobServiceClient(
            GetUri(connection.StorageAccountName),
            new ClientSecretCredential(
                connection.TenantId,
                connection.ApplicationId,
                connection.ClientSecret,
                new ClientSecretCredentialOptions()));
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobServiceClient GetBlobServiceClientWithArcManagedIdentity(Connection connection)
    {
        {
            var credentials = new ManagedIdentityCredential();

            return new BlobServiceClient(GetUri(connection.StorageAccountName), credentials);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobServiceClient GetBlobServiceClientWithArcManagedIdentityCrossTenant(
        Connection connection,
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

            return new BlobServiceClient(GetUri(connection.StorageAccountName), assertion);
        }
    }

    private static Uri GetUri(string storageAccountName, string sasToken = null)
    {
        return sasToken is null
            ? new Uri($"https://{storageAccountName}.blob.core.windows.net")
            : new Uri($"https://{storageAccountName}.blob.core.windows.net?{sasToken}");
    }
}
