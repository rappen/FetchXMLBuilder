using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Views;
using System;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class SelectMLDialog : Form
    {
        private FetchXmlBuilder Caller;
        public Entity View;

        public SelectMLDialog(FetchXmlBuilder caller)
        {
            InitializeComponent();
            Caller = caller;
            txtFetch.ConfigureForXml(caller.settings);
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
            QElist.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            QElist.Criteria.AddCondition("type", ConditionOperator.Equal, true);
            QElist.Criteria.AddCondition("query", ConditionOperator.NotNull);

            try
            {
                var lists = Caller.RetrieveMultiple(QElist);
                foreach (var list in lists.Entities)
                {
                    cmbML.Items.Add(new ViewItem(list));
                }
            }
            catch (Exception ex)
            {
                Caller.ShowErrorDialog(ex, "Load Lists", null, false);
            }
            Enabled = true;
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
                txtFetch.FormatXML(((ViewItem)cmbML.SelectedItem).GetFetch(), Caller.settings);
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
            txtFetch.Text = "";
            UpdateMLs();
        }
    }
}