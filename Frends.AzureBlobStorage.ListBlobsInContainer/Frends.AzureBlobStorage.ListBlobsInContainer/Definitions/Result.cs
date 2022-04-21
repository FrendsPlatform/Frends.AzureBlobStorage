using System.Collections.Generic;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions
{
    /// <summary>
    /// Returns list of blobs and subdirectories in Azure Storage container with flat or hierarchical listing structure.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// List of blobs.
        /// </summary>
        public List<BlobData> BlobList { get; set; }
    }
}
