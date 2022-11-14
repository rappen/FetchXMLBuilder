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
            if (settings.CodeGenerators.IncludeComments)
            {
                code.AppendLine("// Instantiate QueryExpression " + qename);
            }
            code.AppendLine("var " + qename + " = new QueryExpression(" + GetCodeEntity(qex.EntityName) + ");");
            if (qex.NoLock)
            {
                code.AppendLine(qename + ".NoLock = true;");
            }
            if (qex.Distinct)
            {
                code.AppendLine(qename + ".Distinct = true;");
            }
            if (qex.TopCount != null)
            {
                code.AppendLine(qename + ".TopCount = " + qex.TopCount.ToString() + ";");
            }
            code.Append(GetColumns(qex.EntityName, qex.ColumnSet, qename + ".ColumnSet"));
            var links = new StringBuilder();
            foreach (var link in qex.LinkEntities)
            {
                links.Append(GetLinkEntity(link, qename));
            }
            code.Append(GetFilter(qex.EntityName, qex.Criteria, qename, "Criteria"));
            code.Append(links);
            foreach (var order in qex.Orders)
            {
                code.AppendLine(qename + ".AddOrder(" + GetCodeAttribute(qex.EntityName, order.AttributeName) + ", OrderType." + order.OrderType.ToString() + ");");
            }
            var codestr = ReplaceValueTokens(code.ToString());
            return codestr;
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
                var firstsep = settings.CodeGenerators.EarlyBound && columns.Columns.Count > 1 ? "\n    " : "";
                var separator = settings.CodeGenerators.EarlyBound ? "\n    " : ", ";
                if (settings.CodeGenerators.IncludeComments)
                {
                    code.AppendLine();
                    code.AppendLine("// Add columns to " + LineStart);
                }
                var cols = firstsep + string.Join(separator, columns.Columns.Select(c => GetCodeAttribute(entity, c)));
                code.AppendLine(LineStart + ".AddColumns(" + cols + ");");
            }
            return code.ToString();
        }

        private string GetLinkEntity(LinkEntity link, string LineStart)
        {
            var code = new StringBuilder();
            var linkname = GetVarName(string.IsNullOrEmpty(link.EntityAlias) ? LineStart + "_" + link.LinkToEntityName : link.EntityAlias);
            code.AppendLine();
            if (settings.CodeGenerators.IncludeComments)
            {
                code.AppendLine("// Add link-entity " + linkname);
            }
            var join = link.JoinOperator == JoinOperator.Inner ? "" : ", JoinOperator." + link.JoinOperator.ToString();
            code.AppendLine($"var {linkname} = {LineStart}.AddLink({GetCodeEntity(link.LinkToEntityName)}, {GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName)}, {GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName)}{join});");
            if (!string.IsNullOrWhiteSpace(link.EntityAlias))
            {
                entityaliases.Add(link.EntityAlias, link.LinkToEntityName);
                code.AppendLine(linkname + ".EntityAlias = \"" + link.EntityAlias + "\";");
            }
            code.Append(GetColumns(link.LinkToEntityName, link.Columns, linkname + ".Columns"));
            code.Append(GetFilter(link.LinkToEntityName, link.LinkCriteria, linkname, "LinkCriteria"));
            foreach (var sublink in link.LinkEntities)
            {
                code.Append(GetLinkEntity(sublink, linkname));
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
                    code.AppendLine("// Define filter " + LineStart);
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

        private string ReplaceValueTokens(string code)
        {
            if (!code.Contains("<<<"))
            {
                return code;
            }
            var variables = new StringBuilder();
            if (settings.CodeGenerators.IncludeComments)
            {
                variables.AppendLine("// Define Condition Values");
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
            if (settings.CodeGenerators.EarlyBound &&
                metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity)
            {
                return entity.SchemaName + "." + settings.CodeGenerators.EBG_EntityLogicalNames;
            }
            return "\"" + entityname + "\"";
        }

        private string GetCodeAttribute(string entityname, string attributename)
        {
            if (settings.CodeGenerators.EarlyBound &&
                metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity &&
                entity.Attributes.FirstOrDefault(a => a.LogicalName.Equals(attributename)) is AttributeMetadata attribute)
            {
                return entity.SchemaName + "." + settings.CodeGenerators.EBG_AttributeLogicalNameClass + attribute.SchemaName;
            }
            return "\"" + attributename + "\"";
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