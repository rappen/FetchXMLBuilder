using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class RefreshMetadataOptions : Form
    {
        private FetchXmlBuilder fxb;
        private Guid SelectedSolution;
        private Guid SleectedPublisher;
        private List<Entity> Solutions;
        private List<Entity> Publishers;

        public static bool Show(FetchXmlBuilder fxb, Action<bool, FilterSetting> Callback)
        {
            fxb.WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading dialog",
                Work = (worker, args) =>
                {
                    args.Result = new RefreshMetadataOptions
                    {
                        fxb = fxb
                    };
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null) fxb.ShowErrorDialog(args.Error);
                    else if (args.Result is RefreshMetadataOptions form)
                    {
                        var ok = form.ShowDialog() == DialogResult.OK;
                        Callback?.Invoke(ok, ok ? form.BuildOptions() : null);
                    }
                }
            });
            return false;
        }

        private RefreshMetadataOptions()
        {
            InitializeComponent();
        }

        private FilterSetting BuildOptions()
        {
            return new FilterSetting
            {
                ShowAllSolutions = false,
                ShowUnmanagedSolutions = rbUnmanagedSolution.Checked,
                ShowSolution = rbSpecificSolution.Checked,
                ShowPublisher = rbSpecificPublisher.Checked,
                SolutionId = rbSpecificSolution.Checked && xrmSolution.SelectedRecord != null ? xrmSolution.SelectedRecord.Id : Guid.Empty,
                PublisherId = rbSpecificPublisher.Checked && xrmSolution.SelectedRecord != null ? GetPublisherId(xrmSolution.SelectedRecord) : Guid.Empty,
            };
        }

        private Guid GetPublisherId(Entity entity)
        {
            return
                entity?.Contains("Id") == true &&
                entity["Id"] is AliasedValue value &&
                value?.Value is Guid id ? id : Guid.Empty;
        }

        private void SetPublisherId(Guid id)
        {
            xrmSolution.SelectedItem =
                xrmSolution.Items
                .OfType<Helpers.ControlItems.EntityItem>()
                .FirstOrDefault(ei => GetPublisherId(ei.Entity).Equals(id));
        }

        private void rbAllSolutions_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSpecificSolution.Checked)
            {
                panSelectSolution.Enabled = true;
                PopulateSolutions();
            }
            else if (rbSpecificPublisher.Checked)
            {
                panSelectSolution.Enabled = true;
                PopulatePublishers();
            }
            else
            {
                panSelectSolution.Enabled = false;
                xrmSolution.SelectedIndex = -1;
            }
        }

        private void PopulateSolutions()
        {
            if (!Visible)
            {
                return;
            }
            if (Solutions == null || Solutions.Count == 0)
            {
                LoadSolutions();
            }
            xrmSolution.DisplayFormat = "{friendlyname} ({P.friendlyname})";
            xrmSolution.Service = fxb.Service;
            xrmSolution.DataSource = Solutions.Where(s => chkShowAllSolutions.Checked || s.GetAttributeValue<bool>("isvisible") == true);
            xrmSolution.SetSelected(SelectedSolution);
            Enabled = true;
        }

        private void PopulatePublishers()
        {
            if (!Visible)
            {
                return;
            }
            LoadPublishers();
            xrmSolution.DisplayFormat = "{Name} ({Solutions} solutions)";
            xrmSolution.Service = fxb.Service;
            xrmSolution.DataSource = Publishers;
            SetPublisherId(SleectedPublisher);
            Enabled = true;
        }

        private void chkShowAllSolutions_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSpecificSolution.Checked)
            {
                PopulateSolutions();
            }
            else if (rbSpecificPublisher.Checked)
            {
                PopulatePublishers();
            }
        }

        private void ShowMetadataOptions_Load(object sender, EventArgs e)
        {
            if (rbSpecificSolution.Checked)
            {
                PopulateSolutions();
            }
            else if (rbSpecificPublisher.Checked)
            {
                PopulatePublishers();
            }
        }

        private void xrmSolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (xrmSolution.Focused && xrmSolution.SelectedRecord is Entity selected)
            {
                if (rbSpecificSolution.Checked)
                {
                    SelectedSolution = selected.Id;
                }
                else if (rbSpecificPublisher.Checked)
                {
                    SleectedPublisher = GetPublisherId(selected);
                }
            }
        }

        internal void LoadSolutions()
        {
            if (fxb.Service == null)
            {
                Solutions = new List<Entity>();
                return;
            }
            var query = new QueryExpression("solution");
            query.ColumnSet.AddColumns("friendlyname", "uniquename", "ismanaged", "isvisible", "version");
            query.AddOrder("ismanaged", OrderType.Descending);
            query.AddOrder("friendlyname", OrderType.Ascending);
            var publisher = query.AddLink("publisher", "publisherid", "publisherid");
            publisher.EntityAlias = "P";
            publisher.Columns.AddColumns("customizationprefix", "uniquename", "friendlyname");
            try
            {
                Solutions = fxb.RetrieveMultiple(query).Entities.ToList();
            }
            catch (Exception ex)
            {
                fxb.ShowErrorDialog(ex, "Loading Solutions");
            }
        }

        private void LoadPublishers()
        {
            if (fxb.Service == null)
            {
                Publishers = new List<Entity>();
                return;
            }
            var fetch = @"<fetch aggregate='true'>
  <entity name='publisher'>
    <attribute name='friendlyname' alias='Name' groupby='true' />
    <attribute name='publisherid' alias='Id' groupby='true' />
    <order alias='Name' />
    <link-entity name='solution' from='publisherid' to='publisherid' link-type='inner' alias='S'>
      <attribute name='solutionid' alias='Solutions' aggregate='countcolumn' />
      {0}
    </link-entity>
  </entity>
</fetch>";
            fetch = string.Format(fetch, chkShowAllSolutions.Checked ? "" : "<filter><condition attribute='isvisible' operator='eq' value='1'/></filter>");
            try
            {
                Cursor = Cursors.WaitCursor;
                Publishers = fxb.RetrieveMultiple(new FetchExpression(fetch)).Entities.ToList();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                fxb.ShowErrorDialog(ex, "Loading Publishers");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}