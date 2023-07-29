using MarkMpn.FetchXmlToWebAPI;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    public class ODataCodeGenerator
    {
        public static string ConvertToOData4(string fetchxml, FetchXmlBuilder fxb)
        {
            // Find correct WebAPI base url
            var baseUrl = fxb.ConnectionDetail.WebApplicationUrl;
            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";
            var url = new Uri(new Uri(baseUrl), $"api/data/v{fxb.ConnectionDetail.OrganizationMajorVersion}.{fxb.ConnectionDetail.OrganizationMinorVersion}");
            var converter = new FetchXmlToWebAPIConverter(new WebAPIMetadataProvider(fxb), url.ToString());
            var odata4 = converter.ConvertFetchXmlToWebAPI(fetchxml);
            return odata4;
        }

        public static string ConvertToOData2(FetchType fetch, FetchXmlBuilder sender)
        {
            if (sender.Service == null)
            {
                throw new Exception("Must have an active connection to CRM to compose OData query.");
            }
            var organizationServiceUrl = sender.ConnectionDetail.OrganizationDataServiceUrl;
            var url = organizationServiceUrl;
            var entity = fetch.Items.Where(i => i is FetchEntityType).FirstOrDefault() as FetchEntityType;
            if (entity == null)
            {
                throw new Exception("Fetch must contain entity definition");
            }
            url += "/" + LogicalToSchemaName(entity.name, sender) + "Set";

            var query = "";
            if (!string.IsNullOrEmpty(fetch.top))
            {
                query = AppendQuery(query, "$top", fetch.top);
            }
            if (entity.Items != null)
            {
                var select = GetSelect(entity, sender);
                var order = GetOrder(entity, sender);
                var expand = GetExpand(entity, sender, ref select);
                var filter = GetFilter(entity, sender);
                query = AppendQuery(query, "$select", select);
                query = AppendQuery(query, "$orderby", order);
                query = AppendQuery(query, "$expand", expand);
                query = AppendQuery(query, "$filter", filter);
            }

            if (!string.IsNullOrEmpty(query))
            {
                url += "?" + query;
            }

            return url;
        }

        private static string AppendQuery(string query, string paramname, string append)
        {
            var result = new StringBuilder(query);
            if (!string.IsNullOrEmpty(append))
            {
                if (!string.IsNullOrEmpty(query))
                {
                    result.Append("&");
                }
                result.Append(paramname + "=" + append);
            }
            return result.ToString().Trim(',');
        }

        private static string GetSelect(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var result = new List<string>();
            var attributeitems = entity.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
            if (attributeitems.Count > 0)
            {
                foreach (FetchAttributeType attributeitem in attributeitems)
                {
                    result.Add(LogicalToSchemaName(entity.name, attributeitem.name, sender));
                }
            }
            return string.Join(",", result);
        }

        private static string GetExpand(FetchEntityType entity, FetchXmlBuilder sender, ref string select)
        {
            var resultList = new List<string>();
            var selectList = new List<string> { select };
            var linkitems = entity.Items.Where(i => i is FetchLinkEntityType).ToList();
            if (linkitems.Count > 0)
            {
                foreach (FetchLinkEntityType linkitem in linkitems)
                {
                    if (linkitem.linktype == "outer")
                    {
                        throw new Exception("OData queries do not support outer joins");
                    }
                    if (linkitem.Items != null)
                    {
                        if (!linkitem.intersect && linkitem.Items.Where(i => i is FetchLinkEntityType).ToList().Count > 0)
                        {
                            throw new Exception("OData queries only support one level of link entities");
                        }
                        if (linkitem.Items.Where(i => i is filter).ToList().Count > 0)
                        {
                            throw new Exception("OData queries do not support filter on link entities");
                        }
                        if (linkitem.Items.Where(i => i is FetchOrderType).ToList().Count > 0)
                        {
                            throw new Exception("OData queries do not support sorting on link entities");
                        }
                    }
                    var relation = LinkItemToRelation(entity.name, linkitem, sender);
                    resultList.Add(relation.SchemaName);
                    selectList.Add(GetExpandedSelect(linkitem, relation.SchemaName, sender));
                }
            }
            select = string.Join(",", selectList);
            return string.Join(",", resultList);
        }

        private static string GetExpandedSelect(FetchLinkEntityType linkitem, string relation, FetchXmlBuilder sender)
        {
            if (linkitem.Items == null)
            {
                return "";
            }
            var resultList = new List<string>();
            var linkentity = linkitem.name;
            var attributeitems = linkitem.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
            if (linkitem.intersect)
            {
                var linkitems = linkitem.Items.Where(i => i is FetchLinkEntityType).ToList();
                if (linkitems.Count > 1)
                {
                    throw new Exception("Invalid M:M-relation definition for OData");
                }
                if (linkitems.Count == 1)
                {
                    var nextlink = (FetchLinkEntityType)linkitems[0];
                    linkentity = nextlink.name;
                    attributeitems = nextlink.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
                }
            }
            if (attributeitems.Count > 0)
            {
                foreach (FetchAttributeType attributeitem in attributeitems)
                {
                    resultList.Add(relation + "/" + LogicalToSchemaName(linkentity, attributeitem.name, sender));
                }
            }
            return string.Join(",", resultList);
        }

        private static string GetFilter(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var resultList = new StringBuilder();
            var filteritems = entity.Items.Where(i => i is filter && ((filter)i).Items != null && ((filter)i).Items.Length > 0).ToList();
            if (filteritems.Count > 0)
            {
                foreach (filter filteritem in filteritems)
                {
                    resultList.Append(GetFilter(entity, filteritem, sender));
                }
                var result = resultList.ToString();
                if (result.StartsWith("(") && result.EndsWith(")"))
                {
                    result = result.Substring(1, result.Length - 2);
                }
                return result;
            }
            return "";
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
                var attrMeta = sender.GetAttribute(entity.name, condition.attribute);
                if (attrMeta == null)
                {
                    throw new Exception($"No metadata for attribute: {entity.name}.{condition.attribute}");
                }
                result = attrMeta.SchemaName;
                switch (attrMeta.AttributeType)
                {
                    case AttributeTypeCode.Picklist:
                    case AttributeTypeCode.Money:
                    case AttributeTypeCode.State:
                    case AttributeTypeCode.Status:
                        result += "/Value";
                        break;

                    case AttributeTypeCode.Lookup:
                        result += "/Id";
                        break;
                }
                switch (condition.@operator)
                {
                    case @operator.eq:
                    case @operator.ne:
                    case @operator.lt:
                    case @operator.le:
                    case @operator.gt:
                    case @operator.ge:
                        result += $" {condition.@operator} ";
                        break;

                    case @operator.neq:
                        result += " ne ";
                        break;

                    case @operator.@null:
                        result += " eq null";
                        break;

                    case @operator.notnull:
                        result += " ne null";
                        break;

                    case @operator.like:
                        result = $"substringof('{condition.value}', {attrMeta.SchemaName})";
                        break;

                    case @operator.notlike:
                        result = $"not substringof('{condition.value}', {attrMeta.SchemaName})";
                        break;

                    case @operator.@in:
                    case @operator.notin:
                        throw new Exception($"Condition operator '{condition.@operator}' is not yet supported by the OData generator");
                    default:
                        throw new Exception($"Unsupported OData condition operator '{condition.@operator}'");
                }
                if (!string.IsNullOrEmpty(condition.value) && condition.@operator != @operator.like && condition.@operator != @operator.notlike)
                {
                    switch (attrMeta.AttributeType)
                    {
                        case AttributeTypeCode.Money:
                        case AttributeTypeCode.BigInt:
                        case AttributeTypeCode.Boolean:
                        case AttributeTypeCode.Decimal:
                        case AttributeTypeCode.Double:
                        case AttributeTypeCode.Integer:
                        case AttributeTypeCode.State:
                        case AttributeTypeCode.Status:
                        case AttributeTypeCode.Picklist:
                            result += condition.value;
                            break;

                        case AttributeTypeCode.Uniqueidentifier:
                        case AttributeTypeCode.Lookup:
                        case AttributeTypeCode.Customer:
                        case AttributeTypeCode.Owner:
                            result += $"(guid'{condition.value}')";
                            break;

                        case AttributeTypeCode.DateTime:
                            var date = DateTime.Parse(condition.value);
                            var datestr = string.Empty;
                            if (date.Equals(date.Date))
                            {
                                datestr = date.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                datestr = date.ToString("o");
                            }
                            result += $"datetime'{datestr}'";
                            break;

                        default:
                            result += $"'{condition.value}'";
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
                foreach (FetchOrderType orderitem in orderitems)
                {
                    result += LogicalToSchemaName(entity.name, orderitem.attribute, sender);
                    if (orderitem.descending)
                    {
                        result += " desc";
                    }
                    result += ",";
                }
            }
            return result;
        }

        private static string LogicalToSchemaName(string entity, FetchXmlBuilder sender)
        {
            GetEntityMetadata(entity, sender);
            var entityMeta = sender.GetEntity(entity);
            return entityMeta.SchemaName;
        }

        private static string LogicalToSchemaName(string entity, string attribute, FetchXmlBuilder sender)
        {
            GetEntityMetadata(entity, sender);
            var attrMeta = sender.GetAttribute(entity, attribute);
            if (attrMeta == null)
            {
                throw new Exception($"No metadata for attribute: {entity}.{attribute}");
            }
            return attrMeta.SchemaName;
        }

        private static void GetEntityMetadata(string entity, FetchXmlBuilder sender)
        {
            if (sender.NeedToLoadEntity(entity))
            {
                sender.LoadEntityDetails(entity, null, false);
            }
            if (sender.GetEntity(entity) == null)
            {
                throw new Exception($"No metadata for entity: {entity}");
            }
        }

        private static RelationshipMetadataBase LinkItemToRelation(string entityname, FetchLinkEntityType linkitem, FetchXmlBuilder sender)
        {
            GetEntityMetadata(entityname, sender);
            var entity = sender.GetEntity(entityname);
            foreach (var relation in entity.OneToManyRelationships)
            {
                if (relation.ReferencedEntity == entityname &&
                    relation.ReferencedAttribute == linkitem.to &&
                    relation.ReferencingEntity == linkitem.name &&
                    relation.ReferencingAttribute == linkitem.from)
                {
                    return relation;
                }
            }
            foreach (var relation in entity.ManyToOneRelationships)
            {
                if (relation.ReferencingEntity == entityname &&
                    relation.ReferencingAttribute == linkitem.to &&
                    relation.ReferencedEntity == linkitem.name &&
                    relation.ReferencedAttribute == linkitem.from)
                {
                    return relation;
                }
            }
            foreach (var relation in entity.ManyToManyRelationships)
            {
                if (relation.Entity1LogicalName == entityname &&
                    relation.Entity1IntersectAttribute == linkitem.from)
                {
                    var linkitems = linkitem.Items.Where(i => i is FetchLinkEntityType).ToList();
                    if (linkitems.Count > 1)
                    {
                        throw new Exception("Invalid M:M-relation definition for OData");
                    }
                    if (linkitems.Count == 1)
                    {
                        var nextlink = (FetchLinkEntityType)linkitems[0];
                        if (nextlink.linktype == "outer")
                        {
                            throw new Exception("OData queries do not support outer joins");
                        }
                        if (relation.Entity2LogicalName == nextlink.name &&
                            relation.Entity2IntersectAttribute == nextlink.to)
                        {
                            return relation;
                        }
                    }
                }
                if (relation.Entity2LogicalName == entityname &&
                    relation.Entity2IntersectAttribute == linkitem.from)
                {
                    var linkitems = linkitem.Items.Where(i => i is FetchLinkEntityType).ToList();
                    if (linkitems.Count > 1)
                    {
                        throw new Exception("Invalid M:M-relation definition for OData");
                    }
                    if (linkitems.Count == 1)
                    {
                        var nextlink = (FetchLinkEntityType)linkitems[0];
                        if (nextlink.linktype == "outer")
                        {
                            throw new Exception("OData queries do not support outer joins");
                        }
                        if (relation.Entity1LogicalName == nextlink.name &&
                            relation.Entity1IntersectAttribute == nextlink.to)
                        {
                            return relation;
                        }
                    }
                }
            }
            throw new Exception($"Cannot find metadata for relation {entityname}.{linkitem.to} => {linkitem.name}.{linkitem.from}");
        }
    }
}