namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    partial class RefreshMetadataOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RefreshMetadataOptions));
            this.lblEntities = new System.Windows.Forms.Label();
            this.lblAttributes = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panSelectSolution = new System.Windows.Forms.Panel();
            this.chkShowAllSolutions = new System.Windows.Forms.CheckBox();
            this.xrmSolution = new Rappen.XTB.Helpers.Controls.XRMColumnLookup();
            this.panSolutions = new System.Windows.Forms.Panel();
            this.rbSpecificPublisher = new System.Windows.Forms.RadioButton();
            this.rbSpecificSolution = new System.Windows.Forms.RadioButton();
            this.rbUnmanagedSolution = new System.Windows.Forms.RadioButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panSelectSolution.SuspendLayout();
            this.panSolutions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblEntities
            // 
            this.lblEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEntities.Location = new System.Drawing.Point(12, 29);
            this.lblEntities.Name = "lblEntities";
            this.lblEntities.Size = new System.Drawing.Size(227, 47);
            this.lblEntities.TabIndex = 102;
            this.lblEntities.Text = "All entities";
            // 
            // lblAttributes
            // 
            this.lblAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAttributes.Location = new System.Drawing.Point(262, 29);
            this.lblAttributes.Name = "lblAttributes";
            this.lblAttributes.Size = new System.Drawing.Size(227, 47);
            this.lblAttributes.TabIndex = 105;
            this.lblAttributes.Text = "All attributes";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.panSelectSolution);
            this.groupBox2.Controls.Add(this.panSolutions);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(477, 137);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter by Solution";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 85);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(471, 49);
            this.panel1.TabIndex = 101;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(392, 19);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(311, 19);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // panSelectSolution
            // 
            this.panSelectSolution.Controls.Add(this.chkShowAllSolutions);
            this.panSelectSolution.Controls.Add(this.xrmSolution);
            this.panSelectSolution.Enabled = false;
            this.panSelectSolution.Location = new System.Drawing.Point(6, 45);
            this.panSelectSolution.Name = "panSelectSolution";
            this.panSelectSolution.Size = new System.Drawing.Size(465, 34);
            this.panSelectSolution.TabIndex = 3;
            // 
            // chkShowAllSolutions
            // 
            this.chkShowAllSolutions.AutoSize = true;
            this.chkShowAllSolutions.Location = new System.Drawing.Point(384, 6);
            this.chkShowAllSolutions.Name = "chkShowAllSolutions";
            this.chkShowAllSolutions.Size = new System.Drawing.Size(66, 17);
            this.chkShowAllSolutions.TabIndex = 3;
            this.chkShowAllSolutions.Text = "Show all";
            this.chkShowAllSolutions.UseVisualStyleBackColor = true;
            this.chkShowAllSolutions.CheckedChanged += new System.EventHandler(this.chkShowAllSolutions_CheckedChanged);
            // 
            // xrmSolution
            // 
            this.xrmSolution.AddNullRecord = true;
            this.xrmSolution.Column = null;
            this.xrmSolution.DisplayFormat = "{friendlyname}   ({P.friendlyname})";
            this.xrmSolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.xrmSolution.Filter = null;
            this.xrmSolution.FormattingEnabled = true;
            this.xrmSolution.Location = new System.Drawing.Point(12, 4);
            this.xrmSolution.Name = "xrmSolution";
            this.xrmSolution.OnlyActiveRecords = false;
            this.xrmSolution.RecordHost = null;
            this.xrmSolution.Service = null;
            this.xrmSolution.Size = new System.Drawing.Size(357, 21);
            this.xrmSolution.TabIndex = 1;
            this.xrmSolution.SelectedIndexChanged += new System.EventHandler(this.xrmSolution_SelectedIndexChanged);
            // 
            // panSolutions
            // 
            this.panSolutions.Controls.Add(this.rbSpecificPublisher);
            this.panSolutions.Controls.Add(this.rbSpecificSolution);
            this.panSolutions.Controls.Add(this.rbUnmanagedSolution);
            this.panSolutions.Location = new System.Drawing.Point(6, 19);
            this.panSolutions.Name = "panSolutions";
            this.panSolutions.Size = new System.Drawing.Size(465, 26);
            this.panSolutions.TabIndex = 0;
            // 
            // rbSpecificPublisher
            // 
            this.rbSpecificPublisher.AutoSize = true;
            this.rbSpecificPublisher.Location = new System.Drawing.Point(225, 3);
            this.rbSpecificPublisher.Name = "rbSpecificPublisher";
            this.rbSpecificPublisher.Size = new System.Drawing.Size(109, 17);
            this.rbSpecificPublisher.TabIndex = 3;
            this.rbSpecificPublisher.Text = "Specific Publisher";
            this.rbSpecificPublisher.UseVisualStyleBackColor = true;
            this.rbSpecificPublisher.CheckedChanged += new System.EventHandler(this.rbAllSolutions_CheckedChanged);
            // 
            // rbSpecificSolution
            // 
            this.rbSpecificSolution.AutoSize = true;
            this.rbSpecificSolution.Location = new System.Drawing.Point(115, 3);
            this.rbSpecificSolution.Name = "rbSpecificSolution";
            this.rbSpecificSolution.Size = new System.Drawing.Size(104, 17);
            this.rbSpecificSolution.TabIndex = 2;
            this.rbSpecificSolution.Text = "Specific Solution";
            this.rbSpecificSolution.UseVisualStyleBackColor = true;
            this.rbSpecificSolution.CheckedChanged += new System.EventHandler(this.rbAllSolutions_CheckedChanged);
            // 
            // rbUnmanagedSolution
            // 
            this.rbUnmanagedSolution.AutoSize = true;
            this.rbUnmanagedSolution.Checked = true;
            this.rbUnmanagedSolution.Location = new System.Drawing.Point(12, 3);
            this.rbUnmanagedSolution.Name = "rbUnmanagedSolution";
            this.rbUnmanagedSolution.Size = new System.Drawing.Size(97, 17);
            this.rbUnmanagedSolution.TabIndex = 1;
            this.rbUnmanagedSolution.TabStop = true;
            this.rbUnmanagedSolution.Text = "All Unmanaged";
            this.rbUnmanagedSolution.UseVisualStyleBackColor = true;
            this.rbUnmanagedSolution.CheckedChanged += new System.EventHandler(this.rbAllSolutions_CheckedChanged);
            // 
            // RefreshMetadataOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(477, 137);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblAttributes);
            this.Controls.Add(this.lblEntities);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RefreshMetadataOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Refresh Metadata filterd by Solution";
            this.Load += new System.EventHandler(this.ShowMetadataOptions_Load);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panSelectSolution.ResumeLayout(false);
            this.panSelectSolution.PerformLayout();
            this.panSolutions.ResumeLayout(false);
            this.panSolutions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblEntities;
        private System.Windows.Forms.Label lblAttributes;
        private System.Windows.Forms.GroupBox groupBox2;
        private Rappen.XTB.Helpers.Controls.XRMColumnLookup xrmSolution;
        private System.Windows.Forms.Panel panSolutions;
        private System.Windows.Forms.RadioButton rbSpecificSolution;
        private System.Windows.Forms.RadioButton rbUnmanagedSolution;
        private System.Windows.Forms.Panel panSelectSolution;
        private System.Windows.Forms.CheckBox chkShowAllSolutions;
        private System.Windows.Forms.RadioButton rbSpecificPublisher;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}