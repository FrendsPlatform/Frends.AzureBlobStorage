using System.ComponentModel;

namespace Frends.AzureBlobStorage.ReadBlob.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Encoding name in which blob content is read.
    /// </summary>
    /// <example>UTF8</example>
    [DefaultValue(Encode.UTF8)]
    public Encode Encoding { get; set; }

    /// <summary>
    /// Whether to throw an exception on task failure. When false, errors are returned in the result's Error property.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Custom error message prefix used when an exception is thrown or returned. If empty, the original exception message is used.
    /// </summary>
    /// <example>ReadBlob failed</example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}

