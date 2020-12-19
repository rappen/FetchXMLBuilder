using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cinteros.Xrm.FetchXmlBuilder;
using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using FakeXrmEasy;
using McTools.Xrm.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace FXBTests
{
    [TestClass]
    public class OData4Tests
    {
        [TestMethod]
        public void SimpleQuery()
        {
            var fetch = @"
                <fetch>
                    <entity name='account'>
                        <attribute name='name' />
                    </entity>
                </fetch>";

            var odata = ConvertFetchToOData(fetch);

            Assert.AreEqual("https://example.crm.dynamics.com/api/data/v9.0/accounts?$select=name", odata);
        }

        private string ConvertFetchToOData(string fetch)
        {
            var context = new XrmFakedContext();
            context.InitializeMetadata(Assembly.GetExecutingAssembly());
            context.AddFakeMessageExecutor<RetrieveMetadataChangesRequest>(new RetrieveMetadataChangesRequestExecutor());

            foreach (var entity in context.CreateMetadataQuery())
            {
                entity.LogicalCollectionName = entity.LogicalName + "s";
                context.SetEntityMetadata(entity);
            }

            var org = context.GetOrganizationService();

            var serializer = new XmlSerializer(typeof(FetchType));
            using (var input = new StringReader(fetch))
            {
                var parsed = (FetchType)serializer.Deserialize(input);

                var fxb = new FetchXmlBuilder(true);
                var con = new ConnectionDetail { OrganizationVersion = "9.0.0.0" };
                fxb.UpdateConnection(org, con, String.Empty, null);

                return OData4CodeGenerator.GetOData4Query(parsed, $"https://example.crm.dynamics.com/api/data/v{con.OrganizationMajorVersion}.{con.OrganizationMinorVersion}", fxb);
            }
        }
    }
}
