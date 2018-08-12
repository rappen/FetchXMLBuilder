using Microsoft.Xrm.Sdk.Metadata;
using System;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class EntityRelationship : IComparable
    {
        public RelationshipMetadataBase Relationship;
        public string Name = "";

        public EntityRelationship(RelationshipMetadataBase relation, string originatingEntity, string originatingParent = "")
        {
            Relationship = relation;
            if (relation is OneToManyRelationshipMetadata)
            {
                var om = (OneToManyRelationshipMetadata)relation;
                Name = FetchXmlBuilder.GetEntityDisplayName(om.ReferencingEntity) + "." + om.ReferencingAttribute + " -> " +
                    FetchXmlBuilder.GetEntityDisplayName(om.ReferencedEntity) + "." + om.ReferencedAttribute;
            }
            else if (relation is ManyToManyRelationshipMetadata)
            {
                var mm = (ManyToManyRelationshipMetadata)relation;
                if (mm.Entity1LogicalName == originatingEntity)
                {
                    Name = mm.IntersectEntityName + "." + mm.Entity1IntersectAttribute + " -> " + mm.Entity1IntersectAttribute;
                }
                else if (mm.Entity2LogicalName == originatingEntity)
                {
                    Name = mm.IntersectEntityName + "." + mm.Entity2IntersectAttribute + " -> " + mm.Entity2IntersectAttribute;
                }
                else if (mm.IntersectEntityName == originatingEntity)
                {
                    if (mm.Entity1LogicalName == originatingParent)
                    {
                        Name = mm.Entity2IntersectAttribute + " -> " + mm.Entity2LogicalName + "." + mm.Entity2IntersectAttribute;
                    }
                    else if (mm.Entity2LogicalName == originatingParent)
                    {
                        Name = mm.Entity1IntersectAttribute + " -> " + mm.Entity1LogicalName + "." + mm.Entity1IntersectAttribute;
                    }
                }
                if (string.IsNullOrEmpty(Name))
                {
                    Name = "? " + mm.IntersectEntityName + ": " + mm.Entity1LogicalName + "." + mm.Entity1IntersectAttribute + " -> " +
                        mm.Entity2LogicalName + "." + mm.Entity2IntersectAttribute;
                }
            }
        }

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