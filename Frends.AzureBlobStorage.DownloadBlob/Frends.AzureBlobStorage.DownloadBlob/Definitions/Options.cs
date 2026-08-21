using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AzureBlobStorage.DownloadBlob.Attributes;

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
    public FileEncoding Encoding { get; set; } = FileEncoding.UTF8WithBOM;

    /// <summary>
    /// Encoding name used when Encoding is set to <c>Other</c>.
    /// A partial list of possible values: https://en.wikipedia.org/wiki/Windows_code_page#List.
    /// </summary>
    /// <example>windows-1251</example>
    [UIHint(nameof(Encoding), "", FileEncoding.Other)]
    [DisplayFormat(DataFormatString = "Text")]
    public string OtherEncoding { get; set; } = string.Empty;

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

    /// <summary>
    /// When <c>true</c>, the source blob is copied inside the same Azure Blob Storage container after the local download completes.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool CopyBlob { get; set; }

    /// <summary>
    /// Directory path inside the same Azure Blob Storage container where the source blob is copied.
    /// The original blob file name is preserved, unless there is already a file with the same name in the target directory (suffix "(x)" to blob name is added if needed).
    /// </summary>
    /// <example>archive\processed</example>
    [UIHint(nameof(CopyBlob), "", true)]
    [DisplayFormat(DataFormatString = "Text")]
    [RequiredIf(nameof(CopyBlob), true, ErrorMessage = $"{nameof(BlobCopyDir)} is required when {nameof(CopyBlob)} is true.")]
    public string BlobCopyDir { get; set; } = string.Empty;

    /// <summary>
    /// When <c>true</c>, the original blob is deleted from Azure Blob Storage after the local download and optional copy complete.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool DeleteOriginal { get; set; }
}
