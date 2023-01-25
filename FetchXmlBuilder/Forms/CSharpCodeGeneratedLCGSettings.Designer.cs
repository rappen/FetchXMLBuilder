namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    partial class CSharpCodeGeneratedLCGSettings
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSharpCodeGeneratedLCGSettings));
            this.chkConstCamelCased = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbConstantName = new System.Windows.Forms.ComboBox();
            this.txtConstStripPrefix = new System.Windows.Forms.TextBox();
            this.chkConstStripPrefix = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkConstCamelCased
            // 
            this.chkConstCamelCased.AutoSize = true;
            this.chkConstCamelCased.Location = new System.Drawing.Point(299, 105);
            this.chkConstCamelCased.Name = "chkConstCamelCased";
            this.chkConstCamelCased.Size = new System.Drawing.Size(85, 17);
            this.chkConstCamelCased.TabIndex = 64;
            this.chkConstCamelCased.Text = "CamelCased";
            this.chkConstCamelCased.UseVisualStyleBackColor = true;
            this.chkConstCamelCased.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(27, 106);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 61;
            this.label8.Text = "Identifier";
            // 
            // cmbConstantName
            // 
            this.cmbConstantName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConstantName.FormattingEnabled = true;
            this.cmbConstantName.Items.AddRange(new object[] {
            "Display Name",
            "Schema Name",
            "Logical Name"});
            this.cmbConstantName.Location = new System.Drawing.Point(135, 102);
            this.cmbConstantName.Name = "cmbConstantName";
            this.cmbConstantName.Size = new System.Drawing.Size(158, 21);
            this.cmbConstantName.TabIndex = 63;
            this.cmbConstantName.SelectedIndexChanged += new System.EventHandler(this.cmbConstantName_SelectedIndexChanged);
            // 
            // txtConstStripPrefix
            // 
            this.txtConstStripPrefix.Enabled = false;
            this.txtConstStripPrefix.Location = new System.Drawing.Point(221, 130);
            this.txtConstStripPrefix.Name = "txtConstStripPrefix";
            this.txtConstStripPrefix.Size = new System.Drawing.Size(213, 20);
            this.txtConstStripPrefix.TabIndex = 66;
            this.txtConstStripPrefix.Leave += new System.EventHandler(this.txtConstStripPrefix_Leave);
            // 
            // chkConstStripPrefix
            // 
            this.chkConstStripPrefix.AutoSize = true;
            this.chkConstStripPrefix.Enabled = false;
            this.chkConstStripPrefix.Location = new System.Drawing.Point(135, 132);
            this.chkConstStripPrefix.Name = "chkConstStripPrefix";
            this.chkConstStripPrefix.Size = new System.Drawing.Size(89, 17);
            this.chkConstStripPrefix.TabIndex = 65;
            this.chkConstStripPrefix.Text = "Skip prefixes:";
            this.chkConstStripPrefix.UseVisualStyleBackColor = true;
            this.chkConstStripPrefix.CheckedChanged += new System.EventHandler(this.chkConstStripPrefix_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(27, 133);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(33, 13);
            this.label10.TabIndex = 62;
            this.label10.Text = "Prefix";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(447, 63);
            this.label1.TabIndex = 67;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(135, 187);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(299, 23);
            this.btnOpenFile.TabIndex = 68;
            this.btnOpenFile.Text = "Open existing LCG-constants (or project)";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(268, 250);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(97, 23);
            this.btnOK.TabIndex = 141;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(135, 250);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(97, 23);
            this.btnCancel.TabIndex = 142;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // CSharpCodeGeneratedLCGSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(471, 295);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkConstCamelCased);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cmbConstantName);
            this.Controls.Add(this.txtConstStripPrefix);
            this.Controls.Add(this.chkConstStripPrefix);
            this.Controls.Add(this.label10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CSharpCodeGeneratedLCGSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "C# Code LCG Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkConstCamelCased;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbConstantName;
        private System.Windows.Forms.TextBox txtConstStripPrefix;
        private System.Windows.Forms.CheckBox chkConstStripPrefix;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}