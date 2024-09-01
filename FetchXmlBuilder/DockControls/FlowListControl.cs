using Rappen.XTB.FetchXmlBuilder.Converters;
using Rappen.XTB.Helpers;
using System;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    public partial class FlowListControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder fxb;
        private string fetchxmlformated;

        public FlowListControl(FetchXmlBuilder fetchXmlBuilder)
        {
            fxb = fetchXmlBuilder;
            InitializeComponent();
            TabText = Text;
        }

        internal void DisplayFlowList(string fetchxml)
        {
            fetchxmlformated = fetchxml;
            Uri uri;
            try
            {
                var odataurl = ODataCodeGenerator.ConvertToOData4(fetchxml, fxb);
                try
                {
                    uri = new Uri(odataurl);
                    lblError.Visible = false;
                }
                catch
                {
                    uri = new Uri("https://fetchxmlbuilder.com");
                    lblError.Text = odataurl;
                    lblError.Visible = true;
                    lblError.BringToFront();
                }
            }
            catch (Exception ex)
            {
                uri = new Uri("https://fetchxmlbuilder.com");
                lblError.Text = ex.Message;
                lblError.Visible = true;
                lblError.BringToFront();
            }
            fetchxml = string.Join(" ", fetchxml.Split('\n').Select(a => a.Trim()));
            var logicalnamecollection = uri.Segments.Last();
            var displaycollectionname = fxb.entities.FirstOrDefault(e => e.LogicalCollectionName == logicalnamecollection)?.DisplayCollectionName?.UserLocalizedLabel?.Label ?? logicalnamecollection;
            SetLink(displaycollectionname, linkTable);
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$select"), linkSelect);
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$filter"), linkFilter);
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$orderby"), linkOrder);
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$expand"), linkExpand);
            SetLink(fetchxml, linkFetchXml);
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$top"), linkTop);
            tt.SetToolTip(linkFetchXml, fetchxmlformated);
        }

        private void SetLink(string text, LinkLabel link)
        {
            link.Text = text;
        }

        private void FlowListControl_DockStateChanged(object sender, EventArgs e)
        {
            if (!IsHidden)
            {
                DisplayFlowList(fxb.dockControlBuilder.GetFetchString(true, false));
                Height = 185;
            }
            if (DockState != WeifenLuo.WinFormsUI.Docking.DockState.Unknown &&
                DockState != WeifenLuo.WinFormsUI.Docking.DockState.Hidden)
            {
                fxb.settings.DockStates.FlowList = DockState;
            }
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tm.Stop();
            var text = sender == linkFetchXml ? fetchxmlformated : (sender as LinkLabel)?.Text;
            Clipboard.SetText(text);
            lblCopied.Text = $"Copied {(sender as Control)?.Tag}";
            lblCopied.Visible = true;
            tm.Start();
        }

        private void Tm_Tick(object sender, EventArgs e)
        {
            lblCopied.Visible = false;
        }

        private void LinkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            fxb.OpenUrl("https://learn.microsoft.com/en-us/connectors/commondataserviceforapps/#list-rows");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            fxb.OpenUrl("https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/working-with-flows");
        }

        private void helpIcon_Click(object sender, EventArgs e)
        {
            fxb.OpenUrl(tt.GetToolTip(sender as Control));
        }
    }
}