using System;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class ODataControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder fxb;
        private int version;

        public ODataControl(FetchXmlBuilder fetchXmlBuilder, int version)
        {
            fxb = fetchXmlBuilder;
            this.version = version;
            InitializeComponent();
            Text = $"OData v{version}.0";
            TabText = Text;
        }

        internal void DisplayOData(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                var prefix = version == 4 ? "WebAPI: " : "OData: ";
                linkOData.Text = prefix + url;
                linkOData.LinkArea = new LinkArea(prefix.Length, url.Length);
                if (linkOData.Links.Count > 0)
                {
                    linkOData.Links[0].LinkData = url;
                }
            }
            else
            {
                linkOData.Text = url;
                linkOData.Links.Clear();
            }
        }

        private void ODataControl_DockStateChanged(object sender, EventArgs e)
        {
            DockPanel.DockBottomPortion = 80;
            DockPanel.DockTopPortion = 80;
            if (!IsHidden)
            {
                DisplayOData(fxb.GetOData(version));
            }
        }

        private void menuODataExecute_Click(object sender, EventArgs e)
        {
            if (linkOData.Links.Count > 0 && linkOData.Links[0].Enabled)
            {
                FetchXmlBuilder.OpenURL(linkOData.Links[0].LinkData as string);
                fxb.LogUse("ExecuteOData" + version);
            }
            else
            {
                MessageBox.Show("No link to execute");
            }
        }

        private void menuODataCopy_Click(object sender, EventArgs e)
        {
            if (linkOData.Links.Count > 0 && linkOData.Links[0].Enabled)
            {
                Clipboard.SetText(linkOData.Links[0].LinkData as string);
                fxb.LogUse("CopyOData" + version);
            }
            else
            {
                MessageBox.Show("No link to copy");
            }
        }

        private void linkOData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Link.Enabled)
            {
                FetchXmlBuilder.OpenURL(e.Link.LinkData as string);
                fxb.LogUse("ExecuteOData" + version);
            }
        }
    }
}