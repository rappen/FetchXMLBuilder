using System;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class ShareLink : Form
    {
        private string fetch;
        private string conn;

        public static void Open(string fetchxml, string connection)
        {
            new ShareLink
            {
                fetch = fetchxml,
                conn = connection
            }.ShowDialog();
        }

        private ShareLink()
        {
            InitializeComponent();
        }

        private void ShareLink_Load(object sender, System.EventArgs e)
        {
            SetLink();
        }

        private void SetLink()
        {
            var plugin = $"/plugin:\"FetchXML Builder\" ";
            var connection = $"/connection:\"{conn}\" ";
            var data = $"/data:{fetch}";
            var link = plugin + (chkConnection.Checked ? connection : "") + data;
            link = $"xrmtoolbox://{Uri.EscapeDataString(link)}";
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

        private void linkInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.OpenURL("https://fetchxmlbuilder.com/sharing-queries/");
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtLink.Text);
            Close();
        }

        private void settings_CheckedChanged(object sender, EventArgs e)
        {
            SetLink();
        }
    }
}