namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Possible operations if file already exists.
/// </summary>
public enum FileExistsAction
{
#pragma warning disable CS1591 // self explanatory
    Error,
    Rename,
    Overwrite
#pragma warning restore CS1591 // self explanatory
}