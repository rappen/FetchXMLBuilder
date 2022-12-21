using Cinteros.Xrm.FetchXmlBuilder.Converters;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    public class CSharpCodeGenerator
    {
        private List<string> varList;
        internal static string CRLF = "\r\n";
        internal QueryExpression qex;
        internal List<EntityMetadata> metas;
        internal CodeGenerators settings;
        private readonly string separators;
        internal Dictionary<string, string> entityaliases;

        public static string GetCSharpQueryExpression(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings settings)
        {
            if (settings.CodeGenerators.QExFlavor == QExFlavorEnum.LCGconstants)
            {
                throw new ArgumentOutOfRangeException("Flavor", "LCG is not yet implemented.");
            }

            switch (settings.CodeGenerators.QExStyle)
            {
                case QExStyleEnum.QueryExpression:
                    return new QExVanilla(new CSharpCodeGenerator(QEx, entities, settings)).Generated;

                case QExStyleEnum.QueryExpressionFactory:
                    return new QExFactory(new CSharpCodeGenerator(QEx, entities, settings)).Generated;

                default:
                    throw new NotImplementedException();
            }
        }

        private CSharpCodeGenerator(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings fxbsettings)
        {
            varList = new List<string>();
            entityaliases = new Dictionary<string, string>();
            metas = entities;
            qex = QEx;
            settings = fxbsettings.CodeGenerators;
            separators = settings.ObjectInitializer ? $",{CRLF}" : $"{CRLF}";
            StoreLinkEntityAliases(qex.LinkEntities);
        }

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

        internal string GetVarName(string requestedname, bool numberit = false)
        {
            var result = requestedname.Replace(".", "_");
            if (varList.Contains(result) || numberit)
            {
                var i = 1;
                while (varList.Contains(result + i.ToString()))
                {
                    i++;
                }
                result += i.ToString();
            }
            varList.Add(result);
            return result;
        }

        internal string Indent(int indents = 1)
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

        internal string GetQueryOptions(QueryExpression qex, string objectname)
        {
            var queryoptions = new List<string>();
            if (qex.NoLock)
            {
                queryoptions.Add("NoLock = true");
            }
            if (qex.Distinct)
            {
                queryoptions.Add("Distinct = true");
            }
            if (qex.TopCount != null)
            {
                queryoptions.Add("TopCount = " + qex.TopCount.ToString());
            }
            if (!string.IsNullOrWhiteSpace(qex.PageInfo?.PagingCookie))
            {
                queryoptions.Add($"PageInfo = new PagingInfo{CRLF}{Indent()}{{{CRLF}{Indent(2)}PageNumber = {qex.PageInfo.PageNumber},{CRLF}{Indent(2)}PagingCookie = \"{qex.PageInfo.PagingCookie}\"{CRLF}{Indent()}}}");
            }
            if (!queryoptions.Any())
            {
                return string.Empty;
            }
            if (settings.ObjectInitializer)
            {
                return GetCodeParametersMaxWidth(0, 1, false, queryoptions.ToArray());
            }
            return string.Join(CRLF, queryoptions.Select(o => $"{objectname}.{o};"));
        }

        internal string GetCodeParametersMaxWidth(int maxwidth, int indents, bool withnewline, params string[] parameters)
        {
            var result = string.Join("`´", parameters.Where(p => !string.IsNullOrWhiteSpace(p)));
            if (result.Length > maxwidth)
            {
                result = CRLF + Indent(indents) + result.Replace("`´", $",{CRLF}{Indent(indents)}") + (withnewline ? CRLF : "");
            }
            else
            {
                result = result.Replace("`´", ", ");
            }
            return result;
        }

        internal string GetColumns(string entity, ColumnSet columns, string LineStart, int indents)
        {
            var code = new StringBuilder();
            if (columns.AllColumns)
            {
                if (settings.IncludeComments)
                {
                    code.AppendLine("// Add all columns to " + LineStart);
                }
                if (settings.ObjectInitializer)
                {
                    switch (settings.QExFlavor)
                    {
                        case QExFlavorEnum.EarlyBound:
                            code.Append("new ColumnSet(true)");
                            break;

                        default:
                            code.Append("ColumnSet = new ColumnSet(true)");
                            break;
                    }
                }
                else
                {
                    code.AppendLine(LineStart + ".AllColumns = true;");
                }
            }
            else if (columns.Columns.Count > 0)
            {
                if (settings.IncludeComments)
                {
                    code.AppendLine("// Add columns to " + LineStart);
                }
                if (settings.ObjectInitializer)
                {
                    switch (settings.QExStyle)
                    {
                        case QExStyleEnum.QueryExpressionFactory:
                            switch (settings.QExFlavor)
                            {
                                case QExFlavorEnum.EarlyBound:
                                    LineStart = GetCodeEntityPrefix(entity) + " => new { ";
                                    break;

                                default:
                                    LineStart = $"new ColumnSet(";
                                    break;
                            }
                            break;

                        default:
                            LineStart += " = new ColumnSet(";
                            break;
                    }
                }
                else
                {
                    LineStart += ".AddColumn" + (columns.Columns.Count > 1 ? "s" : "") + "(";
                }
                var colsEB = GetCodeParametersMaxWidth(120 - LineStart.Length, indents + 1, false, columns.Columns.Select(c => GetCodeAttribute(entity, c)).ToArray());
                var muliplerows = colsEB.Contains(CRLF);
                code.Append(Indent(indents) + LineStart + colsEB);
                if (settings.ObjectInitializer)
                {
                    switch (settings.QExFlavor)
                    {
                        case QExFlavorEnum.EarlyBound:
                            code.Append(muliplerows ? $"{CRLF}{Indent(indents)}}}" : " }");
                            break;

                        default:
                            code.Append(muliplerows ? $"{CRLF}{Indent(indents)})" : ")");
                            break;
                    }
                }
                else
                {
                    code.Append(");");
                }
            }
            return code.ToString();
        }

        private string GetFilters(string entity, FilterExpression filter, string ownerName, ParentFilterType ownerType, int indentslevel)
        {
            if (filter?.Filters?.Any() != true)
            {
                return string.Empty;
            }
            var filters = filter.Filters.Where(f => f.Conditions.Any() || f.Filters.Any());
            var several = filters.Count() > 1;
            var filtercodes = new List<string>();
            var filterscode = "";
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
            if (settings.ObjectInitializer)
            {
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        if (!rootfilters)
                        {
                            filterscode = $"{Indent(indentslevel)}Filters = // Filters{CRLF}{Indent(indentslevel++)}{{{CRLF}";
                        }
                        break;

                    default:
                        filterscode = $"{Indent(indentslevel)}Filters = // Filters{CRLF}{Indent(indentslevel++)}{{{CRLF}";
                        break;
                }
            }
            foreach (var filteritem in filters)
            {
                filtercodes.Add(GetFilter(entity, filteritem, filter.FilterHint ?? ownerName, ParentFilterType.Filters, indentslevel, several));
            }
            filterscode += string.Join(separators, filtercodes.Where(f => !string.IsNullOrWhiteSpace(f)));
            if (settings.ObjectInitializer)
            {
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        if (!rootfilters)
                        {
                            filterscode += $"{CRLF}{Indent(--indentslevel)}}} // Filters";
                        }
                        break;

                    default:
                        filterscode += $"{CRLF}{Indent(--indentslevel)}}} // Filters";
                        break;
                }
            }
            return filterscode;
        }

        internal string GetFilter(string entity, FilterExpression filter, string ownerName, ParentFilterType ownerType, int indentslevel, bool several = false)
        {
            if (filter == null || (!filter.Conditions.Any() && !filter.Filters.Any()))
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            filter.FilterHint = $"{ownerName}.{ownerType}";
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
            if (!settings.ObjectInitializer)
            {
                if (ownerType == ParentFilterType.Filters)
                {
                    filter.FilterHint = GetVarName($"{ownerName}.{(filter.FilterOperator == LogicalOperator.Or ? "Or" : "And")}".Replace(".Criteria", "").Replace(".LinkCriteria", ""), several);

                    if (settings.IncludeComments)
                    {
                        code.AppendLine();
                        code.AppendLine($"// Add filter {filter.FilterHint} to {ownerName}");
                    }
                    code.AppendLine($"var {filter.FilterHint} = new FilterExpression({(filter.FilterOperator == LogicalOperator.Or ? "LogicalOperator.Or" : "")});");
                    code.AppendLine($"{ownerName}.AddFilter({filter.FilterHint});");
                }
            }
            else    // Object Initializer
            {
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        if (!rootfilters)
                        {
                            code.Append($"{Indent(indentslevel)}new FilterExpression({(filter.FilterOperator == LogicalOperator.Or ? "LogicalOperator.Or" : "")}) // Filter new{CRLF}{Indent(indentslevel++)}{{{CRLF}");
                        }
                        break;

                    default:
                        if (ownerType == ParentFilterType.Filters)
                        {
                            code.Append($"{Indent(indentslevel)}new FilterExpression({(filter.FilterOperator == LogicalOperator.Or ? "LogicalOperator.Or" : "")}) // Filter new{CRLF}{Indent(indentslevel++)}{{{CRLF}");
                        }
                        else
                        {
                            code.Append($"{Indent(indentslevel)}{ownerType} = // Filter type{CRLF}{Indent(indentslevel++)}{{{CRLF}");
                        }
                        break;
                }
            }
            var filtercode = new List<string>
            {
                GetConditions(entity, filter, indentslevel),
                GetFilters(entity, filter, ownerName, ownerType, indentslevel)
            };
            code.Append(string.Join(separators, filtercode.Where(f => !string.IsNullOrEmpty(f))));
            if (settings.ObjectInitializer)
            {
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        if (!rootfilters)
                        {
                            code.Append($"{CRLF}{Indent(--indentslevel)}}} /* Filter new */");
                        }
                        break;

                    default:
                        code.Append($"{CRLF}{Indent(--indentslevel)}}} /* Filter new */");
                        break;
                }
            }
            return code.ToString();
        }

        private string GetConditions(string entity, FilterExpression filter, int indentslevel)
        {
            if (filter?.Conditions?.Any() != true)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            if (settings.IncludeComments)
            {
                code.AppendLine();
                code.AppendLine("// Add conditions " + filter.FilterHint);
            }
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
            if (settings.ObjectInitializer)
            {
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        if (!rootfilters)
                        {
                            code.Append($"{Indent(indentslevel)}Conditions = // Filter conds{CRLF}{Indent(indentslevel++)}{{{CRLF}");
                        }
                        break;

                    default:
                        code.Append($"{Indent(indentslevel)}Conditions = // Filter conds{CRLF}{Indent(indentslevel++)}{{{CRLF}");
                        break;
                }
            }
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
                if (settings.ObjectInitializer)
                {
                    if (settings.QExStyle == QExStyleEnum.QueryExpressionFactory &&
                        string.IsNullOrWhiteSpace(entityalias) &&
                        cond.Operator == ConditionOperator.Equal && cond.Values?.Count == 1)
                    {
                        conditionscode.Add($"{Indent(indentslevel)}{attributename}{values}");
                    }
                    else
                    {
                        conditionscode.Add($"{Indent(indentslevel)}new ConditionExpression({entityalias}{attributename}, ConditionOperator.{cond.Operator}{values})");
                    }
                }
                else
                {
                    conditionscode.Add($"{filter.FilterHint}.AddCondition({entityalias}{attributename}, ConditionOperator.{cond.Operator}{values});");
                }
            }
            code.Append(string.Join(separators, conditionscode));
            if (settings.ObjectInitializer)
            {
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        if (!rootfilters)
                        {
                            code.Append($"{CRLF}{Indent(--indentslevel)}}}  /* Filter conds */");
                        }
                        break;

                    default:
                        code.Append($"{CRLF}{Indent(--indentslevel)}}}  /* Filter conds */");
                        break;
                }
            }
            return code.ToString();
        }

        internal string GetOrders(string entityname, DataCollection<OrderExpression> orders, string ownerName, int indentslevel, bool root = false)
        {
            if (orders.Count == 0)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            if (settings.IncludeComments)
            {
                code.AppendLine();
                code.AppendLine("// Add orders");
            }
            ownerName += root ? ".AddOrder(" : ".Orders.Add(new OrderExpression(";
            var LineEnd = root ? ");" : "));";
            foreach (var order in orders)
            {
                code.AppendLine(ownerName + GetCodeAttribute(entityname, order.AttributeName) + ", OrderType." + order.OrderType.ToString() + LineEnd);
            }
            return code.ToString();
        }

        internal static string GetConditionValues(DataCollection<object> values, string token, bool createvariables)
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

        internal string GetCodeEntityPrefix(string entityname)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity)
            {
                return entity.DisplayName.UserLocalizedLabel.Label.Substring(0, 1).ToLowerInvariant();
            }
            return entityname.Substring(0, 1).ToLowerInvariant();
        }

        internal string GetCodeAttribute(string entityname, string attributename)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity &&
                entity.Attributes.FirstOrDefault(a => a.LogicalName.Equals(attributename)) is AttributeMetadata attribute)
            {
                if (settings.QExFlavor == QExFlavorEnum.EBGconstants)
                {
                    return entity.SchemaName + "." + settings.EBG_AttributeLogicalNameClass + attribute.SchemaName;
                }
                else if (settings.QExFlavor == QExFlavorEnum.EarlyBound)
                {
                    return GetCodeEntityPrefix(entityname) + "." + attribute.SchemaName;
                }
            }
            return "\"" + attributename + "\"";
        }
    }

    public class QExStyle
    {
        public QExStyleEnum Tag;
        public string Creator;
        public string HelpUrl;
        public List<QExFlavor> Flavors;

        public override string ToString() => $"{Tag} ({Creator})";

        internal static object[] GetComboBoxItems()
        {
            return new object[]
            {
                new QExStyle { Tag = QExStyleEnum.QueryExpression, Creator = "Microsoft.CrmSdk.CoreAssemblies" },
                new QExStyle { Tag = QExStyleEnum.OrganizationServiceContext, Creator = "Microsoft.Xrm.Sdk.Client" },
                new QExStyle { Tag = QExStyleEnum.QueryExpressionFactory, Creator = "daryllabar/DLaB.Xrm" },
                new QExStyle { Tag = QExStyleEnum.FluentQueryExpression, Creator = "MscrmTools" },
                new QExStyle { Tag = QExStyleEnum.FetchXML, Creator="common" }
            };
        }
    }

    public class QExFlavor
    {
        public string Name;
        public QExFlavorEnum Tag;
        public string HelpUrl;

        public override string ToString() => $"{Name}";

        internal static object[] GetComboBoxItems()
        {
            return new object[]
            {
                new QExFlavor { Name = "Late Bound strings", Tag = QExFlavorEnum.LateBound },
                new QExFlavor { Name = "Late Bound EBG constants", Tag = QExFlavorEnum.EBGconstants },
                new QExFlavor { Name = "Late Bound LCG constants", Tag = QExFlavorEnum.LCGconstants },
                new QExFlavor { Name = "Early Bound", Tag = QExFlavorEnum.EarlyBound }
            };
        }
    }

    internal enum ParentFilterType
    {
        Criteria,
        LinkCriteria,
        Filters
    }
}