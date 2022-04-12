using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.DeleteContainer.Definitions
{
    public class Options
    {
        /// <summary>
        ///     Throw an error if container isn't found to delete.
        /// </summary>
        [DefaultValue(false)]
        [DisplayName("Throw error if container doesn't exist")]
        public bool IfThrow { get; set; }
    }
}
