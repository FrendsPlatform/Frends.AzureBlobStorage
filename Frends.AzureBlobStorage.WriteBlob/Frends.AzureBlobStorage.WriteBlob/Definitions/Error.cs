using System;

namespace Frends.AzureBlobStorage.WriteBlob.Definitions;

/// <summary>
/// Error details returned on failure when ThrowErrorOnFailure is false.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Connection refused: unable to reach Azure Blob Storage endpoint.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>null</example>
    public Exception AdditionalInfo { get; set; }
}
