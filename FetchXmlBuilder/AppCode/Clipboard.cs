using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    /// <summary>
    /// Clipboard to manage Treeview nodes action
    /// </summary>
    internal class Clipboard
    {
        #region Variables

        /// <summary>
        /// TreeNode in memory
        /// </summary>
        private TreeNode _tempTreeNode;

        /// <summary>
        /// Original parent of treenode in memory
        /// </summary>
        private TreeNode _originalParentNode;

        /// <summary>
        /// Original index of treenode in memory
        /// </summary>
        private int _originalIndex;

        private bool _isCutAction;

        private bool _hasBeenPasted;

        #endregion Variables

        #region Methods

        /// <summary>
        /// Checks if the in-memory TreeNode can be pasted to the selected TreeNode
        /// </summary>
        /// <param name="targetNodeName">TreeNode currently selected</param>
        /// <returns>Flag that indicates if the in-memory TreeNode can be pasted</returns>
        internal bool IsValidForPaste(TreeNode node)
        {
            if (_tempTreeNode != null)
            {
                var targetNodeName = node.Text.Split(' ')[0];
                var source = _tempTreeNode.Text;
                switch (targetNodeName)
                {
                    case "Blocks":
                        return source.StartsWith("DataBlock") || source.StartsWith("SolutionBlock");
                    case "DataBlock":
                        return (source.StartsWith("Export") && !node.Nodes.ContainsKey("Export")) ||
                               (source.StartsWith("Import") && !node.Nodes.ContainsKey("Import")) ||
                               (source.StartsWith("Relation") && !node.Nodes.ContainsKey("Relation"));
                    case "SolutionBlock":
                        return (source.StartsWith("Export") && !node.Nodes.ContainsKey("Export")) ||
                               (source.StartsWith("Import") && !node.Nodes.ContainsKey("Import"));
                    case "Export":
                        return (node.Parent != null && node.Parent.Text.StartsWith("DataBlock") &&
                                 ((source.StartsWith("Attributes")) && !node.Nodes.ContainsKey("Attributes")) ||
                                 source.StartsWith("Filter") ||
                                 source.StartsWith("Sort")) ||
                               (node.Parent != null && node.Parent.Text.StartsWith("SolutionBlock") && source.StartsWith("Settings") && !node.Nodes.ContainsKey("Settings"));
                    case "Attributes":
                        return source.StartsWith("Attribute");
                    case "Import":
                        return (node.Parent != null && node.Parent.Text.StartsWith("DataBlock") && source.StartsWith("Match")) && !node.Nodes.ContainsKey("Match") ||
                               (node.Parent != null && node.Parent.Text.StartsWith("SolutionBlock") && source.StartsWith("PreRequisites") && !node.Nodes.ContainsKey("PreRequisites"));
                    case "Match":
                        return source.StartsWith("Attribute");
                    case "PreRequisites":
                        return source.StartsWith("Solution");
                    default:
                        return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Cuts the selected TreeNode
        /// </summary>
        /// <param name="node">Selected TreeNode</param>
        internal void Cut(TreeNode node)
        {
            Copy(node, true);

            node.Remove();
        }

        /// <summary>
        /// Copies the selected TreeNode
        /// </summary>
        /// <param name="node">selected TreeNode</param>
        /// <param name="isCut">Flag that indicates if the copy action is a cut action</param>
        internal void Copy(TreeNode node, bool isCut = false)
        {
            // If there was already a TreeNode in memory which is different
            // from the currently selected one, we replace the current in-memory
            // TreeNode to its original location
            if (!_hasBeenPasted && _tempTreeNode != null && _tempTreeNode != node && _isCutAction)
            {
                _originalParentNode.Nodes.Insert(_originalIndex, _tempTreeNode);
            }

            // Saves the current selected TreeNode information
            _tempTreeNode = node;
            _originalParentNode = node.Parent;
            _originalIndex = node.Index;

            _isCutAction = isCut;
            _hasBeenPasted = false;
        }

        /// <summary>
        /// Pastes the in-memory TreeNode under the target TreeNode
        /// </summary>
        /// <param name="targetNode">Target TreeNode</param>
        internal void Paste(TreeNode targetNode)
        {
            if (_tempTreeNode == null)
                return;

            if (IsValidForPaste(targetNode))
            {
                // The in-memory TreeNode is cloned to avoid having two occurences
                // of the same TreeNode in the TreeView
                var clonedNode = (TreeNode)_tempTreeNode.Clone();

                // If the target TreeNode already contains the in-memory TreeNode or 
                // another TreeNode with the same Text, we need to change the name
                if (targetNode.Nodes.ContainsKey(_tempTreeNode.Text.Replace(" ", "")))
                {
                    clonedNode.Text = clonedNode.Text.Replace("(", "(Copy of ");

                    // Clone content
                    UpdateContentForCloning(clonedNode);

                    // Update ids
                    var ticks = DateTime.Now.Ticks;
                    UpdateIds(clonedNode, ref ticks);
                }

                targetNode.Nodes.Add(clonedNode);
                targetNode.TreeView.SelectedNode = clonedNode;

                // Clean the in-memory TreeNode
                // tempTreeNode = null;
                _hasBeenPasted = true;
            }
            else
            {
                MessageBox.Show("You can't paste this item under the selected node!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Updates all Ids for the current node tag
        /// </summary>
        /// <param name="node">Current treenode</param>
        /// <param name="idPart"> </param>
        private void UpdateIds(TreeNode node, ref long idPart)
        {
            var attributes = (Dictionary<string, string>)node.Tag;
            if (attributes.ContainsKey("Id"))
                attributes["Id"] = string.Format("{0}_{1}",
                                                 attributes["Id"],
                                                 idPart);

            node.Tag = attributes;

            idPart++;

            foreach (TreeNode childNode in node.Nodes)
            {
                UpdateIds(childNode, ref idPart);
            }
        }

        private void UpdateContentForCloning(TreeNode node)
        {
            var attributes = (Dictionary<string, string>)node.Tag;
            var clonedAttributes = attributes.Keys.ToDictionary(key => key, key => attributes[key]);
            clonedAttributes.Remove("ResourceId");
            clonedAttributes.Remove("DescriptionResourceId");
            node.Tag = clonedAttributes;

            foreach (TreeNode childNode in node.Nodes)
            {
                UpdateContentForCloning(childNode);
            }
        }

        #endregion Methods
    }
}
