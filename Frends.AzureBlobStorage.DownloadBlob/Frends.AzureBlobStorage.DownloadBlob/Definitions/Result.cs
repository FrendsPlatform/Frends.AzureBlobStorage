namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Result returned by the DownloadBlob task.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the download completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; internal set; }

    /// <summary>
    /// Full path to the downloaded file on the local file system.
    /// <c>null</c> when the task fails and Options.ThrowErrorOnFailure is <c>false</c>.
    /// </summary>
    /// <example>c:\temp\sample.txt</example>
    public string FilePath { get; internal set; }

    /// <summary>
    /// Error details when the task fails and Options.ThrowErrorOnFailure is <c>false</c>.
    /// <c>null</c> on success.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; internal set; }
}
