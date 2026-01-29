namespace Frends.AzureBlobStorage.ReadBlob.Definitions;

/// <summary>
/// Encoding name in which blob content is read.
/// </summary>
public enum Encode
{
#pragma warning disable CS1591 // self explanatory
    UTF8,
    UTF32,
    Unicode,
    ASCII
#pragma warning restore CS1591 // self explanatory
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
    /// Authenticate with OAuth2.
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
