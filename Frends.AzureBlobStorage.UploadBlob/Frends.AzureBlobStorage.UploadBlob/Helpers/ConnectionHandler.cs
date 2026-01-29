using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Frends.AzureBlobStorage.UploadBlob.Definitions;

namespace Frends.AzureBlobStorage.UploadBlob.Helpers;

internal static class ConnectionHandler
{
    internal static BlobBaseClient GetBlobClient(Connection connection, Options options, string blobName,
        CancellationToken cancellationToken)
    {
        try
        {
            return connection.ConnectionMethod switch
            {
                ConnectionMethod.ConnectionString => GetClientWithConnectionString(connection, options, blobName),
                ConnectionMethod.SasToken => GetClientWithSasToken(connection, options, blobName),
                ConnectionMethod.OAuth2 => GetClientWithOAuth2(connection, options, blobName),
                ConnectionMethod.ArcManagedIdentity => GetClientWithArcManagedIdentity(connection, options, blobName),
                ConnectionMethod.ArcManagedIdentityCrossTenant => GetClientWithArcManagedIdentityCrossTenant(connection,
                    options, blobName,
                    cancellationToken),
                _ => throw new NotSupportedException(),
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("GetBlobContainerClient error: ", ex);
        }
    }

    private static BlobBaseClient GetClientWithConnectionString(Connection connection, Options options, string blobName)
    {
        var containerClient = new BlobContainerClient(connection.ConnectionString, connection.ContainerName.ToLower());

        return options.BlobType switch
        {
            AzureBlobType.Append => containerClient.GetAppendBlobClient(blobName),
            AzureBlobType.Block => containerClient.GetBlobClient(blobName),
            AzureBlobType.Page => containerClient.GetPageBlobClient(blobName),
            _ => throw new NotSupportedException(),
        };
    }

    private static BlobBaseClient GetClientWithSasToken(Connection connection, Options options, string blobName)
    {
        var containerClient = new BlobContainerClient(new Uri($"{connection.Uri}/{connection.ContainerName}?"),
            new AzureSasCredential(connection.SasToken));

        return options.BlobType switch
        {
            AzureBlobType.Append => containerClient.GetAppendBlobClient(blobName),
            AzureBlobType.Block => containerClient.GetBlobClient(blobName),
            AzureBlobType.Page => containerClient.GetPageBlobClient(blobName),
            _ => throw new NotSupportedException(),
        };
    }

    private static BlobBaseClient GetClientWithOAuth2(Connection connection, Options options, string blobName)
    {
        var credentials = new ClientSecretCredential(connection.TenantId, connection.ApplicationId,
            connection.ClientSecret,
            new ClientSecretCredentialOptions());
        Uri uri = new($"{connection.Uri}/{connection.ContainerName.ToLower()}/{blobName}");

        return options.BlobType switch
        {
            AzureBlobType.Append => new AppendBlobClient(uri, credentials),
            AzureBlobType.Block => new BlobClient(uri, credentials),
            AzureBlobType.Page => new PageBlobClient(uri, credentials),
            _ => throw new NotSupportedException(),
        };
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobBaseClient GetClientWithArcManagedIdentity(Connection connection, Options options,
        string blobName)
    {
        var credentials = new ManagedIdentityCredential();
        Uri uri = new($"{connection.Uri}/{connection.ContainerName.ToLower()}/{blobName}");

        return options.BlobType switch
        {
            AzureBlobType.Append => new AppendBlobClient(uri, credentials),
            AzureBlobType.Block => new BlobClient(uri, credentials),
            AzureBlobType.Page => new PageBlobClient(uri, credentials),
            _ => throw new NotSupportedException(),
        };
    }

    [ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
    private static BlobBaseClient GetClientWithArcManagedIdentityCrossTenant(Connection connection, Options options,
        string blobName,
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
            Uri uri = new($"{connection.Uri}/{connection.ContainerName.ToLower()}/{blobName}");

            return options.BlobType switch
            {
                AzureBlobType.Append => new AppendBlobClient(uri, assertion),
                AzureBlobType.Block => new BlobClient(uri, assertion),
                AzureBlobType.Page => new PageBlobClient(uri, assertion),
                _ => throw new NotSupportedException(),
            };
        }
    }

}
