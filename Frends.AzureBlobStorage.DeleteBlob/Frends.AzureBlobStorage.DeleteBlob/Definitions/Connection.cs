using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.DeleteBlob.Attributes;

namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Connection parameters
/// </summary>
public sealed class Connection
{
    /// <summary>
    /// Defines which connection method should be used for connecting to Azure Blob Storage.
    /// </summary>
    /// <example>ConnectionMethod.ConnectionString</example>
    [DefaultValue(ConnectionMethod.ConnectionString)]
    public ConnectionMethod AuthenticationMethod { get; set; } = ConnectionMethod.ConnectionString;

    /// <summary>
    /// Connection string to Azure storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=account-name;AccountKey=abc==;EndpointSuffix=core.windows.net</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.ConnectionString)]
    [RequiredIf(nameof(AuthenticationMethod), ConnectionMethod.ConnectionString)]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Application (Client) ID of Azure AD Application.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-abc33h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.OAuth2)]
    [RequiredIf(nameof(AuthenticationMethod), ConnectionMethod.OAuth2)]

    public string ApplicationId { get; set; }

    /// <summary>
    /// Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-abc33h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.OAuth2)]
    [RequiredIf(nameof(AuthenticationMethod), ConnectionMethod.OAuth2)]

    public string TenantId { get; set; }

    /// <summary>
    /// Client Secret of Azure AD Application.
    /// </summary>
    /// <example>Password!</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.OAuth2)]
    [RequiredIf(nameof(AuthenticationMethod), ConnectionMethod.OAuth2)]
    [PasswordPropertyText]
    public string ClientSecret { get; set; }

    /// <summary>
    /// A shared access signature to use when connecting to an Azure storage container.
    /// Grants restricted access rights to Azure Storage resources when combined with URI.
    /// </summary>
    /// <example>sv=2021-04-10&amp;se=2022-04-10T10%3A431Z&amp;sr=c&amp;sp=l&amp;sig=ZJg983RovE%23ZXI</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.SasToken)]
    [RequiredIf(nameof(AuthenticationMethod), ConnectionMethod.SasToken)]
    [PasswordPropertyText]
    public string SasToken { get; set; }

    /// <summary>
    /// Name of the Azure storage account.
    /// </summary>
    /// <example>TestStorage</example>
    [UIHint(
        nameof(AuthenticationMethod),
        "",
        ConnectionMethod.OAuth2,
        ConnectionMethod.SasToken,
        ConnectionMethod.ArcManagedIdentity,
        ConnectionMethod.ArcManagedIdentityCrossTenant)]
    [RequiredIf(
        nameof(AuthenticationMethod),
        ConnectionMethod.OAuth2,
        ConnectionMethod.SasToken,
        ConnectionMethod.ArcManagedIdentity,
        ConnectionMethod.ArcManagedIdentityCrossTenant),]
    public string StorageAccountName { get; set; }

    /// <summary>
    /// Scopes used when authenticating with Arc Managed Identity Cross Tenant.
    /// </summary>
    /// <example>[api://AzureADTokenExchange/.default]</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.ArcManagedIdentityCrossTenant)]
	[RequiredIf(nameof(AuthenticationMethod), ConnectionMethod.ArcManagedIdentityCrossTenant)]
    public string[] Scopes { get; set; } = [];

    /// <summary>
    /// Target Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-abc33h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.ArcManagedIdentityCrossTenant)]
    [RequiredIf(nameof(AuthenticationMethod), ConnectionMethod.ArcManagedIdentityCrossTenant)]

    public string TargetTenantId { get; set; }

    /// <summary>
    /// Target Client ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-abc33h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.ArcManagedIdentityCrossTenant)]
    [RequiredIf(nameof(AuthenticationMethod), ConnectionMethod.ArcManagedIdentityCrossTenant)]
    public string TargetClientId { get; set; }
}
