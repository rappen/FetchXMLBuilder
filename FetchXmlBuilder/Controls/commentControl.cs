using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System.Collections.Generic;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class commentControl : FetchXmlElementControlBase
    {
        public commentControl() : this(null, null)
        {
        }

        public commentControl(Dictionary<string, string> collection, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, null, tree, null);
        }

        public override void Focus()
        {
            textBox1.Focus();
        }
    }
}