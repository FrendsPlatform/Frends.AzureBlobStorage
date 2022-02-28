using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CS1591

namespace Frends.AzureBlobStorage.DownloadBlob
{
    /// <summary>
    /// Options-class for DownloadBlob-task.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Download destination directory.
        /// </summary>
        [DefaultValue(@"c:\temp")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Directory { get; set; }

        /// <summary>
        /// Error: Throws exception if destination file exists.
        /// Rename: Adds '(1)' at the end of file name. Incerements the number if (1) already exists.
        /// Overwrite: Overwrites existing file.
        /// </summary>
        [DefaultValue(FileExistsAction.Error)]
        public FileExistsAction FileExistsOperation { get; set; }
    }

    /// <summary>
    /// Possible operations if file already exists.
    /// </summary>
    public enum FileExistsAction
    {
        Error,
        Rename,
        Overwrite
    }
}
