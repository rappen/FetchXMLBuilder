using Cinteros.Xrm.FetchXmlBuilder.Controls;
using Microsoft.Xrm.Sdk.Metadata;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    internal static class Validations
    {
        internal static ControlValidationResult GetWarning(TreeNode node, FetchXmlBuilder fxb)
        {
            var name = TreeNodeHelper.GetAttributeFromNode(node, "name");
            var attribute = TreeNodeHelper.GetAttributeFromNode(node, "attribute");
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
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Link-Entity must be included on Name, To, From.");
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
                    var alias = TreeNodeHelper.GetAttributeFromNode(node, "alias");
                    if (TreeNodeHelper.IsFetchAggregate(node))
                    {
                        if (string.IsNullOrWhiteSpace(alias))
                        {
                            return new ControlValidationResult(ControlValidationLevel.Warning, "Aggregate should always have an Alias.");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(alias))
                        {
                            return new ControlValidationResult(ControlValidationLevel.Info, "Alias is not recommending for not Aggregate queries.");
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
                    if (string.IsNullOrWhiteSpace(attribute))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Order Name must be included.");
                    }
                    break;
            }
            return null;
        }
    }
}
