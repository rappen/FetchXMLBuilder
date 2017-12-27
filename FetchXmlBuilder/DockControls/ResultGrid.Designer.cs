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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.crmGridView1 = new Cinteros.Xrm.CRMWinForm.CRMGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.displayOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFriendly = new System.Windows.Forms.ToolStripMenuItem();
            this.menuIdColumn = new System.Windows.Forms.ToolStripMenuItem();
            this.menuIndexColumn = new System.Windows.Forms.ToolStripMenuItem();
            this.hintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doubleClickARowToOpenRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySelectedCellsUsingCTRLCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuCopyHeaders = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crmGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(731, 468);
            this.panel2.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.crmGridView1);
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(731, 468);
            this.panel1.TabIndex = 2;
            // 
            // crmGridView1
            // 
            this.crmGridView1.AllowUserToAddRows = false;
            this.crmGridView1.AllowUserToDeleteRows = false;
            this.crmGridView1.AllowUserToOrderColumns = true;
            this.crmGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Azure;
            this.crmGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.crmGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.crmGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.crmGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.crmGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crmGridView1.EnableHeadersVisualStyles = false;
            this.crmGridView1.EntityReferenceClickable = true;
            this.crmGridView1.Location = new System.Drawing.Point(0, 24);
            this.crmGridView1.Name = "crmGridView1";
            this.crmGridView1.ReadOnly = true;
            this.crmGridView1.RowHeadersWidth = 24;
            this.crmGridView1.Size = new System.Drawing.Size(731, 444);
            this.crmGridView1.TabIndex = 1;
            this.crmGridView1.RecordClick += new Cinteros.Xrm.CRMWinForm.CRMRecordEventHandler(this.crmGridView1_RecordClick);
            this.crmGridView1.RecordDoubleClick += new Cinteros.Xrm.CRMWinForm.CRMRecordEventHandler(this.crmGridView1_RecordDoubleClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayOptionsToolStripMenuItem,
            this.hintsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(731, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // displayOptionsToolStripMenuItem
            // 
            this.displayOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFriendly,
            this.toolStripMenuItem1,
            this.menuIdColumn,
            this.menuIndexColumn,
            this.toolStripMenuItem2,
            this.menuCopyHeaders});
            this.displayOptionsToolStripMenuItem.Name = "displayOptionsToolStripMenuItem";
            this.displayOptionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.displayOptionsToolStripMenuItem.Text = "Options";
            // 
            // menuFriendly
            // 
            this.menuFriendly.CheckOnClick = true;
            this.menuFriendly.Name = "menuFriendly";
            this.menuFriendly.Size = new System.Drawing.Size(215, 22);
            this.menuFriendly.Text = "Friendly Names and Values";
            this.menuFriendly.CheckedChanged += new System.EventHandler(this.menuFriendly_CheckedChanged);
            // 
            // menuIdColumn
            // 
            this.menuIdColumn.CheckOnClick = true;
            this.menuIdColumn.Name = "menuIdColumn";
            this.menuIdColumn.Size = new System.Drawing.Size(215, 22);
            this.menuIdColumn.Text = "Record ID column";
            this.menuIdColumn.CheckedChanged += new System.EventHandler(this.menuIdColumn_CheckedChanged);
            // 
            // menuIndexColumn
            // 
            this.menuIndexColumn.CheckOnClick = true;
            this.menuIndexColumn.Name = "menuIndexColumn";
            this.menuIndexColumn.Size = new System.Drawing.Size(215, 22);
            this.menuIndexColumn.Text = "Index column";
            this.menuIndexColumn.CheckedChanged += new System.EventHandler(this.menuIndexColumn_CheckedChanged);
            // 
            // hintsToolStripMenuItem
            // 
            this.hintsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.doubleClickARowToOpenRecordToolStripMenuItem,
            this.copySelectedCellsUsingCTRLCToolStripMenuItem});
            this.hintsToolStripMenuItem.Name = "hintsToolStripMenuItem";
            this.hintsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.hintsToolStripMenuItem.Text = "Hints...";
            // 
            // doubleClickARowToOpenRecordToolStripMenuItem
            // 
            this.doubleClickARowToOpenRecordToolStripMenuItem.Name = "doubleClickARowToOpenRecordToolStripMenuItem";
            this.doubleClickARowToOpenRecordToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.doubleClickARowToOpenRecordToolStripMenuItem.Text = "Double click a row to open record";
            // 
            // copySelectedCellsUsingCTRLCToolStripMenuItem
            // 
            this.copySelectedCellsUsingCTRLCToolStripMenuItem.Name = "copySelectedCellsUsingCTRLCToolStripMenuItem";
            this.copySelectedCellsUsingCTRLCToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.copySelectedCellsUsingCTRLCToolStripMenuItem.Text = "Copy selected cells using <CTRL>+C";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(212, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(212, 6);
            // 
            // menuCopyHeaders
            // 
            this.menuCopyHeaders.CheckOnClick = true;
            this.menuCopyHeaders.Name = "menuCopyHeaders";
            this.menuCopyHeaders.Size = new System.Drawing.Size(215, 22);
            this.menuCopyHeaders.Text = "Copy with Headers";
            this.menuCopyHeaders.CheckedChanged += new System.EventHandler(this.menuCopyHeaders_CheckedChanged);
            // 
            // ResultGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 468);
            this.Controls.Add(this.panel2);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ResultGrid";
            this.Text = "FetchXML Builder - Result Grid";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ResultGrid_FormClosing);
            this.Load += new System.EventHandler(this.ResultGrid_Load);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crmGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private CRMWinForm.CRMGridView crmGridView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem displayOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuFriendly;
        private System.Windows.Forms.ToolStripMenuItem menuIdColumn;
        private System.Windows.Forms.ToolStripMenuItem menuIndexColumn;
        private System.Windows.Forms.ToolStripMenuItem hintsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doubleClickARowToOpenRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySelectedCellsUsingCTRLCToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuCopyHeaders;
    }
}