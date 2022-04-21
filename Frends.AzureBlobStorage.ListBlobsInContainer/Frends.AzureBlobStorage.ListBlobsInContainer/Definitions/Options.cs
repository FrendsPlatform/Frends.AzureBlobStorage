using System.ComponentModel;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions
{
    /// <summary>
    /// Options for the task.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// List blobs in a flat listing structure or hierarchically.
        /// Hierarchical listing returns container's blobs and subdirectories names such as file.txt or directoryname/. Flat listing does the same as hierarchical listing but also returns blobs in those subdirectories such as directoryname/file.txt.
        /// </summary>
        [DefaultValue(ListingStructure.Flat)]
        public ListingStructure ListingStructure { get; set; }

        /// <summary>
        /// Specify a prefix to return blobs whose names begin with that character or string.
        /// </summary>
        public string Prefix { get; set; }

    }
}
