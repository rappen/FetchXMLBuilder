using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public ResultGrid(EntityCollection Entities)
        {
            InitializeComponent();
            entities = Entities;
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
                    if (!columns.Contains(attribute))
                    {
                        columns.Add(attribute);
                    }
                }
            }
            lvGrid.Columns.Clear();
            foreach (var col in columns)
            {
                lvGrid.Columns.Add(col);
            }
        }

        private void FillData()
        {
            lvGrid.Items.Clear();
            foreach (var entity in entities.Entities)
            {
                var item = lvGrid.Items.Add(entity.Id.ToString());
                for (var i = 1; i < columns.Count; i++)
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
    }
}
