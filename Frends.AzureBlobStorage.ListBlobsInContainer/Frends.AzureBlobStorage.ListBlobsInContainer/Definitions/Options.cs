using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// List blobs in a flat listing structure or hierarchically.
    /// Hierarchical listing returns container's blobs and subdirectories names such as file.txt or directoryname/. 
    /// Flat listing does the same as hierarchical listing but also returns blobs in those subdirectories such as directoryname/file.txt.
    /// </summary>
    /// <example>ListingStructure.Flat</example>
    [DefaultValue(ListingStructure.Flat)]
    public ListingStructure ListingStructure { get; set; }

    /// <summary>
    /// Specify a prefix to return blobs whose names begin with that character or string.
    /// </summary>
    /// <example>test</example>
    public string Prefix { get; set; }

    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Custom error message</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
