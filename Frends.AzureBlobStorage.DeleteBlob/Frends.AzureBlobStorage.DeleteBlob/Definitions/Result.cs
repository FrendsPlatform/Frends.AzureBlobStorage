namespace Frends.AzureBlobStorage.DeleteBlob;

/// <summary>
/// Result-class for DeleteBlob-task.
/// </summary>
public class Result
{
    /// <summary>
    /// Was the operation successful?
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    internal Result(bool success)
    {
        Success = success;
    }
}