using System.ComponentModel;

namespace Frends.AzureBlobStorage.ListContainers.Definitions
{
    /// <summary>
    /// Essential parameters.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Flags indicating which container states to include when listing (e.g. deleted or system containers).
        /// </summary>
        /// <example>BlobContainerStates.Deleted</example>
        [DefaultValue(ContainerStateFilter.None)]
        public ContainerStateFilter States { get; set; }

#nullable enable
        /// <summary>
        /// Filter by container name prefix. Only containers starting with this value will be returned.
        /// </summary>
        /// <example>"test-"</example>
        public string? Prefix { get; set; }
    }
}
