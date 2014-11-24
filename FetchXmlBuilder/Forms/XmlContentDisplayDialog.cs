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

        public XmlContentDisplayDialog(string xmlString, string header, bool allowEdit)
        {
            InitializeComponent();
            Text = string.IsNullOrEmpty(header) ? "FetchXmlBuilder" : header;
            panBottom.Visible = allowEdit;
            txtXML.Text = xmlString;
            FormatXML();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormatXML();
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

        private void FormatXML()
        {
            try
            {
                txtXML.Process(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
