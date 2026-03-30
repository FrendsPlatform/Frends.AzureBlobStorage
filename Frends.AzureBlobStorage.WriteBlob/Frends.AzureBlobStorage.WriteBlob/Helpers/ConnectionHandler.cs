using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.WriteBlob.Definitions;

namespace Frends.AzureBlobStorage.WriteBlob.Helpers;
internal static class ConnectionHandler
{
	private static Uri GetUri(string uri, string containerName, string blobName) =>
		new($"{uri}/{containerName}/{blobName}");

	internal static BlobClient GetBlobClient(Connection connection, CancellationToken cancellationToken)
	{
		try
		{
			return connection.AuthenticationMethod switch
			{
				AuthenticationMethod.ConnectionString => GetBlobClientWithConnectionString(connection),
				AuthenticationMethod.SASToken => GetBlobClientWithSasToken(connection),
				AuthenticationMethod.OAuth2 => GetBlobClientWithOAuth2(connection),
				AuthenticationMethod.ArcManagedIdentity => GetBlobClientWithArcManagedIdentity(connection),
				AuthenticationMethod.ArcManagedIdentityCrossTenant => GetBlobClientWithArcManagedIdentityCrossTenant(connection, cancellationToken),
				AuthenticationMethod.DefaultManagedIdentity => GetClientWithDefaultManagedIdentity(connection),
				_ => throw new NotSupportedException()
			};
		}
		catch (Exception ex)
		{
			throw new ArgumentException($"GetBlobClient error: {ex.Message}", ex);
		}
	}

	internal static BlobContainerClient GetBlobContainerClient(Connection connection, CancellationToken cancellationToken = default)
	{
		try
		{
			var containerName = connection.ContainerName.ToLower();
			return connection.AuthenticationMethod switch
			{
				AuthenticationMethod.SASToken => new BlobContainerClient(
					new Uri($"{connection.Uri}/{containerName}?"),
					new AzureSasCredential(connection.SasToken)),
				_ => GetServiceClient(connection, cancellationToken).GetBlobContainerClient(containerName)
			};
		}
		catch (Exception ex)
		{
			throw new ArgumentException($"GetBlobContainerClient error: {ex.Message}", ex);
		}
	}

	private static BlobServiceClient GetServiceClient(Connection connection, CancellationToken cancellationToken)
	{
		return connection.AuthenticationMethod switch
		{
			AuthenticationMethod.ConnectionString => GetServiceBlobClientWithConnectionString(connection),
			AuthenticationMethod.OAuth2 => GetBlobServiceClientWithOAuth2(connection),
			AuthenticationMethod.ArcManagedIdentity => GetBlobServiceClientWithArcManagedIdentity(connection),
			AuthenticationMethod.ArcManagedIdentityCrossTenant => GetBlobServiceClientWithArcManagedIdentityCrossTenant(connection, cancellationToken),
			AuthenticationMethod.DefaultManagedIdentity => GetBlobServiceClientWithDefaultManagedIdentity(connection),
			_ => throw new NotSupportedException()
		};
	}

	private static BlobServiceClient GetServiceBlobClientWithConnectionString(Connection connection)
	{
		return new BlobServiceClient(connection.ConnectionString);
	}

	private static BlobClient GetBlobClientWithConnectionString(Connection connection)
	{
		return new BlobClient(connection.ConnectionString, connection.ContainerName.ToLower(), connection.BlobName);
	}

	private static BlobClient GetBlobClientWithSasToken(Connection connection)
	{
		var blobContainerClient = new BlobContainerClient(new Uri($"{connection.Uri}/{connection.ContainerName}?"),
			new AzureSasCredential(connection.SasToken));

		return blobContainerClient.GetBlobClient(connection.BlobName);
	}

	private static BlobServiceClient GetBlobServiceClientWithOAuth2(Connection connection)
	{
		var serviceUri = new Uri($"{connection.Uri}");
		var credentials = new ClientSecretCredential(connection.TenantId, connection.ApplicationId,
			connection.ClientSecret, new ClientSecretCredentialOptions());

		return new BlobServiceClient(serviceUri, credentials);
	}

	private static BlobClient GetBlobClientWithOAuth2(Connection connection)
	{
		var credentials = new ClientSecretCredential(connection.TenantId, connection.ApplicationId,
			connection.ClientSecret, new ClientSecretCredentialOptions());
		var uri = new Uri($"{connection.Uri}/{connection.ContainerName.ToLower()}/{connection.BlobName}");
		return new BlobClient(uri, credentials);
	}

	[ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
	private static BlobClient GetBlobClientWithArcManagedIdentity(Connection destination)
	{
		{
			var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
			var uri = new Uri($"{destination.Uri}/{destination.ContainerName.ToLower()}/{destination.BlobName}");

			return new BlobClient(uri, credentials);
		}
	}

	[ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
	private static BlobServiceClient GetBlobServiceClientWithArcManagedIdentity(Connection destination)
	{
		var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
		return new BlobServiceClient(new Uri(destination.Uri), credentials);
	}

	[ExcludeFromCodeCoverage(Justification = "We do not have environment prepared to test this connection")]
	private static BlobClient GetBlobClientWithArcManagedIdentityCrossTenant(Connection destination,
		CancellationToken cancellationToken)
	{
		{
			var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
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
	private static BlobServiceClient GetBlobServiceClientWithArcManagedIdentityCrossTenant(Connection destination,
		CancellationToken cancellationToken)
	{
		var credentials = new ManagedIdentityCredential(new ManagedIdentityCredentialOptions());
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
		return new BlobServiceClient(new Uri(destination.Uri), assertion);
	}

	private static BlobClient GetClientWithDefaultManagedIdentity(Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.Uri))
		{
			throw new Exception("URI is required.");
		}

		return new BlobClient(GetUri(connection.Uri, connection.ContainerName, connection.BlobName), new DefaultAzureCredential());
	}

	private static BlobServiceClient GetBlobServiceClientWithDefaultManagedIdentity(Connection connection)
	{
		if (string.IsNullOrWhiteSpace(connection.Uri))
			throw new Exception("URI is required.");

		return new BlobServiceClient(new Uri(connection.Uri), new DefaultAzureCredential());
	}
}