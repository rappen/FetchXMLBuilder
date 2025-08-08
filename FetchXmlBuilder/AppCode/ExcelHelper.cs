using Rappen.XTB.Helpers.Controls;
using System;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using McTools.Xrm.Connection;
using Rappen.XTB.FetchXmlBuilder.Settings;

namespace Rappen.XTB.FetchXmlBuilder.AppCode
{
    public static class ExcelHelper
    {
        public static void ExportToExcel(XRMDataGridView xrmgrid, string fetch, string layout, FXBSettings settings, ConnectionDetail conndet)
        {
            var tempCopyMode = xrmgrid.ClipboardCopyMode;
            xrmgrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            xrmgrid.SelectAll();
            var dataObj = xrmgrid.GetClipboardContent();
            xrmgrid.ClearSelection();
            xrmgrid.ClipboardCopyMode = tempCopyMode;
            if (dataObj == null)
            {
                return;
            }
            Clipboard.SetDataObject(dataObj);
            var xlexcel = new Microsoft.Office.Interop.Excel.Application();
            xlexcel.Visible = true;
            var xlWorkBook = xlexcel.Workbooks.Add(System.Reflection.Missing.Value);

            // Create sheet for results
            var xlResultSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlResultSheet.Name = "FetchXML Builder - Result";

            if (settings.Results.ExcelAdvanced)
            {   // This is slower, but handles new-lines correctly
                var xlcolhdr = 0;
                for (int col = 0; col < xrmgrid.Columns.Count; col++)
                {
                    if (!xrmgrid.Columns[col].Visible)
                    {
                        continue;
                    }
                    xlcolhdr++;
                    xlResultSheet.Cells[1, xlcolhdr] = xrmgrid.Columns[col].HeaderText;
                }
                var xlrow = 1;
                for (int row = 0; row < xrmgrid.Rows.Count; row++)
                {
                    if (!xrmgrid.Rows[row].Visible)
                    {
                        continue;
                    }
                    xlrow++;
                    var xlcol = 0;
                    for (int col = 0; col < xrmgrid.Columns.Count; col++)
                    {
                        if (!xrmgrid.Columns[col].Visible)
                        {
                            continue;
                        }
                        xlcol++;
                        if (xrmgrid[col, row].Value != null)
                        {
                            xlResultSheet.Cells[xlrow, xlcol] = xrmgrid[col, row].Value.ToString().Replace("\n", "\r\n");
                        }
                    }
                }
            }
            else
            {   // Paste all data, faster
                var cellResultA1 = (Range)xlResultSheet.Cells[1, 1];
                cellResultA1.Select();
                xlResultSheet.PasteSpecial(cellResultA1, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            }

            // Format width and headers
            var header = (Range)xlResultSheet.Rows[1];
            header.Font.Bold = true;
            header.Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            header.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThick;
            header.AutoFilter(1, Type.Missing, XlAutoFilterOperator.xlAnd, Type.Missing, true);
            xlResultSheet.Activate();
            xlResultSheet.Application.ActiveWindow.SplitRow = 1;
            try { xlResultSheet.Application.ActiveWindow.FreezePanes = true; } catch { }
            xlResultSheet.Columns.AutoFit();
            xlResultSheet.Rows.VerticalAlignment = XlVAlign.xlVAlignTop;

            // Create sheet for source
            var xlSourceSheet = (Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[xlWorkBook.Sheets.Count]);
            xlSourceSheet.Name = "FetchXML Builder - Source";
            if (settings.ExecuteOptions.AllPages)
            {
                var fetchtype = XRM.Helpers.FetchXML.Fetch.FromString(fetch);
                if (fetchtype.PagingCookie != null || fetchtype.PageNumber != null)
                {
                    fetchtype.PagingCookie = null;
                    fetchtype.PageNumber = null;
                    fetch = fetchtype.ToString();
                }
            }
            ((Range)xlSourceSheet.Cells[1, 1]).Value = "Connection";
            ((Range)xlSourceSheet.Cells[1, 2]).Value = conndet?.ConnectionName;
            ((Range)xlSourceSheet.Cells[2, 1]).Value = "URL";
            ((Range)xlSourceSheet.Cells[2, 2]).Value = conndet?.WebApplicationUrl;
            ((Range)xlSourceSheet.Cells[3, 1]).Value = "Query";
            ((Range)xlSourceSheet.Cells[3, 2]).Value = fetch;
            if (settings.Layout.Enabled)
            {
                ((Range)xlSourceSheet.Cells[4, 1]).Value = "Layout";
                ((Range)xlSourceSheet.Cells[4, 2]).Value = layout;
            }
            var sourceheader = (Range)xlSourceSheet.Columns[1];
            sourceheader.Font.Bold = true;
            sourceheader.Cells.VerticalAlignment = XlVAlign.xlVAlignTop;
            ((Range)xlSourceSheet.Cells[1, 1]).EntireColumn.AutoFit();
            ((Range)xlSourceSheet.Cells[1, 2]).EntireColumn.ColumnWidth = 150;
            xlSourceSheet.Rows.AutoFit();

            // Select result sheet
            xlResultSheet.Activate();
            xlResultSheet.Range["A1", "A1"].Select();
        }
    }
}