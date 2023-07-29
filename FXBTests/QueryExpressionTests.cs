using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Converters;
using Rappen.XTB.FetchXmlBuilder.Settings;

namespace FXBTests
{
    [TestClass]
    public class QueryExpressionTests
    {
        [TestMethod]
        public void StandardCondition()
        {
            var qe = new QueryExpression("contact");
            qe.ColumnSet = new ColumnSet("fullname", "lastname");
            qe.Criteria.AddCondition("modifiedon", ConditionOperator.GreaterThan, "2020-01-01");

            var converted = CSharpCodeGenerator.GetCSharpQueryExpression(qe, null, new FXBSettings());

            var expected = @"// Define Condition Values
var query_modifiedon = ""2020-01-01"";

// Instantiate QueryExpression query
var query = new QueryExpression(""contact"");

// Add columns to query.ColumnSet
query.ColumnSet.AddColumns(""fullname"", ""lastname"");

// Define filter query.Criteria
query.Criteria.AddCondition(""modifiedon"", ConditionOperator.GreaterThan, query_modifiedon);
";

            Assert.AreEqual(expected, converted);
        }

        [TestMethod]
        public void ColumnComparisonCondition()
        {
            var qe = new QueryExpression("contact");
            qe.ColumnSet = new ColumnSet("fullname", "lastname");
            qe.Criteria.AddCondition(new ConditionExpression("modifiedon", ConditionOperator.GreaterThan, true, "createdon"));

            var converted = CSharpCodeGenerator.GetCSharpQueryExpression(qe, null, new FXBSettings());

            var expected = @"// Define Condition Values
var query_modifiedon = ""createdon"";

// Instantiate QueryExpression query
var query = new QueryExpression(""contact"");

// Add columns to query.ColumnSet
query.ColumnSet.AddColumns(""fullname"", ""lastname"");

// Define filter query.Criteria
query.Criteria.AddCondition(""modifiedon"", ConditionOperator.GreaterThan, true, query_modifiedon);
";

            Assert.AreEqual(expected, converted);
        }

        [TestMethod]
        public void MultipleComparisonsOnSameField()
        {
            var qe = new QueryExpression("contact");
            qe.ColumnSet = new ColumnSet("fullname", "lastname");
            qe.Criteria.AddCondition("modifiedon", ConditionOperator.GreaterThan, "2020-01-01");
            qe.Criteria.AddCondition(new ConditionExpression("modifiedon", ConditionOperator.GreaterThan, true, "createdon"));

            var converted = CSharpCodeGenerator.GetCSharpQueryExpression(qe, null, new FXBSettings());

            var expected = @"// Define Condition Values
var query_modifiedon = ""2020-01-01"";
var query_modifiedon1 = ""createdon"";

// Instantiate QueryExpression query
var query = new QueryExpression(""contact"");

// Add columns to query.ColumnSet
query.ColumnSet.AddColumns(""fullname"", ""lastname"");

// Define filter query.Criteria
query.Criteria.AddCondition(""modifiedon"", ConditionOperator.GreaterThan, query_modifiedon);
query.Criteria.AddCondition(""modifiedon"", ConditionOperator.GreaterThan, true, query_modifiedon1);
";

            Assert.AreEqual(expected, converted);
        }
    }
}