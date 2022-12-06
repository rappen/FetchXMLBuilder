using Microsoft.CSharp;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using System;
using System.CodeDom.Compiler;
using System.Text;
using System.Text.RegularExpressions;

namespace Cinteros.Xrm.FetchXmlBuilder.Converters
{
    internal static class QExParse
    {
        private delegate QueryExpression queryExpressionCompiler();

        internal static string GetFetchXmlFromCSharpQueryExpression(string query, IOrganizationService organizationService)
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