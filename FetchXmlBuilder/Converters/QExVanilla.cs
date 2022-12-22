using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Converters;
using Rappen.XTB.FetchXmlBuilder.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinteros.Xrm.FetchXmlBuilder.Converters
{
    internal class QExVanilla
    {
        private readonly CSharpCodeGenerator gen;
        private readonly bool comments;
        private readonly string CRLF;

        internal QExVanilla(CSharpCodeGenerator generator)
        {
            if (generator.settings.QExFlavor == QExFlavorEnum.EarlyBound)
            {
                throw new ArgumentOutOfRangeException("Flavor", "Early Bound is not possible for vanilla CRM SDK libraries.");
            }
            gen = generator ?? throw new ArgumentNullException(nameof(generator));
            comments = gen.settings.IncludeComments;
            CRLF = CSharpCodeGenerator.CRLF;
        }

        internal string Generated => CreateCode(gen.qex);

        private string CreateCode(QueryExpression qex)
        {
            var code = new StringBuilder();
            var qename = gen.GetVarName("query");
            code.Append(GetQueryCodeStart(qename, qex));
            if (!gen.settings.ObjectInitializer)
            {
                code.AppendLine(gen.GetColumnsLbL(qex.EntityName, qex.ColumnSet, qename + ".ColumnSet"));
                code.AppendLine(gen.GetFilterLbL(qex.EntityName, qex.Criteria, qename, ParentFilterType.Criteria));
                code.AppendLine(gen.GetOrdersLbL(qex.EntityName, qex.Orders, qename, true));
                code.AppendLine(GetLinkEntities(qex.LinkEntities, qename));
            }
            var codestr = gen.ReplaceValueTokens(code.ToString());
            return codestr;
        }

        private string GetQueryCodeStart(string qename, QueryExpression qex)
        {
            var querycode = string.Empty;
            if (comments)
            {
                querycode += "// Instantiate QueryExpression " + qename + CRLF;
            }
            querycode += "var " + qename + " = new QueryExpression(" + gen.GetCodeEntity(qex.EntityName) + ")";
            var intocurly = false;
            var options = gen.GetQueryOptions(qex, qename);
            if (!string.IsNullOrWhiteSpace(options) && gen.settings.ObjectInitializer)
            {
                querycode += $"{CRLF}{{{options}";
                intocurly = true;
            }
            if (gen.settings.ObjectInitializer)
            {
                var objinicode = new List<string>
                {
                    gen.GetColumnsOI(qex.EntityName, qex.ColumnSet, "ColumnSet", 1),
                    gen.GetFilterOI(qex.EntityName, qex.Criteria, qename, ParentFilterType.Criteria, 1),
                    gen.GetOrdersOI(qex.EntityName, qex.Orders, qename, 1),
           //         GetLinkEntities(qex.LinkEntities, qename)
                }.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
                if (objinicode.Any())
                {
                    if (!intocurly)
                    {
                        querycode += $"{CRLF}{{{CRLF}";
                    }
                    else
                    {
                        querycode += $",{CRLF}";
                    }
                    querycode += string.Join($",{CRLF}", objinicode) + $"{CRLF}}};{CRLF}";
                }
            }
            else
            {
                querycode += $";{CRLF}";
                if (!string.IsNullOrWhiteSpace(options))
                {
                    querycode += $"{options}{CRLF}";
                }
            }
            return querycode;
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
                var linkname = gen.GetVarName(string.IsNullOrEmpty(link.EntityAlias) ? LineStart + "_" + link.LinkToEntityName : link.EntityAlias);
                code.AppendLine();
                if (comments)
                {
                    code.AppendLine("// Add link-entity " + linkname);
                }
                var join = link.JoinOperator == JoinOperator.Inner ? "" : "JoinOperator." + link.JoinOperator.ToString();
                var varstart =
                    link.LinkEntities.Count > 0 ||
                    link.Columns.Columns.Count > 0 ||
                    link.LinkCriteria.Conditions.Count > 0 ||
                    link.Orders.Count > 0 ? $"var {linkname} = " : String.Empty;
                var parms = gen.GetCodeParametersMaxWidth(120 - varstart.Length - LineStart.Length, 1, true,
                    gen.GetCodeEntity(link.LinkToEntityName),
                    gen.GetCodeAttribute(link.LinkFromEntityName, link.LinkFromAttributeName),
                    gen.GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName),
                    join);
                code.AppendLine($"{varstart}{LineStart}.AddLink({parms});");
                if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                {
                    code.AppendLine(linkname + ".EntityAlias = \"" + link.EntityAlias + "\";");
                }
                code.Append(gen.GetColumnsLbL(link.LinkToEntityName, link.Columns, linkname + ".Columns"));
                code.Append(gen.GetFilterLbL(link.LinkToEntityName, link.LinkCriteria, linkname, ParentFilterType.LinkCriteria));
                code.Append(gen.GetOrdersLbL(link.LinkToEntityName, link.Orders, linkname));
                code.Append(GetLinkEntities(link.LinkEntities, linkname));
            }
            return code.ToString();
        }
    }
}