using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class ODataControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder fxb;

        public ODataControl(FetchXmlBuilder fetchXmlBuilder)
        {
            fxb = fetchXmlBuilder;
            InitializeComponent();
        }

        internal void DisplayOData(string url)
        {
            try
            {
                var prefix = "OData: ";
                linkOData.Text = prefix + url;
                linkOData.LinkArea = new LinkArea(prefix.Length, url.Length);
                if (linkOData.Links.Count > 0)
                {
                    linkOData.Links[0].LinkData = url;
                }
            }
            catch (Exception ex)
            {
                linkOData.Text = ex.Message;
                linkOData.Links.Clear();
            }
        }
        
        private void ODataControl_DockStateChanged(object sender, EventArgs e)
        {
            DockPanel.DockBottomPortion = 80;
            DockPanel.DockTopPortion = 80;
            if (!IsHidden)
            {
                DisplayOData(fxb.GetOData());
            }
        }

        private void menuODataExecute_Click(object sender, EventArgs e)
        {
            if (linkOData.Links.Count > 0 && linkOData.Links[0].Enabled)
            {
                System.Diagnostics.Process.Start(linkOData.Links[0].LinkData as string);
                fxb.LogUse("ExecuteOData");
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
                fxb.LogUse("CopyOData");
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
                System.Diagnostics.Process.Start(e.Link.LinkData as string);
                fxb.LogUse("ExecuteOData");
            }
        }

    }
}
