using Microsoft.CSharp;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Settings;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    public class QueryExpressionCodeGenerator
    {
        private const string CRLF = "\r\n";
        private const string Indent = "    ";
        private List<string> varList;
        private QueryExpression qex;
        private List<EntityMetadata> metas;
        private Dictionary<string, string> entityaliases;
        private FXBSettings settings;

        private delegate QueryExpression queryExpressionCompiler();

        public static string GetCSharpQueryExpression(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings settings)
        {
            return new QueryExpressionCodeGenerator
            {
                metas = entities,
                qex = QEx,
                settings = settings
            }.CreateCSharpQueryExpression();
        }

        public static string GetFetchXmlFromCSharpQueryExpression(string query, IOrganizationService organizationService)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            parameters.ReferencedAssemblies.Add("Microsoft.Xrm.Sdk.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            CompilerResults compilerResults = provider.CompileAssemblyFromSource(parameters, GetQueryExpressionFromScript(query));

            if (compilerResults.Errors.HasErrors)
            {
                StringBuilder sbuilder = new StringBuilder();
                foreach (CompilerError compilerError in compilerResults.Errors)
                {
                    sbuilder.AppendLine($"Error ({compilerError.ErrorNumber}): {compilerError.ErrorText}");
                }
                throw new InvalidOperationException(sbuilder.ToString());
            }

            QueryExpression queryExpression =
                ((queryExpressionCompiler)Delegate.CreateDelegate(typeof(queryExpressionCompiler),
                    compilerResults.CompiledAssembly.GetType("DynamicContentGenerator.Generator"), "Generate"))();

            return organizationService.QueryExpressionToFetchXml(queryExpression);
        }

        private string CreateCSharpQueryExpression()
        {
            varList = new List<string>();
            entityaliases = new Dictionary<string, string>();
            var code = new StringBuilder();
            var qename = GetVarName("query");
            code.Append(GetQueryCodeStart(qename));
            code.Append(GetQueryOptions());
            code.Append(GetQueryCodeEnd());
            code.Append(GetColumns(qex.EntityName, qex.ColumnSet, qename + ".ColumnSet"));
            code.Append(GetFilter(qex.EntityName, qex.Criteria, qename, "Criteria"));
            code.Append(GetOrders(qex.EntityName, qex.Orders, qename, true));
            code.Append(GetLinkEntities(qex.LinkEntities, qename));
            var codestr = ReplaceValueTokens(code.ToString());
            return codestr;
        }

        private string GetQueryCodeStart(string qename)
        {
            var querycode = string.Empty;
            if (settings.CodeGenerators.IncludeComments)
            {
                querycode += "// Instantiate QueryExpression " + qename + CRLF;
            }
            switch (settings.CodeGenerators.Style)
            {
                case CodeGenerationStyle.QueryExpressionFactory:
                    querycode += "var " + qename + " = QueryExpressionFactory.Create<" + GetCodeEntity(qex.EntityName) + ">(";
                    break;

                default:
                    querycode += "var " + qename + " = new QueryExpression(" + GetCodeEntity(qex.EntityName) + ")";
                    break;
            }

            return querycode;
        }

        private string GetQueryCodeEnd()
        {
            switch (settings.CodeGenerators.Style)
            {
                case CodeGenerationStyle.QueryExpressionFactory:
                    return ");" + CRLF;
                default:
                    return ";" + CRLF;
            }
        }

        private string GetQueryOptions()
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
                queryoptions.Add($"PageInfo = new PagingInfo{CRLF}{Indent}{{{CRLF}{Indent}{Indent}PageNumber = {qex.PageInfo.PageNumber},{CRLF}{Indent}{Indent}PagingCookie = \"{qex.PageInfo.PagingCookie}\"{CRLF}{Indent}}}");
            }
            if (queryoptions.Count > 0)
            {
                return CRLF + "{" + GetCodeParametersMaxWidth(0, queryoptions.ToArray()) + "}";
            }
            return string.Empty;
        }

        private string GetVarName(string requestedname)
        {
            var result = requestedname;
            if (varList.Contains(result))
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

        private string GetColumns(string entity, ColumnSet columns, string LineStart)
        {
            var code = new StringBuilder();
            if (columns.AllColumns)
            {
                code.AppendLine();
                if (settings.CodeGenerators.IncludeComments)
                {
                    code.AppendLine("// Add all columns to " + LineStart);
                }
                code.AppendLine(LineStart + ".AllColumns = true;");
            }
            else if (columns.Columns.Count > 0)
            {
                if (settings.CodeGenerators.IncludeComments)
                {
                    code.AppendLine();
                    code.AppendLine("// Add columns to " + LineStart);
                }
                LineStart =
                    settings.CodeGenerators.Style == CodeGenerationStyle.QueryExpressionFactory ?
                        GetCodeEntityPrefix(entity) + " => new { " :
                        LineStart + ".AddColumn" + (columns.Columns.Count > 1 ? "s" : "") + "(";
                var LineEnd = settings.CodeGenerators.Style == CodeGenerationStyle.QueryExpressionFactory ? " }" : ");";
                var cols = GetCodeParametersMaxWidth(120 - LineStart.Length, columns.Columns.Select(c => GetCodeAttribute(entity, c)).ToArray());
                code.AppendLine(LineStart + cols + LineEnd);
            }
            return code.ToString();
        }

        private string GetLinkEntities(DataCollection<LinkEntity> linkEntities, string LineStart)
        {
            if (linkEntities?.Count == 0)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            foreach (var link in linkEntities)
            {
                var linkname = GetVarName(string.IsNullOrEmpty(link.EntityAlias) ? LineStart + "_" + link.LinkToEntityName : link.EntityAlias);
                code.AppendLine();
                if (settings.CodeGenerators.IncludeComments)
                {
                    code.AppendLine("// Add link-entity " + linkname);
                }
                var join = link.JoinOperator == JoinOperator.Inner ? "" : "JoinOperator." + link.JoinOperator.ToString();
                var varstart =
                    link.LinkEntities.Count > 0 ||
                    link.Columns.Columns.Count > 0 ||
                    link.LinkCriteria.Conditions.Count > 0 ||
                    link.Orders.Count > 0 ? $"var {linkname} = " : String.Empty;
                var parms = GetCodeParametersMaxWidth(120 - varstart.Length - LineStart.Length,
                        GetCodeEntity(link.LinkToEntityName),
                        GetCodeAttribute(link.LinkFromEntityName,
                        link.LinkFromAttributeName),
                        GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName),
                        join);
                code.AppendLine($"{varstart}{LineStart}.AddLink({parms});");
                if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                {
                    entityaliases.Add(link.EntityAlias, link.LinkToEntityName);
                    code.AppendLine(linkname + ".EntityAlias = \"" + link.EntityAlias + "\";");
                }
                code.Append(GetColumns(link.LinkToEntityName, link.Columns, linkname + ".Columns"));
                code.Append(GetFilter(link.LinkToEntityName, link.LinkCriteria, linkname, "LinkCriteria"));
                code.Append(GetOrders(link.LinkToEntityName, link.Orders, linkname));
                code.Append(GetLinkEntities(link.LinkEntities, linkname));
            }
            return code.ToString();
        }

        private string GetFilter(string entity, FilterExpression filterExpression, string parentName, string property)
        {
            var LineStart = parentName + (!string.IsNullOrEmpty(property) ? "." + property : "");
            var code = new StringBuilder();
            if (filterExpression.FilterOperator == LogicalOperator.Or || filterExpression.Conditions.Count > 0 || filterExpression.Filters.Count > 0)
            {
                if (settings.CodeGenerators.IncludeComments)
                {
                    code.AppendLine();
                    code.AppendLine("// Add filter " + LineStart);
                }
                if (filterExpression.FilterOperator == LogicalOperator.Or)
                {
                    code.AppendLine(LineStart + ".FilterOperator = LogicalOperator.Or;");
                }
                foreach (var cond in filterExpression.Conditions)
                {
                    var filterentity = entity;
                    var entityalias = "";
                    var values = "";
                    var token = LineStart.Replace(".", "_").Replace("_Criteria", "").Replace("_LinkCriteria", "");
                    if (!string.IsNullOrWhiteSpace(cond.EntityName))
                    {
                        filterentity = entityaliases.FirstOrDefault(a => a.Key.Equals(cond.EntityName)).Value ?? cond.EntityName;
                        entityalias = "\"" + cond.EntityName + "\", ";
                        token += "_" + cond.EntityName;
                    }
                    token += "_" + cond.AttributeName;
                    if (cond.Values.Count > 0)
                    {
                        values = ", " + GetConditionValues(cond.Values, token);

                        if (cond.CompareColumns)
                        {
                            values = ", true" + values;
                        }
                    }
                    code.AppendLine($"{LineStart}.AddCondition({entityalias}{GetCodeAttribute(filterentity, cond.AttributeName)}, ConditionOperator.{cond.Operator}{values});");
                }
                var i = 0;
                foreach (var subfilter in filterExpression.Filters)
                {
                    var filtername = GetVarName(LineStart.Replace(".", "_") + "_" + i.ToString());
                    code.AppendLine($"var {filtername} = new FilterExpression();");
                    code.AppendLine($"{LineStart}.AddFilter({filtername});");
                    code.Append(GetFilter(entity, subfilter, filtername, null));
                    i++;
                }
            }
            return code.ToString();
        }

        private string GetOrders(string entityname, DataCollection<OrderExpression> orders, string LineStart, bool root = false)
        {
            if (orders.Count == 0)
            {
                return string.Empty;
            }
            var code = new StringBuilder();
            if (settings.CodeGenerators.IncludeComments)
            {
                code.AppendLine();
                code.AppendLine("// Add orders");
            }
            LineStart += root ? ".AddOrder(" : ".Orders.Add(new OrderExpression(";
            var LineEnd = root ? ");" : "));";
            foreach (var order in orders)
            {
                code.AppendLine(LineStart + GetCodeAttribute(entityname, order.AttributeName) + ", OrderType." + order.OrderType.ToString() + LineEnd);
            }
            return code.ToString();
        }

        private string ReplaceValueTokens(string code)
        {
            if (!code.Contains("<<<"))
            {
                return code;
            }
            var variables = new StringBuilder();
            if (settings.CodeGenerators.IncludeComments)
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

        private string GetCodeEntity(string entityname)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity)
            {
                switch (settings.CodeGenerators.Style)
                {
                    case CodeGenerationStyle.EarlyBoundEBG:
                        return entity.SchemaName + "." + settings.CodeGenerators.EBG_EntityLogicalNames;

                    case CodeGenerationStyle.QueryExpressionFactory:
                        return entity.SchemaName;
                }
            }
            return "\"" + entityname + "\"";
        }

        private string GetCodeEntityPrefix(string entityname)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity)
            {
                switch (settings.CodeGenerators.Style)
                {
                    case CodeGenerationStyle.EarlyBoundEBG:
                    case CodeGenerationStyle.QueryExpressionFactory:
                        return entity.DisplayName.UserLocalizedLabel.Label.Substring(0, 1).ToLowerInvariant();
                }
            }
            return entityname.Substring(0, 1).ToLowerInvariant();
        }

        private string GetCodeAttribute(string entityname, string attributename)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity &&
                entity.Attributes.FirstOrDefault(a => a.LogicalName.Equals(attributename)) is AttributeMetadata attribute)
            {
                switch (settings.CodeGenerators.Style)
                {
                    case CodeGenerationStyle.EarlyBoundEBG:
                        return entity.SchemaName + "." + settings.CodeGenerators.EBG_AttributeLogicalNameClass + attribute.SchemaName;

                    case CodeGenerationStyle.QueryExpressionFactory:
                        return GetCodeEntityPrefix(entityname) + "." + attribute.SchemaName;
                }
            }
            return "\"" + attributename + "\"";
        }

        private static string GetCodeParametersMaxWidth(int maxwidth, params string[] parameters)
        {
            var result = string.Join("`´", parameters.Where(p => !string.IsNullOrWhiteSpace(p)));
            if (result.Length > maxwidth)
            {
                result = CRLF + Indent + result.Replace("`´", $",{CRLF}{Indent}") + CRLF;
            }
            else
            {
                result = result.Replace("`´", ", ");
            }
            return result;
        }

        private static string GetConditionValues(DataCollection<object> values, string token)
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
                if (values.Count == 1)
                {
                    strings.Add($"<<<{token}|{valuestr}>>>");
                }
                else
                {
                    strings.Add($"<<<{token}_{i++}|{valuestr}>>>");
                }
            }
            return string.Join(", ", strings);
        }

        private static string GetQueryExpressionFromScript(string query)
        {
            Regex varMatcher = new Regex(@"(var|QueryExpression)\W+([^\W]+)\W*=\W*new QueryExpression\W*\(");
            Match match = varMatcher.Match(query);

            if (match.Success)
            {
                return $@"
                    using System;
                    using Microsoft.Xrm.Sdk;
                    using Microsoft.Xrm.Sdk.Query;

                    namespace DynamicContentGenerator
                    {{
                        public class Generator
                        {{
                            public static QueryExpression Generate()
                            {{
                                {query}
                                return {match.Groups[2].Value};
                            }}
                        }}
                    }}";
            }
            else
            {
                throw new Exception("Could not determine QueryExpression variable.");
            }
        }
    }
}