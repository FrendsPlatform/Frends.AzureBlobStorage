using System.Collections.Generic;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions
{
    /// <summary>
    ///     Returns list of container's blobs.
    /// </summary>
    public class Result
    {
        /// <summary>
        ///     Blob's type.
        /// </summary>
        public string BlobType { get; set; }

        /// <summary>
        ///     Blob's URI.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        ///     Blob's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Blob's entity tag.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        ///     List of container's blobs.
        /// </summary>
        public List<Result> BlobList { get; set; }

    }
}

