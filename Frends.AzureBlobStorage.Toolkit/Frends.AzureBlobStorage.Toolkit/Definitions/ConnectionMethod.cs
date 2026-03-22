namespace Frends.AzureBlobStorage.Toolkit.Definitions;

/// <summary>
/// Connection methods.
/// </summary>
public enum ConnectionMethod
{
#pragma warning disable SA1602 // self explanatory
    ConnectionString,
    OAuth2,
    SasToken,
    ArcManagedIdentity,
    ArcManagedIdentityCrossTenant,
#pragma warning restore SA1602 // self explanatory
}
