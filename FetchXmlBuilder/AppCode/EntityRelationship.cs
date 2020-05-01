using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Linq;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class EntityRelationship : IComparable
    {
        public RelationshipMetadataBase Relationship;
        public string Name = "";

        public EntityRelationship(RelationshipMetadataBase relation, EntityRole role, string originatingEntity, FetchXmlBuilder fxb, string originatingParent = "")
        {
            Relationship = relation;
            Role = role;

            if (relation is OneToManyRelationshipMetadata)
            {
                var om = (OneToManyRelationshipMetadata)relation;

                if (role == EntityRole.Referenced)
                {
                    Name = fxb.GetEntityDisplayName(om.ReferencedEntity) + "." + om.ReferencedAttribute + " <- " +
                        fxb.GetEntityDisplayName(om.ReferencingEntity) + "." + om.ReferencingAttribute;
                }
                else
                {
                    Name = fxb.GetEntityDisplayName(om.ReferencingEntity) + "." + om.ReferencingAttribute + " -> " +
                        fxb.GetEntityDisplayName(om.ReferencedEntity) + "." + om.ReferencedAttribute;
                }
            }
            else if (relation is ManyToManyRelationshipMetadata)
            {
                var mm = (ManyToManyRelationshipMetadata)relation;

                if (fxb.NeedToLoadEntity(mm.Entity1LogicalName))
                {
                    fxb.LoadEntityDetails(mm.Entity1LogicalName, null, false);
                }

                if (fxb.NeedToLoadEntity(mm.Entity2LogicalName))
                {
                    fxb.LoadEntityDetails(mm.Entity2LogicalName, null, false);
                }

                var entity1PrimaryKey = fxb.GetPrimaryIdAttribute(mm.Entity1LogicalName);
                var entity2PrimaryKey = fxb.GetPrimaryIdAttribute(mm.Entity2LogicalName);

                if (mm.Entity1LogicalName == originatingEntity && role == EntityRole.Referencing)
                {
                    Name = fxb.GetEntityDisplayName(mm.Entity1LogicalName) + "." + entity1PrimaryKey + " -> " + mm.IntersectEntityName + "." + mm.Entity1IntersectAttribute;
                }
                else if (mm.Entity2LogicalName == originatingEntity && role == EntityRole.Referenced)
                {
                    Name = fxb.GetEntityDisplayName(mm.Entity2LogicalName) + "." + entity2PrimaryKey + " -> " + mm.IntersectEntityName + "." + mm.Entity2IntersectAttribute;
                }
                else if (mm.IntersectEntityName == originatingEntity)
                {
                    if (mm.Entity1LogicalName == originatingParent && role == EntityRole.Referencing)
                    {
                        Name = mm.IntersectEntityName + "." + mm.Entity2IntersectAttribute + " -> " + fxb.GetEntityDisplayName(mm.Entity2LogicalName) + "." + entity2PrimaryKey;
                    }
                    else if (mm.Entity2LogicalName == originatingParent && role == EntityRole.Referenced)
                    {
                        Name = mm.IntersectEntityName + "." + mm.Entity1IntersectAttribute + " -> " + fxb.GetEntityDisplayName(mm.Entity1LogicalName) + "." + entity1PrimaryKey;
                    }
                }
                if (string.IsNullOrEmpty(Name))
                {
                    Name = "? " + mm.IntersectEntityName + ": " + mm.Entity1LogicalName + "." + mm.Entity1IntersectAttribute + " -> " +
                        mm.Entity2LogicalName + "." + mm.Entity2IntersectAttribute;
                }
            }
        }
        public EntityRole Role { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(obj.ToString());
        }
    }
}