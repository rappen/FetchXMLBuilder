
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
            this.propMeta = new System.Windows.Forms.PropertyGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panInfo1 = new System.Windows.Forms.Panel();
            this.lblInfo1 = new System.Windows.Forms.Label();
            this.lblInfo1Value = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panInfo2 = new System.Windows.Forms.Panel();
            this.lblInfo2 = new System.Windows.Forms.Label();
            this.lblInfo2Value = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panInfo1.SuspendLayout();
            this.panInfo2.SuspendLayout();
            this.SuspendLayout();
            // 
            // propMeta
            // 
            this.propMeta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propMeta.HelpVisible = false;
            this.propMeta.LineColor = System.Drawing.SystemColors.Window;
            this.propMeta.Location = new System.Drawing.Point(0, 53);
            this.propMeta.Name = "propMeta";
            this.propMeta.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propMeta.SelectedObject = this.propMeta;
            this.propMeta.Size = new System.Drawing.Size(262, 621);
            this.propMeta.TabIndex = 0;
            this.propMeta.ToolbarVisible = false;
            this.propMeta.ViewBorderColor = System.Drawing.SystemColors.Window;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 674);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(262, 28);
            this.panel1.TabIndex = 1;
            // 
            // panInfo1
            // 
            this.panInfo1.Controls.Add(this.lblInfo1Value);
            this.panInfo1.Controls.Add(this.lblInfo1);
            this.panInfo1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panInfo1.Location = new System.Drawing.Point(0, 0);
            this.panInfo1.Name = "panInfo1";
            this.panInfo1.Size = new System.Drawing.Size(262, 28);
            this.panInfo1.TabIndex = 2;
            // 
            // lblInfo1
            // 
            this.lblInfo1.AutoSize = true;
            this.lblInfo1.Location = new System.Drawing.Point(24, 13);
            this.lblInfo1.Name = "lblInfo1";
            this.lblInfo1.Size = new System.Drawing.Size(33, 13);
            this.lblInfo1.TabIndex = 0;
            this.lblInfo1.Text = "Entity";
            // 
            // lblInfo1Value
            // 
            this.lblInfo1Value.AutoSize = true;
            this.lblInfo1Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo1Value.Location = new System.Drawing.Point(113, 13);
            this.lblInfo1Value.Name = "lblInfo1Value";
            this.lblInfo1Value.Size = new System.Drawing.Size(46, 13);
            this.lblInfo1Value.TabIndex = 1;
            this.lblInfo1Value.Text = "account";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(24, 8);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(190, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "MscrmTools Metadata Browser Code™";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // panInfo2
            // 
            this.panInfo2.Controls.Add(this.lblInfo2Value);
            this.panInfo2.Controls.Add(this.lblInfo2);
            this.panInfo2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panInfo2.Location = new System.Drawing.Point(0, 28);
            this.panInfo2.Name = "panInfo2";
            this.panInfo2.Size = new System.Drawing.Size(262, 17);
            this.panInfo2.TabIndex = 3;
            // 
            // lblInfo2
            // 
            this.lblInfo2.AutoSize = true;
            this.lblInfo2.Location = new System.Drawing.Point(24, 3);
            this.lblInfo2.Name = "lblInfo2";
            this.lblInfo2.Size = new System.Drawing.Size(46, 13);
            this.lblInfo2.TabIndex = 1;
            this.lblInfo2.Text = "Attribute";
            // 
            // lblInfo2Value
            // 
            this.lblInfo2Value.AutoSize = true;
            this.lblInfo2Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo2Value.Location = new System.Drawing.Point(113, 3);
            this.lblInfo2Value.Name = "lblInfo2Value";
            this.lblInfo2Value.Size = new System.Drawing.Size(33, 13);
            this.lblInfo2Value.TabIndex = 2;
            this.lblInfo2Value.Text = "name";
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 45);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(262, 8);
            this.panel2.TabIndex = 4;
            // 
            // MetadataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(262, 702);
            this.Controls.Add(this.propMeta);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panInfo2);
            this.Controls.Add(this.panInfo1);
            this.Controls.Add(this.panel1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MetadataControl";
            this.Text = "Metadata";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panInfo1.ResumeLayout(false);
            this.panInfo1.PerformLayout();
            this.panInfo2.ResumeLayout(false);
            this.panInfo2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propMeta;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel panInfo1;
        private System.Windows.Forms.Label lblInfo1Value;
        private System.Windows.Forms.Label lblInfo1;
        private System.Windows.Forms.Panel panInfo2;
        private System.Windows.Forms.Label lblInfo2;
        private System.Windows.Forms.Label lblInfo2Value;
        private System.Windows.Forms.Panel panel2;
    }
}