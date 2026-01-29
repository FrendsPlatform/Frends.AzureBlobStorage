using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DeleteBlob.Definitions;

namespace Frends.AzureBlobStorage.DeleteBlob.Helpers;

internal static class ConnectionHandler
{
    private static Uri GetUri(string storageAccountName, string containerName, string blobName) =>
        new($"https://{storageAccountName}.blob.core.windows.net/{containerName}/{blobName}");

    internal static BlobClient GetBlobContainerClient(Input input, CancellationToken cancellationToken)
    {
        try
        {
            return input.ConnectionMethod switch
            {
                ConnectionMethod.ConnectionString => GetClientWithConnectionString(input),
                ConnectionMethod.OAuth2 => GetClientWithOAuth2(input),
                ConnectionMethod.ArcManagedIdentity => GetClientWithArcManagedIdentity(input),
                ConnectionMethod.ArcManagedIdentityCrossTenant => GetClientWithArcManagedIdentityCrossTenant(input,
                    cancellationToken),
                _ => throw new NotSupportedException(),
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobContainerClient error: ", ex);
        }
    }

    private static BlobClient GetClientWithConnectionString(Input input)
    {
        return new BlobClient(input.ConnectionString, input.ContainerName, input.BlobName);
    }

    private static BlobClient GetClientWithOAuth2(Input input)
    {
        var credentials = new ClientSecretCredential(input.TenantID, input.ApplicationID, input.ClientSecret,
            new ClientSecretCredentialOptions());

        return new BlobClient(GetUri(input.StorageAccountName, input.ContainerName, input.BlobName), credentials);
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobClient GetClientWithArcManagedIdentity(Input input)
    {
        {
            var credentials = new ManagedIdentityCredential();

            return new BlobClient(GetUri(input.StorageAccountName, input.ContainerName, input.BlobName), credentials);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobClient GetClientWithArcManagedIdentityCrossTenant(Input input,
        CancellationToken cancellationToken)
    {
        {
            var credentials = new ManagedIdentityCredential();
            ClientAssertionCredential assertion = new(
                input.TargetTenantId,
                input.TargetClientId,
                async _ =>
                {
                    var tokenRequestContext = new TokenRequestContext(input.Scopes);
                    var accessToken = await credentials
                        .GetTokenAsync(tokenRequestContext, cancellationToken).ConfigureAwait(false);

                    return accessToken.Token;
                });

            return new BlobClient(GetUri(input.StorageAccountName, input.ContainerName, input.BlobName), assertion);
        }
    }

}
