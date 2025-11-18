using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XTB.Helpers;
using Rappen.XTB.Helpers.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.AppCode
{
    public static class ExcelHelper
    {
        private const string LinkIcon = "🔗";
        private const int XlHAlignCenter = -4108;
        private const int XlVAlignTop = -4160;
        private const int XlCalculationManual = -4135;
        private const int XlCalculationAutomatic = -4105;

        private static readonly Dictionary<string, string> PrimaryNameCache = new(StringComparer.OrdinalIgnoreCase);

        // NOTE: Optional Excel/UI polish. Failures ignored per CONTRIBUTING.md §3 (non‑critical, documented).
        private static void Try(Action action)
        { try { action(); } catch { /* ignored: optional non-critical */ } }

        /// <summary>
        /// Exports current grid content plus FetchXML/Layout to Excel, applying formatting and link enrichment.
        /// </summary>
        public static void ExportToExcel(FetchXmlBuilder fxb, XRMDataGridView xrmgrid, string fetch, string layout, Action doneaction)
        {
            var dataObj = PrepareExcelClipboardOnUIThread(xrmgrid, xrmgrid, fxb.settings?.Results?.ExcelAddLinks ?? false, fxb.ConnectionDetail);
            if (dataObj == null)
            {
                return;
            }

            fxb.WorkAsync(new XrmToolBox.Extensibility.WorkAsyncInfo
            {
                Message = "Opening in Excel...",
                Work = (w, a) => ExportClipboardToExcel(w, fetch, layout, fxb.settings?.ExecuteOptions.AllPages ?? false, fxb.ConnectionDetail, dataObj),
                ProgressChanged = p => fxb.SetWorkingMessage(p.UserState.ToString()),
                PostWorkCallBack = a =>
                {
                    if (a.Error != null)
                    {
                        fxb.ShowErrorDialog(a.Error, "Open Excel");
                    }
                    doneaction?.Invoke();
                }
            });
        }

        private static DataObject PrepareExcelClipboardOnUIThread(Control invoker, XRMDataGridView xrmgrid, bool addlinks, ConnectionDetail conndet)
        {
            if (invoker == null || invoker.IsDisposed)
            {
                return null;
            }

            DataObject result = null;
            Exception invokeEx = null;

            if (!invoker.IsHandleCreated)
            {
                _ = invoker.Handle;
            }

            void action()
            {
                try
                {
                    result = BuildClipboardData(xrmgrid, addlinks, conndet);
                }
                catch (Exception ex)
                {
                    invokeEx = ex;
                }
            }

            // Clipboard and grid APIs must run on the UI thread.
            if (invoker.InvokeRequired)
            {
                invoker.Invoke((MethodInvoker)action);
            }
            else
            {
                action();
            }

            if (invokeEx != null)
            {
                throw invokeEx;
            }

            return result;
        }

        private static DataObject BuildClipboardData(XRMDataGridView xrmgrid, bool addlinks, ConnectionDetail conndet)
        {
            var originalMode = xrmgrid.ClipboardCopyMode;
            xrmgrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            xrmgrid.SelectAll();
            var dataObj = xrmgrid.GetClipboardContent() as DataObject;
            xrmgrid.ClearSelection();
            xrmgrid.ClipboardCopyMode = originalMode;

            if (dataObj == null)
            {
                return null;
            }

            if (!addlinks)
            {
                return dataObj;
            }

            var allVisible = xrmgrid.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            var baseNames = allVisible.ToDictionary(c => c, c => c.Name.Split('|')[0]);
            var noHash = allVisible.Where(c => !baseNames[c].StartsWith("#")).ToList();

            var hasPrimary = TryGetPrimaryNameAttribute(xrmgrid, out var primaryAttr);
            var primaryIndex = hasPrimary ? GetVisibleColumnIndexOfAttribute(noHash, primaryAttr) : -1;
            hasPrimary = hasPrimary && primaryIndex >= 0;

            var htmlFragment = BuildHtmlTable(xrmgrid, conndet, allVisible, hasPrimary ? noHash[primaryIndex] : null);
            var cfHtml = WrapAsClipboardHtml(htmlFragment);
            Try(() => dataObj.SetData(DataFormats.Html, cfHtml));
            return dataObj;
        }

        // Excel export (Result: columns AutoFit; Source: A AutoFit, B fixed width + Wrap).
        private static void ExportClipboardToExcel(System.ComponentModel.BackgroundWorker bw, string fetch, string layout, bool allpages, ConnectionDetail conndet, DataObject dataObj)
        {
            if (dataObj == null)
            {
                return;
            }

            bw?.ReportProgress(10, "Generate rows for Excel...");

            Exception excelThreadException = null;

            var sta = new Thread(() =>
            {
                // Excel automation and Clipboard require STA.
                dynamic app = null;
                bool? prevScreenUpdating = null, prevDisplayAlerts = null, prevEnableEvents = null;
                int? prevCalculation = null;

                try
                {
                    Try(() => Clipboard.SetDataObject(dataObj, true, retryTimes: 10, retryDelay: 50));
                    for (var i = 0; i < 10; i++)
                    {
                        try
                        {
                            if (Clipboard.ContainsData(DataFormats.Html) || Clipboard.ContainsData(DataFormats.Text))
                            {
                                break;
                            }
                        }
                        catch { }
                        Thread.Sleep(20);
                    }

                    bw?.ReportProgress(20, "Starting Excel...");
                    var excelType = Type.GetTypeFromProgID("Excel.Application") ?? throw new Exception("Microsoft Excel is not installed.");
                    try
                    {
                        app = Activator.CreateInstance(excelType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to start Microsoft Excel.{Environment.NewLine}{ex.Message}", ex);
                    }

                    // Keep Excel hidden while preparing content; show only when ready.
                    Try(() => app.Visible = false);

                    bw?.ReportProgress(30, "Populating results...");
                    try
                    {
                        Try(() => { prevScreenUpdating = app.ScreenUpdating; app.ScreenUpdating = false; });
                        Try(() => { prevDisplayAlerts = app.DisplayAlerts; app.DisplayAlerts = false; });
                        Try(() => { prevEnableEvents = app.EnableEvents; app.EnableEvents = false; });
                        Try(() => { prevCalculation = app.Calculation; app.Calculation = XlCalculationManual; });

                        dynamic wb = app.Workbooks.Add();
                        dynamic resultSheet = wb.Worksheets[1];
                        resultSheet.Name = "FetchXML Builder - Result";

                        const int xlEdgeBottom = 9;
                        const int xlContinuous = 1;
                        const int xlThick = 4;
                        const int xlAnd = 1;

                        try
                        {
                            resultSheet.Paste(resultSheet.Cells[1, 1]);
                        }
                        catch
                        {
                            Try(() => resultSheet.Paste());
                        }
                        Try(() => app.CutCopyMode = false);

                        Try(() =>
                        {
                            var headerA1 = (string)(resultSheet.Cells[1, 1].Value ?? string.Empty);
                            if (string.Equals(headerA1, LinkIcon, StringComparison.Ordinal))
                            {
                                resultSheet.Columns[1].HorizontalAlignment = XlHAlignCenter;
                            }
                        });

                        dynamic header = resultSheet.Rows[1];
                        header.Font.Bold = true;
                        header.Borders[xlEdgeBottom].LineStyle = xlContinuous;
                        header.Borders[xlEdgeBottom].Weight = xlThick;
                        header.AutoFilter(1, Type.Missing, xlAnd, Type.Missing, true);
                        Try(() => resultSheet.Application.ActiveWindow.SplitRow = 1);
                        Try(() => resultSheet.Application.ActiveWindow.FreezePanes = true);
                        Try(() => resultSheet.Rows.VerticalAlignment = XlVAlignTop);

                        bw?.ReportProgress(35, "Autosizing columns...");
                        Try(() =>
                        {
                            // HTML paste enables WrapText; with wrap on, AutoFit grows rows, not columns.
                            // Turn off wrap to allow column width AutoFit to compute correctly.
                            dynamic used = resultSheet.UsedRange;
                            used.WrapText = false;
                            used.Columns.AutoFit();
                        });

                        bw?.ReportProgress(40, "Copying FetchXML and LayoutXML...");
                        PopulateSourceSheet(wb, fetch, layout, allpages, conndet);

                        bw?.ReportProgress(90, "Finalizing Excel...");
                        Try(() => resultSheet.Activate());
                        Try(() => resultSheet.Range["A1", "A1"].Select());
                    }
                    finally
                    {
                        Try(() => app.Calculation = prevCalculation ?? XlCalculationAutomatic);
                        Try(() => app.EnableEvents = prevEnableEvents ?? true);
                        Try(() => app.DisplayAlerts = prevDisplayAlerts ?? true);
                        Try(() => app.ScreenUpdating = prevScreenUpdating ?? true);
                    }

                    bw?.ReportProgress(95, "Opening Excel...");
                    Try(() => app.Visible = true);
                    Try(() => app.Workbooks[1].Activate());
                    Try(() => app.Windows[1].Activate());
                }
                catch (Exception ex)
                {
                    excelThreadException = ex;
                }
            });

            sta.SetApartmentState(ApartmentState.STA);
            sta.IsBackground = true;
            sta.Start();
            sta.Join();

            if (excelThreadException != null)
            {
                throw excelThreadException;
            }
        }

        private static void PopulateSourceSheet(dynamic wb, string fetch, string layout, bool allpages, ConnectionDetail conndet)
        {
            dynamic sourceSheet = wb.Sheets.Add();
            Try(() => sourceSheet.Move(After: wb.Sheets[wb.Sheets.Count]));
            sourceSheet.Name = "FetchXML Builder - Source";

            var fetchLocal = fetch;
            if (allpages && !string.IsNullOrEmpty(fetchLocal))
            {
                var fetchtype = XRM.Helpers.FetchXML.Fetch.FromString(fetchLocal);
                if (fetchtype.PagingCookie != null || fetchtype.PageNumber != null)
                {
                    fetchtype.PagingCookie = null;
                    fetchtype.PageNumber = null;
                    fetchLocal = fetchtype.ToString();
                }
            }

            sourceSheet.Cells[1, 1].Value = "Connection";
            sourceSheet.Cells[1, 2].Value = conndet?.ConnectionName;
            sourceSheet.Cells[2, 1].Value = "URL";
            sourceSheet.Cells[2, 2].Value = conndet?.WebApplicationUrl;
            sourceSheet.Cells[3, 1].Value = "Query";
            sourceSheet.Cells[3, 2].Value = fetchLocal;
            if (!string.IsNullOrEmpty(layout))
            {
                sourceSheet.Cells[4, 1].Value = "Layout";
                sourceSheet.Cells[4, 2].Value = layout;
            }

            dynamic sourceHeaderCol = sourceSheet.Columns[1];
            sourceHeaderCol.Font.Bold = true;
            sourceHeaderCol.Cells.VerticalAlignment = XlVAlignTop;

            Try(() => sourceSheet.Columns[1].AutoFit());
            Try(() => sourceSheet.Columns[2].WrapText = true);
            Try(() =>
            {
                // Excel column width approximation for pixels: (px - 5) / 7.0 (empirical; consistent enough for 800 px target).
                const int targetPixels = 800;
                var excelWidth = (targetPixels - 5) / 7.0;
                sourceSheet.Columns[2].ColumnWidth = excelWidth;
            });
            Try(() => sourceSheet.Rows.VerticalAlignment = XlVAlignTop);
            Try(() => sourceSheet.Rows.AutoFit());
        }

        private static int GetVisibleColumnIndexOfAttribute(List<DataGridViewColumn> visibleCols, string attributeLogicalName)
        {
            return visibleCols.FindIndex(c =>
            {
                var baseName = c.Name.Split('|')[0];
                return !baseName.Contains(".") &&
                       string.Equals(baseName, attributeLogicalName, StringComparison.OrdinalIgnoreCase);
            });
        }

        /// <summary>
        /// Builds an HTML table from the grid. If a primary name column is visible, that cell is linked; otherwise a separate link icon column is added.
        /// EntityReference values get links to their records when possible.
        /// </summary>
        private static string BuildHtmlTable(XRMDataGridView grid, ConnectionDetail conndet, List<DataGridViewColumn> orderedAllVisible, DataGridViewColumn primaryColOrNull)
        {
            var hasPrimary = primaryColOrNull != null;
            var primaryAttr = hasPrimary ? primaryColOrNull.Name.Split('|')[0] : null;

            var sb = new StringBuilder();
            sb.Append("<table border=\"0\" cellpadding=\"2\" cellspacing=\"0\"><tr>");
            if (!hasPrimary)
            {
                sb.Append($"<th>{HtmlEncode(LinkIcon)}</th>");
            }

            foreach (var col in orderedAllVisible)
            {
                sb.Append($"<th>{HtmlEncode(col.HeaderText)}</th>");
            }

            sb.Append("</tr>");

            for (var r = 0; r < grid.Rows.Count; r++)
            {
                var row = grid.Rows[r];
                if (!row.Visible)
                {
                    continue;
                }

                var entity = grid.GetXRMEntity(r);
                var urlPrimary = entity?.GetEntityUrl(conndet);

                sb.Append("<tr>");
                if (!hasPrimary)
                {
                    if (!string.IsNullOrWhiteSpace(urlPrimary))
                    {
                        sb.Append($"<td><a href=\"{HtmlAttr(urlPrimary)}\">{HtmlEncode(LinkIcon)}</a></td>");
                    }
                    else
                    {
                        sb.Append("<td></td>");
                    }
                }

                foreach (var col in orderedAllVisible)
                {
                    var raw = grid[col.Index, r]?.Value?.ToString();
                    var display = raw?.Replace("\r\n", "\n").Replace("\n", "<br/>");
                    var encoded = HtmlEncode(display);
                    var baseName = col.Name.Split('|')[0];
                    var isRootAttr = !baseName.StartsWith("#") && !baseName.Contains(".");
                    string linkUrl = null;

                    if (hasPrimary && isRootAttr && string.Equals(baseName, primaryAttr, StringComparison.OrdinalIgnoreCase))
                    {
                        linkUrl = urlPrimary;
                    }
                    else if (isRootAttr && entity != null && entity.Contains(baseName))
                    {
                        var val = entity[baseName];
                        if (val is AliasedValue av)
                        {
                            val = av.Value;
                        }

                        if (val is EntityReference er)
                        {
                            linkUrl = er.GetEntityUrl(conndet);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(linkUrl) && !string.IsNullOrEmpty(display))
                    {
                        sb.Append($"<td><a href=\"{HtmlAttr(linkUrl)}\">{encoded}</a></td>");
                    }
                    else
                    {
                        sb.Append($"<td>{encoded}</td>");
                    }
                }
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// Wraps the HTML fragment into CF_HTML clipboard format.
        /// See: https://learn.microsoft.com/windows/win32/dataxchg/html-clipboard-format
        /// Offsets are byte positions (header is ASCII, body is UTF-8).
        /// </summary>
        private static string WrapAsClipboardHtml(string fragment)
        {
            const string headerTemplate =
                "Version:1.0\r\nStartHTML:{0:0000000000}\r\nEndHTML:{1:0000000000}\r\nStartFragment:{2:0000000000}\r\nEndFragment:{3:0000000000}\r\n";

            var pre = "<html><body><!--StartFragment-->";
            var post = "<!--EndFragment--></body></html>";
            var html = pre + fragment + post;

            var utf8 = Encoding.UTF8;
            var preLenBytes = utf8.GetByteCount(pre);
            var fragLenBytes = utf8.GetByteCount(fragment);
            var htmlLenBytes = utf8.GetByteCount(html);

            var headerWithZeroes = string.Format(headerTemplate, 0, 0, 0, 0);
            var headerLenBytes = Encoding.ASCII.GetByteCount(headerWithZeroes);

            var startHTML = headerLenBytes;
            var endHTML = headerLenBytes + htmlLenBytes;
            var startFragment = headerLenBytes + preLenBytes;
            var endFragment = startFragment + fragLenBytes;

            var header = string.Format(headerTemplate, startHTML, endHTML, startFragment, endFragment);
            return header + html;
        }

        private static string HtmlEncode(string s) =>
            string.IsNullOrEmpty(s) ? string.Empty :
            s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");

        private static string HtmlAttr(string s) =>
            string.IsNullOrEmpty(s) ? string.Empty :
            s.Replace("&", "&amp;").Replace("\"", "&quot;");

        private static bool TryGetPrimaryNameAttribute(XRMDataGridView grid, out string primaryName)
        {
            primaryName = null;
            if (grid?.Service == null || string.IsNullOrWhiteSpace(grid.EntityName))
            {
                return false;
            }

            if (!PrimaryNameCache.TryGetValue(grid.EntityName, out primaryName))
            {
                var meta = grid.Service.GetEntity(grid.EntityName);
                primaryName = meta?.PrimaryNameAttribute;
                if (string.IsNullOrWhiteSpace(primaryName))
                {
                    return false;
                }

                PrimaryNameCache[grid.EntityName] = primaryName;
            }
            return true;
        }
    }
}