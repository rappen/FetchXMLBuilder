namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    partial class attributeControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.cmbAttribute = new System.Windows.Forms.ComboBox();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbAggregate = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkGroupBy = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbDateGrouping = new System.Windows.Forms.ComboBox();
            this.chkUserTZ = new System.Windows.Forms.CheckBox();
            this.chkDistinct = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Attribute name";
            // 
            // cmbAttribute
            // 
            this.cmbAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAttribute.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbAttribute.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAttribute.FormattingEnabled = true;
            this.cmbAttribute.Location = new System.Drawing.Point(7, 16);
            this.cmbAttribute.Name = "cmbAttribute";
            this.cmbAttribute.Size = new System.Drawing.Size(283, 21);
            this.cmbAttribute.Sorted = true;
            this.cmbAttribute.TabIndex = 27;
            this.cmbAttribute.Tag = "name|true";
            // 
            // txtAlias
            // 
            this.txtAlias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAlias.Location = new System.Drawing.Point(7, 56);
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(283, 20);
            this.txtAlias.TabIndex = 28;
            this.txtAlias.Tag = "alias";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Alias";
            // 
            // cmbAggregate
            // 
            this.cmbAggregate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAggregate.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbAggregate.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAggregate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAggregate.FormattingEnabled = true;
            this.cmbAggregate.Items.AddRange(new object[] {
            "",
            "avg",
            "count",
            "countcolumn",
            "max",
            "min",
            "sum"});
            this.cmbAggregate.Location = new System.Drawing.Point(7, 96);
            this.cmbAggregate.Name = "cmbAggregate";
            this.cmbAggregate.Size = new System.Drawing.Size(283, 21);
            this.cmbAggregate.Sorted = true;
            this.cmbAggregate.TabIndex = 30;
            this.cmbAggregate.Tag = "aggregate";
            this.cmbAggregate.SelectedIndexChanged += new System.EventHandler(this.cmbAggregate_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Aggregate";
            // 
            // chkGroupBy
            // 
            this.chkGroupBy.AutoSize = true;
            this.chkGroupBy.Location = new System.Drawing.Point(7, 124);
            this.chkGroupBy.Name = "chkGroupBy";
            this.chkGroupBy.Size = new System.Drawing.Size(69, 17);
            this.chkGroupBy.TabIndex = 33;
            this.chkGroupBy.Tag = "groupby";
            this.chkGroupBy.Text = "Group by";
            this.chkGroupBy.UseVisualStyleBackColor = true;
            this.chkGroupBy.CheckedChanged += new System.EventHandler(this.chkGroupBy_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 146);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Date grouping";
            // 
            // cmbDateGrouping
            // 
            this.cmbDateGrouping.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDateGrouping.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbDateGrouping.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbDateGrouping.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDateGrouping.Enabled = false;
            this.cmbDateGrouping.FormattingEnabled = true;
            this.cmbDateGrouping.Items.AddRange(new object[] {
            "",
            "day",
            "fiscal-period",
            "fiscal-year",
            "month",
            "quarter",
            "week",
            "year"});
            this.cmbDateGrouping.Location = new System.Drawing.Point(7, 160);
            this.cmbDateGrouping.Name = "cmbDateGrouping";
            this.cmbDateGrouping.Size = new System.Drawing.Size(283, 21);
            this.cmbDateGrouping.Sorted = true;
            this.cmbDateGrouping.TabIndex = 40;
            this.cmbDateGrouping.Tag = "dategrouping";
            // 
            // chkUserTZ
            // 
            this.chkUserTZ.AutoSize = true;
            this.chkUserTZ.Enabled = false;
            this.chkUserTZ.Location = new System.Drawing.Point(149, 124);
            this.chkUserTZ.Name = "chkUserTZ";
            this.chkUserTZ.Size = new System.Drawing.Size(96, 17);
            this.chkUserTZ.TabIndex = 37;
            this.chkUserTZ.Tag = "usertimezone";
            this.chkUserTZ.Text = "User time zone";
            this.chkUserTZ.UseVisualStyleBackColor = true;
            // 
            // chkDistinct
            // 
            this.chkDistinct.AutoSize = true;
            this.chkDistinct.Enabled = false;
            this.chkDistinct.Location = new System.Drawing.Point(82, 124);
            this.chkDistinct.Name = "chkDistinct";
            this.chkDistinct.Size = new System.Drawing.Size(61, 17);
            this.chkDistinct.TabIndex = 34;
            this.chkDistinct.Tag = "distinct";
            this.chkDistinct.Text = "Distinct";
            this.chkDistinct.UseVisualStyleBackColor = true;
            // 
            // attributeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkDistinct);
            this.Controls.Add(this.chkUserTZ);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cmbDateGrouping);
            this.Controls.Add(this.chkGroupBy);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbAggregate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAlias);
            this.Controls.Add(this.cmbAttribute);
            this.Controls.Add(this.label2);
            this.Name = "attributeControl";
            this.Size = new System.Drawing.Size(293, 207);
            this.Leave += new System.EventHandler(this.Control_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbAttribute;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbAggregate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkGroupBy;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbDateGrouping;
        private System.Windows.Forms.CheckBox chkUserTZ;
        private System.Windows.Forms.CheckBox chkDistinct;
    }
}
