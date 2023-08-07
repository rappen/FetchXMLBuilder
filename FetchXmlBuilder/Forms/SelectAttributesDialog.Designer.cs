namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    partial class SelectAttributesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectAttributesDialog));
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkMetamore = new System.Windows.Forms.CheckBox();
            this.panOk = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.panCancel = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lvAttributes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lnkShowRequired = new System.Windows.Forms.LinkLabel();
            this.lblSelectedNo = new System.Windows.Forms.Label();
            this.metadataControl1 = new Rappen.XTB.FetchXmlBuilder.Controls.XRMMetadataControl();
            this.lnkShowAll = new System.Windows.Forms.LinkLabel();
            this.panel2.SuspendLayout();
            this.panOk.SuspendLayout();
            this.panCancel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblSelectedNo);
            this.panel2.Controls.Add(this.chkMetamore);
            this.panel2.Controls.Add(this.panOk);
            this.panel2.Controls.Add(this.panCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 450);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(493, 51);
            this.panel2.TabIndex = 6;
            // 
            // chkMetamore
            // 
            this.chkMetamore.AutoSize = true;
            this.chkMetamore.Location = new System.Drawing.Point(15, 18);
            this.chkMetamore.Name = "chkMetamore";
            this.chkMetamore.Size = new System.Drawing.Size(100, 17);
            this.chkMetamore.TabIndex = 7;
            this.chkMetamore.Text = "Show metadata";
            this.chkMetamore.UseVisualStyleBackColor = true;
            this.chkMetamore.CheckedChanged += new System.EventHandler(this.chkMetamore_CheckedChanged);
            // 
            // panOk
            // 
            this.panOk.Controls.Add(this.btnOk);
            this.panOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.panOk.Location = new System.Drawing.Point(300, 0);
            this.panOk.Name = "panOk";
            this.panOk.Size = new System.Drawing.Size(102, 51);
            this.panOk.TabIndex = 6;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(23, 14);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // panCancel
            // 
            this.panCancel.Controls.Add(this.button2);
            this.panCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.panCancel.Location = new System.Drawing.Point(402, 0);
            this.panCancel.Name = "panCancel";
            this.panCancel.Size = new System.Drawing.Size(91, 51);
            this.panCancel.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(2, 14);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtFilter);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(493, 45);
            this.panel1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(167, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Attribute filter";
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(251, 16);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(229, 20);
            this.txtFilter.TabIndex = 1;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(15, 18);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(125, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Check / Uncheck all";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // lvAttributes
            // 
            this.lvAttributes.CheckBoxes = true;
            this.lvAttributes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.lvAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAttributes.FullRowSelect = true;
            this.lvAttributes.HideSelection = false;
            this.lvAttributes.Location = new System.Drawing.Point(9, 72);
            this.lvAttributes.Name = "lvAttributes";
            this.lvAttributes.ShowGroups = false;
            this.lvAttributes.Size = new System.Drawing.Size(484, 378);
            this.lvAttributes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvAttributes.TabIndex = 8;
            this.lvAttributes.UseCompatibleStateImageBehavior = false;
            this.lvAttributes.View = System.Windows.Forms.View.Details;
            this.lvAttributes.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvAttributes_ColumnClick);
            this.lvAttributes.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvAttributes_ItemChecked);
            this.lvAttributes.SelectedIndexChanged += new System.EventHandler(this.lvAttributes_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 166;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Logical Name";
            this.columnHeader2.Width = 169;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Tag = "meta";
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 114;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Tag = "meta";
            this.columnHeader4.Text = "Read";
            this.columnHeader4.Width = 40;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Tag = "meta";
            this.columnHeader5.Text = "Grid";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Tag = "meta";
            this.columnHeader6.Text = "Adv.Find";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Tag = "meta";
            this.columnHeader7.Text = "Retriev";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvAttributes);
            this.splitContainer1.Panel1.Controls.Add(this.panel4);
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.metadataControl1);
            this.splitContainer1.Size = new System.Drawing.Size(792, 501);
            this.splitContainer1.SplitterDistance = 493;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 10;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 45);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(9, 405);
            this.panel3.TabIndex = 9;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lnkShowAll);
            this.panel4.Controls.Add(this.lnkShowRequired);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(9, 45);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(484, 27);
            this.panel4.TabIndex = 10;
            // 
            // lnkShowRequired
            // 
            this.lnkShowRequired.AutoSize = true;
            this.lnkShowRequired.Location = new System.Drawing.Point(27, 3);
            this.lnkShowRequired.Name = "lnkShowRequired";
            this.lnkShowRequired.Size = new System.Drawing.Size(50, 13);
            this.lnkShowRequired.TabIndex = 0;
            this.lnkShowRequired.TabStop = true;
            this.lnkShowRequired.Text = "Required";
            this.lnkShowRequired.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkShowRequired_LinkClicked);
            // 
            // lblSelectedNo
            // 
            this.lblSelectedNo.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblSelectedNo.Location = new System.Drawing.Point(150, 0);
            this.lblSelectedNo.Name = "lblSelectedNo";
            this.lblSelectedNo.Size = new System.Drawing.Size(150, 51);
            this.lblSelectedNo.TabIndex = 8;
            this.lblSelectedNo.Text = "Selected 0/x";
            this.lblSelectedNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // metadataControl1
            // 
            this.metadataControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metadataControl1.HeaderSeparator = false;
            this.metadataControl1.Location = new System.Drawing.Point(0, 0);
            this.metadataControl1.MscrmLinkSeparator = false;
            this.metadataControl1.Name = "metadataControl1";
            this.metadataControl1.Size = new System.Drawing.Size(291, 501);
            this.metadataControl1.TabIndex = 0;
            // 
            // lnkShowAll
            // 
            this.lnkShowAll.AutoSize = true;
            this.lnkShowAll.Location = new System.Drawing.Point(3, 3);
            this.lnkShowAll.Name = "lnkShowAll";
            this.lnkShowAll.Size = new System.Drawing.Size(18, 13);
            this.lnkShowAll.TabIndex = 1;
            this.lnkShowAll.TabStop = true;
            this.lnkShowAll.Text = "All";
            this.lnkShowAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkShowAll_LinkClicked);
            // 
            // SelectAttributesDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(792, 501);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectAttributesDialog";
            this.Text = "Select Attributes";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panOk.ResumeLayout(false);
            this.panCancel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panOk;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panCancel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ListView lvAttributes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.CheckBox chkMetamore;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Controls.XRMMetadataControl metadataControl1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.LinkLabel lnkShowRequired;
        private System.Windows.Forms.Label lblSelectedNo;
        private System.Windows.Forms.LinkLabel lnkShowAll;
    }
}