using Cinteros.Xrm.FetchXmlBuilder.Controls;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    internal static class Validations
    {
        internal static ControlValidationResult GetWarning(TreeNode node, FetchXmlBuilder fxb)
        {
            var name = TreeNodeHelper.GetAttributeFromNode(node, "name");
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
                    if (fxb.entities != null && node.Parent.Tag is Dictionary<string, string> partag && partag.ContainsKey("name"))
                    {
                        var parname = partag["name"];
                        if (fxb.GetAttribute(parname, name) is AttributeMetadata metaatt)
                        {
                            if (metaatt.IsValidForGrid.Value == false)
                            {
                                return new ControlValidationResult(ControlValidationLevel.Warning, $"Attribute '{name}' has 'IsValidForGrid=false'.");
                            }
                        }
                        else
                        {
                            return new ControlValidationResult(ControlValidationLevel.Warning, $"Attribute '{name}' is not in the table '{parname}'.");
                        }
                    }
                    var alias = TreeNodeHelper.GetAttributeFromNode(node, "alias");
                    if (TreeBuilderControl.IsFetchAggregate(node))
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
                    break;
                case "value":
                    break;
                case "order":
                    if (string.IsNullOrWhiteSpace(TreeNodeHelper.GetAttributeFromNode(node, "attribute")))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Order Name must be included.");
                    }
                    break;
            }
            return null;
        }
    }
}
