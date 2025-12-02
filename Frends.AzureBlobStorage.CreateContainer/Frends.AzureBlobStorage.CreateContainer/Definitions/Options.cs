using System.ComponentModel;

namespace Frends.AzureBlobStorage.CreateContainer.Definitions;

/// <summary>
/// Options for the CreateContainer task.
/// </summary>
public class Options
{
    /// <summary>
    /// Throw error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Error message on failure.
    /// </summary>
    /// <example>CreateContainer failed</example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
