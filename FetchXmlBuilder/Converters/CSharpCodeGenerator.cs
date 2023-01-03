using Cinteros.Xrm.FetchXmlBuilder.Converters;
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
        private List<string> varList;
        internal static string CRLF = "\r\n";
        internal QueryExpression qex;
        internal List<EntityMetadata> metas;
        internal CodeGenerators settings;
        internal Dictionary<string, string> entityaliases;

        public static string GetCSharpQueryExpression(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings settings)
        {
            if (settings.CodeGenerators.QExFlavor == QExFlavorEnum.LCGconstants)
            {
                throw new ArgumentOutOfRangeException("Flavor", "LCG is not yet implemented.");
            }
            var result = string.Empty;
            var coder = new CSharpCodeGenerator(QEx, entities, settings);
            switch (settings.CodeGenerators.QExStyle)
            {
                case QExStyleEnum.QueryExpression:
                    result = new QExVanilla(coder).Generated;
                    break;

                case QExStyleEnum.QueryExpressionFactory:
                    result = new QExFactory(coder).Generated;
                    break;

                default:
                    throw new NotImplementedException();
            }
            result = string.Join("\n", result.Split('\n').Select(l => Indent(settings.CodeGenerators.Indents) + l));
            return result;
        }

        private CSharpCodeGenerator(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings fxbsettings)
        {
            varList = new List<string>();
            entityaliases = new Dictionary<string, string>();
            metas = entities;
            qex = QEx;
            settings = fxbsettings.CodeGenerators;
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

        internal static string Indent(int indents = 1)
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

        internal string GetColumnsLbL(string entity, ColumnSet columns, string ownername)
        {
            var code = new StringBuilder();
            if (columns.AllColumns)
            {   // All Columns
                if (settings.IncludeComments)
                {
                    code.AppendLine("// Add all columns to " + ownername);
                }
                code.AppendLine(ownername + ".AllColumns = true;");
            }
            else if (columns.Columns.Count > 0)
            {   // Selected Columns
                if (settings.IncludeComments)
                {
                    code.AppendLine("// Add columns to " + ownername);
                }
                ownername += ".AddColumn" + (columns.Columns.Count > 1 ? "s" : "") + "(";
                var colsEB = GetCodeParametersMaxWidth(120 - ownername.Length, 1, false, columns.Columns.Select(c => GetCodeAttribute(entity, c)).ToArray());
                code.Append(ownername + colsEB + ");");
            }
            return code.ToString();
        }

        private string GetFiltersLbL(string entity, FilterExpression filter, string ownerName, ParentFilterType ownerType)
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
                filtercodes.Add(GetFilterLbL(entity, filteritem, filter.FilterHint ?? ownerName, ParentFilterType.Filters, several));
            }
            return string.Join(CRLF, filtercodes.Where(f => !string.IsNullOrWhiteSpace(f)));
        }

        internal string GetFilterLbL(string entity, FilterExpression filter, string ownerName, ParentFilterType ownerType, bool several = false)
        {
            if (filter == null || (!filter.Conditions.Any() && !filter.Filters.Any()))
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            filter.FilterHint = $"{ownerName}.{ownerType}";
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
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
            var filtercode = new List<string>
            {
                GetConditionsLbL(entity, filter),
                GetFiltersLbL(entity, filter, ownerName, ownerType)
            };
            code.Append(string.Join(CRLF, filtercode.Where(f => !string.IsNullOrEmpty(f))));
            return code.ToString();
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
                code.AppendLine();
                code.AppendLine("// Add conditions " + filter.FilterHint);
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
            code.Append(string.Join(CRLF, conditionscode));
            return code.ToString();
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

        internal string GetColumnsOI(string entity, ColumnSet columns, string ownername, int indentslevel)
        {
            var code = new StringBuilder();
            if (columns.AllColumns)
            {   // All Columns
                if (settings.IncludeComments)
                {
                    code.AppendLine($"{Indent(indentslevel)}// Add all columns to " + ownername);
                }
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
            else if (columns.Columns.Count > 0)
            {   // Selected Columns
                if (settings.IncludeComments)
                {
                    code.AppendLine($"{Indent(indentslevel)}// Add columns to " + ownername);
                }
                var prefix = string.Empty;
                switch (settings.QExStyle)
                {
                    case QExStyleEnum.QueryExpressionFactory:
                        switch (settings.QExFlavor)
                        {
                            case QExFlavorEnum.EarlyBound:
                                prefix = GetCodeEntityPrefix(entity) + " => new { ";
                                break;

                            default:
                                prefix = $"new ColumnSet(";
                                break;
                        }
                        break;

                    default:
                        prefix = $"{ownername} = new ColumnSet(";
                        break;
                }
                var colsEB = GetCodeParametersMaxWidth(120 - prefix.Length, indentslevel + 1, false, columns.Columns.Select(c => GetCodeAttribute(entity, c, settings.QExFlavor == QExFlavorEnum.EarlyBound)).ToArray());
                var muliplerows = colsEB.Contains(CRLF);
                code.Append(Indent(indentslevel) + prefix + colsEB);
                switch (settings.QExFlavor)
                {
                    case QExFlavorEnum.EarlyBound:
                        code.Append(muliplerows ? $"{CRLF}{Indent(indentslevel)}}}" : " }");
                        break;

                    default:
                        code.Append(muliplerows ? $"{CRLF}{Indent(indentslevel)})" : ")");
                        break;
                }
            }
            return code.ToString();
        }

        private string GetFiltersOI(string entity, FilterExpression filter, string ownerName, ParentFilterType ownerType, int indentslevel)
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
            var several = filters.Count() > 1;
            var filtercodes = new List<string>();
            var rootfilters = filter.FilterHint.EndsWith("Criteria") || filter.FilterHint.EndsWith("Criteria.Filters");
            switch (settings.QExStyle)
            {
                case QExStyleEnum.QueryExpressionFactory:
                    if (!rootfilters)
                    {
                        filterscode += $"{Indent(indentslevel)}Filters ={CRLF}{Indent(indentslevel++)}{{{CRLF}";
                    }
                    break;

                default:
                    filterscode += $"{Indent(indentslevel)}Filters ={CRLF}{Indent(indentslevel++)}{{{CRLF}";
                    break;
            }
            foreach (var filteritem in filters)
            {
                filtercodes.Add(GetFilterOI(entity, filteritem, filter.FilterHint ?? ownerName, ParentFilterType.Filters, indentslevel, several));
            }
            filterscode += string.Join($",{CRLF}", filtercodes.Where(f => !string.IsNullOrWhiteSpace(f)));
            switch (settings.QExStyle)
            {
                case QExStyleEnum.QueryExpressionFactory:
                    if (!rootfilters)
                    {
                        filterscode += $"{CRLF}{Indent(--indentslevel)}}}";
                    }
                    break;

                default:
                    filterscode += $"{CRLF}{Indent(--indentslevel)}}}";
                    break;
            }
            return filterscode;
        }

        internal string GetFilterOI(string entity, FilterExpression filter, string ownerName, ParentFilterType ownerType, int indentslevel, bool several = false)
        {
            if (filter == null || (!filter.Conditions.Any() && !filter.Filters.Any()))
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            filter.FilterHint = $"{ownerName}.{ownerType}";
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

                default:
                    if (ownerType == ParentFilterType.Filters)
                    {
                        code.Append($"{Indent(indentslevel)}new FilterExpression({(filter.FilterOperator == LogicalOperator.Or ? "LogicalOperator.Or" : "")}){CRLF}{Indent(indentslevel++)}{{{CRLF}");
                    }
                    else
                    {
                        code.Append($"{Indent(indentslevel)}{ownerType} ={CRLF}{Indent(indentslevel++)}{{{CRLF}");
                    }
                    break;
            }
            var filtercode = new List<string>
            {
                GetConditionsOI(entity, filter, indentslevel),
                GetFiltersOI(entity, filter, ownerName, ownerType, indentslevel)
            };
            code.Append(string.Join($",{CRLF}", filtercode.Where(f => !string.IsNullOrEmpty(f))));
            switch (settings.QExStyle)
            {
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
                if (rootfiltersinline &&
                    settings.QExStyle == QExStyleEnum.QueryExpressionFactory &&
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
            code.Append(string.Join($",{CRLF}", conditionscode));
            switch (settings.QExStyle)
            {
                default:
                    if (!rootfiltersinline)
                    {
                        code.Append($"{CRLF}{Indent(--indentslevel)}}}");
                    }
                    break;
            }
            return code.ToString();
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
                default:
                    code.Append($"{Indent(indentslevel)}Orders ={CRLF}{Indent(indentslevel++)}{{{CRLF}");
                    break;
            }
            var orderscode = new List<string>();
            foreach (var order in orders)
            {
                orderscode.Add($"{Indent(indentslevel)}new OrderExpression({GetCodeAttribute(entityname, order.AttributeName)}, OrderType.{order.OrderType})");
            }
            code.Append(string.Join($",{CRLF}", orderscode));
            switch (settings.QExStyle)
            {
                default:
                    code.Append($"{CRLF}{Indent(--indentslevel)}}}");
                    break;
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

        internal string GetCodeAttribute(string entityname, string attributename, bool prefix = false)
        {
            if (settings.QExFlavor != QExFlavorEnum.LateBound &&
                metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity &&
                entity.Attributes.FirstOrDefault(a => a.LogicalName.Equals(attributename)) is AttributeMetadata attribute)
            {
                if (prefix)
                {
                    return GetCodeEntityPrefix(entityname) + "." + attribute.SchemaName;
                }
                else
                {
                    switch (settings.QExFlavor)
                    {
                        default:
                            return entity.SchemaName + "." + settings.EBG_AttributeLogicalNameClass + attribute.SchemaName;
                    }
                }
            }
            return "\"" + attributename + "\"";
        }
    }

    public class QExStyle
    {
        public QExStyleEnum Tag;
        public string Creator;
        public string ClassName;
        public string HelpUrl;
        public List<QExFlavor> Flavors;

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

        internal static object[] GetComboBoxItems()
        {
            return new object[]
            {
                new QExStyle
                {
                    Tag = QExStyleEnum.QueryExpression,
                    Creator = "Microsoft",
                    ClassName = "Microsoft.CrmSdk.CoreAssemblies",
                    HelpUrl="https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/samples/retrieve-multiple-queryexpression-class"
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.OrganizationServiceContext,
                    Creator = "Microsoft",
                    ClassName = "Microsoft.Xrm.Sdk.Client",
                    HelpUrl="https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/organizationservicecontext"
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.QueryExpressionFactory,
                    Creator = "Daryl LaBar",
                    ClassName = "daryllabar/DLaB.Xrm",
                    HelpUrl="https://github.com/daryllabar/DLaB.Xrm"
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.FluentQueryExpression,
                    Creator = "Tanguy Touzard",
                    ClassName = "MscrmTools.FluentQueryExpressions",
                    HelpUrl="https://github.com/MscrmTools/MscrmTools.FluentQueryExpressions"
                },
                new QExStyle
                {
                    Tag = QExStyleEnum.FetchXML,
                    Creator = "Microsoft",
                    ClassName = "<plain fetchxml>",
                    HelpUrl="https://learn.microsoft.com/en-us/power-apps/developer/data-platform/use-fetchxml-construct-query"
                }
            };
        }
    }

    public class QExFlavor
    {
        public string Name;
        public QExFlavorEnum Tag;
        public string Creator;
        public string HelpUrl;

        public override string ToString() => Name;

        internal static object[] GetComboBoxItems()
        {
            return new object[]
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

    internal enum ParentFilterType
    {
        Criteria,
        LinkCriteria,
        Filters
    }
}