using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.WriteBlob.Attributes;
using Frends.AzureBlobStorage.WriteBlob.Enums;

namespace Frends.AzureBlobStorage.WriteBlob.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Selection of source types.
    /// </summary>
    /// <example>SourceType.String</example>
    [DefaultValue(SourceType.Bytes)]
    public SourceType SourceType { get; set; }

    /// <summary>
    /// Source content in string format.
    /// </summary>
    /// <example>This is test content</example>
    [UIHint(nameof(SourceType), "", SourceType.String)]
    public string ContentString { get; set; }

    /// <summary>
    /// Source content in byte array.
    /// </summary>
    /// <example>VGhpcyBpcyB0ZXN0</example>
    [DisplayFormat(DataFormatString = "Expression")]
    [UIHint(nameof(SourceType), "", SourceType.Bytes)]
    public byte[] ContentBytes { get; set; }

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
    /// <example>true</example>
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
    /// Name of the Azure Blob Storage container.
    /// Task will convert all letters to lowercase.
    /// See more info: https://learn.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata#container-names
    /// </summary>
    /// <example>examplecontainer</example>
    [DisplayFormat(DataFormatString = "Text")]
    [NotEmptyString]
    public string ContainerName { get; set; }

    /// <summary>
    /// Name of the blob. Blob name can also be folder structure and folders will be created to Blob Storage.
    /// See more info: https://learn.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata#blob-names
    /// </summary>
    /// <example>myblob.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [NotEmptyString]
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
    [DefaultValue(false)]
    public bool CreateContainerIfItDoesNotExist { get; set; }

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
    /// Should the content be compressed before sending?
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool Compress { get; set; }
}
