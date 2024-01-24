using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

/// <summary>
/// Tag parameters.
/// </summary>
public class Tag
{
    /// <summary>
    /// Name of the tag.
    /// </summary>
    /// <example>Name</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Name { get; set; }

    /// <summary>
    /// Value of the tag.
    /// </summary>
    /// <example>Value</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Value { get; set; }
}