using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

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
    /// Name of the Azure Blob Storage container.
    /// Task will convert all letters to lowercase.
    /// </summary>
    /// <example>examplecontainer</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ContainerName { get; set; }

    /// <summary>
    /// Connection string for Azure blob storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ConnectionString)]
    [PasswordPropertyText]
    [DisplayFormat(DataFormatString = "Text")]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Name of the Azure storage account.
    /// </summary>
    /// <example>Storager</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.OAuth2)]
    public string StorageAccountName { get; set; }

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
    /// Specifies the target folder path for the blob in Azure Blob Storage.
    /// This value will be prepended to blob names during upload, effectively creating a virtual directory structure.
    /// </summary>
    /// <example>backups/2024/</example>
    public string TargetFolder { get; set; }

    /// <summary>
    /// Determines if the container should be created if it does not exist. 
    /// See https://docs.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata for naming rules.
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(ConnectionMethod), "", ConnectionMethod.ConnectionString)]
    [DefaultValue(false)]
    public bool CreateContainerIfItDoesNotExist { get; set; }

    /// <summary>
    /// Azure blob type to upload.
    /// Append: Made up of blocks like block blobs, but are optimized for append operations. Append blobs are ideal for scenarios such as logging data from virtual machines.
    /// Block: Store text and binary data. Block blobs are made up of blocks of data that can be managed individually. Block blobs can store up to about 190.7 TiB.
    /// Page: Store random access files up to 8 TiB in size. Page blobs store virtual hard drive (VHD) files and serve as disks for Azure virtual machines.
    /// </summary>
    /// <example>Block</example>
    [DefaultValue(AzureBlobType.Block)]
    public AzureBlobType BlobType { get; set; }

    /// <summary>
    /// Page blob size must be a multiple of 512. 
    /// If set to true, this Task will attempt to fill the file with empty bytes until it meets the requirements. When appending to an existing blob, the Task will download the original blob, append the new file to it, and then fill the required bytes.
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(BlobType), "", AzureBlobType.Page)]
    [DefaultValue(false)]
    public bool ResizeFile { get; set; }

    /// <summary>
    /// Specifies the maximum size for a new Page blob, up to 8 TB.
    /// The size must be a multiple of 512 (512, 1024, 1536...). 
    /// If the given value is less than what is required for the upload, the Task will calculate the minimum value.
    /// </summary>
    /// <example>1024</example>
    [UIHint(nameof(BlobType), "", AzureBlobType.Page)]
    public long PageMaxSize { get; set; }

    /// <summary>
    /// Specifies the starting offset for the content to be written as a Page. 
    /// If set to -1 and Destination.HandleExistingFile = Append, offset will be calculated from original blob's size (before append process).
    /// </summary>
    /// <example>0</example>
    [UIHint(nameof(BlobType), "", AzureBlobType.Page)]
    public long PageOffset { get; set; }

    /// <summary>
    /// Set desired content-type. 
    /// If empty, the Task tries to guess from mime-type.
    /// </summary>
    /// <example>text/xml</example>
    public string ContentType { get; set; }

    /// <summary>
    /// Set desired content-encoding. 
    /// Defaults to UTF8 BOM.
    /// </summary>
    /// <example>utf8</example>
    public string FileEncoding { get; set; }

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
    /// How many work items to process concurrently.
    /// </summary>
    /// <example>64</example>
    [DefaultValue(64)]
    public int ParallelOperations { get; set; }
}