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
    /// If true, throw an error if blob to be deleted doesn't exist.
    /// If false, Task will return Success = true.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ThrowErrorIfBlobDoesNotExists { get; set; }
}
