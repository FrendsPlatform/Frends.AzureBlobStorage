using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ListContainers.Definitions
{
    public class ContainerInfo
    {
        public string Name { get; set; }
        public string PublicAccess { get; set; }
        public DateTime? LastModified { get; set; }
        public string? ETag { get; set; }
        public string? LeaseStatus { get; set; }
        public string? LeaseState { get; set; }
    }
}
