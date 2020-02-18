using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public class FetchXmlElementControlBase : UserControl, IDefinitionSavable
    {
        private Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        public void InitializeFXB(Dictionary<string, string> collection, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree, TreeNode node)
        {
            fxb = fetchXmlBuilder;
            Node = node;
            Tree = tree;
            if (collection != null)
                collec = collection;
            else if (node != null)
                collec = (Dictionary<string, string>)node.Tag;

            PopulateControls();
            ControlUtils.FillControls(collec, this.Controls, this);
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            Saved += tree.CtrlSaved;
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
                e.Cancel = !Save(false);
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
                    return false;

                SaveInternal(silent);
            }
            catch (ArgumentNullException ex)
            {
                if (!silent)
                    MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Stop);

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

        protected virtual bool ValidateControls(bool silent)
        {
            return true;
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
    }
}
