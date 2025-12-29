using Rappen.XTB.Helpers;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class ShareLink : Form
    {
        private const string toolname = "FetchXML Builder";
        private string dataparam;
        private string connection;

        public static void Open(PluginControlBase tool, string param)
        {
            new ShareLink
            {
                dataparam = param,
                connection = tool.ConnectionDetail?.ConnectionName
            }.ShowDialog();
        }

        private ShareLink()
        {
            InitializeComponent();
        }

        private void ShareLink_Load(object sender, System.EventArgs e)
        {
            chkConnection.Text = $"Include connection {connection}";
            SetLink();
        }

        private void SetLink()
        {
            var plugin = $"/plugin%3A{Encoded("\"" + toolname + "\"")} ";
            var connection = chkConnection.Checked ? $"/connection%3A{Encoded("\"" + this.connection + "\"")} " : "";
            var data = $"/data%3A{Encoded("\"" + dataparam + "\"")}";
            var link = $"xrmtoolbox://{plugin + connection + data}";
            if (rbUrl.Checked)
            {
                txtLink.Text = link;
            }
            else if (rbHtml.Checked)
            {
                txtLink.Text = $"<a href=\"{link}\">{txtLinkName.Text}</a>";
            }
            else if (rbMarkdown.Checked)
            {
                txtLink.Text = $"[{txtLinkName.Text}]({link})";
            }
            else
            {
                txtLink.Text = "";
            }
        }

        private string Encoded(string param) => rbSafeLink.Checked ? Uri.EscapeDataString(param) : param;

        private void linkInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl("https://fetchxmlbuilder.com/sharing-queries/");
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtLink.Text);
            Close();
            MessageBoxEx.Show(this, "Link is copied!", "Share Query via XrmToolBox", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void settings_CheckedChanged(object sender, EventArgs e)
        {
            txtLinkName.Enabled = !rbUrl.Checked;
            SetLink();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Process.Start(txtLink.Text);
        }
    }
}