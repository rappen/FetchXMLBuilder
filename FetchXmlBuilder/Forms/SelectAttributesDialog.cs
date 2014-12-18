using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
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
    public partial class SelectAttributesDialog : Form
    {
        public SelectAttributesDialog(List<AttributeMetadata> attributes, List<string> selectedAttributes)
        {
            InitializeComponent();
            lvAttributes.Items.Clear();
            foreach (var attribute in attributes)
            {
                var name = attribute.DisplayName.UserLocalizedLabel != null ? attribute.DisplayName.UserLocalizedLabel.Label : attribute.LogicalName;
                var item = new ListViewItem(new string[] { name, attribute.LogicalName, attribute.AttributeType.ToString() });
                item.Name = attribute.LogicalName;
                item.Text = name;
                item.Tag = attribute;
                item.Checked = selectedAttributes.Contains(attribute.LogicalName);
                lvAttributes.Items.Add(item);
            }
        }

        public List<AttributeMetadata> GetSelectedAttributes()
        {
            var result = new List<AttributeMetadata>();
            foreach (ListViewItem item in lvAttributes.CheckedItems)
            {
                result.Add((AttributeMetadata)item.Tag);
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
    }
}
