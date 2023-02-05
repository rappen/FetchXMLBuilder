using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Settings;
using Rappen.XTB.LCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    public class CSharpCodeGenerator
    {
        private List<string> globalVariables;
        private static string CRLF = "\r\n";
        private QueryExpression qex;
        private List<EntityMetadata> metas;
        private CodeGenerators settings;
        private Dictionary<string, string> entityaliases;
        private string betweenchar = ";";
        private const int CODE_WIDTH_LIMIT = 120;

        public static string GetCSharpQueryExpression(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings settings)
        {
            if (settings.CodeGenerators.QExStyle == QExStyleEnum.QueryExpression &&
                settings.CodeGenerators.QExFlavor == QExFlavorEnum.EarlyBound)
            {
                throw new ArgumentOutOfRangeException("Style & Flavor", "Combo is not possible.");
            }
            if (settings.CodeGenerators.QExStyle == QExStyleEnum.QueryExpressionFactory)
            {
                throw new ArgumentOutOfRangeException("Style", "Sorry, not yet finalized. It's a bit tricky, but we're getting there... One day...");
            }
            if (settings.CodeGenerators.QExStyle == QExStyleEnum.OrganizationServiceContext)
            {
                throw new ArgumentOutOfRangeException("Style", "Not really started yet. Might get there... One month...");
            }

            var coder = new CSharpCodeGenerator(QEx, entities, settings);
            var result = coder.GetQueryCode();
            result = string.Join("\n", result.Split('\n').Select(l => Indent(settings.CodeGenerators.Indents) + l));
            return result;
        }

        private CSharpCodeGenerator(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings fxbsettings)
        {
            globalVariables = new List<string>();
            entityaliases = new Dictionary<string, string>();
            metas = entities;
            qex = QEx;
            settings = fxbsettings.CodeGenerators;
            betweenchar = settings.QExStyle == QExStyleEnum.FluentQueryExpression ? "" : ",";
            StoreLinkEntityAliases(qex.LinkEntities);
        }

        #region General

        private string GetQueryCode()
        {
            var qename = GetVarName(GetQueryObjectName(settings.QExStyle));
            var queryclass = QExStyle.StyleClassName(settings.QExStyle);
            var entityname = GetCodeEntity(qex.EntityName);
            var earlybound = settings.QExFlavor == QExFlavorEnum.EarlyBound;
            var queryproperties = new List<string>();
            if (settings.ObjectInitializer)
            {
                if (settings.QExStyle != QExStyleEnum.QueryExpressionFactory)
                {
                    queryproperties.AddRange(GetQueryOptions(qex, qename, 1));
                }
                queryproperties.AddRange(GetObjectInitializer(qename, qex, 1));
            }

            var querycode = string.Empty;
            if (settings.IncludeComments)
            {
                querycode += $"// Instantiate {queryclass} {qename}{CRLF}";
            }
            querycode += $"var {qename} = ";
            switch (settings.QExStyle)
            {
                case QExStyleEnum.QueryExpressionFactory:
                    querycode += $"{queryclass}.Create{(earlybound ? "<" : "(")}{entityname}{(earlybound ? ">(" : "")}";
                    break;

                case QExStyleEnum.FluentQueryExpression:
                    querycode += $"new {queryclass}{(earlybound ? "<" : "(")}{entityname}{(earlybound ? ">()" : ")")}";
                    break;

                default:
                    querycode += $"new {queryclass}({entityname})";
                    if (settings.ObjectInitializer)
                    {
                        querycode += $"{CRLF}{{";
                    }
                    break;
            }

            if (settings.ObjectInitializer)
            {
                if (queryproperties.Any())
                {
                    querycode += CRLF + string.Join($"{betweenchar}{CRLF}", queryproperties);
                }
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.FluentQueryExpression:
                        querycode += ";";
                        break;

                    case QExStyleEnum.QueryExpressionFactory:
                        querycode += $");{CRLF}";
                        querycode += string.Join(CRLF, GetQueryOptions(qex, qename, 0).Select(p => $"{qename}.{p};")) + CRLF;
                        var linkentities = GetLinkEntitiesLbL(qex.LinkEntities, qename);
                        if (!string.IsNullOrWhiteSpace(linkentities))
                        {
                            querycode += $"{linkentities}";
                        }
                        querycode += GetOrdersLbL(qex.EntityName, qex.Orders, qename, true);
                        break;

                    default:
                        querycode += $"{CRLF}}};";
                        break;
                }
            }
            else
            {
                if (settings.QExStyle == QExStyleEnum.QueryExpressionFactory)
                {
                    var columnscode = GetColumnsOI(entityname, qex.ColumnSet, OwnersType.Root, 1);
                    if (!string.IsNullOrWhiteSpace(columnscode))
                    {
                        querycode += $",{CRLF}{columnscode}";
                    }
                    var filterscode = GetFilterOI(entityname, qex.Criteria, qename, OwnersType.Root, 1);
                    if (!string.IsNullOrWhiteSpace(filterscode))
                    {
                        querycode += $",{CRLF}{filterscode}";
                    }
                    querycode += ")";
                }
                querycode += $";{CRLF}";
                querycode += string.Join(CRLF, GetQueryOptions(qex, qename, 0).Select(p => $"{qename}.{p};"));
                if (settings.QExStyle != QExStyleEnum.QueryExpressionFactory)
                {
                    querycode += GetColumnsLbL(qex.EntityName, qex.ColumnSet, qename, OwnersType.Root);
                    querycode += GetFilterLbL(qex.EntityName, qex.Criteria, qename, OwnersType.Root);
                }
                querycode += GetOrdersLbL(qex.EntityName, qex.Orders, qename, true);
                querycode += GetLinkEntitiesLbL(qex.LinkEntities, qename);
                querycode = querycode.Replace($"//{CRLF}", CRLF);
            }

            querycode = ReplaceValueTokens(querycode);
            return querycode;
        }

        private List<string> GetObjectInitializer(string qename, QueryExpression qex, int indentslevel)
        {
            return new List<string>
            {
                GetColumnsOI(qex.EntityName, qex.ColumnSet, OwnersType.Root, indentslevel),
                GetFilterOI(qex.EntityName, qex.Criteria, qename, OwnersType.Root, indentslevel),
                settings.QExStyle == QExStyleEnum.QueryExpressionFactory ? "" : GetOrdersOI(qex.EntityName, qex.Orders, qename, OwnersType.Root, indentslevel),
                settings.QExStyle == QExStyleEnum.QueryExpressionFactory ? "" : GetLinkEntitiesOI(qex.LinkEntities, qename, indentslevel)
            }.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

        private string GetColumns(string entity, ColumnSet columns, string ownerName, OwnersType ownerType, int indentslevel) => settings.ObjectInitializer ? GetColumnsOI(entity, columns, ownerType, indentslevel) : GetColumnsLbL(entity, columns, ownerName, ownerType);

        private string GetFilter(string entity, FilterExpression filter, string ownerName, OwnersType ownerType, int indentslevel = 1, List<string> namestree = null) => settings.ObjectInitializer ? GetFilterOI(entity, filter, ownerName, ownerType, indentslevel, namestree) : GetFilterLbL(entity, filter, ownerName, ownerType);

        private string GetOrders(string entityname, DataCollection<OrderExpression> orders, string ownerName, OwnersType ownerType, int indentslevel = 1) => settings.ObjectInitializer ? GetOrdersOI(entityname, orders, ownerName, ownerType, indentslevel) : GetOrdersLbL(entityname, orders, ownerName);

        private string GetLinkEntities(DataCollection<LinkEntity> linkEntities, string ownerName, int indentslevel = 1, List<string> namestree = null) => settings.ObjectInitializer ? GetLinkEntitiesOI(linkEntities, ownerName, indentslevel, namestree) : GetLinkEntitiesLbL(linkEntities, ownerName);

        #endregion General

        #region Line-by Line

        private string GetColumnsLbL(string entity, ColumnSet columns, string ownerName, OwnersType ownerType)
        {
            ownerName = $"{ownerName}.{(ownerType == OwnersType.Root ? "ColumnSet" : ownerType == OwnersType.Link ? "Columns" : "** WRONG **")}";
            var code = new StringBuilder();
            if (columns.AllColumns)
            {   // All Columns
                if (settings.IncludeComments)
                {
                    code.AppendLine($"//{CRLF}// Add all columns to " + ownerName);
                }
                code.AppendLine(ownerName + ".AllColumns = true;");
            }
            else if (columns.Columns.Count > 0)
            {   // Selected Columns
                if (settings.IncludeComments)
                {
                    code.AppendLine($"//{CRLF}// Add columns to " + ownerName);
                }
                ownerName += ".AddColumn" + (columns.Columns.Count > 1 ? "s" : "");
                var colsEB = GetCodeParametersMaxWidth(CODE_WIDTH_LIMIT - ownerName.Length, 0, AroundBy.Parentheses, columns.Columns.Select(c => GetCodeAttribute(entity, c)).ToArray());
                code.AppendLine(ownerName + colsEB + ";");
            }
            return code.ToString().TrimEnd() + CRLF;
        }

        private string GetFiltersLbL(string entity, FilterExpression filter, string ownerName)
        {
            if (filter?.Filters?.Any() != true)
            {
                return string.Empty;
            }
            var filters = filter.Filters.Where(f => f.Conditions.Any() || f.Filters.Any());
            var several = filters.Count() > 1;
            var filtercodes = new List<string>();
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
            foreach (var filteritem in filters)
            {
                filtercodes.Add(GetFilterLbL(entity, filteritem, filter.FilterHint ?? ownerName, OwnersType.Sub, several));
            }
            return string.Join(CRLF, filtercodes.Select(f => f.Trim()).Where(f => !string.IsNullOrWhiteSpace(f)));
        }

        private string GetFilterLbL(string entity, FilterExpression filter, string ownerName, OwnersType ownerType, bool several = false)
        {
            if (filter == null || (!filter.Conditions.Any() && !filter.Filters.Any()))
            {
                return string.Empty;
            }
            if (settings.QExStyle == QExStyleEnum.QueryByAttribute && ownerType != OwnersType.Root)
            {
                throw new Exception("Only root filters are supported for QueryByAttribute. Use QueryExpression if you need it.");
            }
            var code = new StringBuilder();
            filter.FilterHint = $"{ownerName}.{(ownerType == OwnersType.Root ? "Criteria" : ownerType == OwnersType.Link ? "LinkCriteria" : "Filters")}";
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
            if (ownerType == OwnersType.Sub)
            {
                filter.FilterHint = GetVarName($"{ownerName}.{(filter.FilterOperator == LogicalOperator.Or ? "Or" : "And")}".Replace(".Criteria", "").Replace(".LinkCriteria", ""), several);

                if (settings.IncludeComments)
                {
                    code.AppendLine($"//{CRLF}// Add filter {filter.FilterHint} to {ownerName}");
                }
                code.AppendLine($"var {filter.FilterHint} = new FilterExpression({(filter.FilterOperator == LogicalOperator.Or ? "LogicalOperator.Or" : "")});");
                code.AppendLine($"{ownerName}.AddFilter({filter.FilterHint});");
            }
            var filtercode = new List<string>
            {
                GetConditionsLbL(entity, filter),
                GetFiltersLbL(entity, filter, ownerName)
            };
            code.Append(string.Join(CRLF, filtercode.Select(f => f.Trim()).Where(f => !string.IsNullOrWhiteSpace(f))));
            return code.ToString().TrimEnd() + CRLF;
        }

        private string GetConditionsLbL(string entity, FilterExpression filter)
        {
            if (filter?.Conditions?.Any() != true)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            if (settings.IncludeComments)
            {
                code.AppendLine($"//{CRLF}// Add conditions to {filter.FilterHint}");
            }
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
            var basedname = filter.FilterHint.Replace(".", "_").Replace("_Criteria", "").Replace("_LinkCriteria", "");
            var conditionscode = new List<string>();
            foreach (var cond in filter.Conditions)
            {
                var filterentity = entity;
                var entityalias = "";
                if (!string.IsNullOrWhiteSpace(cond.EntityName))
                {
                    filterentity = entityaliases.FirstOrDefault(a => a.Key.Equals(cond.EntityName)).Value ?? cond.EntityName;
                    entityalias = "\"" + cond.EntityName + "\", ";
                }
                var attributename = GetCodeAttribute(filterentity, cond.AttributeName);
                var values = GetConditionValues(filterentity, cond, basedname);
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryByAttribute:
                        if (cond.Operator != ConditionOperator.Equal)
                        {
                            throw new Exception("Only equal conditions are supported for QueryByAttribute. Use QueryExpression if you need it.");
                        }
                        if (!string.IsNullOrWhiteSpace(entityalias))
                        {
                            throw new Exception("Alias not supported for QueryByAttribute. Use QueryExpression if you need it.");
                        }
                        if (cond.Values.Count != 1)
                        {
                            throw new Exception("Has to be one (1) value for conditions for QueryByAttribute. Use QueryExpression if you need it.");
                        }
                        conditionscode.Add($"{basedname}.AddAttributeValue({attributename}{values});");
                        break;

                    default:
                        conditionscode.Add($"{filter.FilterHint}.AddCondition({entityalias}{attributename}, ConditionOperator.{cond.Operator}{values});");
                        break;
                }
            }
            code.Append(string.Join(CRLF, conditionscode.Select(f => f.Trim()).Where(f => !string.IsNullOrWhiteSpace(f))));
            return code.ToString().TrimEnd() + CRLF;
        }

        private string GetLinkEntitiesLbL(DataCollection<LinkEntity> linkEntities, string LineStart)
        {
            if (linkEntities?.Count == 0)
            {
                return string.Empty;
            }
            if (settings.QExStyle == QExStyleEnum.QueryByAttribute)
            {
                throw new Exception("Link Entity is not supported for QueryByAttribute. Use QueryExpression if you need it.");
            }
            var linkcodec = new List<string>();
            foreach (var link in linkEntities)
            {
                var linkcode = string.Empty;
                var linkname = GetVarName(string.IsNullOrEmpty(link.EntityAlias) ? LineStart + "_" + link.LinkToEntityName : link.EntityAlias);
                if (settings.IncludeComments)
                {
                    linkcode += $"//{CRLF}// Add link-entity {linkname}{CRLF}";
                }
                var join = link.JoinOperator == JoinOperator.Inner ? "" : "JoinOperator." + link.JoinOperator.ToString();
                var varstart =
                    link.LinkEntities.Count > 0 ||
                    link.Columns.Columns.Count > 0 ||
                    link.LinkCriteria.Conditions.Count > 0 ||
                    link.Orders.Count > 0 ? $"var {linkname} = " : String.Empty;
                var fromattribute = GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName);
                var toattribute = GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName);
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        if (fromattribute == toattribute)
                        {
                            toattribute = null;
                        }
                        break;
                }
                var parms = GetCodeParametersMaxWidth(CODE_WIDTH_LIMIT - varstart.Length - LineStart.Length, 0, AroundBy.Parentheses,
                    GetCodeEntity(link.LinkToEntityName),
                    fromattribute,
                    toattribute,
                    join);
                linkcode += $"{varstart}{LineStart}.AddLink{parms};{CRLF}";
                if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                {
                    linkcode += $"{linkname}.EntityAlias = \"{link.EntityAlias}\";{CRLF}";
                }
                linkcode += GetColumnsLbL(link.LinkToEntityName, link.Columns, linkname, OwnersType.Link);
                linkcode += GetFilter(link.LinkToEntityName, link.LinkCriteria, linkname, OwnersType.Link);
                linkcode += GetOrders(link.LinkToEntityName, link.Orders, linkname, OwnersType.Link);
                linkcode += GetLinkEntities(link.LinkEntities, linkname);
                linkcodec.Add(linkcode);
            }
            return string.Join(CRLF, linkcodec.Select(f => f.Trim())).TrimEnd() + CRLF;
        }

        private string GetOrdersLbL(string entityname, DataCollection<OrderExpression> orders, string ownerName, bool root = false)
        {
            if (orders.Count == 0)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            if (!root && settings.QExStyle == QExStyleEnum.QueryExpressionFactory)
            {
                code.AppendLine($"// Orders for link entities is currently not supported for QueryExpressionFactory.{CRLF}// Please add it manually:");
                ownerName = "// " + ownerName;
            }
            else if (settings.IncludeComments)
            {
                code.AppendLine($"//{CRLF}// Add orders");
            }
            ownerName += root ? ".AddOrder(" : ".Orders.Add(new OrderExpression(";
            var LineEnd = root ? ");" : "));";
            var ordercodes = new List<string>();
            foreach (var order in orders)
            {
                ordercodes.Add(ownerName + GetCodeAttribute(entityname, order.AttributeName) + ", OrderType." + order.OrderType.ToString() + LineEnd);
            }
            code.AppendLine(string.Join(CRLF, ordercodes.Select(f => f.Trim())));
            return code.ToString().TrimEnd() + CRLF;
        }

        #endregion Line-by Line

        #region Object Initilization

        private string GetColumnsOI(string entity, ColumnSet columns, OwnersType ownerType, int indentslevel)
        {
            var objectName = ownerType == OwnersType.Root ? "ColumnSet" : ownerType == OwnersType.Link ? "Columns" : "** WRONG **";
            var code = new StringBuilder();
            if (columns.AllColumns)
            {   // All Columns
                if (settings.IncludeComments)
                {
                    code.AppendLine($"{Indent(indentslevel)}// Add all columns to {entity}");
                }
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.FluentQueryExpression:
                        code.Append($"{Indent(indentslevel)}.Select(true)");
                        break;

                    default:
                        switch (settings.QExFlavor)
                        {
                            case QExFlavorEnum.EarlyBound:
                                code.Append($"{Indent(indentslevel)}new ColumnSet(true)");
                                break;

                            default:
                                code.Append($"{Indent(indentslevel)}ColumnSet = new ColumnSet(true)");
                                break;
                        }
                        break;
                }
            }
            else if (columns.Columns.Count > 0)
            {   // Selected Columns
                if (settings.IncludeComments)
                {
                    code.AppendLine($"{Indent(indentslevel)}// Add {columns.Columns.Count} columns to {entity}");
                }
                var columnsstart = string.Empty;
                var columnend = string.Empty;
                var aroundby = AroundBy.Parentheses;
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        switch (settings.QExFlavor)
                        {
                            case QExFlavorEnum.EarlyBound:
                                columnsstart = $"{GetCodeEntityPrefix(entity)} => new ";
                                aroundby = AroundBy.Braces;
                                break;

                            default:
                                columnsstart = $"new ColumnSet";
                                break;
                        }
                        break;

                    case QExStyleEnum.FluentQueryExpression:
                        switch (settings.QExFlavor)
                        {
                            case QExFlavorEnum.EarlyBound:
                                columnsstart = $".Select({GetCodeEntityPrefix(entity)} => new ";
                                columnend = ")";
                                aroundby = AroundBy.Braces;
                                break;

                            default:
                                columnsstart = $".Select";
                                break;
                        }
                        break;

                    default:
                        columnsstart = $"{objectName} = new ColumnSet";
                        break;
                }
                var columnscode = columns.Columns.Select(c => GetCodeAttribute(entity, c, settings.QExFlavor == QExFlavorEnum.EarlyBound)).ToArray();
                var colsEB = GetCodeParametersMaxWidth(CODE_WIDTH_LIMIT - columnsstart.Length, indentslevel, aroundby, columnscode);
                code.Append(Indent(indentslevel) + columnsstart + colsEB + columnend);
            }
            return code.ToString();
        }

        private string GetFiltersOI(string entity, FilterExpression filter, string ownerName, int indentslevel, List<string> namestree = null)

        {
            if (filter?.Filters?.Any() != true)
            {
                return string.Empty;
            }
            var filterscode = "";
            var filters = filter.Filters.Where(f => f.Conditions.Any() || f.Filters.Any());
            var filtercodes = new List<string>();
            var rootfilters = settings.QExFlavor == QExFlavorEnum.EarlyBound && (filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters"));
            var comment = $"{Indent(indentslevel)}// Add {filters.Count()} filters to {entity}{CRLF}";
            // Before filters
            switch (settings.QExStyle)
            {
                case QExStyleEnum.QueryExpressionFactory:
                    if (!rootfilters)
                    {
                        filterscode += $"{Indent(indentslevel)}Filters ={CRLF}{Indent(indentslevel++)}{{{CRLF}";
                    }
                    break;

                case QExStyleEnum.FluentQueryExpression:
                    break;

                default:
                    filterscode += $"{Indent(indentslevel)}Filters ={CRLF}{Indent(indentslevel++)}{{{CRLF}";
                    break;
            }
            if (settings.IncludeComments && !string.IsNullOrWhiteSpace(filterscode))
            {
                filterscode = $"{comment}{filterscode}";
            }
            // Add the filters
            filters.ToList().ForEach(f => filtercodes.Add(GetFilterOI(entity, f, filter.FilterHint ?? ownerName, OwnersType.Sub, indentslevel, namestree)));
            filterscode += string.Join($"{betweenchar}{CRLF}", filtercodes.Where(f => !string.IsNullOrWhiteSpace(f)));
            // After filters
            switch (settings.QExStyle)
            {
                case QExStyleEnum.QueryExpressionFactory:
                    if (!rootfilters)
                    {
                        filterscode += $"{CRLF}{Indent(--indentslevel)}}}";
                    }
                    break;

                case QExStyleEnum.FluentQueryExpression:
                    break;

                default:
                    filterscode += $"{CRLF}{Indent(--indentslevel)}}}";
                    break;
            }
            return filterscode;
        }

        private string GetFilterOI(string entity, FilterExpression filter, string ownerName, OwnersType ownerType, int indentslevel, List<string> namestree = null)
        {
            if (filter == null || (!filter.Conditions.Any() && !filter.Filters.Any()))
            {
                return string.Empty;
            }
            if (settings.QExStyle == QExStyleEnum.QueryByAttribute && ownerType != OwnersType.Root)
            {
                throw new Exception("Only root filters are supported for QueryByAttribute. Use QueryExpression if you need it.");
            }
            var code = new StringBuilder();
            var filterObjectName = ownerType == OwnersType.Root ? "Criteria" : ownerType == OwnersType.Link ? "LinkCriteria" : "Filters";
            filter.FilterHint = $"{ownerName}.{filterObjectName}";
            var commentconds = filter.Conditions.Count > 0 ? $" {filter.Conditions.Count} conditions" : "";
            var commentfilts = filter.Filters.Count > 0 ? $" {filter.Filters.Count} filters" : "";
            var comment = $"{Indent(indentslevel)}// Add filter to {entity} with{commentconds}{commentfilts}{CRLF}";
            var dlabrootfilters = /*settings.QExFlavor == QExFlavorEnum.EarlyBound &&*/ (filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters"));
            var addfilterexpression = true;
            switch (settings.QExStyle)
            {
                case QExStyleEnum.QueryExpressionFactory:
                    if (!dlabrootfilters || filter.FilterOperator == LogicalOperator.Or || !filter.Conditions.Any())
                    {
                        code.Append($"{Indent(indentslevel)}new FilterExpression({(filter.FilterOperator == LogicalOperator.Or ? "LogicalOperator.Or" : "")}){CRLF}{Indent(indentslevel++)}{{{CRLF}");
                    }
                    else
                    {
                        addfilterexpression = false;
                    }
                    break;

                case QExStyleEnum.FluentQueryExpression:
                    if (ownerType == OwnersType.Sub || filter.FilterOperator == LogicalOperator.Or)
                    {
                        switch (settings.QExFlavor)
                        {
                            case QExFlavorEnum.EarlyBound:
                                var filtershortname = GetVarName("f", false, namestree = namestree ?? new List<string>());
                                code.Append($"{Indent(indentslevel++)}.AddFilter({filtershortname} => {filtershortname}{CRLF}");
                                if (filter.FilterOperator == LogicalOperator.Or)
                                {
                                    code.Append($"{Indent(indentslevel)}.SetLogicalOperator(LogicalOperator.Or){CRLF}");
                                }
                                break;

                            default:
                                code.Append($"{Indent(indentslevel++)}.AddFilter(new Filter({(filter.FilterOperator == LogicalOperator.Or ? "LogicalOperator.Or" : "")}){CRLF}");
                                break;
                        }
                    }
                    else
                    {
                        addfilterexpression = false;
                    }
                    break;

                case QExStyleEnum.QueryByAttribute:
                    break;

                default:
                    if (ownerType == OwnersType.Sub)
                    {
                        code.Append($"{Indent(indentslevel)}new FilterExpression({(filter.FilterOperator == LogicalOperator.Or ? "LogicalOperator.Or" : "")}){CRLF}{Indent(indentslevel++)}{{{CRLF}");
                    }
                    else
                    {
                        code.Append($"{Indent(indentslevel)}{filterObjectName} ={CRLF}{Indent(indentslevel++)}{{{CRLF}");
                    }
                    break;
            }
            if (settings.IncludeComments && !string.IsNullOrWhiteSpace(code.ToString()))
            {
                code.Insert(0, comment);
            }
            var filtercode = new List<string>
            {
                GetConditionsOI(entity, filter, indentslevel),
                GetFiltersOI(entity, filter, ownerName, indentslevel, namestree)
            };
            code.Append(string.Join($"{betweenchar}{CRLF}", filtercode.Where(f => !string.IsNullOrEmpty(f))));
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                    if (addfilterexpression)
                    {
                        code.Append($"{CRLF}{Indent(--indentslevel)})");
                    }
                    break;

                case QExStyleEnum.QueryByAttribute:
                    break;

                default:
                    if (addfilterexpression)
                    {
                        code.Append($"{CRLF}{Indent(--indentslevel)}}}");
                    }
                    break;
            }
            return code.ToString();
        }

        private string GetConditionsOI(string entity, FilterExpression filter, int indentslevel)
        {
            if (filter?.Conditions?.Any() != true)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            if (settings.IncludeComments)
            {
                code.AppendLine($"{Indent(indentslevel)}// Add {filter.Conditions.Count} conditions to {entity}");
            }
            var dlabrootfiltersinline =
                settings.QExStyle == QExStyleEnum.QueryExpressionFactory &&
                // settings.QExFlavor == QExFlavorEnum.EarlyBound &&
                (filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters"));
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                case QExStyleEnum.QueryByAttribute:
                    break;

                default:
                    if (!dlabrootfiltersinline)
                    {
                        code.Append($"{Indent(indentslevel)}Conditions ={CRLF}{Indent(indentslevel++)}{{{CRLF}");
                    }
                    break;
            }
            var conditionscode = new List<string>();
            foreach (var cond in filter.Conditions)
            {
                conditionscode.Add(GetConditionOI(entity, cond, filter.FilterHint, indentslevel, dlabrootfiltersinline));
            }
            if (settings.QExStyle == QExStyleEnum.QueryByAttribute)
            {
                var conditionattributes = conditionscode.Select(c => c.Split(':')[0]).ToArray();
                var conditionvalues = conditionscode.Select(c => c.Split(':')[1].Substring(2)).ToArray();
                var codestartsize = $"{Indent(indentslevel)}Attributes = ".Length; ;
                code.Append($"{Indent(indentslevel)}Attributes = " + GetCodeParametersMaxWidth(CODE_WIDTH_LIMIT - codestartsize, indentslevel, AroundBy.Braces, conditionattributes) + $",{CRLF}");
                code.Append($"{Indent(indentslevel)}Values = " + GetCodeParametersMaxWidth(CODE_WIDTH_LIMIT - codestartsize, indentslevel, AroundBy.Braces, conditionvalues));
            }
            else
            {
                code.Append(string.Join($"{betweenchar}{CRLF}", conditionscode));
            }
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                case QExStyleEnum.QueryByAttribute:
                    break;

                default:
                    if (!dlabrootfiltersinline)
                    {
                        code.Append($"{CRLF}{Indent(--indentslevel)}}}");
                    }
                    break;
            }
            return code.ToString();
        }

        private string GetConditionOI(string entity, ConditionExpression cond, string filtername, int indentslevel, bool dlabrootfiltersinline)
        {
            if (settings.QExStyle == QExStyleEnum.QueryByAttribute && cond.Operator != ConditionOperator.Equal)
            {
                throw new Exception("Only equal conditions are supported for QueryByAttribute. Use QueryExpression if you need it.");
            }
            var filterentity = entity;
            var entityprefix = GetCodeEntityPrefix(entity);
            var entityalias = "";
            if (!string.IsNullOrWhiteSpace(cond.EntityName))
            {
                filterentity = entityaliases.FirstOrDefault(a => a.Key.Equals(cond.EntityName)).Value ?? cond.EntityName;
                entityalias = "\"" + cond.EntityName + "\", ";
            }
            var attributename = GetCodeAttribute(filterentity, cond.AttributeName, settings.QExFlavor == QExFlavorEnum.EarlyBound);
            var values = GetConditionValues(filterentity, cond, filtername);
            var condcode = string.Empty;
            switch (settings.QExStyle)
            {
                case QExStyleEnum.QueryByAttribute:
                    return $"{attributename}:{values}";

                case QExStyleEnum.FluentQueryExpression:
                    var wheretype = $"Where{cond.Operator}";
                    switch (settings.QExFlavor)
                    {
                        case QExFlavorEnum.EarlyBound:
                            var attributeprefix = GetCodeEntityPrefix(filterentity);
                            if (!string.IsNullOrWhiteSpace(entityalias))
                            {
                                var earlyentity = GetCodeEntity(filterentity);
                                return $"{Indent(indentslevel)}.{wheretype}<{earlyentity}>({entityalias}{attributeprefix} => {attributename}{values})";
                            }
                            else
                            {
                                return $"{Indent(indentslevel)}.{wheretype}({attributeprefix} => {attributename}{values})";
                            }

                        default:
                            entityalias = string.IsNullOrWhiteSpace(entityalias) ? "" : $", {entityalias}".TrimEnd().TrimEnd(',');
                            return $"{Indent(indentslevel)}.{wheretype}({attributename}{values}{entityalias})";
                    }

                default:
                    if (dlabrootfiltersinline &&
                        settings.QExStyle == QExStyleEnum.QueryExpressionFactory &&
                        string.IsNullOrWhiteSpace(entityalias) &&
                        cond.Operator == ConditionOperator.Equal && cond.Values?.Count == 1)
                    {
                        return $"{Indent(indentslevel)}{attributename}{values}";
                    }
                    else
                    {
                        return $"{Indent(indentslevel)}new ConditionExpression({entityalias}{attributename}, ConditionOperator.{cond.Operator}{values})";
                    }
            }
        }

        private string GetLinkEntitiesOI(DataCollection<LinkEntity> linkEntities, string ownerName, int indentslevel, List<string> namestree = null)
        {
            if (linkEntities?.Any() != true)
            {
                return string.Empty;
            }
            if (settings.QExStyle == QExStyleEnum.QueryByAttribute)
            {
                throw new Exception("Link Entity is not supported for QueryByAttribute. Use QueryExpression if you need it.");
            }
            var linkscode = string.Empty;
            var comment = $"{Indent(indentslevel)}// Add {linkEntities.Count} link-entity to {ownerName}{CRLF}";
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                    break;

                default:
                    linkscode += $"{Indent(indentslevel)}LinkEntities ={CRLF}{Indent(indentslevel++)}{{{CRLF}";
                    break;
            }
            if (settings.IncludeComments && !string.IsNullOrWhiteSpace(linkscode))
            {
                linkscode = comment + linkscode;
            }
            var linkcodes = new List<string>();
            foreach (var link in linkEntities)
            {
                string linkcode = GetLinkEntityOI(link, ownerName, indentslevel, ref namestree);
                linkcodes.Add(linkcode);
            }
            linkscode += string.Join($"{betweenchar}{CRLF}", linkcodes);
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                    break;

                default:
                    linkscode += $"{CRLF}{Indent(--indentslevel)}}}";
                    break;
            }
            return linkscode;
        }

        private string GetLinkEntityOI(LinkEntity link, string ownerName, int indentslevel, ref List<string> namestree)
        {
            var linkcode = string.Empty;
            var linkname = GetVarName(string.IsNullOrEmpty(link.EntityAlias) ? ownerName + "." + link.LinkToEntityName : link.EntityAlias);
            if (settings.IncludeComments)
            {
                linkcode += $"{Indent(indentslevel)}// Add link-entity {linkname}{CRLF}";
            }
            var aliascode = !string.IsNullOrWhiteSpace(link.EntityAlias) ? $"{Indent(indentslevel + 1)}EntityAlias = \"{link.EntityAlias}\"" : string.Empty;
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                    if (settings.QExFlavor == QExFlavorEnum.EarlyBound)
                    {
                        var linkshortname = GetVarName("l", false, namestree = namestree ?? new List<string>());
                        var fromprefix = GetCodeEntityPrefix(link.LinkFromEntityName);
                        var toprefix = GetCodeEntityPrefix(link.LinkToEntityName);
                        linkcode += $"{Indent(indentslevel)}.AddLink<{GetCodeEntity(link.LinkToEntityName)}>({fromprefix} => {fromprefix}.{GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName)}, {toprefix} => {toprefix}.{GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName)}, {linkshortname} => {linkshortname}";
                    }
                    else
                    {
                        linkcode += $"{Indent(indentslevel)}.AddLink(new Link({GetCodeEntity(link.LinkToEntityName)}, {GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName)}, {GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName)}, JoinOperator.{link.JoinOperator})";
                    }
                    if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                    {
                        aliascode = $"{Indent(indentslevel + 1)}.SetAlias(\"{link.EntityAlias}\")";
                    }
                    break;

                default:
                    var varstart = $"{Indent(indentslevel)}new LinkEntity";
                    linkcode += varstart +
                        GetCodeParametersMaxWidth(CODE_WIDTH_LIMIT - varstart.Length, indentslevel, AroundBy.Parentheses,
                            GetCodeEntity(link.LinkFromEntityName),
                            GetCodeEntity(link.LinkToEntityName),
                            GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName),
                            GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName),
                            $"JoinOperator.{link.JoinOperator}");
                    break;
            }
            var objinicode = new List<string>
            {
                aliascode,
                GetColumns(link.LinkToEntityName, link.Columns, linkname, OwnersType.Link, indentslevel + 1),
                GetFilter(link.LinkToEntityName, link.LinkCriteria, linkname, OwnersType.Link, indentslevel + 1),
                GetOrders(link.LinkToEntityName, link.Orders, linkname, OwnersType.Link, indentslevel + 1),
                GetLinkEntities(link.LinkEntities, linkname, indentslevel + 1, namestree)
            }.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
            if (objinicode.Any())
            {
                if (settings.QExStyle == QExStyleEnum.FluentQueryExpression)
                {
                    linkcode += CRLF;
                }
                else
                {
                    linkcode += $"{CRLF}{Indent(indentslevel++)}{{{CRLF}";
                }
                linkcode += string.Join($"{betweenchar}{CRLF}", objinicode);
                if (settings.QExStyle == QExStyleEnum.FluentQueryExpression)
                {
                    linkcode += $"{CRLF}{Indent(indentslevel)}";
                }
                else
                {
                    linkcode += $"{CRLF}{Indent(--indentslevel)}}}";
                }
            }
            if (settings.QExStyle == QExStyleEnum.FluentQueryExpression)
            {
                linkcode += $")";
            }

            return linkcode;
        }

        private string GetOrdersOI(string entityname, DataCollection<OrderExpression> orders, string ownerName, OwnersType ownerType, int indentslevel)
        {
            if (orders.Count == 0)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            var notsupported = ownerType == OwnersType.Link && settings.QExStyle == QExStyleEnum.QueryExpressionFactory;
            if (notsupported)
            {
                code.AppendLine($"Orders for link entities is currently not supported for QueryExpressionFactory.{CRLF}Please add it manually:");
            }
            else if (settings.IncludeComments)
            {
                code.AppendLine($"{Indent(indentslevel)}// Add orders to {ownerName}");
            }
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                    break;

                default:
                    code.Append($"{Indent(indentslevel)}Orders ={CRLF}{Indent(indentslevel++)}{{{CRLF}");
                    break;
            }
            var orderscode = new List<string>();
            foreach (var order in orders)
            {
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.FluentQueryExpression:
                        var desc = order.OrderType == OrderType.Descending ? "Descending" : "";
                        if (settings.QExFlavor == QExFlavorEnum.EarlyBound)
                        {
                            orderscode.Add($"{Indent(indentslevel)}.OrderBy{desc}({GetCodeEntityPrefix(entityname)} => {GetCodeAttribute(entityname, order.AttributeName, true)})");
                        }
                        else
                        {
                            orderscode.Add($"{Indent(indentslevel)}.OrderBy{desc}({GetCodeAttribute(entityname, order.AttributeName, false)})");
                        }
                        break;

                    default:
                        orderscode.Add($"{Indent(indentslevel)}new OrderExpression({GetCodeAttribute(entityname, order.AttributeName)}, OrderType.{order.OrderType})");
                        break;
                }
            }
            code.Append(string.Join($"{betweenchar}{CRLF}", orderscode));
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                    break;

                default:
                    code.Append($"{CRLF}{Indent(--indentslevel)}}}");
                    break;
            }
            if (notsupported)
            {
                return CommentLines(code.ToString());
            }
            return code.ToString();
        }

        #endregion Object Initilization

        #region Helpers

        private void StoreLinkEntityAliases(DataCollection<LinkEntity> linkEntities)
        {
            foreach (var link in linkEntities)
            {
                if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                {
                    entityaliases.Add(link.EntityAlias, link.LinkToEntityName);
                }
                StoreLinkEntityAliases(link.LinkEntities);
            }
        }

        private string GetVarName(string requestedname, bool numberit = false, List<string> list = null)
        {
            if (list == null)
            {
                list = globalVariables;
            }
            var result = requestedname.Replace(".", "_");
            if (list.Contains(result) || numberit)
            {
                var i = 1;
                while (list.Contains(result + i.ToString()))
                {
                    i++;
                }
                result += i.ToString();
            }
            list.Add(result);
            return result;
        }

        private static string Indent(int indents = 1)
        {
            return string.Concat(Enumerable.Repeat("    ", indents));
        }

        private string ReplaceValueTokens(string code)
        {
            if (!code.Contains("<<<"))
            {
                return code;
            }
            var variables = new StringBuilder();
            if (settings.IncludeComments)
            {
                variables.AppendLine("// Set Condition Values");
            }
            while (code.Contains("<<<"))
            {
                var tokenvalue = code.Substring(code.IndexOf("<<<") + 3);
                if (!tokenvalue.Contains("|") || !tokenvalue.Contains(">>>") || tokenvalue.IndexOf("|") > tokenvalue.IndexOf(">>>"))
                {
                    throw new Exception($"Unexpected value token: {tokenvalue}");
                }
                tokenvalue = tokenvalue.Substring(0, tokenvalue.IndexOf(">>>"));
                var token = tokenvalue.Split('|')[0];
                token = GetVarName(token);
                var value = tokenvalue.Split('|')[1];
                variables.AppendLine($"var {token} = {value};");
                code = code.Replace("<<<" + tokenvalue + ">>>", token);
            }
            variables.AppendLine();
            code = variables.ToString() + code;
            return code;
        }

        private List<string> GetQueryOptions(QueryExpression qex, string objectname, int indentslevel)
        {
            var queryoptions = new List<string>();
            var pre = settings.QExStyle == QExStyleEnum.FluentQueryExpression ? "." : "";
            var valuepre = settings.QExStyle == QExStyleEnum.FluentQueryExpression ? "(" : " = ";
            var valuepost = settings.QExStyle == QExStyleEnum.FluentQueryExpression ? ")" : "";
            if (qex.NoLock)
            {
                if (settings.QExStyle == QExStyleEnum.QueryByAttribute)
                {
                    throw new Exception("NoLock is not supported for QueryByAttribute. Use QueryExpression if you need it.");
                }
                queryoptions.Add($"{Indent(indentslevel)}{pre}NoLock{valuepre}true{valuepost}");
            }
            if (qex.Distinct)
            {
                if (settings.QExStyle == QExStyleEnum.QueryByAttribute)
                {
                    throw new Exception("Distinct is not supported for QueryByAttribute. Use QueryExpression if you need it.");
                }
                queryoptions.Add($"{Indent(indentslevel)}{pre}Distinct{valuepre}true{valuepost}");
            }
            if (qex.TopCount != null)
            {
                queryoptions.Add($"{Indent(indentslevel)}{pre}TopCount{valuepre}{qex.TopCount}{valuepost}");
            }
            if (!string.IsNullOrWhiteSpace(qex.PageInfo?.PagingCookie))
            {
                queryoptions.Add($"{Indent(indentslevel)}{pre}PageInfo{valuepre}new PagingInfo{CRLF}{Indent(indentslevel + 1)}{{{CRLF}{Indent(indentslevel + 2)}PageNumber = {qex.PageInfo.PageNumber},{CRLF}{Indent(indentslevel + 2)}PagingCookie = \"{qex.PageInfo.PagingCookie}\"{CRLF}{Indent(indentslevel)}}}{valuepost}");
            }
            return queryoptions;
        }

        private string GetCodeParametersMaxWidth(int maxwidth, int multilineindents, AroundBy aroundby, params string[] parameters)
        {
            var resultstart = aroundby == AroundBy.Parentheses ? "(" : aroundby == AroundBy.Braces ? "{ " : "";
            var resultend = aroundby == AroundBy.Parentheses ? ")" : aroundby == AroundBy.Braces ? " }" : "";
            var between = ", ";
            var paramsstring = string.Join("`´", parameters.Where(p => !string.IsNullOrWhiteSpace(p)));
            if (paramsstring.Length > maxwidth)
            {
                if (aroundby == AroundBy.Braces)
                {
                    resultstart = CRLF + Indent(multilineindents) + resultstart.Trim() + CRLF + Indent(multilineindents + 1);
                    resultend = CRLF + Indent(multilineindents) + resultend.Trim();
                }
                else
                {
                    resultstart = resultstart.Trim() + CRLF + Indent(multilineindents + 1);
                }
                between = $",{CRLF}{Indent(multilineindents + 1)}";
            }
            var result = resultstart + paramsstring.Replace("`´", between) + resultend;
            return result;
        }

        private EntityMetadata GetEntityMetadata(string entity) => metas.FirstOrDefault(e => e.LogicalName.Equals(entity));

        private string GetEntityName(string entityname)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity)
            {
                switch (settings.QExFlavor)
                {
                    case QExFlavorEnum.EBGconstants:
                    case QExFlavorEnum.EarlyBound:
                        return entity.SchemaName;

                    case QExFlavorEnum.LCGconstants:
                        return entity.GetEntityClass(settings.LCG_Settings);
                }
            }
            return entityname;
        }

        private string GetCodeEntity(string entityname)
        {
            var result = GetEntityName(entityname);
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata)
            {
                switch (settings.QExFlavor)
                {
                    case QExFlavorEnum.EBGconstants:
                        return result + "." + settings.EBG_EntityLogicalNames;

                    case QExFlavorEnum.LCGconstants:
                        return result + ".EntityName";

                    case QExFlavorEnum.EarlyBound:
                        return result;
                }
            }
            return "\"" + entityname + "\"";
        }

        private string GetCodeEntityPrefix(string entityname)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity)
            {
                return entity.DisplayName.UserLocalizedLabel.Label.Substring(0, 1).ToLowerInvariant();
            }
            return entityname.Substring(0, 1).ToLowerInvariant();
        }

        private AttributeMetadata GetAttributeMetadata(string entityname, string attributename)
        {
            return metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity ?
                entity.Attributes.FirstOrDefault(a => a.LogicalName.Equals(attributename)) : null;
        }

        private string GetCodeAttribute(string entityname, string attributename, bool addentityprefix = false)
        {
            if (settings.QExFlavor != QExFlavorEnum.LateBound &&
                metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity &&
                entity.Attributes.FirstOrDefault(a => a.LogicalName.Equals(attributename)) is AttributeMetadata attribute)
            {
                switch (settings.QExFlavor)
                {
                    case QExFlavorEnum.EarlyBound:
                        if (addentityprefix)
                        {
                            return GetCodeEntityPrefix(entityname) + "." + attribute.SchemaName;
                        }
                        return attribute.SchemaName;

                    case QExFlavorEnum.LCGconstants:
                        return LCG.Extensions.GetEntityClass(entity, settings.LCG_Settings) + "." + LCG.Extensions.GetAttributeProperty(attribute, settings.LCG_Settings);

                    default:
                        return entity.SchemaName + "." + settings.EBG_AttributeLogicalNameClass + attribute.SchemaName;
                }
            }
            return "\"" + attributename + "\"";
        }

        private static string GetQueryObjectName(QExStyleEnum style)
        {
            switch (style)
            {
                case QExStyleEnum.QueryExpression:
                    return "query";

                case QExStyleEnum.QueryByAttribute:
                    return "qrybyattr";

                case QExStyleEnum.OrganizationServiceContext:
                    return "ctxqry";

                case QExStyleEnum.QueryExpressionFactory:
                    return "fctqry";

                case QExStyleEnum.FluentQueryExpression:
                    return "fluqry";

                case QExStyleEnum.FetchXML:
                    return "fetch";

                default:
                    return "qry";
            }
        }

        private static string CommentLines(string codes)
        {
            return string.Join("\n", codes.Split('\n').Select(l => $"// {l}"));
        }

        private string GetConditionValues(string entity, ConditionExpression cond, string filtername)
        {
            if (cond.Values.Count == 0)
            {
                return string.Empty;
            }
            var token = filtername.Replace(".", "_").Replace("_Criteria", "").Replace("_LinkCriteria", "");
            if (!string.IsNullOrWhiteSpace(cond.EntityName))
            {
                token += "_" + cond.EntityName;
            }
            token += "_" + cond.AttributeName;

            var entitymeta = GetEntityMetadata(entity);
            var attributemeta = GetAttributeMetadata(entity, cond.AttributeName);
            var enumattr = attributemeta as EnumAttributeMetadata;
            var lcgentity = entitymeta.GetEntityClass(settings.LCG_Settings);
            var lcgoptionset = attributemeta.GetNameTechnical(settings.LCG_Settings) + "_OptionSet";
            var ebgentity = GetEntityName(entity);
            var ebgoptionset = enumattr != null ?
                enumattr.OptionSet?.IsGlobal == true ?
                    enumattr.SchemaName :
                    ebgentity + (enumattr is StateAttributeMetadata ? "State" : "_" + enumattr.SchemaName) : string.Empty;
            var strings = new List<string>();
            var i = 1;
            foreach (var value in cond.Values)
            {
                var usetoken = settings.FilterVariables;
                var valuestr = string.Empty;

                if (enumattr != null)
                {
                    var valueint = value as int?;
                    if (valueint == null)
                    {
                        valueint = int.Parse(value as string);
                    }
                    var optionsetvalue = enumattr.OptionSet.Options.FirstOrDefault(o => o.Value == valueint);
                    switch (settings.QExFlavor)
                    {
                        case QExFlavorEnum.LCGconstants:
                            var lcgenumvalue = optionsetvalue.GetOptionSetValueName(settings.LCG_Settings);
                            valuestr = $"(int){lcgentity}.{lcgoptionset}.{lcgenumvalue}";
                            usetoken = false;
                            break;

                        case QExFlavorEnum.EBGconstants:
                        case QExFlavorEnum.EarlyBound:
                            var ebgenumvalue = LCG.Extensions.StringToCSharpIdentifier(optionsetvalue.Label.UserLocalizedLabel.Label);
                            valuestr = $"(int){ebgoptionset}.{ebgenumvalue}";
                            usetoken = false;
                            break;

                        default:
                            valuestr = value.ToString();
                            break;
                    }
                }
                else if (value is string || value is Guid)
                {
                    valuestr = "\"" + value.ToString() + "\"";
                }
                else if (value is bool)
                {
                    valuestr = (bool)value ? "true" : "false";
                }
                else
                {
                    valuestr = value.ToString();
                }
                if (usetoken)
                {
                    if (cond.Values.Count == 1)
                    {
                        strings.Add($"<<<{token}|{valuestr}>>>");
                    }
                    else
                    {
                        strings.Add($"<<<{token}_{i++}|{valuestr}>>>");
                    }
                }
                else
                {
                    strings.Add(valuestr);
                }
            }
            var values = ", " + string.Join(", ", strings);
            if (cond.CompareColumns)
            {
                values = ", true" + values;
            }

            return values;
        }

        #endregion Helpers
    }

    public class QExStyle
    {
        public QExStyleEnum Tag;
        public string Creator;
        public string ClassName;
        public string HelpUrl;

        public override string ToString() => Tag.ToString();

        internal string LinkName
        {
            get
            {
                var name = Creator;
                if (!string.IsNullOrEmpty(ClassName))
                {
                    if (string.IsNullOrEmpty(name) || ClassName.ToLowerInvariant().Contains(name.ToLowerInvariant()))
                    {
                        return ClassName;
                    }
                    return name + ": " + ClassName;
                }
                return null;
            }
        }

        internal static QExStyle[] GetComboBoxItems()
        {
            return new QExStyle[]
            {
                new QExStyle
                {
                    Tag = QExStyleEnum.QueryExpression,
                    Creator = "Microsoft",
                    ClassName = "Microsoft.CrmSdk.CoreAssemblies",
                    HelpUrl = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/samples/retrieve-multiple-queryexpression-class",
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.QueryByAttribute,
                    Creator = "Microsoft",
                    ClassName = "Microsoft.CrmSdk.CoreAssemblies",
                    HelpUrl = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/samples/retrieve-multiple-querybyattribute-class",
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.FluentQueryExpression,
                    Creator = "MscrmTools",
                    ClassName = "MscrmTools.FluentQueryExpressions",
                    HelpUrl = "https://github.com/MscrmTools/MscrmTools.FluentQueryExpressions",
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.FetchXML,
                    Creator = "Microsoft",
                    ClassName = "plain fetchxml",
                    HelpUrl = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/use-fetchxml-construct-query",
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.OrganizationServiceContext,
                    Creator = "Microsoft",
                    ClassName = "Microsoft.Xrm.Sdk.Client",
                    HelpUrl = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/build-queries-with-linq-net-language-integrated-query",
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.QueryExpressionFactory,
                    Creator = "Daryl LaBar",
                    ClassName = "DLaB.Xrm",
                    HelpUrl = "https://github.com/daryllabar/DLaB.Xrm/wiki/Query-Helpers",
                }
            };
        }

        internal static string StyleClassName(QExStyleEnum style)
        {
            switch (style)
            {
                case QExStyleEnum.FluentQueryExpression:
                    return "Query";

                default:
                    return style.ToString();
            }
        }
    }

    public class QExFlavor
    {
        public string Name;
        public QExFlavorEnum Tag;
        public string Creator;
        public string HelpUrl;

        public override string ToString() => Name;

        internal static QExFlavor[] GetComboBoxItems()
        {
            return new QExFlavor[]
            {
                new QExFlavor
                {
                    Name = "Late Bound strings",
                    Tag = QExFlavorEnum.LateBound
                },
                new QExFlavor
                {
                    Name = "Late Bound EBG constants",
                    Creator = "Daryl LaBar's tool EBG",
                    HelpUrl = "https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/wiki/Early-Bound-Generator",
                    Tag = QExFlavorEnum.EBGconstants
                },
                new QExFlavor
                {
                    Name = "Late Bound LCG constants",
                    Creator = "Jonas Rapp's tool LCG",
                    HelpUrl = "https://github.com/rappen/LCG-UDG",
                    Tag = QExFlavorEnum.LCGconstants
                },
                new QExFlavor
                {
                    Name = "Early Bound",
                    Creator = "Microsoft's classes",
                    HelpUrl = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/generate-early-bound-classes",
                    Tag = QExFlavorEnum.EarlyBound
                }
            };
        }
    }

    internal enum OwnersType
    {
        Root,
        Link,
        Sub
    }

    internal enum AroundBy
    {
        Parentheses,
        Braces
    }
}