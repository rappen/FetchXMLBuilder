namespace Rappen.XTB.FetchXmlBuilder.Controls
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
            this.components = new System.ComponentModel.Container();
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
            this.panOperator = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panValue = new System.Windows.Forms.Panel();
            this.panValueLookup = new System.Windows.Forms.Panel();
            this.btnLookup = new System.Windows.Forms.Button();
            this.txtLookup = new Rappen.XTB.Helpers.Controls.XRMColumnText();
            this.xrmRecord = new Rappen.XTB.Helpers.Controls.XRMRecordHost();
            this.label1 = new System.Windows.Forms.Label();
            this.panValueHint = new System.Windows.Forms.Panel();
            this.dlgLookup = new Rappen.XTB.Helpers.Controls.XRMLookupDialog();
            this.panUitype = new System.Windows.Forms.Panel();
            this.txtUitype = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtValueOf = new System.Windows.Forms.TextBox();
            this.panGuidSelector = new System.Windows.Forms.Panel();
            this.rbEnterGuid = new System.Windows.Forms.RadioButton();
            this.rbUseLookup = new System.Windows.Forms.RadioButton();
            this.panValueOf = new System.Windows.Forms.Panel();
            this.panValoeOfAttr = new System.Windows.Forms.Panel();
            this.cmbValueOf = new System.Windows.Forms.ComboBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panValueOfAlias = new System.Windows.Forms.Panel();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbValueOfAlias = new System.Windows.Forms.ComboBox();
            this.panAttrEntity = new System.Windows.Forms.Panel();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.panAttribute = new System.Windows.Forms.Panel();
            this.panAttr = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            this.panDateSelector = new System.Windows.Forms.Panel();
            this.rbDateText = new System.Windows.Forms.RadioButton();
            this.rbDatePicker = new System.Windows.Forms.RadioButton();
            this.panDatePicker = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panOperator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panValue.SuspendLayout();
            this.panValueLookup.SuspendLayout();
            this.panValueHint.SuspendLayout();
            this.panUitype.SuspendLayout();
            this.panGuidSelector.SuspendLayout();
            this.panValueOf.SuspendLayout();
            this.panValoeOfAttr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panValueOfAlias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.panAttrEntity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.panAttribute.SuspendLayout();
            this.panAttr.SuspendLayout();
            this.panDateSelector.SuspendLayout();
            this.panDatePicker.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 2);
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
            this.cmbAttribute.Location = new System.Drawing.Point(7, 16);
            this.cmbAttribute.Name = "cmbAttribute";
            this.cmbAttribute.Size = new System.Drawing.Size(177, 21);
            this.cmbAttribute.Sorted = true;
            this.cmbAttribute.TabIndex = 2;
            this.cmbAttribute.Tag = "attribute|true";
            this.cmbAttribute.SelectedIndexChanged += new System.EventHandler(this.cmbAttribute_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 2);
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
            this.cmbOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            "yesterday",
            "contains-values",
            "not-contains-values"});
            this.cmbOperator.Location = new System.Drawing.Point(7, 16);
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
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Link Entity";
            this.toolTip1.SetToolTip(this.label9, "Filters on link-entity");
            // 
            // cmbEntity
            // 
            this.cmbEntity.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbEntity.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbEntity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntity.FormattingEnabled = true;
            this.cmbEntity.Location = new System.Drawing.Point(7, 16);
            this.cmbEntity.Name = "cmbEntity";
            this.cmbEntity.Size = new System.Drawing.Size(95, 21);
            this.cmbEntity.TabIndex = 1;
            this.cmbEntity.Tag = "entityname";
            this.toolTip1.SetToolTip(this.cmbEntity, "Filters on link-entity");
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
            this.cmbValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbValue_KeyPress);
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
            // panOperator
            // 
            this.panOperator.Controls.Add(this.pictureBox2);
            this.panOperator.Controls.Add(this.cmbOperator);
            this.panOperator.Controls.Add(this.label4);
            this.panOperator.Dock = System.Windows.Forms.DockStyle.Top;
            this.panOperator.Location = new System.Drawing.Point(0, 40);
            this.panOperator.Name = "panOperator";
            this.panOperator.Size = new System.Drawing.Size(311, 40);
            this.panOperator.TabIndex = 2;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(52, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(14, 14);
            this.pictureBox2.TabIndex = 47;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Tag = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/fetchxml/fil" +
    "ter-rows#operator-parameters";
            this.pictureBox2.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(49, 2);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(14, 14);
            this.pictureBox4.TabIndex = 46;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Tag = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/fetchxml/fil" +
    "ter-rows";
            this.pictureBox4.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(84, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(14, 14);
            this.pictureBox1.TabIndex = 45;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/filter-ro" +
    "ws#filters-on-link-entity";
            this.pictureBox1.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // panValue
            // 
            this.panValue.Controls.Add(this.cmbValue);
            this.panValue.Controls.Add(this.label5);
            this.panValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.panValue.Location = new System.Drawing.Point(0, 80);
            this.panValue.Name = "panValue";
            this.panValue.Size = new System.Drawing.Size(311, 40);
            this.panValue.TabIndex = 3;
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
            this.panValueLookup.TabIndex = 5;
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
            this.txtLookup.Column = null;
            this.txtLookup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtLookup.DisplayFormat = "";
            this.txtLookup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline);
            this.txtLookup.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.txtLookup.Location = new System.Drawing.Point(7, 16);
            this.txtLookup.Name = "txtLookup";
            this.txtLookup.RecordHost = this.xrmRecord;
            this.txtLookup.Size = new System.Drawing.Size(254, 20);
            this.txtLookup.TabIndex = 32;
            this.txtLookup.Tag = "uiname";
            this.txtLookup.Click += new System.EventHandler(this.txtLookup_Click);
            // 
            // xrmRecord
            // 
            this.xrmRecord.Id = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.xrmRecord.LogicalName = null;
            this.xrmRecord.Record = null;
            this.xrmRecord.Service = null;
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
            this.panValueHint.Location = new System.Drawing.Point(0, 306);
            this.panValueHint.Name = "panValueHint";
            this.panValueHint.Size = new System.Drawing.Size(311, 32);
            this.panValueHint.TabIndex = 47;
            // 
            // dlgLookup
            // 
            this.dlgLookup.AdditionalViews = ((System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Microsoft.Xrm.Sdk.Entity>>)(resources.GetObject("dlgLookup.AdditionalViews")));
            this.dlgLookup.IncludePersonalViews = true;
            this.dlgLookup.LogicalName = "";
            this.dlgLookup.LogicalNames = null;
            this.dlgLookup.Record = null;
            this.dlgLookup.Service = null;
            this.dlgLookup.Title = null;
            // 
            // panUitype
            // 
            this.panUitype.Controls.Add(this.txtUitype);
            this.panUitype.Controls.Add(this.label3);
            this.panUitype.Dock = System.Windows.Forms.DockStyle.Top;
            this.panUitype.Location = new System.Drawing.Point(0, 338);
            this.panUitype.Name = "panUitype";
            this.panUitype.Size = new System.Drawing.Size(311, 33);
            this.panUitype.TabIndex = 48;
            this.panUitype.Visible = false;
            // 
            // txtUitype
            // 
            this.txtUitype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUitype.Location = new System.Drawing.Point(52, 0);
            this.txtUitype.Name = "txtUitype";
            this.txtUitype.Size = new System.Drawing.Size(236, 20);
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
            // txtValueOf
            // 
            this.txtValueOf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValueOf.BackColor = System.Drawing.SystemColors.Window;
            this.txtValueOf.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtValueOf.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtValueOf.Location = new System.Drawing.Point(9, 39);
            this.txtValueOf.Name = "txtValueOf";
            this.txtValueOf.ReadOnly = true;
            this.txtValueOf.Size = new System.Drawing.Size(177, 13);
            this.txtValueOf.TabIndex = 33;
            this.txtValueOf.Tag = "valueof";
            this.txtValueOf.TextChanged += new System.EventHandler(this.txtValueOf_TextChanged);
            // 
            // panGuidSelector
            // 
            this.panGuidSelector.Controls.Add(this.rbEnterGuid);
            this.panGuidSelector.Controls.Add(this.rbUseLookup);
            this.panGuidSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.panGuidSelector.Location = new System.Drawing.Point(0, 225);
            this.panGuidSelector.Name = "panGuidSelector";
            this.panGuidSelector.Size = new System.Drawing.Size(311, 25);
            this.panGuidSelector.TabIndex = 6;
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
            this.panValueOf.Controls.Add(this.panValoeOfAttr);
            this.panValueOf.Controls.Add(this.panValueOfAlias);
            this.panValueOf.Dock = System.Windows.Forms.DockStyle.Top;
            this.panValueOf.Location = new System.Drawing.Point(0, 250);
            this.panValueOf.Name = "panValueOf";
            this.panValueOf.Size = new System.Drawing.Size(311, 56);
            this.panValueOf.TabIndex = 8;
            // 
            // panValoeOfAttr
            // 
            this.panValoeOfAttr.Controls.Add(this.cmbValueOf);
            this.panValoeOfAttr.Controls.Add(this.txtValueOf);
            this.panValoeOfAttr.Controls.Add(this.pictureBox3);
            this.panValoeOfAttr.Controls.Add(this.label6);
            this.panValoeOfAttr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panValoeOfAttr.Location = new System.Drawing.Point(104, 0);
            this.panValoeOfAttr.Name = "panValoeOfAttr";
            this.panValoeOfAttr.Size = new System.Drawing.Size(207, 56);
            this.panValoeOfAttr.TabIndex = 2;
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
            this.cmbValueOf.Size = new System.Drawing.Size(177, 21);
            this.cmbValueOf.Sorted = true;
            this.cmbValueOf.TabIndex = 32;
            this.cmbValueOf.Tag = "";
            this.toolTip1.SetToolTip(this.cmbValueOf, "Filter on column values in the same row");
            this.cmbValueOf.SelectedIndexChanged += new System.EventHandler(this.cmbValueOf_Changed);
            this.cmbValueOf.TextUpdate += new System.EventHandler(this.cmbValueOf_Changed);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(52, 2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(14, 14);
            this.pictureBox3.TabIndex = 44;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/filter-ro" +
    "ws#filter-on-column-values-in-the-same-row";
            this.toolTip1.SetToolTip(this.pictureBox3, "MS Learn: Filter on column values in the same row");
            this.pictureBox3.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Value Of";
            this.toolTip1.SetToolTip(this.label6, "Filter on column values in the same row");
            // 
            // panValueOfAlias
            // 
            this.panValueOfAlias.Controls.Add(this.pictureBox5);
            this.panValueOfAlias.Controls.Add(this.label7);
            this.panValueOfAlias.Controls.Add(this.cmbValueOfAlias);
            this.panValueOfAlias.Dock = System.Windows.Forms.DockStyle.Left;
            this.panValueOfAlias.Location = new System.Drawing.Point(0, 0);
            this.panValueOfAlias.Name = "panValueOfAlias";
            this.panValueOfAlias.Size = new System.Drawing.Size(104, 56);
            this.panValueOfAlias.TabIndex = 1;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(56, 2);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(14, 14);
            this.pictureBox5.TabIndex = 47;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.Tag = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/fetchxml/fil" +
    "ter-rows#cross-table-comparisons";
            this.toolTip1.SetToolTip(this.pictureBox5, "MS Learn: Cross table comparisons");
            this.pictureBox5.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 2);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 46;
            this.label7.Text = "Link Alias";
            this.toolTip1.SetToolTip(this.label7, "Cross table comparisons");
            // 
            // cmbValueOfAlias
            // 
            this.cmbValueOfAlias.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbValueOfAlias.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbValueOfAlias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbValueOfAlias.FormattingEnabled = true;
            this.cmbValueOfAlias.Location = new System.Drawing.Point(7, 16);
            this.cmbValueOfAlias.Name = "cmbValueOfAlias";
            this.cmbValueOfAlias.Size = new System.Drawing.Size(95, 21);
            this.cmbValueOfAlias.TabIndex = 45;
            this.cmbValueOfAlias.Tag = "";
            this.toolTip1.SetToolTip(this.cmbValueOfAlias, "Cross table comparisons");
            this.cmbValueOfAlias.SelectedIndexChanged += new System.EventHandler(this.cmbValueOfAlias_SelectedIndexChanged);
            // 
            // panAttrEntity
            // 
            this.panAttrEntity.Controls.Add(this.pictureBox6);
            this.panAttrEntity.Controls.Add(this.cmbEntity);
            this.panAttrEntity.Controls.Add(this.label9);
            this.panAttrEntity.Controls.Add(this.pictureBox1);
            this.panAttrEntity.Dock = System.Windows.Forms.DockStyle.Left;
            this.panAttrEntity.Location = new System.Drawing.Point(0, 0);
            this.panAttrEntity.Name = "panAttrEntity";
            this.panAttrEntity.Size = new System.Drawing.Size(104, 40);
            this.panAttrEntity.TabIndex = 1;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
            this.pictureBox6.Location = new System.Drawing.Point(59, 2);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(14, 14);
            this.pictureBox6.TabIndex = 48;
            this.pictureBox6.TabStop = false;
            this.pictureBox6.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/filter-ro" +
    "ws#filters-on-link-entity";
            this.toolTip1.SetToolTip(this.pictureBox6, "MS Learn: Filters on link-entity");
            this.pictureBox6.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // panAttribute
            // 
            this.panAttribute.Controls.Add(this.cmbAttribute);
            this.panAttribute.Controls.Add(this.pictureBox4);
            this.panAttribute.Controls.Add(this.label2);
            this.panAttribute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panAttribute.Location = new System.Drawing.Point(104, 0);
            this.panAttribute.Name = "panAttribute";
            this.panAttribute.Size = new System.Drawing.Size(207, 40);
            this.panAttribute.TabIndex = 2;
            // 
            // panAttr
            // 
            this.panAttr.Controls.Add(this.panAttribute);
            this.panAttr.Controls.Add(this.panAttrEntity);
            this.panAttr.Dock = System.Windows.Forms.DockStyle.Top;
            this.panAttr.Location = new System.Drawing.Point(0, 0);
            this.panAttr.Name = "panAttr";
            this.panAttr.Size = new System.Drawing.Size(311, 40);
            this.panAttr.TabIndex = 1;
            // 
            // dtPicker
            // 
            this.dtPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtPicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPicker.Location = new System.Drawing.Point(7, 16);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(281, 20);
            this.dtPicker.TabIndex = 50;
            this.dtPicker.ValueChanged += new System.EventHandler(this.dtPicker_ValueChanged);
            // 
            // panDateSelector
            // 
            this.panDateSelector.Controls.Add(this.rbDateText);
            this.panDateSelector.Controls.Add(this.rbDatePicker);
            this.panDateSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.panDateSelector.Location = new System.Drawing.Point(0, 200);
            this.panDateSelector.Name = "panDateSelector";
            this.panDateSelector.Size = new System.Drawing.Size(311, 25);
            this.panDateSelector.TabIndex = 7;
            this.panDateSelector.Visible = false;
            // 
            // rbDateText
            // 
            this.rbDateText.AutoSize = true;
            this.rbDateText.Location = new System.Drawing.Point(108, 3);
            this.rbDateText.Name = "rbDateText";
            this.rbDateText.Size = new System.Drawing.Size(70, 17);
            this.rbDateText.TabIndex = 1;
            this.rbDateText.Text = "Enter text";
            this.rbDateText.UseVisualStyleBackColor = true;
            // 
            // rbDatePicker
            // 
            this.rbDatePicker.AutoSize = true;
            this.rbDatePicker.Checked = true;
            this.rbDatePicker.Location = new System.Drawing.Point(7, 3);
            this.rbDatePicker.Name = "rbDatePicker";
            this.rbDatePicker.Size = new System.Drawing.Size(100, 17);
            this.rbDatePicker.TabIndex = 0;
            this.rbDatePicker.TabStop = true;
            this.rbDatePicker.Text = "Use DatePicker";
            this.rbDatePicker.UseVisualStyleBackColor = true;
            this.rbDatePicker.CheckedChanged += new System.EventHandler(this.rbDatePicker_CheckedChanged);
            // 
            // panDatePicker
            // 
            this.panDatePicker.Controls.Add(this.dtPicker);
            this.panDatePicker.Controls.Add(this.linkLabel1);
            this.panDatePicker.Dock = System.Windows.Forms.DockStyle.Top;
            this.panDatePicker.Location = new System.Drawing.Point(0, 120);
            this.panDatePicker.Name = "panDatePicker";
            this.panDatePicker.Size = new System.Drawing.Size(311, 40);
            this.panDatePicker.TabIndex = 4;
            this.panDatePicker.Visible = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(13, 21);
            this.linkLabel1.Location = new System.Drawing.Point(4, 2);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(121, 17);
            this.linkLabel1.TabIndex = 51;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Tag = "https://wikipedia.org/wiki/ISO_8601";
            this.linkLabel1.Text = "Date / Time (ISO 8601)";
            this.linkLabel1.UseCompatibleTextRendering = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // conditionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panUitype);
            this.Controls.Add(this.panValueHint);
            this.Controls.Add(this.panValueOf);
            this.Controls.Add(this.panGuidSelector);
            this.Controls.Add(this.panDateSelector);
            this.Controls.Add(this.panValueLookup);
            this.Controls.Add(this.panDatePicker);
            this.Controls.Add(this.panValue);
            this.Controls.Add(this.panOperator);
            this.Controls.Add(this.panAttr);
            this.Name = "conditionControl";
            this.Size = new System.Drawing.Size(311, 523);
            this.panOperator.ResumeLayout(false);
            this.panOperator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
            this.panValoeOfAttr.ResumeLayout(false);
            this.panValoeOfAttr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panValueOfAlias.ResumeLayout(false);
            this.panValueOfAlias.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.panAttrEntity.ResumeLayout(false);
            this.panAttrEntity.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.panAttribute.ResumeLayout(false);
            this.panAttribute.PerformLayout();
            this.panAttr.ResumeLayout(false);
            this.panDateSelector.ResumeLayout(false);
            this.panDateSelector.PerformLayout();
            this.panDatePicker.ResumeLayout(false);
            this.panDatePicker.PerformLayout();
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
        private System.Windows.Forms.Panel panOperator;
        private System.Windows.Forms.Panel panValue;
        private System.Windows.Forms.Panel panValueLookup;
        private System.Windows.Forms.Panel panDatePicker;
        private System.Windows.Forms.Panel panGuidSelector;
        private System.Windows.Forms.Panel panDateSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLookup;
        private Rappen.XTB.Helpers.Controls.XRMRecordHost xrmRecord;
        private Rappen.XTB.Helpers.Controls.XRMColumnText txtLookup;
        private System.Windows.Forms.Panel panValueHint;
        private Rappen.XTB.Helpers.Controls.XRMLookupDialog dlgLookup;
        private System.Windows.Forms.Panel panUitype;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUitype;
        private System.Windows.Forms.RadioButton rbEnterGuid;
        private System.Windows.Forms.RadioButton rbUseLookup;
        private System.Windows.Forms.Panel panValueOf;
        private System.Windows.Forms.ComboBox cmbValueOf;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.ComboBox cmbValueOfAlias;
        private System.Windows.Forms.Panel panAttrEntity;
        private System.Windows.Forms.Panel panAttribute;
        private System.Windows.Forms.Panel panAttr;
        private System.Windows.Forms.Panel panValueOfAlias;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panValoeOfAttr;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txtValueOf;
        private System.Windows.Forms.DateTimePicker dtPicker;
        private System.Windows.Forms.RadioButton rbDateText;
        private System.Windows.Forms.RadioButton rbDatePicker;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
