using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class OData4CodeGenerator
    {
        public static string GetOData4Query(FetchType fetch, string organizationServiceUrl, FetchXmlBuilder sender)
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
            url += "/" + LogicalToCollectionName(entity.name, sender);

            var query = "";
            if (!string.IsNullOrEmpty(fetch.top))
            {
                query = AppendQuery(query, "$top", fetch.top);
            }
            if (entity.Items != null)
            {
                if (fetch.aggregate)
                {
                    var apply = GetApply(entity, sender);
                    
                    query = AppendQuery(query, "$apply", apply);
                }
                else
                {
                    var select = GetSelect(entity, sender);
                    var order = GetOrder(entity, sender);
                    var expandFilter = "";
                    var expand = GetExpand(entity, sender, ref expandFilter);
                    var filter = GetFilter(entity, sender, expandFilter);

                    query = AppendQuery(query, "$select", select);
                    query = AppendQuery(query, "$orderby", order);
                    query = AppendQuery(query, "$expand", expand);
                    query = AppendQuery(query, "$filter", filter);
                }
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
                    result.Add(attributeitem.name);
                }
            }
            return string.Join(",", result);
        }

        private static string GetExpand(FetchEntityType entity, FetchXmlBuilder sender, ref string filterString)
        {
            var resultList = new List<string>();
            var linkitems = entity.Items.Where(i => i is FetchLinkEntityType).ToList();
            if (linkitems.Count > 0)
            {
                foreach (FetchLinkEntityType linkitem in linkitems)
                {
                    var navigationProperty = LinkItemToNavigationProperty(entity.name, linkitem, sender);

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
                            foreach (var filter in linkitem.Items.OfType<filter>())
                            {
                                foreach (var condition in filter.Items.OfType<condition>())
                                {
                                    var targetLogicalName = linkitem.name;
                                    GetEntityMetadata(targetLogicalName, sender);
                                    if (condition.attribute == sender.entities[targetLogicalName].PrimaryIdAttribute &&
                                        condition.@operator == @operator.eq)
                                    {
                                        if (!String.IsNullOrEmpty(filterString))
                                            filterString += " and ";

                                        filterString += $"{navigationProperty}/{condition.attribute} eq {condition.value}";
                                    }
                                    else
                                    {
                                        throw new Exception("OData queries do not support filter on link entities except by primary key");
                                    }
                                }
                            }
                        }
                        if (linkitem.Items.Where(i => i is FetchOrderType).ToList().Count > 0)
                        {
                            throw new Exception("OData queries do not support sorting on link entities");
                        }
                    }

                    var expandedSelect = GetExpandedSelect(linkitem, sender);

                    if (String.IsNullOrEmpty(expandedSelect))
                        resultList.Add(navigationProperty);
                    else
                        resultList.Add(navigationProperty + "($select=" + GetExpandedSelect(linkitem, sender) + ")");
                }
            }
            return string.Join(",", resultList);
        }

        private static string GetExpandedSelect(FetchLinkEntityType linkitem, FetchXmlBuilder sender)
        {
            if (linkitem.Items == null)
            {
                return "";
            }
            var resultList = new List<string>();
            var linkentity = linkitem.name;

            if (linkentity == null)
                return null;

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

                    if (nextlink.Items == null)
                        return null;

                    attributeitems = nextlink.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
                }
            }
            if (attributeitems.Count > 0)
            {
                foreach (FetchAttributeType attributeitem in attributeitems)
                {
                    resultList.Add(attributeitem.name);
                }
            }
            return string.Join(",", resultList);
        }

        private static string GetFilter(FetchEntityType entity, FetchXmlBuilder sender, string expandFilter)
        {
            var resultList = new StringBuilder();
            var filteritems = entity.Items.Where(i => i is filter && ((filter)i).Items != null && ((filter)i).Items.Length > 0).ToList();
            if (filteritems.Count > 0)
            {
                var and = true;
                foreach (filter filteritem in filteritems)
                {
                    resultList.Append(GetFilter(entity, filteritem, sender));
                    if (filteritem.type == filterType.or)
                        and = false;
                }
                var result = resultList.ToString();
                if (result.StartsWith("(") && result.EndsWith(")"))
                {
                    result = result.Substring(1, result.Length - 2);
                }
                if (!String.IsNullOrEmpty(expandFilter))
                {
                    if (!and)
                        result = "(" + result + ")";

                    result += " and " + expandFilter;
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
                result = attrMeta.LogicalName;
                if (attrMeta is LookupAttributeMetadata lookupAttrMeta)
                {
                    if (lookupAttrMeta.Targets.Length > 1)
                        throw new Exception($"Multiple targets for lookup attribute: {entity.name}.{condition.attribute}. Use a link entity and filter on the primary key attribute instead");

                    GetEntityMetadata(lookupAttrMeta.Targets[0], sender);
                    result += "/" + sender.entities[lookupAttrMeta.Targets[0]].PrimaryIdAttribute;
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
                        result = $"contains({attrMeta.LogicalName}, '{condition.value}')";
                        break;
                    case @operator.notlike:
                        result = $"not contains({attrMeta.LogicalName}, '{condition.value}')";
                        break;
                    case @operator.beginswith:
                        result = $"startswith({attrMeta.LogicalName}, '{condition.value}')";
                        break;
                    case @operator.endswith:
                        result = $"endswith({attrMeta.LogicalName}, '{condition.value}')";
                        break;
                    case @operator.@in:
                    case @operator.notin:
                        throw new Exception($"Condition operator '{condition.@operator}' is not yet supported by the OData generator");
                    default:
                        throw new Exception($"Unsupported OData condition operator '{condition.@operator}'");
                }
                if (!string.IsNullOrEmpty(condition.value) && condition.@operator != @operator.like && condition.@operator != @operator.notlike &&
                    condition.@operator != @operator.beginswith && condition.@operator != @operator.endswith)
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
                        case AttributeTypeCode.Uniqueidentifier:
                        case AttributeTypeCode.Lookup:
                        case AttributeTypeCode.Customer:
                        case AttributeTypeCode.Owner:
                            result += condition.value;
                            break;
                        case AttributeTypeCode.DateTime:
                            var date = DateTimeOffset.Parse(condition.value);
                            var datestr = string.Empty;
                            if (date.Equals(date.Date))
                            {
                                datestr = date.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                datestr = date.ToString("u").Replace(' ', 'T');
                            }
                            result += datestr;
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
                    result += orderitem.attribute;
                    if (orderitem.descending)
                    {
                        result += " desc";
                    }
                    result += ",";
                }
            }
            return result;
        }

        private static string GetApply(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var groups = entity.Items.OfType<FetchAttributeType>()
                .Where(a => a.groupbySpecified)
                .Select(a => a.name)
                .ToList();
            
            var aggregates = entity.Items.OfType<FetchAttributeType>()
                .Where(a => a.aggregateSpecified)
                .Select(a => $"aggregate({a.name} with {GetAggregateType(a.aggregate)} as {a.alias})")
                .ToList();

            if (groups.Count > 0)
            {
                var result = "groupby((" + String.Join(",", groups) + ")";

                if (aggregates.Count > 0)
                    result += "," + String.Join(",", aggregates);

                result += ")";
                return result;
            }

            return String.Join(",", aggregates);
        }

        private static string GetAggregateType(AggregateType aggregate)
        {
            switch (aggregate)
            {
                case AggregateType.avg:
                    return "average";

                case AggregateType.count:
                    throw new ApplicationException("\"count\" aggregate is not supported by OData4. Please use \"countcolumn\" instead");

                case AggregateType.countcolumn:
                    return "countdistinct";

                case AggregateType.max:
                case AggregateType.min:
                case AggregateType.sum:
                    return aggregate.ToString();
            }

            throw new ApplicationException("Unknown aggregate type " + aggregate);
        }

        private static string LogicalToCollectionName(string entity, FetchXmlBuilder sender)
        {
            GetEntityMetadata(entity, sender);
            var entityMeta = sender.entities[entity];
            return entityMeta.LogicalCollectionName;
        }

        private static void GetEntityMetadata(string entity, FetchXmlBuilder sender)
        {
            if (sender.NeedToLoadEntity(entity))
            {
                sender.LoadEntityDetails(entity, null, false);
            }
            if (!sender.entities.ContainsKey(entity))
            {
                throw new Exception($"No metadata for entity: {entity}");
            }
        }

        private static string LinkItemToNavigationProperty(string entityname, FetchLinkEntityType linkitem, FetchXmlBuilder sender)
        {
            GetEntityMetadata(entityname, sender);
            var entity = sender.entities[entityname];
            foreach (var relation in entity.OneToManyRelationships)
            {
                if (relation.ReferencedEntity == entityname &&
                    relation.ReferencedAttribute == linkitem.to &&
                    relation.ReferencingEntity == linkitem.name &&
                    relation.ReferencingAttribute == linkitem.from)
                {
                    return relation.ReferencedEntityNavigationPropertyName;
                }
            }
            foreach (var relation in entity.ManyToOneRelationships)
            {
                if (relation.ReferencingEntity == entityname &&
                    relation.ReferencingAttribute == linkitem.to &&
                    relation.ReferencedEntity == linkitem.name &&
                    relation.ReferencedAttribute == linkitem.from)
                {
                    return relation.ReferencingEntityNavigationPropertyName;
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
                            return relation.Entity1NavigationPropertyName;
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
                            relation.Entity1IntersectAttribute == nextlink.from)
                        {
                            return relation.Entity2NavigationPropertyName;
                        }
                    }
                }
            }
            throw new Exception($"Cannot find metadata for relation {entityname}.{linkitem.to} => {linkitem.name}.{linkitem.from}");
        }
    }
}