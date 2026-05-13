namespace Frends.AzureBlobStorage.DeleteContainer.Definitions;

/// <summary>
/// Error information returned when the task fails and ThrowErrorOnFailure is false.
/// </summary>
public class Error
{
    /// <summary>
    /// Human-readable error message describing the failure.
    /// </summary>
    /// <example>DeleteContainer failed: The specified container does not exist.</example>
    public string Message { get; internal set; }

    /// <summary>
    /// Additional details about the failure, typically the exception that was thrown.
    /// </summary>
    /// <example>System.Exception: The specified container does not exist.</example>
    public object AdditionalInfo { get; internal set; }
}
