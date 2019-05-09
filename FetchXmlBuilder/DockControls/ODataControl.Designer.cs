namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    partial class ODataControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ODataControl));
            this.panOData = new System.Windows.Forms.Panel();
            this.linkOData = new System.Windows.Forms.LinkLabel();
            this.panODataLabel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuOData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuODataExecute = new System.Windows.Forms.ToolStripMenuItem();
            this.menuODataCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.panOData.SuspendLayout();
            this.panODataLabel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuOData.SuspendLayout();
            this.SuspendLayout();
            // 
            // panOData
            // 
            this.panOData.BackColor = System.Drawing.SystemColors.Window;
            this.panOData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panOData.Controls.Add(this.linkOData);
            this.panOData.Controls.Add(this.panODataLabel);
            this.panOData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panOData.Location = new System.Drawing.Point(0, 0);
            this.panOData.Name = "panOData";
            this.panOData.Padding = new System.Windows.Forms.Padding(4);
            this.panOData.Size = new System.Drawing.Size(732, 50);
            this.panOData.TabIndex = 28;
            // 
            // linkOData
            // 
            this.linkOData.ContextMenuStrip = this.menuOData;
            this.linkOData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkOData.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.linkOData.Location = new System.Drawing.Point(46, 4);
            this.linkOData.Name = "linkOData";
            this.linkOData.Padding = new System.Windows.Forms.Padding(2);
            this.linkOData.Size = new System.Drawing.Size(680, 40);
            this.linkOData.TabIndex = 3;
            this.linkOData.Text = "OData query";
            this.linkOData.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkOData_LinkClicked);
            this.linkOData.UseMnemonic = false;
            // 
            // panODataLabel
            // 
            this.panODataLabel.Controls.Add(this.pictureBox1);
            this.panODataLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.panODataLabel.Location = new System.Drawing.Point(4, 4);
            this.panODataLabel.Name = "panODataLabel";
            this.panODataLabel.Size = new System.Drawing.Size(42, 40);
            this.panODataLabel.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(42, 38);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // menuOData
            // 
            this.menuOData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuODataExecute,
            this.menuODataCopy});
            this.menuOData.Name = "menuOData";
            this.menuOData.Size = new System.Drawing.Size(127, 48);
            // 
            // menuODataExecute
            // 
            this.menuODataExecute.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.menuODataExecute.Name = "menuODataExecute";
            this.menuODataExecute.Size = new System.Drawing.Size(126, 22);
            this.menuODataExecute.Text = "Execute";
            this.menuODataExecute.Click += new System.EventHandler(this.menuODataExecute_Click);
            // 
            // menuODataCopy
            // 
            this.menuODataCopy.Name = "menuODataCopy";
            this.menuODataCopy.Size = new System.Drawing.Size(126, 22);
            this.menuODataCopy.Text = "Copy URL";
            this.menuODataCopy.Click += new System.EventHandler(this.menuODataCopy_Click);
            // 
            // ODataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 50);
            this.Controls.Add(this.panOData);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Name = "ODataControl";
            this.TabText = "OData";
            this.Text = "OData";
            this.DockStateChanged += new System.EventHandler(this.ODataControl_DockStateChanged);
            this.panOData.ResumeLayout(false);
            this.panODataLabel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuOData.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panOData;
        private System.Windows.Forms.LinkLabel linkOData;
        private System.Windows.Forms.Panel panODataLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip menuOData;
        private System.Windows.Forms.ToolStripMenuItem menuODataExecute;
        private System.Windows.Forms.ToolStripMenuItem menuODataCopy;
    }
}
