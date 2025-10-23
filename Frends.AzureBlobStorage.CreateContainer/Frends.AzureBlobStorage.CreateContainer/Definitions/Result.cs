namespace Frends.AzureBlobStorage.CreateContainer.Definitions;

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

    internal Result(bool success, string uri)
    {
        Success = success;
        Uri = uri;
        ErrorMessage = string.Empty;
    }

    internal Result(bool success, string uri, string errorMessage)
    {
        Success = success;
        Uri = uri;
        ErrorMessage = errorMessage;
    }
}
