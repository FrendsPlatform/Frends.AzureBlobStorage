using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

/// <summary>
/// Source parameters.
/// </summary>
public class Source
{
    /// <summary>
    /// Authentication method to use when connecting to Azure Blob Storage.
    /// </summary>
    /// <example>AuthenticationMethod.ConnectionString</example>
    [DefaultValue(AuthenticationMethod.ConnectionString)]
    public AuthenticationMethod AuthenticationMethod { get; set; }

    /// <summary>
    /// Scopes used when authenticating with Arc Managed Identity Cross Tenant.
    /// </summary>
    /// <example>[api://AzureADTokenExchange/.default]</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.ArcManagedIdentityCrossTenant)]
    public string[] Scopes { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The name of the blob container in the storage account.
    /// </summary>
    /// <example>ExampleContainer</example>
    public string ContainerName { get; set; }

    /// <summary>
    /// A connection string includes the authentication information required to access data in an Azure Storage account.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.ConnectionString)]
    [PasswordPropertyText]
    public string ConnectionString { get; set; }

    /// <summary>
    /// A shared access signature to use when connecting to Azure storage container.
    /// Grants restricted access rights to Azure Storage resources when combined with URI.
    /// </summary>
    /// <example>sv=2021-04-10&amp;se=2022-04-10T10%3A431Z&amp;sr=c&amp;sp=l&amp;sig=ZJg983RovE%2BZXI</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.SASToken)]
    [PasswordPropertyText]
    public string SASToken { get; set; }

    /// <summary>
    /// The base URI for the Azure Storage container.
    /// Required for SAS and OAuth 2 authentication methods.
    /// </summary>
    /// <example>https://{account_name}.blob.core.windows.net</example>
    public string URI { get; set; }

    /// <summary>
    /// Application (Client) ID of Azure AD Application.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.OAuth2)]
    public string ApplicationID { get; set; }

    /// <summary>
    /// Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.OAuth2)]
    public string TenantID { get; set; }

    /// <summary>
    /// Client Secret of Azure AD Application.
    /// </summary>
    /// <example>Password!</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.OAuth2)]
    [PasswordPropertyText]
    public string ClientSecret { get; set; }

    /// <summary>
    /// Target Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.ArcManagedIdentityCrossTenant)]
    public string TargetTenantId { get; set; }

    /// <summary>
    /// Target Client ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.ArcManagedIdentityCrossTenant)]
    public string TargetClientId { get; set; }
}
