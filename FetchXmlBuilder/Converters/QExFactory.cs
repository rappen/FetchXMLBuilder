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
        private readonly string Indent;
        private Dictionary<string, string> entityaliases;

        internal QExFactory(QueryExpressionCodeGenerator generator)
        {
            gen = generator ?? throw new ArgumentNullException(nameof(generator));
            objectini = gen.settings.ObjectInitializer;
            comments = gen.settings.IncludeComments;
            CRLF = QueryExpressionCodeGenerator.CRLF;
            Indent = QueryExpressionCodeGenerator.Indent;
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
                code.Append(GetColumns(qex.EntityName, qex.ColumnSet, qename));
                code.Append(GetConditions(qex.EntityName, qex.Criteria.Conditions, qename));
                code.Append(GetFilters(qex.EntityName, qex.Criteria.Filters, qename, "Criteria"));
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

            querycode += QueryExpressionCodeGenerator.GetQueryOptions(qex);

            var columns = GetColumns(qex.EntityName, qex.ColumnSet, "ColumnSet", 2);
            if (!string.IsNullOrEmpty(columns))
            {
                querycode += ", " + CRLF + columns;
            }
            var filters = GetConditions(qex.EntityName, qex.Criteria.Conditions, qename);
            if (!string.IsNullOrEmpty(filters))
            {
                querycode += ", " + CRLF + filters;
            }
            querycode = querycode + ");" + CRLF;
            return querycode;
        }

        private string GetColumns(string entity, ColumnSet columns, string LineStart, int indents = 1)
        {
            var code = new StringBuilder();
            if (columns.AllColumns)
            {
                if (comments)
                {
                    code.AppendLine("// Add all columns to " + LineStart);
                }
                code.Append("new ColumnSet(true)");
            }
            else if (columns.Columns.Count > 0)
            {
                if (comments)
                {
                    code.AppendLine("// Add columns to " + LineStart);
                }
                switch (gen.settings.QExFlavor)
                {
                    case QExFlavorEnum.EarlyBound:
                        LineStart = gen.GetCodeEntityPrefix(entity) + " => new { ";
                        break;

                    default:
                        LineStart = $"{Indent}new ColumnSet(";
                        break;
                }
                var colsEB = QueryExpressionCodeGenerator.GetCodeParametersMaxWidth(120 - LineStart.Length, indents, columns.Columns.Select(c => gen.GetCodeAttribute(entity, c)).ToArray());
                code.Append(LineStart + colsEB);
                switch (gen.settings.QExFlavor)
                {
                    case QExFlavorEnum.EarlyBound:
                        code.Append(" }");
                        break;

                    default:
                        code.Append(")");
                        break;
                }
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
                    entityaliases.Add(link.EntityAlias, link.LinkToEntityName);
                    code.AppendLine(linkname + ".EntityAlias = \"" + link.EntityAlias + "\";");
                }
                code.Append(GetColumns(link.LinkToEntityName, link.Columns, linkname + ".Columns"));
                code.Append(GetConditions(link.LinkToEntityName, link.LinkCriteria.Conditions, ""));
                code.Append(GetFilters(link.LinkToEntityName, link.LinkCriteria.Filters, linkname, "LinkCriteria"));
                code.Append(GetOrders(link.LinkToEntityName, link.Orders, linkname));
                code.Append(GetLinkEntities(link.LinkEntities, linkname));
            }
            return code.ToString();
        }

        private string GetFilters(string entity, IEnumerable<FilterExpression> filters, string parentName, string property)
        {
            if (filters?.Any() != true)
            {
                return string.Empty;
            }

            var filterscode = new List<string>();
            var i = 0;
            foreach (var filter in filters.Where(f => f.Conditions.Any() || f.Filters.Any()))
            {
                var code = new StringBuilder();
                var LineStart = parentName + (!string.IsNullOrEmpty(property) ? "." + property : "");
                var filtername = LineStart.Replace(".", "_");
                if (filters.Count() > 1)
                {
                    filtername += "_" + i++.ToString();
                }
                filtername = gen.GetVarName(filtername);
                if (comments)
                {
                    code.AppendLine();
                    code.AppendLine("// Add filter " + LineStart);
                }
                if (!objectini)
                {
                    code.AppendLine($"var {filtername} = new FilterExpression();");
                    code.AppendLine($"{LineStart}.AddFilter({filtername});");
                }
                if (filter.FilterOperator == LogicalOperator.Or)
                {
                    code.AppendLine(LineStart + ".FilterOperator = LogicalOperator.Or;");
                }
                code.AppendLine(GetConditions(entity, filter.Conditions, LineStart));
                code.Append(GetFilters(entity, filter.Filters, filtername, null));
                filterscode.Add(code.ToString());
            }
            return string.Join($",{CRLF}", filterscode);
        }

        private string GetConditions(string entity, IEnumerable<ConditionExpression> conditions, string LineStart)
        {
            if (conditions?.Any() != true)
            {
                return string.Empty;
            }
            var conditionscode = new List<string>();
            foreach (var cond in conditions)
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
                    values = ", " + QueryExpressionCodeGenerator.GetConditionValues(cond.Values, token, gen.settings.FilterVariables);
                    if (cond.CompareColumns)
                    {
                        values = ", true" + values;
                    }
                }
                if (cond.Operator == ConditionOperator.Equal && cond.Values.Count == 1 && string.IsNullOrEmpty(entityalias))
                {
                    conditionscode.Add($"{gen.GetCodeAttribute(filterentity, cond.AttributeName)}{values}");
                }
                else
                {
                    conditionscode.Add($"{LineStart}.AddCondition({entityalias}{gen.GetCodeAttribute(filterentity, cond.AttributeName)}, ConditionOperator.{cond.Operator}{values});");
                }
            }
            return Indent + string.Join($",{CRLF}{Indent}", conditionscode);
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