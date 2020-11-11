using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public class FetchXmlElementControlBase : UserControl, IDefinitionSavable, ISupportInitializeNotification
    {
        private Dictionary<string, string> collec;
        private Dictionary<string, string> original;
        private string controlsCheckSum = "";
        private ErrorProvider errorProvider;
        private ErrorProvider warningProvider;
        private ErrorProvider infoProvider;
        private bool validationSuspended = false;
        private int initCount;

        static FetchXmlElementControlBase()
        {
            // Create the small warning icon to use for user feedback
            // https://stackoverflow.com/questions/3031124/is-there-a-way-to-get-different-sizes-of-the-windows-system-icons-in-net
            var iconSize = SystemInformation.SmallIconSize;
            var bitmapWarning = new Bitmap(iconSize.Width, iconSize.Height);
            using (var gw = Graphics.FromImage(bitmapWarning))
            {
                gw.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gw.DrawImage(SystemIcons.Warning.ToBitmap(), new Rectangle(Point.Empty, iconSize));
            }
            WarningIcon = Icon.FromHandle(bitmapWarning.GetHicon());
            var bitmapInfo = new Bitmap(iconSize.Width, iconSize.Height);
            using (var gi = Graphics.FromImage(bitmapInfo))
            {
                gi.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gi.DrawImage(SystemIcons.Information.ToBitmap(), new Rectangle(Point.Empty, iconSize));
            }
            InfoIcon = Icon.FromHandle(bitmapInfo.GetHicon());
        }

        protected static Icon WarningIcon { get; }
        protected static Icon InfoIcon { get; }

        public void InitializeFXB(Dictionary<string, string> collection, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree, TreeNode node)
        {
            BeginInit();

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
            errorProvider = new ErrorProvider(this)
            {
                BlinkStyle = ErrorBlinkStyle.NeverBlink
            };
            warningProvider = new ErrorProvider(this)
            {
                BlinkStyle = ErrorBlinkStyle.NeverBlink,
                Icon = WarningIcon
            };
            infoProvider = new ErrorProvider(this)
            {
                BlinkStyle = ErrorBlinkStyle.NeverBlink,
                Icon = InfoIcon
            };
            ShowHelpIcon(this, fxb.settings.ShowHelpLinks);
            PopulateControls();
            ControlUtils.FillControls(collec, Controls, this);
            controlsCheckSum = ControlUtils.ControlsChecksum(Controls);
            Saved += tree.CtrlSaved;
            AttachValidatingEvent(this);
            ValidateControlRecursive(this);

            EndInit();
        }

        protected FetchXmlBuilder fxb { get; set; }

        protected TreeNode Node { get; set; }

        protected TreeBuilderControl Tree { get; set; }

        protected bool ValidationSuspended
        {
            get => validationSuspended;
            set
            {
                var revalidate = validationSuspended && !value;
                validationSuspended = value;
                if (revalidate)
                {
                    ValidateControlRecursive(this);
                }
            }
        }

        private void ShowHelpIcon(Control control, bool show)
        {
            control.Controls.OfType<Control>().ToList().ForEach(c =>
            {
                if (c.Tag is string tag && tag.StartsWith("http"))
                {
                    c.Visible = show;
                }
                ShowHelpIcon(c, show);
            });
        }

        protected virtual void PopulateControls() { }

        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);

            if (RequiresSave())
            {
                Save(false);
            }
        }

        protected virtual bool RequiresSave()
        {
            return !controlsCheckSum.Equals(ControlUtils.ControlsChecksum(this.Controls));
        }

        public virtual bool Save(bool keyPress)
        {
            try
            {
                if (!ValidateControlRecursive(this))
                {
                    return false;
                }

                SaveInternal(keyPress);
            }
            catch (ArgumentNullException ex)
            {
                if (!keyPress)
                {
                    MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                return false;
            }

            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            return true;
        }

        protected virtual void SaveInternal(bool keyPress)
        {
            if (IsInitialized)
            {
                SendSaveMessage(GetAttributesCollection(), keyPress);
            }
        }

        protected virtual Dictionary<string, string> GetAttributesCollection()
        {
            return ControlUtils.GetAttributesCollection(this.Controls, true);
        }

        protected virtual ControlValidationResult ValidateControl(Control control)
        {
            return null;
        }

        private bool ValidateControlRecursive(Control control)
        {
            var result = ValidateControlAndShowIcons(control);
            var valid = result?.Level != ControlValidationLevel.Error;
            control.Controls.OfType<Control>().ToList().ForEach(c => valid &= ValidateControlRecursive(c));
            return valid;
        }

        private ControlValidationResult ValidateControlAndShowIcons(Control control)
        {
            if (validationSuspended)
            {
                return null;
            }
            var result = ValidateControl(control);
            errorProvider.SetError(control, result?.Level == ControlValidationLevel.Error ? result.Message : null);
            warningProvider.SetError(control, result?.Level == ControlValidationLevel.Warning ? result.Message : null);
            infoProvider.SetError(control, result?.Level == ControlValidationLevel.Info ? result.Message : null);
            return result;
        }

        private void AttachValidatingEvent(Control control)
        {
            control.Controls.OfType<Control>().ToList().ForEach(c =>
            {
                c.Validating += Control_Validating;
                AttachValidatingEvent(c);
            });
        }

        private void Control_Validating(object sender, CancelEventArgs e)
        {
            if (sender is Control ctrl)
            {
                ValidateControlAndShowIcons(ctrl);
            }
        }

        /// <summary>
        /// Sends a connection success message
        /// </summary>
        /// <param name="service">IOrganizationService generated</param>
        /// <param name="parameters">Lsit of parameter</param>
        private void SendSaveMessage(Dictionary<string, string> collection, bool keyPress)
        {
            Saved?.Invoke(this, new SaveEventArgs { AttributeCollection = collection, KeyPress = keyPress });
        }

        public event EventHandler<SaveEventArgs> Saved;

        protected void ReFillControl(Control control)
        {
            BeginInit();

            ControlUtils.FillControl(collec, control, null);

            EndInit();
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            const int WM_KEYDOWN = 0x0100;
            const int VK_ESCAPE = 27;

            if (m.Msg == WM_KEYDOWN && (int)m.WParam == VK_ESCAPE)
            {
                collec = new Dictionary<string, string>(original);
                ControlUtils.FillControls(collec, Controls, this);
                controlsCheckSum = ControlUtils.ControlsChecksum(Controls);
                SendSaveMessage(original, false);
                return true;
            }

            return base.ProcessKeyPreview(ref m);
        }

        public void BeginInit()
        {
            initCount++;
        }

        public void EndInit()
        {
            initCount--;

            if (initCount == 0)
                Initialized?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Initialized;

        public bool IsInitialized => initCount == 0;
    }

    public enum ControlValidationLevel
    {
        Success,
        Info,
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
