using Cinteros.Xrm.CRMWinForm;
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class ResultGrid : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        FetchXmlBuilder form;

        public ResultGrid(EntityCollection entities, FetchXmlBuilder fetchXmlBuilder)
        {
            InitializeComponent();
            form = fetchXmlBuilder;
            chkFriendly.Checked = form.currentSettings.gridFriendly;
            chkIdCol.Checked = form.currentSettings.gridId;
            chkIndexCol.Checked = form.currentSettings.gridIndex;
            chkCopyHeaders.Checked = form.currentSettings.gridCopyHeaders;
            ApplySettingsToGrid();
            SetData(entities);
        }

        internal void SetData(EntityCollection entities)
        {
            switch (DockState)
            {
                case WeifenLuo.WinFormsUI.Docking.DockState.Unknown:
                case WeifenLuo.WinFormsUI.Docking.DockState.Hidden:
                    Show(form.dockContainer, WeifenLuo.WinFormsUI.Docking.DockState.Document);
                    break;
                case WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide:
                    DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
                    break;
                case WeifenLuo.WinFormsUI.Docking.DockState.DockTopAutoHide:
                    DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockTop;
                    break;
                case WeifenLuo.WinFormsUI.Docking.DockState.DockLeftAutoHide:
                    DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
                    break;
                case WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide:
                    DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
                    break;
            }
            crmGridView1.DataSource = entities;
            crmGridView1.Refresh();
            crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }

        private void ApplySettingsToGrid()
        {
            crmGridView1.ShowFriendlyNames = form.currentSettings.gridFriendly;
            crmGridView1.ShowIdColumn = form.currentSettings.gridId;
            crmGridView1.ShowIndexColumn = form.currentSettings.gridIndex;
            crmGridView1.ClipboardCopyMode = form.currentSettings.gridCopyHeaders ?
                DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText : DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            crmGridView1.OrganizationService = form.Service;
            crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }

        private void UpddateSettingsFromGrid()
        {
            form.currentSettings.gridFriendly = chkFriendly.Checked;
            form.currentSettings.gridIndex = chkIndexCol.Checked;
            form.currentSettings.gridId = chkIdCol.Checked;
            form.currentSettings.gridCopyHeaders = chkCopyHeaders.Checked;
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

        private void chkGridOptions_CheckedChanged(object sender, EventArgs e)
        {
            UpddateSettingsFromGrid();
        }

        private void ResultGrid_DockStateChanged(object sender, EventArgs e)
        {
            if (DockState == WeifenLuo.WinFormsUI.Docking.DockState.Unknown)
            {
                if (this == form.resultGrid)
                {
                    form.resultGrid = null;
                }
            }
        }
    }
}
