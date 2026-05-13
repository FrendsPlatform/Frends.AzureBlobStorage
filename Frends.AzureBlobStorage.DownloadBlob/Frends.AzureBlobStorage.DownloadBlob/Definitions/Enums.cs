namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Possible operations if the destination file already exists.
/// </summary>
public enum FileExistsAction
{
    /// <summary>
    /// Throw an exception if the destination file already exists.
    /// </summary>
    Throw,

    /// <summary>
    /// Rename the downloaded file by appending an incrementing number, e.g. file(1).txt.
    /// </summary>
    Rename,

    /// <summary>
    /// Overwrite the existing destination file.
    /// </summary>
    Overwrite,
}

/// <summary>
/// Content encoding used when writing the downloaded file.
/// </summary>
public enum FileEncoding
{
    /// <summary>
    /// UTF-8 with byte-order mark (BOM).
    /// </summary>
    UTF8WithBOM,

    /// <summary>
    /// UTF-8 without byte-order mark.
    /// </summary>
    UTF8,

    /// <summary>
    /// System default encoding.
    /// </summary>
    Default,

    /// <summary>
    /// ASCII encoding.
    /// </summary>
    ASCII,

    /// <summary>
    /// Windows-1252 (Western European) code page.
    /// </summary>
    Windows1252,

    /// <summary>
    /// Other enables users to add other encoding options as string.
    /// </summary>
    Other,
}
