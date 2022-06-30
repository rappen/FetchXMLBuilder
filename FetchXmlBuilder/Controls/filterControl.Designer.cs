namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    partial class filterControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(filterControl));
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.chkIsQF = new System.Windows.Forms.CheckBox();
            this.chkOverrideQFLimit = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Filter type";
            // 
            // cmbType
            // 
            this.cmbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "",
            "and",
            "or"});
            this.cmbType.Location = new System.Drawing.Point(7, 16);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(247, 21);
            this.cmbType.Sorted = true;
            this.cmbType.TabIndex = 27;
            this.cmbType.Tag = "type";
            // 
            // chkIsQF
            // 
            this.chkIsQF.AutoSize = true;
            this.chkIsQF.Location = new System.Drawing.Point(7, 43);
            this.chkIsQF.Name = "chkIsQF";
            this.chkIsQF.Size = new System.Drawing.Size(88, 17);
            this.chkIsQF.TabIndex = 28;
            this.chkIsQF.Tag = "isquickfindfields";
            this.chkIsQF.Text = "Is Quick Find";
            this.chkIsQF.UseVisualStyleBackColor = true;
            this.chkIsQF.CheckedChanged += new System.EventHandler(this.chkIsQF_CheckedChanged);
            // 
            // chkOverrideQFLimit
            // 
            this.chkOverrideQFLimit.AutoSize = true;
            this.chkOverrideQFLimit.Location = new System.Drawing.Point(7, 66);
            this.chkOverrideQFLimit.Name = "chkOverrideQFLimit";
            this.chkOverrideQFLimit.Size = new System.Drawing.Size(128, 17);
            this.chkOverrideQFLimit.TabIndex = 29;
            this.chkOverrideQFLimit.Tag = "overridequickfindrecordlimitenabled";
            this.chkOverrideQFLimit.Text = "Override Record Limit";
            this.chkOverrideQFLimit.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(91, 45);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(14, 14);
            this.pictureBox1.TabIndex = 46;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/quick-fi" +
    "nd-limit";
            this.pictureBox1.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(56, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(14, 14);
            this.pictureBox2.TabIndex = 47;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Tag = "https://docs.microsoft.com/en-us/powerapps/developer/data-platform/org-service/us" +
    "e-filterexpression-class";
            this.pictureBox2.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // filterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.chkOverrideQFLimit);
            this.Controls.Add(this.chkIsQF);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label2);
            this.Name = "filterControl";
            this.Size = new System.Drawing.Size(257, 95);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.CheckBox chkIsQF;
        private System.Windows.Forms.CheckBox chkOverrideQFLimit;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
