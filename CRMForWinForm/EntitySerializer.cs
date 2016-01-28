using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Cinteros.Xrm.CRMWinForm
{
    public class EntitySerializer
    {
        public static XmlDocument Serialize(Entity entity, XmlNode parent)
        {
            XmlDocument result;
            if (parent != null)
            {
                result = parent.OwnerDocument;
            }
            else
            {
                result = new XmlDocument();
                parent = result.CreateElement("Entities");
                result.AppendChild(parent);
            }
            XmlNode xEntity = result.CreateElement("Entity");
            XmlAttribute xEntityName = result.CreateAttribute("name");
            xEntityName.Value = entity.LogicalName;
            xEntity.Attributes.Append(xEntityName);
            XmlAttribute xEntityId = result.CreateAttribute("id");
            xEntityId.Value = entity.Id.ToString();
            xEntity.Attributes.Append(xEntityId);
            foreach (KeyValuePair<string, object> attribute in entity.Attributes)
            {
                if (attribute.Key == entity.LogicalName + "id")
                {   // Don't include PK
                    continue;
                }
                XmlNode xAttribute = result.CreateNode(XmlNodeType.Element, "Attribute", "");
                XmlAttribute xName = result.CreateAttribute("name");
                xName.Value = attribute.Key;
                xAttribute.Attributes.Append(xName);
                object value = attribute.Value;
                if (value is AliasedValue)
                {
                    if (!string.IsNullOrEmpty(((AliasedValue)value).EntityLogicalName))
                    {
                        XmlAttribute xAliasedEntity = result.CreateAttribute("entitylogicalname");
                        xAliasedEntity.Value = ((AliasedValue)value).EntityLogicalName;
                        xAttribute.Attributes.Append(xAliasedEntity);
                    }
                    if (!string.IsNullOrEmpty(((AliasedValue)value).AttributeLogicalName))
                    {
                        XmlAttribute xAliasedAttribute = result.CreateAttribute("attributelogicalname");
                        xAliasedAttribute.Value = ((AliasedValue)value).AttributeLogicalName;
                        xAttribute.Attributes.Append(xAliasedAttribute);
                    }
                    value = ((AliasedValue)value).Value;
                }
                XmlAttribute xType = result.CreateAttribute("type");
                xType.Value = LastClassName(value);
                xAttribute.Attributes.Append(xType);
                if (value is EntityReference)
                {
                    XmlAttribute xRefEntity = result.CreateAttribute("entity");
                    xRefEntity.Value = ((EntityReference)value).LogicalName;
                    xAttribute.Attributes.Append(xRefEntity);
                }
                object basetypevalue = AttributeToBaseType(value);
                if (basetypevalue != null)
                {
                    XmlText xValue = result.CreateTextNode(basetypevalue.ToString());
                    xAttribute.AppendChild(xValue);
                }
                xEntity.AppendChild(xAttribute);
            }
            parent.AppendChild(xEntity);
            return result;
        }

        public static string ToJSON(Entity entity, Formatting format, int indent)
        {
            StringBuilder sb = new StringBuilder();
            var space = format == Formatting.Indented ? " " : "";
            sb.Append(
                Sep(format, indent + 0) + "{" + space +
                Sep(format, indent + 1) + "\"entity\":" + space + "\"" + entity.LogicalName + "\"," +
                Sep(format, indent + 1) + "\"id\":" + space + "\"{" + entity.Id.ToString() + "}\"," +
                Sep(format, indent + 1) + "\"attributes\":" + space + "[");

            bool first = true;
            foreach (KeyValuePair<string, object> attribute in entity.Attributes)
            {
                Object value = attribute.Value;
                if (attribute.Key == entity.LogicalName + "id")
                {
                    continue;
                }
                if (attribute.Key.EndsWith("_base") && entity.Contains(attribute.Key.Substring(0, attribute.Key.Length - 5)))
                {
                    continue;
                }

                if (first)
                {
                    sb.Append(Sep(format, indent + 2) + "{");
                    first = false;
                }
                else
                    sb.Append("," + Sep(format, indent + 2) + "{");

                if (value is AliasedValue)
                {
                    if (!string.IsNullOrEmpty(((AliasedValue)value).AttributeLogicalName))
                    {
                        sb.Append(Sep(format, indent + 3) + "\"attributelogicalname\":" + space + "\"" + (((AliasedValue)value).AttributeLogicalName) + "\",");
                    }
                    if (!string.IsNullOrEmpty(((AliasedValue)value).EntityLogicalName))
                    {
                        sb.Append(Sep(format, indent + 3) + "\"entitylogicalname\":" + space + "\"" + (((AliasedValue)value).EntityLogicalName) + "\",");
                    }
                    value = (((AliasedValue)value).Value);
                }

                sb.Append(Sep(format, indent + 3) + "\"name\":" + space + "\"" + attribute.Key + "\",");
                sb.Append(Sep(format, indent + 3) + "\"type\":" + space + "\"" + LastClassName(value) + "\",");

                if (value is EntityReference)
                {
                    sb.Append(Sep(format, indent + 3) + "\"entity\":" + space + "\"" + ((EntityReference)value).LogicalName + "\",");
                    if (!string.IsNullOrEmpty(((EntityReference)value).Name))
                    {
                        sb.Append(Sep(format, indent + 3) + "\"namevalue\":" + space + "\"" + ((EntityReference)value).Name + "\",");
                    }
                    value = ((EntityReference)value).Id;

                }

                if (value != null)
                {
                    sb.Append(string.Format(Sep(format, indent + 3) + "\"value\":" + space + "\"{0}\"", AttributeToBaseType(value)));
                }

                sb.Append(Sep(format, indent + 2) + "}");
            }
            sb.Append(Sep(format, indent + 1) + "]");
            sb.Append(Sep(format, indent + 0) + "}");
            return sb.ToString();
        }

        private static string LastClassName(object obj)
        {
            string result = obj == null ? "null" : obj.GetType().ToString();
            result = result.Split('.')[result.Split('.').Length - 1];
            return result;
        }

        public static object AttributeToBaseType(object attribute)
        {
            if (attribute is AliasedValue)
                return AttributeToBaseType(((AliasedValue)attribute).Value);
            else if (attribute is EntityReference)
                return ((EntityReference)attribute).Id;
            else if (attribute is EntityReferenceCollection)
            {
                var referencedEntity = "";
                foreach (var er in (EntityReferenceCollection)attribute)
                {
                    if (referencedEntity == "")
                    {
                        referencedEntity = er.LogicalName;
                    }
                    else if (referencedEntity != er.LogicalName)
                    {
                        referencedEntity = "";
                        break;
                    }
                }
                var result = "";
                foreach (var er in (EntityReferenceCollection)attribute)
                {
                    if (result != "")
                    {
                        result += ",";
                    }
                    if (referencedEntity != "")
                    {
                        result += er.Id.ToString();
                    }
                    else
                    {
                        result += er.LogicalName + ":" + er.Id.ToString();
                    }
                }
                return result;
            }
            else if (attribute is EntityCollection)
            {
                var result = "";
                if (((EntityCollection)attribute).Entities.Count > 0)
                {
                    foreach (var entity in ((EntityCollection)attribute).Entities)
                    {
                        if (result != "")
                        {
                            result += ",";
                        }
                        result += entity.Id.ToString();
                    }
                    result = ((EntityCollection)attribute).EntityName + ":" + result;
                }
                return result;
            }
            else if (attribute is OptionSetValue)
                return ((OptionSetValue)attribute).Value;
            else if (attribute is Money)
                return ((Money)attribute).Value;
            else if (attribute is BooleanManagedProperty)
                return ((BooleanManagedProperty)attribute).Value;
            else
                return attribute;
        }

        public static string AttributeToString(object attribute, AttributeMetadata meta)
        {
            if (attribute == null)
                return "";
            if (attribute is AliasedValue)
                return AttributeToString(((AliasedValue)attribute).Value, meta);
            else if (attribute is EntityReference)
                if (!string.IsNullOrEmpty(((EntityReference)attribute).Name))
                    return ((EntityReference)attribute).Name;
                else
                    return ((EntityReference)attribute).Id.ToString();
            else if (attribute is EntityCollection && ((EntityCollection)attribute).EntityName == "activityparty")
            {
                var result = "";
                if (((EntityCollection)attribute).Entities.Count > 0)
                {
                    foreach (var entity in ((EntityCollection)attribute).Entities)
                    {
                        var party = "";
                        if (entity.Contains("partyid") && entity["partyid"] is EntityReference)
                        {
                            party = ((EntityReference)entity["partyid"]).Name;
                        }
                        if (string.IsNullOrEmpty(party) && entity.Contains("addressused"))
                        {
                            party = entity["addressused"].ToString();
                        }
                        if (string.IsNullOrEmpty(party))
                        {
                            party = entity.Id.ToString();
                        }
                        if (!string.IsNullOrEmpty(result))
                        {
                            result += ", ";
                        }
                        result += party;
                    }
                }
                return result;
            }
            else if (attribute is OptionSetValue)
            {
                var value = ((OptionSetValue)attribute).Value;
                if (meta != null && meta is EnumAttributeMetadata)
                {
                    foreach (var osv in ((EnumAttributeMetadata)meta).OptionSet.Options)
                    {
                        if (osv.Value == value)
                        {
                            return osv.Label.UserLocalizedLabel.Label;
                        }
                    }
                }
                return value.ToString();
            }
            else if (attribute is Money)
                return ((Money)attribute).Value.ToString();
            else if (attribute is BooleanManagedProperty)
                return ((BooleanManagedProperty)attribute).Value.ToString();
            else
                return attribute.ToString();
        }

        internal static string Sep(Formatting format, int indent)
        {
            if (format == Formatting.None)
            {
                return "";
            }
            return "\n" + new string(' ', indent * 4);
        }
    }
}
