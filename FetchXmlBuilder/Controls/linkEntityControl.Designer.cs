namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    partial class linkEntityControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(linkEntityControl));
            this.label2 = new System.Windows.Forms.Label();
            this.cmbEntity = new System.Windows.Forms.ComboBox();
            this.cmbFrom = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbTo = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.chkIntersect = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbRelationship = new System.Windows.Forms.ComboBox();
            this.rbAttrIdOnly = new System.Windows.Forms.RadioButton();
            this.rbAttrAll = new System.Windows.Forms.RadioButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Entity name";
            // 
            // cmbEntity
            // 
            this.cmbEntity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEntity.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbEntity.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbEntity.FormattingEnabled = true;
            this.cmbEntity.Location = new System.Drawing.Point(7, 56);
            this.cmbEntity.Name = "cmbEntity";
            this.cmbEntity.Size = new System.Drawing.Size(214, 21);
            this.cmbEntity.Sorted = true;
            this.cmbEntity.TabIndex = 2;
            this.cmbEntity.Tag = "name|true";
            this.cmbEntity.SelectedIndexChanged += new System.EventHandler(this.cmbEntity_SelectedIndexChanged);
            this.cmbEntity.Enter += new System.EventHandler(this.cmbEntity_Enter);
            // 
            // cmbFrom
            // 
            this.cmbFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFrom.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbFrom.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbFrom.FormattingEnabled = true;
            this.cmbFrom.Location = new System.Drawing.Point(7, 96);
            this.cmbFrom.Name = "cmbFrom";
            this.cmbFrom.Size = new System.Drawing.Size(214, 21);
            this.cmbFrom.Sorted = true;
            this.cmbFrom.TabIndex = 3;
            this.cmbFrom.Tag = "from|true";
            this.cmbFrom.Enter += new System.EventHandler(this.cmbEntity_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "From";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "To";
            // 
            // cmbTo
            // 
            this.cmbTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbTo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbTo.FormattingEnabled = true;
            this.cmbTo.Location = new System.Drawing.Point(7, 136);
            this.cmbTo.Name = "cmbTo";
            this.cmbTo.Size = new System.Drawing.Size(214, 21);
            this.cmbTo.Sorted = true;
            this.cmbTo.TabIndex = 4;
            this.cmbTo.Tag = "to|true";
            this.cmbTo.Enter += new System.EventHandler(this.cmbEntity_Enter);
            // 
            // comboBox2
            // 
            this.comboBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "",
            "inner",
            "outer"});
            this.comboBox2.Location = new System.Drawing.Point(7, 176);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(214, 21);
            this.comboBox2.Sorted = true;
            this.comboBox2.TabIndex = 5;
            this.comboBox2.Tag = "link-type";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 162);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Link type";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 202);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 35;
            this.label7.Tag = "alias";
            this.label7.Text = "Alias";
            // 
            // txtAlias
            // 
            this.txtAlias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAlias.Location = new System.Drawing.Point(7, 216);
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(214, 20);
            this.txtAlias.TabIndex = 6;
            this.txtAlias.Tag = "alias";
            // 
            // chkIntersect
            // 
            this.chkIntersect.AutoSize = true;
            this.chkIntersect.Location = new System.Drawing.Point(7, 244);
            this.chkIntersect.Name = "chkIntersect";
            this.chkIntersect.Size = new System.Drawing.Size(67, 17);
            this.chkIntersect.TabIndex = 7;
            this.chkIntersect.Tag = "intersect";
            this.chkIntersect.Text = "Intersect";
            this.chkIntersect.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(106, 244);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(56, 17);
            this.checkBox2.TabIndex = 8;
            this.checkBox2.Tag = "visible";
            this.checkBox2.Text = "Visible";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 2);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 13);
            this.label10.TabIndex = 39;
            this.label10.Text = "Relationship";
            // 
            // cmbRelationship
            // 
            this.cmbRelationship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRelationship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRelationship.FormattingEnabled = true;
            this.cmbRelationship.Location = new System.Drawing.Point(7, 16);
            this.cmbRelationship.Name = "cmbRelationship";
            this.cmbRelationship.Size = new System.Drawing.Size(214, 21);
            this.cmbRelationship.TabIndex = 1;
            this.cmbRelationship.DropDown += new System.EventHandler(this.cmbRelationship_DropDown);
            this.cmbRelationship.SelectedIndexChanged += new System.EventHandler(this.cmbRelationship_SelectedIndexChanged);
            this.cmbRelationship.DropDownClosed += new System.EventHandler(this.cmbRelationship_DropDownClosed);
            // 
            // rbAttrIdOnly
            // 
            this.rbAttrIdOnly.AutoSize = true;
            this.rbAttrIdOnly.Checked = true;
            this.rbAttrIdOnly.Location = new System.Drawing.Point(106, 267);
            this.rbAttrIdOnly.Name = "rbAttrIdOnly";
            this.rbAttrIdOnly.Size = new System.Drawing.Size(103, 17);
            this.rbAttrIdOnly.TabIndex = 40;
            this.rbAttrIdOnly.TabStop = true;
            this.rbAttrIdOnly.Text = "Id / Lookup only";
            this.rbAttrIdOnly.UseVisualStyleBackColor = true;
            // 
            // rbAttrAll
            // 
            this.rbAttrAll.AutoSize = true;
            this.rbAttrAll.Location = new System.Drawing.Point(7, 267);
            this.rbAttrAll.Name = "rbAttrAll";
            this.rbAttrAll.Size = new System.Drawing.Size(82, 17);
            this.rbAttrAll.TabIndex = 41;
            this.rbAttrAll.Text = "All attributes";
            this.rbAttrAll.UseVisualStyleBackColor = true;
            this.rbAttrAll.CheckedChanged += new System.EventHandler(this.rbAttr_CheckedChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(69, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(14, 14);
            this.pictureBox2.TabIndex = 48;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Tag = "https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.query.linkentity";
            this.pictureBox2.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(54, 162);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(14, 14);
            this.pictureBox1.TabIndex = 49;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.query.joinoperator";
            this.pictureBox1.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // linkEntityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.rbAttrAll);
            this.Controls.Add(this.rbAttrIdOnly);
            this.Controls.Add(this.cmbRelationship);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.chkIntersect);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtAlias);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbTo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbFrom);
            this.Controls.Add(this.cmbEntity);
            this.Controls.Add(this.label2);
            this.Name = "linkEntityControl";
            this.Size = new System.Drawing.Size(247, 304);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbEntity;
        private System.Windows.Forms.ComboBox cmbFrom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbTo;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.CheckBox chkIntersect;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbRelationship;
        private System.Windows.Forms.RadioButton rbAttrIdOnly;
        private System.Windows.Forms.RadioButton rbAttrAll;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
