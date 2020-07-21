using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class OperatorItem : IComboBoxItem
    {
        private ConditionOperator oper = ConditionOperator.Equal;

        /// <summary>Property that indicates what type the value must have for the condition to be valid</summary>
        public AttributeTypeCode? ValueType { get { return GetValueType(); } }

        /// <summary>Property that indicates if operator allows "values" collection</summary>
        public bool IsMultipleValuesType { get { return GetIsMultipleValuesType(); } }

        /// <summary>Property that indicates if operator allows column comparison</summary>
        public bool SupportsColumnComparison { get { return GetSupportsColumnComparison(); } }

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
                case ConditionOperator.EqualUserOrUserHierarchy:
                    return "eq-useroruserhierarchy";
                case ConditionOperator.EqualUserOrUserHierarchyAndTeams:
                    return "eq-useroruserhierarchyandteams";
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
                case ConditionOperator.AboveOrEqual:
                    return "eq-or-above";
                case ConditionOperator.UnderOrEqual:
                    return "eq-or-under";
            }
            var coname = oper.ToString();
            if (coname.StartsWith("OlderThan"))
            {   // "olderthan" is written together, not "older-than" in fetchxml
                coname = coname.Replace("OlderThan", "Olderthan");
            }
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
                case ConditionOperator.EqualUserOrUserHierarchy:
                case ConditionOperator.EqualUserOrUserHierarchyAndTeams:
                case ConditionOperator.EqualBusinessId:
                case ConditionOperator.NotEqualBusinessId:
                case ConditionOperator.EqualUserLanguage:
                case ConditionOperator.EqualUserTeams:
                case ConditionOperator.EqualUserOrUserTeams:
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
                case ConditionOperator.OlderThanXYears:
                case ConditionOperator.OlderThanXMonths:
                case ConditionOperator.OlderThanXWeeks:
                case ConditionOperator.OlderThanXDays:
                case ConditionOperator.OlderThanXHours:
                case ConditionOperator.OlderThanXMinutes:
                case ConditionOperator.LastXFiscalYears:
                case ConditionOperator.LastXFiscalPeriods:
                case ConditionOperator.NextXFiscalYears:
                case ConditionOperator.NextXFiscalPeriods:
                case ConditionOperator.InFiscalPeriod:
                case ConditionOperator.InFiscalPeriodAndYear:
                case ConditionOperator.InFiscalYear:
                case ConditionOperator.InOrAfterFiscalPeriodAndYear:
                case ConditionOperator.InOrBeforeFiscalPeriodAndYear:
                    result = AttributeTypeCode.Integer;
                    break;
            }
            return result;
        }

        private bool GetIsMultipleValuesType()
        {
            switch (oper)
            {
                case ConditionOperator.In:
                case ConditionOperator.NotIn:
                case ConditionOperator.Between:
                case ConditionOperator.NotBetween:
                case ConditionOperator.InFiscalPeriodAndYear:
                case ConditionOperator.InOrAfterFiscalPeriodAndYear:
                case ConditionOperator.InOrBeforeFiscalPeriodAndYear:
                    return true;
            }
            return false;
        }

        private bool GetSupportsColumnComparison()
        {
            switch (oper)
            {
                case ConditionOperator.Equal:
                case ConditionOperator.NotEqual:
                case ConditionOperator.GreaterThan:
                case ConditionOperator.GreaterEqual:
                case ConditionOperator.LessThan:
                case ConditionOperator.LessEqual:
                    return true;
            }
            return false;
        }

        private AttributeTypeCode? GetAttributeType()
        {
            // Default type to indicate "it depends on the attribute"
            AttributeTypeCode? result = null;

            switch (oper)
            {
                case ConditionOperator.EqualUserId:
                case ConditionOperator.NotEqualUserId:
                case ConditionOperator.EqualUserOrUserHierarchy:
                case ConditionOperator.EqualUserOrUserHierarchyAndTeams:
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
                case ConditionOperator.OlderThanXYears:
                case ConditionOperator.OlderThanXMonths:
                case ConditionOperator.OlderThanXWeeks:
                case ConditionOperator.OlderThanXDays:
                case ConditionOperator.OlderThanXHours:
                case ConditionOperator.OlderThanXMinutes:
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

        public static OperatorItem[] GetConditionsByAttributeType(AttributeTypeCode valueType)
        {
            var validConditionsList = new List<OperatorItem>
            {
                new OperatorItem(ConditionOperator.Equal),
                new OperatorItem(ConditionOperator.NotEqual),
                new OperatorItem(ConditionOperator.In),
                new OperatorItem(ConditionOperator.NotIn),
                new OperatorItem(ConditionOperator.Null),
                new OperatorItem(ConditionOperator.NotNull)
            };

            if (valueType != AttributeTypeCode.Boolean &&
                valueType != AttributeTypeCode.DateTime &&
                valueType != AttributeTypeCode.Integer &&
                valueType != AttributeTypeCode.State &&
                valueType != AttributeTypeCode.Status &&
                valueType != AttributeTypeCode.Picklist &&
                valueType != AttributeTypeCode.BigInt &&
                valueType != AttributeTypeCode.Decimal &&
                valueType != AttributeTypeCode.Double &&
                valueType != AttributeTypeCode.Money &&
                valueType != AttributeTypeCode.Money &&
                valueType != AttributeTypeCode.Lookup &&
                valueType != AttributeTypeCode.Customer &&
                valueType != AttributeTypeCode.Owner &&
                valueType != AttributeTypeCode.Uniqueidentifier)
            {
                validConditionsList.Add(new OperatorItem(ConditionOperator.BeginsWith));
                validConditionsList.Add(new OperatorItem(ConditionOperator.DoesNotBeginWith));
                validConditionsList.Add(new OperatorItem(ConditionOperator.Contains));
                validConditionsList.Add(new OperatorItem(ConditionOperator.DoesNotContain));
                validConditionsList.Add(new OperatorItem(ConditionOperator.EndsWith));
                validConditionsList.Add(new OperatorItem(ConditionOperator.DoesNotEndWith));
                validConditionsList.Add(new OperatorItem(ConditionOperator.Like));
                validConditionsList.Add(new OperatorItem(ConditionOperator.NotLike));
            }
            if (valueType == AttributeTypeCode.DateTime ||
                valueType == AttributeTypeCode.Integer ||
                valueType == AttributeTypeCode.State ||
                valueType == AttributeTypeCode.Status ||
                valueType == AttributeTypeCode.Picklist ||
                valueType == AttributeTypeCode.BigInt ||
                valueType == AttributeTypeCode.Decimal ||
                valueType == AttributeTypeCode.Double ||
                valueType == AttributeTypeCode.Money)
            {
                validConditionsList.Add(new OperatorItem(ConditionOperator.Between));
                validConditionsList.Add(new OperatorItem(ConditionOperator.NotBetween));
                validConditionsList.Add(new OperatorItem(ConditionOperator.GreaterThan));
                validConditionsList.Add(new OperatorItem(ConditionOperator.GreaterEqual));
                validConditionsList.Add(new OperatorItem(ConditionOperator.LessThan));
                validConditionsList.Add(new OperatorItem(ConditionOperator.LessEqual));
            }
            switch (valueType)
            {
                case AttributeTypeCode.DateTime:
                    validConditionsList.Add(new OperatorItem(ConditionOperator.InFiscalPeriod));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.InFiscalPeriodAndYear));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.InFiscalYear));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.InOrAfterFiscalPeriodAndYear));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.InOrBeforeFiscalPeriodAndYear));

                    validConditionsList.Add(new OperatorItem(ConditionOperator.Last7Days));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastFiscalPeriod));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastFiscalYear));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastMonth));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastWeek));

                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastXDays));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastXFiscalPeriods));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastXFiscalYears));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastXHours));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastXMonths));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastXWeeks));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastXYears));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.LastYear));

                    validConditionsList.Add(new OperatorItem(ConditionOperator.Next7Days));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextFiscalPeriod));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextFiscalYear));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextMonth));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextWeek));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextXDays));

                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextXFiscalPeriods));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextXFiscalYears));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextXHours));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextXMonths));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextXWeeks));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextXYears));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NextYear));

                    validConditionsList.Add(new OperatorItem(ConditionOperator.OlderThanXYears));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.OlderThanXMonths));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.OlderThanXWeeks));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.OlderThanXDays));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.OlderThanXHours));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.OlderThanXMinutes));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.On));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.OnOrAfter));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.OnOrBefore));

                    validConditionsList.Add(new OperatorItem(ConditionOperator.ThisFiscalPeriod));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.ThisFiscalYear));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.ThisMonth));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.ThisWeek));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.ThisYear));

                    validConditionsList.Add(new OperatorItem(ConditionOperator.Today));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.Tomorrow));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.Yesterday));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NotOn));

                    break;
                case AttributeTypeCode.Uniqueidentifier:
                    validConditionsList.Add(new OperatorItem(ConditionOperator.Above));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.AboveOrEqual));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.Under));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.UnderOrEqual));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NotUnder));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.EqualBusinessId));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.EqualUserId));
                    break;
                case AttributeTypeCode.Owner:
                    validConditionsList.Add(new OperatorItem(ConditionOperator.EqualBusinessId));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.EqualUserId));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.NotEqualUserId));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.EqualUserOrUserHierarchy));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.EqualUserOrUserHierarchyAndTeams));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.EqualUserOrUserTeams));
                    validConditionsList.Add(new OperatorItem(ConditionOperator.EqualUserTeams));
                    break;
            }
            return validConditionsList.ToArray();
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