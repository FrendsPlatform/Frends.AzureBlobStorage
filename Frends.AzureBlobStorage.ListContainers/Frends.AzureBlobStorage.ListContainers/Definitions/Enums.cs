namespace Frends.AzureBlobStorage.ListContainers.Definitions;

/// <summary>
/// Connection methods.
/// </summary>
public enum ConnectionMethod
{
    /// <summary>
    /// Authenticate with connection string.
    /// </summary>
    ConnectionString,

    /// <summary>
    /// Authenticate with SAS Token. Requires Storage URI.
    /// </summary>
    SasToken,

    /// <summary>
    /// OAuth2.
    /// </summary>
    OAuth2,
}
