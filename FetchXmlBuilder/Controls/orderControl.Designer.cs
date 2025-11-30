namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    partial class orderControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(orderControl));
            this.label4 = new System.Windows.Forms.Label();
            this.chkDescending = new System.Windows.Forms.CheckBox();
            this.cmbAlias = new System.Windows.Forms.ComboBox();
            this.panAttr = new System.Windows.Forms.Panel();
            this.panAttribute = new System.Windows.Forms.Panel();
            this.cmbAttribute = new System.Windows.Forms.ComboBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panAttrEntity = new System.Windows.Forms.Panel();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.cmbEntity = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panAttr.SuspendLayout();
            this.panAttribute.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.panAttrEntity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
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
            // chkDescending
            // 
            this.chkDescending.AutoSize = true;
            this.chkDescending.Location = new System.Drawing.Point(7, 83);
            this.chkDescending.Name = "chkDescending";
            this.chkDescending.Size = new System.Drawing.Size(83, 17);
            this.chkDescending.TabIndex = 3;
            this.chkDescending.Tag = "descending";
            this.chkDescending.Text = "Descending";
            this.chkDescending.UseVisualStyleBackColor = true;
            // 
            // cmbAlias
            // 
            this.cmbAlias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAlias.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbAlias.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAlias.FormattingEnabled = true;
            this.cmbAlias.Location = new System.Drawing.Point(7, 56);
            this.cmbAlias.Name = "cmbAlias";
            this.cmbAlias.Size = new System.Drawing.Size(281, 21);
            this.cmbAlias.Sorted = true;
            this.cmbAlias.TabIndex = 2;
            this.cmbAlias.Tag = "alias";
            // 
            // panAttr
            // 
            this.panAttr.Controls.Add(this.panAttribute);
            this.panAttr.Controls.Add(this.panAttrEntity);
            this.panAttr.Dock = System.Windows.Forms.DockStyle.Top;
            this.panAttr.Location = new System.Drawing.Point(0, 0);
            this.panAttr.Name = "panAttr";
            this.panAttr.Size = new System.Drawing.Size(311, 40);
            this.panAttr.TabIndex = 30;
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
            this.cmbAttribute.Tag = "attribute";
            this.cmbAttribute.SelectedIndexChanged += new System.EventHandler(this.cmbAttribute_SelectedIndexChanged);
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
            this.cmbEntity.SelectedIndexChanged += new System.EventHandler(this.cmbEntity_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 2);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Link Entity";
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
            // 
            // orderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panAttr);
            this.Controls.Add(this.cmbAlias);
            this.Controls.Add(this.chkDescending);
            this.Controls.Add(this.label4);
            this.Name = "orderControl";
            this.Size = new System.Drawing.Size(311, 300);
            this.panAttr.ResumeLayout(false);
            this.panAttribute.ResumeLayout(false);
            this.panAttribute.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.panAttrEntity.ResumeLayout(false);
            this.panAttrEntity.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkDescending;
        private System.Windows.Forms.ComboBox cmbAlias;
        private System.Windows.Forms.Panel panAttr;
        private System.Windows.Forms.Panel panAttribute;
        private System.Windows.Forms.ComboBox cmbAttribute;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panAttrEntity;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.ComboBox cmbEntity;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
