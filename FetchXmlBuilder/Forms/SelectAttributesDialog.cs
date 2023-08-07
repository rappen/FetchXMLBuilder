using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class SelectAttributesDialog : Form
    {
        private int sortcolumn = 0;
        private List<ListViewItem> allItems;

        public SelectAttributesDialog(List<AttributeMetadata> attributes, List<string> selectedAttributes)
        {
            InitializeComponent();
            GenerateAllItems(attributes, selectedAttributes);
            PopulateAttributes();
            SetSelectedNos();
            this.ActiveControl = txtFilter;
        }

        private void GenerateAllItems(List<AttributeMetadata> attributes, List<string> selectedAttributes)
        {
            chkMetamore_CheckedChanged(null, null);
            allItems = new List<ListViewItem>();
            foreach (var attribute in attributes)
            {
                var name = attribute.DisplayName?.UserLocalizedLabel?.Label ?? attribute.LogicalName;
                var item = new ListViewItem(new string[] {
                    name,
                    attribute.LogicalName,
                    attribute.ToTypeString(),
                    attribute.IsValidForRead.HasValue ? attribute.IsValidForRead.Value.ToString() : "",
                    attribute.IsValidForGrid.HasValue ? attribute.IsValidForGrid.Value.ToString() : "",
                    attribute.IsValidForAdvancedFind.Value.ToString(),
                    attribute.IsRetrievable.HasValue ? attribute.IsRetrievable.Value.ToString() : ""
                });
                item.Name = attribute.LogicalName;
                item.Text = name;
                item.Tag = attribute;
                item.Checked = selectedAttributes.Contains(attribute.LogicalName);
                allItems.Add(item);
            }
        }

        private void PopulateAttributes(bool required = false)
        {
            lvAttributes.Items.Clear();
            foreach (var item in allItems)
            {
                var filter = txtFilter.Text.ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(filter) || item.Name.ToUpperInvariant().Contains(filter) || item.Text.ToUpperInvariant().Contains(filter))
                {
                    if (required && !IsRequired(item.Tag as AttributeMetadata))
                    {
                        continue;
                    }
                    lvAttributes.Items.Add(item);
                }
            }
        }

        private bool IsRequired(AttributeMetadata meta)
        {
            return (meta.RequiredLevel?.Value == AttributeRequiredLevel.ApplicationRequired ||
                    meta.RequiredLevel?.Value == AttributeRequiredLevel.SystemRequired) &&
                   string.IsNullOrEmpty(meta.AttributeOf);
        }

        public List<AttributeMetadata> GetSelectedAttributes()
        {
            var result = new List<AttributeMetadata>();
            foreach (var item in allItems)
            {
                if (item.Checked)
                {
                    result.Add((AttributeMetadata)item.Tag);
                }
            }
            return result;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvAttributes.Items)
            {
                item.Checked = checkBox1.Checked;
            }
        }

        private void lvAttributes_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortcolumn)
            {
                lvAttributes.Sorting = lvAttributes.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                lvAttributes.Sorting = SortOrder.Ascending;
            }
            lvAttributes.ListViewItemSorter = new ListViewItemComparer(e.Column, lvAttributes.Sorting);
            sortcolumn = e.Column;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            PopulateAttributes();
        }

        private void chkMetamore_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var col in from ColumnHeader col in lvAttributes.Columns
                                where col.Tag?.ToString() == "meta"
                                select col)
            {
                ShowHideMeta(chkMetamore.Checked, col);
            }
            splitContainer1.Panel2Collapsed = !chkMetamore.Checked;
            var width = (from ColumnHeader col in lvAttributes.Columns select col.Width).Sum() + 70;
            Width = Math.Max(width, 400) + splitContainer1.Panel2.Width;
        }

        private void ShowHideMeta(bool on, ColumnHeader col)
        {
            if (on)
            {
                col.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            else
            {
                col.Width = 0;
            }
        }

        private void lvAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAttributes.SelectedItems.Cast<ListViewItem>().FirstOrDefault() is ListViewItem item &&
                item.Tag is AttributeMetadata meta)
            {
                metadataControl1.SelectedObject = meta;
            }
        }

        private void lvAttributes_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            SetSelectedNos();
        }

        private void SetSelectedNos()
        {
            try
            {
                lblSelectedNo.Text = $"Selected {allItems.Where(a => a.Checked).Count()}/{allItems.Count}";
            }
            catch { }
        }

        private void lnkShowRequired_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateAttributes(true);
        }

        private void lnkShowAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateAttributes();
        }
    }
}