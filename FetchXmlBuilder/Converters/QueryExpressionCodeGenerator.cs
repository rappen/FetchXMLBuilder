using Cinteros.Xrm.FetchXmlBuilder.Converters;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    public class QueryExpressionCodeGenerator
    {
        private List<string> varList;
        internal static string CRLF = "\r\n";
        internal static string Indent = "    ";
        internal QueryExpression qex;
        internal List<EntityMetadata> metas;
        internal CodeGenerators settings;
        internal Dictionary<string, string> entityaliases;

        public static string GetCSharpQueryExpression(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings settings)
        {
            if (settings.CodeGenerators.QExFlavor == QExFlavorEnum.LCGconstants)
            {
                throw new ArgumentOutOfRangeException("Flavor", "LCG is not yet implemented.");
            }
            var codegenerator = new QueryExpressionCodeGenerator(QEx, entities, settings);
            switch (settings.CodeGenerators.QExStyle)
            {
                case QExStyleEnum.QueryExpression:
                    return new QExVanilla(codegenerator).Generated;

                case QExStyleEnum.QueryExpressionFactory:
                    return new QExFactory(codegenerator).Generated;

                default:
                    throw new NotImplementedException();
            }
        }

        private QueryExpressionCodeGenerator(QueryExpression QEx, List<EntityMetadata> entities, FXBSettings fxbsettings)
        {
            varList = new List<string>();
            entityaliases = new Dictionary<string, string>();
            metas = entities;
            qex = QEx;
            settings = fxbsettings.CodeGenerators;
            StoreLinkEntityAliases(qex.LinkEntities);
        }

        private void StoreLinkEntityAliases(DataCollection<LinkEntity> linkEntities)
        {
            foreach (var link in linkEntities)
            {
                if (!string.IsNullOrWhiteSpace(link.EntityAlias))
                {
                    entityaliases.Add(link.EntityAlias, link.LinkToEntityName);
                }
                StoreLinkEntityAliases(link.LinkEntities);
            }
        }

        internal string GetVarName(string requestedname)
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

        internal string ReplaceValueTokens(string code)
        {
            if (!code.Contains("<<<"))
            {
                return code;
            }
            var variables = new StringBuilder();
            if (settings.IncludeComments)
            {
                variables.AppendLine("// Set Condition Values");
            }
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

        internal static string GetQueryOptions(QueryExpression qex)
        {
            var queryoptions = new List<string>();
            if (qex.NoLock)
            {
                queryoptions.Add("NoLock = true");
            }
            if (qex.Distinct)
            {
                queryoptions.Add("Distinct = true");
            }
            if (qex.TopCount != null)
            {
                queryoptions.Add("TopCount = " + qex.TopCount.ToString());
            }
            if (!string.IsNullOrWhiteSpace(qex.PageInfo?.PagingCookie))
            {
                queryoptions.Add($"PageInfo = new PagingInfo{CRLF}{Indent}{{{CRLF}{Indent}{Indent}PageNumber = {qex.PageInfo.PageNumber},{CRLF}{Indent}{Indent}PagingCookie = \"{qex.PageInfo.PagingCookie}\"{CRLF}{Indent}}}");
            }
            var options = queryoptions.Count > 0 ? CRLF + "{" + QueryExpressionCodeGenerator.GetCodeParametersMaxWidth(0, 1, queryoptions.ToArray()) + "}" : "";
            return options;
        }

        internal static string GetCodeParametersMaxWidth(int maxwidth, int indents, params string[] parameters)
        {
            var currentindent = string.Concat(Enumerable.Repeat(Indent, indents));
            var result = string.Join("`´", parameters.Where(p => !string.IsNullOrWhiteSpace(p)));
            if (result.Length > maxwidth)
            {
                result = CRLF + currentindent + result.Replace("`´", $",{CRLF}{currentindent}") + CRLF;
            }
            else
            {
                result = result.Replace("`´", ", ");
            }
            return result;
        }

        internal static string GetConditionValues(DataCollection<object> values, string token, bool createvariables)
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
                if (createvariables)
                {
                    if (values.Count == 1)
                    {
                        strings.Add($"<<<{token}|{valuestr}>>>");
                    }
                    else
                    {
                        strings.Add($"<<<{token}_{i++}|{valuestr}>>>");
                    }
                }
                else
                {
                    strings.Add(valuestr);
                }
            }
            return string.Join(", ", strings);
        }

        internal string GetCodeEntity(string entityname)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity)
            {
                if (settings.QExFlavor == QExFlavorEnum.EBGconstants)
                {
                    return entity.SchemaName + "." + settings.EBG_EntityLogicalNames;
                }
                else if (settings.QExFlavor == QExFlavorEnum.EarlyBound)
                {
                    return entity.SchemaName;
                }
            }
            return "\"" + entityname + "\"";
        }

        internal string GetCodeEntityPrefix(string entityname)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity)
            {
                return entity.DisplayName.UserLocalizedLabel.Label.Substring(0, 1).ToLowerInvariant();
            }
            return entityname.Substring(0, 1).ToLowerInvariant();
        }

        internal string GetCodeAttribute(string entityname, string attributename)
        {
            if (metas.FirstOrDefault(e => e.LogicalName.Equals(entityname)) is EntityMetadata entity &&
                entity.Attributes.FirstOrDefault(a => a.LogicalName.Equals(attributename)) is AttributeMetadata attribute)
            {
                if (settings.QExFlavor == QExFlavorEnum.EBGconstants)
                {
                    return entity.SchemaName + "." + settings.EBG_AttributeLogicalNameClass + attribute.SchemaName;
                }
                else if (settings.QExFlavor == QExFlavorEnum.EarlyBound)
                {
                    return GetCodeEntityPrefix(entityname) + "." + attribute.SchemaName;
                }
            }
            return "\"" + attributename + "\"";
        }
    }

    public class QExStyle
    {
        public QExStyleEnum Tag;
        public string Creator;
        public string HelpUrl;
        public List<QExFlavor> Flavors;

        public override string ToString() => $"{Tag} ({Creator})";

        internal static object[] GetComboBoxItems()
        {
            return new object[]
            {
                new QExStyle { Tag = QExStyleEnum.QueryExpression, Creator = "Microsoft SDK" },
                new QExStyle { Tag = QExStyleEnum.FluentQueryExpression, Creator = "MscrmTools" },
                new QExStyle { Tag = QExStyleEnum.QueryExpressionFactory, Creator = "DLaB" }
            };
        }
    }

    public class QExFlavor
    {
        public string Name;
        public QExFlavorEnum Tag;
        public string HelpUrl;

        public override string ToString() => $"{Name}";

        internal static object[] GetComboBoxItems()
        {
            return new object[]
            {
                new QExFlavor { Name = "Late Bound strings", Tag = QExFlavorEnum.LateBound },
                new QExFlavor { Name = "Late Bound EBG constants", Tag = QExFlavorEnum.EBGconstants },
                new QExFlavor { Name = "Late Bound LCG constants", Tag = QExFlavorEnum.LCGconstants },
                new QExFlavor { Name = "Early Bound", Tag = QExFlavorEnum.EarlyBound }
            };
        }
    }

    public enum QExStyleEnum
    {
        QueryExpression,
        FluentQueryExpression,
        QueryExpressionFactory
    }

    public enum QExFlavorEnum
    {
        LateBound,
        EBGconstants,
        LCGconstants,
        EarlyBound
    }
}