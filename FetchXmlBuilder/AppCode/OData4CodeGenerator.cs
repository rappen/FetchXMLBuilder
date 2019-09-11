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
                    var aggregate = GetAggregate(entity, sender);
                    var filter = GetFilter(entity, sender, null);

                    var apply = aggregate;
                    if (!String.IsNullOrEmpty(filter))
                        apply = $"filter({filter})/{aggregate}";

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
            return result.ToString();
        }

        private static string GetSelect(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var attributeitems = entity.Items
                .OfType<FetchAttributeType>()
                .Where(i => i.name != null);

            var result = GetAttributeNames(entity.name, attributeitems, sender);
            return string.Join(",", result);
        }

        private static IEnumerable<string> GetAttributeNames(string entityName, IEnumerable<FetchAttributeType> attributeitems, FetchXmlBuilder sender)
        {
            GetEntityMetadata(entityName, sender);
            var entityMeta = sender.entities[entityName];

            foreach (FetchAttributeType attributeitem in attributeitems)
            {
                if (!String.IsNullOrEmpty(attributeitem.alias))
                    throw new ApplicationException($"OData queries do not support aliasing columns except for aggregate queries");

                var attrMeta = entityMeta.Attributes.SingleOrDefault(a => a.LogicalName == attributeitem.name);

                if (attrMeta == null)
                    throw new ApplicationException($"Unknown attribute {entityName}.{attributeitem.name}");

                yield return GetPropertyName(attrMeta);
            }
        }

        private static string GetExpand(FetchEntityType entity, FetchXmlBuilder sender, ref string filterString)
        {
            var resultList = new List<string>();
            var linkitems = entity.Items.Where(i => i is FetchLinkEntityType).ToList();
            if (linkitems.Count > 0)
            {
                foreach (FetchLinkEntityType linkitem in linkitems)
                {
                    var navigationProperty = LinkItemToNavigationProperty(entity.name, linkitem, sender, out var child);

                    if (linkitem.Items != null)
                    {
                        if (!linkitem.intersect && linkitem.Items.Where(i => i is FetchLinkEntityType).ToList().Count > 0)
                        {
                            throw new Exception("OData queries only support one level of link entities");
                        }

                        if (!child)
                        {
                            if (linkitem.Items.Where(i => i is filter).ToList().Count > 0)
                            {
                                foreach (var filter in linkitem.Items.OfType<filter>())
                                {
                                    foreach (var condition in filter.Items.OfType<condition>())
                                    {
                                        var targetLogicalName = linkitem.name;
                                        GetEntityMetadata(targetLogicalName, sender);
                                        if (condition.attribute == sender.entities[targetLogicalName].PrimaryIdAttribute)
                                        {
                                            if (!String.IsNullOrEmpty(filterString))
                                                filterString += $" {filter.type} ";

                                            filterString += navigationProperty + "/" + GetCondition(linkitem.name, condition, sender);
                                        }
                                        else
                                        {
                                            throw new Exception($"OData queries do not support filter on link entities except by primary key. Filter on {linkitem.name}.{condition.attribute} is not allowed");
                                        }
                                    }
                                }
                            }
                            if (linkitem.Items.Where(i => i is FetchOrderType).ToList().Count > 0)
                            {
                                throw new Exception("OData queries do not support sorting on parent link entities");
                            }
                        }
                    }

                    var expandedSelect = GetExpandedSelect(linkitem, sender);
                    var childFilter = child ? linkitem.Items?.OfType<filter>().FirstOrDefault() : null;
                    var expandedFilter = childFilter == null ? null : GetFilter(linkitem.name, childFilter, sender);
                    var childOrders = child ? linkitem.Items?.OfType<FetchOrderType>().ToList() : null;
                    var expandedOrder = childOrders == null ? null : GetOrder(linkitem.name, sender, childOrders);

                    if (String.IsNullOrEmpty(expandedSelect) && String.IsNullOrEmpty(expandedFilter) && String.IsNullOrEmpty(expandedOrder))
                    {
                        resultList.Add(navigationProperty);
                    }
                    else
                    {
                        var options = new List<string>();

                        if (!String.IsNullOrEmpty(expandedSelect))
                            options.Add("$select=" + expandedSelect);

                        if (!String.IsNullOrEmpty(expandedFilter))
                            options.Add("$filter=" + expandedFilter);

                        if (!String.IsNullOrEmpty(expandedOrder))
                            options.Add("$orderby=" + expandedOrder);

                        resultList.Add(navigationProperty + "(" + String.Join(";", options) + ")");
                    }
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

            var linkentity = linkitem.name;
            if (linkentity == null)
                return null;

            var attributeitems = linkitem.Items
                .OfType<FetchAttributeType>()
                .Where(i => i.name != null);

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

                    attributeitems = nextlink.Items
                        .OfType<FetchAttributeType>()
                        .Where(i => i.name != null);
                }
            }

            var resultList = GetAttributeNames(linkentity, attributeitems, sender);
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
                    resultList.Append(GetFilter(entity.name, filteritem, sender));
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
            return expandFilter;
        }

        private static string GetFilter(string entity, filter filteritem, FetchXmlBuilder sender)
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

        private static string GetCondition(string entityName, condition condition, FetchXmlBuilder sender)
        {
            var result = "";
            if (!string.IsNullOrEmpty(condition.attribute))
            {
                if (!String.IsNullOrEmpty(condition.entityname))
                    throw new ApplicationException($"OData queries do not support filtering on link entities. If filtering on the primary key of an N:1 related entity, please add the filter to the link entity itself");

                GetEntityMetadata(entityName, sender);
                var attrMeta = sender.GetAttribute(entityName, condition.attribute);
                if (attrMeta == null)
                {
                    throw new Exception($"No metadata for attribute: {entityName}.{condition.attribute}");
                }
                result = GetPropertyName(attrMeta);
                string function = null;
                var functionParameters = 1;
                var functionParameterType = typeof(string);

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
                    case @operator.above:
                        function = "Above";
                        break;
                    case @operator.eqorabove:
                        function = "AboveOrEqual";
                        break;
                    case @operator.between:
                        function = "Between";
                        functionParameters = Int32.MaxValue;
                        break;
                    case @operator.containvalues:
                        function = "ContainsValues";
                        functionParameters = Int32.MaxValue;
                        break;
                    case @operator.notcontainvalues:
                        function = "DoesNotContainValues";
                        functionParameters = Int32.MaxValue;
                        break;
                    case @operator.eqbusinessid:
                        function = "EqualBusinessId";
                        functionParameters = 0;
                        break;
                    case @operator.equserid:
                        function = "EqualUserId";
                        functionParameters = 0;
                        break;
                    case @operator.equserlanguage:
                        function = "EqualUserLanguage";
                        functionParameters = 0;
                        break;
                    case @operator.equseroruserhierarchy:
                        function = "EqualUserOrUserHierarchy";
                        functionParameters = 0;
                        break;
                    case @operator.equseroruserhierarchyandteams:
                        function = "EqualUserOrUserHierarchyAndTeams";
                        functionParameters = 0;
                        break;
                    case @operator.equseroruserteams:
                        function = "EqualUserOrUserTeams";
                        functionParameters = 0;
                        break;
                    case @operator.equserteams:
                        function = "EqualUserTeams";
                        functionParameters = 0;
                        break;
                    case @operator.@in:
                        function = "In";
                        functionParameters = Int32.MaxValue;
                        break;
                    case @operator.infiscalperiod:
                        function = "InFiscalPeriod";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.infiscalperiodandyear:
                        function = "InFiscalPeriodAndYear";
                        functionParameters = 2;
                        functionParameterType = typeof(long);
                        break;
                    case @operator.infiscalyear:
                        function = "InFiscalYear";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.inorafterfiscalperiodandyear:
                        function = "InOrAfterFiscalPeriodAndYear";
                        functionParameters = 2;
                        functionParameterType = typeof(long);
                        break;
                    case @operator.inorbeforefiscalperiodandyear:
                        function = "InOrBeforeFiscalPeriodAndYear";
                        functionParameters = 2;
                        functionParameterType = typeof(long);
                        break;
                    case @operator.lastsevendays:
                        function = "Last7Days";
                        functionParameters = 0;
                        break;
                    case @operator.lastfiscalperiod:
                        function = "LastFiscalPeriod";
                        functionParameters = 0;
                        break;
                    case @operator.lastfiscalyear:
                        function = "LastFiscalYear";
                        functionParameters = 0;
                        break;
                    case @operator.lastmonth:
                        function = "LastMonth";
                        functionParameters = 0;
                        break;
                    case @operator.lastweek:
                        function = "LastWeek";
                        functionParameters = 0;
                        break;
                    case @operator.lastxdays:
                        function = "LastXDays";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.lastxfiscalperiods:
                        function = "LastXFiscalPeriods";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.lastxfiscalyears:
                        function = "LastXFiscalYears";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.lastxhours:
                        function = "LastXHours";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.lastxmonths:
                        function = "LastXMonths";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.lastxweeks:
                        function = "LastXWeeks";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.lastxyears:
                        function = "LastXYears";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.lastyear:
                        function = "LastYear";
                        functionParameters = 0;
                        break;
                    case @operator.nextsevendays:
                        function = "Next7Days";
                        functionParameters = 0;
                        break;
                    case @operator.nextfiscalperiod:
                        function = "NextFiscalPeriod";
                        functionParameters = 0;
                        break;
                    case @operator.nextfiscalyear:
                        function = "NextFiscalYear";
                        functionParameters = 0;
                        break;
                    case @operator.nextmonth:
                        function = "NextMonth";
                        functionParameters = 0;
                        break;
                    case @operator.nextweek:
                        function = "NextWeek";
                        functionParameters = 0;
                        break;
                    case @operator.nextxdays:
                        function = "NextXDays";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.nextxfiscalperiods:
                        function = "NextXFiscalPeriods";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.nextxfiscalyears:
                        function = "NextXFiscalYears";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.nextxhours:
                        function = "NextXHours";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.nextxmonths:
                        function = "NextXMonths";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.nextxweeks:
                        function = "NextXWeeks";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.nextxyears:
                        function = "NextXYears";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.nextyear:
                        function = "NextYear";
                        functionParameters = 0;
                        break;
                    case @operator.notbetween:
                        function = "NotBetween";
                        functionParameters = Int32.MaxValue;
                        break;
                    case @operator.nebusinessid:
                        function = "NotEqualBusinessId";
                        functionParameters = 0;
                        break;
                    case @operator.neuserid:
                        function = "NotEqualUserId";
                        functionParameters = 0;
                        break;
                    case @operator.notin:
                        function = "NotIn";
                        functionParameters = Int32.MaxValue;
                        break;
                    case @operator.notunder:
                        function = "NotUnder";
                        break;
                    case @operator.olderthanxdays:
                        function = "OlderThanXDays";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.olderthanxhours:
                        function = "OlderThanXHours";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.olderthanxminutes:
                        function = "OlderThanXMinutes";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.olderthanxmonths:
                        function = "OlderThanXMonths";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.olderthanxweeks:
                        function = "OlderThanXWeeks";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.olderthanxyears:
                        function = "OlderThanXYears";
                        functionParameterType = typeof(long);
                        break;
                    case @operator.on:
                        function = "On";
                        break;
                    case @operator.onorafter:
                        function = "OnOrAfter";
                        break;
                    case @operator.onorbefore:
                        function = "OnOrBefore";
                        break;
                    case @operator.thisfiscalperiod:
                        function = "ThisFiscalPeriod";
                        functionParameters = 0;
                        break;
                    case @operator.thisfiscalyear:
                        function = "ThisFiscalYear";
                        functionParameters = 0;
                        break;
                    case @operator.thismonth:
                        function = "ThisMonth";
                        functionParameters = 0;
                        break;
                    case @operator.thisweek:
                        function = "ThisWeek";
                        functionParameters = 0;
                        break;
                    case @operator.thisyear:
                        function = "ThisYear";
                        functionParameters = 0;
                        break;
                    case @operator.today:
                        function = "Today";
                        functionParameters = 0;
                        break;
                    case @operator.tomorrow:
                        function = "Tomorrow";
                        functionParameters = 0;
                        break;
                    case @operator.under:
                        function = "Under";
                        break;
                    case @operator.eqorunder:
                        function = "UnderOrEqual";
                        break;
                    case @operator.yesterday:
                        function = "Yesterday";
                        functionParameters = 0;
                        break;
                    default:
                        throw new Exception($"Unsupported OData condition operator '{condition.@operator}'");
                }

                if (!String.IsNullOrEmpty(function))
                {
                    if (functionParameters == Int32.MaxValue)
                        return $"Microsoft.Dynamics.CRM.{function}(PropertyName='{attrMeta.LogicalName}',PropertyValues=[{String.Join(",", condition.Items.Select(i => FormatValue(functionParameterType, i.Value)))}])";
                    else if (functionParameters == 0)
                        return $"Microsoft.Dynamics.CRM.{function}(PropertyName='{attrMeta.LogicalName}')";
                    else if (functionParameters == 1)
                        return $"Microsoft.Dynamics.CRM.{function}(PropertyName='{attrMeta.LogicalName}',PropertyValue={FormatValue(functionParameterType, condition.value)})";
                    else
                        return $"Microsoft.Dynamics.CRM.{function}(PropertyName='{attrMeta.LogicalName}',{String.Join(",", condition.Items.Select((i, idx) => $"Property{idx + 1}={FormatValue(functionParameterType, i.Value)}"))})";
                }

                if (!string.IsNullOrEmpty(condition.value) && !result.Contains("("))
                {
                    var valueType = typeof(string);

                    switch (attrMeta.AttributeType)
                    {
                        case AttributeTypeCode.Money:
                        case AttributeTypeCode.Decimal:
                            valueType = typeof(decimal);
                            break;

                        case AttributeTypeCode.BigInt:
                            valueType = typeof(long);
                            break;

                        case AttributeTypeCode.Boolean:
                            valueType = typeof(bool);
                            break;

                        case AttributeTypeCode.Double:
                            valueType = typeof(double);
                            break;

                        case AttributeTypeCode.Integer:
                        case AttributeTypeCode.State:
                        case AttributeTypeCode.Status:
                        case AttributeTypeCode.Picklist:
                            valueType = typeof(int);
                            break;

                        case AttributeTypeCode.Uniqueidentifier:
                        case AttributeTypeCode.Lookup:
                        case AttributeTypeCode.Customer:
                        case AttributeTypeCode.Owner:
                            valueType = typeof(Guid);
                            break;

                        case AttributeTypeCode.DateTime:
                            valueType = typeof(DateTime);
                            break;
                    }

                    result += FormatValue(valueType, condition.value);
                }
            }
            return result;
        }

        private static string GetPropertyName(AttributeMetadata attr)
        {
            if (attr is LookupAttributeMetadata)
                return $"_{attr.LogicalName}_value";

            return attr.LogicalName;
        }

        private static string FormatValue(Type type, string s)
        {
            if (type == typeof(string))
                return "'" + s.Replace("'", "''") + "'";

            if (type == typeof(DateTime))
            {
                var date = DateTimeOffset.Parse(s);
                var datestr = string.Empty;
                if (date.Equals(date.Date))
                    return date.ToString("yyyy-MM-dd");
                else
                    return date.ToString("u").Replace(' ', 'T');
            }

            if (type == typeof(bool))
                return s == "1" ? "true" : "false";

            if (type == typeof(Guid))
                return Guid.Parse(s).ToString();

            return Convert.ChangeType(s, type).ToString();
        }

        private static string GetOrder(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var orderitems = entity.Items
                .OfType<FetchOrderType>()
                .Where(i => i.attribute != null);
            return GetOrder(entity.name, sender, orderitems);
        }

        private static string GetOrder(string entityName, FetchXmlBuilder sender, IEnumerable<FetchOrderType> orderitems)
        {
            var results = new List<string>();

            foreach (FetchOrderType orderitem in orderitems)
            {
                if (!String.IsNullOrEmpty(orderitem.alias))
                    throw new ApplicationException($"OData queries do not support ordering on link entities. Please remove the sort on {orderitem.alias}.{orderitem.attribute}");

                var attrMetadata = sender.entities[entityName].Attributes.SingleOrDefault(a => a.LogicalName == orderitem.attribute);
                if (attrMetadata == null)
                    throw new ApplicationException($"No metadata for attribute {entityName}.{orderitem.attribute}");

                var result = GetPropertyName(attrMetadata);
                if (orderitem.descending)
                {
                    result += " desc";
                }
                results.Add(result);
            }

            return String.Join(",", results);
        }

        private static string GetAggregate(FetchEntityType entity, FetchXmlBuilder sender)
        {
            var groups = entity.Items.OfType<FetchAttributeType>()
                .Where(a => a.groupbySpecified)
                .Select(a => a.name)
                .ToList();

            var aggregates = entity.Items.OfType<FetchAttributeType>()
                .Where(a => a.aggregateSpecified && a.aggregate != AggregateType.count)
                .Select(a => $"{a.name} with {GetAggregateType(a.aggregate)} as {a.alias}")
                .ToList();

            aggregates.AddRange(entity.Items.OfType<FetchAttributeType>()
                .Where(a => a.aggregateSpecified && a.aggregate == AggregateType.count)
                .Select(a => $"$count as {a.alias}"));

            var aggregateText = "aggregate(" + String.Join(",", aggregates) + ")";

            if (groups.Count > 0)
            {
                var result = "groupby((" + String.Join(",", groups) + ")";

                if (aggregates.Count > 0)
                    result += "," + aggregateText;

                result += ")";
                return result;
            }

            return aggregateText;
        }

        private static string GetAggregateType(AggregateType aggregate)
        {
            switch (aggregate)
            {
                case AggregateType.avg:
                    return "average";

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

        private static string LinkItemToNavigationProperty(string entityname, FetchLinkEntityType linkitem, FetchXmlBuilder sender, out bool child)
        {
            GetEntityMetadata(entityname, sender);
            var entity = sender.entities[entityname];
            foreach (var relation in entity.OneToManyRelationships
                .Where(r =>
                    r.ReferencedEntity == entityname &&
                    r.ReferencedAttribute == linkitem.to &&
                    r.ReferencingEntity == linkitem.name &&
                    r.ReferencingAttribute == linkitem.from))
            {
                if (linkitem.linktype != "outer")
                    throw new ApplicationException($"OData queries do not support inner joins on 1:N relationships. Try changing link to {linkitem.name} to an outer join");

                child = true;
                return relation.ReferencedEntityNavigationPropertyName;
            }
            foreach (var relation in entity.ManyToOneRelationships
                .Where(r =>
                    r.ReferencingEntity == entityname &&
                    r.ReferencingAttribute == linkitem.to &&
                    r.ReferencedEntity == linkitem.name &&
                    r.ReferencedAttribute == linkitem.from))
            {
                // OData $expand is equivalent to an outer join. Replicate inner join behaviour by adding a not-null filter on primary key
                // of related record type
                if (linkitem.linktype != "outer")
                {
                    if (linkitem.Items == null)
                    {
                        linkitem.Items = new object[0];
                    }
                    var filter = linkitem.Items.OfType<filter>().FirstOrDefault(f => f.type == filterType.and);
                    if (filter == null)
                    {
                        filter = new filter
                        {
                            type = filterType.and,
                            Items = new object[0]
                        };
                        var items = new List<object>(linkitem.Items);
                        items.Add(filter);
                        linkitem.Items = items.ToArray();
                    }
                    GetEntityMetadata(linkitem.name, sender);
                    var linkedEntity = sender.entities[linkitem.name];
                    var condition = filter.Items.OfType<condition>()
                        .FirstOrDefault(c => c.attribute == linkedEntity.PrimaryIdAttribute && c.@operator == @operator.notnull);

                    if (condition == null)
                    {
                        condition = new condition
                        {
                            attribute = linkedEntity.PrimaryIdAttribute,
                            @operator = @operator.notnull
                        };
                        var items = new List<object>(filter.Items);
                        items.Add(condition);
                        filter.Items = items.ToArray();
                    }
                }

                child = false;
                return relation.ReferencingEntityNavigationPropertyName;
            }
            foreach (var relation in entity.ManyToManyRelationships
                .Where(r =>
                    r.Entity1LogicalName == entityname &&
                    r.Entity1IntersectAttribute == linkitem.from))
            {
                var linkitems = linkitem.Items.Where(i => i is FetchLinkEntityType).ToList();
                if (linkitems.Count > 1)
                {
                    throw new Exception("Invalid M:M-relation definition for OData");
                }
                if (linkitems.Count == 1)
                {
                    var nextlink = (FetchLinkEntityType)linkitems[0];
                    if (nextlink.linktype != "outer")
                    {
                        throw new Exception($"OData queries do not support inner joins on N:N relationships. Try changing link to {nextlink.name} to an outer join");
                    }
                    if (relation.Entity2LogicalName == nextlink.name &&
                        relation.Entity2IntersectAttribute == nextlink.to)
                    {
                        child = true;
                        return relation.Entity1NavigationPropertyName;
                    }
                }
            }
            foreach (var relation in entity.ManyToManyRelationships
                .Where(r =>
                    r.Entity2LogicalName == entityname &&
                    r.Entity2IntersectAttribute == linkitem.from))
            {
                var linkitems = linkitem.Items.Where(i => i is FetchLinkEntityType).ToList();
                if (linkitems.Count > 1)
                {
                    throw new Exception("Invalid M:M-relation definition for OData");
                }
                if (linkitems.Count == 1)
                {
                    var nextlink = (FetchLinkEntityType)linkitems[0];
                    if (nextlink.linktype != "outer")
                    {
                        throw new Exception($"OData queries do not support inner joins on N:N relationships. Try changing link to {nextlink.name} to an outer join");
                    }
                    if (relation.Entity1LogicalName == nextlink.name &&
                        relation.Entity1IntersectAttribute == nextlink.from)
                    {
                        child = true;
                        return relation.Entity2NavigationPropertyName;
                    }
                }
            }
            throw new Exception($"Cannot find metadata for relation {entityname}.{linkitem.to} => {linkitem.name}.{linkitem.from}");
        }
    }
}