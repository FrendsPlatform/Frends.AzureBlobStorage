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
