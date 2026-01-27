using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DownloadBlob.Definitions;

namespace Frends.AzureBlobStorage.DownloadBlob.Helpers;

internal static class ConnectionHandler
{
    internal static BlobClient GetBlobClient(Source source, CancellationToken cancellationToken)
    {
        try
        {
            return source.ConnectionMethod switch
            {
                ConnectionMethod.ConnectionString => GetClientWithConnectionString(source),
                ConnectionMethod.SASToken => GetClientWithSasToken(source),
                ConnectionMethod.OAuth2 => GetClientWithOAuth2(source),
                ConnectionMethod.ArcManagedIdentity => GetClientWithArcManagedIdentity(source),
                ConnectionMethod.ArcManagedIdentityCrossTenant => GetClientWithArcManagedIdentityCrossTenant(source,
                    cancellationToken),
                _ => throw new NotSupportedException(),
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobContainerClient error: ", ex);
        }
    }

    private static Uri GetUri(string uri, string containerName, string blobName) =>
        new($"{uri}/{containerName.ToLower()}/{blobName}");

    private static BlobClient GetClientWithConnectionString(Source source)
    {
        return new BlobClient(source.ConnectionString, source.ContainerName, source.BlobName);
    }

    private static BlobClient GetClientWithSasToken(Source source)
    {
        if (string.IsNullOrWhiteSpace(source.SASToken) || string.IsNullOrWhiteSpace(source.Uri))
            throw new Exception("SAS Token and URI required.");

        return new BlobClient(GetUri(source.Uri, source.ContainerName, source.BlobName),
            new AzureSasCredential(source.SASToken));
    }

    private static BlobClient GetClientWithOAuth2(Source source)
    {
        return new BlobClient(GetUri(source.Uri, source.ContainerName, source.BlobName),
            new ClientSecretCredential(source.TenantID, source.ApplicationID, source.ClientSecret,
                new ClientSecretCredentialOptions()));
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobClient GetClientWithArcManagedIdentity(Source source)
    {
        {
            var credentials = new ManagedIdentityCredential();

            return new BlobClient(GetUri(source.Uri, source.ContainerName, source.BlobName), credentials);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobClient GetClientWithArcManagedIdentityCrossTenant(Source source,
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

            return new BlobClient(GetUri(source.Uri, source.ContainerName, source.BlobName), assertion);
        }
    }
}
