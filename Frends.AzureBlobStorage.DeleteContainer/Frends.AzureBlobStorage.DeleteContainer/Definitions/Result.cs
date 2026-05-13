namespace Frends.AzureBlobStorage.DeleteContainer.Definitions;

/// <summary>
/// Result parameters.
/// object { bool Success, object Error { string Message, object AdditionalInfo } }
/// </summary>
public class Result
{
    /// <summary>
    /// Returns true when the container was successfully deleted.
    /// Returns false when the container was not found and FailOnContainerNotFound is false, or when any failure occurs and ThrowErrorOnFailure is false.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Error details when the task fails and ThrowErrorOnFailure is false. Null on success.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }

    internal Result(bool success, Error error)
    {
        Success = success;
        Error = error;
    }
}
