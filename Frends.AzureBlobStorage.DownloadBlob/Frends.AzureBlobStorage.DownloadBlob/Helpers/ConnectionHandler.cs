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
	private static Uri GetUri(string uri, string containerName, string blobName) =>
		new($"{uri}/{containerName}/{blobName}");

	internal static BlobClient GetBlobClient(Input input, Connection connection, CancellationToken cancellationToken)
	{
		try
		{
			return connection.AuthenticationMethod switch
			{
				AuthenticationMethod.ConnectionString => GetClientWithConnectionString(input, connection),
				AuthenticationMethod.SASToken => GetClientWithSasToken(input, connection),
				AuthenticationMethod.OAuth2 => GetClientWithOAuth2(input, connection),
				AuthenticationMethod.ArcManagedIdentity => GetClientWithArcManagedIdentity(input, connection),
				AuthenticationMethod.ArcManagedIdentityCrossTenant => GetClientWithArcManagedIdentityCrossTenant(input, connection, cancellationToken),
				AuthenticationMethod.DefaultManagedIdentity => GetClientWithDefaultManagedIdentity(input, connection),
				_ => throw new NotSupportedException(),
			};
		}
		catch (Exception ex)
		{
			throw new ArgumentException($"GetBlobContainerClient error: {ex.Message}", ex);
		}
	}

	private static BlobClient GetClientWithConnectionString(Input input, Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.ConnectionString))
		{
			throw new Exception("Connection string required.");
		}

		return new BlobClient(connection.ConnectionString, connection.ContainerName, input.BlobName);
	}

	private static BlobClient GetClientWithSasToken(Input input, Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.SasToken) || string.IsNullOrWhiteSpace(connection.Uri))
		{
			throw new Exception("SAS Token and URI required.");
		}

		return new BlobClient(GetUri(connection.Uri, connection.ContainerName, input.BlobName), new AzureSasCredential(connection.SasToken));
	}


	private static BlobClient GetClientWithOAuth2(Input input, Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.ApplicationId) || string.IsNullOrWhiteSpace(connection.TenantId) || string.IsNullOrWhiteSpace(connection.ClientSecret))
		{
			throw new Exception("Application ID, Tenant ID and Client Secret required.");
		}

		var credentials = new ClientSecretCredential(connection.TenantId, connection.ApplicationId, connection.ClientSecret,
			new ClientSecretCredentialOptions());

		return new BlobClient(GetUri(connection.Uri, connection.ContainerName, input.BlobName), credentials);
	}

	[ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
	private static BlobClient GetClientWithArcManagedIdentity(Input input, Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.Uri))
		{
			throw new Exception("URI required.");
		}

		var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
		return new BlobClient(GetUri(connection.Uri, connection.ContainerName, input.BlobName), credentials);
	}

	[ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
	private static BlobClient GetClientWithArcManagedIdentityCrossTenant(Input input,
		Connection connection,
		CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(connection.Uri) || string.IsNullOrWhiteSpace(connection.TargetTenantId) || string.IsNullOrWhiteSpace(connection.TargetClientId))
		{
			throw new Exception("URI, Target Tenant ID and Target Client ID required.");
		}

		var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
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

		return new BlobClient(GetUri(connection.Uri, connection.ContainerName, input.BlobName), assertion);
	}

	private static BlobClient GetClientWithDefaultManagedIdentity(
		Input input,
		Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.Uri))
		{
			throw new Exception("URI is required.");
		}

		return new BlobClient(GetUri(connection.Uri, connection.ContainerName, input.BlobName), new DefaultAzureCredential());
	}
}
