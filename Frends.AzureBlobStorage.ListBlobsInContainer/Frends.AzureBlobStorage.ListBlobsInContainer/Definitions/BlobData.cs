namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions
{
    /// <summary>
    /// Blob's data.
    /// </summary>
    public class BlobData
    {
        /// <summary>
        /// Flat or hierarchically listing structure.
        /// </summary>
        /// <example>Flat</example>
        public string ListingStructure { get; set; }

        /// <summary>
        /// Blob's type.
        ///     Directory is a sub directory in container.
        ///     Block blobs store text and binary data. Block blobs are made up of blocks of data that can be managed individually.
        ///     Append blobs are made up of blocks like block blobs, but are optimized for append operations.
        ///     Page blobs store random access files up to 8 TiB in size.Page blobs store virtual hard drive(VHD) files and serve as disks for Azure virtual machines.
        /// </summary>
        /// <example>Block</example>
        public string Type { get; set; }

        /// <summary>
        /// Blob's name.
        /// </summary>
        /// <example>file.txt</example>
        /// <example>directory/file.txt</example>
        
        public string Name { get; set; }


        /// <summary>
        /// Blob's URI.
        /// </summary>
        /// <example>https://storageaccount.blob.core.windows.net/containername/file.txt?</example>
        public string URI { get; set; }


        /// <summary>
        /// Blob's entity tag.
        /// </summary>
        /// <example>0x8D6FACDEB709651</example>
        public string ETag { get; set; }
    }
}
