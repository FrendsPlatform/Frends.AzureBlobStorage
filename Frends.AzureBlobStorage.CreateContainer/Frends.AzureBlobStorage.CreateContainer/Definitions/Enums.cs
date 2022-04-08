using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.CreateContainer.Definitions
{
    public class Enums
    {

        [Flags]
        public enum Encoding
        {
            UTF8,
            UTF7,
            UTF32,
            Unicode,
            ASCII,
        }

        public enum AzureBlobType
        {
            Append,
            Block,
            Page
        }
    }
}
