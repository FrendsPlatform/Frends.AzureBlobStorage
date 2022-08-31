using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.AzureBlobStorage.DeleteBlob;

/// <summary>
/// Input-class for DeleteBlob-task.
/// </summary>
public class Input
{
    /// <summary>
    /// Name of the blob to delete.
    /// </summary>
    /// <example>TestFile.xml</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BlobName { get; set; }

    /// <summary>
    /// Connection string to Azure storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net</example>
    [PasswordPropertyText]
    [DisplayFormat(DataFormatString = "Text")]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Name of the container where delete blob exists.
    /// </summary>
    /// <example>ExampleContaner</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ContainerName { get; set; }
}