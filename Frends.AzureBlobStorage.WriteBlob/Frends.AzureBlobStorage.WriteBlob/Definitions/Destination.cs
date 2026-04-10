using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.WriteBlob.Attributes;
using Frends.AzureBlobStorage.WriteBlob.Enums;

namespace Frends.AzureBlobStorage.WriteBlob.Definitions;

/// <summary>
/// Destination parameters.
/// </summary>
public class Destination
{
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
    /// <example>BlobName.txt; C:\folder\blobName.txt</example>
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
    /// Should the string be compressed before sending?
    /// </summary>
    [DefaultValue(false)]
    public bool Compress { get; set; }
}
