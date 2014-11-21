using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class XmlContentDisplayDialog : Form
    {
        public XmlContentDisplayDialog(string xmlString, string header)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(header))
            {
                lblHeader.Text = header;
            }
            txtXML.Text = xmlString;
            txtXML.Process(true);
        }
    }
}
