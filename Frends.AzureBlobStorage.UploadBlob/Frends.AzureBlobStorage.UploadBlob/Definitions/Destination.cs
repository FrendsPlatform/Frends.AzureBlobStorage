using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

/// <summary>
/// Destination and optional parameters.
/// </summary>
public class Destination
{
    /// <summary>
    /// Which connection method should be used for connecting to Azure Blob Storage?
    /// </summary>
    /// <example>ConnectionMethod.ConnectionString</example>
    [DefaultValue(ConnectionMethod.ConnectionString)]
    public ConnectionMethod ConnectionMethod { get; set; }

    /// <summary>
    /// Name of the Azure blob storage container which will be deleted.
    /// </summary>
    /// <example>test-container</example>
    public string ContainerName { get; set; }

    /// <summary>
    /// Connection string to Azure storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ConnectionString)]
    [PasswordPropertyText]
    public string ConnectionString { get; set; }

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
    /// Name of the storage account.
    /// </summary>
    /// <example>Storager</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.OAuth2)]
    public string StorageAccountName { get; set; }

    /// <summary>
    /// Determines if the container should be created if it does not exist. See https://docs.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata for naming rules.
    /// </summary>
    /// <example>true</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ConnectionString)]
    [DefaultValue(true)]
    public bool CreateContainerIfItDoesNotExist { get; set; }

    /// <summary>
    /// Azure blob type to upload: Append, Block or Page.
    /// </summary>
    /// <example>Block</example>
    [DefaultValue(AzureBlobType.Block)]
    public AzureBlobType BlobType { get; set; }

    /// <summary>
    /// Specifies the maximum size for the Page blob, up to 8 TB. The size must be aligned to a 512-byte boundary (512, 1024, 1536..). Calculating minimum value from file's size if given value is less than 512.
    /// </summary>
    /// <example>1024</example>
    [UIHint(nameof(BlobType), "", AzureBlobType.Page)]
    public long PageMaxSize { get; set; }

    /// <summary>
    /// Specifies the starting offset for the content to be written as a Page. Value range from 0 to 'page max-file size'. If set to -1, 'page max-file size' will be calculated from 'PageMaxSize'-file's size.  
    /// </summary>
    /// <example>0</example>
    [UIHint(nameof(BlobType), "", AzureBlobType.Page)]
    public long PageOffset { get; set; }

    /// <summary>
    /// Source file can be renamed to this name in Azure Blob Storage.
    /// </summary>
    /// <example>Renamed.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string RenameTo { get; set; }

    /// <summary>
    /// Set desired content-type. If empty, task tries to guess from mime-type.
    /// </summary>
    /// <example>text/xml</example>
    public string ContentType { get; set; }

    /// <summary>
    /// Set desired content-encoding. Defaults to UTF8 BOM.
    /// </summary>
    /// <example>utf8</example>
    public string FileEncoding { get; set; }

    /// <summary>
    /// How existing blob will be handled.
    /// </summary>
    /// <example>HandleExistingFile.Error</example>
    [DefaultValue(HandleExistingFile.Error)]
    public HandleExistingFile HandleExistingFile { get; set; }

    /// <summary>
    /// Blob's name. 'Source File' will be appended into this blob.
    /// </summary>
    /// <example>TestFile.txt</example>
    [UIHint(nameof(HandleExistingFile), "", HandleExistingFile.Append)]
    public string BlobName { get; set; }

    /// <summary>
    /// How many work items to process concurrently.
    /// </summary>
    /// <example>64</example>
    [DefaultValue(64)]
    public int ParallelOperations { get; set; }
}