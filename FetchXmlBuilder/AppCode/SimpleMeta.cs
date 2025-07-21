using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XRM.Helpers.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Rappen.XTB.FXB.AppCode
{
    public abstract class SimpleAiMeta
    {
        /// <summary>LogicalName</summary>
        public string L { get; set; }

        /// <summary>DisplayName</summary>
        public string D { get; set; }

        public override string ToString() => $"{L} = {D}";
    }

    public class SimpleAiMetaEntity : SimpleAiMeta
    {
        public static List<SimpleAiMetaEntity> FromEntities(IEnumerable<EntityMetadata> ems)
        {
            var result = new List<SimpleAiMetaEntity>();
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

        private static SimpleAiMetaEntity FromEntity(EntityMetadata em)
        {
            if (em == null ||
                string.IsNullOrEmpty(em.LogicalName) ||
                em.LogicalName.StartsWith("msdyn_") ||
                em.LogicalName.StartsWith("msfp_"))
            {
                return null;
            }
            return new SimpleAiMetaEntity { L = em.LogicalName, D = em.ToDisplayName() };
        }
    }

    public class SimpleAiMetaAttribute : SimpleAiMeta
    {
        /// <summary>Type</summary>
        public string T { get; set; }

        /// <summary>Entity name</summary>
        public object E { get; set; }

        public static List<SimpleAiMetaAttribute> FromAttributes(IEnumerable<AttributeMetadata> ams, bool IncludeType)
        {
            var result = new List<SimpleAiMetaAttribute>();
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

        private static SimpleAiMetaAttribute FromAttribute(AttributeMetadata am, bool IncludeType)
        {
            if (am == null ||
                string.IsNullOrEmpty(am.LogicalName) ||
                am.LogicalName.StartsWith("msdyn_") ||
                am.LogicalName.StartsWith("msfp_"))
            {
                return null;
            }
            var result = new SimpleAiMetaAttribute { L = am.LogicalName, D = am.ToDisplayName() };
            if (IncludeType)
            {
                result.T = am.ToTypeName();
                if (am is LookupAttributeMetadata lookup)
                {
                    result.E = string.Join(",", lookup.Targets);
                }
                else if (am is EnumAttributeMetadata picklist)
                {
                    result.E = SimpleAiMetaOptionSet.FromChoice(picklist.OptionSet);
                }
                else if (am is MultiSelectPicklistAttributeMetadata multiSelect)
                {
                    result.E = SimpleAiMetaOptionSet.FromChoice(multiSelect.OptionSet);
                }
            }
            return result;
        }
    }

    public class SimpleAiMetaOptionSet : SimpleAiMeta
    {
        /// <summary>OptionSet/Picklist/Choice</summary>
        public List<SimpleAiMetaOptionsSetValue> O { get; set; }

        public static SimpleAiMetaOptionSet FromChoice(OptionSetMetadata osm)
        {
            var result = new SimpleAiMetaOptionSet
            {
                L = osm?.Name,
                D = osm?.DisplayName?.LocalizedLabels?.FirstOrDefault()?.Label,
                O = osm?.Options?
                    .Select(om => SimpleAiMetaOptionsSetValue.FromOption(om))
                    .Where(o => o != null)
                    .ToList()
            };
            return result;
        }
    }

    public class SimpleAiMetaOptionsSetValue : SimpleAiMeta
    {
        /// <summary>Value</summary>
        public int V { get; set; }

        public static SimpleAiMetaOptionsSetValue FromOption(OptionMetadata om)
        {
            if (om == null || string.IsNullOrEmpty(om.Value.ToString()) || string.IsNullOrEmpty(om.Label?.LocalizedLabels?.FirstOrDefault()?.Label))
            {
                return null;
            }
            return new SimpleAiMetaOptionsSetValue
            {
                D = om.Label?.LocalizedLabels?.FirstOrDefault()?.Label,
                V = om.Value.Value
            };
        }
    }
}