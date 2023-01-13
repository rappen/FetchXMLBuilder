using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    public class CSharpCodeGenerator
    {
        private List<string> globalVariables;
        internal static string CRLF = "\r\n";
        internal QueryExpression qex;
        internal List<EntityMetadata> metas;
        internal CodeGenerators settings;
        internal Dictionary<string, string> entityaliases;
        private string betweenchar = ";";

        public static string GetCSharpQueryExpression(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings settings)
        {
            if (settings.CodeGenerators.QExFlavor == QExFlavorEnum.LCGconstants)
            {
                throw new ArgumentOutOfRangeException("Flavor", "LCG is not yet implemented.");
            }
            if (settings.CodeGenerators.QExStyle == QExStyleEnum.QueryExpression &&
                settings.CodeGenerators.QExFlavor == QExFlavorEnum.EarlyBound)
            {
                throw new ArgumentOutOfRangeException("Style & Flavor", $"Combo is not possible.");
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

        internal string GetQueryCode()
        {
            var qename = GetVarName(GetQueryObjectName(settings.QExStyle));
            var queryclass = QExStyle.StyleClassName(settings.QExStyle);
            var entityname = GetCodeEntity(qex.EntityName);
            var earlybound = settings.QExFlavor == QExFlavorEnum.EarlyBound;
            var queryproperties = GetQueryOptions(qex, qename, settings.ObjectInitializer ? 1 : 0);
            if (settings.ObjectInitializer)
            {
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
                    querycode += $"{queryclass}.Create{(earlybound ? "<" : "(")}{entityname}{(earlybound ? ">(" : ",")}";
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
                        querycode += GetOrdersLbL(qex.EntityName, qex.Orders, qename, true);
                        break;

                    default:
                        querycode += $"{CRLF}}};";
                        break;
                }
            }
            else
            {
                querycode += $";{CRLF}";
                querycode += string.Join(CRLF, queryproperties.Select(p => p + $";")) + CRLF;
                querycode += GetColumnsLbL(qex.EntityName, qex.ColumnSet, qename, OwnersType.Root);
                querycode += GetFilterLbL(qex.EntityName, qex.Criteria, qename, OwnersType.Root);
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
                    GetOrdersOI(qex.EntityName, qex.Orders, qename, indentslevel),
                    GetLinkEntitiesOI(qex.LinkEntities, qename, indentslevel)
                }.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

        #endregion General

        #region Line-by Line

        internal string GetColumnsLbL(string entity, ColumnSet columns, string ownerName, OwnersType ownerType)
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
                ownerName += ".AddColumn" + (columns.Columns.Count > 1 ? "s" : "") + "(";
                var colsEB = GetCodeParametersMaxWidth(120 - ownerName.Length, 1, false, columns.Columns.Select(c => GetCodeAttribute(entity, c)).ToArray());
                code.AppendLine(ownerName + colsEB + ");");
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

        internal string GetFilterLbL(string entity, FilterExpression filter, string ownerName, OwnersType ownerType, bool several = false)
        {
            if (filter == null || (!filter.Conditions.Any() && !filter.Filters.Any()))
            {
                return string.Empty;
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
                code.AppendLine($"//{CRLF}// Add conditions {filter.FilterHint}");
            }
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
            var conditionscode = new List<string>();
            foreach (var cond in filter.Conditions)
            {
                var filterentity = entity;
                var entityalias = "";
                var values = "";
                var token = filter.FilterHint.Replace(".", "_").Replace("_Criteria", "").Replace("_LinkCriteria", "");
                if (!string.IsNullOrWhiteSpace(cond.EntityName))
                {
                    filterentity = entityaliases.FirstOrDefault(a => a.Key.Equals(cond.EntityName)).Value ?? cond.EntityName;
                    entityalias = "\"" + cond.EntityName + "\", ";
                    token += "_" + cond.EntityName;
                }
                token += "_" + cond.AttributeName;
                if (cond.Values.Count > 0)
                {
                    values = ", " + GetConditionValues(cond.Values, token, settings.FilterVariables);
                    if (cond.CompareColumns)
                    {
                        values = ", true" + values;
                    }
                }
                var attributename = GetCodeAttribute(filterentity, cond.AttributeName);
                conditionscode.Add($"{filter.FilterHint}.AddCondition({entityalias}{attributename}, ConditionOperator.{cond.Operator}{values});");
            }
            code.Append(string.Join(CRLF, conditionscode.Select(f => f.Trim()).Where(f => !string.IsNullOrWhiteSpace(f))));
            return code.ToString().TrimEnd() + CRLF;
        }

        internal string GetLinkEntitiesLbL(DataCollection<LinkEntity> linkEntities, string LineStart)
        {
            if (linkEntities?.Count == 0)
            {
                return string.Empty;
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
                var parms = GetCodeParametersMaxWidth(120 - varstart.Length - LineStart.Length, 1, true,
                GetCodeEntity(link.LinkToEntityName),
                    GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName),
                    GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName),
                    join);
                linkcode += $"{varstart}{LineStart}.AddLink({parms});{CRLF}";
                if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                {
                    linkcode += $"{linkname}.EntityAlias = \"{link.EntityAlias}\";{CRLF}";
                }
                linkcode += GetColumnsLbL(link.LinkToEntityName, link.Columns, linkname, OwnersType.Link);
                linkcode += GetFilterLbL(link.LinkToEntityName, link.LinkCriteria, linkname, OwnersType.Link);
                linkcode += GetOrdersLbL(link.LinkToEntityName, link.Orders, linkname);
                linkcode += GetLinkEntitiesLbL(link.LinkEntities, linkname);
                linkcodec.Add(linkcode);
            }
            return string.Join(CRLF, linkcodec.Select(f => f.Trim())).TrimEnd() + CRLF;
        }

        internal string GetOrdersLbL(string entityname, DataCollection<OrderExpression> orders, string ownerName, bool root = false)
        {
            if (orders.Count == 0)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            if (settings.IncludeComments)
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

        internal string GetColumnsOI(string entity, ColumnSet columns, OwnersType ownerType, int indentslevel)
        {
            var objectName = ownerType == OwnersType.Root ? "ColumnSet" : ownerType == OwnersType.Link ? "Columns" : "** WRONG **";
            var code = new StringBuilder();
            if (columns.AllColumns)
            {   // All Columns
                if (settings.IncludeComments)
                {
                    code.AppendLine($"{Indent(indentslevel)}// Add all columns to " + objectName);
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
                    code.AppendLine($"{Indent(indentslevel)}// Add columns to " + objectName);
                }
                var ini = string.Empty;
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        switch (settings.QExFlavor)
                        {
                            case QExFlavorEnum.EarlyBound:
                                ini = GetCodeEntityPrefix(entity) + " => new { ";
                                break;

                            default:
                                ini = $"new ColumnSet(";
                                break;
                        }
                        break;

                    case QExStyleEnum.FluentQueryExpression:
                        switch (settings.QExFlavor)
                        {
                            case QExFlavorEnum.EarlyBound:
                                ini = $".Select({GetCodeEntityPrefix(entity)} => new {{ ";
                                break;

                            default:
                                ini = $".Select(";
                                break;
                        }
                        break;

                    default:
                        ini = $"{objectName} = new ColumnSet(";
                        break;
                }
                var colsEB = GetCodeParametersMaxWidth(120 - ini.Length, indentslevel + 1, false, columns.Columns.Select(c => GetCodeAttribute(entity, c, settings.QExFlavor == QExFlavorEnum.EarlyBound)).ToArray());
                var muliplerows = colsEB.Contains(CRLF);
                code.Append(Indent(indentslevel) + ini + colsEB);
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.FluentQueryExpression:
                        switch (settings.QExFlavor)
                        {
                            case QExFlavorEnum.EarlyBound:
                                code.Append(muliplerows ? $"{CRLF}{Indent(indentslevel)}}}" : " }");
                                break;

                            default:
                                code.Append(muliplerows ? $"{CRLF}{Indent(indentslevel)}" : "");
                                break;
                        }
                        code.Append(")");
                        break;

                    default:
                        switch (settings.QExFlavor)
                        {
                            case QExFlavorEnum.EarlyBound:
                                code.Append(muliplerows ? $"{CRLF}{Indent(indentslevel)}}}" : " }");
                                break;

                            default:
                                code.Append(muliplerows ? $"{CRLF}{Indent(indentslevel)})" : ")");
                                break;
                        }
                        break;
                }
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
            if (settings.IncludeComments)
            {
                filterscode += ($"{Indent(indentslevel)}// Add filters to {ownerName}{CRLF}");
            }
            var filters = filter.Filters.Where(f => f.Conditions.Any() || f.Filters.Any());
            var filtercodes = new List<string>();
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
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

        internal string GetFilterOI(string entity, FilterExpression filter, string ownerName, OwnersType ownerType, int indentslevel, List<string> namestree = null)
        {
            if (filter == null || (!filter.Conditions.Any() && !filter.Filters.Any()))
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            var filterObjectName = ownerType == OwnersType.Root ? "Criteria" : ownerType == OwnersType.Link ? "LinkCriteria" : "Filters";
            filter.FilterHint = $"{ownerName}.{filterObjectName}";
            if (settings.IncludeComments)
            {
                code.AppendLine($"{Indent(indentslevel)}// Add filter {filter.FilterHint}");
            }
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
            var addfilterexpression = true;
            switch (settings.QExStyle)
            {
                case QExStyleEnum.QueryExpressionFactory:
                    if (!rootfilters || filter.FilterOperator == LogicalOperator.Or || !filter.Conditions.Any())
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
                code.AppendLine($"{Indent(indentslevel)}// Add conditions {filter.FilterHint}");
            }
            var rootfiltersinline = settings.QExStyle == QExStyleEnum.QueryExpressionFactory && (filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters"));
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                    break;

                default:
                    if (!rootfiltersinline)
                    {
                        code.Append($"{Indent(indentslevel)}Conditions ={CRLF}{Indent(indentslevel++)}{{{CRLF}");
                    }
                    break;
            }
            var conditionscode = new List<string>();
            foreach (var cond in filter.Conditions)
            {
                conditionscode.Add(GetConditionOI(entity, cond, filter.FilterHint, indentslevel, rootfiltersinline));
            }
            code.Append(string.Join($"{betweenchar}{CRLF}", conditionscode));
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                    break;

                default:
                    if (!rootfiltersinline)
                    {
                        code.Append($"{CRLF}{Indent(--indentslevel)}}}");
                    }
                    break;
            }
            return code.ToString();
        }

        private string GetConditionOI(string entity, ConditionExpression cond, string filtername, int indentslevel, bool rootfiltersinline)
        {
            var filterentity = entity;
            var entityprefix = GetCodeEntityPrefix(entity);
            var entityalias = "";
            var values = "";
            var token = filtername.Replace(".", "_").Replace("_Criteria", "").Replace("_LinkCriteria", "");
            if (!string.IsNullOrWhiteSpace(cond.EntityName))
            {
                filterentity = entityaliases.FirstOrDefault(a => a.Key.Equals(cond.EntityName)).Value ?? cond.EntityName;
                entityalias = "\"" + cond.EntityName + "\", ";
                token += "_" + cond.EntityName;
            }
            token += "_" + cond.AttributeName;
            if (cond.Values.Count > 0)
            {
                values = ", " + GetConditionValues(cond.Values, token, settings.FilterVariables);
                if (cond.CompareColumns)
                {
                    values = ", true" + values;
                }
            }
            var attributename = GetCodeAttribute(filterentity, cond.AttributeName, settings.QExFlavor == QExFlavorEnum.EarlyBound);
            var condcode = string.Empty;
            switch (settings.QExStyle)
            {
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
                    if (!rootfiltersinline &&
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

        internal string GetLinkEntitiesOI(DataCollection<LinkEntity> linkEntities, string ownerName, int indentslevel, List<string> namestree = null)
        {
            if (linkEntities?.Any() != true)
            {
                return string.Empty;
            }
            var linkscode = string.Empty;
            if (settings.IncludeComments)
            {
                linkscode += $"{Indent(indentslevel)}// Add {linkEntities.Count} link-entity to {ownerName}{CRLF}";
            }
            switch (settings.QExStyle)
            {
                case QExStyleEnum.FluentQueryExpression:
                    break;

                default:
                    linkscode += $"{Indent(indentslevel)}LinkEntities ={CRLF}{Indent(indentslevel++)}{{{CRLF}";
                    break;
            }
            var linkcodes = new List<string>();
            foreach (var link in linkEntities)
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
                            var toprefix = GetCodeEntityPrefix(link.LinkFromEntityName);
                            linkcode += $"{Indent(indentslevel)}.AddLink<{GetCodeEntity(link.LinkToEntityName)}>({fromprefix} => {fromprefix}.{GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName)}, {toprefix} => {toprefix}.{GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName)}, {linkshortname} => {linkshortname}";
                        }
                        else
                        {
                            linkcode += $"{Indent(indentslevel)}.AddLink(new Link({GetCodeEntity(link.LinkToEntityName)}, {GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName)}, {GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName)}, JoinOperator.{link.JoinOperator})";
                        }
                        linkcode += CRLF;
                        if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                        {
                            aliascode = $"{Indent(indentslevel + 1)}.SetAlias(\"{link.EntityAlias}\")";
                        }
                        break;

                    default:
                        var varstart = $"{Indent(indentslevel)}new LinkEntity(";
                        linkcode += varstart +
                            GetCodeParametersMaxWidth(120 - varstart.Length, indentslevel, false,
                                GetCodeEntity(link.LinkFromEntityName),
                                GetCodeEntity(link.LinkToEntityName),
                                GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName),
                                GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName),
                                $"JoinOperator.{link.JoinOperator}") + ")";
                        break;
                }
                var objinicode = new List<string>
                {
                    aliascode,
                    GetColumnsOI(link.LinkToEntityName, link.Columns, OwnersType.Link, indentslevel + 1),
                    GetFilterOI(link.LinkToEntityName, link.LinkCriteria, linkname, OwnersType.Link, indentslevel + 1),
                    GetOrdersOI(link.LinkToEntityName, link.Orders, linkname, indentslevel + 1),
                    GetLinkEntitiesOI(link.LinkEntities, linkname, indentslevel + 1, namestree)
                }.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
                if (objinicode.Any())
                {
                    if (settings.QExStyle != QExStyleEnum.FluentQueryExpression)
                    {
                        linkcode += $"{CRLF}{Indent(indentslevel++)}{{{CRLF}";
                    }
                    linkcode += string.Join($"{betweenchar}{CRLF}", objinicode);
                    if (settings.QExStyle == QExStyleEnum.FluentQueryExpression)
                    {
                        linkcode += $"{CRLF}{Indent(indentslevel)})";
                    }
                    else
                    {
                        linkcode += $"{CRLF}{Indent(--indentslevel)}}}";
                    }
                }
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

        internal string GetOrdersOI(string entityname, DataCollection<OrderExpression> orders, string ownerName, int indentslevel)
        {
            if (orders.Count == 0)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            if (settings.IncludeComments)
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

        internal string GetVarName(string requestedname, bool numberit = false, List<string> list = null)
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

        internal string ReplaceValueTokens(string code)
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

        internal List<string> GetQueryOptions(QueryExpression qex, string objectname, int indentslevel)
        {
            var queryoptions = new List<string>();
            var pre = settings.QExStyle == QExStyleEnum.FluentQueryExpression ? "." : "";
            var valuepre = settings.QExStyle == QExStyleEnum.FluentQueryExpression ? "(" : " = ";
            var valuepost = settings.QExStyle == QExStyleEnum.FluentQueryExpression ? ")" : "";
            if (qex.NoLock)
            {
                queryoptions.Add($"{Indent(indentslevel)}{pre}NoLock{valuepre}true{valuepost}");
            }
            if (qex.Distinct)
            {
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

        internal string GetCodeParametersMaxWidth(int maxwidth, int multilineindents, bool multilinenewline, params string[] parameters)
        {
            var result = string.Join("`´", parameters.Where(p => !string.IsNullOrWhiteSpace(p)));
            if (result.Length > maxwidth)
            {
                result = CRLF + Indent(multilineindents) + result.Replace("`´", $",{CRLF}{Indent(multilineindents)}") + (multilinenewline ? CRLF : "");
            }
            else
            {
                result = result.Replace("`´", ", ");
            }
            return result;
        }

        private static string GetConditionValues(DataCollection<object> values, string token, bool createvariables)
        {
            var strings = new List<string>();
            var i = 1;
            foreach (var value in values)
            {
                var valuestr = string.Empty;
                if (value is string || value is Guid)
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
                if (createvariables)
                {
                    if (values.Count == 1)
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
            return string.Join(", ", strings);
        }

        internal string GetCodeEntity(string entityname)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity)
            {
                if (settings.QExFlavor == QExFlavorEnum.EBGconstants)
                {
                    return entity.SchemaName + "." + settings.EBG_EntityLogicalNames;
                }
                else if (settings.QExFlavor == QExFlavorEnum.EarlyBound)
                {
                    return entity.SchemaName;
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

        internal string GetCodeAttribute(string entityname, string attributename, bool addentityprefix = false)
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
                    Tag = QExStyleEnum.OrganizationServiceContext,
                    Creator = "Microsoft",
                    ClassName = "Microsoft.Xrm.Sdk.Client",
                    HelpUrl = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/build-queries-with-linq-net-language-integrated-query",
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.QueryExpressionFactory,
                    Creator = "Daryl LaBar",
                    ClassName = "daryllabar/DLaB.Xrm",
                    HelpUrl = "https://github.com/daryllabar/DLaB.Xrm/wiki/Query-Helpers",
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
                    ClassName = "<plain fetchxml>",
                    HelpUrl = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/use-fetchxml-construct-query",
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
                    Creator = "Daryl LaBar",
                    HelpUrl = "https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/wiki/Early-Bound-Generator",
                    Tag = QExFlavorEnum.EBGconstants
                },
                new QExFlavor
                {
                    Name = "Late Bound LCG constants",
                    Creator = "Jonas Rapp",
                    HelpUrl = "https://github.com/rappen/LCG-UDG",
                    Tag = QExFlavorEnum.LCGconstants
                },
                new QExFlavor
                {
                    Name = "Early Bound",
                    Creator = "Microsoft",
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
}