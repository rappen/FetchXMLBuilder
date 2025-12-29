using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class ShowMetadataOptions : Form
    {
        private FetchXmlBuilder fxb;
        private Guid selectedsolution;
        private Guid selectedpublisher;
        private List<Entity> solutions;
        private List<Entity> publishers;

        public static bool Show(FetchXmlBuilder fxb, Action<bool> applysetting)
        {
            fxb.WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading dialog",
                Work = (worker, args) =>
                {
                    var form = new ShowMetadataOptions();
                    form.fxb = fxb;
                    form.PopulateSolutionsSettings(fxb.connectionsettings.FilterSetting);
                    form.PopulateEntitiesSettings(fxb.connectionsettings.ShowEntities);
                    form.PopulateAttributesSettings(fxb.connectionsettings.ShowAttributes);
                    //form.LoadSolutions();
                    //form.LoadPublishers();
                    args.Result = form;
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        fxb.ShowErrorDialog(args.Error);
                    }
                    else if (args.Result is ShowMetadataOptions form)
                    {
                        form.UpdateSelections();
                        var result = form.ShowDialog() == DialogResult.OK;
                        if (result)
                        {
                            fxb.connectionsettings.FilterSetting = form.GetFilterSetting();
                            fxb.connectionsettings.ShowEntities = form.GetEntitiesSettings();
                            fxb.connectionsettings.ShowAttributes = form.GetAttributesSettings();
                        }
                        applysetting?.Invoke(result);
                    }
                }
            });
            return false;
        }

        private ShowMetadataOptions()
        {
            InitializeComponent();
        }

        private void PopulateSolutionsSettings(FilterSetting setting)
        {
            rbAllSolutions.Checked = setting.ShowAllSolutions;
            rbUnmanagedSolution.Checked = setting.ShowUnmanagedSolutions;
            rbSpecificSolution.Checked = setting.ShowSolution;
            rbSpecificPublisher.Checked = setting.ShowPublisher;
            selectedsolution = setting.SolutionId;
            selectedpublisher = setting.PublisherId;
            chkFilterMetadata.Checked = setting.FilterByMetadata;
            chkHideDeprecated.Checked = setting.HideDeprecated;
            chkAShowPrimary.Checked = setting.AlwaysPrimary;
            chkAShowAddress.Checked = setting.AlwaysAddresses;
        }

        private void PopulateEntitiesSettings(ShowMetaTypesEntity setting)
        {
            chkEManaged.CheckState = setting.IsManaged;
            chkECustom.CheckState = setting.IsCustom;
            chkECustomizable.CheckState = setting.IsCustomizable;
            chkVirtual.CheckState = setting.Virtual;
            chkEIntersect.CheckState = setting.IsIntersect;
            chkEAdvFind.CheckState = setting.IsValidForAdvancedFind;
            chkEAudit.CheckState = setting.IsAuditEnabled;
            chkELogical.CheckState = setting.IsLogical;
            chkEActivity.CheckState = setting.IsActivity;
            chkEActivityParty.CheckState = setting.IsActivityParty;
            cmbEOwnership.SelectedIndex = setting.OwnershipType;
        }

        private void PopulateAttributesSettings(ShowMetaTypesAttribute setting)
        {
            chkAAdvFind.CheckState = setting.IsValidForAdvancedFind;
            chkAAudit.CheckState = setting.IsAuditEnabled;
            chkACustom.CheckState = setting.IsCustom;
            chkACustomizable.CheckState = setting.IsCustomizable;
            chkAFiltered.CheckState = setting.IsFiltered;
            chkAGrid.CheckState = setting.IsValidForGrid;
            chkALogical.CheckState = setting.IsLogical;
            chkAManaged.CheckState = setting.IsManaged;
            chkARead.CheckState = setting.IsValidForRead;
            chkARetrievable.CheckState = setting.IsRetrievable;
            chkAAttributeOf.CheckState = setting.AttributeOf;
            chkACalculationOf.CheckState = setting.CalculationOf;
        }

        private FilterSetting GetFilterSetting()
        {
            return new FilterSetting
            {
                ShowAllSolutions = rbAllSolutions.Checked,
                ShowUnmanagedSolutions = rbUnmanagedSolution.Checked,
                ShowSolution = rbSpecificSolution.Checked && xrmSolution.SelectedRecord != null,
                ShowPublisher = rbSpecificPublisher.Checked && xrmSolution.SelectedRecord != null,
                SolutionId = selectedsolution,
                PublisherId = selectedpublisher,
                FilterByMetadata = chkFilterMetadata.Checked,
                HideDeprecated = chkHideDeprecated.Checked,
                AlwaysPrimary = chkAShowPrimary.Checked,
                AlwaysAddresses = chkAShowAddress.Checked
            };
        }

        private ShowMetaTypesEntity GetEntitiesSettings()
        {
            return new ShowMetaTypesEntity
            {
                IsManaged = chkEManaged.CheckState,
                IsCustom = chkECustom.CheckState,
                IsCustomizable = chkECustomizable.CheckState,
                Virtual = chkVirtual.CheckState,
                IsIntersect = chkEIntersect.CheckState,
                IsValidForAdvancedFind = chkEAdvFind.CheckState,
                IsAuditEnabled = chkEAudit.CheckState,
                IsLogical = chkELogical.CheckState,
                IsActivity = chkEActivity.CheckState,
                IsActivityParty = chkEActivityParty.CheckState,
                OwnershipType = cmbEOwnership.SelectedIndex
            };
        }

        private ShowMetaTypesAttribute GetAttributesSettings()
        {
            return new ShowMetaTypesAttribute
            {
                IsManaged = chkAManaged.CheckState,
                IsCustom = chkACustom.CheckState,
                IsCustomizable = chkACustomizable.CheckState,
                IsValidForAdvancedFind = chkAAdvFind.CheckState,
                IsAuditEnabled = chkAAudit.CheckState,
                IsLogical = chkALogical.CheckState,
                IsValidForRead = chkARead.CheckState,
                IsValidForGrid = chkAGrid.CheckState,
                IsFiltered = chkAFiltered.CheckState,
                IsRetrievable = chkARetrievable.CheckState,
                AttributeOf = chkAAttributeOf.CheckState,
                CalculationOf = chkACalculationOf.CheckState
            };
        }

        private Guid GetPublisherId(Entity entity)
        {
            return
                entity != null &&
                entity.Contains("Id") &&
                entity["Id"] is AliasedValue value &&
                value != null &&
                value.Value is Guid id ? id : Guid.Empty;
        }

        private void SetPublisherId(Guid id)
        {
            foreach (var item in xrmSolution.Items)
            {
                if (item is Helpers.ControlItems.EntityItem entityitem)
                {
                    if (GetPublisherId(entityitem.Entity).Equals(id))
                    {
                        xrmSolution.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void UpdateSelections(object sender = null, EventArgs e = null)
        {
            if (!Visible)
            {
                return;
            }
            var settingsentities = GetEntitiesSettings();
            var selentities = gbEntities.Controls
                   .OfType<CheckBox>()
                   .Where(c => c.CheckState != CheckState.Indeterminate)
                   .Select(c => c.Text).ToList();
            if (settingsentities.Ownerships != null)
            {
                selentities.AddRange(settingsentities.Ownerships
                    .Select(o => o.ToString().RemoveEnd("Owned")).ToList());
            }
            lblEntities.Text = string.Join(", ", selentities.ToArray());
            var selattributes = gbAttributes.Controls
                   .OfType<CheckBox>()
                   .Where(c => c.CheckState != CheckState.Indeterminate)
                   .Select(c => c.Text).ToList();
            if (chkAShowPrimary.Checked)
            {
                selattributes.Add("Always Primary");
            }
            if (chkAShowAddress.Checked)
            {
                selattributes.Add("Always Addresses");
            }
            lblAttributes.Text = string.Join(", ", selattributes.ToArray());
            UpdatePreviewEntities();
        }

        private void UpdatePreviewEntities()
        {
            var selected = lbPreviewEntities.SelectedItem?.ToString();
            var entities = fxb.GetDisplayEntities(GetFilterSetting(), GetEntitiesSettings());
            lblPreviewEntities.Text = $"Entities filtered: {entities.Count}";
            lbPreviewEntities.Items.Clear();
            lbPreviewEntities.Items.AddRange(entities.Select(en => en.LogicalName).ToArray());
            lbPreviewEntities.SelectedItem = selected;
            UpdatePreviewAttributes();
        }

        private void UpdatePreviewAttributes(object sender = null, EventArgs e = null)
        {
            lbPreviewAttributes.Items.Clear();
            if (lbPreviewEntities.SelectedItem == null)
            {
                if (lbPreviewEntities.Items.Count == 0)
                {
                    lblPreviewAttributes.Text = "Select an entity to see attributes";
                }
                else
                {
                    lbPreviewEntities.SelectedIndex = 0;
                }
                return;
            }
            var entity = lbPreviewEntities.SelectedItem.ToString();
            var attributes = fxb.GetDisplayAttributes(entity, GetFilterSetting(), GetAttributesSettings());
            lblPreviewAttributes.Text = $"Attributes filtered for {entity}: {attributes.Count()}";
            lbPreviewAttributes.Items.AddRange(attributes.Select(at => at.LogicalName).ToArray());
        }

        private void btnEClear_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx.Show(this, "Reset to default entities.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                PopulateEntitiesSettings(new ShowMetaTypesEntity());
            }
        }

        private void btnAClear_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx.Show(this, "Reset to default attributes.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                PopulateAttributesSettings(new ShowMetaTypesAttribute());
            }
        }

        private void helpIcon_Click(object sender, EventArgs e)
        {
            fxb.OpenUrl(sender);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            fxb.OpenUrl(sender);
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
            UpdateSelections();
        }

        private void PopulateSolutions()
        {
            if (!Visible)
            {
                return;
            }
            if (solutions == null || solutions.Count == 0)
            {
                LoadSolutions();
            }
            xrmSolution.DisplayFormat = "{friendlyname} ({P.friendlyname})";
            xrmSolution.Service = fxb.Service;
            xrmSolution.DataSource = solutions.Where(s => chkShowAllSolutions.Checked || s.GetAttributeValue<bool>("isvisible") == true);
            xrmSolution.SetSelected(selectedsolution);
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
            xrmSolution.DataSource = publishers;
            SetPublisherId(selectedpublisher);
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
            UpdateSelections();
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
            UpdateSelections();
        }

        private void xrmSolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (xrmSolution.Focused && xrmSolution.SelectedRecord is Entity selected)
            {
                if (rbSpecificSolution.Checked)
                {
                    selectedsolution = selected.Id;
                }
                else if (rbSpecificPublisher.Checked)
                {
                    selectedpublisher = GetPublisherId(selected);
                }
            }
            fxb.solutionentities = null;
            fxb.solutionattributes = null;
            UpdateSelections();
        }

        private void chkFilterMetadata_CheckedChanged(object sender, EventArgs e)
        {
            gbEntities.Enabled = chkFilterMetadata.Checked;
            gbAttributes.Enabled = chkFilterMetadata.Checked;
            UpdateSelections();
        }

        internal void LoadSolutions()
        {
            if (fxb.Service == null)
            {
                solutions = new List<Entity>();
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
                solutions = fxb.RetrieveMultiple(query).Entities.ToList();
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
                publishers = new List<Entity>();
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
                publishers = fxb.RetrieveMultiple(new FetchExpression(fetch)).Entities.ToList();
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