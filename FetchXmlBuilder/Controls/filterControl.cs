using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class filterControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion Delegates

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion Event Handlers

        public filterControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public filterControl(Dictionary<string, string> collection, TreeBuilderControl tree)
            : this()
        {
            if (collection != null)
                collec = collection;

            ControlUtils.FillControls(collec, this.Controls);
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            Saved += tree.CtrlSaved;
        }

        public void Save()
        {
            try
            {
                Dictionary<string, string> collection = ControlUtils.GetAttributesCollection(this.Controls, true);
                SendSaveMessage(collection);
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
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

        private void Control_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlUtils.ControlsChecksum(this.Controls))
            {
                Save();
            }
        }
    }
}