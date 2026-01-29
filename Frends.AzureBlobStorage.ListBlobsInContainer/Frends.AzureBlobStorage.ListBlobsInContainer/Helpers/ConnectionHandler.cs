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
    internal static BlobContainerClient GetBlobContainerClient(Source source, CancellationToken cancellationToken)
    {
        try
        {
            return source.AuthenticationMethod switch
            {
                AuthenticationMethod.ConnectionString => GetClientWithConnectionString(source),
                AuthenticationMethod.SASToken => GetClientWithSasToken(source),
                AuthenticationMethod.OAuth2 => GetClientWithOAuth2(source),
                AuthenticationMethod.ArcManagedIdentity => GetClientWithArcManagedIdentity(source),
                AuthenticationMethod.ArcManagedIdentityCrossTenant => GetClientWithArcManagedIdentityCrossTenant(source,
                    cancellationToken),
                _ => throw new NotSupportedException()
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobContainerClient error: ", ex);
        }
    }

    private static BlobContainerClient GetClientWithConnectionString(Source source)
    {
        return string.IsNullOrWhiteSpace(source.ConnectionString)
            ? throw new Exception("Connection string required.")
            : new BlobContainerClient(source.ConnectionString, source.ContainerName);
    }

    private static BlobContainerClient GetClientWithSasToken(Source source)
    {
        if (string.IsNullOrWhiteSpace(source.SASToken) || string.IsNullOrWhiteSpace(source.URI))
            throw new Exception("SAS Token and URI required.");

        return new BlobContainerClient(new Uri($"{source.URI}/{source.ContainerName}?"),
            new AzureSasCredential(source.SASToken));
    }

    private static BlobContainerClient GetClientWithOAuth2(Source source)
    {
        var credentials = new ClientSecretCredential(source.TenantID, source.ApplicationID,
            source.ClientSecret, new ClientSecretCredentialOptions());
        var blobServiceClient = new BlobServiceClient(new Uri($"{source.URI}"), credentials);

        return blobServiceClient.GetBlobContainerClient(source.ContainerName);
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobContainerClient GetClientWithArcManagedIdentity(Source source)
    {
        {
            var credentials = new ManagedIdentityCredential();
            var blobServiceClient = new BlobServiceClient(new Uri($"{source.URI}"), credentials);

            return blobServiceClient.GetBlobContainerClient(source.ContainerName);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobContainerClient GetClientWithArcManagedIdentityCrossTenant(Source source,
        CancellationToken cancellationToken)
    {
        {
            var credentials = new ManagedIdentityCredential();
            ClientAssertionCredential assertion = new(
                source.TargetTenantId,
                source.TargetClientId,
                async _ =>
                {
                    var tokenRequestContext = new TokenRequestContext(source.Scopes);
                    var accessToken = await credentials
                        .GetTokenAsync(tokenRequestContext, cancellationToken).ConfigureAwait(false);

                    return accessToken.Token;
                });

            var blobServiceClient = new BlobServiceClient(new Uri(source.URI), assertion);

            return blobServiceClient.GetBlobContainerClient(source.ContainerName);
        }
    }

}
