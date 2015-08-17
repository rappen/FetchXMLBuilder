using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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
    public partial class SelectMLDialog : Form
    {
        FetchXmlBuilder Caller;
        public Entity View;

        public SelectMLDialog(FetchXmlBuilder caller)
        {
            InitializeComponent();
            Caller = caller;
            PopulateForm();
        }

        private void PopulateForm()
        {
            UpdateMLs();
        }

        private void UpdateMLs()
        {
            cmbML.Items.Clear();
            cmbML.Text = "";
            txtFetch.Text = "";
            btnOk.Enabled = false;

            // Instantiate QueryExpression QElist
            var QElist = new QueryExpression("list");

            // Add columns to QElist.ColumnSet
            QElist.ColumnSet.AddColumns("listname", "query");
            QElist.AddOrder("listname", OrderType.Ascending);

            //// Define filter QElist.Criteria
            //QElist.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            //QElist.Criteria.AddCondition("type", ConditionOperator.Equal, True);
            //QElist.Criteria.AddCondition("query", ConditionOperator.NotNull);

             
            //if (FetchXmlBuilder.views.ContainsKey(entity + "|S"))
            //{
            //    var views = FetchXmlBuilder.views[entity + "|S"];
            //    cmbML.Items.Add("-- System Views --");
            //    foreach (var view in views)
            //    {
            //        cmbML.Items.Add(new ViewItem(view));
            //    }
            //}
            //if (FetchXmlBuilder.views.ContainsKey(entity + "|U"))
            //{
            //    var views = FetchXmlBuilder.views[entity + "|U"];
            //    cmbML.Items.Add("-- Personal Views --");
            //    foreach (var view in views)
            //    {
            //        cmbML.Items.Add(new ViewItem(view));
            //    }
            //}
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbML.SelectedItem is ViewItem)
            {
                View = ((ViewItem)cmbML.SelectedItem).GetView();
            }
            else
            {
                View = null;
            }
        }

        private void cmbView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbML.SelectedItem is ViewItem)
            {
                txtFetch.Text = ((ViewItem)cmbML.SelectedItem).GetFetch();
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
            cmbML.SelectedIndex = -1;
            //cmbEntity.SelectedIndex = -1;
            txtFetch.Text = "";
            FetchXmlBuilder.views = null;
            Caller.LoadViews(PopulateForm);
        }
    }
}
