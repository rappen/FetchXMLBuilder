using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class ShowMetadataOptions : Form
    {
        private FetchXmlBuilder fxb;
        private Guid selectedsolution;
        private List<Entity> solutions;

        public static bool Show(FetchXmlBuilder fxb, Action<bool> applysetting)
        {
            fxb.WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading dialog",
                Work = (worker, args) =>
                {
                    var form = new ShowMetadataOptions();
                    form.fxb = fxb;
                    form.btnPreview.Enabled = fxb.Service != null;
                    form.PopulateSolutionsSettings(fxb.connectionsettings.FilterSetting);
                    form.PopulateEntitiesSettings(fxb.connectionsettings.ShowEntities);
                    form.PopulateAttributesSettings(fxb.connectionsettings.ShowAttributes);
                    form.LoadSolutions();
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
            Preview(false);
        }

        private void PopulateSolutionsSettings(FilterSetting setting)
        {
            rbAllSolutions.Checked = setting.ShowAllSolutions;
            rbUnmanagedSolution.Checked = setting.ShowUnmanagedSolutions;
            rbSpecificSolution.Checked = !setting.ShowSolution.Equals(Guid.Empty);
            selectedsolution = setting.ShowSolution;
            chkFilterMetadata.Checked = setting.FilterByMetadata;
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
        }

        private FilterSetting GetFilterSetting()
        {
            return new FilterSetting
            {
                ShowAllSolutions = rbAllSolutions.Checked,
                ShowUnmanagedSolutions = rbUnmanagedSolution.Checked,
                ShowSolution = rbSpecificSolution.Checked && xrmSolution.SelectedRecord != null ? xrmSolution.SelectedRecord.Id : Guid.Empty,
                FilterByMetadata = chkFilterMetadata.Checked,
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
            };
        }

        private void checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chk && chk.Tag is string tag)
            {
                chk.Text = chk.CheckState.TriToString(tag);
            }
            UpdateSelections();
        }

        private void UpdateSelections(object sender = null, EventArgs e = null)
        {
            var settingsfilter = GetFilterSetting();
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
            if (Width < 600)
            {
                return;
            }
            var entities = fxb.GetDisplayEntities(settingsfilter, settingsentities);
            gbPreviewEntities.Text = $"Preview Entities selected: {entities.Count}";
            lbPreviewEntities.Items.Clear();
            lbPreviewEntities.Items.AddRange(entities.Keys.ToArray());
        }

        private void btnEClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset to default entities.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                PopulateEntitiesSettings(new ShowMetaTypesEntity());
            }
        }

        private void btnAClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset to default attributes.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                PopulateAttributesSettings(new ShowMetaTypesAttribute());
            }
        }

        private void helpIcon_Click(object sender, EventArgs e)
        {
            FetchXmlBuilder.HelpClick(sender);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.HelpClick(sender);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Preview(Width < 600);
        }

        private void Preview(bool preview)
        {
            if (preview)
            {
                Width = gbPreviewEntities.Left + gbPreviewEntities.Width + 30;
                UpdateSelections();
            }
            else
            {
                Width = gbAttributes.Left + gbAttributes.Width + 30;
            }
        }

        private void chkAShowAlways_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelections();
        }

        private void rbAllSolutions_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSpecificSolution.Checked)
            {
                panSelectSolution.Enabled = true;
                PopulateSolutions();
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
            xrmSolution.Service = fxb.Service;
            xrmSolution.DataSource = solutions.Where(s => chkShowAllSolutions.Checked || s.GetAttributeValue<bool>("isvisible") == true);
            xrmSolution.SetSelected(selectedsolution);
            Enabled = true;
        }

        private void chkShowAllSolutions_CheckedChanged(object sender, EventArgs e)
        {
            PopulateSolutions();
            UpdateSelections();
        }

        private void ShowMetadataOptions_Load(object sender, EventArgs e)
        {
            PopulateSolutions();
            UpdateSelections();
        }

        private void xrmSolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            fxb.solutionentities = null;
            fxb.solutionattributes = null;
            UpdateSelections();
        }

        private void chkFilterMetadata_CheckedChanged(object sender, EventArgs e)
        {
            gbEntities.Enabled = chkFilterMetadata.Checked;
            gbAttributes.Enabled = chkFilterMetadata.Checked;
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
                solutions = fxb.Service.RetrieveMultiple(query).Entities.ToList();
            }
            catch (Exception ex)
            {
                fxb.ShowErrorDialog(ex, "Loading Solutions");
            }
        }
    }
}
