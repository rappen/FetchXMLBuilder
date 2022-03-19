using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XRM.Helpers.FetchXML;
using Rappen.XTB.Helpers.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class SimplerBuilder : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder fxb;
        private string _entity;

        public SimplerBuilder(FetchXmlBuilder fetchXmlBuilder)
        {
            fxb = fetchXmlBuilder;
            InitializeComponent();
            xrmTable.DataSource = fxb.GetDisplayEntities().Values;
            ParseXML();
        }

        internal void ParseXML()
        {
            var query = new FetchXML(fxb.dockControlBuilder.GetFetchDocument());
            xrmTable.SelectedIndex = -1;
            gbColumns.Controls.Clear();
            gbFilters.Controls.Clear();
            gbRelateds.Controls.Clear();
            SuspendLayout();
            if (fxb.dockControlBuilder?.tvFetch?.Nodes[0] == null)
            {
                return;
            }
            AddNodeToSimpler(fxb.dockControlBuilder.tvFetch.Nodes[0]);
            FixSizes();
            ResumeLayout();
        }

        private void AddNodeToSimpler(TreeNode node)
        {
            Panel pan = null;
            switch (node.Name)
            {
                case "fetch":
                    gbTable.Tag = node;
                    break;
                case "entity":
                    _entity = node.Value("name");
                    xrmTable.SetSelected(_entity);
                    xrmTable.Tag = node;
                    break;
                case "attribute":
                    pan = AddColumn(node);
                    break;
                case "filter":
                    pan = AddFilter(node);
                    break;
                case "condition":
                    pan = AddCondition(node);
                    break;
            }

            node.Nodes.OfType<TreeNode>().ToList().ForEach(n => AddNodeToSimpler(n));

            if (pan != null)
            {
                FixPanelSize(pan);
            }
        }

        private void FixSizes()
        {
            gbColumns.Height = 40 + gbColumns.Controls.OfType<Panel>().Where(c => c.Dock == DockStyle.Top || c.Dock == DockStyle.Bottom).Sum(c => c.Height);
            gbFilters.Height = 40 + gbFilters.Controls.OfType<Panel>().Where(c => c.Dock == DockStyle.Top || c.Dock == DockStyle.Bottom).Sum(c => c.Height);
            gbRelateds.Height = 40 + gbRelateds.Controls.OfType<Panel>().Where(c => c.Dock == DockStyle.Top || c.Dock == DockStyle.Bottom).Sum(c => c.Height);
        }

        private void FixPanelSize(Panel pan)
        {
            pan.Height = 4 + pan.Controls.OfType<Control>().Where(c => c.Dock == DockStyle.Top || c.Dock == DockStyle.Bottom).Sum(c => c.Height);
        }

        private static Panel AddPanel(TreeNode node, Control parent)
        {
            var pan = new Panel
            {
                Parent = parent,
                Height = 30,
                Dock = DockStyle.Top,
                Padding = new Padding(parent is GroupBox ? 10 : 0, 2, 0, 2),
                BorderStyle = BorderStyle.None,
                Tag = node
            };
            pan.BringToFront();
            return pan;
        }

        private Panel AddColumn(TreeNode node)
        {
            var pan = AddPanel(node, gbColumns);
            var entity = node.LocalEntityName();
            var attr = AddAttribute(pan, entity);
            attr.SetSelected(node.Value("name"));
            return pan;
        }

        private XRMAttributeComboBox AddAttribute(Panel pan, string entity)
        {
            return new XRMAttributeComboBox
            {
                Parent = pan,
                Dock = DockStyle.Top,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                DataSource = fxb.GetDisplayAttributes(entity)
            };
        }

        private Panel AddFilter(TreeNode node)
        {
            var parentcontrol = node.Parent.Name == "filter" ? GetNodeControl(gbFilters, node.Parent) : gbFilters;
            var pan = AddPanel(node, parentcontrol);
            pan.BorderStyle = BorderStyle.FixedSingle;
            pan.Height = 50;
            var filt = new LinkLabel
            {
                Parent = pan,
                AutoSize = false,
                Dock = DockStyle.Left,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Width = 50,
                Text = node.Value("type")
            };
            if (string.IsNullOrWhiteSpace(filt.Text))
            {
                filt.Text = "and";
            }
            filt.LinkClicked += Filt_LinkClicked;
            return pan;
        }

        private Panel AddCondition(TreeNode node)
        {
            var filter = GetNodeControl(gbFilters, node.Parent);
            var pan = AddPanel(node, filter);
            var entity = node.LocalEntityName();
            var attr = AddAttribute(pan, entity);
            attr.SetSelected(node.Value("attribute"));
            var opers = new ComboBox
            {
                Parent = pan,
                Dock = DockStyle.Right,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
            };
            foreach (var oper in Enum.GetValues(typeof(ConditionOperator)))
            {
                opers.Items.Add(new OperatorItem((ConditionOperator)oper));
            }
            var operation = node.Value("operation");

            return pan;
        }

        private void Filt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is LinkLabel filt)
            {
                filt.Text = filt.Text == "and" ? "or" : "and";
            }
        }

        private static Control GetNodeControl(Control root, TreeNode node)
        {
            return GetNodeControl(root.Controls.OfType<Control>(), node) ?? root;
        }

        private static Control GetNodeControl(IEnumerable<Control> children, TreeNode node)
        {
            var result = children.FirstOrDefault(c => c.Tag == node);
            if (result != null)
            {
                return result;
            }
            foreach (var child in children)
            {
                result = GetNodeControl(child.Controls.OfType<Control>(), node);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        internal void EnableControls(bool enabled)
        {

        }
    }
}
