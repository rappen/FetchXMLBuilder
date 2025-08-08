using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XRM.Helpers.FetchXML;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Builder
{
    internal static class Validations
    {
        internal const string allowedaliaschars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";

        internal static ControlValidationResult GetWarning(TreeNode node, FetchXmlBuilder fxb)
        {
            if (!fxb.settings.ShowValidation || node == null)
            {
                return null;
            }
            switch (node.Name)
            {
                case "fetch":
                    return ValidateFetch(node, fxb);

                case "entity":
                    return ValidateEntity(node, fxb);

                case "link-entity":
                    return ValidateLinkEntity(node, fxb);

                case "attribute":
                    return ValidateAttribute(node, fxb);

                case "all-attributes":
                    return ValidateAllAttribute(node, fxb);

                case "filter":
                    return ValidateFilter(node, fxb);

                case "condition":
                    return ValidateCondition(node, fxb);

                case "value":
                    return ValidateValue(node, fxb);

                case "order":
                    return ValidateOrder(node, fxb);
            }
            return null;
        }

        internal static ControlValidationResult ValidateAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return null;
            }
            var aliasinvalid = alias.Where(c => !allowedaliaschars.Contains(c) && !char.IsDigit(c));
            if (aliasinvalid.Any())
            {
                return new ControlValidationResult(ControlValidationLevel.Error, $"Incorrect characters in Alias: {string.Join(", ", aliasinvalid)}");
            }
            if (!allowedaliaschars.Contains(alias[0]))
            {
                return new ControlValidationResult(ControlValidationLevel.Error, $"Incorrect first characters in Alias: {alias[0]}");
            }
            return null;
        }

        private static ControlValidationResult ValidateFetch(TreeNode node, FetchXmlBuilder fxb)
        {
            if (!node.Nodes.OfType<TreeNode>().Any(n => n.Name == "entity"))
            {
                return new ControlValidationResult(ControlValidationLevel.Error, "Missing entity under the fetch.");
            }
            if (!string.IsNullOrWhiteSpace(node.Value("datasource")) && node.Value("datasource") != "archive")
            {
                return new ControlValidationResult(ControlValidationLevel.Error, "Invalid data source value");
            }
            if (node.Value("datasource") == "archive" && node.IsFetchAggregate())
            {
                return new ControlValidationResult(ControlValidationLevel.Error, "Aggregate queries cannot use Long Term Retention data", "https://learn.microsoft.com/en-us/power-apps/maker/data-platform/data-retention-view#limitations-for-retrieval-of-retained-data");
            }
            return null;
        }

        private static ControlValidationResult ValidateEntity(TreeNode node, FetchXmlBuilder fxb)
        {
            var name = node.Value("name");
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ControlValidationResult(ControlValidationLevel.Warning, "Entity Name must be included.");
            }
            return null;
        }

        private static ControlValidationResult ValidateLinkEntity(TreeNode node, FetchXmlBuilder fxb)
        {
            if (node.TreeView.Nodes[0].Value("datasource") == "archive")
            {
                return new ControlValidationResult(ControlValidationLevel.Error, "Link-Entity cannot be used with Long Term Retention data", "https://learn.microsoft.com/en-us/power-apps/maker/data-platform/data-retention-view#limitations-for-retrieval-of-retained-data");
            }

            var name = node.Value("name");
            var alias = node.Value("alias");
            if (ValidateAlias(alias) is ControlValidationResult aliasresult)
            {
                return aliasresult;
            }
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(node.Value("to")) ||
                string.IsNullOrWhiteSpace(node.Value("from")))
            {
                return new ControlValidationResult(ControlValidationLevel.Warning, "Link-Entity must include Name, To, From");
            }
            if (fxb.settings.Layout.Enabled &&
                string.IsNullOrWhiteSpace(alias) &&
                node.Nodes.OfType<TreeNode>().Any(n => n.Name == "attribute"))
            {
                return new ControlValidationResult(ControlValidationLevel.Warning, "Using Layout: Alias is needed to show these attributes");
            }
            var type = node.Value("link-type");
            if (node.Parent is TreeNode parent && parent.Name == "filter")
            {
                if (!type.Equals("any") && !type.Equals("not any") && !type.Equals("all") && !type.Equals("not all"))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Filter by link-entity has to be type any, not any, all, or not all", "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/filter-rows#filter-on-values-in-related-records");
                }
                if (node.Nodes.OfType<TreeNode>().Any(c => c.Name == "attribute"))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Link-entity under filter can't return any attributes", "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/filter-rows#filter-on-values-in-related-records");
                }
            }
            else
            {
                if (type.Equals("any") || type.Equals("not any") || type.Equals("all") || type.Equals("not all"))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Link-entity type any, not any, all, and not all can only be under a filter", "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/filter-rows#filter-on-values-in-related-records");
                }
            }
            if (node.Value("intersect") != "true" &&
                fxb.GetAttribute(name, node.Value("from")) is AttributeMetadata fromAttr && fromAttr.IsPrimaryId == false)
            {
                return new ControlValidationResult(ControlValidationLevel.Info, "Links to records that aren't parents may cause paging issues", "https://markcarrington.dev/2021/02/23/msdyn365-internals-paging-gotchas/#multiple_linked_entities");
            }
            return null;
        }

        private static ControlValidationResult ValidateAttribute(TreeNode node, FetchXmlBuilder fxb)
        {
            var name = node.Value("name");
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ControlValidationResult(ControlValidationLevel.Warning, "Attribute Name must be included.");
            }
            var parententity = node.LocalEntityName();
            if (fxb.entities != null)
            {
                if (fxb.GetAttribute(parententity, name) is AttributeMetadata metaatt)
                {
                    if (metaatt.IsValidForGrid.HasValue && metaatt.IsValidForGrid.Value == false && metaatt.IsPrimaryId.Value != true)
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, $"Attribute '{name}' has 'IsValidForGrid=false'.");
                    }
                }
                else
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, $"Attribute '{name}' is not in the table '{parententity}'.");
                }
            }
            if (node.Parent.ParentNotEntity().Name == "filter")
            {
                return new ControlValidationResult(ControlValidationLevel.Error, "Attribute under filter is not allowed.");
            }
            var alias = node.Value("alias");
            if (ValidateAlias(alias) is ControlValidationResult aliasresult)
            {
                return aliasresult;
            }
            if (node.IsFetchAggregate())
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Aggregate should always have an Alias.", "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/aggregate-data#about-aggregation");
                }

                if (node.Value("groupby") == "true")
                {
                    if (!HasSortOnAttribute(node))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Aggregate queries should be sorted by all grouped attributes for correct paging.", "https://markcarrington.dev/2022/01/13/fetchxml-aggregate-queries-lookup-fields-and-paging/");
                    }

                    if (fxb.GetAttribute(parententity, name) is LookupAttributeMetadata)
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Grouping by lookup columns can give inconsistent results across multiple pages.", "https://markcarrington.dev/2022/01/13/fetchxml-aggregate-queries-lookup-fields-and-paging/");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(alias))
                {
                    return new ControlValidationResult(ControlValidationLevel.Info, "Alias is not recommended for not Aggregate queries.");
                }

                if (node.IsFetchDistinct() && !HasSortOnAttribute(node) && !HasPrimaryIdAttribute(node.Parent, fxb))
                {
                    return new ControlValidationResult(ControlValidationLevel.Info, "Distinct queries should be sorted by all attributes for correct paging.", "https://markcarrington.dev/2020/12/08/dataverse-paging-with-distinct/");
                }
            }
            return null;
        }

        private static ControlValidationResult ValidateAllAttribute(TreeNode node, FetchXmlBuilder fxb)
        {
            if (fxb.settings.Layout.Enabled)
            {
                return new ControlValidationResult(ControlValidationLevel.Warning, "Using Layout: All-Attributes is not possible to show");
            }
            return null;
        }

        private static ControlValidationResult ValidateFilter(TreeNode node, FetchXmlBuilder fxb)
        {
            if (node.Nodes.Count == 0)
            {
                return new ControlValidationResult(ControlValidationLevel.Info, "Filter shound have at least one Condition.");
            }
            return null;
        }

        private static ControlValidationResult ValidateCondition(TreeNode node, FetchXmlBuilder fxb)
        {
            var attribute = node.Value("attribute");
            if (string.IsNullOrWhiteSpace(attribute))
            {
                return new ControlValidationResult(ControlValidationLevel.Warning, "Attribute must be included.");
            }
            var oper = node.Value("operator");
            if (oper == "contains" || oper == "does-not-contain")
            {
                return new ControlValidationResult(ControlValidationLevel.Error, $"Condition operator '{oper}' is not supported by FetchXml.", "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/reference/");
            }
            var entityname = node.Value("entityname");
            if (!string.IsNullOrWhiteSpace(entityname) && !node.LocalEntityIsRoot())
            {
                return new ControlValidationResult(ControlValidationLevel.Error, "Cannot enter Entity for Link-Entity condition.");
            }
            if (string.IsNullOrWhiteSpace(entityname) && fxb.entities != null)
            {
                var parententity = node.LocalEntityName();
                if (fxb.GetAttribute(parententity, attribute) is AttributeMetadata metaatt)
                {
                    if (metaatt.IsValidForGrid.HasValue && metaatt.IsValidForGrid.Value == false)
                    {
                        //  return new ControlValidationResult(ControlValidationLevel.Error, $"Attribute '{attribute}' has 'IsValidForGrid=false'.");
                    }
                }
                else
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, $"Attribute '{attribute}' is not in the table '{parententity}'.");
                }
            }
            return null;
        }

        private static ControlValidationResult ValidateValue(TreeNode node, FetchXmlBuilder fxb)
        {
            if (string.IsNullOrWhiteSpace(node.Value("#text")))
            {
                return new ControlValidationResult(ControlValidationLevel.Warning, "Value should be added.");
            }
            return null;
        }

        private static ControlValidationResult ValidateOrder(TreeNode node, FetchXmlBuilder fxb)
        {
            var attribute = node.Value("attribute");
            var alias = node.Value("alias");
            if (node.IsFetchAggregate())
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Order Alias must be included in aggregate query.", "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/aggregate-data#order-by");
                }
                if (!string.IsNullOrWhiteSpace(attribute))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Order Name must NOT be included in aggregate query.", "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/aggregate-data#order-by");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(attribute))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Order Name must be included.");
                }
            }

            if (node.Parent.Name == "link-entity")
            {
                return new ControlValidationResult(ControlValidationLevel.Info, "Sorting on a link-entity triggers legacy paging.", "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/order-rows#process-link-entity-orders-first");
            }
            var parententity = node.LocalEntityName();
            if (fxb.entities != null && !string.IsNullOrWhiteSpace(attribute))
            {
                if (fxb.GetAttribute(parententity, attribute) is AttributeMetadata metaatt)
                {
                }
                else
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, $"Order Attribute '{attribute}' is not in the table '{parententity}'.");
                }
            }
            if (node.IsFetchAggregate() && !string.IsNullOrWhiteSpace(alias))
            {
                var attr = node.Parent.Nodes.OfType<TreeNode>()
                    .Where(n => n.Name == "attribute" && n.Value("alias") == alias)
                    .FirstOrDefault();

                if (attr != null &&
                    attr.Value("groupby") == "true" &&
                    fxb.GetAttribute(parententity, attr.Value("name")) is LookupAttributeMetadata)
                {
                    return new ControlValidationResult(ControlValidationLevel.Info, "Sorting on a grouped lookup column may cause paging problems.", "https://markcarrington.dev/2022/01/13/fetchxml-aggregate-queries-lookup-fields-and-paging/");
                }
            }
            return null;
        }

        private static bool HasPrimaryIdAttribute(TreeNode parent, FetchXmlBuilder fxb)
        {
            var entity = parent.Value("name");

            if (string.IsNullOrWhiteSpace(entity))
                return true;

            if (fxb == null || fxb.entities == null)
                return true;

            if (!(fxb.GetEntity(entity) is EntityMetadata metadata))
                return true;

            if (string.IsNullOrWhiteSpace(metadata.PrimaryIdAttribute))
                return true;

            var attr = parent.Nodes.OfType<TreeNode>()
                .Where(n => n.Name == "attribute" && n.Value("name") == metadata.PrimaryIdAttribute)
                .FirstOrDefault();

            if (attr != null)
                return true;

            return false;
        }

        private static bool HasSortOnAttribute(TreeNode node)
        {
            var attrName = node.Value("name");
            var attrAlias = node.Value("alias");

            foreach (var sort in node.Parent.Nodes.OfType<TreeNode>().Where(c => c.Name == "order"))
            {
                var sortAttribute = sort.Value("attribute");
                var sortAlias = sort.Value("alias");

                if (string.IsNullOrWhiteSpace(attrAlias))
                {
                    if (attrName == sortAttribute)
                    {
                        return true;
                    }
                }
                else
                {
                    if (attrAlias == sortAlias)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}