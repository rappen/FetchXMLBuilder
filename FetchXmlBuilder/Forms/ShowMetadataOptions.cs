using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class ShowMetadataOptions : Form
    {
        private FetchXmlBuilder fxb;

        public static bool Show(FetchXmlBuilder fxb)
        {
            var form = new ShowMetadataOptions();
            form.fxb = fxb;
            form.PopulateEntitiesSettings(fxb.settings.ShowEntities);
            form.PopulateAttributesSettings(fxb.settings.ShowAttributes);
            if (form.ShowDialog() == DialogResult.OK)
            {
                fxb.settings.ShowEntities = form.GetEntitiesSettings();
                fxb.settings.ShowAttributes = form.GetAttributesSettings();
                return true;
            }
            return false;
        }

        private ShowMetadataOptions()
        {
            InitializeComponent();
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
                AttributeOf = chkAAttributeOf.CheckState
            };
        }

        private void checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chk && chk.Tag is string tag)
            {
                chk.Text = chk.CheckState.TriToString(tag);
            }
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
    }
}
