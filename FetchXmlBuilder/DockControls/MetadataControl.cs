using Microsoft.Xrm.Sdk.Metadata;
using MsCrmTools.MetadataBrowser.AppCode;
using MsCrmTools.MetadataBrowser.AppCode.AttributeMd;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class MetadataControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder fxb;

        public MetadataControl(FetchXmlBuilder fetchXmlBuilder)
        {
            fxb = fetchXmlBuilder;
            InitializeComponent();
        }

        internal void UpdateMeta(MetadataBase meta)
        {
            if (meta is EntityMetadata ent)
            {
                Text = $"Meta Entity {fxb.GetEntityDisplayName(ent.LogicalName)}";
                propMeta.SelectedObject = new EntityMetadataInfo(ent);
            }
            else if (meta is AttributeMetadata att)
            {
                Text = $"Meta Attribute {fxb.GetAttributeDisplayName(att.EntityLogicalName, att.LogicalName)}";
                propMeta.SelectedObject = new AttributeMetadataInfo(att);
            }
            else
            {
                Text = "Metadata";
                propMeta.SelectedObject = null;
            }
        }
    }
}
