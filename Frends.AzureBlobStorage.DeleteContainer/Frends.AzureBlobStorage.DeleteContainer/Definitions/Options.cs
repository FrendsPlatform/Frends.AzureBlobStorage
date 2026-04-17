using System.ComponentModel;
namespace Frends.AzureBlobStorage.DeleteContainer.Definitions;

/// <summary>
/// Option parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// If true, throw an error if the container to be deleted doesn't exist.
    /// If false, the task will return Success = false without throwing.
    /// </summary>
    /// <example>false</example>
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
