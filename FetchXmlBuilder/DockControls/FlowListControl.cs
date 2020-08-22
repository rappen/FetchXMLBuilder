using System;
using System.Web;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class FlowListControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder fxb;

        public FlowListControl(FetchXmlBuilder fetchXmlBuilder)
        {
            fxb = fetchXmlBuilder;
            InitializeComponent();
            TabText = Text;
        }

        internal void DisplayFlowList(string url)
        {
            Uri uri;
            try
            {
                uri = new Uri(url);
                lblError.Visible = false;
            }
            catch
            {
                uri = new Uri("https://jonasrapp.net");
                lblError.Text = url;
                lblError.Visible = true;
                lblError.BringToFront();
            }
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$apply"), linkAggr);
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$filter"), linkFilter);
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$orderby"), linkOrder);
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$expand"), linkExpand);
            SetLink(HttpUtility.ParseQueryString(uri.Query).Get("$top"), linkTop);
        }

        private void SetLink(string text, LinkLabel link)
        {
            link.Text = text;
        }

        private void FlowListControl_DockStateChanged(object sender, EventArgs e)
        {
            if (!IsHidden)
            {
                DisplayFlowList(fxb.GetOData(4));
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
            Clipboard.SetText((sender as LinkLabel)?.Text);
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
            System.Diagnostics.Process.Start("https://docs.microsoft.com/en-us/connectors/commondataservice/#list-records");
        }
    }
}