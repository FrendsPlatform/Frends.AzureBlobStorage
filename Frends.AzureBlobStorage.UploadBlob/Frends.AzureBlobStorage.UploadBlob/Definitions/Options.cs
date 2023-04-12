using System.ComponentModel;

namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

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
    public bool ThrowErrorOnFailure { get; set; }
}