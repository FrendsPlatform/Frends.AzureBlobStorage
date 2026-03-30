using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.CreateContainer.Definitions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Frends.AzureBlobStorage.CreateContainer.Helpers;

internal static class ConnectionHandler
{
	private static Uri GetUri(string uri, string containerName) => new($"{uri}/{containerName}");

	internal static BlobContainerClient GetBlobContainerClient(Input input, Connection connection, CancellationToken cancellationToken)
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
				_ => throw new NotSupportedException()
			};
		}
		catch (Exception ex)
		{
			throw new ArgumentException("GetBlobContainerClient error: ", ex);
		}
	}

	private static BlobContainerClient GetClientWithConnectionString(Input input, Connection connection)
	{
		return string.IsNullOrWhiteSpace(connection.ConnectionString)
			? throw new Exception("Connection string required.")
			: new BlobContainerClient(connection.ConnectionString, input.ContainerName);
	}

	private static BlobContainerClient GetClientWithSasToken(Input input, Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.SasToken) || string.IsNullOrWhiteSpace(connection.Uri))
			throw new Exception("SAS Token and URI required.");

		return new BlobContainerClient(new Uri($"{connection.Uri}/{input.ContainerName}?"),
			new AzureSasCredential(connection.SasToken));
	}

	private static BlobContainerClient GetClientWithOAuth2(Input input, Connection connection)
	{
		var credentials = new ClientSecretCredential(connection.TenantId, connection.ApplicationId,
			connection.ClientSecret, new ClientSecretCredentialOptions());
		var blobServiceClient = new BlobServiceClient(new Uri($"{connection.Uri}"), credentials);

		return blobServiceClient.GetBlobContainerClient(input.ContainerName);
	}

	[ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
	private static BlobContainerClient GetClientWithArcManagedIdentity(Input input, Connection connection)
	{
		var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
		var blobServiceClient = new BlobServiceClient(new Uri($"{connection.Uri}"), credentials);

		return blobServiceClient.GetBlobContainerClient(input.ContainerName);
	}

	[ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
	private static BlobContainerClient GetClientWithArcManagedIdentityCrossTenant(Input input, Connection connection,
		CancellationToken cancellationToken)
	{
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

		var blobServiceClient = new BlobServiceClient(new Uri(connection.Uri), assertion);

		return blobServiceClient.GetBlobContainerClient(input.ContainerName);
	}

	private static BlobContainerClient GetClientWithDefaultManagedIdentity(Input input, Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.Uri))
		{
			throw new Exception("URI is required.");
		}

		return new BlobContainerClient(GetUri(connection.Uri,input.ContainerName), new DefaultAzureCredential());
	}
}
