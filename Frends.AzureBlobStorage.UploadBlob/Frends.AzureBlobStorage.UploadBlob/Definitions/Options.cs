using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

/// <summary>
/// Optional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// True: Throw an exception.
    /// False: If the error is ignorable, such as when a Blob already exists, the error will be added to the Result.ErrorMessages list instead of stopping the Task.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; }

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

    /// <summary>
    /// How many work items to process concurrently.
    /// </summary>
    /// <example>64</example>
    [DefaultValue(64)]
    public int ParallelOperations { get; set; }
}