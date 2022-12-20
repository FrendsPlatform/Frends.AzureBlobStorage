namespace Frends.AzureBlobStorage.DeleteContainer.Definitions;

/// <summary>
/// Result parameters.
/// </summary>
public class Result
{
    /// <summary>
    /// Returns true when container has been deleted.
    /// </summary>
    /// <example>true</example>
    public bool ContainerWasDeleted { get; private set; }

    /// <summary>
    /// Description about action's result.
    /// </summary>
    /// <example>Container deleted successfully.</example>
    public string Message { get; private set; }

    internal Result(bool containerWasDeleted, string message) {
        ContainerWasDeleted = containerWasDeleted;
        Message = message;
    }
}
