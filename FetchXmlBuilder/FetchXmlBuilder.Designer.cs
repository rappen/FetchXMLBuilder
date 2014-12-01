namespace Cinteros.Xrm.FetchXmlBuilder
{
    partial class FetchXmlBuilder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FetchXmlBuilder));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.tsbCloseThisTab = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbNew = new System.Windows.Forms.ToolStripButton();
            this.tsbOpen = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiSaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveFileAs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbExecute = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiFetchRequest = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRetrieveMultiple = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.gbFetchTree = new System.Windows.Forms.GroupBox();
            this.tvFetch = new System.Windows.Forms.TreeView();
            this.chkFriendlyNames = new System.Windows.Forms.CheckBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.nodeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dummyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorBeginOfEdition = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMain.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.gbFetchTree.SuspendLayout();
            this.nodeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Cinteros 100 transp.png");
            // 
            // toolStripMain
            // 
            this.toolStripMain.AutoSize = false;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCloseThisTab,
            this.toolStripSeparator4,
            this.tsbNew,
            this.tsbEdit,
            this.toolStripSeparator2,
            this.tsbExecute,
            this.toolStripSeparator3,
            this.tsbOpen,
            this.tsbSave});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(884, 25);
            this.toolStripMain.TabIndex = 22;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // tsbCloseThisTab
            // 
            this.tsbCloseThisTab.Image = ((System.Drawing.Image)(resources.GetObject("tsbCloseThisTab.Image")));
            this.tsbCloseThisTab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCloseThisTab.Name = "tsbCloseThisTab";
            this.tsbCloseThisTab.Size = new System.Drawing.Size(98, 22);
            this.tsbCloseThisTab.Text = "Close this tab";
            this.tsbCloseThisTab.Click += new System.EventHandler(this.tsbCloseThisTab_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbNew
            // 
            this.tsbNew.Image = ((System.Drawing.Image)(resources.GetObject("tsbNew.Image")));
            this.tsbNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNew.Name = "tsbNew";
            this.tsbNew.Size = new System.Drawing.Size(51, 22);
            this.tsbNew.Text = "New";
            this.tsbNew.ToolTipText = "New Fetch XML";
            this.tsbNew.Click += new System.EventHandler(this.tsbNew_Click);
            // 
            // tsbOpen
            // 
            this.tsbOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpenFile,
            this.toolStripSeparator1,
            this.tsmiOpenView});
            this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(65, 22);
            this.tsbOpen.Text = "Open";
            this.tsbOpen.ToolTipText = "Open Fetch XML file";
            // 
            // tsmiOpenFile
            // 
            this.tsmiOpenFile.Name = "tsmiOpenFile";
            this.tsmiOpenFile.Size = new System.Drawing.Size(140, 22);
            this.tsmiOpenFile.Text = "Open File...";
            this.tsmiOpenFile.Click += new System.EventHandler(this.tsmiOpenFile_Click);
            // 
            // tsmiOpenView
            // 
            this.tsmiOpenView.Name = "tsmiOpenView";
            this.tsmiOpenView.Size = new System.Drawing.Size(140, 22);
            this.tsmiOpenView.Text = "Open View...";
            this.tsmiOpenView.Click += new System.EventHandler(this.tsmiOpenView_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSaveFile,
            this.tsmiSaveFileAs,
            this.toolStripSeparator5,
            this.tsmiSaveView});
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(60, 22);
            this.tsbSave.Text = "Save";
            this.tsbSave.ToolTipText = "Save Fetch XML";
            // 
            // tsmiSaveFile
            // 
            this.tsmiSaveFile.Enabled = false;
            this.tsmiSaveFile.Name = "tsmiSaveFile";
            this.tsmiSaveFile.Size = new System.Drawing.Size(152, 22);
            this.tsmiSaveFile.Text = "Save File";
            this.tsmiSaveFile.Click += new System.EventHandler(this.tsmiSaveFile_Click);
            // 
            // tsmiSaveFileAs
            // 
            this.tsmiSaveFileAs.Name = "tsmiSaveFileAs";
            this.tsmiSaveFileAs.Size = new System.Drawing.Size(142, 22);
            this.tsmiSaveFileAs.Text = "Save File as...";
            this.tsmiSaveFileAs.Click += new System.EventHandler(this.tsmiSaveFileAs_Click);
            // 
            // tsmiSaveView
            // 
            this.tsmiSaveView.Enabled = false;
            this.tsmiSaveView.Name = "tsmiSaveView";
            this.tsmiSaveView.Size = new System.Drawing.Size(152, 22);
            this.tsmiSaveView.Text = "Save View";
            this.tsmiSaveView.Click += new System.EventHandler(this.tsmiSaveView_Click);
            // 
            // tsbEdit
            // 
            this.tsbEdit.Image = ((System.Drawing.Image)(resources.GetObject("tsbEdit.Image")));
            this.tsbEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEdit.Name = "tsbEdit";
            this.tsbEdit.Size = new System.Drawing.Size(74, 22);
            this.tsbEdit.Text = "Edit XML";
            this.tsbEdit.Click += new System.EventHandler(this.tsbEdit_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbExecute
            // 
            this.tsbExecute.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFetchRequest,
            this.tsmiRetrieveMultiple});
            this.tsbExecute.Enabled = false;
            this.tsbExecute.Image = ((System.Drawing.Image)(resources.GetObject("tsbExecute.Image")));
            this.tsbExecute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExecute.Name = "tsbExecute";
            this.tsbExecute.Size = new System.Drawing.Size(108, 22);
            this.tsbExecute.Text = "Execute Fetch";
            this.tsbExecute.ToolTipText = "Execute Fetch XML to see the results";
            // 
            // tsmiFetchRequest
            // 
            this.tsmiFetchRequest.Name = "tsmiFetchRequest";
            this.tsmiFetchRequest.Size = new System.Drawing.Size(160, 22);
            this.tsmiFetchRequest.Tag = "FetchRequest";
            this.tsmiFetchRequest.Text = "FetchRequest";
            this.tsmiFetchRequest.Click += new System.EventHandler(this.toolStripButtonExecute_Click);
            // 
            // tsmiRetrieveMultiple
            // 
            this.tsmiRetrieveMultiple.Name = "tsmiRetrieveMultiple";
            this.tsmiRetrieveMultiple.Size = new System.Drawing.Size(160, 22);
            this.tsmiRetrieveMultiple.Tag = "RetrieveMultiple";
            this.tsmiRetrieveMultiple.Text = "RetrieveMultiple";
            this.tsmiRetrieveMultiple.Click += new System.EventHandler(this.toolStripButtonExecute_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // gbProperties
            // 
            this.gbProperties.Controls.Add(this.panelContainer);
            this.gbProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbProperties.Location = new System.Drawing.Point(394, 25);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(490, 599);
            this.gbProperties.TabIndex = 25;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Fetch XML node attributes";
            // 
            // panelContainer
            // 
            this.panelContainer.BackColor = System.Drawing.SystemColors.Control;
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(3, 16);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(484, 580);
            this.panelContainer.TabIndex = 14;
            // 
            // gbFetchTree
            // 
            this.gbFetchTree.Controls.Add(this.tvFetch);
            this.gbFetchTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.gbFetchTree.Location = new System.Drawing.Point(0, 25);
            this.gbFetchTree.Name = "gbFetchTree";
            this.gbFetchTree.Size = new System.Drawing.Size(394, 599);
            this.gbFetchTree.TabIndex = 24;
            this.gbFetchTree.TabStop = false;
            this.gbFetchTree.Text = "Fetch XML outline";
            // 
            // tvFetch
            // 
            this.tvFetch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFetch.HideSelection = false;
            this.tvFetch.Location = new System.Drawing.Point(3, 16);
            this.tvFetch.Name = "tvFetch";
            this.tvFetch.ShowNodeToolTips = true;
            this.tvFetch.Size = new System.Drawing.Size(388, 580);
            this.tvFetch.TabIndex = 0;
            this.tvFetch.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvFetch_NodeMouseClick);
            // 
            // chkFriendlyNames
            // 
            this.chkFriendlyNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkFriendlyNames.AutoSize = true;
            this.chkFriendlyNames.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFriendlyNames.Enabled = false;
            this.chkFriendlyNames.Location = new System.Drawing.Point(783, 4);
            this.chkFriendlyNames.Name = "chkFriendlyNames";
            this.chkFriendlyNames.Size = new System.Drawing.Size(96, 17);
            this.chkFriendlyNames.TabIndex = 18;
            this.chkFriendlyNames.Text = "Friendly names";
            this.chkFriendlyNames.UseVisualStyleBackColor = true;
            this.chkFriendlyNames.CheckedChanged += new System.EventHandler(this.chkTechNames_CheckedChanged);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(394, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 599);
            this.splitter1.TabIndex = 26;
            this.splitter1.TabStop = false;
            // 
            // nodeMenu
            // 
            this.nodeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparatorBeginOfEdition,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem2,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.nodeMenu.Name = "nodeMenu";
            this.nodeMenu.Size = new System.Drawing.Size(138, 176);
            this.nodeMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.nodeMenu_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(134, 6);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dummyToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.addToolStripMenuItem.Tag = "Add";
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.nodeMenu_ItemClicked);
            // 
            // dummyToolStripMenuItem
            // 
            this.dummyToolStripMenuItem.Name = "dummyToolStripMenuItem";
            this.dummyToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.dummyToolStripMenuItem.Text = "dummy";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem.Image")));
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // toolStripSeparatorBeginOfEdition
            // 
            this.toolStripSeparatorBeginOfEdition.Name = "toolStripSeparatorBeginOfEdition";
            this.toolStripSeparatorBeginOfEdition.Size = new System.Drawing.Size(134, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.cutToolStripMenuItem.Tag = "Cut";
            this.cutToolStripMenuItem.Text = "Cut";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.copyToolStripMenuItem.Tag = "Copy";
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.pasteToolStripMenuItem.Tag = "Paste";
            this.pasteToolStripMenuItem.Text = "Paste";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(134, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("moveUpToolStripMenuItem.Image")));
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.moveUpToolStripMenuItem.Text = "Move up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonMoveUp_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("moveDownToolStripMenuItem.Image")));
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.moveDownToolStripMenuItem.Text = "Move down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonMoveDown_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(149, 6);
            // 
            // FetchXmlBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkFriendlyNames);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.gbProperties);
            this.Controls.Add(this.gbFetchTree);
            this.Controls.Add(this.toolStripMain);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FetchXmlBuilder";
            this.Size = new System.Drawing.Size(884, 624);
            this.OnCloseTool += new System.EventHandler(this.FetchXmlBuilder_OnCloseTool);
            this.ConnectionUpdated += new XrmToolBox.PluginBase.ConnectionUpdatedHandler(this.FetchXmlBuilder_ConnectionUpdated);
            this.Load += new System.EventHandler(this.FetchXmlBuilder_Load);
            this.Leave += new System.EventHandler(this.FetchXmlBuilder_Leave);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.gbProperties.ResumeLayout(false);
            this.gbFetchTree.ResumeLayout(false);
            this.nodeMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        internal System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton tsbCloseThisTab;
        internal System.Windows.Forms.GroupBox gbProperties;
        internal System.Windows.Forms.Panel panelContainer;
        internal System.Windows.Forms.GroupBox gbFetchTree;
        internal System.Windows.Forms.TreeView tvFetch;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripButton tsbNew;
        internal System.Windows.Forms.ContextMenuStrip nodeMenu;
        internal System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparatorBeginOfEdition;
        internal System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dummyToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkFriendlyNames;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsbEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton tsbExecute;
        private System.Windows.Forms.ToolStripMenuItem tsmiFetchRequest;
        private System.Windows.Forms.ToolStripMenuItem tsmiRetrieveMultiple;
        internal System.Windows.Forms.ToolStripDropDownButton tsbOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenView;
        internal System.Windows.Forms.ToolStripDropDownButton tsbSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveFileAs;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    }
}
