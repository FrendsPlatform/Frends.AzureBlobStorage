namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Error information returned when <see cref="Options.ThrowErrorOnFailure"/> is <c>false</c>
/// and the task encounters an error.
/// </summary>
public class Error
{
    /// <summary>
    /// Human-readable error message.
    /// </summary>
    /// <example>The specified blob does not exist.</example>
    public string Message { get; internal set; }

    /// <summary>
    /// Additional error detail, typically the original exception.
    /// </summary>
    /// <example>System.Exception: ...</example>
    public object AdditionalInfo { get; internal set; }
}
