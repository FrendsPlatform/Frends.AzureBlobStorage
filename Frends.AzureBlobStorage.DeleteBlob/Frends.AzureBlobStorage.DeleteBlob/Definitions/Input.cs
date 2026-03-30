using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Name of the blob to delete.
    /// </summary>
    /// <example>TestFile.xml</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BlobName { get; set; }

}
