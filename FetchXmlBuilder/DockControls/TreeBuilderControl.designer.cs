namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbFetchTree = new System.Windows.Forms.GroupBox();
            this.tvFetch = new System.Windows.Forms.TreeView();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.panelButtonSpacer = new System.Windows.Forms.Panel();
            this.gbQuickActions = new System.Windows.Forms.GroupBox();
            this.menuControl = new System.Windows.Forms.MenuStrip();
            this.addMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.nothingToAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorBeginOfEdition = new System.Windows.Forms.ToolStripSeparator();
            this.commentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbFetchTree.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.gbQuickActions.SuspendLayout();
            this.addMenu.SuspendLayout();
            this.nodeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbFetchTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbProperties);
            this.splitContainer1.Panel2.Controls.Add(this.panelButtonSpacer);
            this.splitContainer1.Panel2.Controls.Add(this.gbQuickActions);
            this.splitContainer1.Size = new System.Drawing.Size(943, 438);
            this.splitContainer1.SplitterDistance = 414;
            this.splitContainer1.TabIndex = 0;
            // 
            // gbFetchTree
            // 
            this.gbFetchTree.Controls.Add(this.tvFetch);
            this.gbFetchTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbFetchTree.Location = new System.Drawing.Point(0, 0);
            this.gbFetchTree.Name = "gbFetchTree";
            this.gbFetchTree.Size = new System.Drawing.Size(414, 438);
            this.gbFetchTree.TabIndex = 25;
            this.gbFetchTree.TabStop = false;
            this.gbFetchTree.Text = "FetchXML outline";
            // 
            // tvFetch
            // 
            this.tvFetch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFetch.HideSelection = false;
            this.tvFetch.Location = new System.Drawing.Point(3, 16);
            this.tvFetch.Name = "tvFetch";
            this.tvFetch.ShowNodeToolTips = true;
            this.tvFetch.Size = new System.Drawing.Size(408, 419);
            this.tvFetch.TabIndex = 0;
            this.tvFetch.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFetch_AfterSelect);
            this.tvFetch.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvFetch_NodeMouseClick);
            this.tvFetch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvFetch_KeyDown);
            // 
            // gbProperties
            // 
            this.gbProperties.Controls.Add(this.panelContainer);
            this.gbProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbProperties.Location = new System.Drawing.Point(0, 71);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(525, 367);
            this.gbProperties.TabIndex = 34;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "FetchXML node attributes";
            // 
            // panelContainer
            // 
            this.panelContainer.BackColor = System.Drawing.SystemColors.Control;
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(3, 16);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(519, 348);
            this.panelContainer.TabIndex = 14;
            // 
            // panelButtonSpacer
            // 
            this.panelButtonSpacer.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtonSpacer.Location = new System.Drawing.Point(0, 50);
            this.panelButtonSpacer.Name = "panelButtonSpacer";
            this.panelButtonSpacer.Size = new System.Drawing.Size(525, 21);
            this.panelButtonSpacer.TabIndex = 33;
            // 
            // gbQuickActions
            // 
            this.gbQuickActions.Controls.Add(this.menuControl);
            this.gbQuickActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbQuickActions.Location = new System.Drawing.Point(0, 0);
            this.gbQuickActions.Name = "gbQuickActions";
            this.gbQuickActions.Size = new System.Drawing.Size(525, 50);
            this.gbQuickActions.TabIndex = 32;
            this.gbQuickActions.TabStop = false;
            this.gbQuickActions.Text = "Quick Actions";
            // 
            // menuControl
            // 
            this.menuControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuControl.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuControl.Location = new System.Drawing.Point(3, 16);
            this.menuControl.Name = "menuControl";
            this.menuControl.Size = new System.Drawing.Size(519, 31);
            this.menuControl.TabIndex = 16;
            this.menuControl.Text = "menuStrip1";
            this.menuControl.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.nodeMenu_ItemClicked);
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
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparatorBeginOfEdition,
            this.commentToolStripMenuItem,
            this.uncommentToolStripMenuItem,
            this.toolStripMenuItem1,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.nodeMenu.Name = "nodeMenu";
            this.nodeMenu.Size = new System.Drawing.Size(203, 170);
            this.nodeMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.nodeMenu_ItemClicked);
            // 
            // selectAttributesToolStripMenuItem
            // 
            this.selectAttributesToolStripMenuItem.Name = "selectAttributesToolStripMenuItem";
            this.selectAttributesToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.selectAttributesToolStripMenuItem.Tag = "SelectAttributes";
            this.selectAttributesToolStripMenuItem.Text = "Select attributes...";
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
            // TreeBuilderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 438);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.splitContainer1);
            this.Name = "TreeBuilderControl";
            this.TabText = "Query Builder";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbFetchTree.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.gbQuickActions.ResumeLayout(false);
            this.gbQuickActions.PerformLayout();
            this.addMenu.ResumeLayout(false);
            this.nodeMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        internal System.Windows.Forms.GroupBox gbFetchTree;
        internal System.Windows.Forms.TreeView tvFetch;
        private System.Windows.Forms.GroupBox gbQuickActions;
        internal System.Windows.Forms.MenuStrip menuControl;
        private System.Windows.Forms.Panel panelButtonSpacer;
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
    }
}
