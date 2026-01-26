using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DeleteContainer.Definitions;

namespace Frends.AzureBlobStorage.DeleteContainer.Helpers;

internal static class ConnectionHandler
{
    internal static BlobContainerClient GetBlobContainerClient(Input input, CancellationToken cancellationToken)
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

    private static Uri GetUri(string storageAccountName) => new($"https://{storageAccountName}.blob.core.windows.net");

    private static BlobContainerClient GetClientWithConnectionString(Input input)
    {
        var client = new BlobServiceClient(input.ConnectionString);

        return client.GetBlobContainerClient(input.ContainerName);
    }

    private static BlobContainerClient GetClientWithOAuth2(Input input)
    {
        var credentials = new ClientSecretCredential(input.TenantID, input.ApplicationID, input.ClientSecret,
            new ClientSecretCredentialOptions());
        var client = new BlobServiceClient(GetUri(input.StorageAccountName), credentials);

        return client.GetBlobContainerClient(input.ContainerName);
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobContainerClient GetClientWithArcManagedIdentity(Input input)
    {
        {
            var credentials = new ManagedIdentityCredential();
            var blobServiceClient = new BlobServiceClient(GetUri(input.StorageAccountName), credentials);

            return blobServiceClient.GetBlobContainerClient(input.ContainerName);
        }
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobContainerClient GetClientWithArcManagedIdentityCrossTenant(Input input,
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

            var blobServiceClient = new BlobServiceClient(GetUri(input.StorageAccountName), assertion);

            return blobServiceClient.GetBlobContainerClient(input.ContainerName);
        }
    }

}
