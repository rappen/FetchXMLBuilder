namespace Cinteros.Xrm.FetchXmlBuilder.Controls
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
            this.components = new System.ComponentModel.Container();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbAttribute = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkDescending = new System.Windows.Forms.CheckBox();
            this.cmbAlias = new System.Windows.Forms.ComboBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
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
            this.cmbAttribute.Size = new System.Drawing.Size(194, 21);
            this.cmbAttribute.Sorted = true;
            this.cmbAttribute.TabIndex = 1;
            this.cmbAttribute.Tag = "attribute";
            this.cmbAttribute.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAttribute_Validating);
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
            this.cmbAlias.Size = new System.Drawing.Size(194, 21);
            this.cmbAlias.Sorted = true;
            this.cmbAlias.TabIndex = 2;
            this.cmbAlias.Tag = "alias";
            this.cmbAlias.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAlias_Validating);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // orderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbAlias);
            this.Controls.Add(this.chkDescending);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbAttribute);
            this.Controls.Add(this.label2);
            this.Name = "orderControl";
            this.Size = new System.Drawing.Size(224, 115);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbAttribute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkDescending;
        private System.Windows.Forms.ComboBox cmbAlias;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
