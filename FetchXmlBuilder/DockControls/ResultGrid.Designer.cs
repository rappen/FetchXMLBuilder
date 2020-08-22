using xrmtb.XrmToolBox.Controls;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    partial class ResultGrid
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.crmGridView1 = new xrmtb.XrmToolBox.Controls.CRMGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblOptionsExpander = new System.Windows.Forms.Label();
            this.mnuOptions = new System.Windows.Forms.MenuStrip();
            this.mnuColumns = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIndexCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIdCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNullCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSysCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBehavior = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFriendly = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLocalTime = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyHeaders = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.crmGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.mnuOptions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // crmGridView1
            // 
            this.crmGridView1.AllowUserToAddRows = false;
            this.crmGridView1.AllowUserToDeleteRows = false;
            this.crmGridView1.AllowUserToOrderColumns = true;
            this.crmGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.crmGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.crmGridView1.AutoRefresh = false;
            this.crmGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.crmGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.crmGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.crmGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.crmGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.crmGridView1.ColumnOrder = "";
            this.crmGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crmGridView1.EnableHeadersVisualStyles = false;
            this.crmGridView1.EntityReferenceClickable = true;
            this.crmGridView1.FilterColumns = "";
            this.crmGridView1.Location = new System.Drawing.Point(0, 0);
            this.crmGridView1.Name = "crmGridView1";
            this.crmGridView1.ReadOnly = true;
            this.crmGridView1.RowHeadersWidth = 24;
            this.crmGridView1.ShowEditingIcon = false;
            this.crmGridView1.Size = new System.Drawing.Size(650, 230);
            this.crmGridView1.TabIndex = 1;
            this.crmGridView1.RecordClick += new xrmtb.XrmToolBox.Controls.CRMRecordEventHandler(this.crmGridView1_RecordClick);
            this.crmGridView1.RecordDoubleClick += new xrmtb.XrmToolBox.Controls.CRMRecordEventHandler(this.crmGridView1_RecordDoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblOptionsExpander);
            this.groupBox1.Controls.Add(this.mnuOptions);
            this.groupBox1.Location = new System.Drawing.Point(-1, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(652, 42);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // lblOptionsExpander
            // 
            this.lblOptionsExpander.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOptionsExpander.AutoSize = true;
            this.lblOptionsExpander.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblOptionsExpander.Location = new System.Drawing.Point(631, 0);
            this.lblOptionsExpander.Name = "lblOptionsExpander";
            this.lblOptionsExpander.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblOptionsExpander.Size = new System.Drawing.Size(14, 13);
            this.lblOptionsExpander.TabIndex = 60;
            this.lblOptionsExpander.Text = "–";
            this.lblOptionsExpander.Click += new System.EventHandler(this.lblOptionsExpander_Click);
            // 
            // mnuOptions
            // 
            this.mnuOptions.BackColor = System.Drawing.SystemColors.Window;
            this.mnuOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mnuOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuBehavior,
            this.mnuColumns});
            this.mnuOptions.Location = new System.Drawing.Point(3, 15);
            this.mnuOptions.Name = "mnuOptions";
            this.mnuOptions.ShowItemToolTips = true;
            this.mnuOptions.Size = new System.Drawing.Size(646, 24);
            this.mnuOptions.TabIndex = 61;
            this.mnuOptions.Text = "menuStrip1";
            // 
            // mnuColumns
            // 
            this.mnuColumns.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuIndexCol,
            this.mnuIdCol,
            this.mnuNullCol,
            this.mnuSysCol});
            this.mnuColumns.Name = "mnuColumns";
            this.mnuColumns.Size = new System.Drawing.Size(67, 20);
            this.mnuColumns.Text = "Columns";
            // 
            // mnuIndexCol
            // 
            this.mnuIndexCol.CheckOnClick = true;
            this.mnuIndexCol.Name = "mnuIndexCol";
            this.mnuIndexCol.Size = new System.Drawing.Size(180, 22);
            this.mnuIndexCol.Text = "Index";
            this.mnuIndexCol.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuIdCol
            // 
            this.mnuIdCol.CheckOnClick = true;
            this.mnuIdCol.Name = "mnuIdCol";
            this.mnuIdCol.Size = new System.Drawing.Size(180, 22);
            this.mnuIdCol.Text = "Primary Key";
            this.mnuIdCol.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuNullCol
            // 
            this.mnuNullCol.CheckOnClick = true;
            this.mnuNullCol.Name = "mnuNullCol";
            this.mnuNullCol.Size = new System.Drawing.Size(180, 22);
            this.mnuNullCol.Text = "Without value";
            this.mnuNullCol.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuSysCol
            // 
            this.mnuSysCol.CheckOnClick = true;
            this.mnuSysCol.Name = "mnuSysCol";
            this.mnuSysCol.Size = new System.Drawing.Size(180, 22);
            this.mnuSysCol.Text = "System added";
            this.mnuSysCol.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuBehavior
            // 
            this.mnuBehavior.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFriendly,
            this.mnuLocalTime,
            this.mnuCopyHeaders});
            this.mnuBehavior.Name = "mnuBehavior";
            this.mnuBehavior.Size = new System.Drawing.Size(65, 20);
            this.mnuBehavior.Text = "Behavior";
            // 
            // mnuFriendly
            // 
            this.mnuFriendly.CheckOnClick = true;
            this.mnuFriendly.Name = "mnuFriendly";
            this.mnuFriendly.Size = new System.Drawing.Size(180, 22);
            this.mnuFriendly.Text = "Friendly Names";
            this.mnuFriendly.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuLocalTime
            // 
            this.mnuLocalTime.CheckOnClick = true;
            this.mnuLocalTime.Name = "mnuLocalTime";
            this.mnuLocalTime.Size = new System.Drawing.Size(180, 22);
            this.mnuLocalTime.Text = "Local Times";
            this.mnuLocalTime.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuCopyHeaders
            // 
            this.mnuCopyHeaders.CheckOnClick = true;
            this.mnuCopyHeaders.Name = "mnuCopyHeaders";
            this.mnuCopyHeaders.Size = new System.Drawing.Size(180, 22);
            this.mnuCopyHeaders.Text = "Copy with Headers";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 230);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(650, 43);
            this.panel1.TabIndex = 5;
            // 
            // ResultGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(650, 273);
            this.Controls.Add(this.crmGridView1);
            this.Controls.Add(this.panel1);
            this.HideOnClose = true;
            this.MainMenuStrip = this.mnuOptions;
            this.Name = "ResultGrid";
            this.Text = "Result View";
            this.DockStateChanged += new System.EventHandler(this.ResultGrid_DockStateChanged);
            ((System.ComponentModel.ISupportInitialize)(this.crmGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.mnuOptions.ResumeLayout(false);
            this.mnuOptions.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private CRMGridView crmGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblOptionsExpander;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.MenuStrip mnuOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuColumns;
        private System.Windows.Forms.ToolStripMenuItem mnuIndexCol;
        private System.Windows.Forms.ToolStripMenuItem mnuIdCol;
        private System.Windows.Forms.ToolStripMenuItem mnuNullCol;
        private System.Windows.Forms.ToolStripMenuItem mnuSysCol;
        private System.Windows.Forms.ToolStripMenuItem mnuBehavior;
        private System.Windows.Forms.ToolStripMenuItem mnuFriendly;
        private System.Windows.Forms.ToolStripMenuItem mnuLocalTime;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyHeaders;
    }
}