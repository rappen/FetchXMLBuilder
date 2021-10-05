
namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    partial class MetadataControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetadataControl));
            this.metadataControl1 = new Cinteros.Xrm.FetchXmlBuilder.Controls.XRMMetadataControl();
            this.SuspendLayout();
            // 
            // metadataControl1
            // 
            this.metadataControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metadataControl1.Location = new System.Drawing.Point(0, 0);
            this.metadataControl1.Name = "metadataControl1";
            this.metadataControl1.Size = new System.Drawing.Size(262, 702);
            this.metadataControl1.TabIndex = 0;
            // 
            // MetadataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(262, 702);
            this.Controls.Add(this.metadataControl1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MetadataControl";
            this.Text = "Metadata";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.XRMMetadataControl metadataControl1;
    }
}