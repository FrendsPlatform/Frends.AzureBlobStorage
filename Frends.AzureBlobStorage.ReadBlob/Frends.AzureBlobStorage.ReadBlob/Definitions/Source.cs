using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.ReadBlob.Attributes;

namespace Frends.AzureBlobStorage.ReadBlob.Definitions;

/// <summary>
/// Source parameters.
/// </summary>
public class Source
{
    /// <summary>
    /// Azure storage container's name.
    /// </summary>
    /// <example>Container1</example>
    [NotEmptyString]
    public string ContainerName { get; set; }

    /// <summary>
    /// Name of the blob which content is read.
    /// </summary>
    /// <example>File.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [NotEmptyString]
    public string BlobName { get; set; }
}
