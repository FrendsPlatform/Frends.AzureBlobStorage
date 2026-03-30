using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.ReadBlob.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
	/// <summary>
	/// Name of the blob to read.
	/// </summary>
	/// <example>TestFile.xml</example>
	[DisplayFormat(DataFormatString = "Text")]
	public string BlobName { get; set; }
}
