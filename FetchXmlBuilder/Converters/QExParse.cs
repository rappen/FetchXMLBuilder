using Microsoft.CSharp;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Settings;
using System;
using System.CodeDom.Compiler;
using System.Text;
using System.Text.RegularExpressions;

namespace Cinteros.Xrm.FetchXmlBuilder.Converters
{
    internal static class QExParse
    {
        private delegate QueryExpression queryExpressionCompiler();

        private delegate QueryByAttribute queryByAttributeCompiler();

        internal static string GetFetchXmlFromCSharpQueryExpression(string query, QExStyleEnum style, IOrganizationService organizationService)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            parameters.ReferencedAssemblies.Add("Microsoft.Xrm.Sdk.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            var fromscript = GetQueryExpressionFromScript(query, style);
            CompilerResults compilerResults = provider.CompileAssemblyFromSource(parameters, fromscript);

            if (compilerResults.Errors.HasErrors)
            {
                StringBuilder sbuilder = new StringBuilder();
                foreach (CompilerError compilerError in compilerResults.Errors)
                {
                    sbuilder.AppendLine($"Error ({compilerError.ErrorNumber}): {compilerError.ErrorText}");
                }
                throw new InvalidOperationException(sbuilder.ToString());
            }

            QueryBase queryBase = null;
            switch (style)
            {
                case QExStyleEnum.QueryExpression:
                    queryBase = ((queryExpressionCompiler)Delegate.CreateDelegate(typeof(queryExpressionCompiler),
                        compilerResults.CompiledAssembly.GetType("DynamicContentGenerator.Generator"), "Generate"))();
                    break;

                case QExStyleEnum.QueryByAttribute:
                    queryBase = ((queryByAttributeCompiler)Delegate.CreateDelegate(typeof(queryByAttributeCompiler),
                        compilerResults.CompiledAssembly.GetType("DynamicContentGenerator.Generator"), "Generate"))();
                    break;
            }

            return organizationService.QueryExpressionToFetchXml(queryBase);
        }

        private static string GetQueryExpressionFromScript(string query, QExStyleEnum style)
        {
            switch (style)
            {
                case QExStyleEnum.QueryExpression:
                    Regex varMatcher1 = new Regex(@"(var|QueryExpression)\W+([^\W]+)\W*=\W*new QueryExpression\W*\(");
                    Match match1 = varMatcher1.Match(query);
                    if (match1.Success)
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
                                        return {match1.Groups[2].Value};
                                    }}
                                }}
                            }}";
                    }
                    break;

                case QExStyleEnum.QueryByAttribute:
                    Regex varMatcher2 = new Regex(@"(var|QueryByAttribute)\W+([^\W]+)\W*=\W*new QueryByAttribute\W*\(");
                    Match match2 = varMatcher2.Match(query);
                    if (match2.Success)
                    {
                        return $@"
                            using System;
                            using Microsoft.Xrm.Sdk;
                            using Microsoft.Xrm.Sdk.Query;

                            namespace DynamicContentGenerator
                            {{
                                public class Generator
                                {{
                                    public static QueryByAttribute Generate()
                                    {{
                                        {query}
                                        return {match2.Groups[2].Value};
                                    }}
                                }}
                            }}";
                    }
                    break;
            }
            throw new Exception($"Could not determine {style} variable.");
        }
    }
}