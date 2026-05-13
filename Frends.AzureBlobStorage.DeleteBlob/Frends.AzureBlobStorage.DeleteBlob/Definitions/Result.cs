namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Result of the DeleteBlob task.
/// object { bool Success, Error Error }
/// </summary>
public class Result
{
    /// <summary>
    /// True if the blob was successfully deleted. 
    /// False if the blob did not exist or if an error occurred (and 'ThrowErrorOnFailure' is false).
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
