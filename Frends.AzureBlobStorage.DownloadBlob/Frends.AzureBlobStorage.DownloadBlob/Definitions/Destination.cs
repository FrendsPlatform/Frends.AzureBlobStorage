using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Options-class for DownloadBlob-task.
/// </summary>
public class Destination
{
    /// <summary>
    /// Destination directory.
    /// </summary>
    /// <example>c:\temp</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Directory { get; set; }

    /// <summary>
    /// Error: Throws exception if destination file exists.
    /// Rename: Adds '(1)' at the end of file name. Incerements the number if (1) already exists.
    /// Overwrite: Overwrites existing file.
    /// </summary>
    /// <example>Error</example>
    [DefaultValue(FileExistsAction.Error)]
    public FileExistsAction FileExistsOperation { get; set; }

    /// <summary>
    /// Optional custom filename for the downloaded blob. If not specified, the original blob name will be used.
    /// </summary>
    /// <example>customname.txt</example>
    [DefaultValue("")]
    public string TargetFileName { get; set; } = = string.Empty;