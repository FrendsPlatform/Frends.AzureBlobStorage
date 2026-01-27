using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.WriteBlob.Definitions;
using Frends.AzureBlobStorage.WriteBlob.Enums;

namespace Frends.AzureBlobStorage.WriteBlob.Helpers;

internal static class ConnectionHandler
{
    internal static BlobClient GetBlobClient(Destination destination, CancellationToken cancellationToken)
    {
        try
        {
            return destination.ConnectionMethod switch
            {
                ConnectionMethod.ConnectionString => GetBlobClientWithConnectionString(destination),
                ConnectionMethod.SASToken => GetBlobClientWithSasToken(destination),
                ConnectionMethod.OAuth2 => GetBlobClientWithOAuth2(destination),
                ConnectionMethod.ArcManagedIdentity => GetBlobClientWithArcManagedIdentity(destination),
                ConnectionMethod.ArcManagedIdentityCrossTenant => GetBlobClientWithArcManagedIdentityCrossTenant(
                    destination,
                    cancellationToken),
                _ => throw new NotSupportedException()
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobContainerClient error: ", ex);
        }
    }

    internal static BlobServiceClient GetBlobServiceClient(Destination destination, CancellationToken cancellationToken)
    {
        try
        {
            return destination.ConnectionMethod switch
            {
                ConnectionMethod.ConnectionString => GetServiceBlobClientWithConnectionString(destination),
                ConnectionMethod.OAuth2 => GetBlobServiceClientWithOAuth2(destination),
                ConnectionMethod.ArcManagedIdentity => GetBlobServiceClientWithArcManagedIdentity(destination),
                ConnectionMethod.ArcManagedIdentityCrossTenant => GetBlobServiceClientWithArcManagedIdentityCrossTenant(
                    destination,
                    cancellationToken),
                _ => throw new NotSupportedException(),
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobContainerClient error: ", ex);
        }
    }

    private static BlobServiceClient GetServiceBlobClientWithConnectionString(Destination destination)
    {
        return new BlobServiceClient(destination.ConnectionString);
    }

    private static BlobClient GetBlobClientWithConnectionString(Destination destination)
    {
        return new BlobClient(destination.ConnectionString, destination.ContainerName.ToLower(), destination.BlobName);
    }

    private static BlobClient GetBlobClientWithSasToken(Destination destination)
    {
        var blobContainerClient = new BlobContainerClient(new Uri($"{destination.Uri}/{destination.ContainerName}?"),
            new AzureSasCredential(destination.SASToken));

        return blobContainerClient.GetBlobClient(destination.BlobName);
    }

    private static BlobServiceClient GetBlobServiceClientWithOAuth2(Destination destination)
    {
        var serviceUri = new Uri($"{destination.Uri}");
        var credentials = new ClientSecretCredential(destination.TenantID, destination.ApplicationID,
            destination.ClientSecret, new ClientSecretCredentialOptions());

        return new BlobServiceClient(serviceUri, credentials);
    }

    private static BlobClient GetBlobClientWithOAuth2(Destination destination)
    {
        var credentials = new ClientSecretCredential(destination.TenantID, destination.ApplicationID,
            destination.ClientSecret, new ClientSecretCredentialOptions());
        var uri = new Uri($"{destination.Uri}/{destination.ContainerName.ToLower()}/{destination.BlobName}");

        return new BlobClient(uri, credentials);
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobClient GetBlobClientWithArcManagedIdentity(Destination destination)
    {
        {
            var credentials = new ManagedIdentityCredential();
            var uri = new Uri($"{destination.Uri}/{destination.ContainerName.ToLower()}/{destination.BlobName}");

            return new BlobClient(uri, credentials);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobServiceClient GetBlobServiceClientWithArcManagedIdentity(Destination destination)
    {
        {
            var credentials = new ManagedIdentityCredential();
            var uri = new Uri($"{destination.Uri}/{destination.ContainerName.ToLower()}/{destination.BlobName}");

            return new BlobServiceClient(uri, credentials);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobClient GetBlobClientWithArcManagedIdentityCrossTenant(Destination destination,
        CancellationToken cancellationToken)
    {
        {
            var credentials = new ManagedIdentityCredential();
            ClientAssertionCredential assertion = new(
                destination.TargetTenantId,
                destination.TargetClientId,
                async _ =>
                {
                    var tokenRequestContext = new TokenRequestContext(destination.Scopes);
                    var accessToken = await credentials
                        .GetTokenAsync(tokenRequestContext, cancellationToken).ConfigureAwait(false);

                    return accessToken.Token;
                });
            var uri = new Uri($"{destination.Uri}/{destination.ContainerName.ToLower()}/{destination.BlobName}");


            return new BlobClient(uri, assertion);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobServiceClient GetBlobServiceClientWithArcManagedIdentityCrossTenant(Destination destination,
        CancellationToken cancellationToken)
    {
        {
            var credentials = new ManagedIdentityCredential();
            ClientAssertionCredential assertion = new(
                destination.TargetTenantId,
                destination.TargetClientId,
                async _ =>
                {
                    var tokenRequestContext = new TokenRequestContext(destination.Scopes);
                    var accessToken = await credentials
                        .GetTokenAsync(tokenRequestContext, cancellationToken).ConfigureAwait(false);

                    return accessToken.Token;
                });
            var uri = new Uri($"{destination.Uri}/{destination.ContainerName.ToLower()}/{destination.BlobName}");

            return new BlobServiceClient(uri, assertion);
        }
    }
}
