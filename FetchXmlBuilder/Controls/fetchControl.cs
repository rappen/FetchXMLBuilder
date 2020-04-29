using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System;
using System.Collections.Generic;

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

        private void textPagingCookie_Leave(object sender, EventArgs e)
        {
            textPagingCookie.Text = textPagingCookie.Text.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");
        }


    }
}