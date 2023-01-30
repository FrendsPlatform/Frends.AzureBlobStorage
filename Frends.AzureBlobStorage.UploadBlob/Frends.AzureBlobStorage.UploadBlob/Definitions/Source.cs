using Azure;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

/// <summary>
/// Source parameters.
/// </summary>
public class Source
{
    /// <summary>
    /// File which will be uploaded. This file will be appended into target blob in Append process.
    /// </summary>
    /// <example>c:\temp\testfile.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue(@"c:\temp\testfile.txt")]
    public string SourceFile { get; set; }

    /// <summary>
    /// Uses stream to read file content.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ContentsOnly { get; set; }

    /// <summary>
    /// Gzip compression. Works only when transferring stream content.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    [DisplayName("Gzip compression")]
    public bool Compress { get; set; }

    /// <summary>
    /// Tags for the uploaded blob (AzureBlobType: Block and Append).
    /// </summary>
    /// <example>{name, value}</example>
    public Tag[] Tags { get; set; }
}

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