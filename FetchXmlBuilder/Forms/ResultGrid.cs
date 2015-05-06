using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.XmlEditorUtils;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class ResultGrid : Form
    {
        private EntityCollection entities = null;
        private Dictionary<string, AttributeItem> columns = null;
        private int sortcolumn = -1;
        FetchXmlBuilder form;

        public ResultGrid(EntityCollection Entities, FetchXmlBuilder fetchXmlBuilder)
        {
            InitializeComponent();
            entities = Entities;
            form = fetchXmlBuilder;
            var size = form.currentSettings.gridWinSize;
            if (size != null && size.Width > 0 && size.Height > 0)
            {
                Width = size.Width;
                Height = size.Height; ;
            }
            if (form.currentSettings.gridFriendly)
            {   // This pretty stupid if/else because setting the Friendly flag will trigger RefreshAll, and we don't want it twice.
                chkFriendly.Checked = true;
            }
            else
            {
                RefreshAll();
            }
        }

        private void RefreshAll()
        {
            if (form.currentSettings.gridFriendly && form.NeedToLoadEntity(entities.EntityName))
            {
                form.LoadEntityDetails(entities.EntityName, RefreshAll);
            }
            else
            {
                var sort = lvGrid.Sorting;
                lvGrid.Sorting = SortOrder.None;
                SetupColumns();
                FillData();
                lvGrid.Sorting = sort;
            }
        }

        private void SetupColumns()
        {
            columns = new Dictionary<string, AttributeItem>();
            foreach (var entity in entities.Entities)
            {
                foreach (var attribute in entity.Attributes.Keys)
                {
                    if (entity[attribute] is Guid && (Guid)entity[attribute] == entity.Id)
                    {
                        continue;
                    }
                    if (columns.ContainsKey(attribute))
                    {
                        continue;
                    }
                    var meta = FetchXmlBuilder.GetAttribute(entities.EntityName, attribute);
                    columns.Add(attribute, new AttributeItem(meta));
                }
            }
            lvGrid.Columns.Clear();
            var nohdr = lvGrid.Columns.Add("#", 20, HorizontalAlignment.Right);
            lvGrid.Columns.Add("Id");
            foreach (var col in columns)
            {
                lvGrid.Columns.Add(form.currentSettings.gridFriendly && col.Value.Metadata != null ? col.Value.Metadata.DisplayName.UserLocalizedLabel.Label : col.Key);
            }
        }

        private void FillData()
        {
            lvGrid.Items.Clear();
            var no = 0;
            foreach (var entity in entities.Entities)
            {
                var item = lvGrid.Items.Add((++no).ToString());
                item.SubItems.Add(entity.Id.Equals(Guid.Empty) ? "" : entity.Id.ToString());
                foreach (var column in columns)
                {
                    var col = column.Key;
                    var valuestr = "";
                    if (entity.Contains(col))
                    {
                        var value = entity[col];
                        if (value != null)
                        {
                            try
                            {
                                if (form.currentSettings.gridFriendly)
                                {
                                    valuestr = EntitySerializer.AttributeToString(value, column.Value.Metadata);
                                }
                                else
                                {
                                    valuestr = EntitySerializer.AttributeToBaseType(value).ToString();
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Attribute " + col + " failed");
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(valuestr))
                    {
                        valuestr = "<null>";
                    }
                    item.SubItems.Add(valuestr);
                }
            }
            lvGrid.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void lvGrid_DoubleClick(object sender, EventArgs e)
        {
            if (lvGrid.SelectedIndices.Count == 0)
            {
                return;
            }
            var index = lvGrid.SelectedIndices[0];
            var entity = entities[index];
            if (entity != null && !entity.Id.Equals(Guid.Empty))
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
                    "/main.aspx?etn=",
                    entities.EntityName,
                    "&pagetype=entityrecord&id=",
                    entity.Id.ToString());

                Process.Start(url);
            }
        }

        private void ResultGrid_FormClosing(object sender, FormClosingEventArgs e)
        {
            form.currentSettings.gridWinSize = new System.Drawing.Size(Width, Height);
        }

        private void chkFriendly_CheckedChanged(object sender, EventArgs e)
        {
            form.currentSettings.gridFriendly = chkFriendly.Checked;
            RefreshAll();
        }

        private void lvGrid_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortcolumn)
            {
                lvGrid.Sorting = lvGrid.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                lvGrid.Sorting = SortOrder.Ascending;
            }
            lvGrid.ListViewItemSorter = new ListViewItemComparer(e.Column, lvGrid.Sorting);
            sortcolumn = e.Column;
        }
    }
}
