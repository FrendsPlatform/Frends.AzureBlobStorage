using System;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

/// <summary>
/// Error information from the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Failed to list blobs.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { Exception = new ArgumentException("...") }</example>
    public Exception AdditionalInfo { get; set; }
}
