using Cinteros.Xrm.CRMWinForm;
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using System.Linq;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class ResultGrid : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        FetchXmlBuilder form;
        QueryInfo queryinfo;

        public ResultGrid(FetchXmlBuilder fetchXmlBuilder)
        {
            InitializeComponent();
            this.PrepareGroupBoxExpanders();
            form = fetchXmlBuilder;
            chkFriendly.Checked = form.settings.Results.Friendly;
            chkIdCol.Checked = form.settings.Results.Id;
            chkIndexCol.Checked = form.settings.Results.Index;
            chkCopyHeaders.Checked = form.settings.Results.CopyHeaders;
            ApplySettingsToGrid();
        }

        internal void SetData(QueryInfo queryinfo)
        {
            this.queryinfo = queryinfo;
            var entities = queryinfo.Results;
            if (entities != null)
            {
                this.EnsureVisible(form.dockContainer, form.settings.DockStates.ResultView);
            }
            crmGridView1.DataSource = entities;
            crmGridView1.Refresh();
            ArrangeColumns();
        }

        private void ArrangeColumns()
        {
            if (queryinfo == null)
            {
                return;
            }
            var pos = 2;
            foreach (var attribute in queryinfo.AttributesSignature?.Split('\n').Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)))
            {
                if (crmGridView1.Columns.Contains(attribute))
                {
                    crmGridView1.Columns[attribute].DisplayIndex = pos;
                    pos++;
                }
            }
            crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }

        private void ApplySettingsToGrid()
        {
            crmGridView1.ShowFriendlyNames = form.settings.Results.Friendly;
            crmGridView1.ShowIdColumn = form.settings.Results.Id;
            crmGridView1.ShowIndexColumn = form.settings.Results.Index;
            crmGridView1.ClipboardCopyMode = form.settings.Results.CopyHeaders ?
                DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText : DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            crmGridView1.OrganizationService = form.Service;
            ArrangeColumns();
        }

        private void UpdateSettingsFromGrid()
        {
            form.settings.Results.Friendly = chkFriendly.Checked;
            form.settings.Results.Index = chkIndexCol.Checked;
            form.settings.Results.Id = chkIdCol.Checked;
            form.settings.Results.CopyHeaders = chkCopyHeaders.Checked;
            ApplySettingsToGrid();
        }

        private void crmGridView1_RecordDoubleClick(object sender, CRMRecordEventArgs e)
        {
            if (e.Entity != null)
            {
                string url = GetEntityUrl(e.Entity);
                if (!string.IsNullOrEmpty(url))
                {
                    form.LogUse("OpenRecord");
                    Process.Start(url);
                }
            }
        }

        private string GetEntityUrl(Entity entity)
        {
            var entref = entity.ToEntityReference();
            switch (entref.LogicalName)
            {
                case "activitypointer":
                    if (!entity.Contains("activitytypecode"))
                    {
                        MessageBox.Show("To open records of type activitypointer, attribute 'activitytypecode' must be included in the query.", "Open Record", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        entref.LogicalName = string.Empty;
                    }
                    else
                    {
                        entref.LogicalName = entity["activitytypecode"].ToString();
                    }
                    break;
                case "activityparty":
                    if (!entity.Contains("partyid"))
                    {
                        MessageBox.Show("To open records of type activityparty, attribute 'partyid' must be included in the query.", "Open Record", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        entref.LogicalName = string.Empty;
                    }
                    else
                    {
                        var party = (EntityReference)entity["partyid"];
                        entref.LogicalName = party.LogicalName;
                        entref.Id = party.Id;
                    }
                    break;
            }
            return GetEntityReferenceUrl(entref);
        }

        private string GetEntityReferenceUrl(EntityReference entref)
        {
            if (!string.IsNullOrEmpty(entref.LogicalName) && !entref.Id.Equals(Guid.Empty))
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
                url = string.Concat(url,
                    url.EndsWith("/") ? "" : "/",
                    "main.aspx?etn=",
                    entref.LogicalName,
                    "&pagetype=entityrecord&id=",
                    entref.Id.ToString());
                return url;
            }
            return string.Empty;
        }

        private void crmGridView1_RecordClick(object sender, CRMRecordEventArgs e)
        {
            if (e.Value is EntityReference)
            {
                string url = GetEntityReferenceUrl(e.Value as EntityReference);
                if (!string.IsNullOrEmpty(url))
                {
                    form.LogUse("OpenParentRecord");
                    Process.Start(url);
                }
            }
        }

        private void chkGridOptions_Click(object sender, EventArgs e)
        {
            UpdateSettingsFromGrid();
        }

        private void ResultGrid_DockStateChanged(object sender, EventArgs e)
        {
            if (DockState != WeifenLuo.WinFormsUI.Docking.DockState.Unknown &&
                DockState != WeifenLuo.WinFormsUI.Docking.DockState.Hidden)
            {
                form.settings.DockStates.ResultView = DockState;
            }
        }

        private void lblOptionsExpander_Click(object sender, EventArgs e)
        {
            (sender as System.Windows.Forms.Label)?.GroupBoxSetState(tt);
        }
    }
}
