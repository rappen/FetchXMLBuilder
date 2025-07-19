using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    partial class TreeBuilderControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeBuilderControl));
            this.splitQueryBuilder = new System.Windows.Forms.SplitContainer();
            this.tvFetch = new System.Windows.Forms.TreeView();
            this.imgFetch = new System.Windows.Forms.ImageList(this.components);
            this.panProperties = new System.Windows.Forms.Panel();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.panQuickActions = new System.Windows.Forms.Panel();
            this.gbQuickActions = new System.Windows.Forms.GroupBox();
            this.lblQAExpander = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panTreeSplitter = new System.Windows.Forms.Panel();
            this.addMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.nothingToAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addOneMoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorBeginOfEdition = new System.Windows.Forms.ToolStripSeparator();
            this.commentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.lblWarning = new System.Windows.Forms.LinkLabel();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryBuilder)).BeginInit();
            this.splitQueryBuilder.Panel1.SuspendLayout();
            this.splitQueryBuilder.Panel2.SuspendLayout();
            this.splitQueryBuilder.SuspendLayout();
            this.panProperties.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.panQuickActions.SuspendLayout();
            this.gbQuickActions.SuspendLayout();
            this.addMenu.SuspendLayout();
            this.nodeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitQueryBuilder
            // 
            this.splitQueryBuilder.BackColor = System.Drawing.SystemColors.Window;
            this.splitQueryBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitQueryBuilder.Location = new System.Drawing.Point(0, 0);
            this.splitQueryBuilder.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.splitQueryBuilder.Name = "splitQueryBuilder";
            this.splitQueryBuilder.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitQueryBuilder.Panel1
            // 
            this.splitQueryBuilder.Panel1.Controls.Add(this.tvFetch);
            this.splitQueryBuilder.Panel1.Controls.Add(this.lblWarning);
            // 
            // splitQueryBuilder.Panel2
            // 
            this.splitQueryBuilder.Panel2.Controls.Add(this.panProperties);
            this.splitQueryBuilder.Panel2.Controls.Add(this.panQuickActions);
            this.splitQueryBuilder.Panel2.Controls.Add(this.panTreeSplitter);
            this.splitQueryBuilder.Size = new System.Drawing.Size(594, 668);
            this.splitQueryBuilder.SplitterDistance = 378;
            this.splitQueryBuilder.SplitterWidth = 8;
            this.splitQueryBuilder.TabIndex = 0;
            // 
            // tvFetch
            // 
            this.tvFetch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvFetch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFetch.HideSelection = false;
            this.tvFetch.ImageIndex = 0;
            this.tvFetch.ImageList = this.imgFetch;
            this.tvFetch.Location = new System.Drawing.Point(0, 0);
            this.tvFetch.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tvFetch.Name = "tvFetch";
            this.tvFetch.SelectedImageIndex = 0;
            this.tvFetch.ShowNodeToolTips = true;
            this.tvFetch.Size = new System.Drawing.Size(594, 350);
            this.tvFetch.TabIndex = 0;
            this.tvFetch.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFetch_AfterSelect);
            this.tvFetch.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvFetch_NodeMouseClick);
            this.tvFetch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvFetch_KeyDown);
            // 
            // imgFetch
            // 
            this.imgFetch.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgFetch.ImageStream")));
            this.imgFetch.TransparentColor = System.Drawing.Color.Transparent;
            this.imgFetch.Images.SetKeyName(0, "blank");
            this.imgFetch.Images.SetKeyName(1, "fetch");
            this.imgFetch.Images.SetKeyName(2, "entity");
            this.imgFetch.Images.SetKeyName(3, "attribute");
            this.imgFetch.Images.SetKeyName(4, "all-attributes");
            this.imgFetch.Images.SetKeyName(5, "link-entity");
            this.imgFetch.Images.SetKeyName(6, "filter");
            this.imgFetch.Images.SetKeyName(7, "condition");
            this.imgFetch.Images.SetKeyName(8, "value");
            this.imgFetch.Images.SetKeyName(9, "order");
            this.imgFetch.Images.SetKeyName(10, "#comment");
            this.imgFetch.Images.SetKeyName(11, "info");
            this.imgFetch.Images.SetKeyName(12, "warning");
            this.imgFetch.Images.SetKeyName(13, "error");
            // 
            // panProperties
            // 
            this.panProperties.Controls.Add(this.gbProperties);
            this.panProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panProperties.Location = new System.Drawing.Point(0, 56);
            this.panProperties.Name = "panProperties";
            this.panProperties.Size = new System.Drawing.Size(594, 226);
            this.panProperties.TabIndex = 36;
            // 
            // gbProperties
            // 
            this.gbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProperties.Controls.Add(this.panelContainer);
            this.gbProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbProperties.Location = new System.Drawing.Point(-1, 8);
            this.gbProperties.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.gbProperties.Size = new System.Drawing.Size(596, 220);
            this.gbProperties.TabIndex = 34;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Node Properties";
            // 
            // panelContainer
            // 
            this.panelContainer.AutoScroll = true;
            this.panelContainer.BackColor = System.Drawing.SystemColors.Window;
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(2, 16);
            this.panelContainer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(592, 201);
            this.panelContainer.TabIndex = 14;
            // 
            // panQuickActions
            // 
            this.panQuickActions.Controls.Add(this.gbQuickActions);
            this.panQuickActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panQuickActions.Location = new System.Drawing.Point(0, 1);
            this.panQuickActions.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panQuickActions.Name = "panQuickActions";
            this.panQuickActions.Padding = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this.panQuickActions.Size = new System.Drawing.Size(594, 55);
            this.panQuickActions.TabIndex = 17;
            // 
            // gbQuickActions
            // 
            this.gbQuickActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbQuickActions.Controls.Add(this.lblQAExpander);
            this.gbQuickActions.Controls.Add(this.linkLabel1);
            this.gbQuickActions.Location = new System.Drawing.Point(-1, 8);
            this.gbQuickActions.Name = "gbQuickActions";
            this.gbQuickActions.Padding = new System.Windows.Forms.Padding(8, 6, 3, 3);
            this.gbQuickActions.Size = new System.Drawing.Size(596, 49);
            this.gbQuickActions.TabIndex = 19;
            this.gbQuickActions.TabStop = false;
            this.gbQuickActions.Text = "Quick Actions";
            // 
            // lblQAExpander
            // 
            this.lblQAExpander.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQAExpander.AutoSize = true;
            this.lblQAExpander.Location = new System.Drawing.Point(576, 0);
            this.lblQAExpander.Name = "lblQAExpander";
            this.lblQAExpander.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblQAExpander.Size = new System.Drawing.Size(14, 13);
            this.lblQAExpander.TabIndex = 9;
            this.lblQAExpander.Text = "–";
            this.lblQAExpander.Click += new System.EventHandler(this.lblQAExpander_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.linkLabel1.Location = new System.Drawing.Point(8, 19);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(55, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "linkLabel1";
            // 
            // panTreeSplitter
            // 
            this.panTreeSplitter.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.panTreeSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panTreeSplitter.Location = new System.Drawing.Point(0, 0);
            this.panTreeSplitter.Name = "panTreeSplitter";
            this.panTreeSplitter.Size = new System.Drawing.Size(594, 1);
            this.panTreeSplitter.TabIndex = 35;
            // 
            // addMenu
            // 
            this.addMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nothingToAddToolStripMenuItem});
            this.addMenu.Name = "addMenu";
            this.addMenu.OwnerItem = this.addToolStripMenuItem;
            this.addMenu.Size = new System.Drawing.Size(154, 26);
            this.addMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.nodeMenu_ItemClicked);
            // 
            // nothingToAddToolStripMenuItem
            // 
            this.nothingToAddToolStripMenuItem.Name = "nothingToAddToolStripMenuItem";
            this.nothingToAddToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.nothingToAddToolStripMenuItem.Text = "nothing to add";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDown = this.addMenu;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.ShortcutKeyDisplayString = "Ins";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.addToolStripMenuItem.Tag = "Add";
            this.addToolStripMenuItem.Text = "Add";
            // 
            // nodeMenu
            // 
            this.nodeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAttributesToolStripMenuItem,
            this.addOneMoreToolStripMenuItem,
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparatorBeginOfEdition,
            this.commentToolStripMenuItem,
            this.uncommentToolStripMenuItem,
            this.toolStripMenuItem3,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.duplicateToolStripMenuItem,
            this.toolStripMenuItem1,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripSeparator1,
            this.showMetadataToolStripMenuItem});
            this.nodeMenu.Name = "nodeMenu";
            this.nodeMenu.Size = new System.Drawing.Size(203, 314);
            this.nodeMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.nodeMenu_ItemClicked);
            // 
            // selectAttributesToolStripMenuItem
            // 
            this.selectAttributesToolStripMenuItem.Name = "selectAttributesToolStripMenuItem";
            this.selectAttributesToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.selectAttributesToolStripMenuItem.Tag = "SelectAttributes";
            this.selectAttributesToolStripMenuItem.Text = "Select attributes...";
            // 
            // addOneMoreToolStripMenuItem
            // 
            this.addOneMoreToolStripMenuItem.Name = "addOneMoreToolStripMenuItem";
            this.addOneMoreToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.addOneMoreToolStripMenuItem.Text = "Add one more";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeyDisplayString = "Del";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // toolStripSeparatorBeginOfEdition
            // 
            this.toolStripSeparatorBeginOfEdition.Name = "toolStripSeparatorBeginOfEdition";
            this.toolStripSeparatorBeginOfEdition.Size = new System.Drawing.Size(199, 6);
            // 
            // commentToolStripMenuItem
            // 
            this.commentToolStripMenuItem.Name = "commentToolStripMenuItem";
            this.commentToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+K";
            this.commentToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.commentToolStripMenuItem.Tag = "Comment";
            this.commentToolStripMenuItem.Text = "Comment";
            // 
            // uncommentToolStripMenuItem
            // 
            this.uncommentToolStripMenuItem.Name = "uncommentToolStripMenuItem";
            this.uncommentToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+U";
            this.uncommentToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.uncommentToolStripMenuItem.Tag = "Uncomment";
            this.uncommentToolStripMenuItem.Text = "Uncomment";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(199, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(199, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Up";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.moveUpToolStripMenuItem.Text = "Move up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonMoveUp_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Down";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.moveDownToolStripMenuItem.Text = "Move down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonMoveDown_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(199, 6);
            // 
            // lblWarning
            // 
            this.lblWarning.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblWarning.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblWarning.ImageIndex = 11;
            this.lblWarning.ImageList = this.imgFetch;
            this.lblWarning.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.lblWarning.Location = new System.Drawing.Point(0, 350);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.lblWarning.Size = new System.Drawing.Size(594, 28);
            this.lblWarning.TabIndex = 1;
            this.lblWarning.Text = "      Warning";
            this.lblWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblWarning.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblWarning_LinkClicked);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_cut_16;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.cutToolStripMenuItem.Tag = "Cut";
            this.cutToolStripMenuItem.Text = "Cut";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_copy2_16;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.copyToolStripMenuItem.Tag = "Copy";
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_paste2_16;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.pasteToolStripMenuItem.Tag = "Paste";
            this.pasteToolStripMenuItem.Text = "Paste";
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_duplicate_16;
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.V)));
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.duplicateToolStripMenuItem.Tag = "Duplicate";
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            // 
            // showMetadataToolStripMenuItem
            // 
            this.showMetadataToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showMetadataToolStripMenuItem.Image")));
            this.showMetadataToolStripMenuItem.Name = "showMetadataToolStripMenuItem";
            this.showMetadataToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.showMetadataToolStripMenuItem.Text = "Show Metadata";
            this.showMetadataToolStripMenuItem.Click += new System.EventHandler(this.showMetadataToolStripMenuItem_Click);
            // 
            // TreeBuilderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(594, 668);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.splitQueryBuilder);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "TreeBuilderControl";
            this.TabText = "Query Builder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TreeBuilderControl_FormClosing);
            this.Load += new System.EventHandler(this.TreeBuilderControl_Load);
            this.Enter += new System.EventHandler(this.TreeBuilderControl_Enter);
            this.splitQueryBuilder.Panel1.ResumeLayout(false);
            this.splitQueryBuilder.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitQueryBuilder)).EndInit();
            this.splitQueryBuilder.ResumeLayout(false);
            this.panProperties.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.panQuickActions.ResumeLayout(false);
            this.gbQuickActions.ResumeLayout(false);
            this.gbQuickActions.PerformLayout();
            this.addMenu.ResumeLayout(false);
            this.nodeMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitQueryBuilder;
        internal System.Windows.Forms.TreeView tvFetch;
        internal System.Windows.Forms.GroupBox gbProperties;
        internal System.Windows.Forms.Panel panelContainer;
        internal System.Windows.Forms.ContextMenuStrip addMenu;
        private System.Windows.Forms.ToolStripMenuItem nothingToAddToolStripMenuItem;
        internal System.Windows.Forms.ContextMenuStrip nodeMenu;
        internal System.Windows.Forms.ToolStripMenuItem selectAttributesToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparatorBeginOfEdition;
        internal System.Windows.Forms.ToolStripMenuItem commentToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem uncommentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.Panel panQuickActions;
        private System.Windows.Forms.LinkLabel linkLabel1;
        internal System.Windows.Forms.GroupBox gbQuickActions;
        private System.Windows.Forms.Panel panTreeSplitter;
        private System.Windows.Forms.Panel panProperties;
        internal System.Windows.Forms.Label lblQAExpander;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showMetadataToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem addOneMoreToolStripMenuItem;
        private System.Windows.Forms.ImageList imgFetch;
        private System.Windows.Forms.LinkLabel lblWarning;
        private ToolStripSeparator toolStripMenuItem3;
        internal ToolStripMenuItem copyToolStripMenuItem;
        internal ToolStripMenuItem pasteToolStripMenuItem;
        internal ToolStripMenuItem duplicateToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
    }
}
