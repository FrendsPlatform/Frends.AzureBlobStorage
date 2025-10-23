namespace Frends.AzureBlobStorage.CreateContainer.Definitions;

/// <summary>
/// Error information.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>CreateContainer failed</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional error information.
    /// </summary>
    /// <example>null</example>
    public object AdditionalInfo { get; set; }
}

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Container created.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// URI string of newly created container.
    /// </summary>
    /// <example>https://test.blob.core.windows.net/test8f237ae0-ad33-b4b3-48d9-23b20a14c909</example>
    public string Uri { get; private set; }

    /// <summary>
    /// Error message if operation failed.
    /// </summary>
    /// <example>CreateContainer failed</example>
    public string ErrorMessage { get; private set; }

    /// <summary>
    /// Error information if operation failed.
    /// </summary>
    public Error Error { get; private set; }

    internal Result(bool success, string uri)
    {
        Success = success;
        Uri = uri;
        ErrorMessage = string.Empty;
        Error = null;
    }

    internal Result(bool success, string uri, string errorMessage)
    {
        Success = success;
        Uri = uri;
        ErrorMessage = errorMessage;
        Error = errorMessage != string.Empty ? new Error { Message = errorMessage, AdditionalInfo = null } : null;
    }

    internal Result(bool success, string uri, string errorMessage, object additionalInfo)
    {
        Success = success;
        Uri = uri;
        ErrorMessage = errorMessage;
        Error = errorMessage != string.Empty ? new Error { Message = errorMessage, AdditionalInfo = additionalInfo } : null;
    }
}
