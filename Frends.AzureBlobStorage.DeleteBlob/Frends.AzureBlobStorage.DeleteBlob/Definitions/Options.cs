using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// What should be done with blob snapshots?
    /// </summary>
    /// <example>IncludeSnapshots</example>
    [DefaultValue(SnapshotDeleteOption.IncludeSnapshots)]
    public SnapshotDeleteOption SnapshotDeleteOption { get; set; }

    /// <summary>
    /// Delete blob only if the ETag matches. Leave empty if verification is not needed.
    /// </summary>
    /// <example>0x9FE13BAA323E5A4</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string VerifyETagWhenDeleting { get; set; }

    /// <summary>
    /// If true, the task throws an exception on failure.
    /// If false, the task returns Success = false with error details in the Error property.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Optional custom error message prefix included in the exception or result Error.Message on failure.
    /// If left empty, the original error message is used.
    /// </summary>
    /// <example>DeleteBlob failed for my-container/my-blob.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ErrorMessageOnFailure { get; set; }
}
