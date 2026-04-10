namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

/// <summary>
/// Upload a single file or entire directory.
/// </summary>
public enum UploadSourceType
{
#pragma warning disable CS1591 // self explanatory
    File,
    Directory
#pragma warning restore CS1591 // self explanatory
}

/// <summary>
/// Action taken when a blob already exists.
/// </summary>
public enum OnExistingFile
{
    /// <summary>
    /// Throw an error.
    /// </summary>
    Throw,

    /// <summary>
    /// Overwrite with source file.
    /// </summary>
    Overwrite,

    /// <summary>
    /// Append blob with 'Source File'. Block and Page blob will be downloaded as temp file which will be deleted after local append and reupload processes are complete. No downloading needed for Append Blob.
    /// </summary>
    Append
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
    Windows1252,
#pragma warning restore CS1591 // self explanatory
    /// <summary>
    /// Other enables users to add other encoding options as string.
    /// </summary>
    Other,
}
