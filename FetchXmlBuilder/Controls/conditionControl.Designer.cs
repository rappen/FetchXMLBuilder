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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(conditionControl));
            this.label2 = new System.Windows.Forms.Label();
            this.cmbAttribute = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbOperator = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbEntity = new System.Windows.Forms.ComboBox();
            this.cmbValue = new System.Windows.Forms.ComboBox();
            this.lblValueHint = new System.Windows.Forms.Label();
            this.panAttribte = new System.Windows.Forms.Panel();
            this.panValue = new System.Windows.Forms.Panel();
            this.panValueLookup = new System.Windows.Forms.Panel();
            this.btnLookup = new System.Windows.Forms.Button();
            this.txtLookup = new xrmtb.XrmToolBox.Controls.Controls.CDSDataTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panValueHint = new System.Windows.Forms.Panel();
            this.dlgLookup = new xrmtb.XrmToolBox.Controls.Controls.CDSLookupDialog();
            this.panUitype = new System.Windows.Forms.Panel();
            this.txtUitype = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panGuidSelector = new System.Windows.Forms.Panel();
            this.rbEnterGuid = new System.Windows.Forms.RadioButton();
            this.rbUseLookup = new System.Windows.Forms.RadioButton();
            this.panValueOf = new System.Windows.Forms.Panel();
            this.cmbValueOf = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panAttribte.SuspendLayout();
            this.panValue.SuspendLayout();
            this.panValueLookup.SuspendLayout();
            this.panValueHint.SuspendLayout();
            this.panUitype.SuspendLayout();
            this.panGuidSelector.SuspendLayout();
            this.panValueOf.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
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
            this.cmbAttribute.Size = new System.Drawing.Size(281, 21);
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
            this.cmbOperator.Size = new System.Drawing.Size(281, 21);
            this.cmbOperator.TabIndex = 3;
            this.cmbOperator.Tag = "operator|true";
            this.cmbOperator.SelectedIndexChanged += new System.EventHandler(this.cmbOperator_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 2);
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
            this.cmbEntity.Size = new System.Drawing.Size(281, 21);
            this.cmbEntity.TabIndex = 1;
            this.cmbEntity.Tag = "entityname";
            this.cmbEntity.SelectedIndexChanged += new System.EventHandler(this.cmbEntity_SelectedIndexChanged);
            // 
            // cmbValue
            // 
            this.cmbValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbValue.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbValue.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbValue.FormattingEnabled = true;
            this.cmbValue.Location = new System.Drawing.Point(7, 16);
            this.cmbValue.Name = "cmbValue";
            this.cmbValue.Size = new System.Drawing.Size(281, 21);
            this.cmbValue.Sorted = true;
            this.cmbValue.TabIndex = 4;
            this.cmbValue.Tag = "value";
            // 
            // lblValueHint
            // 
            this.lblValueHint.AutoSize = true;
            this.lblValueHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValueHint.Location = new System.Drawing.Point(4, 8);
            this.lblValueHint.Name = "lblValueHint";
            this.lblValueHint.Size = new System.Drawing.Size(53, 13);
            this.lblValueHint.TabIndex = 41;
            this.lblValueHint.Text = "ValueHint";
            this.lblValueHint.Visible = false;
            // 
            // panAttribte
            // 
            this.panAttribte.Controls.Add(this.cmbOperator);
            this.panAttribte.Controls.Add(this.label2);
            this.panAttribte.Controls.Add(this.cmbAttribute);
            this.panAttribte.Controls.Add(this.label4);
            this.panAttribte.Controls.Add(this.cmbEntity);
            this.panAttribte.Controls.Add(this.label9);
            this.panAttribte.Dock = System.Windows.Forms.DockStyle.Top;
            this.panAttribte.Location = new System.Drawing.Point(0, 0);
            this.panAttribte.Name = "panAttribte";
            this.panAttribte.Size = new System.Drawing.Size(311, 120);
            this.panAttribte.TabIndex = 43;
            // 
            // panValue
            // 
            this.panValue.Controls.Add(this.cmbValue);
            this.panValue.Controls.Add(this.label5);
            this.panValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.panValue.Location = new System.Drawing.Point(0, 120);
            this.panValue.Name = "panValue";
            this.panValue.Size = new System.Drawing.Size(311, 40);
            this.panValue.TabIndex = 44;
            // 
            // panValueLookup
            // 
            this.panValueLookup.Controls.Add(this.btnLookup);
            this.panValueLookup.Controls.Add(this.txtLookup);
            this.panValueLookup.Controls.Add(this.label1);
            this.panValueLookup.Dock = System.Windows.Forms.DockStyle.Top;
            this.panValueLookup.Location = new System.Drawing.Point(0, 160);
            this.panValueLookup.Name = "panValueLookup";
            this.panValueLookup.Size = new System.Drawing.Size(311, 40);
            this.panValueLookup.TabIndex = 45;
            this.panValueLookup.Visible = false;
            // 
            // btnLookup
            // 
            this.btnLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLookup.Image = ((System.Drawing.Image)(resources.GetObject("btnLookup.Image")));
            this.btnLookup.Location = new System.Drawing.Point(265, 15);
            this.btnLookup.Name = "btnLookup";
            this.btnLookup.Size = new System.Drawing.Size(23, 23);
            this.btnLookup.TabIndex = 33;
            this.btnLookup.UseVisualStyleBackColor = true;
            this.btnLookup.Click += new System.EventHandler(this.btnLookup_Click);
            // 
            // txtLookup
            // 
            this.txtLookup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLookup.BackColor = System.Drawing.SystemColors.Window;
            this.txtLookup.Clickable = true;
            this.txtLookup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtLookup.DisplayFormat = "";
            this.txtLookup.Entity = null;
            this.txtLookup.EntityReference = null;
            this.txtLookup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline);
            this.txtLookup.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.txtLookup.Id = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.txtLookup.Location = new System.Drawing.Point(7, 16);
            this.txtLookup.LogicalName = null;
            this.txtLookup.Name = "txtLookup";
            this.txtLookup.OrganizationService = null;
            this.txtLookup.Size = new System.Drawing.Size(254, 20);
            this.txtLookup.TabIndex = 32;
            this.txtLookup.Tag = "uiname";
            this.txtLookup.RecordClick += new xrmtb.XrmToolBox.Controls.CDSRecordEventHandler(this.txtLookup_RecordClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Record";
            // 
            // panValueHint
            // 
            this.panValueHint.Controls.Add(this.lblValueHint);
            this.panValueHint.Dock = System.Windows.Forms.DockStyle.Top;
            this.panValueHint.Location = new System.Drawing.Point(0, 225);
            this.panValueHint.Name = "panValueHint";
            this.panValueHint.Size = new System.Drawing.Size(311, 32);
            this.panValueHint.TabIndex = 47;
            // 
            // dlgLookup
            // 
            this.dlgLookup.Entity = null;
            this.dlgLookup.IncludePersonalViews = true;
            this.dlgLookup.LogicalName = "";
            this.dlgLookup.LogicalNames = null;
            this.dlgLookup.Service = null;
            this.dlgLookup.Title = null;
            // 
            // panUitype
            // 
            this.panUitype.Controls.Add(this.txtUitype);
            this.panUitype.Controls.Add(this.label3);
            this.panUitype.Dock = System.Windows.Forms.DockStyle.Top;
            this.panUitype.Location = new System.Drawing.Point(0, 257);
            this.panUitype.Name = "panUitype";
            this.panUitype.Size = new System.Drawing.Size(311, 46);
            this.panUitype.TabIndex = 48;
            this.panUitype.Visible = false;
            // 
            // txtUitype
            // 
            this.txtUitype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUitype.Location = new System.Drawing.Point(7, 16);
            this.txtUitype.Name = "txtUitype";
            this.txtUitype.Size = new System.Drawing.Size(281, 20);
            this.txtUitype.TabIndex = 32;
            this.txtUitype.Tag = "uitype";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "UI Type";
            // 
            // panGuidSelector
            // 
            this.panGuidSelector.Controls.Add(this.rbEnterGuid);
            this.panGuidSelector.Controls.Add(this.rbUseLookup);
            this.panGuidSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.panGuidSelector.Location = new System.Drawing.Point(0, 200);
            this.panGuidSelector.Name = "panGuidSelector";
            this.panGuidSelector.Size = new System.Drawing.Size(311, 25);
            this.panGuidSelector.TabIndex = 49;
            this.panGuidSelector.Visible = false;
            // 
            // rbEnterGuid
            // 
            this.rbEnterGuid.AutoSize = true;
            this.rbEnterGuid.Location = new System.Drawing.Point(108, 3);
            this.rbEnterGuid.Name = "rbEnterGuid";
            this.rbEnterGuid.Size = new System.Drawing.Size(75, 17);
            this.rbEnterGuid.TabIndex = 1;
            this.rbEnterGuid.Text = "Enter Guid";
            this.rbEnterGuid.UseVisualStyleBackColor = true;
            // 
            // rbUseLookup
            // 
            this.rbUseLookup.AutoSize = true;
            this.rbUseLookup.Checked = true;
            this.rbUseLookup.Location = new System.Drawing.Point(7, 3);
            this.rbUseLookup.Name = "rbUseLookup";
            this.rbUseLookup.Size = new System.Drawing.Size(83, 17);
            this.rbUseLookup.TabIndex = 0;
            this.rbUseLookup.TabStop = true;
            this.rbUseLookup.Text = "Use Lookup";
            this.rbUseLookup.UseVisualStyleBackColor = true;
            this.rbUseLookup.CheckedChanged += new System.EventHandler(this.rbUseLookup_CheckedChanged);
            // 
            // panValueOf
            // 
            this.panValueOf.Controls.Add(this.cmbValueOf);
            this.panValueOf.Controls.Add(this.label6);
            this.panValueOf.Dock = System.Windows.Forms.DockStyle.Top;
            this.panValueOf.Location = new System.Drawing.Point(0, 303);
            this.panValueOf.Name = "panValueOf";
            this.panValueOf.Size = new System.Drawing.Size(311, 40);
            this.panValueOf.TabIndex = 50;
            // 
            // cmbValueOf
            // 
            this.cmbValueOf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbValueOf.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbValueOf.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbValueOf.FormattingEnabled = true;
            this.cmbValueOf.Location = new System.Drawing.Point(7, 16);
            this.cmbValueOf.Name = "cmbValueOf";
            this.cmbValueOf.Size = new System.Drawing.Size(281, 21);
            this.cmbValueOf.Sorted = true;
            this.cmbValueOf.TabIndex = 32;
            this.cmbValueOf.Tag = "valueof";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Value Of";
            // 
            // conditionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panUitype);
            this.Controls.Add(this.panValueHint);
            this.Controls.Add(this.panValueOf);
            this.Controls.Add(this.panGuidSelector);
            this.Controls.Add(this.panValueLookup);
            this.Controls.Add(this.panValue);
            this.Controls.Add(this.panAttribte);
            this.Name = "conditionControl";
            this.Size = new System.Drawing.Size(311, 258);
            this.panAttribte.ResumeLayout(false);
            this.panAttribte.PerformLayout();
            this.panValue.ResumeLayout(false);
            this.panValue.PerformLayout();
            this.panValueLookup.ResumeLayout(false);
            this.panValueLookup.PerformLayout();
            this.panValueHint.ResumeLayout(false);
            this.panValueHint.PerformLayout();
            this.panUitype.ResumeLayout(false);
            this.panUitype.PerformLayout();
            this.panGuidSelector.ResumeLayout(false);
            this.panGuidSelector.PerformLayout();
            this.panValueOf.ResumeLayout(false);
            this.panValueOf.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbAttribute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbOperator;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbEntity;
        private System.Windows.Forms.ComboBox cmbValue;
        private System.Windows.Forms.Label lblValueHint;
        private System.Windows.Forms.Panel panAttribte;
        private System.Windows.Forms.Panel panValue;
        private System.Windows.Forms.Panel panValueLookup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLookup;
        private xrmtb.XrmToolBox.Controls.Controls.CDSDataTextBox txtLookup;
        private System.Windows.Forms.Panel panValueHint;
        private xrmtb.XrmToolBox.Controls.Controls.CDSLookupDialog dlgLookup;
        private System.Windows.Forms.Panel panUitype;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUitype;
        private System.Windows.Forms.Panel panGuidSelector;
        private System.Windows.Forms.RadioButton rbEnterGuid;
        private System.Windows.Forms.RadioButton rbUseLookup;
        private System.Windows.Forms.Panel panValueOf;
        private System.Windows.Forms.ComboBox cmbValueOf;
        private System.Windows.Forms.Label label6;
    }
}
