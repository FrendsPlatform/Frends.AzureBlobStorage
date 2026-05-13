using System.ComponentModel;
namespace Frends.AzureBlobStorage.DeleteContainer.Definitions;

/// <summary>
/// Option parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// If true, the absence of the container is treated as a task failure. 
    /// This failure will be processed according to the 'ThrowErrorOnFailure' setting: 
    /// an exception will be thrown if 'ThrowErrorOnFailure' is true, or a Result object with Error details will be returned if false.
    /// If false, a missing container is treated as a valid "no-action" scenario, returning Success = false and Error = null, regardless of the 'ThrowErrorOnFailure' setting.
    /// </summary>
    [DefaultValue(false)]
    public bool FailOnContainerNotFound { get; set; }

    /// <summary>
    /// If true, throws an exception on task failure.
    /// If false, returns a result object with Success = false and Error details.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; }

    /// <summary>
    /// Optional custom error message included in the exception or result Error.Message on failure.
    /// If left empty, the original exception message is used.
    /// </summary>
    /// <example>DeleteContainer failed.</example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; }
}
