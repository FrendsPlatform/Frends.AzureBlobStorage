namespace Frends.AzureBlobStorage.DeleteContainer.Definitions;

/// <summary>
/// Connection methods.
/// </summary>
public enum ConnectionMethod
{
#pragma warning disable CS1591 // self explanatory
    ConnectionString,
    OAuth2,
    ArcManagedIdentity,
    ArcManagedIdentityCrossTenant,
#pragma warning restore CS1591 // self explanatory
}
