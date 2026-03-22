namespace Frends.AzureBlobStorage.Toolkit.Definitions;

/// <summary>
/// Connection methods.
/// </summary>
public enum ConnectionMethod
{
#pragma warning disable SA1602 // self explanatory
    ConnectionString = 1,
    OAuth2 = 2,
    SasToken = 3,
    ArcManagedIdentity = 4,
    ArcManagedIdentityCrossTenant = 5,
#pragma warning restore SA1602 // self explanatory
}
