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
            return connection.ConnectionMethod switch
            {
                ConnectionMethod.ConnectionString => GetClientWithConnectionString(connection),
                ConnectionMethod.SasToken => GetClientWithSasToken(connection),
                ConnectionMethod.OAuth2 => GetClientWithOAuth2(connection),
                ConnectionMethod.ArcManagedIdentity => GetClientWithArcManagedIdentity(connection),
                ConnectionMethod.ArcManagedIdentityCrossTenant => GetClientWithArcManagedIdentityCrossTenant(
                    connection,
                    cancellationToken),
                _ => throw new NotSupportedException(),
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobContainerClient error: ", ex);
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
        {
            var credentials = new ManagedIdentityCredential();

            return new BlobServiceClient(new Uri($"{connection.Uri}"), credentials);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobServiceClient GetClientWithArcManagedIdentityCrossTenant(
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

            return new BlobServiceClient(new Uri(connection.Uri), assertion);
        }
    }
}
#pragma warning restore SA1600
