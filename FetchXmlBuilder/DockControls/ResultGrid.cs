using Microsoft.Xrm.Sdk;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using System;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
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
            mnuQuickFilter.Checked = form.settings.Results.QuickFilter;
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
            RefreshData();
        }

        internal void ApplySettingsToGrid()
        {
            crmGridView1.ShowFriendlyNames = form.settings.Results.Friendly;
            crmGridView1.ShowIdColumn = form.settings.Results.Id;
            crmGridView1.ShowIndexColumn = form.settings.Results.Index;
            crmGridView1.ShowAllColumnsInColumnOrder = form.settings.Results.NullColumns;
            crmGridView1.ShowColumnsNotInColumnOrder = form.settings.Results.SysColumns;
            crmGridView1.ShowLocalTimes = form.settings.Results.LocalTime;
            crmGridView1.ClipboardCopyMode = form.settings.Results.CopyHeaders ?
                DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText : DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            crmGridView1.Service = form.Service;
            panQuickFilter.Visible = form.settings.Results.QuickFilter;
            RefreshData();
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
            form.settings.Results.QuickFilter = mnuQuickFilter.Checked;
            ApplySettingsToGrid();
        }

        private void RefreshData()
        {
            if (panQuickFilter.Visible)
            {
                crmGridView1.FilterColumns = crmGridView1.ColumnOrder;
                crmGridView1.FilterText = txtQuickFilter.Text;
            }
            else
            {
                crmGridView1.FilterText = string.Empty;
            }
            crmGridView1.SuspendLayout();
            crmGridView1.Refresh();
            crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            crmGridView1.ResumeLayout();
        }

        private void crmGridView1_RecordDoubleClick(object sender, Rappen.XTB.Helpers.Controls.XRMRecordEventArgs e)
        {
            if (!form.settings.Results.ClickableLinks)
            {
                return;
            }
            if (e.Value is EntityReference entref && form.ConnectionDetail.GetEntityReferenceUrl(entref) is string urlref && !string.IsNullOrEmpty(urlref))
            {
                form.LogUse("OpenParentRecord");
                form.OpenURLProfile(urlref, false);
            }
            else if (e.Entity != null && form.ConnectionDetail.GetEntityUrl(e.Entity) is string urlentity && !string.IsNullOrEmpty(urlentity))
            {
                form.LogUse("OpenRecord");
                form.OpenURLProfile(urlentity, false);
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

        private void txtQuickFilter_TextChanged(object sender, EventArgs e)
        {
            tmFilter.Stop();
            tmFilter.Start();
        }

        private void mnuQuickFilter_Click(object sender, EventArgs e)
        {
            UpdateSettingsFromSelectedOptions();
            if (panQuickFilter.Visible)
            {
                txtQuickFilter.Focus();
            }
        }

        private void tmFilter_Tick(object sender, EventArgs e)
        {
            tmFilter.Stop();
            RefreshData();
        }

        private void ctxmenuGrid_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var temp1 = new ToolStripItem[mnuBehavior.DropDownItems.Count];
            mnuBehavior.DropDownItems.CopyTo(temp1, 0);
            foreach (var item in temp1)
            {
                ctxBehavior.DropDownItems.Add(item);
            }
            var temp2 = new ToolStripItem[mnuColumns.DropDownItems.Count];
            mnuColumns.DropDownItems.CopyTo(temp2, 0);
            foreach (var item in temp2)
            {
                ctxColumns.DropDownItems.Add(item);
            }
        }

        private void ctxmenuGrid_Opened(object sender, EventArgs e)
        {
            ctxRecord.Enabled = (ctxRecordOpen.Tag is string url1 && !string.IsNullOrWhiteSpace(url1));
            ctxColumn.Enabled = (ctxColumnOpen.Tag is string url2 && !string.IsNullOrWhiteSpace(url2));
        }

        private void mnuBehaviorColumns_DropDownOpening(object sender, EventArgs e)
        {
            var temp1 = new ToolStripItem[ctxBehavior.DropDownItems.Count];
            ctxBehavior.DropDownItems.CopyTo(temp1, 0);
            foreach (var item in temp1)
            {
                mnuBehavior.DropDownItems.Add(item);
            }
            var temp2 = new ToolStripItem[ctxColumns.DropDownItems.Count];
            ctxColumns.DropDownItems.CopyTo(temp2, 0);
            foreach (var item in temp2)
            {
                mnuColumns.DropDownItems.Add(item);
            }
        }

        private void ctxFind_Click(object sender, EventArgs e)
        {
            mnuQuickFilter.Checked = true;
            mnuQuickFilter_Click(sender, e);
        }

        private void crmGridView1_RecordEnter(object sender, Rappen.XTB.Helpers.Controls.XRMRecordEventArgs e)
        {
            ctxRecordOpen.Tag = form.ConnectionDetail.GetEntityUrl(e.Entity);
            ctxRecordCopy.Tag = ctxRecordOpen.Tag;
            ctxColumnOpen.Tag = form.ConnectionDetail.GetEntityReferenceUrl(e.Value as EntityReference);
            ctxColumnCopy.Tag = ctxColumnOpen.Tag;
        }

        private void ctxOpen_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem tool && tool.Tag is string url && !string.IsNullOrWhiteSpace(url))
            {
                form.LogUse("OpenRecord");
                form.OpenURLProfile(url, false);
            }
        }

        private void ctxCopy_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem tool && tool.Tag is string url && !string.IsNullOrWhiteSpace(url))
            {
                form.LogUse("CopyRecord");
                Clipboard.SetText(url);
            }
        }
    }
}