using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class OData4CodeGenerator
    {
        class LinkEntityOData
        {
            protected virtual string Separator => ";";

            public string PropertyName { get; set; }

            public List<string> Select { get; } = new List<string>();

            public List<LinkEntityOData> Expand { get; } = new List<LinkEntityOData>();

            public List<FilterOData> Filter { get; } = new List<FilterOData>();

            protected virtual IEnumerable<string> GetParts()
            {
                if (Select.Any())
                    yield return "$select=" + String.Join(",", Select);

                if (Expand.Any())
                    yield return "$expand=" + String.Join(",", Expand.Select(e => $"{e.PropertyName}({e})"));

                if (Filter.Any())
                    yield return "$filter=" + String.Join(" and ", Filter);
            }

            public override string ToString()
            {
                return String.Join(Separator, GetParts());
            }
        }

        class FilterOData
        {
            public bool And { get; set; }

            public List<string> Conditions { get; } = new List<string>();

            public List<FilterOData> Filters { get; } = new List<FilterOData>();

            public override string ToString()
            {
                if (Conditions.Count == 0 && Filters.Count == 0)
                    return null;

                var items = Conditions.Select(c => c.ToString())
                    .Concat(Filters.Select(f => f.ToString()))
                    .Where(c => !String.IsNullOrEmpty(c));

                var logicalOperator = And ? " and " : " or ";

                return "(" + String.Join(logicalOperator, items) + ")";

            }
        }

        class EntityOData : LinkEntityOData
        {
            public int? Top { get; set; }

            public List<OrderOData> OrderBy { get; } = new List<OrderOData>();

            public List<string> Groups { get; } = new List<string>();

            public List<string> Aggregates { get; } = new List<string>();

            protected override string Separator => "&";

            protected override IEnumerable<string> GetParts()
            {
                if (Aggregates.Any())
                {
                    var aggregate = $"aggregate({String.Join(",", Aggregates)})";

                    if (Groups.Any())
                    {
                        aggregate = $"groupby(({String.Join(",", Groups)}),{aggregate})";
                    }

                    if (Filter.Any())
                    {
                        aggregate = $"filter({String.Join(" and ", Filter)})/{aggregate}";
                    }

                    yield return "$apply=" + aggregate;
                    yield break;
                }

                foreach (var part in base.GetParts())
                    yield return part;

                if (OrderBy.Any())
                    yield return "$orderby=" + String.Join(",", OrderBy);

                if (Top != null)
                    yield return "$top=" + Top;
            }

            public override string ToString()
            {
                var query = base.ToString();

                return "/" + PropertyName + (String.IsNullOrEmpty(query) ? "" : ("?" + query));
            }
        }

        class OrderOData
        {
            public string PropertyName { get; set; }

            public bool Descending { get; set; }

            public override string ToString()
            {
                return PropertyName + (Descending ? " desc" : " asc");
            }
        }

        public static string GetOData4Query(FetchType fetch, string organizationServiceUrl, FetchXmlBuilder sender)
        {
            if (sender.Service == null)
            {
                throw new Exception("Must have an active connection to CRM to compose OData query.");
            }

            var converted = ConvertOData(fetch, sender);

            var url = organizationServiceUrl + converted;
            return url;
        }

        private static EntityOData ConvertOData(FetchType fetch, FetchXmlBuilder fxb)
        {
            var entity = fetch.Items.Where(i => i is FetchEntityType).FirstOrDefault() as FetchEntityType;

            if (entity == null)
            {
                throw new Exception("Fetch must contain entity definition");
            }

            var odata = new EntityOData();
            odata.PropertyName = LogicalToCollectionName(entity.name, fxb);

            if (!string.IsNullOrEmpty(fetch.top))
            {
                odata.Top = Int32.Parse(fetch.top);
            }

            if (entity.Items == null)
            {
                return odata;
            }

            if (fetch.aggregate)
            {
                odata.Groups.AddRange(ConvertGroups(entity.Items));
                odata.Aggregates.AddRange(ConvertAggregates(entity.Items));
            }

            odata.Select.AddRange(ConvertSelect(entity.name, entity.Items, fxb));
            odata.OrderBy.AddRange(ConvertOrder(entity.name, entity.Items, fxb));
            odata.Filter.AddRange(ConvertFilters(entity.name, entity.Items, fxb, entity.Items));
            odata.Expand.AddRange(ConvertJoins(entity.name, entity.Items, fxb, entity.Items));

            // Add extra filters to simulate inner joins
            var count = 1;
            odata.Filter.AddRange(ConvertInnerJoinFilters(entity.name, entity.Items, fxb, entity.Items, "", ref count));

            return odata;
        }

        private static IEnumerable<string> ConvertGroups(object[] items)
        {
            return items.OfType<FetchAttributeType>()
                .Where(a => a.groupbySpecified)
                .Select(a => a.name);
        }

        private static IEnumerable<string> ConvertAggregates(object[] items)
        {
            return items.OfType<FetchAttributeType>()
                .Where(a => a.aggregateSpecified)
                .Select(a => a.aggregate == AggregateType.count ? $"$count as {a.alias}" : $"{a.name} with {GetAggregateType(a.aggregate)} as {a.alias}");
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

        private static List<FilterOData> ConvertInnerJoinFilters(string entityName, object[] items, FetchXmlBuilder fxb, object[] rootEntityItems, string path, ref int count)
        {
            var filters = new List<FilterOData>();

            foreach (var linkEntity in items.OfType<FetchLinkEntityType>().Where(l => l.linktype == "inner" || String.IsNullOrEmpty(l.linktype)))
            {
                var currentLinkEntity = linkEntity;
                var propertyName = path + LinkItemToNavigationProperty(entityName, currentLinkEntity, fxb, out var child, out var manyToManyNextLink);

                currentLinkEntity = manyToManyNextLink ?? currentLinkEntity;

                if (!child)
                {
                    var childFilter = currentLinkEntity.Items == null ? new List<FilterOData>() : ConvertFilters(currentLinkEntity.name, currentLinkEntity.Items, fxb, rootEntityItems, propertyName + "/").ToList();

                    if (childFilter.Count == 0)
                    {
                        GetEntityMetadata(currentLinkEntity.name, fxb);
                        filters.Add(new FilterOData { Conditions = { $"{propertyName}/{fxb.entities[currentLinkEntity.name].PrimaryIdAttribute} ne null" } });
                    }

                    if (currentLinkEntity.Items != null)
                    {
                        childFilter.AddRange(ConvertInnerJoinFilters(currentLinkEntity.name, currentLinkEntity.Items, fxb, rootEntityItems, path + propertyName + "/", ref count));
                    }

                    filters.AddRange(childFilter);
                }
                else
                {
                    var rangeVariable = "o" + (count++);
                    var childFilter = currentLinkEntity.Items == null ? new List<FilterOData>() : ConvertFilters(currentLinkEntity.name, currentLinkEntity.Items, fxb, rootEntityItems, $"{rangeVariable}/").ToList();

                    if (childFilter.Count == 0)
                    {
                        GetEntityMetadata(currentLinkEntity.name, fxb);
                        childFilter.Add(new FilterOData { Conditions = { $"{rangeVariable}/{fxb.entities[currentLinkEntity.name].PrimaryIdAttribute} ne null" } });
                    }

                    if (currentLinkEntity.Items != null)
                    {
                        childFilter.AddRange(ConvertInnerJoinFilters(currentLinkEntity.name, currentLinkEntity.Items, fxb, rootEntityItems, path + rangeVariable + "/", ref count));
                    }

                    var condition = propertyName + $"/any({rangeVariable}:{String.Join(" and ", childFilter)})";
                    filters.Add(new FilterOData { Conditions = { condition } });
                }
            }

            return filters;
        }

        private static IEnumerable<LinkEntityOData> ConvertJoins(string entityName, object[] items, FetchXmlBuilder fxb, object[] rootEntityItems)
        {
            foreach (var linkEntity in items.OfType<FetchLinkEntityType>().Where(l => l.Items != null && l.Items.Any()))
            {
                var currentLinkEntity = linkEntity;
                var expand = new LinkEntityOData();
                expand.PropertyName = LinkItemToNavigationProperty(entityName, currentLinkEntity, fxb, out var child, out var manyToManyNextLink);
                currentLinkEntity = manyToManyNextLink ?? currentLinkEntity;
                expand.Select.AddRange(ConvertSelect(currentLinkEntity.name, currentLinkEntity.Items, fxb));

                if (linkEntity.linktype == "outer" || child)
                {
                    // Don't need to add filters at this point for single-valued properties in inner joins, they'll be added separately later
                    expand.Filter.AddRange(ConvertFilters(currentLinkEntity.name, currentLinkEntity.Items, fxb, rootEntityItems));
                }

                // Recurse into child joins
                expand.Expand.AddRange(ConvertJoins(currentLinkEntity.name, currentLinkEntity.Items, fxb, rootEntityItems));

                yield return expand;
            }
        }

        private static IEnumerable<string> ConvertSelect(string entityName, object[] items, FetchXmlBuilder fxb)
        {
            var attributeitems = items
                .OfType<FetchAttributeType>()
                .Where(i => i.name != null);

            return GetAttributeNames(entityName, attributeitems, fxb);
        }

        private static IEnumerable<string> GetAttributeNames(string entityName, IEnumerable<FetchAttributeType> attributeitems, FetchXmlBuilder sender)
        {
            GetEntityMetadata(entityName, sender);
            var entityMeta = sender.entities[entityName];

            foreach (FetchAttributeType attributeitem in attributeitems)
            {
                var attrMeta = entityMeta.Attributes.SingleOrDefault(a => a.LogicalName == attributeitem.name);

                if (attrMeta == null)
                    throw new ApplicationException($"Unknown attribute {entityName}.{attributeitem.name}");

                yield return GetPropertyName(attrMeta);
            }
        }

        private static IEnumerable<FilterOData> ConvertFilters(string entityName, object[] items, FetchXmlBuilder fxb, object[] rootEntityItems, string navigationProperty = "")
        {
            return items
                .OfType<filter>()
                .Where(f => f.Items != null && f.Items.Any())
                .Select(f =>
                {
                    var filterOData = new FilterOData { And = f.type == filterType.and };
                    filterOData.Conditions.AddRange(ConvertConditions(entityName, f.Items, fxb, rootEntityItems, navigationProperty));
                    filterOData.Filters.AddRange(ConvertFilters(entityName, f.Items, fxb, rootEntityItems, navigationProperty));
                    return filterOData;
                });
        }

        private static IEnumerable<string> ConvertConditions(string entityName, object[] items, FetchXmlBuilder fxb, object[] rootEntityItems, string navigationProperty = "")
        {
            return items
                .OfType<condition>()
                .Select(c => GetCondition(entityName, c, fxb, rootEntityItems, navigationProperty));
        }

        private static string GetCondition(string entityName, condition condition, FetchXmlBuilder fxb, object[] rootEntityItems, string navigationProperty = "")
        {
            var result = "";
            if (!string.IsNullOrEmpty(condition.attribute))
            {
                if (!String.IsNullOrEmpty(condition.entityname))
                {
                    var linkEntity = FindLinkEntity(entityName, rootEntityItems, fxb, condition.entityname, "", out navigationProperty, out var child);

                    if (linkEntity == null)
                    {
                        throw new Exception($"Cannot find filter entity " + condition.entityname);
                    }

                    if (child)
                    {
                        // Filtering a child collection separately has different semantics in OData vs. FetchXML, e.g.:
                        //
                        // <fetch top="50" >
                        //   <entity name="account" >
                        //     <attribute name="name" />
                        //     <filter type="or" >
                        //       <condition attribute="name" operator="eq" value="fxb" />
                        //       <condition entityname="contact" attribute="firstname" operator="eq" value="jonas" />
                        //     </filter>
                        //     <link-entity name="contact" from="parentcustomerid" to="accountid" link-type="inner" >
                        //       <attribute name="fullname" />
                        //       <filter>
                        //         <condition attribute="lastname" operator="eq" value="rapp" />
                        //       </filter>
                        //     </link-entity>
                        //   </entity>
                        // </fetch>
                        //
                        // gives a result only where a contact matches both the firstname and lastname filters, unless the account name is "fxb"
                        // in which case only the lastname filter needs to match. By comparison, the similar OData query
                        //
                        // /accounts?$select=name&$expand=contact_customer_accounts($select=fullname;$filter=lastname eq 'rapp')&$filter=(name eq 'fxb' or contact_customer_accounts/any(o1:(o1/firstname eq 'jonas'))) and (contact_customer_accounts/any(o1:(o1/lastname eq 'rapp')))&$top=50
                        //
                        // applies the firstname and lastname filters separately on the full list of contacts, so as long as one contact matches the firstname
                        // filter it doesn't matter if it is the same record that matches the lastname filter.
                        throw new Exception("Cannot apply filter to child collection " + navigationProperty);
                    }

                    entityName = linkEntity.name;
                }

                GetEntityMetadata(entityName, fxb);
                var attrMeta = fxb.GetAttribute(entityName, condition.attribute);
                if (attrMeta == null)
                {
                    throw new Exception($"No metadata for attribute: {entityName}.{condition.attribute}");
                }
                result = navigationProperty + GetPropertyName(attrMeta);
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
                    case @operator.notlike:
                        var value = condition.value;
                        var func = "contains";

                        if (value.IndexOf('%') == value.Length - 1)
                        {
                            value = value.Substring(0, value.Length - 1);
                            func = "startswith";
                        }
                        else if (value.LastIndexOf('%') == 0)
                        {
                            value = value.Substring(1);
                            func = "endswith";
                        }

                        result = $"{func}({HttpUtility.UrlEncode(navigationProperty + attrMeta.LogicalName)}, {FormatValue(typeof(string), value)})";

                        if (condition.@operator == @operator.notlike)
                        {
                            result = "not " + result;
                        }
                        break;
                    case @operator.beginswith:
                    case @operator.notbeginwith:
                        result = $"startswith({HttpUtility.UrlEncode(navigationProperty + attrMeta.LogicalName)}, {FormatValue(typeof(string), condition.value)})";

                        if (condition.@operator == @operator.notbeginwith)
                        {
                            result = "not " + result;
                        }
                        break;
                    case @operator.endswith:
                    case @operator.notendwith:
                        result = $"endswith({HttpUtility.UrlEncode(navigationProperty + attrMeta.LogicalName)}, {FormatValue(typeof(string), condition.value)})";

                        if (condition.@operator == @operator.notendwith)
                        {
                            result = "not " + result;
                        }
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
                        function = "ContainValues";
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
                        return $"Microsoft.Dynamics.CRM.{HttpUtility.UrlEncode(function)}(PropertyName='{HttpUtility.UrlEncode(navigationProperty + attrMeta.LogicalName)}',PropertyValues=[{String.Join(",", condition.Items.Select(i => FormatValue(functionParameterType, i.Value)))}])";
                    else if (functionParameters == 0)
                        return $"Microsoft.Dynamics.CRM.{HttpUtility.UrlEncode(function)}(PropertyName='{HttpUtility.UrlEncode(navigationProperty + attrMeta.LogicalName)}')";
                    else if (functionParameters == 1)
                        return $"Microsoft.Dynamics.CRM.{HttpUtility.UrlEncode(function)}(PropertyName='{HttpUtility.UrlEncode(navigationProperty + attrMeta.LogicalName)}',PropertyValue={FormatValue(functionParameterType, condition.value)})";
                    else
                        return $"Microsoft.Dynamics.CRM.{HttpUtility.UrlEncode(function)}(PropertyName='{HttpUtility.UrlEncode(navigationProperty + attrMeta.LogicalName)}',{String.Join(",", condition.Items.Select((i, idx) => $"Property{idx + 1}={FormatValue(functionParameterType, i.Value)}"))})";
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
                else if (!string.IsNullOrEmpty(condition.valueof))
                {
                    result += condition.valueof;
                }
            }
            return result;
        }

        private static FetchLinkEntityType FindLinkEntity(string entityName, object[] items, FetchXmlBuilder fxb, string alias, string path, out string navigationProperty, out bool child)
        {
            child = false;
            navigationProperty = path;

            foreach (var linkItem in items.OfType<FetchLinkEntityType>())
            {
                var currentLinkItem = linkItem;
                var propertyName = LinkItemToNavigationProperty(entityName, linkItem, fxb, out child, out var manyToManyNextLink);
                currentLinkItem = manyToManyNextLink ?? currentLinkItem;

                navigationProperty = path + propertyName + "/";

                if (linkItem.alias == alias || (string.IsNullOrEmpty(linkItem.alias) && linkItem.name == alias))
                {
                    return linkItem;
                }

                var childMatch = FindLinkEntity(linkItem.name, linkItem.Items, fxb, alias, navigationProperty, out navigationProperty, out child);

                if (childMatch != null)
                {
                    return childMatch;
                }
            }

            return null;
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
                return "'" + HttpUtility.UrlEncode(s.Replace("'", "''")) + "'";

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

            return HttpUtility.UrlEncode(Convert.ChangeType(s, type).ToString());
        }

        private static IEnumerable<OrderOData> ConvertOrder(string entityName, object[] items, FetchXmlBuilder sender)
        {
            return items
                .OfType<FetchOrderType>()
                .Where(o => o.attribute != null)
                .Select(o => ConvertOrder(entityName, o, sender));
        }

        private static OrderOData ConvertOrder(string entityName, FetchOrderType orderitem, FetchXmlBuilder sender)
        {
            if (!String.IsNullOrEmpty(orderitem.alias))
                throw new ApplicationException($"OData queries do not support ordering on link entities. Please remove the sort on {orderitem.alias}.{orderitem.attribute}");

            var attrMetadata = sender.entities[entityName].Attributes.SingleOrDefault(a => a.LogicalName == orderitem.attribute);
            if (attrMetadata == null)
                throw new ApplicationException($"No metadata for attribute {entityName}.{orderitem.attribute}");

            var odata = new OrderOData
            {
                PropertyName = GetPropertyName(attrMetadata),
                Descending = orderitem.descending
            };

            return odata;
        }
        /*
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
        */
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

        private static string LinkItemToNavigationProperty(string entityname, FetchLinkEntityType linkitem, FetchXmlBuilder sender, out bool child, out FetchLinkEntityType manyToManyNextLink)
        {
            manyToManyNextLink = null;
            GetEntityMetadata(entityname, sender);
            var entity = sender.entities[entityname];
            foreach (var relation in entity.OneToManyRelationships
                .Where(r =>
                    r.ReferencedEntity == entityname &&
                    r.ReferencedAttribute == linkitem.to &&
                    r.ReferencingEntity == linkitem.name &&
                    r.ReferencingAttribute == linkitem.from))
            {
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
                    if (relation.Entity2LogicalName == nextlink.name &&
                        relation.Entity2IntersectAttribute == nextlink.to)
                    {
                        child = true;
                        manyToManyNextLink = nextlink;
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
                    if (relation.Entity1LogicalName == nextlink.name &&
                        relation.Entity1IntersectAttribute == nextlink.from)
                    {
                        child = true;
                        manyToManyNextLink = nextlink;
                        return relation.Entity2NavigationPropertyName;
                    }
                }
            }
            throw new Exception($"Cannot find metadata for relation {entityname}.{linkitem.to} => {linkitem.name}.{linkitem.from}");
        }
    }
}