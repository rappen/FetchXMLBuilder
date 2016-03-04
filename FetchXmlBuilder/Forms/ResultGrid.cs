using Cinteros.Xrm.CRMWinForm;
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class ResultGrid : Form
    {
        FetchXmlBuilder form;

        public ResultGrid(EntityCollection entities, FetchXmlBuilder fetchXmlBuilder)
        {
            InitializeComponent();
            form = fetchXmlBuilder;
            var size = form.currentSettings.gridWinSize;
            if (size != null && size.Width > 0 && size.Height > 0)
            {
                Width = size.Width;
                Height = size.Height; ;
            }
            menuFriendly.Checked = form.currentSettings.gridFriendly;
            menuIdColumn.Checked = form.currentSettings.gridId;
            menuIndexColumn.Checked = form.currentSettings.gridIndex;
            crmGridView1.ShowFriendlyNames = form.currentSettings.gridFriendly;
            crmGridView1.ShowIdColumn = form.currentSettings.gridId;
            crmGridView1.ShowIndexColumn = form.currentSettings.gridIndex;
            crmGridView1.OrganizationService = form.Service;
            crmGridView1.DataSource = entities;
        }

        private void ResultGrid_Load(object sender, EventArgs e)
        {   // Must be done after form has become visible
            crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader);
        }

        private void crmGridView1_RecordDoubleClick(object sender, CRMRecordEventArgs e)
        {
            if (e.Entity != null)
            {
                var url = form.ConnectionDetail.WebApplicationUrl;
                if (string.IsNullOrEmpty(url))
                {
                    url = string.Concat(form.ConnectionDetail.ServerName, "/", form.ConnectionDetail.Organization);
                    if (!url.ToLower().StartsWith("http"))
                    {
                        url = string.Concat("http://", url);
                    }
                }
                var entity = e.Entity.LogicalName;
                var id = e.Entity.Id;
                switch (e.Entity.LogicalName)
                {
                    case "activitypointer":
                            if (!e.Entity.Contains("activitytypecode"))
                            {
                                MessageBox.Show("To open records of type activitypointer, attribute 'activitytypecode' must be included in the query.", "Open Record", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            entity = e.Entity["activitytypecode"].ToString();
                        break;
                    case "activityparty":
                        if (!e.Entity.Contains("partyid"))
                        {
                            MessageBox.Show("To open records of type activityparty, attribute 'partyid' must be included in the query.", "Open Record", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var party = (EntityReference)e.Entity["partyid"];
                        entity = party.LogicalName;
                        id = party.Id;
                        break;
                }
                if (!string.IsNullOrEmpty(entity) && !id.Equals(Guid.Empty))
                {
                    url = string.Concat(url,
                        "/main.aspx?etn=",
                        entity,
                        "&pagetype=entityrecord&id=",
                        id.ToString());
                    form.LogUse("OpenRecord");
                    Process.Start(url);
                }
            }
        }

        private void ResultGrid_FormClosing(object sender, FormClosingEventArgs e)
        {
            form.currentSettings.gridWinSize = new System.Drawing.Size(Width, Height);
            form.currentSettings.gridFriendly = menuFriendly.Checked;
            form.currentSettings.gridId = menuIdColumn.Checked;
            form.currentSettings.gridIndex = menuIndexColumn.Checked;
        }

        private void menuFriendly_CheckedChanged(object sender, EventArgs e)
        {
            crmGridView1.ShowFriendlyNames = menuFriendly.Checked;
        }

        private void menuIdColumn_CheckedChanged(object sender, EventArgs e)
        {
            crmGridView1.ShowIdColumn = menuIdColumn.Checked;
        }

        private void menuIndexColumn_CheckedChanged(object sender, EventArgs e)
        {
            crmGridView1.ShowIndexColumn = menuIndexColumn.Checked;
        }
    }
}
