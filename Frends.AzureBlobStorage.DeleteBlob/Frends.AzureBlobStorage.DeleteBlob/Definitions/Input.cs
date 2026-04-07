using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.DeleteBlob.Attributes;


namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Name of the Azure Blob Storage container where the data will be uploaded.
    /// Naming: lowercase
    /// Valid chars: alphanumeric and dash, but cannot start or end with dash.
    /// </summary>
    /// <example>test-container</example>
    [DefaultValue("test-container")]
    [DisplayFormat(DataFormatString = "Text")]
    [NotEmptyString]
    public string ContainerName { get; set; }

    /// <summary>
    /// Name of the blob to delete.
    /// </summary>
    /// <example>TestFile.xml</example>
    [DisplayFormat(DataFormatString = "Text")]
    [NotEmptyString]
    public string BlobName { get; set; }
}
