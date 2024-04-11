using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    internal class SQLQueryGenerator
    {
        private static Dictionary<string, string> aliasmap;
        private static List<string> selectcols;
        private static List<string> ordercols;

        internal static string GetSQLQuery(FetchType fetch, FetchXmlBuilder fxb)
        {
            aliasmap = new Dictionary<string, string>();
            var sql = new StringBuilder();
            var entity = fetch.Item;
            if (entity == null)
            {
                throw new Exception("Fetch must contain entity definition");
            }
            sql.Append("SELECT ");
            if (fetch.distinctSpecified && fetch.distinct)
            {
                sql.Append("DISTINCT ");
            }
            if (!string.IsNullOrEmpty(fetch.top))
            {
                sql.Append($"TOP {fetch.top} ");
            }
            if (entity.Items != null)
            {
                selectcols = GetSelect(entity);
                ordercols = GetOrder(entity.Items.Where(i => i is FetchOrderType).ToList(), string.Empty);
                var join = GetJoin(entity.Items.Where(i => i is FetchLinkEntityType).ToList(), entity.name, fxb);
                var where = GetWhere(entity.name, string.Empty, entity.Items.Where(i => i is FetchFilterType && ((FetchFilterType)i).Items != null && ((FetchFilterType)i).Items.Length > 0).ToList(), fxb);
                sql.AppendLine(string.Join(", ", selectcols));
                sql.AppendLine($"FROM {entity.name}");
                if (join != null && join.Count > 0)
                {
                    sql.AppendLine(string.Join("\n", join));
                }
                if (!string.IsNullOrEmpty(where))
                {
                    sql.AppendLine($"WHERE {where}");
                }
                if (ordercols != null && ordercols.Count > 0)
                {
                    sql.Append("ORDER BY ");
                    sql.AppendLine(string.Join(", ", ordercols));
                }
            }

            return sql.ToString();
        }

        private static List<string> GetSelect(FetchEntityType entity)
        {
            var result = new List<string>();
            var attributeitems = entity.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
            if (attributeitems.Count > 0)
            {
                foreach (FetchAttributeType attributeitem in attributeitems)
                {
                    result.Add(attributeitem.name);
                }
            }
            else
            {
                result.Add("*");
            }
            return result;
        }

        private static List<string> GetJoin(List<object> linkentities, string entityalias, FetchXmlBuilder fxb)
        {
            var joinList = new List<string>();
            foreach (FetchLinkEntityType linkitem in linkentities)
            {
                var join = new StringBuilder();
                if (linkitem.linktype == FetchLinkEntityTypeLinktype.outer)
                {
                    join.Append("LEFT OUTER ");
                }
                var linkalias = string.IsNullOrEmpty(linkitem.alias) ? linkitem.name : linkitem.alias;
                if (linkalias != linkitem.name)
                {
                    aliasmap.Add(linkalias, linkitem.name);
                }
                join.Append($"JOIN {linkitem.name} {linkalias} ON {linkalias}.{linkitem.from} = {entityalias}.{linkitem.to}");
                if (linkitem.Items != null)
                {
                    var linkwhere = GetWhere(linkitem.name, linkalias, linkitem.Items.Where(i => i is FetchFilterType && ((FetchFilterType)i).Items != null && ((FetchFilterType)i).Items.Length > 0).ToList(), fxb);
                    if (!string.IsNullOrEmpty(linkwhere))
                    {
                        join.Append($" AND {linkwhere} ");
                    }
                }
                joinList.Add(join.ToString().Trim());
                if (linkitem.Items != null)
                {
                    selectcols.AddRange(GetExpandedSelect(linkitem, linkalias));
                    ordercols.AddRange(GetOrder(linkitem.Items.Where(i => i is FetchOrderType).ToList(), linkalias));
                    joinList.AddRange(GetJoin(linkitem.Items.Where(i => i is FetchLinkEntityType).ToList(), linkalias, fxb));
                }
            }
            return joinList;
        }

        private static List<string> GetExpandedSelect(FetchLinkEntityType linkitem, string relation)
        {
            var resultList = new List<string>();
            if (linkitem.Items != null)
            {
                var attributeitems = linkitem.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
                if (linkitem.intersect)
                {
                    var linkitems = linkitem.Items.Where(i => i is FetchLinkEntityType).ToList();
                    if (linkitems.Count > 1)
                    {
                        throw new Exception("Invalid M:M-relation definition for OData");
                    }
                    if (linkitems.Count == 1)
                    {
                        var nextlink = (FetchLinkEntityType)linkitems[0];
                        attributeitems = nextlink.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
                    }
                }
                if (attributeitems.Count > 0)
                {
                    foreach (FetchAttributeType attributeitem in attributeitems)
                    {
                        resultList.Add(relation + "." + attributeitem.name);
                    }
                }
            }
            return resultList;
        }

        private static string GetWhere(string entityname, string entityalias, List<object> filteritems, FetchXmlBuilder fxb)
        {
            var resultList = new StringBuilder();
            if (filteritems.Count > 0)
            {
                foreach (FetchFilterType filteritem in filteritems)
                {
                    resultList.Append(GetFilter(entityname, entityalias, filteritem, fxb));
                }
            }
            return resultList.ToString();
        }

        private static string GetFilter(string entityname, string entityalias, FetchFilterType filteritem, FetchXmlBuilder fxb)
        {
            var result = "";
            if (filteritem.Items == null || filteritem.Items.Length == 0)
            {
                return "";
            }
            var logical = filteritem.type == FetchFilterTypeType.or ? " OR " : " AND ";
            if (filteritem.Items.Length > 1)
            {
                result = "(";
            }
            foreach (var item in filteritem.Items)
            {
                if (item is FetchConditionType)
                {
                    result += GetCondition(entityname, entityalias, item as FetchConditionType, fxb);
                }
                else if (item is FetchFilterType)
                {
                    result += GetFilter(entityname, entityalias, item as FetchFilterType, fxb);
                }
                result += logical;
            }
            if (result.EndsWith(logical))
            {
                result = result.Substring(0, result.Length - logical.Length);
            }
            if (filteritem.Items.Length > 1)
            {
                result += ")";
            }
            return result;
        }

        private static string GetCondition(string entityname, string entityalias, FetchConditionType condition, FetchXmlBuilder fxb)
        {
            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(entityalias))
            {
                result.Append(entityalias);
                result.Append(".");
            }
            if (!string.IsNullOrEmpty(condition.attribute))
            {
                if (!string.IsNullOrEmpty(condition.entityname))
                {
                    result.Append($"{condition.entityname}.");
                    if (aliasmap.ContainsKey(condition.entityname))
                    {
                        entityname = aliasmap[condition.entityname];
                    }
                    else
                    {
                        entityname = condition.entityname;
                    }
                }
                result.Append(condition.attribute);
                var attrMeta = fxb.GetAttribute(entityname, condition.attribute);
                if (attrMeta == null)
                {
                    throw new Exception($"No metadata for attribute: {entityname}.{condition.attribute}");
                }
                switch (condition.@operator)
                {
                    case @operator.eq:
                    case @operator.on:
                        result.Append(" = ");
                        break;

                    case @operator.ne:
                    case @operator.neq:
                        result.Append(" != ");
                        break;

                    case @operator.lt:
                        result.Append(" < ");
                        break;

                    case @operator.le:
                    case @operator.onorbefore:
                        result.Append(" <= ");
                        break;

                    case @operator.gt:
                        result.Append(" > ");
                        break;

                    case @operator.ge:
                    case @operator.onorafter:
                        result.Append(" >= ");
                        break;

                    case @operator.@null:
                        result.Append(" IS NULL");
                        break;

                    case @operator.notnull:
                        result.Append(" IS NOT NULL");
                        break;

                    case @operator.like:
                        result.Append(" LIKE ");
                        break;

                    case @operator.notlike:
                        result.Append(" NOT LIKE ");
                        break;
                    //case @operator.beginswith:
                    //    result.Append(" LIKE \"{0}%\"");
                    //    break;
                    //case @operator.@in:
                    //    result.Append(" IN ");
                    //    break;
                    //case @operator.notin:
                    //    result.Append(" NOT IN ");
                    //    break;
                    default:
                        throw new Exception($"Unsupported SQL condition operator '{condition.@operator}'");
                }

                if (!string.IsNullOrEmpty(condition.value))
                {
                    switch (attrMeta.AttributeType)
                    {
                        case AttributeTypeCode.Money:
                        case AttributeTypeCode.BigInt:
                        case AttributeTypeCode.Boolean:
                        case AttributeTypeCode.Decimal:
                        case AttributeTypeCode.Double:
                        case AttributeTypeCode.Integer:
                        case AttributeTypeCode.State:
                        case AttributeTypeCode.Status:
                        case AttributeTypeCode.Picklist:
                            result.Append(condition.value);
                            break;

                        default:
                            result.Append($"'{condition.value}'");
                            break;
                    }
                }
            }
            return result.ToString();
        }

        private static List<string> GetOrder(List<object> orderitems, string entityalias)
        {
            var result = new List<string>();
            foreach (FetchOrderType orderitem in orderitems)
            {
                var order = new StringBuilder();
                if (!string.IsNullOrEmpty(entityalias))
                {
                    order.Append($"{entityalias}.");
                }
                if (!string.IsNullOrEmpty(orderitem.alias))
                {
                    order.Append(orderitem.alias);
                }
                else
                {
                    order.Append(orderitem.attribute);
                }
                if (orderitem.descending)
                {
                    order.Append(" DESC");
                }
                result.Add(order.ToString());
            }
            return result;
        }
    }
}