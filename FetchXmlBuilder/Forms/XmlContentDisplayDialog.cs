using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class XmlContentDisplayDialog : Form
    {
        public XmlNode result;
        public bool execute;
        private string findtext = "";
        FetchXmlBuilder fxb;
        SaveFormat format;

        internal static XmlContentDisplayDialog Show(string xmlString, string header, bool allowEdit, bool allowFormat, bool allowExecute, SaveFormat saveFormat, FetchXmlBuilder caller)
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
            var xcdDialog = new XmlContentDisplayDialog(xmlString, header, allowEdit, allowFormat, allowExecute, saveFormat, caller);
            xcdDialog.StartPosition = FormStartPosition.CenterParent;
            xcdDialog.ShowDialog();
            return xcdDialog;
        }

        internal XmlContentDisplayDialog(string xmlString, string header, bool allowEdit, bool allowFormat, bool allowExecute, SaveFormat saveFormat, FetchXmlBuilder caller)
        {
            InitializeComponent();
            format = saveFormat;
            fxb = caller;
            result = null;
            execute = false;
            if (fxb.currentSettings.xmlWinSize != null && fxb.currentSettings.xmlWinSize.Width > 0 && fxb.currentSettings.xmlWinSize.Height > 0)
            {
                Width = fxb.currentSettings.xmlWinSize.Width;
                Height = fxb.currentSettings.xmlWinSize.Height;
            }
            Text = string.IsNullOrEmpty(header) ? "FetchXML Builder" : header;
            panOk.Visible = allowEdit;
            if (!allowEdit)
            {
                btnCancel.Text = "Close";
            }
            btnFormat.Visible = allowFormat;
            btnExecute.Visible = allowExecute;
            btnSave.Visible = format != SaveFormat.None;
            UpdateXML(xmlString);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormatXML(false);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SetResult();
        }

        private void SetResult()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(txtXML.Text.Replace("\n", "\r\n"));
                result = doc.DocumentElement;
            }
            catch (Exception error)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show(this, "Error while parsing Xml: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void XmlContentDisplayDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            fxb.currentSettings.xmlWinSize = new System.Drawing.Size(Width, Height);
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
            SetResult();
            execute = true;
        }

        private void XmlContentDisplayDialog_Load(object sender, EventArgs e)
        {
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
    }

    internal enum SaveFormat
    {
        None = 0,
        XML = 1,
        JSON = 2
    }
}
