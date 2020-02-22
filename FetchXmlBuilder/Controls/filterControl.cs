using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System.Collections.Generic;

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