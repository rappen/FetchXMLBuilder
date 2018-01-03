using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class XmlContentDisplayDialog : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private string findtext = "";
        private FetchXmlBuilder fxb;
        private ContentType contenttype;
        private SaveFormat format;
        private List<KeyValuePair<string, int>> groupBoxHeights;
        private ToolTip tt;

        internal static XmlContentDisplayDialog ShowDialog(string xmlString, ContentType contentType, SaveFormat saveFormat, FetchXmlBuilder caller)
        {
            if (xmlString.Length > 100000)
            {
                var dlgresult = MessageBox.Show("Huge result, this may take a while!\n" + xmlString.Length.ToString() + " characters in the XML document.\n\nContinue?", "Huge result",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dlgresult == DialogResult.No)
                {
                    return null;
                }
            }
            var xcdDialog = new XmlContentDisplayDialog(contentType, false, saveFormat, caller);
            xcdDialog.panCancel.Visible = true;
            xcdDialog.UpdateXML(xmlString);
            xcdDialog.StartPosition = FormStartPosition.CenterParent;
            xcdDialog.ShowDialog();
            return xcdDialog;
        }

        internal XmlContentDisplayDialog(FetchXmlBuilder caller) : this(ContentType.FetchXML, true, SaveFormat.XML, caller) { }

        internal XmlContentDisplayDialog(ContentType contentType, bool allowEdit, SaveFormat saveFormat, FetchXmlBuilder caller)
        {
            InitializeComponent();
            groupBoxHeights = this.GetAll(typeof(GroupBox)).Select(g => new KeyValuePair<string, int>(g.Name, g.GetDockedContainer().Height)).ToList();
            tt = new ToolTip();
            contenttype = contentType;
            format = saveFormat;
            fxb = caller;
            Text = contenttype.ToString().Replace("_", " ").Replace("CSharp", "C#");
            TabText = Text;
            txtXML.KeyUp += fxb.LiveXML_KeyUp;
            panLiveUpdate.Visible = allowEdit;
            panOk.Visible = allowEdit;
            panFormatting.Visible = allowEdit;
            panExecute.Visible = allowEdit;
            panSave.Visible = format != SaveFormat.None;
            var windowSettings = fxb.currentSettings.ContentWindows.GetContentWindow(contenttype);
            chkLiveUpdate.Checked = allowEdit && windowSettings.LiveUpdate;
            GroupBoxSetState(lblFormatExpander, windowSettings.FormatExpanded);
            GroupBoxSetState(lblActionsExpander, windowSettings.ActionExpanded);
            UpdateButtons();
        }

        protected override string GetPersistString()
        {
            return GetPersistString(contenttype);
        }

        internal static string GetPersistString(ContentType type)
        {
            return typeof(XmlContentDisplayDialog).ToString() + "." + type.ToString();
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            FormatXML(false);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            fxb.dockControlBuilder.Init(txtXML.Text, "manual edit", true);
        }

        private void FormatXML(bool silent)
        {
            try
            {
                txtXML.Process();
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show(ex.Message, "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void XmlContentDisplayDialog_KeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox textBox = txtXML;
            findtext = FindTextHandler.HandleFindKeyPress(e, textBox, findtext);
        }

        public void UpdateXML(string xmlString)
        {
            txtXML.Text = xmlString;
            txtXML.Settings.QuoteCharacter = fxb.currentSettings.useSingleQuotation ? '\'' : '"';
            FormatXML(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            fxb.FetchResults(txtXML.Text);
        }

        private void XmlContentDisplayDialog_Load(object sender, EventArgs e)
        {
            panActions.Visible = gbActions.Controls.Cast<Control>().Any(c => c.Visible);
            if (DialogResult == DialogResult.Cancel)
            {
                Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = $"Save {format}",
                Filter = $"{format} file (*.{format.ToString().ToLowerInvariant()})|*.{format.ToString().ToLowerInvariant()}"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                txtXML.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
                MessageBox.Show($"{format} saved to {sfd.FileName}");
            }
        }

        private void FormatAsXML()
        {
            if (FetchIsHtml())
            {
                txtXML.Text = HttpUtility.HtmlDecode(txtXML.Text.Trim());
            }
            else if (FetchIsEscaped())
            {
                txtXML.Text = Uri.UnescapeDataString(txtXML.Text.Trim());
            }
            else
            {
                if (MessageBox.Show("Unrecognized encoding, unsure what to do with it.\n" +
                    "Currently FXB can handle htmlencoded and urlescaped strings.\n\n" +
                    "Would you like to submit an issue to FetchXML Builder to be able to handle this?",
                    "Decode FetchXML", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/Innofactor/FetchXMLBuilder/issues/new");
                }
                return;
            }
            FormatXML(false);
        }

        private void FormatAsHtml()
        {
            var response = MessageBox.Show("Strip spaces from encoded XML?", "Encode XML", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (response == DialogResult.Cancel)
            {
                UpdateButtons();
                return;
            }
            if (!FetchIsPlain())
            {
                FormatAsXML();
            }
            var xml = response == DialogResult.Yes ? GetCompactXml() : txtXML.Text;
            txtXML.Text = HttpUtility.HtmlEncode(xml);
        }

        private void FormatAsEsc()
        {
            if (!FetchIsPlain())
            {
                FormatAsXML();
            }
            txtXML.Text = Uri.EscapeDataString(GetCompactXml());
        }

        private string GetCompactXml()
        {
            if (!FetchIsPlain())
            {
                FormatAsXML();
            }
            var xml = txtXML.Text;
            while (xml.Contains(" <")) xml = xml.Replace(" <", "<");
            while (xml.Contains(" >")) xml = xml.Replace(" >", ">");
            while (xml.Contains(" />")) xml = xml.Replace(" />", "/>");
            return xml.Trim();
        }

        private bool FetchIsPlain()
        {
            return txtXML.Text.Trim().ToLowerInvariant().StartsWith("<fetch");
        }

        private bool FetchIsHtml()
        {
            return txtXML.Text.Trim().ToLowerInvariant().StartsWith("&lt;fetch");
        }

        private bool FetchIsEscaped()
        {
            return txtXML.Text.Trim().ToLowerInvariant().StartsWith("%3cfetch");
        }

        private void txtXML_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var plain = FetchIsPlain();
            rbFormatEsc.Checked = FetchIsEscaped();
            rbFormatHTML.Checked = FetchIsHtml();
            rbFormatXML.Checked = plain;
            btnFormat.Enabled = plain;
            btnExecute.Enabled = plain && !chkLiveUpdate.Checked;
            btnOk.Enabled = !chkLiveUpdate.Checked;
        }

        private void XmlContentDisplayDialog_DockStateChanged(object sender, EventArgs e)
        {
            if (DockState != WeifenLuo.WinFormsUI.Docking.DockState.Unknown &&
                DockState != WeifenLuo.WinFormsUI.Docking.DockState.Hidden)
            {
                switch (contenttype)
                {
                    case ContentType.FetchXML:
                        fxb.currentSettings.dockStates.FetchXML = DockState;
                        break;
                    case ContentType.CSharp_Query:
                        fxb.currentSettings.dockStates.FetchXMLCs = DockState;
                        break;
                    case ContentType.JavaScript_Query:
                        fxb.currentSettings.dockStates.FetchXMLJs = DockState;
                        break;
                    case ContentType.QueryExpression:
                        fxb.currentSettings.dockStates.QueryExpression = DockState;
                        break;
                    case ContentType.SQL_Query:
                        fxb.currentSettings.dockStates.SQLQuery = DockState;
                        break;
                }
            }
        }

        private void rbFormatXML_Click(object sender, EventArgs e)
        {
            FormatAsXML();
        }

        private void rbFormatHTML_Click(object sender, EventArgs e)
        {
            FormatAsHtml();
        }

        private void rbFormatEsc_Click(object sender, EventArgs e)
        {
            FormatAsEsc();
        }

        private void chkLiveUpdate_CheckedChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void XmlContentDisplayDialog_VisibleChanged(object sender, EventArgs e)
        {
            fxb.UpdateLiveXML();
        }

        private void llGroupBoxExpander_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GroupBoxToggle(sender as Label);
        }

        private void llGroupBoxExpander_Clicked(object sender, EventArgs e)
        {
            GroupBoxToggle(sender as Label);
        }

        private void GroupBoxToggle(Label link)
        {
            if (link.GetDockedContainer().Height > 20)
            {
                GroupBoxCollapse(link);
            }
            else
            {
                GroupBoxExpand(link);
            }
        }

        private void GroupBoxSetState(Label link, bool expanded)
        {
            if (expanded)
            {
                GroupBoxExpand(link);
            }
            else
            {
                GroupBoxCollapse(link);
            }
        }
        private void GroupBoxCollapse(Label link)
        {
            // ↑↓–+˄˅
            link.GetDockedContainer().Height = 18;
            link.Text = "+";
            tt.SetToolTip(link, "Open");
        }

        private void GroupBoxExpand(Label link)
        {
            link.GetDockedContainer().Height = groupBoxHeights.FirstOrDefault(g => g.Key == link.Parent.Name).Value;
            link.Text = "–";
            tt.SetToolTip(link, "Close");
        }

        private void XmlContentDisplayDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            var windowSettings = new ContentWindow
            {
                LiveUpdate = chkLiveUpdate.Checked,
                FormatExpanded = gbFormatting.GetDockedContainer().Height > 20,
                ActionExpanded = gbActions.GetDockedContainer().Height > 20
            };
            fxb.currentSettings.ContentWindows.SetContentWindow(contenttype, windowSettings);
        }
    }

    public enum ContentType
    {
        FetchXML,
        FetchXML_Result,
        Serialized_Result_XML,
        Serialized_Result_JSON,
        QueryExpression,
        SQL_Query,
        JavaScript_Query,
        CSharp_Query
    }

    internal enum SaveFormat
    {
        None = 0,
        XML = 1,
        JSON = 2,
        SQL = 3
    }
}
