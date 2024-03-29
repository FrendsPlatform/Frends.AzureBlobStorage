﻿namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

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
/// How to handle an existing blob.
/// </summary>
public enum HandleExistingFile
{
    /// <summary>
    /// An error.
    /// </summary>
    Error,

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
#pragma warning disable CS1591 // self explanatory
    ConnectionString,
    OAuth2
#pragma warning restore CS1591 // self explanatory
}