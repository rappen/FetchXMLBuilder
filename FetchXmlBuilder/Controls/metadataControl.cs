using Microsoft.Xrm.Sdk.Metadata;
using MsCrmTools.MetadataBrowser.AppCode;
using MsCrmTools.MetadataBrowser.AppCode.AttributeMd;
using System.Diagnostics;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class metadataControl : UserControl
    {
        public metadataControl()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/MscrmTools/MsCrmTools.MetadataBrowser/wiki");
        }

        internal void SetMeta(MetadataBase meta)
        {
            if (meta is EntityMetadata ent)
            {
                lblInfo1Value.Text = ent.LogicalName;
                panInfo1.Visible = true;
                panInfo2.Visible = false;
                propMeta.SelectedObject = new EntityMetadataInfo(ent);
            }
            else if (meta is AttributeMetadata att)
            {
                lblInfo1Value.Text = att.EntityLogicalName;
                lblInfo2Value.Text = att.LogicalName;
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
    }
}
