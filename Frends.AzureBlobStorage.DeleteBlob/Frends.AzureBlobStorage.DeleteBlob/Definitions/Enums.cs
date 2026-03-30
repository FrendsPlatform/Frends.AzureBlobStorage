namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Options for Blob spanshots.
/// </summary>
public enum SnapshotDeleteOption
{
    /// <summary>
    /// No specific options.
    /// </summary>
    None,

    /// <summary>
    /// Also delete snapshots of the blob.
    /// </summary>
    IncludeSnapshots,

    /// <summary>
    /// Delete only blob's snapshots.
    /// </summary>
    DeleteSnapshotsOnly
}


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