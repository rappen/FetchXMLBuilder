using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using Microsoft.Crm.Sdk.Messages;
using Rappen.XTB.FetchXmlBuilder.Extensions;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    public class QueryExpressionCodeGenerator
    {
        private static List<string> varList = new List<string>();

        public static string GetCSharpQueryExpression(QueryExpression QEx)
        {
            varList.Clear();
            var code = new StringBuilder();
            var qename = GetVarName("query");
            code.AppendLine("// Instantiate QueryExpression " + qename);
            code.AppendLine("var " + qename + " = new QueryExpression(\"" + QEx.EntityName + "\");");
            if (QEx.NoLock)
            {
                code.AppendLine(qename + ".NoLock = true;");
            }
            if (QEx.Distinct)
            {
                code.AppendLine(qename + ".Distinct = true;");
            }
            if (QEx.TopCount != null)
            {
                code.AppendLine(qename + ".TopCount = " + QEx.TopCount.ToString() + ";");
            }
            code.Append(GetColumns(QEx.ColumnSet, qename + ".ColumnSet"));
            foreach (var order in QEx.Orders)
            {
                code.AppendLine(qename + ".AddOrder(\"" + order.AttributeName + "\", OrderType." + order.OrderType.ToString() + ");");
            }
            code.Append(GetFilter(QEx.Criteria, qename, "Criteria"));
            foreach (var link in QEx.LinkEntities)
            {
                code.Append(GetLinkEntity(link, qename));
            }
            var codestr = ReplaceValueTokens(code.ToString());
            return codestr;
        }

        private static string GetVarName(string requestedname)
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

        private static string GetColumns(ColumnSet columns, string LineStart)
        {
            var code = new StringBuilder();
            if (columns.AllColumns)
            {
                code.AppendLine();
                code.AppendLine("// Add all columns to " + LineStart);
                code.AppendLine(LineStart + ".AllColumns = true;");
            }
            else if (columns.Columns.Count > 0)
            {
                code.AppendLine();
                code.AppendLine("// Add columns to " + LineStart);
                var cols = "\"" + string.Join("\", \"", columns.Columns) + "\"";
                code.AppendLine(LineStart + ".AddColumns(" + cols + ");");
            }
            return code.ToString();
        }

        private static string GetLinkEntity(LinkEntity link, string LineStart)
        {
            var code = new StringBuilder();
            var linkname = GetVarName(string.IsNullOrEmpty(link.EntityAlias) ? LineStart + "_" + link.LinkToEntityName : link.EntityAlias);
            code.AppendLine();
            code.AppendLine("// Add link-entity " + linkname);
            var join = link.JoinOperator == JoinOperator.Inner ? "" : ", JoinOperator." + link.JoinOperator.ToString();
            code.AppendLine($"var {linkname} = {LineStart}.AddLink(\"{link.LinkToEntityName}\", \"{link.LinkFromAttributeName}\", \"{link.LinkToAttributeName}\"{join});");
            if (!string.IsNullOrWhiteSpace(link.EntityAlias))
            {
                code.AppendLine(linkname + ".EntityAlias = \"" + link.EntityAlias + "\";");
            }
            code.Append(GetColumns(link.Columns, linkname + ".Columns"));
            code.Append(GetFilter(link.LinkCriteria, linkname, "LinkCriteria"));
            foreach (var sublink in link.LinkEntities)
            {
                code.Append(GetLinkEntity(sublink, linkname));
            }
            return code.ToString();
        }

        private static string GetFilter(FilterExpression filterExpression, string parentName, string property)
        {
            var LineStart = parentName + (!string.IsNullOrEmpty(property) ? "." + property : "");
            var code = new StringBuilder();
            if (filterExpression.FilterOperator == LogicalOperator.Or || filterExpression.Conditions.Count > 0 || filterExpression.Filters.Count > 0)
            {
                code.AppendLine();
                code.AppendLine("// Define filter " + LineStart);
                if (filterExpression.FilterOperator == LogicalOperator.Or)
                {
                    code.AppendLine(LineStart + ".FilterOperator = LogicalOperator.Or;");
                }
                foreach (var cond in filterExpression.Conditions)
                {
                    var entity = "";
                    var values = "";
                    var token = LineStart.Replace(".", "_").Replace("_Criteria", "").Replace("_LinkCriteria", "");
                    if (!string.IsNullOrWhiteSpace(cond.EntityName))
                    {
                        entity = "\"" + cond.EntityName + "\", ";
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
                    code.AppendLine($"{LineStart}.AddCondition({entity}\"{cond.AttributeName}\", ConditionOperator.{cond.Operator.ToString()}{values});");
                }
                var i = 0;
                foreach (var subfilter in filterExpression.Filters)
                {
                    var filtername = GetVarName(LineStart.Replace(".", "_") + "_" + i.ToString());
                    code.AppendLine($"var {filtername} = new FilterExpression();");
                    code.AppendLine($"{LineStart}.AddFilter({filtername});");
                    code.Append(GetFilter(subfilter, filtername, null));
                    i++;
                }
            }
            return code.ToString();
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

        private static string ReplaceValueTokens(string code)
        {
            if (!code.Contains("<<<"))
            {
                return code;
            }
            var variables = new StringBuilder();
            variables.AppendLine("// Define Condition Values");
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

        private delegate QueryExpression queryExpressionCompiler();

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