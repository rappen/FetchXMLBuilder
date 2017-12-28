namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    partial class conditionControl
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
            this.label4 = new System.Windows.Forms.Label();
            this.cmbOperator = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbEntity = new System.Windows.Forms.ComboBox();
            this.btnGetGuid = new System.Windows.Forms.Button();
            this.cmbValue = new System.Windows.Forms.ComboBox();
            this.lblValueHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Attribute";
            // 
            // cmbAttribute
            // 
            this.cmbAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAttribute.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbAttribute.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAttribute.FormattingEnabled = true;
            this.cmbAttribute.Location = new System.Drawing.Point(7, 56);
            this.cmbAttribute.Name = "cmbAttribute";
            this.cmbAttribute.Size = new System.Drawing.Size(301, 21);
            this.cmbAttribute.Sorted = true;
            this.cmbAttribute.TabIndex = 2;
            this.cmbAttribute.Tag = "attribute|true";
            this.cmbAttribute.SelectedIndexChanged += new System.EventHandler(this.cmbAttribute_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Operator";
            // 
            // cmbOperator
            // 
            this.cmbOperator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbOperator.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbOperator.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbOperator.FormattingEnabled = true;
            this.cmbOperator.Items.AddRange(new object[] {
            "begins-with",
            "between",
            "ends-with",
            "eq",
            "eq-businessid",
            "eq-userid",
            "eq-userlanguage",
            "eq-useroruserteams",
            "eq-userteams",
            "ge",
            "gt",
            "in",
            "in-fiscal-period",
            "in-fiscal-period-and-year",
            "in-fiscal-year",
            "in-or-after-fiscal-period-and-year",
            "in-or-before-fiscal-period-and-year",
            "last-fiscal-period",
            "last-fiscal-year",
            "last-month",
            "last-seven-days",
            "last-week",
            "last-x-days",
            "last-x-fiscal-periods",
            "last-x-fiscal-years",
            "last-x-hours",
            "last-x-months",
            "last-x-weeks",
            "last-x-years",
            "last-year",
            "le",
            "like",
            "lt",
            "ne",
            "ne-businessid",
            "ne-userid",
            "neq",
            "next-fiscal-period",
            "next-fiscal-year",
            "next-month",
            "next-seven-days",
            "next-week",
            "next-x-days",
            "next-x-fiscal-periods",
            "next-x-fiscal-years",
            "next-x-hours",
            "next-x-months",
            "next-x-weeks",
            "next-x-years",
            "next-year",
            "not-begin-with",
            "not-between",
            "not-end-with",
            "not-in",
            "not-like",
            "not-null",
            "null",
            "olderthan-x-months",
            "on",
            "on-or-after",
            "on-or-before",
            "this-fiscal-period",
            "this-fiscal-year",
            "this-month",
            "this-week",
            "this-year",
            "today",
            "tomorrow",
            "yesterday"});
            this.cmbOperator.Location = new System.Drawing.Point(7, 96);
            this.cmbOperator.Name = "cmbOperator";
            this.cmbOperator.Size = new System.Drawing.Size(301, 21);
            this.cmbOperator.TabIndex = 3;
            this.cmbOperator.Tag = "operator|true";
            this.cmbOperator.SelectedIndexChanged += new System.EventHandler(this.cmbOperator_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Value";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 2);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Entity";
            // 
            // cmbEntity
            // 
            this.cmbEntity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEntity.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbEntity.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbEntity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntity.FormattingEnabled = true;
            this.cmbEntity.Location = new System.Drawing.Point(7, 16);
            this.cmbEntity.Name = "cmbEntity";
            this.cmbEntity.Size = new System.Drawing.Size(301, 21);
            this.cmbEntity.TabIndex = 1;
            this.cmbEntity.Tag = "entityname";
            this.cmbEntity.SelectedIndexChanged += new System.EventHandler(this.cmbEtity_SelectedIndexChanged);
            // 
            // btnGetGuid
            // 
            this.btnGetGuid.Location = new System.Drawing.Point(7, 163);
            this.btnGetGuid.Name = "btnGetGuid";
            this.btnGetGuid.Size = new System.Drawing.Size(95, 22);
            this.btnGetGuid.TabIndex = 40;
            this.btnGetGuid.Text = "Generate Guid";
            this.btnGetGuid.UseVisualStyleBackColor = true;
            this.btnGetGuid.Visible = false;
            this.btnGetGuid.Click += new System.EventHandler(this.btnGetGuid_Click);
            // 
            // cmbValue
            // 
            this.cmbValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbValue.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbValue.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbValue.FormattingEnabled = true;
            this.cmbValue.Location = new System.Drawing.Point(7, 136);
            this.cmbValue.Name = "cmbValue";
            this.cmbValue.Size = new System.Drawing.Size(301, 21);
            this.cmbValue.Sorted = true;
            this.cmbValue.TabIndex = 4;
            this.cmbValue.Tag = "value";
            // 
            // lblValueHint
            // 
            this.lblValueHint.AutoSize = true;
            this.lblValueHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValueHint.Location = new System.Drawing.Point(4, 168);
            this.lblValueHint.Name = "lblValueHint";
            this.lblValueHint.Size = new System.Drawing.Size(53, 13);
            this.lblValueHint.TabIndex = 41;
            this.lblValueHint.Text = "ValueHint";
            this.lblValueHint.Visible = false;
            // 
            // conditionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblValueHint);
            this.Controls.Add(this.cmbValue);
            this.Controls.Add(this.btnGetGuid);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmbEntity);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbOperator);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbAttribute);
            this.Controls.Add(this.label2);
            this.Name = "conditionControl";
            this.Size = new System.Drawing.Size(311, 194);
            this.Leave += new System.EventHandler(this.Control_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbAttribute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbOperator;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbEntity;
        private System.Windows.Forms.Button btnGetGuid;
        private System.Windows.Forms.ComboBox cmbValue;
        private System.Windows.Forms.Label lblValueHint;
    }
}
