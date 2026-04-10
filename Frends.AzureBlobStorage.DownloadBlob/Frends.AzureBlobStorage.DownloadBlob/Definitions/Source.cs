using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.DownloadBlob.Attributes;

namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Input class for DownloadBlob-task.
/// </summary>
public class Source
{
    /// <summary>
    /// Name of the Azure Blob Storage container where the file is downloaded from.
    /// </summary>
    /// <example>ExampleContainer</example>
    [DisplayFormat(DataFormatString = "Text")]
    [NotEmptyString]
    public string ContainerName { get; set; }

    /// <summary>
    /// Name of the blob to download.
    /// </summary>
    /// <example>sample.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [NotEmptyString]
    public string BlobName { get; set; }

    /// <summary>
    /// Set desired content-encoding.
    /// Defaults to UTF8 BOM.
    /// </summary>
    /// <example>utf8</example>
    [DefaultValue(FileEncoding.UTF8)]
    public FileEncoding Encoding { get; set; }

    /// <summary>
    /// Enables BOM for UTF-8.
    /// </summary>
    [UIHint(nameof(Encoding), "", FileEncoding.UTF8)]
    [DefaultValue(true)]
    public bool EnableBOM { get; set; }

    /// <summary>
    /// Content encoding as string. A partial list of possible encodings: https://en.wikipedia.org/wiki/Windows_code_page#List.
    /// </summary>
    /// <example>windows-1252</example>
    [UIHint(nameof(Encoding), "", FileEncoding.Other)]
    public string FileEncodingString { get; set; }
}
