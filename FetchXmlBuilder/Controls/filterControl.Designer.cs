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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(filterControl));
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.chkIsQF = new System.Windows.Forms.CheckBox();
            this.chkOverrideQFLimit = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.chkOverrideQFLimitBypass = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
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
            this.toolTip1.SetToolTip(this.label2, "And - needs all Conditions to return True (default)\r\nOr - needs any one Condition" +
        " return True\r\nhttps://learn.microsoft.com/power-apps/developer/data-platform/fet" +
        "chxml/filter-rows\r\n");
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
            this.cmbType.Size = new System.Drawing.Size(278, 21);
            this.cmbType.Sorted = true;
            this.cmbType.TabIndex = 1;
            this.cmbType.Tag = "type";
            this.toolTip1.SetToolTip(this.cmbType, "And - needs all Conditions to return True (default)\r\nOr - needs any one Condition" +
        " return True\r\nhttps://learn.microsoft.com/power-apps/developer/data-platform/fet" +
        "chxml/filter-rows\r\n");
            // 
            // chkIsQF
            // 
            this.chkIsQF.AutoSize = true;
            this.chkIsQF.Location = new System.Drawing.Point(7, 43);
            this.chkIsQF.Name = "chkIsQF";
            this.chkIsQF.Size = new System.Drawing.Size(88, 17);
            this.chkIsQF.TabIndex = 2;
            this.chkIsQF.Tag = "isquickfindfields";
            this.chkIsQF.Text = "Is Quick Find";
            this.toolTip1.SetToolTip(this.chkIsQF, "Read all about Quick Find!\r\nhttps://learn.microsoft.com/power-apps/developer/data" +
        "-platform/quick-find\r\n");
            this.chkIsQF.UseVisualStyleBackColor = true;
            this.chkIsQF.CheckedChanged += new System.EventHandler(this.chkIsQF_CheckedChanged);
            // 
            // chkOverrideQFLimit
            // 
            this.chkOverrideQFLimit.AutoSize = true;
            this.chkOverrideQFLimit.Location = new System.Drawing.Point(7, 66);
            this.chkOverrideQFLimit.Name = "chkOverrideQFLimit";
            this.chkOverrideQFLimit.Size = new System.Drawing.Size(172, 17);
            this.chkOverrideQFLimit.TabIndex = 3;
            this.chkOverrideQFLimit.Tag = "overridequickfindrecordlimitenabled";
            this.chkOverrideQFLimit.Text = "Apply the quick find record limit";
            this.toolTip1.SetToolTip(this.chkOverrideQFLimit, "Advanced feature about Quick Find. Read more:\r\nhttps://learn.microsoft.com/power-" +
        "apps/developer/data-platform/quick-find#apply-the-quick-find-record-limit\r\n");
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
            this.pictureBox1.Tag = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/quick-find";
            this.toolTip1.SetToolTip(this.pictureBox1, "Read all about Quick Find!\r\nhttps://learn.microsoft.com/power-apps/developer/data" +
        "-platform/quick-find");
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
            this.pictureBox2.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/filter-ro" +
    "ws";
            this.toolTip1.SetToolTip(this.pictureBox2, "And - needs all Conditions to return True (default)\r\nOr - needs any one Condition" +
        " return True\r\nhttps://learn.microsoft.com/power-apps/developer/data-platform/fet" +
        "chxml/filter-rows");
            this.pictureBox2.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "",
            "union"});
            this.comboBox1.Location = new System.Drawing.Point(7, 124);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(278, 21);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.Tag = "hint";
            this.toolTip1.SetToolTip(this.comboBox1, "Read more about Union Hint:\r\nhttps://learn.microsoft.com/power-apps/developer/dat" +
        "a-platform/fetchxml/optimize-performance#union-hint\r\n");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "Hint";
            this.toolTip1.SetToolTip(this.label1, "Read more about Union Hint:\r\nhttps://learn.microsoft.com/power-apps/developer/dat" +
        "a-platform/fetchxml/optimize-performance#union-hint\r\n");
            // 
            // pictureBox3
            // 
            this.pictureBox3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(31, 109);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(14, 14);
            this.pictureBox3.TabIndex = 50;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/optimize-" +
    "performance#union-hint";
            this.toolTip1.SetToolTip(this.pictureBox3, "Read more about Union Hint:\r\nhttps://learn.microsoft.com/power-apps/developer/dat" +
        "a-platform/fetchxml/optimize-performance#union-hint");
            this.pictureBox3.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // chkOverrideQFLimitBypass
            // 
            this.chkOverrideQFLimitBypass.AutoSize = true;
            this.chkOverrideQFLimitBypass.Location = new System.Drawing.Point(7, 89);
            this.chkOverrideQFLimitBypass.Name = "chkOverrideQFLimitBypass";
            this.chkOverrideQFLimitBypass.Size = new System.Drawing.Size(180, 17);
            this.chkOverrideQFLimitBypass.TabIndex = 4;
            this.chkOverrideQFLimitBypass.Tag = "overridequickfindrecordlimitdisabled";
            this.chkOverrideQFLimitBypass.Text = "Bypass the quick find record limit";
            this.toolTip1.SetToolTip(this.chkOverrideQFLimitBypass, "Advanced feature about Quick Find. Read more:\r\nhttps://learn.microsoft.com/power-" +
        "apps/developer/data-platform/quick-find#bypass-the-quick-find-record-limit\r\n");
            this.chkOverrideQFLimitBypass.UseVisualStyleBackColor = true;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(174, 68);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(14, 14);
            this.pictureBox4.TabIndex = 52;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/quick-find#apply-t" +
    "he-quick-find-record-limit";
            this.toolTip1.SetToolTip(this.pictureBox4, "Advanced feature about Quick Find. Read more:\r\nhttps://learn.microsoft.com/power-" +
        "apps/developer/data-platform/quick-find#apply-the-quick-find-record-limit");
            this.pictureBox4.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox5
            // 
            this.pictureBox5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(182, 91);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(14, 14);
            this.pictureBox5.TabIndex = 53;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/quick-find#bypass-" +
    "the-quick-find-record-limit";
            this.toolTip1.SetToolTip(this.pictureBox5, "Advanced feature about Quick Find. Read more:\r\nhttps://learn.microsoft.com/power-" +
        "apps/developer/data-platform/quick-find#bypass-the-quick-find-record-limit\r\n\r\n");
            this.pictureBox5.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // filterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.chkOverrideQFLimitBypass);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.chkOverrideQFLimit);
            this.Controls.Add(this.chkIsQF);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label2);
            this.Name = "filterControl";
            this.Size = new System.Drawing.Size(288, 230);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
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
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkOverrideQFLimitBypass;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
    }
}
