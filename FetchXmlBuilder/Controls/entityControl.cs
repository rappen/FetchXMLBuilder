using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class entityControl : FetchXmlElementControlBase
    {
        public entityControl() : this(new Dictionary<string, string>(), null, null)
        {
        }

        public entityControl(Dictionary<string, string> collection, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, fetchXmlBuilder, tree, null);
        }

        protected override void PopulateControls()
        {
            cmbEntity.Items.Clear();
            var entities = fxb.GetDisplayEntities();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    cmbEntity.Items.Add(new EntityItem(entity.Value));
                }
            }
        }
    }
}