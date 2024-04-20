using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XTB.Helpers.Interfaces;

namespace Rappen.XTB.FetchXmlBuilder.ControlsClasses
{
    public class EntityNameItem : IXRMControlItem
    {
        private EntityMetadata meta = null;

        public EntityNameItem(EntityMetadata Entity)
        {
            meta = Entity;
        }

        public override string ToString() => FetchXmlBuilder.GetEntityDisplayName(meta);

        public string GetValue() => meta.ObjectTypeCode.Value.ToString();
    }
}