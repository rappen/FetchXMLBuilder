using Microsoft.Xrm.Sdk.Metadata;

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
            metadataControl1.SetMeta(meta);
        }
    }
}
