using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinteros.Xrm.FetchXmlBuilder.Converters
{
    internal class QExVanilla
    {
        private readonly QueryExpressionCodeGenerator gen;
        private readonly bool comments;
        private readonly string CRLF;
        private readonly string Indent;

        internal QExVanilla(QueryExpressionCodeGenerator generator)
        {
            if (generator.settings.QExFlavor == QExFlavorEnum.EarlyBound)
            {
                throw new ArgumentOutOfRangeException("Flavor", "Early Bound is not possible for vanilla CRM SDK libraries.");
            }
            gen = generator ?? throw new ArgumentNullException(nameof(generator));
            comments = gen.settings.IncludeComments;
            CRLF = QueryExpressionCodeGenerator.CRLF;
            Indent = QueryExpressionCodeGenerator.Indent;
        }

        internal string Generated => CreateCode(gen.qex);

        private string CreateCode(QueryExpression qex)
        {
            var code = new StringBuilder();
            var qename = gen.GetVarName("query");
            code.Append(GetQueryCodeStart(qename, qex));
            code.Append(GetColumns(qex.EntityName, qex.ColumnSet, qename + ".ColumnSet"));
            code.Append(GetFilter(qex.EntityName, qex.Criteria, qename, "Criteria"));
            code.Append(GetOrders(qex.EntityName, qex.Orders, qename, true));
            code.Append(GetLinkEntities(qex.LinkEntities, qename));
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

            querycode += QueryExpressionCodeGenerator.GetQueryOptions(qex) + ";" + CRLF;
            return querycode;
        }

        private string GetColumns(string entity, ColumnSet columns, string LineStart)
        {
            var code = new StringBuilder();
            if (columns.AllColumns)
            {
                code.AppendLine();
                if (comments)
                {
                    code.AppendLine("// Add all columns to " + LineStart);
                }
                code.AppendLine(LineStart + ".AllColumns = true;");
            }
            else if (columns.Columns.Count > 0)
            {
                if (comments)
                {
                    code.AppendLine();
                    code.AppendLine("// Add columns to " + LineStart);
                }
                LineStart += ".AddColumn" + (columns.Columns.Count > 1 ? "s" : "") + "(";
                var LineEnd = ");";
                var cols = QueryExpressionCodeGenerator.GetCodeParametersMaxWidth(120 - LineStart.Length, 1, columns.Columns.Select(c => gen.GetCodeAttribute(entity, c)).ToArray());
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
                var parms = QueryExpressionCodeGenerator.GetCodeParametersMaxWidth(120 - varstart.Length - LineStart.Length, 1,
                        gen.GetCodeEntity(link.LinkToEntityName),
                        gen.GetCodeAttribute(link.LinkFromEntityName,
                        link.LinkFromAttributeName),
                        gen.GetCodeAttribute(link.LinkToEntityName, link.LinkToAttributeName),
                        join);
                code.AppendLine($"{varstart}{LineStart}.AddLink({parms});");
                if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                {
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
                if (comments)
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
                        filterentity = gen.entityaliases.FirstOrDefault(a => a.Key.Equals(cond.EntityName)).Value ?? cond.EntityName;
                        entityalias = "\"" + cond.EntityName + "\", ";
                        token += "_" + cond.EntityName;
                    }
                    token += "_" + cond.AttributeName;
                    if (cond.Values.Count > 0)
                    {
                        values = ", " + QueryExpressionCodeGenerator.GetConditionValues(cond.Values, token, gen.settings.FilterVariables);

                        if (cond.CompareColumns)
                        {
                            values = ", true" + values;
                        }
                    }
                    code.AppendLine($"{LineStart}.AddCondition({entityalias}{gen.GetCodeAttribute(filterentity, cond.AttributeName)}, ConditionOperator.{cond.Operator}{values});");
                }
                var i = 0;
                foreach (var subfilter in filterExpression.Filters)
                {
                    var filtername = gen.GetVarName(LineStart.Replace(".", "_") + "_" + i.ToString());
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