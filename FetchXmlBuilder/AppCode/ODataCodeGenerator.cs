using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class ODataCodeGenerator
    {
        public static string GetODataQuery(FetchType fetch, string organizationServiceUrl, FetchXmlBuilder sender)
        {
            if (sender.Service == null)
            {
                throw new Exception("Must have an active connection to CRM to compose OData query.");
            }
            var url = organizationServiceUrl;
            var entity = fetch.Items.Where(i => i is FetchEntityType).FirstOrDefault() as FetchEntityType;
            if (entity == null)
            {
                throw new Exception("Fetch must contain entity definition");
            }
            url += "/" + LogicalToSchemaName(entity.name, sender) + "Set";

            var query = "";
            var select = GetSelect(entity, sender);
            var order = GetOrder(entity, sender);
            var expand = GetExpand(entity, sender);
            var filter = GetFilter(entity, sender);

            query = AppendQuery(query, select);
            query = AppendQuery(query, order);
            query = AppendQuery(query, expand);
            query = AppendQuery(query, filter);

            if (!string.IsNullOrEmpty(query))
            {
                url += "?" + query;
            }

            return url;
        }

        private static string AppendQuery(string query, string append)
        {
            if (!string.IsNullOrEmpty(append))
            {
                if (!string.IsNullOrEmpty(query))
                {
                    query += "&";
                }
                query += append;
            }
            return query;
        }

        private static string GetSelect(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var result = "";
            var attributeitems = entity.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
            if (attributeitems.Count > 0)
            {
                result += "$select=";
                foreach (FetchAttributeType attributeitem in attributeitems)
                {
                    result += LogicalToSchemaName(entity.name, attributeitem.name, sender) + ",";
                }
                result = result.Trim(',');
            }
            return result;
        }

        private static string GetExpand(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var result = "";
            var linkitems = entity.Items.Where(i => i is FetchLinkEntityType).ToList();
            if (linkitems.Count > 0)
            {
                throw new Exception("Link-entities is not yet supported by the OData generator");
            }
            return result;
        }

        private static string GetFilter(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var result = "";
            var filteritems = entity.Items.Where(i => i is filter && ((filter)i).Items != null && ((filter)i).Items.Length > 0).ToList();
            if (filteritems.Count > 0)
            {
                foreach (filter filteritem in filteritems)
                {
                    result += GetFilter(entity, filteritem, sender);
                }
                if (result.StartsWith("(") && result.EndsWith(")"))
                {
                    result = result.Substring(1, result.Length - 2);
                }
                result = "$filter=" + result;
            }
            return result;
        }

        private static string GetFilter(FetchEntityType entity, filter filteritem, FetchXmlBuilder sender)
        {
            var result = "";
            if (filteritem.Items == null || filteritem.Items.Length == 0)
            {
                return "";
            }
            var logical = filteritem.type == filterType.or ? " or " : " and ";
            if (filteritem.Items.Length > 1)
            {
                result = "(";
            }
            foreach (var item in filteritem.Items)
            {
                if (item is condition)
                {
                    result += GetCondition(entity, item as condition, sender);
                }
                else if (item is filter)
                {
                    result += GetFilter(entity, item as filter, sender);
                }
                result += logical;
            }
            if (result.EndsWith(logical))
            {
                result = result.Substring(0, result.Length - logical.Length);
            }
            if (filteritem.Items.Length > 1)
            {
                result += ")";
            }
            return result;
        }

        private static string GetCondition(FetchEntityType entity, condition condition, FetchXmlBuilder sender)
        {
            var result = "";
            if (!string.IsNullOrEmpty(condition.attribute))
            {
                GetEntityMetadata(entity.name, sender);
                var attrMeta = FetchXmlBuilder.GetAttribute(entity.name, condition.attribute);
                if (attrMeta == null)
                {
                    throw new Exception(string.Format("No metadata for attribute: {0}.{1}", entity.name, condition.attribute));
                }
                result = attrMeta.SchemaName;
                switch (attrMeta.AttributeType)
                {
                    case AttributeTypeCode.Picklist:
                    case AttributeTypeCode.Money:
                        result += "/Value";
                        break;
                    case AttributeTypeCode.Lookup:
                        result += "/Id";
                        break;
                }
                switch (condition.@operator)
                {
                    case @operator.eq:
                        result += " eq ";
                        break;
                    case @operator.ne:
                    case @operator.neq:
                        result += " ne ";
                        break;
                    //case @operator.like:
                    //    result = string.Format(substringof('{condition.value}', {attrMeta.SchemaName})";
                    //    break;
                    case @operator.@null:
                        result += " eq null";
                        break;
                    case @operator.notnull:
                        result += " ne null";
                        break;
                    case @operator.@in:
                    case @operator.notin:
                    case @operator.lt:
                    case @operator.le:
                    case @operator.gt:
                    case @operator.ge:
                        throw new Exception(string.Format("Condition operator '{0}' is not yet supported by the OData generator", condition.@operator));
                    default:
                        throw new Exception(string.Format("Unsupported OData condition operator '{0}'", condition.@operator));
                }
                if (!string.IsNullOrEmpty(condition.value))
                {
                    switch (attrMeta.AttributeType)
                    {
                        case AttributeTypeCode.Picklist:
                        case AttributeTypeCode.Money:
                            result += condition.value;
                            break;
                        case AttributeTypeCode.Lookup:
                            result += string.Format("(guid'{0}')", condition.value);
                            break;
                        default:
                            result += string.Format("'{0}'", condition.value);
                            break;
                    }
                }
            }
            return result;
        }

        private static string GetOrder(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var result = "";
            var orderitems = entity.Items.Where(i => i is FetchOrderType && ((FetchOrderType)i).attribute != null).ToList();
            if (orderitems.Count > 0)
            {
                result += "$orderby=";
                foreach (FetchOrderType orderitem in orderitems)
                {
                    result += LogicalToSchemaName(entity.name, orderitem.attribute, sender) + ",";
                }
                result = result.Trim(',');
            }
            return result;
        }

        private static string LogicalToSchemaName(string entity, FetchXmlBuilder sender)
        {
            GetEntityMetadata(entity, sender);
            var entityMeta = FetchXmlBuilder.entities[entity];
            return entityMeta.SchemaName;
        }

        private static string LogicalToSchemaName(string entity, string attribute, FetchXmlBuilder sender)
        {
            GetEntityMetadata(entity, sender);
            var attrMeta = FetchXmlBuilder.GetAttribute(entity, attribute);
            if (attrMeta == null)
            {
                throw new Exception(string.Format("No metadata for attribute: {0}.{1}", entity, attribute));
            }
            return attrMeta.SchemaName;
        }

        private static void GetEntityMetadata(string entity, FetchXmlBuilder sender)
        {
            if (sender.NeedToLoadEntity(entity))
            {
                sender.LoadEntityDetails(entity, null, false);
            }
            if (!FetchXmlBuilder.entities.ContainsKey(entity))
            {
                throw new Exception(string.Format("No metadata for entity: {0}", entity));
            }
        }
    }
}
