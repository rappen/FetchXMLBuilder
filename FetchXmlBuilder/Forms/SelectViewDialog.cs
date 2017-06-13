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
            PopulateForm();
        }

        private void PopulateForm()
        {
            cmbEntity.Items.Clear();
            var entities = Caller.GetDisplayEntities();
            if (entities != null)
            {
                object selectedItem = null;
                foreach (var entity in entities)
                {
                    if (entity.Value.IsIntersect != true && FetchXmlBuilder.views.ContainsKey(entity.Value.LogicalName + "|S"))
                    {
                        var ei = new EntityItem(entity.Value);
                        cmbEntity.Items.Add(ei);
                        if (entity.Value.LogicalName == Caller.currentSettings.lastOpenedViewEntity)
                        {
                            selectedItem = ei;
                        }
                    }
                }
                if (selectedItem != null)
                {
                    cmbEntity.SelectedItem = selectedItem;
                    UpdateViews();
                }
            }
            Enabled = true;
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
            object selectedItem = null;
            if (FetchXmlBuilder.views.ContainsKey(entity + "|S"))
            {
                var views = FetchXmlBuilder.views[entity + "|S"];
                cmbView.Items.Add("-- System Views --");
                foreach (var view in views)
                {
                    var vi = new ViewItem(view);
                    cmbView.Items.Add(vi);
                    if (view.Id.Equals(Caller.currentSettings.lastOpenedViewId))
                    {
                        selectedItem = vi;
                    }
                }
            }
            if (FetchXmlBuilder.views.ContainsKey(entity + "|U"))
            {
                var views = FetchXmlBuilder.views[entity + "|U"];
                cmbView.Items.Add("-- Personal Views --");
                foreach (var view in views)
                {
                    var vi = new ViewItem(view);
                    cmbView.Items.Add(vi);
                    if (view.Id.Equals(Caller.currentSettings.lastOpenedViewId))
                    {
                        selectedItem = vi;
                    }
                }
            }
            if (selectedItem != null)
            {
                cmbView.SelectedItem = selectedItem;
                UpdateFetch();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbView.SelectedItem is ViewItem)
            {
                View = ((ViewItem)cmbView.SelectedItem).GetView();
                Caller.currentSettings.lastOpenedViewEntity = ControlUtils.GetValueFromControl(cmbEntity);
                Caller.currentSettings.lastOpenedViewId = View.Id;
            }
            else
            {
                View = null;
            }
        }

        private void cmbView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFetch();
        }

        private void UpdateFetch()
        {
            if (cmbView.SelectedItem is ViewItem)
            {
                txtFetch.Text = ((ViewItem)cmbView.SelectedItem).GetFetch();
                txtFetch.Process();
                btnOk.Enabled = true;
            }
            else
            {
                txtFetch.Text = "";
                btnOk.Enabled = false;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Enabled = false;
            cmbView.SelectedIndex = -1;
            cmbEntity.SelectedIndex = -1;
            txtFetch.Text = "";
            FetchXmlBuilder.views = null;
            Caller.LoadViews(PopulateForm);
        }

        private void cmbEntity_KeyDown(object sender, KeyEventArgs e)
        {
            cmbEntity.DroppedDown = false;
        }
    }
}
