namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Result parameters.
/// </summary>
public class Result
{
    /// <summary>
    /// Returns true if the blob has been deleted.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Additional information.
    /// </summary>
    /// <example>Blob file.txt deleted from container test-container."</example>
    public string Info { get; private set; }

    internal Result(bool success, string info)
    {
        Success = success;
        Info = info;
    }
}
