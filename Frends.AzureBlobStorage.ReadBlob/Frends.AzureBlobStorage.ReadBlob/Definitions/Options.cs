using System.ComponentModel;

namespace Frends.AzureBlobStorage.ReadBlob.Definitions
{
    /// <summary>
    /// Options for the task.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Encoding name in which blob content is read.
        /// </summary>
        /// <example>UTF8</example>
        [DefaultValue(Encode.UTF8)]
        public Encode Encoding { get; set; }
    }
}

