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
    internal class QExFactory
    {
        private readonly CSharpCodeGenerator gen;
        private readonly bool comments;
        private readonly string CRLF;
        private Dictionary<string, string> entityaliases;

        internal QExFactory(CSharpCodeGenerator generator)
        {
            gen = generator ?? throw new ArgumentNullException(nameof(generator));
            comments = gen.settings.IncludeComments;
            CRLF = CSharpCodeGenerator.CRLF;
        }

        internal string Generated => CreateCode(gen.qex);

        private string CreateCode(QueryExpression qex)
        {
            entityaliases = new Dictionary<string, string>();
            var code = new StringBuilder();
            var qename = gen.GetVarName("qefactory");
            code.Append(GetQueryCodeStart(qename, qex));
            var codestr = gen.ReplaceValueTokens(code.ToString());
            return codestr;
        }

        private string GetQueryCodeStart(string qename, QueryExpression qex)
        {
            var querycode = string.Empty;
            if (comments)
            {
                querycode += "// Instantiate QueryExpressionFactory " + qename + CRLF;
            }
            var objinicode = new List<string>
            {
                gen.GetColumnsOI(qex.EntityName, qex.ColumnSet, "ColumnSet", 1),
                gen.GetFilterOI(qex.EntityName, qex.Criteria, qename, ParentFilterType.Criteria, 1),
                //GetLinkEntities(qex.LinkEntities, qename)
            }.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();

            var insidecode = string.Join($",{CRLF}", objinicode);
            switch (gen.settings.QExFlavor)
            {
                case QExFlavorEnum.EarlyBound:
                    querycode += $"var {qename} = QueryExpressionFactory.Create<{gen.GetCodeEntity(qex.EntityName)}>({CRLF}{insidecode});";
                    break;

                default:
                    querycode += $"var {qename} = QueryExpressionFactory.Create({gen.GetCodeEntity(qex.EntityName)},{CRLF}{insidecode});";
                    break;
            }
            querycode += CRLF + gen.GetOrdersLbL(qex.EntityName, qex.Orders, qename, true);
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
                        gen.GetCodeAttribute(link.LinkFromEntityName,
                        link.LinkFromAttributeName),
                        gen.GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName),
                        join);
                code.AppendLine($"{varstart}{LineStart}.AddLink({parms});");
                if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                {
                    entityaliases.Add(link.EntityAlias, link.LinkToEntityName);
                    code.AppendLine(linkname + ".EntityAlias = \"" + link.EntityAlias + "\";");
                }
                code.Append(gen.GetColumnsOI(link.LinkToEntityName, link.Columns, linkname + ".Columns", 1));
                code.Append(gen.GetFilterOI(link.LinkToEntityName, link.LinkCriteria, linkname, ParentFilterType.LinkCriteria, 1));
                code.Append(gen.GetOrdersOI(link.LinkToEntityName, link.Orders, linkname, 1));
                code.Append(GetLinkEntities(link.LinkEntities, linkname));
            }
            return code.ToString();
        }
    }
}