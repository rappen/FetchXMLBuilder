using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class SelectViewDialog : Form
    {
        FetchXmlBuilder Caller;
        public Entity View;

        public SelectViewDialog(FetchXmlBuilder caller)
        {
            InitializeComponent();
            Caller = caller;
            cmbEntity.Items.Clear();
            var entities = FetchXmlBuilder.GetDisplayEntities();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (entity.Value.IsIntersect != true && FetchXmlBuilder.views.ContainsKey(entity.Value.LogicalName + "|S"))
                    {
                        cmbEntity.Items.Add(new EntityItem(entity.Value));
                    }
                }
            }
        }

        private void cmbEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateViews();
        }

        private void UpdateViews()
        {
            cmbView.Items.Clear();
            cmbView.Text = "";
            txtFetch.Text = "";
            btnOk.Enabled = false;
            var entity = ControlUtils.GetValueFromControl(cmbEntity);
            if (FetchXmlBuilder.views.ContainsKey(entity + "|S"))
            {
                var views = FetchXmlBuilder.views[entity + "|S"];
                cmbView.Items.Add("-- System Views --");
                foreach (var view in views)
                {
                    cmbView.Items.Add(new ViewItem(view));
                }
            }
            if (FetchXmlBuilder.views.ContainsKey(entity + "|U"))
            {
                var views = FetchXmlBuilder.views[entity + "|U"];
                cmbView.Items.Add("-- Personal Views --");
                foreach (var view in views)
                {
                    cmbView.Items.Add(new ViewItem(view));
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbView.SelectedItem is ViewItem)
            {
                View = ((ViewItem)cmbView.SelectedItem).GetView();
            }
            else
            {
                View = null;
            }
        }

        private void cmbView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbView.SelectedItem is ViewItem)
            {
                txtFetch.Text = ((ViewItem)cmbView.SelectedItem).GetFetch();
                txtFetch.Process(true);
                btnOk.Enabled = true;
            }
            else
            {
                txtFetch.Text = "";
                btnOk.Enabled = false;
            }
        }
    }
}
