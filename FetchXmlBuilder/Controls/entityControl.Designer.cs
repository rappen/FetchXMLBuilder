namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    partial class entityControl
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
            this.cmbEntity = new System.Windows.Forms.ComboBox();
            this.chkIncludeLogicalName = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.panFilter = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.picFilter = new System.Windows.Forms.PictureBox();
            this.panFilter.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 2);
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
            this.cmbEntity.Location = new System.Drawing.Point(7, 16);
            this.cmbEntity.Name = "cmbEntity";
            this.cmbEntity.Size = new System.Drawing.Size(236, 21);
            this.cmbEntity.Sorted = true;
            this.cmbEntity.TabIndex = 27;
            this.cmbEntity.Tag = "name|true";
            this.cmbEntity.SelectedIndexChanged += new System.EventHandler(this.cmbEntity_SelectedIndexChanged);
            // 
            // chkIncludeLogicalName
            // 
            this.chkIncludeLogicalName.AutoSize = true;
            this.chkIncludeLogicalName.Location = new System.Drawing.Point(7, 42);
            this.chkIncludeLogicalName.Margin = new System.Windows.Forms.Padding(2);
            this.chkIncludeLogicalName.Name = "chkIncludeLogicalName";
            this.chkIncludeLogicalName.Size = new System.Drawing.Size(144, 17);
            this.chkIncludeLogicalName.TabIndex = 28;
            this.chkIncludeLogicalName.Text = "Include LogicalName too";
            this.chkIncludeLogicalName.UseVisualStyleBackColor = true;
            this.chkIncludeLogicalName.CheckedChanged += new System.EventHandler(this.chkIncludeLogicalName_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Filter";
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(7, 16);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(2);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(236, 20);
            this.txtFilter.TabIndex = 30;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // panFilter
            // 
            this.panFilter.Controls.Add(this.txtFilter);
            this.panFilter.Controls.Add(this.label1);
            this.panFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panFilter.Location = new System.Drawing.Point(0, 0);
            this.panFilter.Margin = new System.Windows.Forms.Padding(2);
            this.panFilter.Name = "panFilter";
            this.panFilter.Size = new System.Drawing.Size(266, 40);
            this.panFilter.TabIndex = 31;
            this.panFilter.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cmbEntity);
            this.panel2.Controls.Add(this.chkIncludeLogicalName);
            this.panel2.Controls.Add(this.picFilter);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 40);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(266, 70);
            this.panel2.TabIndex = 32;
            // 
            // picFilter
            // 
            this.picFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picFilter.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_search_16;
            this.picFilter.Location = new System.Drawing.Point(224, 0);
            this.picFilter.Name = "picFilter";
            this.picFilter.Size = new System.Drawing.Size(19, 20);
            this.picFilter.TabIndex = 29;
            this.picFilter.TabStop = false;
            this.picFilter.Click += new System.EventHandler(this.picFilter_Click);
            // 
            // entityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panFilter);
            this.Name = "entityControl";
            this.Size = new System.Drawing.Size(266, 262);
            this.panFilter.ResumeLayout(false);
            this.panFilter.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbEntity;
        private System.Windows.Forms.CheckBox chkIncludeLogicalName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Panel panFilter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox picFilter;
    }
}
