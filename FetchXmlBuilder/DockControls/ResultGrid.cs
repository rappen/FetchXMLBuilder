﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Views;
using Rappen.XTB.Helpers;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    public partial class ResultGrid : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder form;
        private QueryInfo queryinfo;
        private bool reloaded;
        private DateTime lasterrormessage;

        #region Public Constructor

        public ResultGrid(FetchXmlBuilder fetchXmlBuilder)
        {
            InitializeComponent();
            this.PrepareGroupBoxExpanders();
            form = fetchXmlBuilder;
            ApplySettingsToGrid();
        }

        #endregion Public Constructor

        #region Internal Methods

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
            mnuExcel.Visible = queryinfo.Results?.Entities?.Count > 0;

            mnuAllPages.Enabled = queryinfo.Results?.Entities?.Count > 0;
            mnuRetrieveTime.Text = queryinfo.Elapsed.ToSmartString();
            mnuRetrieveTime.Visible = form.settings.Results.ShowRetrieveTime;

            if (queryinfo.Query is FetchExpression && !form.settings.ExecuteOptions.AllPages && (queryinfo.Results.MoreRecords || queryinfo.PageNo > 1))
            {
                mnuPage.Text = (queryinfo.PageNo < 10 ? "Page " : "") + queryinfo.PageNo.ToString() + (queryinfo.Pages > 0 ? $"/{queryinfo.Pages}" : "");
                mnuPageMinus.Enabled = queryinfo.PageNo > 1;
                mnuPagePlus.Enabled = queryinfo.Results.MoreRecords;
                mnuPage.Visible = true;
                mnuPageMinus.Visible = true;
                mnuPagePlus.Visible = true;
            }
            else
            {
                mnuPage.Visible = false;
                mnuPageMinus.Visible = false;
                mnuPagePlus.Visible = false;
            }
            if (queryinfo.RecordFrom > -1)
            {
                if (!form.settings.ExecuteOptions.AllPages && queryinfo.RecordTo > queryinfo.RecordFrom && (queryinfo.RecordFrom > 1 || queryinfo.Results.MoreRecords))
                {
                    mnuRecordsNumbers.Text = $"Records: {queryinfo.RecordFrom}-{queryinfo.RecordTo}";
                    if (queryinfo.Results.TotalRecordCount > 0 && queryinfo.Results.TotalRecordCount < 5000)
                    {
                        mnuRecordsNumbers.Text += $" ({queryinfo.Results.TotalRecordCount})";
                    }
                }
                else
                {
                    mnuRecordsNumbers.Text = $"Records: {queryinfo.Results.Entities.Count}";
                }
                mnuRecordsNumbers.Visible = true;
            }
            else
            {
                mnuRecordsNumbers.Visible = false;
            }
        }

        internal void ApplySettingsToGrid()
        {
            mnuFriendly.Checked = form.settings.Results.Friendly;
            mnuBothNames.Checked = form.settings.Results.BothNames;
            mnuIdCol.Checked = form.settings.Results.WorkWithLayout ? false : form.settings.Results.Id;
            mnuIndexCol.Checked = form.settings.Results.WorkWithLayout ? false : form.settings.Results.Index;
            mnuNullCol.Checked = form.settings.Results.WorkWithLayout ? true : form.settings.Results.NullColumns;
            mnuSysCol.Checked = form.settings.Results.WorkWithLayout ? false : form.settings.Results.SysColumns;
            mnuLocalTime.Checked = form.settings.Results.LocalTime;
            mnuCopyHeaders.Checked = form.settings.Results.CopyHeaders;
            mnuExcelAdvanced.Checked = form.settings.Results.ExcelAdvanced;
            mnuQuickFilter.Checked = form.settings.Results.QuickFilter;
            mnuPagingCookie.Checked = form.settings.Results.PagingCookie;
            mnuShowElapsed.Checked = form.settings.Results.ShowRetrieveTime;
            mnuAllPages.Checked = form.settings.ExecuteOptions.AllPages;

            mnuIdCol.Visible = !form.settings.Results.WorkWithLayout;
            mnuIndexCol.Visible = !form.settings.Results.WorkWithLayout;
            mnuNullCol.Visible = !form.settings.Results.WorkWithLayout;
            mnuSysCol.Visible = !form.settings.Results.WorkWithLayout;
            mnuResetLayout.Visible = form.settings.Results.WorkWithLayout && !string.IsNullOrWhiteSpace(crmGridView1.LayoutXML);
            mnuShowAllCol.Visible = form.settings.Results.WorkWithLayout;
            mnuShowLayoutXML.Visible = form.settings.Results.WorkWithLayout;

            if (!form.settings.Results.WorkWithLayout && crmGridView1.LayoutXML != null)
            {
                crmGridView1.LayoutXML = null;
            }
            crmGridView1.ShowFriendlyNames = mnuFriendly.Checked;
            crmGridView1.ShowBothNames = mnuBothNames.Checked;
            crmGridView1.ShowIdColumn = mnuIdCol.Checked;
            crmGridView1.ShowIndexColumn = mnuIndexCol.Checked;
            crmGridView1.ShowAllColumnsInColumnOrder = mnuNullCol.Checked;
            crmGridView1.ShowColumnsNotInColumnOrder = mnuSysCol.Checked;
            crmGridView1.ShowLocalTimes = mnuLocalTime.Checked;
            crmGridView1.ClipboardCopyMode = mnuCopyHeaders.Checked ?
                DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText :
                DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            crmGridView1.Service = form.Service;
            panQuickFilter.Visible = mnuQuickFilter.Checked;
            gbPagingCookie.Visible = mnuPagingCookie.Checked;
            mnuRetrieveTime.Visible = mnuShowElapsed.Checked;
            RefreshData();
        }

        internal void SetQueryIfChangesDesign()
        {
            var changed = queryinfo?.QuerySignature != form.dockControlBuilder?.GetTreeChecksum(null);
            SetQueryChanged(changed);
        }

        internal void SetQueryChanged(bool changed)
        {
            if (IsDisposed)
            {
                return;
            }
            Text = "Result View" + (changed ? " *" : "");
            crmGridView1.DefaultCellStyle.BackColor = changed ? System.Drawing.Color.LightGray : System.Drawing.Color.White;
            crmGridView1.DefaultCellStyle.ForeColor = changed ? System.Drawing.Color.Gray : System.Drawing.SystemColors.ControlText;
            mnuExcel.Enabled = !changed;
            mnuRetrieveTime.Enabled = !changed;
            mnuPage.Enabled = !changed;
            mnuPageMinus.Enabled = !changed;
            mnuPagePlus.Enabled = !changed;
        }

        internal void SetLayoutToGrid()
        {
            if (!form.settings.Results.WorkWithLayout || form.dockControlBuilder?.LayoutXML?.Cells == null)
            {
                return;
            }
            var tmpreloaded = reloaded;
            reloaded = true;
            crmGridView1.LayoutXML = form.dockControlBuilder?.LayoutXML?.ToXMLString();
            crmGridView1.Refresh();
            reloaded = tmpreloaded;
        }

        internal void ShowAllColumns()
        {
            ShowHiddenColumns();
            GetLayoutFromGrid();
        }

        internal void ResetLayout()
        {
            form.dockControlBuilder.ResetLayout();
            crmGridView1.LayoutXML = form.dockControlBuilder.LayoutXML?.ToXMLString();
            RefreshData();
        }

        #endregion Internal Methods

        #region Private Methods

        private void SetSelectedDetails()
        {
            MethodInvoker mi = delegate
            {
                try
                {
                    panSelectedDetails.Visible = false;
                    var details = crmGridView1.GetSelectedDetails();
                    var visibleit = !string.IsNullOrWhiteSpace(string.Join("", details.Select(d => d.Value)));
                    if (visibleit)
                    {
                        txtSelectedDetails.Text = string.Join(Environment.NewLine, details.Select(d => $"{d.Key}:\t{d.Value}"));

                        using (Graphics g = this.CreateGraphics())
                        {
                            var textSize = g.MeasureString(txtSelectedDetails.Text, txtSelectedDetails.Font).ToSize();
                            panSelectedDetails.Width = textSize.Width + 6;
                            panSelectedDetails.Height = textSize.Height + 10;
                            panSelectedDetails.Top = crmGridView1.Top;
                            panSelectedDetails.Left = crmGridView1.Left + crmGridView1.Width - panSelectedDetails.Width;
                            if (crmGridView1.Controls.OfType<VScrollBar>().First().Visible)
                            {
                                panSelectedDetails.Left -= SystemInformation.VerticalScrollBarWidth;
                            }
                            if (panSelectedDetails.Left - 10 < crmGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible))
                            {
                                panSelectedDetails.Top += crmGridView1.ColumnHeadersHeight;
                            }
                        }
                        panSelectedDetails.Visible = true;
                    }
                }
                catch
                {
                    // Now what?
                }
            };
            if (InvokeRequired)
            {
                Invoke(mi);
            }
            else
            {
                mi();
            }
        }

        private void UpdateSettingsFromSelectedOptions()
        {
            var reload = form.settings.ExecuteOptions.AllPages != mnuAllPages.Checked;
            form.settings.Results.Friendly = mnuFriendly.Checked;
            form.settings.Results.BothNames = mnuBothNames.Checked;
            form.settings.Results.Index = mnuIndexCol.Checked;
            form.settings.Results.NullColumns = mnuNullCol.Checked;
            form.settings.Results.SysColumns = mnuSysCol.Checked;
            form.settings.Results.Id = mnuIdCol.Checked;
            form.settings.Results.LocalTime = mnuLocalTime.Checked;
            form.settings.Results.CopyHeaders = mnuCopyHeaders.Checked;
            form.settings.Results.ExcelAdvanced = mnuExcelAdvanced.Checked;
            form.settings.Results.QuickFilter = mnuQuickFilter.Checked;
            form.settings.Results.PagingCookie = mnuPagingCookie.Checked;
            form.settings.Results.ShowRetrieveTime = mnuShowElapsed.Checked;
            form.settings.ExecuteOptions.AllPages = mnuAllPages.Checked;
            ApplySettingsToGrid();
            if (reload)
            {
                form.EnableControls();
                form.FetchResults();
            }
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
            if (form.dockControlBuilder.LayoutXML == null)
            {
                crmGridView1.Refresh();
                ShowHiddenColumns();
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

        private void ShowHiddenColumns()
        {
            crmGridView1.Columns.Cast<DataGridViewColumn>()
                .Where(c => !c.Name.StartsWith("#") && !c.Visible)
                .ToList()
                .ForEach(c => c.Visible = true);
            crmGridView1.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible && c.Width < 10)
                .ToList()
                .ForEach(c => c.Width = 100);
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
                .Where(c => !c.Name.StartsWith("#") && !c.Name.EndsWith("|both") && c.Visible && c.Width > 5)
                .OrderBy(c => c.DisplayIndex)
                .ToDictionary(c => c.Name, c => c.Width);
            form.dockControlBuilder.LayoutXML?.MakeSureAllCellsExistForColumns(columns);
            form.dockControlBuilder.UpdateLayoutXML();
        }

        private void OpenInExcel()
        {
            Cursor = Cursors.WaitCursor;
            mnuExcel.Enabled = false;
            var fetch = queryinfo.Query is FetchExpression fetchexpr ? fetchexpr.Query : queryinfo.Query.ToString();
            var layout = form.dockControlBuilder?.LayoutXML?.ToXMLString();
            try
            {
                ExcelHelper.ExportToExcel(crmGridView1, fetch, layout, form.settings, form.ConnectionDetail);
            }
            catch (Exception ex)
            {
                form.ShowErrorDialog(ex, "Open Excel");
            }
            finally
            {
                Cursor = Cursors.Default;
                mnuExcel.Enabled = true;
            }
        }

        #endregion Private Methods

        #region Private Event Handlers

        private void crmGridView1_RecordDoubleClick(object sender, Rappen.XTB.Helpers.Controls.XRMRecordEventArgs e)
        {
            if (!form.settings.Results.ClickableLinks)
            {
                return;
            }
            if (e.Value is EntityReference entref)
            {
                form.LogUse("OpenParentRecord");
                form.OpenUrl(entref);
            }
            else if (e.Entity != null)
            {
                form.LogUse("OpenRecord");
                form.OpenUrl(e.Entity);
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
            ctxRecordOpen.Tag = UrlUtils.GetEntityUrl(e.Entity, form.ConnectionDetail);
            ctxRecordCopy.Tag = ctxRecordOpen.Tag;
            ctxColumnOpen.Tag = UrlUtils.GetEntityUrl(e.Value as EntityReference, form.ConnectionDetail);
            ctxColumnCopy.Tag = ctxColumnOpen.Tag;
        }

        private void ctxOpen_Click(object sender, EventArgs e)
        {
            if (form.OpenUrl(sender))
            {
                form.LogUse("OpenRecord");
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
            if (!reloaded && !crmGridView1.SettingsWidths)
            {
                GetLayoutFromGrid();
            }
            SetSelectedDetails();
        }

        private void mnuResetLayout_Click(object sender, EventArgs e)
        {
            ResetLayout();
        }

        private void txtPagingCookie_Click(object sender, EventArgs e)
        {
            txtPagingCookie.SelectAll();
        }

        private void mnuPagingCookie_Click(object sender, EventArgs e)
        {
            gbPagingCookie.Visible = mnuPagingCookie.Checked;
        }

        private void RetrievePageNo(int page)
        {
            if (queryinfo.Query is FetchExpression fetchexpr)
            {
                var fetchdoc = fetchexpr.Query.ToXml();
                var fetchnode = fetchdoc.SelectSingleNode("fetch") as XmlElement;
                fetchnode.SetAttribute("page", page.ToString());
                form.FetchResults(fetchdoc.OuterXml);
            }
        }

        private void mnuPagePlusMinus_Click(object sender, EventArgs e)
        {
            var direction = sender == mnuPageMinus ? -1 : sender == mnuPagePlus ? 1 : 0;
            if (direction != 0)
            {
                mnuPageMinus.Enabled = false;
                mnuPagePlus.Enabled = false;
                var page = queryinfo.PageNo + direction;
                RetrievePageNo(page);
            }
        }

        private void mnuPage_Enter(object sender, EventArgs e)
        {
            mnuPage.Text = "";
        }

        private void mnuPage_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !int.TryParse(mnuPage.Text, out var _);
        }

        private void mnuPage_Validated(object sender, EventArgs e)
        {
            if (int.TryParse(mnuPage.Text, out int page) && page != queryinfo.PageNo)
            {
                RetrievePageNo(page);
            }
        }

        private void mnuPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Tab)
            {
                e.KeyChar = (char)Keys.None;
                e.Handled = true;
                crmGridView1.Focus();
            }
        }

        private void mnuAutoSizeAll_Click(object sender, EventArgs e)
        {
            crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            GetLayoutFromGrid();
        }

        private void mnuShowAllCol_Click(object sender, EventArgs e)
        {
            ShowAllColumns();
        }

        private void mnuShowLayoutXML_Click(object sender, EventArgs e)
        {
            form.ShowLayoutXML();
        }

        private void crmGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!reloaded)
            {
                GetLayoutFromGrid();
            }
        }

        private void mnuExcel_Click(object sender, EventArgs e)
        {
            OpenInExcel();
        }

        private void crmGridView1_SelectionChanged(object sender, EventArgs e)
        {
            SetSelectedDetails();
        }

        private void crmGridView1_Resize(object sender, EventArgs e)
        {
            SetSelectedDetails();
        }

        #endregion Private Event Handlers
    }
}