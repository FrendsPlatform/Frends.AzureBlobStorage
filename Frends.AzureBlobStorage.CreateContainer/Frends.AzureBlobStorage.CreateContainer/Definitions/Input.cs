using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Frends.AzureBlobStorage.CreateContainer.Definitions.Enums;

namespace Frends.AzureBlobStorage.CreateContainer.Definitions
{
    public class Input
    {
        /// <summary>
        ///     Encoding type to encode result file: utf-8, utf-7, utf-32, utf-unicode, utf-ascii
        /// </summary>
        [DefaultValue(Encoding.UTF8)]
        [DisplayName("Encoding Type")]
        public Encoding Encode { get; set; }

        [DefaultValue(@"c:\temp\TestFile.xml")]
        [DisplayName("Source File")]
        [DisplayFormat(DataFormatString = "Text")]
        public string SourceFile { get; set; }

        /// <summary>
        ///     Uses stream to read file content.
        /// </summary>
        [DefaultValue(false)]
        [DisplayName("Stream content only")]
        public bool ContentsOnly { get; set; }

        /// <summary>
        ///     Works only when transferring stream content.
        /// </summary>
        [DefaultValue(false)]
        [DisplayName("Gzip compression")]
        public bool Compress { get; set; }
    }
}
