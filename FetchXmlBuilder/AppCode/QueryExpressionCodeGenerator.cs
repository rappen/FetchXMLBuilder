using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class QueryExpressionCodeGenerator
    {
        private static List<string> varList = new List<string>();

        public static string GetCSharpQueryExpression(QueryExpression QEx)
        {
            varList.Clear();
            var code = new StringBuilder();
            var qename = GetVarName("QE" + QEx.EntityName);
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
            code.Append(GetFilter(QEx.Criteria, qename + ".Criteria"));
            foreach (var link in QEx.LinkEntities)
            {
                code.Append(GetLinkEntity(link, qename));
            }

            return code.ToString();
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
            var linkname = GetVarName(LineStart + "_" + link.LinkToEntityName);
            code.AppendLine();
            code.AppendLine("// Add link-entity " + linkname);
            var join = link.JoinOperator == JoinOperator.Inner ? "" : ", JoinOperator." + link.JoinOperator.ToString();
            code.AppendLine("var " + linkname + " = " + LineStart + ".AddLink(\"" + link.LinkToEntityName + "\", \"" + link.LinkFromAttributeName + "\", \"" + link.LinkToEntityName + "\"" + join + ");");
            if (!string.IsNullOrWhiteSpace(link.EntityAlias))
            {
                code.AppendLine(linkname + ".EntityAlias = \"" + link.EntityAlias + "\";");
            }
            code.Append(GetColumns(link.Columns, linkname + ".Columns"));
            code.Append(GetFilter(link.LinkCriteria, linkname + ".LinkCriteria"));
            foreach (var sublink in link.LinkEntities)
            {
                code.Append(GetLinkEntity(sublink, linkname));
            }
            return code.ToString();
        }

        private static string GetFilter(FilterExpression filterExpression, string LineStart)
        {
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
                    if (!string.IsNullOrWhiteSpace(cond.EntityName))
                    {
                        entity = "\"" + cond.EntityName + "\", ";
                    }
                    if (cond.Values.Count > 0)
                    {
                        values = ", " + GetConditionValues(cond.Values);
                    }
                    code.AppendLine(LineStart + ".AddCondition(" + entity + "\"" + cond.AttributeName + "\", ConditionOperator." + cond.Operator.ToString() + values + ");");
                }
                var i = 0;
                foreach (var subfilter in filterExpression.Filters)
                {
                    var filtername = GetVarName(LineStart.Replace(".", "_") + "_" + i.ToString());
                    code.AppendLine("var " + filtername + " = new FilterExpression(LogicalOperator." + subfilter.FilterOperator + ");");
                    code.AppendLine(LineStart + ".AddFilter(" + filtername + ");");
                    code.Append(GetFilter(subfilter, filtername));
                    i++;
                }
            }
            return code.ToString();
        }

        private static string GetConditionValues(DataCollection<object> values)
        {
            var strings = new List<string>();
            foreach (var value in values)
            {
                if (value is string || value is Guid)
                {
                    strings.Add("\"" + value.ToString() + "\"");
                }
                else
                {
                    strings.Add(value.ToString());
                }
            }
            return string.Join(", ", strings);
        }
    }
}
