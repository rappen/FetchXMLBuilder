using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XTB.FetchXmlBuilder.Builder;
using Rappen.XTB.FetchXmlBuilder.ControlsClasses;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    public partial class valueControl : FetchXmlElementControlBase
    {
        private string _entityName;
        private string _attributeName;

        public valueControl() : this(null, null, null)
        {
        }

        public valueControl(TreeNode node, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(null, fetchXmlBuilder, tree, node);

            _attributeName = Node.Parent.Value("attribute");
            _entityName = Node.Parent.Value("entity");

            if (String.IsNullOrWhiteSpace(_entityName))
            {
                _entityName = Node.Parent.Parent.Parent.Value("name");
            }
            else
            {
                // TODO: Entity is an alias, get the actual entity name
            }

            if (fxb.NeedToLoadEntity(_entityName))
            {
                if (!fxb.working)
                {
                    fxb.LoadEntityDetails(_entityName, null, false);
                }
            }
            RefreshValues();
        }

        private void RefreshValues()
        {
            cmbValue.Items.Clear();
            cmbValue.DropDownStyle = ComboBoxStyle.Simple;
            cmbValue.AutoCompleteMode = AutoCompleteMode.None;

            var entities = fxb.GetDisplayEntities();
            if (entities?.FirstOrDefault(e => e.LogicalName.Equals(_entityName)) is EntityMetadata entity)
            {
                var attribute = entity.Attributes.SingleOrDefault(a => a.LogicalName == _attributeName);

                if (attribute != null)
                {
                    // Show correct editor based on type of attribute
                    if (attribute is EnumAttributeMetadata enummeta &&
                         enummeta.OptionSet is OptionSetMetadata options &&
                         !(attribute is EntityNameAttributeMetadata))
                    {
                        cmbValue.Items.AddRange(options.Options.Select(o => new OptionsetItem(o)).ToArray());

                        if (cmbValue.Items.Count > 0 && cmbValue.SelectedIndex == -1 && !string.IsNullOrWhiteSpace(cmbValue.Text))
                        {
                            var item = cmbValue.Items.OfType<OptionsetItem>().FirstOrDefault(i => i.GetValue() == cmbValue.Text);
                            cmbValue.SelectedItem = item;
                        }

                        cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbValue.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    }
                    else if (attribute is BooleanAttributeMetadata boolmeta)
                    {
                        cmbValue.Items.Add(new OptionsetItem(boolmeta.OptionSet.FalseOption));
                        cmbValue.Items.Add(new OptionsetItem(boolmeta.OptionSet.TrueOption));
                        var value = cmbValue.Text;
                        cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbValue.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        cmbValue.SelectedItem = cmbValue.Items.OfType<OptionsetItem>().FirstOrDefault(i => i.GetValue() == value);
                    }
                    else if (attribute is LookupAttributeMetadata lookupmeta)
                    {
                        // TODO: Show lookup control
                    }
                }
            }
        }

        public override void Focus()
        {
            cmbValue.Focus();
        }
    }
}