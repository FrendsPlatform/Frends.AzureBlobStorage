namespace Frends.AzureBlobStorage.ListContainers.Definitions;

/// <summary>
/// Authentication options.
/// </summary>
public enum AuthenticationMethod
{
	/// <summary>
	/// Authenticate with connectiong string.
	/// </summary>
	ConnectionString,

	/// <summary>
	/// Authenticate with SAS Token. Requires Storage URI.
	/// </summary>
	SASToken,

	/// <summary>
	/// OAuth2.
	/// </summary>
	OAuth2,

	/// <summary>
	/// Managed Identity.
	/// </summary>
	ArcManagedIdentity,

	/// <summary>
	/// Managed Identity for cross-tenant.
	/// </summary>
	ArcManagedIdentityCrossTenant,

	/// <summary>
	/// Default Managed Identity.
	/// </summary>
	DefaultManagedIdentity
}

/// <summary>
/// Flags used to filter which types of containers are returned when listing.
/// </summary>
public enum ContainerStateFilter
{
    /// <summary>No filtering; only normal containers are returned.</summary>
	None = 0,

    /// <summary>Include system containers (e.g., $logs, $web).</summary>
    System = 1,

    /// <summary>Include soft-deleted containers.</summary>
    Deleted = 2,
}
