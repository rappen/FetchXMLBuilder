using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class fetchControl : FetchXmlElementControlBase
    {
        public fetchControl() : this(null, null)
        {
        }

        public fetchControl(Dictionary<string, string> collection, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, null, tree, null);
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            textBox4.Text = textBox4.Text.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");
        }
    }
}