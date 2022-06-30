using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk.Metadata;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class EntityNameItem : IComboBoxItem
    {
        private EntityMetadata meta = null;

        public EntityNameItem(EntityMetadata Entity)
        {
            meta = Entity;
        }

        public override string ToString()
        {
            return FetchXmlBuilder.GetEntityDisplayName(meta);
        }

        public string GetValue()
        {
            return meta.ObjectTypeCode.Value.ToString();
        }
    }
}