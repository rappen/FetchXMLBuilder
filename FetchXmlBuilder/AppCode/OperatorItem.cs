using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class OperatorItem : IComboBoxItem
    {
        ConditionOperator oper = ConditionOperator.Equal;

        /// <summary>Property that indicates what type the value must have for the condition to be valid</summary>
        public AttributeTypeCode? ValueType { get { return GetValueType(); } }
        /// <summary>Property that indicates what type the attribute must be of for the condition to be valid</summary>
        public AttributeTypeCode? AttributeType { get { return GetAttributeType(); } }

        public OperatorItem(ConditionOperator Operator)
        {
            oper = Operator;
        }

        public string GetValue()
        {
            switch (oper)
            {
                case ConditionOperator.Equal:
                    return "eq";
                case ConditionOperator.NotEqual:
                    return "neq";
                case ConditionOperator.GreaterThan:
                    return "gt";
                case ConditionOperator.LessThan:
                    return "lt";
                case ConditionOperator.GreaterEqual:
                    return "ge";
                case ConditionOperator.LessEqual:
                    return "le";
                case ConditionOperator.EqualUserId:
                    return "eq-userid";
                case ConditionOperator.NotEqualUserId:
                    return "ne-userid";
                case ConditionOperator.EqualBusinessId:
                    return "eq-businessid";
                case ConditionOperator.NotEqualBusinessId:
                    return "ne-businessid";
                case ConditionOperator.EqualUserLanguage:
                    return "eq-userlanguage";
                case ConditionOperator.DoesNotBeginWith:
                    return "not-begin-with";
                case ConditionOperator.DoesNotEndWith:
                    return "not-end-with";
                case ConditionOperator.EqualUserTeams:
                    return "eq-userteams";
                case ConditionOperator.EqualUserOrUserTeams:
                    return "eq-useroruserteams";
                case ConditionOperator.Last7Days:
                    return "last-seven-days";
                case ConditionOperator.Next7Days:
                    return "next-seven-days";
            }
            var coname = oper.ToString();
            var result = coname.Substring(0, 1).ToLowerInvariant();
            for (var i = 1; i < coname.Length; i++)
            {
                var chr = coname.Substring(i, 1);
                if (chr.ToUpperInvariant() == chr)
                {
                    result += "-" + chr.ToLowerInvariant();
                }
                else
                {
                    result += chr;
                }
            }
            return result;
        }

        public override string ToString()
        {
            return System.Enum.GetName(typeof(ConditionOperator), oper);
        }

        private AttributeTypeCode? GetValueType()
        {
            // Default type to indicate "it depends on the attribute"
            AttributeTypeCode? result = AttributeTypeCode.ManagedProperty;

            switch (oper)
            {
                case ConditionOperator.EqualUserId:
                case ConditionOperator.NotEqualUserId:
                case ConditionOperator.EqualBusinessId:
                case ConditionOperator.NotEqualBusinessId:
                case ConditionOperator.EqualUserLanguage:
                case ConditionOperator.EqualUserTeams:
                case ConditionOperator.EqualUserOrUserTeams:
                case ConditionOperator.In:
                case ConditionOperator.NotIn:
                case ConditionOperator.Between:
                case ConditionOperator.NotBetween:
                case ConditionOperator.Null:
                case ConditionOperator.NotNull:
                case ConditionOperator.Yesterday:
                case ConditionOperator.Today:
                case ConditionOperator.Tomorrow:
                case ConditionOperator.Last7Days:
                case ConditionOperator.Next7Days:
                case ConditionOperator.LastWeek:
                case ConditionOperator.ThisWeek:
                case ConditionOperator.NextWeek:
                case ConditionOperator.LastMonth:
                case ConditionOperator.ThisMonth:
                case ConditionOperator.NextMonth:
                case ConditionOperator.LastYear:
                case ConditionOperator.ThisYear:
                case ConditionOperator.NextYear:
                case ConditionOperator.ThisFiscalYear:
                case ConditionOperator.ThisFiscalPeriod:
                case ConditionOperator.NextFiscalYear:
                case ConditionOperator.NextFiscalPeriod:
                case ConditionOperator.LastFiscalYear:
                case ConditionOperator.LastFiscalPeriod:
                    result = null;
                    break;
                case ConditionOperator.Like:
                case ConditionOperator.NotLike:
                case ConditionOperator.Contains:
                case ConditionOperator.DoesNotContain:
                    result = AttributeTypeCode.String;
                    break;
                case ConditionOperator.On:
                case ConditionOperator.OnOrBefore:
                case ConditionOperator.OnOrAfter:
                case ConditionOperator.NotOn:
                    result = AttributeTypeCode.DateTime;
                    break;
                case ConditionOperator.LastXHours:
                case ConditionOperator.NextXHours:
                case ConditionOperator.LastXDays:
                case ConditionOperator.NextXDays:
                case ConditionOperator.LastXWeeks:
                case ConditionOperator.NextXWeeks:
                case ConditionOperator.LastXMonths:
                case ConditionOperator.NextXMonths:
                case ConditionOperator.LastXYears:
                case ConditionOperator.NextXYears:
                case ConditionOperator.OlderThanXMonths:
                case ConditionOperator.LastXFiscalYears:
                case ConditionOperator.LastXFiscalPeriods:
                case ConditionOperator.NextXFiscalYears:
                case ConditionOperator.NextXFiscalPeriods:
                    result = AttributeTypeCode.Integer;
                    break;
            }
            return result;
        }

        private AttributeTypeCode? GetAttributeType()
        {
            // Default type to indicate "it depends on the attribute"
            AttributeTypeCode? result = null;

            switch (oper)
            {
                case ConditionOperator.EqualUserId:
                case ConditionOperator.NotEqualUserId:
                case ConditionOperator.EqualUserTeams:
                case ConditionOperator.EqualUserOrUserTeams:
                case ConditionOperator.EqualBusinessId:
                case ConditionOperator.NotEqualBusinessId:
                case ConditionOperator.EqualUserLanguage:
                    result = AttributeTypeCode.Lookup;
                    break;
                case ConditionOperator.Yesterday:
                case ConditionOperator.Today:
                case ConditionOperator.Tomorrow:
                case ConditionOperator.Last7Days:
                case ConditionOperator.Next7Days:
                case ConditionOperator.LastWeek:
                case ConditionOperator.ThisWeek:
                case ConditionOperator.NextWeek:
                case ConditionOperator.LastMonth:
                case ConditionOperator.ThisMonth:
                case ConditionOperator.NextMonth:
                case ConditionOperator.LastYear:
                case ConditionOperator.ThisYear:
                case ConditionOperator.NextYear:
                case ConditionOperator.ThisFiscalYear:
                case ConditionOperator.ThisFiscalPeriod:
                case ConditionOperator.NextFiscalYear:
                case ConditionOperator.NextFiscalPeriod:
                case ConditionOperator.LastFiscalYear:
                case ConditionOperator.LastFiscalPeriod:
                case ConditionOperator.On:
                case ConditionOperator.OnOrBefore:
                case ConditionOperator.OnOrAfter:
                case ConditionOperator.NotOn:
                case ConditionOperator.LastXHours:
                case ConditionOperator.NextXHours:
                case ConditionOperator.LastXDays:
                case ConditionOperator.NextXDays:
                case ConditionOperator.LastXWeeks:
                case ConditionOperator.NextXWeeks:
                case ConditionOperator.LastXMonths:
                case ConditionOperator.NextXMonths:
                case ConditionOperator.LastXYears:
                case ConditionOperator.NextXYears:
                case ConditionOperator.OlderThanXMonths:
                case ConditionOperator.LastXFiscalYears:
                case ConditionOperator.LastXFiscalPeriods:
                case ConditionOperator.NextXFiscalYears:
                case ConditionOperator.NextXFiscalPeriods:
                    result = AttributeTypeCode.DateTime;
                    break;
                case ConditionOperator.Like:
                case ConditionOperator.NotLike:
                case ConditionOperator.Contains:
                case ConditionOperator.DoesNotContain:
                    result = AttributeTypeCode.String;
                    break;
            }
            return result;
        }
    }

    //public enum OperatorValueType
    //{
    //    Null,
    //    AttributeType,
    //    String,
    //    Int,
    //    Date,
    //}
}
/*

case ConditionOperator.Equal:
case ConditionOperator.NotEqual:
case ConditionOperator.GreaterThan:
case ConditionOperator.LessThan:
case ConditionOperator.GreaterEqual:
case ConditionOperator.LessEqual:
case ConditionOperator.Like:
case ConditionOperator.NotLike:
case ConditionOperator.In:
case ConditionOperator.NotIn:
case ConditionOperator.Between:
case ConditionOperator.NotBetween:
case ConditionOperator.Null:
case ConditionOperator.NotNull:
case ConditionOperator.Yesterday:
case ConditionOperator.Today:
case ConditionOperator.Tomorrow:
case ConditionOperator.Last7Days:
case ConditionOperator.Next7Days:
case ConditionOperator.LastWeek:
case ConditionOperator.ThisWeek:
case ConditionOperator.NextWeek:
case ConditionOperator.LastMonth:
case ConditionOperator.ThisMonth:
case ConditionOperator.NextMonth:
case ConditionOperator.On:
case ConditionOperator.OnOrBefore:
case ConditionOperator.OnOrAfter:
case ConditionOperator.LastYear:
case ConditionOperator.ThisYear:
case ConditionOperator.NextYear:
case ConditionOperator.LastXHours:
case ConditionOperator.NextXHours:
case ConditionOperator.LastXDays:
case ConditionOperator.NextXDays:
case ConditionOperator.LastXWeeks:
case ConditionOperator.NextXWeeks:
case ConditionOperator.LastXMonths:
case ConditionOperator.NextXMonths:
case ConditionOperator.LastXYears:
case ConditionOperator.NextXYears:
case ConditionOperator.EqualUserId:
case ConditionOperator.NotEqualUserId:
case ConditionOperator.EqualBusinessId:
case ConditionOperator.NotEqualBusinessId:
case ConditionOperator.ChildOf:
case ConditionOperator.Mask:
case ConditionOperator.NotMask:
case ConditionOperator.MasksSelect:
case ConditionOperator.Contains:
case ConditionOperator.DoesNotContain:
case ConditionOperator.EqualUserLanguage:
case ConditionOperator.NotOn:
case ConditionOperator.OlderThanXMonths:
case ConditionOperator.BeginsWith:
case ConditionOperator.DoesNotBeginWith:
case ConditionOperator.EndsWith:
case ConditionOperator.DoesNotEndWith:
case ConditionOperator.ThisFiscalYear:
case ConditionOperator.ThisFiscalPeriod:
case ConditionOperator.NextFiscalYear:
case ConditionOperator.NextFiscalPeriod:
case ConditionOperator.LastFiscalYear:
case ConditionOperator.LastFiscalPeriod:
case ConditionOperator.LastXFiscalYears:
case ConditionOperator.LastXFiscalPeriods:
case ConditionOperator.NextXFiscalYears:
case ConditionOperator.NextXFiscalPeriods:
case ConditionOperator.InFiscalYear:
case ConditionOperator.InFiscalPeriod:
case ConditionOperator.InFiscalPeriodAndYear:
case ConditionOperator.InOrBeforeFiscalPeriodAndYear:
case ConditionOperator.InOrAfterFiscalPeriodAndYear:
case ConditionOperator.EqualUserTeams:
case ConditionOperator.EqualUserOrUserTeams:

*/