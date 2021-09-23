
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
            this.propMeta = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // propMeta
            // 
            this.propMeta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propMeta.HelpVisible = false;
            this.propMeta.Location = new System.Drawing.Point(0, 0);
            this.propMeta.Name = "propMeta";
            this.propMeta.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propMeta.Size = new System.Drawing.Size(262, 541);
            this.propMeta.TabIndex = 0;
            this.propMeta.ToolbarVisible = false;
            // 
            // MetadataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(262, 541);
            this.Controls.Add(this.propMeta);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Name = "MetadataControl";
            this.Text = "Metadata";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propMeta;
    }
}