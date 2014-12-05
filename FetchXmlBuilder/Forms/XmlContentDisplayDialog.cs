using Cinteros.Xrm.FetchXmlBuilder.AppCode;
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
        private string findtext = "";

        public XmlContentDisplayDialog(string xmlString, string header, bool allowEdit)
        {
            InitializeComponent();
            Text = string.IsNullOrEmpty(header) ? "FetchXmlBuilder" : header;
            panOk.Visible = allowEdit;
            if (xmlString.Length > 100000)
            {
                var dlgresult = MessageBox.Show("Huge result, this may take a while!\n" + xmlString.Length.ToString() + " characters in the XML document.\n\nContinue?", "Huge result",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dlgresult == DialogResult.No)
                {
                    xmlString = "";
                }
            }
            txtXML.Text = xmlString;
            FormatXML(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormatXML(false);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(txtXML.Text);
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
                txtXML.Process(true);
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
    }
}
