using System.Collections.Generic;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// List of blobs.
    /// </summary>
    /// <example>[ { Type = Block, Name = file.txt, URL = https://storageaccount.blob.core.windows.net/containername/file.txt, ETag = 0x8D6FACDEB709651, CreatedOn = 2023-12-11T09:00:00.743185+02:00, LastModified = 2023-12-11T09:00:00.743185+02:00 } ]</example>
    public List<BlobData> BlobList { get; private set; }

    internal Result(List<BlobData> blobList)
    {
        BlobList = blobList;
    }
}