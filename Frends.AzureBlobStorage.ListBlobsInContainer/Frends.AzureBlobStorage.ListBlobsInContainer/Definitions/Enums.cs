namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

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
/// Listing options.
/// </summary>
public enum ListingStructure
{
    /// <summary>
    /// Flat listing structure.
    /// </summary>
    Flat,

    /// <summary>
    /// Hierarchical listing structure.
    /// </summary>
    Hierarchical
}

/// <summary>
/// Query type
/// </summary>
public enum QueryType
{
	/// <summary>
	/// Retrieve blobs using the default GetBlob method.
	/// </summary>
	Default,
	/// <summary>
	/// Retrieve blobs using tags by query string.
	/// </summary>
	Tags
}