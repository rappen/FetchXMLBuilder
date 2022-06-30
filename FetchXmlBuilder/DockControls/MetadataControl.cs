using Microsoft.Xrm.Sdk.Metadata;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
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
            metadataControl1.SelectedObject = meta;
        }
    }
}