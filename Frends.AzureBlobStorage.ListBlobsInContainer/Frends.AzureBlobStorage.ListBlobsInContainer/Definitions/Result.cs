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
    public List<BlobData> BlobList { get; private set; }

    internal Result(List<BlobData> blobList)
    {
        BlobList = blobList;
    }
}