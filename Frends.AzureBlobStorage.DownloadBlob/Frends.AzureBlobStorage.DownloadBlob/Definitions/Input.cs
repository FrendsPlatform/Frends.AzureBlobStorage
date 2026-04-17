using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.DownloadBlob.Attributes;

namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Input parameters for the DownloadBlob task, specifying what to download and where to save it.
/// </summary>
public class Input
{
    /// <summary>
    /// Name of the Azure Blob Storage container from which the blob is downloaded.
    /// </summary>
    /// <example>my-container</example>
    [DisplayFormat(DataFormatString = "Text")]
    [NotEmptyString]
    public string ContainerName { get; set; }

    /// <summary>
    /// Name of the blob to download, including any virtual directory path.
    /// </summary>
    /// <example>sample.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [NotEmptyString]
    public string BlobName { get; set; }

    /// <summary>
    /// Local directory where the downloaded file will be saved.
    /// </summary>
    /// <example>c:\temp</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string TargetDirectory { get; set; }

    /// <summary>
    /// Optional custom filename for the downloaded blob. If left empty the original blob name is used.
    /// </summary>
    /// <example>custom-name.txt</example>
    [DefaultValue("")]
    public string TargetFileName { get; set; } = string.Empty;

    /// <summary>
    /// Specifies what to do when a file already exists at the destination path.
    /// Throw: raises an exception; Rename: appends an incrementing number; Overwrite: replaces the file.
    /// </summary>
    /// <example>FileExistsAction.Throw</example>
    [DefaultValue(FileExistsAction.Throw)]
    public FileExistsAction ActionOnExistingFile { get; set; }
}
