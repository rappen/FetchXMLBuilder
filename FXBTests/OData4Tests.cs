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
using Microsoft.Xrm.Sdk.Metadata;

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

        [TestMethod]
        public void LeftOuterJoinParentLink()
        {
            var fetch = @"
                <fetch>
                    <entity name='account'>
                        <attribute name='name' />
                        <link-entity name='contact' from='contactid' to='primarycontactid' link-type='outer'>
                            <attribute name='firstname' />
                        </link-entity>
                    </entity>
                </fetch>";

            var odata = ConvertFetchToOData(fetch);

            Assert.AreEqual("https://example.crm.dynamics.com/api/data/v9.0/accounts?$select=name&$expand=primarycontactid($select=firstname)", odata);
        }

        [TestMethod]
        public void LeftOuterJoinChildLink()
        {
            var fetch = @"
                <fetch>
                    <entity name='account'>
                        <attribute name='name' />
                        <link-entity name='contact' from='parentcustomerid' to='accountid' link-type='outer'>
                            <attribute name='firstname' />
                        </link-entity>
                    </entity>
                </fetch>";

            var odata = ConvertFetchToOData(fetch);

            Assert.AreEqual("https://example.crm.dynamics.com/api/data/v9.0/accounts?$select=name&$expand=account_contacts($select=firstname)", odata);
        }

        private string ConvertFetchToOData(string fetch)
        {
            var context = new XrmFakedContext();

            // FakeXrmEasy doesn't currently implement RetrieveMetadataChangesRequest - implement that here
            context.AddFakeMessageExecutor<RetrieveMetadataChangesRequest>(new RetrieveMetadataChangesRequestExecutor());

            // Add basic metadata
            var relationships = new[]
            {
                new OneToManyRelationshipMetadata
                {
                    SchemaName = "account_contacts",
                    ReferencedEntity = "account",
                    ReferencedAttribute = "accountid",
                    ReferencingEntity = "contact",
                    ReferencingAttribute = "parentcustomerid"
                },
                new OneToManyRelationshipMetadata
                {
                    SchemaName = "account_primarycontact",
                    ReferencedEntity = "contact",
                    ReferencedAttribute = "contactid",
                    ReferencingEntity = "account",
                    ReferencingAttribute = "primarycontactid"
                }
            };

            var entities = new[]
            {
                new EntityMetadata
                {
                    LogicalName = "account",
                    LogicalCollectionName = "accounts"
                },
                new EntityMetadata
                {
                    LogicalName = "contact",
                    LogicalCollectionName = "contacts"
                }
            };

            var attributes = new Dictionary<string, AttributeMetadata[]>
            {
                ["account"] = new AttributeMetadata[]
                {
                    new UniqueIdentifierAttributeMetadata
                    {
                        LogicalName = "accountid"
                    },
                    new StringAttributeMetadata
                    {
                        LogicalName = "name"
                    },
                    new LookupAttributeMetadata
                    {
                        LogicalName = "primarycontactid",
                        Targets = new[] { "contact" }
                    }
                },
                ["contact"] = new AttributeMetadata[]
                {
                    new UniqueIdentifierAttributeMetadata
                    {
                        LogicalName = "contactid"
                    },
                    new StringAttributeMetadata
                    {
                        LogicalName = "firstname"
                    },
                    new LookupAttributeMetadata
                    {
                        LogicalName = "parentcustomerid",
                        Targets = new[] { "account", "contact" }
                    }
                }
            };

            SetRelationships(entities, relationships);
            SetAttributes(entities, attributes);

            foreach (var entity in entities)
                context.SetEntityMetadata(entity);

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

        private void SetAttributes(EntityMetadata[] entities, Dictionary<string, AttributeMetadata[]> attributes)
        {
            foreach (var entity in entities)
            {
                SetSealedProperty(entity, nameof(EntityMetadata.Attributes), attributes[entity.LogicalName]);
            }
        }

        private void SetRelationships(EntityMetadata[] entities, OneToManyRelationshipMetadata[] relationships)
        {
            foreach (var relationship in relationships)
            {
                relationship.ReferencingEntityNavigationPropertyName = relationship.ReferencingAttribute;
                relationship.ReferencedEntityNavigationPropertyName = relationship.SchemaName;
            }

            foreach (var entity in entities)
            {
                var oneToMany = relationships.Where(r => r.ReferencedEntity == entity.LogicalName).ToArray();
                var manyToOne = relationships.Where(r => r.ReferencingEntity == entity.LogicalName).ToArray();

                SetSealedProperty(entity, nameof(EntityMetadata.OneToManyRelationships), oneToMany);
                SetSealedProperty(entity, nameof(EntityMetadata.ManyToOneRelationships), manyToOne);
            }
        }

        private void SetSealedProperty(object target, string name, object value)
        {
            var prop = target.GetType().GetProperty(name);
            prop.SetValue(target, value, null);
        }
    }
}
