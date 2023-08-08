using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class SelectAttributesDialog : Form
    {
        private int sortcolumn = 0;
        private List<ListViewItem> allItems;
        private List<string> viewcolumns;
        private List<string> formcolumns;
        private FetchXmlBuilder fxb;
        private string entity;
        private bool initiating = true;
        private bool working = false;

        public SelectAttributesDialog(FetchXmlBuilder fxb, string entity, List<string> selectedAttributes)
        {
            this.fxb = fxb;
            this.entity = entity;
            var attributes = new List<AttributeMetadata>(fxb.GetDisplayAttributes(entity));
            InitializeComponent();
            GenerateAllItems(attributes, selectedAttributes);
            PopulateAttributes();
            initiating = false;
            SetSelectedNos();
            ActiveControl = txtFilter;
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

        private void PopulateAttributes(bool required = false, bool isonanyview = false, bool isonanyform = false)
        {
            initiating = true;
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
                    if (isonanyview && !IsOnAnyView(item.Tag as AttributeMetadata))
                    {
                        continue;
                    }
                    if (isonanyform && !IsOnAnyForm(item.Tag as AttributeMetadata))
                    {
                        continue;
                    }
                    lvAttributes.Items.Add(item);
                }
            }
            initiating = false;
            SetSelectedNos();
        }

        private bool IsRequired(AttributeMetadata meta)
        {
            return (meta.RequiredLevel?.Value == AttributeRequiredLevel.ApplicationRequired ||
                    meta.RequiredLevel?.Value == AttributeRequiredLevel.SystemRequired) &&
                   string.IsNullOrEmpty(meta.AttributeOf);
        }

        private bool IsOnAnyView(AttributeMetadata meta)
        {
            if (viewcolumns == null)
            {
                if (working)
                {
                    return false;
                }
                working = true;
                Enabled = false;
                fxb.WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading views...",
                    Work = (w, a) =>
                    {
                        if (fxb.Service == null)
                        {
                            throw new Exception("Need a connection to load views.");
                        }
                        var qexs = new QueryExpression("savedquery");
                        qexs.ColumnSet = new ColumnSet("name", "returnedtypecode", "layoutxml", "iscustomizable");
                        qexs.Criteria.AddCondition("returnedtypecode", ConditionOperator.Equal, entity);
                        qexs.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                        qexs.Criteria.AddCondition("layoutxml", ConditionOperator.NotNull);
                        a.Result = fxb.RetrieveMultiple(qexs);
                    },
                    PostWorkCallBack = (a) =>
                    {
                        if (a.Error != null)
                        {
                            fxb.ShowErrorDialog(a.Error);
                        }
                        else if (a.Result is EntityCollection views)
                        {
                            SetViewColumns(views);
                            PopulateAttributes(false, true);
                        }
                        Enabled = true;
                        working = false;
                    }
                });
                return false;
            }
            return viewcolumns.Contains(meta.LogicalName);
        }

        private bool IsOnAnyForm(AttributeMetadata meta)
        {
            if (formcolumns == null)
            {
                if (working)
                {
                    return false;
                }
                working = true;
                Enabled = false;
                fxb.WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading forms...",
                    Work = (w, a) =>
                    {
                        if (fxb.Service == null)
                        {
                            throw new Exception("Need a connection to load forms.");
                        }
                        var query = new QueryExpression("systemform");
                        query.ColumnSet.AddColumns("name", "type", "formxml");
                        query.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, entity);
                        query.Criteria.AddCondition("formactivationstate", ConditionOperator.Equal, 1);
                        query.Criteria.AddCondition("formxml", ConditionOperator.NotNull);
                        a.Result = fxb.RetrieveMultiple(query);
                    },
                    PostWorkCallBack = (a) =>
                    {
                        if (a.Error != null)
                        {
                            fxb.ShowErrorDialog(a.Error);
                        }
                        else if (a.Result is EntityCollection forms)
                        {
                            SetFormColumns(forms);
                            PopulateAttributes(isonanyform: true);
                        }
                        Enabled = true;
                        working = false;
                    }
                });
                return false;
            }
            return formcolumns.Contains(meta.LogicalName);
        }

        private void SetViewColumns(EntityCollection views)
        {
            viewcolumns = new List<string>();
            foreach (var view in views.Entities)
            {
                var layout = view.GetAttributeValue<string>("layoutxml");
                var layoutdoc = new XmlDocument();
                layoutdoc.LoadXml(layout);
                if (layoutdoc.SelectSingleNode("grid") is XmlElement grid &&
                    grid.SelectSingleNode("row") is XmlElement row)
                {
                    viewcolumns.AddRange(row.ChildNodes
                        .Cast<XmlNode>()
                        .Where(n => n.Name == "cell")
                        .Select(c => c.AttributeValue("name")));
                }
            }
            viewcolumns = viewcolumns.Distinct().ToList();
        }

        private void SetFormColumns(EntityCollection forms)
        {
            formcolumns = new List<string>();
            foreach (var form in forms.Entities)
            {
                var formxml = form.GetAttributeValue<string>("formxml");
                var formdoc = new XmlDocument();
                formdoc.LoadXml(formxml);
                var nodes = formdoc.ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    formcolumns.AddRange(FindCellControlFields(node));
                }
            }
            formcolumns = formcolumns.Distinct().ToList();
        }

        private IEnumerable<string> FindCellControlFields(XmlNode node)
        {
            var result = new List<string>();
            if (node.Name == "cell" &&
                node.SelectSingleNode("control") is XmlNode control &&
                control.AttributeValue("datafieldname") is string field &&
                !string.IsNullOrEmpty(field))
            {
                result.Add(field);
            }
            node.ChildNodes.Cast<XmlNode>().ToList().ForEach(n => result.AddRange(FindCellControlFields(n)));
            return result;
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
            Width = Math.Max(width, 450) + splitContainer1.Panel2.Width;
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
            timerSummary.Stop();
            timerSummary.Start();
        }

        private void timerSummary_Tick(object sender, EventArgs e)
        {
            timerSummary.Enabled = false;
            SetSelectedNos();
        }

        private void SetSelectedNos()
        {
            if (initiating || working)
            {
                return;
            }
            //try
            {
                lblSelectedNo.Text = $"Selected {allItems.Where(a => a.Checked).Count()}/{allItems.Count}";
            }
            //catch { }
        }

        private void lnkShowAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateAttributes();
        }

        private void lnkShowRequired_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateAttributes(required: true);
        }

        private void lnkShowOnViews_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateAttributes(isonanyview: true);
        }

        private void lnkShowOnForms_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateAttributes(isonanyform: true);
        }

        private void lnkCheckShown_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lvAttributes.Items.Cast<ListViewItem>().ToList().ForEach(a => a.Checked = true);
        }

        private void lnkUncheckShown_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lvAttributes.Items.Cast<ListViewItem>().ToList().ForEach(a => a.Checked = false);
        }

        private void lnkUncheckAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            allItems.ForEach(a => a.Checked = false);
        }
    }
}