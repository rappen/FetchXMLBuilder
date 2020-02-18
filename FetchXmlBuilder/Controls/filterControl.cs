using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class filterControl : FetchXmlElementControlBase
    {
        public filterControl() : this(null, null)
        {
        }

        public filterControl(Dictionary<string, string> collection, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, null, tree, null);
        }
    }
}