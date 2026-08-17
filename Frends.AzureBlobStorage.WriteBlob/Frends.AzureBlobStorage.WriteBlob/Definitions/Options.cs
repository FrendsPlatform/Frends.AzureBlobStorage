using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.WriteBlob.Definitions;

/// <summary>
/// Optional parameters.
/// </summary>
public class Options
{

    /// <summary>
    /// True: Throw an exception.
    /// False: If the error is ignorable, such as when a Blob already exists, the error will be added to the Result.ErrorMessages list instead of stopping the Task.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>WriteBlob failed: unable to upload file to Azure Blob Storage.</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}