using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using xrmtb.XrmToolBox.Controls;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class ResultGrid : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder form;
        private QueryInfo queryinfo;

        public ResultGrid(FetchXmlBuilder fetchXmlBuilder)
        {
            InitializeComponent();
            this.PrepareGroupBoxExpanders();
            form = fetchXmlBuilder;
            mnuFriendly.Checked = form.settings.Results.Friendly;
            mnuIdCol.Checked = form.settings.Results.Id;
            mnuIndexCol.Checked = form.settings.Results.Index;
            mnuNullCol.Checked = form.settings.Results.NullColumns;
            mnuSysCol.Checked = form.settings.Results.SysColumns;
            mnuLocalTime.Checked = form.settings.Results.LocalTime;
            mnuCopyHeaders.Checked = form.settings.Results.CopyHeaders;
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

            crmGridView1.DataSource = queryinfo.Results;
            crmGridView1.ColumnOrder = queryinfo.AttributesSignature.Trim().Replace('\n', ',');
            crmGridView1.Refresh();
            crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }

        private void ApplySettingsToGrid()
        {
            crmGridView1.ShowFriendlyNames = form.settings.Results.Friendly;
            crmGridView1.ShowIdColumn = form.settings.Results.Id;
            crmGridView1.ShowIndexColumn = form.settings.Results.Index;
            crmGridView1.ShowAllColumnsInColumnOrder = form.settings.Results.NullColumns;
            crmGridView1.ShowColumnsNotInColumnOrder = form.settings.Results.SysColumns;
            crmGridView1.ShowLocalTimes = form.settings.Results.LocalTime;
            crmGridView1.ClipboardCopyMode = form.settings.Results.CopyHeaders ?
                DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText : DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            crmGridView1.OrganizationService = form.Service;
            crmGridView1.Refresh();
            crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }

        private void UpdateSettingsFromSelectedOptions()
        {
            form.settings.Results.Friendly = mnuFriendly.Checked;
            form.settings.Results.Index = mnuIndexCol.Checked;
            form.settings.Results.NullColumns = mnuNullCol.Checked;
            form.settings.Results.SysColumns = mnuSysCol.Checked;
            form.settings.Results.Id = mnuIdCol.Checked;
            form.settings.Results.LocalTime = mnuLocalTime.Checked;
            form.settings.Results.CopyHeaders = mnuCopyHeaders.Checked;
            ApplySettingsToGrid();
        }

        private void crmGridView1_RecordDoubleClick(object sender, CRMRecordEventArgs e)
        {
            if (e.Entity != null)
            {
                var url = form.ConnectionDetail.GetEntityUrl(e.Entity);
                if (!string.IsNullOrEmpty(url))
                {
                    form.LogUse("OpenRecord");
                    Process.Start(url);
                }
            }
        }

        private void crmGridView1_RecordClick(object sender, CRMRecordEventArgs e)
        {
            if (e.Value is EntityReference)
            {
                string url = form.ConnectionDetail.GetEntityReferenceUrl(e.Value as EntityReference);
                if (!string.IsNullOrEmpty(url))
                {
                    form.LogUse("OpenParentRecord");
                    Process.Start(url);
                }
            }
        }

        private void chkGridOptions_Click(object sender, EventArgs e)
        {
            UpdateSettingsFromSelectedOptions();
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