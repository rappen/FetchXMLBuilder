using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XRM.Helpers.Extensions;
using System;
using System.Collections.Generic;

namespace Rappen.XTB.FXB.AppCode
{
    public class SimpleAiMeta
    {
        public string LN { get; set; }
        public string DN { get; set; }

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

        public static List<SimpleAiMeta> FromAttributes(IEnumerable<AttributeMetadata> ams)
        {
            var result = new List<SimpleAiMeta>();
            if (ams == null) return result;
            foreach (var am in ams)
            {
                var aiMeta = FromAttribute(am);
                if (aiMeta != null)
                {
                    result.Add(aiMeta);
                }
            }
            return result;
        }

        public override string ToString() => $"{LN} = {DN}";

        private static SimpleAiMeta FromEntity(EntityMetadata em)
        {
            if (em == null ||
                string.IsNullOrEmpty(em.LogicalName) ||
                !em.LogicalName.Contains("_") ||
                em.LogicalName.StartsWith("msdyn_") ||
                em.LogicalName.StartsWith("msfp_") ||
                em.ToDisplayName().Equals(em.LogicalName, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return new SimpleAiMeta { LN = em.LogicalName, DN = em.ToDisplayName() };
        }

        private static SimpleAiMeta FromAttribute(AttributeMetadata am)
        {
            if (am == null ||
                string.IsNullOrEmpty(am.LogicalName) ||
                !am.LogicalName.Contains("_") ||
                am.LogicalName.StartsWith("msdyn_") ||
                am.LogicalName.StartsWith("msfp_") ||
                am.ToDisplayName().Equals(am.LogicalName, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return new SimpleAiMeta { LN = am.LogicalName, DN = am.ToDisplayName() };
        }
    }
}