using System;

namespace Frends.AzureBlobStorage.ListContainers.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Failed to list containers.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { Exception = new ArgumentException("Settings must be of the form 'name=value'.") }}</example>
    public Exception AdditionalInfo { get; set; }
}
