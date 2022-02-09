#pragma warning disable 1591

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DownloadBlob
{
    /// <summary>
    ///     Output class for the task.
    /// </summary>
    public class DownloadBlobOutput
    {
        /// <summary>
        ///     Name of the downloaded file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     Directory where the file was downloaded.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        ///     Full path to the downloaded file.
        /// </summary>
        public string FullPath { get; set; }
    }

    public class SourceProperties
    {
        /// <summary>
        ///     Connection string to Azure storage.
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Name of the azure blob storage container where the file is downloaded from.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }

        /// <summary>
        ///     Name of the blob to download.
        /// </summary>
        [DefaultValue("example.xml")]
        [DisplayFormat(DataFormatString = "Text")]
        public string BlobName { get; set; }

        /// <summary>
        ///     Set encoding manually.
        ///     Empty value tries to get encoding set in Azure.
        ///     Supported values are utf-8, utf-7, utf-32, unicode, bigendianunicode and ascii.
        /// </summary>
        [DefaultValue("UTF-8")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Encoding { get; set; }
    }

    public class DestinationFileProperties
    {
        /// <summary>
        ///     Download destination directory.
        /// </summary>
        [DefaultValue(@"c:\temp")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Directory { get; set; }

        /// <summary>
        ///     Error: Throws exception if destination file exists.
        ///     Rename: Adds '(1)' at the end of file name. Incerements the number if (1) already exists.
        ///     Overwrite: Overwrites existing file.
        /// </summary>
        [DefaultValue(FileExistsAction.Error)]
        public FileExistsAction FileExistsOperation { get; set; }
    }

    public enum FileExistsAction
    {
        Error,
        Rename,
        Overwrite
    }
}
