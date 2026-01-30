using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.ReadBlob.Definitions;

namespace Frends.AzureBlobStorage.ReadBlob.Helpers;

internal static class ConnectionHandler
{
    internal static BlobClient GetBlobClient(Source source, CancellationToken cancellationToken)
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
                _ => throw new NotSupportedException(),
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"GetBlobContainerClient error: {ex.Message}", ex);
        }
    }

    private static Uri GetUri(Source source) => new(
        $"https://{source.StorageAccountName}.blob.core.windows.net/{source.ContainerName}/{source.BlobName}");

    private static BlobClient GetClientWithConnectionString(Source source)
    {
        return string.IsNullOrWhiteSpace(source.ConnectionString)
            ? throw new Exception("Connection string required.")
            : new BlobClient(source.ConnectionString, source.ContainerName, source.BlobName);
    }

    private static BlobClient GetClientWithSasToken(Source source)
    {
        if (string.IsNullOrWhiteSpace(source.SASToken) || string.IsNullOrWhiteSpace(source.URI))
            throw new Exception("SAS Token and URI required.");
        var uri = $"{source.URI}/{source.ContainerName}/{source.BlobName}?";

        return new BlobClient(new Uri(uri), new AzureSasCredential(source.SASToken));
    }

    private static BlobClient GetClientWithOAuth2(Source source)
    {
        if (source.ApplicationID is null || source.ClientSecret is null || source.TenantID is null ||
            source.StorageAccountName is null)
            throw new Exception("ApplicationID, ClientSecret, TenantID and StorageAccountName required.");
        var credentials = new ClientSecretCredential(source.TenantID, source.ApplicationID, source.ClientSecret,
            new ClientSecretCredentialOptions());

        return new BlobClient(GetUri(source), credentials);
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobClient GetClientWithArcManagedIdentity(Source source)
    {
        {
            var credentials = new ManagedIdentityCredential();

            return new BlobClient(GetUri(source), credentials);
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

            return new BlobClient(GetUri(source), assertion);
        }
    }

}
