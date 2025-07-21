using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XRM.Helpers.Extensions;
using System.Collections.Generic;

namespace Rappen.XTB.FXB.AppCode
{
    public class SimpleAiMeta
    {
        /// <summary>LogicalName</summary>
        public string L { get; set; }

        /// <summary>DisplayName</summary>
        public string D { get; set; }

        /// <summary>Type</summary>
        public string T { get; set; }

        /// <summary>Entity</summary>
        public string E { get; set; }

        public static List<SimpleAiMeta> FromEntities(IEnumerable<EntityMetadata> ems)
        {
            var result = new List<SimpleAiMeta>();
            if (ems == null) return result;
            foreach (var em in ems)
            {
                var aiMeta = FromEntity(em);
                if (aiMeta != null)
                {
                    result.Add(aiMeta);
                }
            }
            return result;
        }

        public static List<SimpleAiMeta> FromAttributes(IEnumerable<AttributeMetadata> ams, bool IncludeType)
        {
            var result = new List<SimpleAiMeta>();
            if (ams == null) return result;
            foreach (var am in ams)
            {
                var aiMeta = FromAttribute(am, IncludeType);
                if (aiMeta != null)
                {
                    result.Add(aiMeta);
                }
            }
            return result;
        }

        public override string ToString() => $"{L} = {D}";

        private static SimpleAiMeta FromEntity(EntityMetadata em)
        {
            if (em == null ||
                string.IsNullOrEmpty(em.LogicalName) ||
                em.LogicalName.StartsWith("msdyn_") ||
                em.LogicalName.StartsWith("msfp_"))
            {
                return null;
            }
            return new SimpleAiMeta { L = em.LogicalName, D = em.ToDisplayName() };
        }

        private static SimpleAiMeta FromAttribute(AttributeMetadata am, bool IncludeType)
        {
            if (am == null ||
                string.IsNullOrEmpty(am.LogicalName) ||
                am.LogicalName.StartsWith("msdyn_") ||
                am.LogicalName.StartsWith("msfp_"))
            {
                return null;
            }
            var result = new SimpleAiMeta { L = am.LogicalName, D = am.ToDisplayName() };
            if (IncludeType)
            {
                result.T = am.ToTypeName();
                result.E = am is LookupAttributeMetadata lookup ? string.Join(",", lookup.Targets) : null;
            }
            return result;
        }
    }
}