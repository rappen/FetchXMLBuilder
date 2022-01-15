using Cinteros.Xrm.FetchXmlBuilder.Controls;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    internal static class Validations
    {
        internal static ControlValidationResult GetWarning(TreeNode node, FetchXmlBuilder fxb)
        {
            if (!fxb.settings.ShowValidation)
            {
                return null;
            }
            var name = TreeNodeHelper.GetAttributeFromNode(node, "name");
            var attribute = TreeNodeHelper.GetAttributeFromNode(node, "attribute");
            var alias = TreeNodeHelper.GetAttributeFromNode(node, "alias");
            var parententity = TreeNodeHelper.ForThisNodeEntityName(node);
            switch (node.Name)
            {
                case "fetch":
                    break;
                case "entity":
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Entity Name must be included.");
                    }
                    break;
                case "link-entity":
                    if (string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(TreeNodeHelper.GetAttributeFromNode(node, "to")) ||
                        string.IsNullOrWhiteSpace(TreeNodeHelper.GetAttributeFromNode(node, "from")))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Link-Entity must include Name, To, From.");
                    }

                    if (fxb.GetAttribute(name, TreeNodeHelper.GetAttributeFromNode(node, "from")) is AttributeMetadata fromAttr && fromAttr.IsPrimaryId == false)
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Links to records that aren't parents may cause paging issues.", "https://markcarrington.dev/2021/02/23/msdyn365-internals-paging-gotchas/#multiple_linked_entities");
                    }
                    break;
                case "attribute":
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Attribute Name must be included.");
                    }
                    if (fxb.entities != null)
                    {
                        if (fxb.GetAttribute(parententity, name) is AttributeMetadata metaatt)
                        {
                            if (metaatt.IsValidForGrid.Value == false && metaatt.IsPrimaryId.Value != true)
                            {
                                return new ControlValidationResult(ControlValidationLevel.Warning, $"Attribute '{name}' has 'IsValidForGrid=false'.");
                            }
                        }
                        else
                        {
                            return new ControlValidationResult(ControlValidationLevel.Warning, $"Attribute '{name}' is not in the table '{parententity}'.");
                        }
                    }
                    if (TreeNodeHelper.IsFetchAggregate(node))
                    {
                        if (string.IsNullOrWhiteSpace(alias))
                        {
                            return new ControlValidationResult(ControlValidationLevel.Warning, "Aggregate should always have an Alias.", "https://docs.microsoft.com/en-us/powerapps/developer/data-platform/use-fetchxml-aggregation#about-aggregation");
                        }

                        if (TreeNodeHelper.GetAttributeFromNode(node, "groupby") == "true")
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

                        if (IsDistinct(node) && !HasSortOnAttribute(node))
                        {
                            return new ControlValidationResult(ControlValidationLevel.Info, "Distinct queries should be sorted by all attributes for correct paging.", "https://markcarrington.dev/2020/12/08/dataverse-paging-with-distinct/");
                        }
                    }
                    break;
                case "filter":
                    if (node.Nodes.Count == 0)
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Filter shound have at least one Condition.");
                    }
                    break;
                case "condition":
                    if (string.IsNullOrWhiteSpace(attribute))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Attribute must be included.");
                    }
                    var entityname = TreeNodeHelper.GetAttributeFromNode(node, "entityname");
                    if (!string.IsNullOrWhiteSpace(entityname) && !TreeNodeHelper.ForThisNodeEntityIsRoot(node))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Error, "Cannot enter Entity for Link-Entity condition.");
                    }
                    if (string.IsNullOrWhiteSpace(entityname) && fxb.entities != null)
                    {
                        if (fxb.GetAttribute(parententity, attribute) is AttributeMetadata metaatt)
                        {
                            if (metaatt.IsValidForGrid.Value == false)
                            {
                                //  return new ControlValidationResult(ControlValidationLevel.Error, $"Attribute '{attribute}' has 'IsValidForGrid=false'.");
                            }
                        }
                        else
                        {
                            return new ControlValidationResult(ControlValidationLevel.Warning, $"Attribute '{attribute}' is not in the table '{parententity}'.");
                        }

                    }
                    break;
                case "value":
                    if (string.IsNullOrWhiteSpace(TreeNodeHelper.GetAttributeFromNode(node, "#text")))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Value should be added.");
                    }
                    break;
                case "order":
                    if (string.IsNullOrWhiteSpace(attribute) && string.IsNullOrWhiteSpace(alias))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Order Name must be included.");
                    }

                    if (node.Parent.Name == "link-entity")
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Sorting on a link-entity triggers legacy paging.", "https://docs.microsoft.com/en-us/powerapps/developer/data-platform/org-service/paging-behaviors-and-ordering#ordering-and-multiple-table-queries");
                    }
                    if (fxb.entities != null)
                    {
                        if (fxb.GetAttribute(parententity, attribute) is AttributeMetadata metaatt)
                        {
                        }
                        else
                        {
                            return new ControlValidationResult(ControlValidationLevel.Warning, $"Order Attribute '{attribute}' is not in the table '{parententity}'.");
                        }
                    }
                    if (TreeNodeHelper.IsFetchAggregate(node) && !string.IsNullOrWhiteSpace(alias))
                    {
                        var attr = node.Parent.Nodes.OfType<TreeNode>()
                            .Where(n => n.Name == "attribute" && TreeNodeHelper.GetAttributeFromNode(n, "alias") == alias)
                            .FirstOrDefault();

                        if (attr != null &&
                            TreeNodeHelper.GetAttributeFromNode(attr, "groupby") == "true" &&
                            fxb.GetAttribute(parententity, TreeNodeHelper.GetAttributeFromNode(attr, "name")) is LookupAttributeMetadata)
                        {
                            return new ControlValidationResult(ControlValidationLevel.Info, "Sorting on a grouped lookup column may cause paging problems.", "https://markcarrington.dev/2022/01/13/fetchxml-aggregate-queries-lookup-fields-and-paging/");
                        }
                    }
                    break;
            }
            return null;
        }

        internal static bool IsDistinct(TreeNode node)
        {
            var distinct = false;
            while (node != null && node.Name != "fetch")
            {
                node = node.Parent;
            }
            if (node != null && node.Name == "fetch")
            {
                distinct = TreeNodeHelper.GetAttributeFromNode(node, "distinct") == "true";
            }
            return distinct;
        }

        private static bool HasSortOnAttribute(TreeNode node)
        {
            var attrName = TreeNodeHelper.GetAttributeFromNode(node, "name");
            var attrAlias = TreeNodeHelper.GetAttributeFromNode(node, "alias");

            foreach (var sort in node.Parent.Nodes.OfType<TreeNode>().Where(c => c.Name == "order"))
            {
                var sortAttribute = TreeNodeHelper.GetAttributeFromNode(sort, "attribute");
                var sortAlias = TreeNodeHelper.GetAttributeFromNode(sort, "alias");

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
