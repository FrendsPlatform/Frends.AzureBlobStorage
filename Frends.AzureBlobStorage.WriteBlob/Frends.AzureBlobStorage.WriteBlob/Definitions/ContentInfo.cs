using System;

namespace Frends.AzureBlobStorage.WriteBlob.Definitions;

/// <summary>
/// Represents a serializable version of BlobContentInfo,
/// containing metadata returned after uploading or modifying a blob.
/// </summary>
public class ContentInfo
{
    /// <summary>
    /// Gets or sets the entity tag (ETag) for the blob, used for concurrency control.
    /// </summary>
    /// <example>"0x8DAA93A13A8B2C1"</example>
    public string ETag { get; set; }

    /// <summary>
    /// Gets or sets the date and time the blob was last modified in UTC.
    /// </summary>
    /// <example>2025-10-07T09:53:12.0000000Z</example>
    public DateTimeOffset LastModified { get; set; }

    /// <summary>
    /// Gets or sets the Base64-encoded MD5 content hash of the blob.
    /// This value can be used to verify data integrity.
    /// </summary>
    /// <example>"hN4fufxg+7/7zG0G1L5yJw=="</example>
    public string ContentHash { get; set; }

    /// <summary>
    /// Gets or sets the unique version ID of the blob (if versioning is enabled).
    /// </summary>
    /// <example>"2025-10-07T09:53:12.1234567Z"</example>
    public string VersionId { get; set; }

    /// <summary>
    /// Gets or sets the Base64-encoded SHA-256 hash of the encryption key used for client-side encryption.
    /// </summary>
    /// <example>"1a2b3c4d5e6f7890abcd1234ef567890abcd1234ef567890abcd1234ef567890"</example>
    public string EncryptionKeySha256 { get; set; }

    /// <summary>
    /// Gets or sets the name of the encryption scope that protects the blob.
    /// </summary>
    /// <example>"my-encryption-scope"</example>
    public string EncryptionScope { get; set; }

    /// <summary>
    /// Gets or sets the sequence number for the blob, applicable only for page blobs.
    /// </summary>
    /// <example>42</example>
    public long? BlobSequenceNumber { get; set; }
}