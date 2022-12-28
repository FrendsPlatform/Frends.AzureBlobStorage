namespace Frends.AzureBlobStorage.DeleteBlob.Definitions;

/// <summary>
/// Options for Blob spanshots.
/// </summary>
public enum SnapshotDeleteOption
{
    /// <summary>
    /// No specific options.
    /// </summary>
    None,

    /// <summary>
    /// Also delete snapshots of the blob.
    /// </summary>
    IncludeSnapshots,

    /// <summary>
    /// Delete only blob's snapshots.
    /// </summary>
    DeleteSnapshotsOnly
}

/// <summary>
/// Connection methods.
/// </summary>
public enum ConnectionMethod
{
#pragma warning disable CS1591 // self explanatory
    ConnectionString,
    OAuth2
#pragma warning restore CS1591 // self explanatory
}