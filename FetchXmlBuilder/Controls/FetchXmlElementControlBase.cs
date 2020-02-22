using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public class FetchXmlElementControlBase : UserControl, IDefinitionSavable
    {
        private Dictionary<string, string> collec;
        private Dictionary<string, string> original;
        private string controlsCheckSum = "";
        private ErrorProvider errorProvider;
        private ErrorProvider warningProvider;
        
        static FetchXmlElementControlBase()
        {
            // Create the small warning icon to use for user feedback
            // https://stackoverflow.com/questions/3031124/is-there-a-way-to-get-different-sizes-of-the-windows-system-icons-in-net
            Size iconSize = SystemInformation.SmallIconSize;
            Bitmap bitmap = new Bitmap(iconSize.Width, iconSize.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(SystemIcons.Warning.ToBitmap(), new Rectangle(Point.Empty, iconSize));
            }

            WarningIcon = Icon.FromHandle(bitmap.GetHicon());
        }

        protected static Icon WarningIcon { get; }

        public void InitializeFXB(Dictionary<string, string> collection, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree, TreeNode node)
        {
            fxb = fetchXmlBuilder;
            Node = node;
            Tree = tree;
            if (collection != null)
            {
                collec = collection;
            }
            else if (node != null)
            {
                collec = (Dictionary<string, string>)node.Tag;
            }

            original = new Dictionary<string, string>(collec);
            errorProvider = new ErrorProvider(this);
            warningProvider = new ErrorProvider(this);
            warningProvider.Icon = WarningIcon;
            PopulateControls();
            ControlUtils.FillControls(collec, this.Controls, this);
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            Saved += tree.CtrlSaved;
            AttachValidatingEvent(this);
        }

        protected FetchXmlBuilder fxb { get; set; }

        protected TreeNode Node { get; set; }

        protected TreeBuilderControl Tree { get; set; }

        protected virtual void PopulateControls()
        {
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);

            if (RequiresSave())
            {
                e.Cancel = !Save(false);
            }
        }

        protected virtual bool RequiresSave()
        {
            return controlsCheckSum != ControlUtils.ControlsChecksum(this.Controls);
        }

        public virtual bool Save(bool silent)
        {
            try
            {
                if (!ValidateControls(silent))
                {
                    return false;
                }

                SaveInternal(silent);
            }
            catch (ArgumentNullException ex)
            {
                if (!silent)
                {
                    MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                return false;
            }

            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            return true;
        }

        protected virtual void SaveInternal(bool silent)
        {
            Dictionary<string, string> collection = ControlUtils.GetAttributesCollection(this.Controls, true);
            SendSaveMessage(collection);
        }

        protected virtual ControlValidationResult ValidateControl(Control control)
        {
            return null;
        }

        protected virtual bool ValidateControls(bool silent)
        {
            return ValidateControlRecursive(this);
        }

        private bool ValidateControlRecursive(Control control)
        {
            var result = ValidateControl(control);

            errorProvider.SetError(control, result?.Level == ControlValidationLevel.Error ? result.Message : null);
            warningProvider.SetError(control, result?.Level == ControlValidationLevel.Warning ? result.Message : null);

            var valid = result == null || result.Level != ControlValidationLevel.Error;

            foreach (Control child in control.Controls)
                valid &= ValidateControlRecursive(child);

            return valid;
        }

        private void AttachValidatingEvent(Control control)
        {
            foreach (Control child in control.Controls)
            {
                child.Validating += Control_Validating;

                AttachValidatingEvent(child);
            }
        }

        private void Control_Validating(object sender, CancelEventArgs e)
        {
            if (sender is Control ctrl)
            {
                var result = ValidateControl(ctrl);

                errorProvider.SetError(ctrl, result?.Level == ControlValidationLevel.Error ? result.Message : null);
                warningProvider.SetError(ctrl, result?.Level == ControlValidationLevel.Warning ? result.Message : null);
            }
        }

        /// <summary>
        /// Sends a connection success message
        /// </summary>
        /// <param name="service">IOrganizationService generated</param>
        /// <param name="parameters">Lsit of parameter</param>
        private void SendSaveMessage(Dictionary<string, string> collection)
        {
            SaveEventArgs sea = new SaveEventArgs { AttributeCollection = collection };

            if (Saved != null)
            {
                Saved(this, sea);
            }
        }

        public event EventHandler<SaveEventArgs> Saved;

        protected void FillControl(Control control)
        {
            ControlUtils.FillControl(collec, control, this);
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            const int WM_KEYDOWN = 0x0100;
            const int VK_ESCAPE = 27;

            if (m.Msg == WM_KEYDOWN && (int)m.WParam == VK_ESCAPE)
            {
                collec = new Dictionary<string, string>(original);
                ControlUtils.FillControls(collec, this.Controls, this);
                controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
                SendSaveMessage(original);
                return true;
            }

            return base.ProcessKeyPreview(ref m);
        }
    }

    public enum ControlValidationLevel
    {
        Success,
        Warning,
        Error
    }

    public class ControlValidationResult
    {
        public ControlValidationResult(ControlValidationLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        public ControlValidationLevel Level { get; }
        public string Message { get; }
    }
}
