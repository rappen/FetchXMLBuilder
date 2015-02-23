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
        private List<string> columns = null;
        ConnectionDetail connection = null;

        public ResultGrid(EntityCollection Entities, ConnectionDetail Connection)
        {
            InitializeComponent();
            entities = Entities;
            connection = Connection;
            SetupColumns();
            FillData();
        }

        private void SetupColumns()
        {
            columns = new List<string>();
            foreach (var entity in entities.Entities)
            {
                foreach (var attribute in entity.Attributes.Keys)
                {
                    if (entity[attribute] is Guid && (Guid)entity[attribute] == entity.Id)
                    {
                        continue;
                    }
                    if (columns.Contains(attribute))
                    {
                        continue;
                    }
                    columns.Add(attribute);
                }
            }
            lvGrid.Columns.Clear();
            var nohdr = lvGrid.Columns.Add("#");
            nohdr.TextAlign = HorizontalAlignment.Right;
            lvGrid.Columns.Add("Id");
            foreach (var col in columns)
            {
                lvGrid.Columns.Add(col);
            }
        }

        private void FillData()
        {
            lvGrid.Items.Clear();
            var no = 0;
            foreach (var entity in entities.Entities)
            {
                var item = lvGrid.Items.Add((++no).ToString());
                item.SubItems.Add(entity.Id.ToString());
                for (var i = 0; i < columns.Count; i++)
                {
                    var col = columns[i];
                    var valuestr = "";
                    if (entity.Contains(col))
                    {
                        var value = entity[col];
                        if (value != null)
                        {
                            try
                            {
                                valuestr = EntitySerializer.AttributeToBaseType(value).ToString();
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
            if (entity != null)
            {
                var url = connection.WebApplicationUrl;
                if (string.IsNullOrEmpty(url))
                {
                    url = string.Concat(connection.ServerName, "/", connection.Organization);
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
    }
}
