using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Input-class for DownloadBlob-task.
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
    /// Connection string to Azure storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ConnectionString)]
    public string ConnectionString { get; set; }

    /// <summary>
    /// OAuth2 connection information.
    /// </summary>
    /// <example>{ OAuthConnection.ApplicationID, OAuthConnection.TenantID, OAuthConnection.ClientSecret, OAuthConnection.StorageAccountName }</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.OAuth2)]
    public OAuthConnection[] Connection { get; set; }

    /// <summary>
    /// Name of the Azure Blob Storage container where the file is downloaded from.
    /// </summary>
    /// <example>ExampleContainer</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ContainerName { get; set; }

    /// <summary>
    /// Name of the blob to download.
    /// </summary>
    /// <example>sample.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BlobName { get; set; }

    /// <summary>
    /// Set encoding manually.
    /// Empty value tries to get encoding set in Azure.
    /// Supported values are utf-8, utf-7, utf-32, unicode, bigendianunicode and ascii.
    /// </summary>
    /// <example>UTF-8</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Encoding { get; set; }
}

/// <summary>
/// OAuthConnection values.
/// </summary>
public class OAuthConnection
{
    /// <summary>
    /// Application (Client) ID of Azure AD Application.
    /// </summary>
    /// <example>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ApplicationID { get; set; }

    /// <summary>
    /// Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DisplayName("Tenant ID")]
    public string TenantID { get; set; }

    /// <summary>
    /// Client Secret of Azure AD Application.
    /// </summary>
    /// <example>xxxxx~xxxxx~xxxxxxxxxxxxxxxxxxxxxxxxxxxx</example>
    [PasswordPropertyText]
    [DisplayName("Client Secret")]
    public string ClientSecret { get; set; }

    /// <summary>
    /// Name of the storage account.
    /// </summary>
    /// <example>ExampleStorage</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string StorageAccountName { get; set; }
}