using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinteros.Xrm.FetchXmlBuilder.Converters
{
    internal class QExFactory
    {
        private readonly QueryExpressionCodeGenerator gen;
        private readonly bool objectini;
        private readonly bool comments;
        private readonly string CRLF;
        private Dictionary<string, string> entityaliases;

        internal QExFactory(QueryExpressionCodeGenerator generator)
        {
            gen = generator ?? throw new ArgumentNullException(nameof(generator));
            objectini = gen.settings.ObjectInitializer;
            comments = gen.settings.IncludeComments;
            CRLF = QueryExpressionCodeGenerator.CRLF;
        }

        internal string Generated => CreateCode(gen.qex);

        private string CreateCode(QueryExpression qex)
        {
            entityaliases = new Dictionary<string, string>();
            var code = new StringBuilder();
            var qename = gen.GetVarName("qefactory");
            code.Append(GetQueryCodeStart(qename, qex));
            if (!objectini)
            {
                code.Append(gen.GetColumns(qex.EntityName, qex.ColumnSet, qename));
                code.Append(gen.GetFilter(qex.EntityName, qex.Criteria, qename, ParentFilterType.Criteria));
                code.Append(GetOrders(qex.EntityName, qex.Orders, qename, true));
                code.Append(GetLinkEntities(qex.LinkEntities, qename));
            }
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
            switch (gen.settings.QExFlavor)
            {
                case QExFlavorEnum.EarlyBound:
                    querycode += $"var {qename} = QueryExpressionFactory.Create<{gen.GetCodeEntity(qex.EntityName)}>(";
                    break;

                default:
                    querycode += $"var {qename} = QueryExpressionFactory.Create({gen.GetCodeEntity(qex.EntityName)}";
                    break;
            }

            if (objectini)
            {
                var objinicode = new List<string>();
                objinicode.Add(gen.GetColumns(qex.EntityName, qex.ColumnSet, "ColumnSet", 2));
                objinicode.Add(gen.GetFilter(qex.EntityName, qex.Criteria, qename, ParentFilterType.Criteria));
                objinicode.Add(GetOrders(qex.EntityName, qex.Orders, qename, true));
                objinicode.Add(GetLinkEntities(qex.LinkEntities, qename));
                querycode += $",{CRLF}" + string.Join($",{CRLF}", objinicode.Where(o => !string.IsNullOrWhiteSpace(o)));
            }
            querycode += $"){gen.GetQueryOptions(qex, qename)};{CRLF}";
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
                code.Append(gen.GetColumns(link.LinkToEntityName, link.Columns, linkname + ".Columns"));
                code.Append(gen.GetFilter(link.LinkToEntityName, link.LinkCriteria, linkname, ParentFilterType.LinkCriteria));
                code.Append(GetOrders(link.LinkToEntityName, link.Orders, linkname));
                code.Append(GetLinkEntities(link.LinkEntities, linkname));
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
            if (comments)
            {
                code.AppendLine();
                code.AppendLine("// Add orders");
            }
            LineStart += root ? ".AddOrder(" : ".Orders.Add(new OrderExpression(";
            var LineEnd = root ? ");" : "));";
            foreach (var order in orders)
            {
                code.AppendLine(LineStart + gen.GetCodeAttribute(entityname, order.AttributeName) + ", OrderType." + order.OrderType.ToString() + LineEnd);
            }
            return code.ToString();
        }
    }
}