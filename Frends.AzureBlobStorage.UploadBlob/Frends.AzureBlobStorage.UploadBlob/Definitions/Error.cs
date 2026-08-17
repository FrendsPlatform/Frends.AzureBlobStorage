namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

/// <summary>
/// Error details returned when the Task fails and Options.ThrowErrorOnFailure is false.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>The specified container does not exist.</example>
    public string Message { get; internal set; }

    /// <summary>
    /// The exception that caused the failure.
    /// </summary>
    /// <example>null</example>
    public object AdditionalInfo { get; internal set; }
}
