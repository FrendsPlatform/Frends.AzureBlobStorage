using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Frends.AzureBlobStorage.CreateContainer.Definitions.Enums;

namespace Frends.AzureBlobStorage.CreateContainer.Definitions
{
    public class Destination
    {
        /// <summary>
        ///     Connection string to Azure storage
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayName("Connection String")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Name of the azure blob storage container where the data will be uploaded.
        ///     Naming: lowercase
        ///     Valid chars: alphanumeric and dash, but cannot start or end with dash
        /// </summary>
        [DefaultValue("test-container")]
        [DisplayName("Container Name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }

        /// <summary>
        ///     Determines if the container should be created if it does not exist
        /// </summary>
        [DisplayName("Create container if it does not exist")]
        public bool CreateContainerIfItDoesNotExist { get; set; }

        /// <summary>
        ///     Azure blob type to upload: Append, Block or Page
        /// </summary>
        [DefaultValue(AzureBlobType.Block)]
        [DisplayName("Blob Type")]
        public AzureBlobType BlobType { get; set; }

        /// <summary>
        ///     Source file can be renamed to this name in azure blob storage
        /// </summary>
        [DefaultValue("")]
        [DisplayName("Rename source file")]
        [DisplayFormat(DataFormatString = "Text")]
        public string RenameTo { get; set; }

        /// <summary>
        ///     Set desired content-type. If empty, task tries to guess from mime-type.
        /// </summary>
        [DefaultValue("")]
        [DisplayName("Force Content-Type")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ContentType { get; set; }

        /// <summary>
        ///     Set desired content-encoding. Defaults to UTF8 BOM.
        /// </summary>
        [DefaultValue("")]
        [DisplayName("Force Content-Encoding")]
        [DisplayFormat(DataFormatString = "Text")]
        public string FileEncoding { get; set; }

        /// <summary>
        ///     Should upload operation overwrite existing file with same name?
        /// </summary>
        [DefaultValue(true)]
        [DisplayName("Overwrite existing file")]
        public bool Overwrite { get; set; }

        /// <summary>
        ///     How many work items to process concurrently.
        /// </summary>
        [DefaultValue(64)]
        [DisplayName("Parallel Operation")]
        public int ParallelOperations { get; set; }

    }
}
