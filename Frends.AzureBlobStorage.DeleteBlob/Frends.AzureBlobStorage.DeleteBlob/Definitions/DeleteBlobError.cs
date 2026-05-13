namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Error details returned when the task fails and ThrowErrorOnFailure is false.
/// </summary>
public class DeleteBlobError
{
    /// <summary>
    /// Human-readable error message. Always present when an error occurs.
    /// </summary>
    /// <example>Blob file.txt doesn't exist in container test-container.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional error context, such as the exception thrown by the Azure SDK.
    /// </summary>
    /// <example>System.Exception: ...</example>
    public object AdditionalInfo { get; set; }
}
