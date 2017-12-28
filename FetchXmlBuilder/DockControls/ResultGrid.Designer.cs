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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.crmGridView1 = new Cinteros.Xrm.CRMWinForm.CRMGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkFriendly = new System.Windows.Forms.CheckBox();
            this.chkIdCol = new System.Windows.Forms.CheckBox();
            this.chkIndexCol = new System.Windows.Forms.CheckBox();
            this.chkCopyHeaders = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.crmGridView1)).BeginInit();
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
            this.crmGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.crmGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.crmGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.crmGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.crmGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.crmGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crmGridView1.EnableHeadersVisualStyles = false;
            this.crmGridView1.EntityReferenceClickable = true;
            this.crmGridView1.Location = new System.Drawing.Point(0, 0);
            this.crmGridView1.Name = "crmGridView1";
            this.crmGridView1.ReadOnly = true;
            this.crmGridView1.RowHeadersWidth = 24;
            this.crmGridView1.Size = new System.Drawing.Size(731, 437);
            this.crmGridView1.TabIndex = 1;
            this.crmGridView1.RecordClick += new Cinteros.Xrm.CRMWinForm.CRMRecordEventHandler(this.crmGridView1_RecordClick);
            this.crmGridView1.RecordDoubleClick += new Cinteros.Xrm.CRMWinForm.CRMRecordEventHandler(this.crmGridView1_RecordDoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkCopyHeaders);
            this.panel1.Controls.Add(this.chkIndexCol);
            this.panel1.Controls.Add(this.chkIdCol);
            this.panel1.Controls.Add(this.chkFriendly);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 437);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(731, 31);
            this.panel1.TabIndex = 3;
            // 
            // chkFriendly
            // 
            this.chkFriendly.AutoSize = true;
            this.chkFriendly.Location = new System.Drawing.Point(11, 7);
            this.chkFriendly.Name = "chkFriendly";
            this.chkFriendly.Size = new System.Drawing.Size(98, 17);
            this.chkFriendly.TabIndex = 0;
            this.chkFriendly.Text = "Friendly Names";
            this.chkFriendly.UseVisualStyleBackColor = true;
            this.chkFriendly.CheckedChanged += new System.EventHandler(this.chkFriendly_CheckedChanged);
            // 
            // chkIdCol
            // 
            this.chkIdCol.AutoSize = true;
            this.chkIdCol.Location = new System.Drawing.Point(221, 7);
            this.chkIdCol.Name = "chkIdCol";
            this.chkIdCol.Size = new System.Drawing.Size(103, 17);
            this.chkIdCol.TabIndex = 1;
            this.chkIdCol.Text = "Show Record Id";
            this.chkIdCol.UseVisualStyleBackColor = true;
            this.chkIdCol.CheckedChanged += new System.EventHandler(this.chkIdCol_CheckedChanged);
            // 
            // chkIndexCol
            // 
            this.chkIndexCol.AutoSize = true;
            this.chkIndexCol.Location = new System.Drawing.Point(115, 7);
            this.chkIndexCol.Name = "chkIndexCol";
            this.chkIndexCol.Size = new System.Drawing.Size(100, 17);
            this.chkIndexCol.TabIndex = 2;
            this.chkIndexCol.Text = "Show Index Col";
            this.chkIndexCol.UseVisualStyleBackColor = true;
            this.chkIndexCol.CheckedChanged += new System.EventHandler(this.chkIndexCol_CheckedChanged);
            // 
            // chkCopyHeaders
            // 
            this.chkCopyHeaders.AutoSize = true;
            this.chkCopyHeaders.Location = new System.Drawing.Point(330, 7);
            this.chkCopyHeaders.Name = "chkCopyHeaders";
            this.chkCopyHeaders.Size = new System.Drawing.Size(115, 17);
            this.chkCopyHeaders.TabIndex = 3;
            this.chkCopyHeaders.Text = "Copy with Headers";
            this.chkCopyHeaders.UseVisualStyleBackColor = true;
            this.chkCopyHeaders.CheckedChanged += new System.EventHandler(this.chkCopyHeaders_CheckedChanged);
            // 
            // ResultGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(731, 468);
            this.Controls.Add(this.crmGridView1);
            this.Controls.Add(this.panel1);
            this.Name = "ResultGrid";
            this.Text = "Results";
            this.DockStateChanged += new System.EventHandler(this.ResultGrid_DockStateChanged);
            this.Load += new System.EventHandler(this.ResultGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.crmGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private CRMWinForm.CRMGridView crmGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkCopyHeaders;
        private System.Windows.Forms.CheckBox chkIndexCol;
        private System.Windows.Forms.CheckBox chkIdCol;
        private System.Windows.Forms.CheckBox chkFriendly;
    }
}