using System;

namespace Rappen.Xrm.FetchXmlBuilder.Models
{
    public class MetadataFilterOptions
    {
        /// <summary>True if "All solutions" was chosen.</summary>
        public bool RefreshAllSolutions { get; set; }

        /// <summary>True if "Unmanaged only" was chosen.</summary>
        public bool RefreshUnmanagedSolutions { get; set; }

        /// <summary>If "Specific solution" was chosen, its ID; otherwise Guid.Empty.</summary>
        public Guid SolutionId { get; set; }

        /// <summary>If "Specific publisher" was chosen, its ID; otherwise Guid.Empty.</summary>
        public Guid PublisherId { get; set; }
    }
}