using Microsoft.Xrm.Sdk.Metadata;
using MsCrmTools.MetadataBrowser.AppCode;
using MsCrmTools.MetadataBrowser.AppCode.AttributeMd;
using System.Diagnostics;

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
                lblInfo1Value.Text= fxb.GetEntityDisplayName(ent.LogicalName);
                panInfo1.Visible = true;
                panInfo2.Visible = false;
                propMeta.SelectedObject = new EntityMetadataInfo(ent);
            }
            else if (meta is AttributeMetadata att)
            {
                lblInfo1Value.Text = fxb.GetEntityDisplayName(att.EntityLogicalName);
                lblInfo2Value.Text = fxb.GetAttributeDisplayName(att.EntityLogicalName, att.LogicalName);
                panInfo1.Visible = true;
                panInfo2.Visible = true;
                propMeta.SelectedObject = new AttributeMetadataInfo(att);
            }
            else
            {
                panInfo1.Visible = false;
                panInfo2.Visible = false;
                propMeta.SelectedObject = null;
            }
        }

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/MscrmTools/MsCrmTools.MetadataBrowser/wiki");
        }
    }
}
