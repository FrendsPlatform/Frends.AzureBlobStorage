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
/// Blob types.
/// </summary>
public enum AzureBlobType
{
    /// <summary>
    /// Made up of blocks like block blobs, but are optimized for append operations. Append blobs are ideal for scenarios such as logging data from virtual machines.
    /// </summary>
    Append,

    /// <summary>
    /// Store text and binary data. Block blobs are made up of blocks of data that can be managed individually. Block blobs can store up to about 190.7 TiB.
    /// </summary>
    Block,

    /// <summary>
    /// Store random access files up to 8 TiB in size. Page blobs store virtual hard drive (VHD) files and serve as disks for Azure virtual machines.
    /// </summary>
    Page
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
/// Connection methods.
/// </summary>
public enum ConnectionMethod
{
    /// <summary>
    /// Authenticate with connectiong string.
    /// </summary>
    ConnectionString,

    /// <summary>
    /// Authenticate with SAS Token. Requires Storage URI.
    /// </summary>
    SasToken,

    /// <summary>
    /// OAuth2.
    /// </summary>
    OAuth2
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