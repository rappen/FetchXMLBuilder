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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectAttributesDialog));
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkMetamore = new System.Windows.Forms.CheckBox();
            this.panOk = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.panCancel = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.lblSelectedNo = new System.Windows.Forms.Label();
            this.lnkShowOnViews = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.lnkShowAll = new System.Windows.Forms.LinkLabel();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lnkShowRequired = new System.Windows.Forms.LinkLabel();
            this.lvAttributes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lnkUncheckAll = new System.Windows.Forms.LinkLabel();
            this.lnkUncheckShown = new System.Windows.Forms.LinkLabel();
            this.lnkCheckShown = new System.Windows.Forms.LinkLabel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.timerSummary = new System.Windows.Forms.Timer(this.components);
            this.metadataControl1 = new Rappen.XTB.FetchXmlBuilder.Controls.XRMMetadataControl();
            this.grpChecked = new System.Windows.Forms.GroupBox();
            this.grpShow = new System.Windows.Forms.GroupBox();
            this.lnkShowOnForms = new System.Windows.Forms.LinkLabel();
            this.panel2.SuspendLayout();
            this.panOk.SuspendLayout();
            this.panCancel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpChecked.SuspendLayout();
            this.grpShow.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chkMetamore);
            this.panel2.Controls.Add(this.panOk);
            this.panel2.Controls.Add(this.panCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 450);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(456, 51);
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
            this.panOk.Location = new System.Drawing.Point(263, 0);
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
            this.panCancel.Location = new System.Drawing.Point(365, 0);
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
            // lblSelectedNo
            // 
            this.lblSelectedNo.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectedNo.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblSelectedNo.Location = new System.Drawing.Point(262, 16);
            this.lblSelectedNo.Name = "lblSelectedNo";
            this.lblSelectedNo.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.lblSelectedNo.Size = new System.Drawing.Size(191, 24);
            this.lblSelectedNo.TabIndex = 8;
            this.lblSelectedNo.Text = "Selected 0/x";
            this.lblSelectedNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lnkShowOnViews
            // 
            this.lnkShowOnViews.AutoSize = true;
            this.lnkShowOnViews.Location = new System.Drawing.Point(122, 19);
            this.lnkShowOnViews.Name = "lnkShowOnViews";
            this.lnkShowOnViews.Size = new System.Drawing.Size(52, 13);
            this.lnkShowOnViews.TabIndex = 2;
            this.lnkShowOnViews.TabStop = true;
            this.lnkShowOnViews.Text = "On Views";
            this.lnkShowOnViews.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkShowOnViews_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(251, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Filter";
            // 
            // lnkShowAll
            // 
            this.lnkShowAll.AutoSize = true;
            this.lnkShowAll.Location = new System.Drawing.Point(12, 19);
            this.lnkShowAll.Name = "lnkShowAll";
            this.lnkShowAll.Size = new System.Drawing.Size(48, 13);
            this.lnkShowAll.TabIndex = 1;
            this.lnkShowAll.TabStop = true;
            this.lnkShowAll.Text = "Show All";
            this.lnkShowAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkShowAll_LinkClicked);
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(286, 16);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(157, 20);
            this.txtFilter.TabIndex = 1;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // lnkShowRequired
            // 
            this.lnkShowRequired.AutoSize = true;
            this.lnkShowRequired.Location = new System.Drawing.Point(66, 19);
            this.lnkShowRequired.Name = "lnkShowRequired";
            this.lnkShowRequired.Size = new System.Drawing.Size(50, 13);
            this.lnkShowRequired.TabIndex = 0;
            this.lnkShowRequired.TabStop = true;
            this.lnkShowRequired.Text = "Required";
            this.lnkShowRequired.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkShowRequired_LinkClicked);
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
            this.lvAttributes.Location = new System.Drawing.Point(9, 91);
            this.lvAttributes.Name = "lvAttributes";
            this.lvAttributes.ShowGroups = false;
            this.lvAttributes.Size = new System.Drawing.Size(447, 359);
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
            this.columnHeader1.Width = 228;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Logical Name";
            this.columnHeader2.Width = 218;
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
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Controls.Add(this.grpChecked);
            this.splitContainer1.Panel1.Controls.Add(this.grpShow);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.metadataControl1);
            this.splitContainer1.Size = new System.Drawing.Size(763, 501);
            this.splitContainer1.SplitterDistance = 456;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 10;
            // 
            // lnkUncheckAll
            // 
            this.lnkUncheckAll.AutoSize = true;
            this.lnkUncheckAll.Location = new System.Drawing.Point(185, 19);
            this.lnkUncheckAll.Name = "lnkUncheckAll";
            this.lnkUncheckAll.Size = new System.Drawing.Size(65, 13);
            this.lnkUncheckAll.TabIndex = 11;
            this.lnkUncheckAll.TabStop = true;
            this.lnkUncheckAll.Text = "Uncheck All";
            this.lnkUncheckAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUncheckAll_LinkClicked);
            // 
            // lnkUncheckShown
            // 
            this.lnkUncheckShown.AutoSize = true;
            this.lnkUncheckShown.Location = new System.Drawing.Point(92, 19);
            this.lnkUncheckShown.Name = "lnkUncheckShown";
            this.lnkUncheckShown.Size = new System.Drawing.Size(87, 13);
            this.lnkUncheckShown.TabIndex = 10;
            this.lnkUncheckShown.TabStop = true;
            this.lnkUncheckShown.Text = "Uncheck Shown";
            this.lnkUncheckShown.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUncheckShown_LinkClicked);
            // 
            // lnkCheckShown
            // 
            this.lnkCheckShown.AutoSize = true;
            this.lnkCheckShown.Location = new System.Drawing.Point(12, 19);
            this.lnkCheckShown.Name = "lnkCheckShown";
            this.lnkCheckShown.Size = new System.Drawing.Size(74, 13);
            this.lnkCheckShown.TabIndex = 9;
            this.lnkCheckShown.TabStop = true;
            this.lnkCheckShown.Text = "Check Shown";
            this.lnkCheckShown.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCheckShown_LinkClicked);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 91);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(9, 359);
            this.panel3.TabIndex = 9;
            // 
            // timerSummary
            // 
            this.timerSummary.Tick += new System.EventHandler(this.timerSummary_Tick);
            // 
            // metadataControl1
            // 
            this.metadataControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metadataControl1.HeaderSeparator = false;
            this.metadataControl1.Location = new System.Drawing.Point(0, 0);
            this.metadataControl1.MscrmLinkSeparator = false;
            this.metadataControl1.Name = "metadataControl1";
            this.metadataControl1.Size = new System.Drawing.Size(299, 501);
            this.metadataControl1.TabIndex = 0;
            // 
            // grpChecked
            // 
            this.grpChecked.Controls.Add(this.lnkUncheckAll);
            this.grpChecked.Controls.Add(this.lblSelectedNo);
            this.grpChecked.Controls.Add(this.lnkUncheckShown);
            this.grpChecked.Controls.Add(this.lnkCheckShown);
            this.grpChecked.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpChecked.Location = new System.Drawing.Point(0, 48);
            this.grpChecked.Name = "grpChecked";
            this.grpChecked.Size = new System.Drawing.Size(456, 43);
            this.grpChecked.TabIndex = 11;
            this.grpChecked.TabStop = false;
            this.grpChecked.Text = "Checked attributes";
            // 
            // grpShow
            // 
            this.grpShow.Controls.Add(this.lnkShowOnForms);
            this.grpShow.Controls.Add(this.lnkShowOnViews);
            this.grpShow.Controls.Add(this.lnkShowAll);
            this.grpShow.Controls.Add(this.label1);
            this.grpShow.Controls.Add(this.lnkShowRequired);
            this.grpShow.Controls.Add(this.txtFilter);
            this.grpShow.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpShow.Location = new System.Drawing.Point(0, 0);
            this.grpShow.Name = "grpShow";
            this.grpShow.Size = new System.Drawing.Size(456, 48);
            this.grpShow.TabIndex = 12;
            this.grpShow.TabStop = false;
            this.grpShow.Text = "Show attributes";
            // 
            // lnkShowOnForms
            // 
            this.lnkShowOnForms.AutoSize = true;
            this.lnkShowOnForms.Enabled = false;
            this.lnkShowOnForms.Location = new System.Drawing.Point(180, 19);
            this.lnkShowOnForms.Name = "lnkShowOnForms";
            this.lnkShowOnForms.Size = new System.Drawing.Size(52, 13);
            this.lnkShowOnForms.TabIndex = 3;
            this.lnkShowOnForms.TabStop = true;
            this.lnkShowOnForms.Text = "On Forms";
            // 
            // SelectAttributesDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(763, 501);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectAttributesDialog";
            this.Text = "Select Attributes";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panOk.ResumeLayout(false);
            this.panCancel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpChecked.ResumeLayout(false);
            this.grpChecked.PerformLayout();
            this.grpShow.ResumeLayout(false);
            this.grpShow.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panOk;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panCancel;
        private System.Windows.Forms.Button button2;
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
        private System.Windows.Forms.LinkLabel lnkShowRequired;
        private System.Windows.Forms.Label lblSelectedNo;
        private System.Windows.Forms.LinkLabel lnkShowAll;
        private System.Windows.Forms.LinkLabel lnkShowOnViews;
        private System.Windows.Forms.Timer timerSummary;
        private System.Windows.Forms.LinkLabel lnkUncheckAll;
        private System.Windows.Forms.LinkLabel lnkUncheckShown;
        private System.Windows.Forms.LinkLabel lnkCheckShown;
        private System.Windows.Forms.GroupBox grpChecked;
        private System.Windows.Forms.GroupBox grpShow;
        private System.Windows.Forms.LinkLabel lnkShowOnForms;
    }
}