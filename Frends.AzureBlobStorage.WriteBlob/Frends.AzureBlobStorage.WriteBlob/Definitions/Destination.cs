using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.WriteBlob.Enums;

namespace Frends.AzureBlobStorage.WriteBlob.Definitions;

/// <summary>
/// Destination parameters.
/// </summary>
public class Destination
{
    /// <summary>
    /// Connection method to be used to connect to Azure Blob Storage.
    /// </summary>
    /// <example>ConnectionMethod.ConnectionString</example>
    [DefaultValue(ConnectionMethod.ConnectionString)]
    public ConnectionMethod ConnectionMethod { get; set; }

    /// <summary>
    /// Connection string for Azure blob storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ConnectionString)]
    [PasswordPropertyText]
    [DisplayFormat(DataFormatString = "Text")]
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
    /// Name of the Azure Blob Storage container.
    /// Task will convert all letters to lowercase.
    /// See more info: https://learn.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata#container-names
    /// </summary>
    /// <example>examplecontainer</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ContainerName { get; set; }

    /// <summary>
    /// Name of the blob. Blob name can also be folder structure and folders will be created to Blob Storage.
    /// See more info: https://learn.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata#blob-names
    /// </summary>
    /// <example>BlobName.txt; C:\folder\blobName.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BlobName { get; set; }

    /// <summary>
    /// Tags for the block or append blob.
    /// </summary>
    /// <example>{name, value}</example>
    public Tag[] Tags { get; set; }

    /// <summary>
    /// Determines if the container should be created if it does not exist.
    /// See https://docs.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata for naming rules.
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ConnectionString, ConnectionMethod.OAuth2)]
    [DefaultValue(false)]
    public bool CreateContainerIfItDoesNotExist { get; set; }

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
    /// How the existing blob will be handled.
    /// Append: Append the blob with Source.SourceFile. Block and Page blobs will be downloaded as a temp file which will be deleted after local append and upload processes are complete. No downloading needed for Append Blob.
    /// Overwrite: The original blob will be deleted before uploading the new one.
    /// Error: Depending on Options.ThrowErrorOnFailure, throw an exception or Result will contain an error message instead of the blob's URL.
    /// </summary>
    /// <example>HandleExistingFile.Error</example>
    [DefaultValue(HandleExistingFile.Error)]
    public HandleExistingFile HandleExistingFile { get; set; }

    /// <summary>
    /// Should the string be compressed before sending?
    /// </summary>
    [DefaultValue(false)]
    public bool Compress { get; set; }
}
