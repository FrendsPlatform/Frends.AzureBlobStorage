using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DeleteBlob
{
    /// <summary>
    /// Options-class for DeleteBlob-task.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// What should be done with blob snapshots?
        /// </summary>
        [DisplayName("Snapshot Delete Option")]
        [DefaultValue(SnapshotDeleteOption.IncludeSnapshots)]
        public SnapshotDeleteOption SnapshotDeleteOption { get; set; }

        /// <summary>
        /// Delete blob only if the ETag matches.
        /// Leave empty if verification is not needed.
        /// </summary>
        [DisplayName("Verify ETag When Deleting")]
        [DefaultValue("0x9FE13BAA323E5A4")]
        [DisplayFormat(DataFormatString = "Text")]
        public string VerifyETagWhenDeleting { get; set; }
    }

    /// <summary>
    /// Options for Blob spanshots.
    /// </summary>
    public enum SnapshotDeleteOption
    {
        /// <summary>
        /// No specific options.
        /// </summary>
        None,

        /// <summary>
        /// Also delete snapshots of the blob.
        /// </summary>
        IncludeSnapshots,

        /// <summary>
        /// Delete only blob's snapshots.
        /// </summary>
        DeleteSnapshotsOnly
    }
}
