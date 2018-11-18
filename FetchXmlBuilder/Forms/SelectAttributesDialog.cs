using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
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
            this.ActiveControl = txtFilter;
        }

        private void GenerateAllItems(List<AttributeMetadata> attributes, List<string> selectedAttributes)
        {
            allItems = new List<ListViewItem>();
            foreach (var attribute in attributes)
            {
                var name = attribute.DisplayName.UserLocalizedLabel != null ? attribute.DisplayName.UserLocalizedLabel.Label : attribute.LogicalName;
                var item = new ListViewItem(new string[] { name, attribute.LogicalName, attribute.AttributeType.ToString() });
                item.Name = attribute.LogicalName;
                item.Text = name;
                item.Tag = attribute;
                item.Checked = selectedAttributes.Contains(attribute.LogicalName);
                allItems.Add(item);
            }
        }

        private void PopulateAttributes()
        {
            lvAttributes.Items.Clear();
            foreach (var item in allItems)
            {
                var filter = txtFilter.Text.ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(filter) || item.Name.ToUpperInvariant().Contains(filter) || item.Text.ToUpperInvariant().Contains(filter))
                {
                    lvAttributes.Items.Add(item);
                }
            }
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
    }
}