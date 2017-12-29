using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class entityControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";
        private FetchXmlBuilder fxb;

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public entityControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public entityControl(Dictionary<string, string> collection, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
            : this()
        {
            fxb = fetchXmlBuilder;
            if (collection != null)
                collec = collection;

            PopulateControls();
            ControlUtils.FillControls(collec, this.Controls);
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            Saved += tree.CtrlSaved;
        }

        private void PopulateControls()
        {
            cmbEntity.Items.Clear();
            var entities = fxb.GetDisplayEntities();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    cmbEntity.Items.Add(new EntityItem(entity.Value));
                }
            }
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

        private void entityControl_Paint(object sender, PaintEventArgs e)
        {
            foreach (var cmb in Controls.Cast<Control>().Where(c => c is ComboBox).Select(c => (ComboBox)c).Where(c => c.DropDownStyle == ComboBoxStyle.DropDown))
            {
                cmb.SelectionLength = 0;
            }
        }
    }
}
