using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Frends.AzureBlobStorage.ListContainers.Definitions;
using Frends.AzureBlobStorage.ListContainers.Helpers;

namespace Frends.AzureBlobStorage.ListContainers;

/// <summary>
/// Azure Blob Storage Task.
/// </summary>
public static class AzureBlobStorage
{
    /// <summary>
    /// Frends Task to list all containers in the specified Azure Blob Storage account.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-AzureBlobStorage-ListContainers)
    /// </summary>
    /// <param name="input">Parameters for container state and prefix filtering.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>object { bool Success, List&lt;ContainerInfo&gt; Containers, Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> ListContainers(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        var containers = new List<ContainerInfo>();

        try
        {
            CheckParameters(connection);

            BlobServiceClient serviceClient = connection.ConnectionMethod switch
            {
                ConnectionMethod.ConnectionString => new BlobServiceClient(connection.ConnectionString),
                ConnectionMethod.SasToken => new BlobServiceClient(new Uri($"{connection.Uri.TrimEnd('/')}?{connection.SasToken}")),
                ConnectionMethod.OAuth2 => new BlobServiceClient(
                    new Uri(connection.Uri),
                    new ClientSecretCredential(
                        connection.TenantId,
                        connection.ApplicationId,
                        connection.ClientSecret,
                        new ClientSecretCredentialOptions())),
                _ => throw new Exception("Invalid or unsupported connection method.")
            };

            await foreach (BlobContainerItem container in serviceClient.GetBlobContainersAsync(
                traits: BlobContainerTraits.None,
                states: (BlobContainerStates)input.States,
                prefix: input.Prefix,
                cancellationToken: cancellationToken))
            {
                containers.Add(new ContainerInfo
                {
                    Name = container.Name,
                    PublicAccess = container.Properties.PublicAccess?.ToString() ?? "Private",
                    LastModified = container.Properties.LastModified.DateTime,
                    ETag = container.Properties.ETag.ToString(),
                    LeaseState = container.Properties.LeaseState?.ToString() ?? string.Empty,
                    LeaseStatus = container.Properties.LeaseStatus?.ToString() ?? string.Empty,
                });
            }

            return new Result { Success = true, Containers = containers, Error = null };
        }
        catch (Exception ex)
        {
            return ErrorHandler.Handle(ex, options);
        }
    }

    private static void CheckParameters(Connection connection)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        switch (connection.ConnectionMethod)
        {
            case ConnectionMethod.ConnectionString:
                if (string.IsNullOrWhiteSpace(connection.ConnectionString))
                    throw new Exception("Connection string cannot be empty.");
                break;

            case ConnectionMethod.SasToken:
                if (string.IsNullOrWhiteSpace(connection.Uri) || string.IsNullOrWhiteSpace(connection.SasToken))
                    throw new Exception("Both URI and SAS token are required for SAS Token connection method.");
                break;

            case ConnectionMethod.OAuth2:
                if (string.IsNullOrWhiteSpace(connection.Uri) ||
                    string.IsNullOrWhiteSpace(connection.TenantId) ||
                    string.IsNullOrWhiteSpace(connection.ApplicationId) ||
                    string.IsNullOrWhiteSpace(connection.ClientSecret))
                    throw new Exception("URI, TenantId, ApplicationId, and ClientSecret are required for OAuth2 connection method.");
                break;

            default:
                throw new Exception("Invalid or unsupported connection method.");
        }
    }
}
