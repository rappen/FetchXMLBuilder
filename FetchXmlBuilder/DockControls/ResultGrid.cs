using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Views;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    public partial class ResultGrid : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder form;
        private QueryInfo queryinfo;
        private bool reloaded;
        private DateTime lasterrormessage;

        public ResultGrid(FetchXmlBuilder fetchXmlBuilder)
        {
            InitializeComponent();
            this.PrepareGroupBoxExpanders();
            form = fetchXmlBuilder;
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
            ApplySettingsToGrid();
            SetQueryIfChangesDesign();
            txtPagingCookie.Text = queryinfo.Results.PagingCookie;
        }

        internal void ApplySettingsToGrid()
        {
            mnuFriendly.Checked = form.settings.Results.Friendly;
            mnuIdCol.Checked = form.settings.Results.WorkWithLayout ? false : form.settings.Results.Id;
            mnuIndexCol.Checked = form.settings.Results.WorkWithLayout ? false : form.settings.Results.Index;
            mnuNullCol.Checked = form.settings.Results.WorkWithLayout ? true : form.settings.Results.NullColumns;
            mnuSysCol.Checked = form.settings.Results.WorkWithLayout ? false : form.settings.Results.SysColumns;
            mnuLocalTime.Checked = form.settings.Results.LocalTime;
            mnuCopyHeaders.Checked = form.settings.Results.CopyHeaders;
            mnuQuickFilter.Checked = form.settings.Results.QuickFilter;
            mnuPagingCookie.Checked = form.settings.Results.PagingCookie;

            mnuIdCol.Enabled = !form.settings.Results.WorkWithLayout;
            mnuIndexCol.Enabled = !form.settings.Results.WorkWithLayout;
            mnuNullCol.Enabled = !form.settings.Results.WorkWithLayout;
            mnuSysCol.Enabled = !form.settings.Results.WorkWithLayout;

            crmGridView1.ShowFriendlyNames = mnuFriendly.Checked;
            crmGridView1.ShowIdColumn = mnuIdCol.Checked;
            crmGridView1.ShowIndexColumn = mnuIndexCol.Checked;
            crmGridView1.ShowAllColumnsInColumnOrder = mnuNullCol.Checked;
            crmGridView1.ShowColumnsNotInColumnOrder = mnuSysCol.Checked;
            crmGridView1.ShowLocalTimes = mnuLocalTime.Checked;
            crmGridView1.ClipboardCopyMode = mnuCopyHeaders.Checked ?
                DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText : DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            crmGridView1.Service = form.Service;
            panQuickFilter.Visible = mnuQuickFilter.Checked;
            gbPagingCookie.Visible = mnuPagingCookie.Checked;
            mnuResetLayout.Visible = form.settings.Results.WorkWithLayout;
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
            form.settings.Results.PagingCookie = mnuPagingCookie.Checked;
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
            reloaded = true;
            crmGridView1.SuspendLayout();
            crmGridView1.Refresh();
            if (form.dockControlBuilder.LayoutXML == null)
            {
                crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            }
            else
            {
                SetLayoutToGrid();
            }
            crmGridView1.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Width > form.settings.Results.MaxColumnWidth)
                .ToList()
                .ForEach(c => c.Width = form.settings.Results.MaxColumnWidth);
            GetLayoutFromGrid();
            crmGridView1.ResumeLayout();
            reloaded = false;
        }

        internal void SetQueryIfChangesDesign()
        {
            var changed = queryinfo?.QuerySignature != form.dockControlBuilder?.GetTreeChecksum(null);
            Text = "Result View" + (changed ? " *" : "");
            crmGridView1.DefaultCellStyle.BackColor = changed ? System.Drawing.Color.LightGray : System.Drawing.Color.White;
            crmGridView1.DefaultCellStyle.ForeColor = changed ? System.Drawing.Color.Gray : System.Drawing.SystemColors.ControlText;
        }

        internal void SetLayoutToGrid()
        {
            if (form.dockControlBuilder?.LayoutXML?.Cells == null)
            {
                return;
            }
            reloaded = true;
            foreach (var cell in form.dockControlBuilder.LayoutXML.Cells)
            {
                var col = crmGridView1.Columns[cell.Name];
                if (col != null)
                {
                    try
                    {
                        col.DisplayIndex = Math.Min(cell.DisplayIndex, crmGridView1.Columns.Count - 1);
                        col.Width = cell.Width;
                        col.Visible = cell.Width > 0;
                    }
                    catch (Exception ex)
                    {
                        if (lasterrormessage < DateTime.Now.AddMinutes(-1))
                        {
                            MessageBox.Show($"Oops - unexpcted error. But never mind, just try again...\n\n{ex}");
                            lasterrormessage = DateTime.Now;
                        }
                    }
                }
            }
            reloaded = false;
        }

        private void GetLayoutFromGrid()
        {
            if (!form.settings.Results.WorkWithLayout ||
                !(form.dockControlBuilder.RootEntityMetadata is EntityMetadata entity) ||
                crmGridView1.DataSource == null)
            {
                return;
            }
            if (form.dockControlBuilder.LayoutXML == null || form.dockControlBuilder.LayoutXML.EntityMeta != entity)
            {
                form.dockControlBuilder.LayoutXML = new LayoutXML(entity, form);
            }
            var columns = crmGridView1.Columns.Cast<DataGridViewColumn>()
                .Where(c => !c.Name.StartsWith("#") && c.Visible && c.Width > 5)
                .OrderBy(c => c.DisplayIndex)
                .ToDictionary(c => c.Name, c => c.Width);
            form.dockControlBuilder.LayoutXML.MakeSureAllCellsExistForColumns(columns);
            form.dockControlBuilder.UpdateLayoutXML();
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

        private void crmGridView1_LayoutChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (!reloaded)
            {
                GetLayoutFromGrid();
            }
        }

        private void mnuResetLayout_Click(object sender, EventArgs e)
        {
            form.dockControlBuilder.ResetLayout();
            RefreshData();
        }

        private void txtPagingCookie_Click(object sender, EventArgs e)
        {
            txtPagingCookie.SelectAll();
        }

        private void mnuPagingCookie_Click(object sender, EventArgs e)
        {
            gbPagingCookie.Visible = mnuPagingCookie.Checked;
        }
    }
}