using Cinteros.Xrm.CRMWinForm;
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class ResultGrid : Form
    {
        FetchXmlBuilder form;

        public ResultGrid(EntityCollection entities, FetchXmlBuilder fetchXmlBuilder)
        {
            InitializeComponent();
            form = fetchXmlBuilder;
            var size = form.currentSettings.gridWinSize;
            if (size != null && size.Width > 0 && size.Height > 0)
            {
                Width = size.Width;
                Height = size.Height; ;
            }
            menuFriendly.Checked = form.currentSettings.gridFriendly;
            menuIdColumn.Checked = form.currentSettings.gridId;
            menuIndexColumn.Checked = form.currentSettings.gridIndex;
            crmGridView1.ShowFriendlyNames = form.currentSettings.gridFriendly;
            crmGridView1.ShowIdColumn = form.currentSettings.gridId;
            crmGridView1.ShowIndexColumn = form.currentSettings.gridIndex;
            crmGridView1.OrganizationService = form.Service;
            crmGridView1.DataSource = entities;
        }

        private void ResultGrid_Load(object sender, EventArgs e)
        {   // Must be done after form has become visible
            crmGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader);
        }

        private void crmGridView1_RecordDoubleClick(object sender, CRMRecordEventArgs e)
        {
            if (e.Entity != null && !e.Entity.Id.Equals(Guid.Empty))
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
                    e.Entity.LogicalName,
                    "&pagetype=entityrecord&id=",
                    e.Entity.Id.ToString());
                form.LogUse("OpenRecord");
                Process.Start(url);
            }
        }

        private void ResultGrid_FormClosing(object sender, FormClosingEventArgs e)
        {
            form.currentSettings.gridWinSize = new System.Drawing.Size(Width, Height);
            form.currentSettings.gridFriendly = menuFriendly.Checked;
            form.currentSettings.gridId = menuIdColumn.Checked;
            form.currentSettings.gridIndex = menuIndexColumn.Checked;
        }

        private void menuFriendly_CheckedChanged(object sender, EventArgs e)
        {
            crmGridView1.ShowFriendlyNames = menuFriendly.Checked;
        }

        private void menuIdColumn_CheckedChanged(object sender, EventArgs e)
        {
            crmGridView1.ShowIdColumn = menuIdColumn.Checked;
        }

        private void menuIndexColumn_CheckedChanged(object sender, EventArgs e)
        {
            crmGridView1.ShowIndexColumn = menuIndexColumn.Checked;
        }
    }
}
