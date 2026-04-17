using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Optional settings controlling encoding behaviour and error handling for the DownloadBlob task.
/// </summary>
public class Options
{
    /// <summary>
    /// Content encoding used when writing the downloaded file to disk.
    /// Defaults to UTF-8 with BOM.
    /// </summary>
    /// <example>FileEncoding.UTF8WithBOM</example>
    [DefaultValue(FileEncoding.UTF8WithBOM)]
    public FileEncoding Encoding { get; set; }

    /// <summary>
    /// Encoding name used when <see cref="Encoding"/> is set to <c>Other</c>.
    /// A partial list of possible values: https://en.wikipedia.org/wiki/Windows_code_page#List.
    /// </summary>
    /// <example>windows-1251</example>
    [UIHint(nameof(Encoding), "", FileEncoding.Other)]
    [DisplayFormat(DataFormatString = "Text")]
    public string OtherEncoding { get; set; }

    /// <summary>
    /// When <c>true</c> (default) any error causes an exception to be thrown.
    /// Set to <c>false</c> to return a result with <c>Success = false</c> instead.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Optional custom error message prepended to the exception or error result message.
    /// Ignored when left empty.
    /// </summary>
    /// <example>Download failed</example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
