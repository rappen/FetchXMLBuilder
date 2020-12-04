using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Query;

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

            var converted = QueryExpressionCodeGenerator.GetCSharpQueryExpression(qe);

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

            var converted = QueryExpressionCodeGenerator.GetCSharpQueryExpression(qe);

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
    }
}
