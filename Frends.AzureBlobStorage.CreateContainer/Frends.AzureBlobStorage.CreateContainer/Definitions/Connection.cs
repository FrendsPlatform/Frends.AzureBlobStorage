using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.CreateContainer.Definitions;

/// <summary>
/// Connection parameters
/// </summary>
public class Connection
{
    /// <summary>
    /// Which connection method should be used for connecting to Azure Blob Storage.
    /// </summary>
    /// <example>ConnectionMethod.ConnectionString</example>
    [DefaultValue(ConnectionMethod.ConnectionString)]
    public ConnectionMethod AuthenticationMethod { get; set; }

    /// <summary>
    /// Connection string to Azure storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.ConnectionString)]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Name of the Azure storage account.
    /// </summary>
    /// <example>Storager</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.OAuth2)]
    public string StorageAccountName { get; set; }

    /// <summary>
    /// Application (Client) ID of Azure AD Application.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.OAuth2)]
    public string ApplicationId { get; set; }

    /// <summary>
    /// Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.OAuth2)]
    public string TenantId { get; set; }

    /// <summary>
    /// Client Secret of Azure AD Application.
    /// </summary>
    /// <example>Password!</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.OAuth2)]
    [PasswordPropertyText]
    public string ClientSecret { get; set; }

    /// <summary>
    /// Scopes used when authenticating with Arc Managed Identity Cross Tenant.
    /// </summary>
    /// <example>[api://AzureADTokenExchange/.default]</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.ArcManagedIdentityCrossTenant)]
    public string[] Scopes { get; set; } = [];

    /// <summary>
    /// Target Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.ArcManagedIdentityCrossTenant)]
    public string TargetTenantId { get; set; }

    /// <summary>
    /// Target Client ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", ConnectionMethod.ArcManagedIdentityCrossTenant)]
    public string TargetClientId { get; set; }
}
