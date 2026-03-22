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

/// <summary>
/// Content encoding.
/// </summary>
public enum FileEncoding
{
#pragma warning disable CS1591 // self explanatory
    UTF8,
    Default,
    ASCII,
    WINDOWS1252,
#pragma warning restore CS1591 // self explanatory
    /// <summary>
    /// Other enables users to add other encoding options as string.
    /// </summary>
    Other,
}
