using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Input-class for DownloadBlob-task.
/// </summary>
public class Source
{
    /// <summary>
    /// Connection string to Azure Blob Storage.
    /// </summary>
    /// <example>DefaultEndpointsProtocol=https;AccountName=account;AccountKey=foobar123Z;EndpointSuffix=core.windows.net</example>
    [PasswordPropertyText]
    [DisplayFormat(DataFormatString = "Text")]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Name of the Azure Blob Storage container where the file is downloaded from.
    /// </summary>
    /// <example>ExampleContainer</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ContainerName { get; set; }

    /// <summary>
    /// Name of the blob to download.
    /// </summary>
    /// <example>sample.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BlobName { get; set; }

    /// <summary>
    /// Set encoding manually.
    /// Empty value tries to get encoding set in Azure.
    /// Supported values are utf-8, utf-7, utf-32, unicode, bigendianunicode and ascii.
    /// </summary>
    /// <example>UTF-8</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Encoding { get; set; }
}