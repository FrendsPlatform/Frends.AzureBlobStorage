using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.WriteBlob.Enums;

namespace Frends.AzureBlobStorage.WriteBlob.Definitions;

/// <summary>
/// Source parameters.
/// </summary>
public class Source
{
    /// <summary>
    /// Selection of source types. 
    /// </summary>
    /// <example>SourceType.String</example>
    [DefaultValue(SourceType.Bytes)]
    public SourceType SourceType { get; set; }

    /// <summary>
    /// Source content in string format.
    /// </summary>
    /// <example>This is test</example>
    [UIHint(nameof(SourceType), "", SourceType.String)]
    public string ContentString { get; set; }

    /// <summary>
    /// Source content in byte array.
    /// </summary>
    /// <example>VGhpcyBpcyB0ZXN0</example>
    [DisplayFormat(DataFormatString = "Expression")]
    [UIHint(nameof(SourceType), "", SourceType.Bytes)]
    public byte[] ContentBytes { get; set; }

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