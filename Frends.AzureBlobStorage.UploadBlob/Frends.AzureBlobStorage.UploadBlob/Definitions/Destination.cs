using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.UploadBlob;

/// <summary>
/// Options-class for UploadBlob-task.
/// </summary>
public class Destination
{
    /// <summary>
    /// Connection string to Azure Blob Storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=account;AccountKey=acCouNtKeY;EndpointSuffix=core.windows.net</example>
    [PasswordPropertyText]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Name of the Azure Blob Storage container where the data will be uploaded.
    /// </summary>
    /// <example>UploadContainer</example>
    public string ContainerName { get; set; }

    /// <summary>
    /// Determines if the container should be created if it does not exist.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool CreateContainerIfItDoesNotExist { get; set; }

    /// <summary>
    /// Azure blob type to upload: Append, Block or Page.
    /// </summary>
    /// <example>Block</example>
    [DefaultValue(AzureBlobType.Block)]
    public AzureBlobType BlobType { get; set; }

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
    /// Should upload operation overwrite existing file with same name?
    /// </summary>
    [DefaultValue(false)]
    public bool Overwrite { get; set; }

    /// <summary>
    /// How many work items to process concurrently.
    /// </summary>
    /// <example>64</example>
    [DefaultValue(64)]
    public int ParallelOperations { get; set; }

    /// <summary>
    /// Append blob with 'Source File'. Block and Page blob will be downloaded into 'Download Folder' and uploaded back to same container after local append process is completed. No downloading needed for Append Blob. Overwrite must be true when targeting Block or Page blob.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool Append { get; set; }

    /// <summary>
    /// Blob's name. 'Source File' will be appended into this blob.
    /// </summary>
    /// <example>TestFile.txt</example>
    [UIHint(nameof(Append), "", true)]
    public string BlobName { get; set; }

    /// <summary>
    /// Directory where blob will be downloaded for appending process. Only Block and Page blobs will be downloaded.
    /// </summary>
    /// <example>c:/temp/downloads</example>
    [UIHint(nameof(Append), "", true)]
    public string DownloadFolder { get; set; }

    /// <summary>
    /// Specifies the maximum size for the page blob, up to 8 TB. The size must be aligned to a 512-byte boundary (512, 1024, 1536..). Calculating minimum value from file's size if given value is less than 512.
    /// </summary>
    /// <example>1024</example>
    [UIHint(nameof(BlobType), "", AzureBlobType.Page)]
    public long PageMaxSize { get; set; }

    /// <summary>
    /// Specifies the starting offset for the content to be written as a page. Value range from 0 to 'page max-file size'. If set -1, 'page max-file size' will be calculated from file's size.  
    /// </summary>
    /// <example>c:/temp/downloads</example>
    [UIHint(nameof(BlobType), "", AzureBlobType.Page)]
    public long PageOffset { get; set; }

    /// <summary>
    /// Delete temp file after append and reupload.  
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(BlobType), "", AzureBlobType.Page)]
    [DefaultValue(false)]
    public bool DeleteTempFile { get; set; }
}

/// <summary>
/// Blob type of uploaded blob.
/// </summary>
public enum AzureBlobType
{
    /// <summary>
    /// Made up of blocks like block blobs, but are optimized for append operations. Append blobs are ideal for scenarios such as logging data from virtual machines.
    /// </summary>
    Append,

    /// <summary>
    /// Store text and binary data. Block blobs are made up of blocks of data that can be managed individually. Block blobs can store up to about 190.7 TiB.
    /// </summary>
    Block,

    /// <summary>
    /// Store random access files up to 8 TiB in size. Page blobs store virtual hard drive (VHD) files and serve as disks for Azure virtual machines.
    /// </summary>
    Page
}
