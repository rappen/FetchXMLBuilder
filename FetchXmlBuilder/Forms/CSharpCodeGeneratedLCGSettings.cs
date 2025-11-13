using Rappen.XTB.Helpers;
using Rappen.XTB.LCG;
using System;
using System.IO;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class CSharpCodeGeneratedLCGSettings : Form
    {
        private string sourcefile;

        private CSharpCodeGeneratedLCGSettings()
        {
            InitializeComponent();
        }

        internal static bool GetSettings(FetchXmlBuilder fxb, LCG.Settings settings)
        {
            using (var settingdlg = new CSharpCodeGeneratedLCGSettings())
            {
                settingdlg.SetSettings(settings);
                if (settingdlg.ShowDialog(fxb) == DialogResult.OK)
                {
                    settings.ConstantName = (NameType)Math.Max(settingdlg.cmbConstantName.SelectedIndex, 0);
                    settings.ConstantCamelCased = settingdlg.chkConstCamelCased.Checked;
                    settings.DoStripPrefix = settingdlg.chkConstStripPrefix.Checked;
                    settings.StripPrefix = settingdlg.txtConstStripPrefix.Text.ToLowerInvariant().TrimEnd('_') + "_";
                    settings.SourceFile = settingdlg.sourcefile;
                    return true;
                }
            }
            return false;
        }

        private void SetSettings(LCG.Settings settings)
        {
            cmbConstantName.SelectedIndex = (int)settings.ConstantName;
            chkConstCamelCased.Checked = settings.ConstantCamelCased && settings.ConstantName != NameType.DisplayName;
            chkConstStripPrefix.Checked = settings.DoStripPrefix && settings.ConstantName != NameType.DisplayName;
            txtConstStripPrefix.Text = settings.StripPrefix;
            sourcefile = settings.SourceFile;
        }

        private void cmbConstantName_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkConstCamelCased.Visible = cmbConstantName.SelectedIndex != (int)NameType.DisplayName;
            chkConstStripPrefix.Enabled = cmbConstantName.SelectedIndex != (int)NameType.DisplayName;
            txtConstStripPrefix.Enabled = chkConstStripPrefix.Enabled && chkConstStripPrefix.Checked;
        }

        private void chkConstStripPrefix_CheckedChanged(object sender, EventArgs e)
        {
            txtConstStripPrefix.Enabled = chkConstStripPrefix.Enabled && chkConstStripPrefix.Checked;
        }

        private void txtConstStripPrefix_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtConstStripPrefix.Text))
            {
                txtConstStripPrefix.Text = txtConstStripPrefix.Text.ToLowerInvariant().TrimEnd('_') + "_";
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = $"Load generated C# or config file",
                Filter = $"C# file (*.cs)|*.cs|XML file (*.xml)|*.xml"
            };
            if (!string.IsNullOrWhiteSpace(sourcefile))
            {
                ofd.InitialDirectory = Path.GetDirectoryName(sourcefile);
            }
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var type = Path.GetExtension(ofd.FileName).ToLowerInvariant();
                try
                {
                    var lcgsettings = new LCG.Settings();
                    if (type == ".cs")
                    {
                        lcgsettings = ConfigurationUtils.GetEmbeddedConfiguration<LCG.Settings>(ofd.FileName, lcgsettings.commonsettings.InlineConfigBegin, lcgsettings.commonsettings.InlineConfigEnd) ?? new LCG.Settings();
                    }
                    else if (type == ".xml")
                    {
                        lcgsettings = XmlAtomicStore.Deserialize<LCG.Settings>(File.ReadAllText(ofd.FileName));
                    }
                    lcgsettings.SourceFile = ofd.FileName;
                    SetSettings(lcgsettings);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to parse configuration.\n\n{ex.Message}", "Open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}