namespace Frends.AzureBlobStorage.ReadBlob.Definitions;

/// <summary>
/// Error details returned when a task fails and ThrowErrorOnFailure is false.
/// </summary>
public class Error
{
    /// <summary>
    /// Human-readable error description.
    /// </summary>
    /// <example>ReadBlob failed: The specified blob does not exist.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional error details, typically the caught exception.
    /// </summary>
    /// <example>System.Exception: The specified blob does not exist.</example>
    public object AdditionalInfo { get; set; }
}
