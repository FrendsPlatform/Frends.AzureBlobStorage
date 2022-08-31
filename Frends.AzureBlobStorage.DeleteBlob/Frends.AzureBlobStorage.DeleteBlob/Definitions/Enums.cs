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