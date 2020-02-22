using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System.Collections.Generic;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class valueControl : FetchXmlElementControlBase
    {
        public valueControl() : this(null, null)
        {
        }

        public valueControl(Dictionary<string, string> collection, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, null, tree, null);
        }
    }
}