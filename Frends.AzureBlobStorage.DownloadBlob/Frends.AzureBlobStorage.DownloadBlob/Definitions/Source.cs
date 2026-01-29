using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Input class for DownloadBlob-task.
/// </summary>
public class Source
{
    /// <summary>
    /// Which connection method should be used for connecting to Azure Blob Storage.
    /// </summary>
    /// <example>ConnectionMethod.ConnectionString</example>
    [DefaultValue(ConnectionMethod.ConnectionString)]
    public ConnectionMethod ConnectionMethod { get; set; }

    /// <summary>
    /// Name of the Azure Blob Storage container where the file is downloaded from.
    /// </summary>
    /// <example>ExampleContainer</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ContainerName { get; set; }

    /// <summary>
    /// Connection string to Azure storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ConnectionString)]
    public string ConnectionString { get; set; }

    /// <summary>
    /// The base URI for the Azure Storage container.
    /// Required for SAS and OAuth 2 authentication methods.
    /// </summary>
    /// <example>https://{account_name}.blob.core.windows.net</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.OAuth2, ConnectionMethod.SASToken)]
    public string Uri { get; set; }

    /// <summary>
    /// Application (Client) ID of Azure AD Application.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.OAuth2)]
    public string ApplicationID { get; set; }

    /// <summary>
    /// Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.OAuth2)]
    public string TenantID { get; set; }

    /// <summary>
    /// Client Secret of Azure AD Application.
    /// </summary>
    /// <example>Password!</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.OAuth2)]
    [PasswordPropertyText]
    public string ClientSecret { get; set; }

    /// <summary>
    /// A shared access signature to use when connecting to Azure storage container.
    /// Grants restricted access rights to Azure Storage resources when combined with URI.
    /// </summary>
    /// <example>sv=2021-04-10&amp;se=2022-04-10T10%3A431Z&amp;sr=c&amp;sp=l&amp;sig=ZJg983RovE%2BZXI</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.SASToken)]
    [PasswordPropertyText]
    public string SASToken { get; set; }

    /// <summary>
    /// Scopes used when authenticating with Arc Managed Identity Cross Tenant.
    /// </summary>
    /// <example>[api://AzureADTokenExchange/.default]</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ArcManagedIdentityCrossTenant)]
    public string[] Scopes { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Target Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ArcManagedIdentityCrossTenant)]
    public string TargetTenantId { get; set; }

    /// <summary>
    /// Target Client ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ArcManagedIdentityCrossTenant)]
    public string TargetClientId { get; set; }

    /// <summary>
    /// Name of the blob to download.
    /// </summary>
    /// <example>sample.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BlobName { get; set; }

    /// <summary>
    /// Set desired content-encoding.
    /// Defaults to UTF8 BOM.
    /// </summary>
    /// <example>utf8</example>
    [DefaultValue(FileEncoding.UTF8)]
    public FileEncoding Encoding { get; set; }

    /// <summary>
    /// Enables BOM for UTF-8.
    /// </summary>
    [UIHint(nameof(Encoding), "", FileEncoding.UTF8)]
    [DefaultValue(true)]
    public bool EnableBOM { get; set; }

    /// <summary>
    /// Content encoding as string. A partial list of possible encodings: https://en.wikipedia.org/wiki/Windows_code_page#List.
    /// </summary>
    /// <example>windows-1252</example>
    [UIHint(nameof(Encoding), "", FileEncoding.Other)]
    public string FileEncodingString { get; set; }
}
