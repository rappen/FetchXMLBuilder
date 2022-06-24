using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk;
using System;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class SelectViewDialog : Form
    {
        private FetchXmlBuilder Caller;
        public Entity View;

        public SelectViewDialog(FetchXmlBuilder caller)
        {
            InitializeComponent();
            Caller = caller;
            txtFetch.ConfigureForXml(caller.settings);
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
                    if (entity.IsIntersect != true && Caller.views.ContainsKey(entity.LogicalName + "|S"))
                    {
                        var ei = new EntityItem(entity);
                        cmbEntity.Items.Add(ei);
                        if (entity.LogicalName == Caller.settings.LastOpenedViewEntity)
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
            lblNotCusomizable.Visible = false;
            btnOk.Enabled = false;
            var entity = ControlUtils.GetValueFromControl(cmbEntity);
            object selectedItem = null;
            if (Caller.views.ContainsKey(entity + "|S"))
            {
                var views = Caller.views[entity + "|S"];
                cmbView.Items.Add("-- System Views --");
                foreach (var view in views)
                {
                    var vi = new ViewItem(view);
                    cmbView.Items.Add(vi);
                    if (view.Id.Equals(Caller.settings.LastOpenedViewId))
                    {
                        selectedItem = vi;
                    }
                }
            }
            if (Caller.views.ContainsKey(entity + "|U"))
            {
                var views = Caller.views[entity + "|U"];
                cmbView.Items.Add("-- Personal Views --");
                foreach (var view in views)
                {
                    var vi = new ViewItem(view);
                    cmbView.Items.Add(vi);
                    if (view.Id.Equals(Caller.settings.LastOpenedViewId))
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
                Caller.settings.LastOpenedViewEntity = ControlUtils.GetValueFromControl(cmbEntity);
                Caller.settings.LastOpenedViewId = View.Id;
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
            if (cmbView.SelectedItem is ViewItem viewitem)
            {
                txtFetch.FormatXML(viewitem.GetFetch(), Caller.settings);
                lblNotCusomizable.Visible = !viewitem.IsCustomizable;
                btnOk.Enabled = true;
            }
            else
            {
                txtFetch.Text = "";
                lblNotCusomizable.Visible = false;
                btnOk.Enabled = false;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Enabled = false;
            cmbView.SelectedIndex = -1;
            cmbEntity.SelectedIndex = -1;
            txtFetch.Text = "";
            Caller.views = null;
            Caller.LoadViews(PopulateForm);
        }

        private void cmbEntity_KeyDown(object sender, KeyEventArgs e)
        {
            cmbEntity.DroppedDown = false;
        }
    }
}