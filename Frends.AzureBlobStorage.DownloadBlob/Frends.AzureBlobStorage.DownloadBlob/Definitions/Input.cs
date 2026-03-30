using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
	/// <summary>
	/// Name of the blob to download.
	/// </summary>
	/// <example>TestFile.xml</example>
	[DisplayFormat(DataFormatString = "Text")]
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
