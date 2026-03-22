namespace Frends.AzureBlobStorage.ListContainers.Definitions;

/// <summary>
/// Flags used to filter which types of containers are returned when listing.
/// </summary>
public enum ContainerStateFilter
{
    /// <summary>No filtering; only normal containers are returned.</summary>
    None = 0,

    /// <summary>Include system containers (e.g., $logs, $web).</summary>
    System = 1,

    /// <summary>Include soft-deleted containers.</summary>
    Deleted = 2,
}
