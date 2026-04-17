namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Result of the DeleteBlob task.
/// object { bool Success, Error Error }
/// </summary>
public class Result
{
    /// <summary>
    /// Returns true if the blob was successfully deleted.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Error information when Success is false.
    /// object { string Message, object AdditionalInfo }
    /// </summary>
    /// <example>null</example>
    public DeleteBlobError Error { get; private set; }

    internal Result(bool success, DeleteBlobError error = null)
    {
        Success = success;
        Error = error;
    }
}

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
