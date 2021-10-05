
namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    partial class metadataControl
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
            this.panInfo1 = new System.Windows.Forms.Panel();
            this.lblInfo1Value = new System.Windows.Forms.Label();
            this.lblInfo1 = new System.Windows.Forms.Label();
            this.panInfo2 = new System.Windows.Forms.Panel();
            this.lblInfo2Value = new System.Windows.Forms.Label();
            this.lblInfo2 = new System.Windows.Forms.Label();
            this.panLink = new System.Windows.Forms.Panel();
            this.panSeparator2 = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.propMeta = new System.Windows.Forms.PropertyGrid();
            this.panSeparator1 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panInfo1.SuspendLayout();
            this.panInfo2.SuspendLayout();
            this.panLink.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panInfo1
            // 
            this.panInfo1.Controls.Add(this.lblInfo1Value);
            this.panInfo1.Controls.Add(this.lblInfo1);
            this.panInfo1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panInfo1.Location = new System.Drawing.Point(0, 0);
            this.panInfo1.Name = "panInfo1";
            this.panInfo1.Size = new System.Drawing.Size(291, 24);
            this.panInfo1.TabIndex = 2;
            // 
            // lblInfo1Value
            // 
            this.lblInfo1Value.AutoSize = true;
            this.lblInfo1Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo1Value.Location = new System.Drawing.Point(113, 9);
            this.lblInfo1Value.Name = "lblInfo1Value";
            this.lblInfo1Value.Size = new System.Drawing.Size(70, 13);
            this.lblInfo1Value.TabIndex = 1;
            this.lblInfo1Value.Text = "<entityname>";
            // 
            // lblInfo1
            // 
            this.lblInfo1.AutoSize = true;
            this.lblInfo1.Location = new System.Drawing.Point(24, 9);
            this.lblInfo1.Name = "lblInfo1";
            this.lblInfo1.Size = new System.Drawing.Size(33, 13);
            this.lblInfo1.TabIndex = 0;
            this.lblInfo1.Text = "Entity";
            // 
            // panInfo2
            // 
            this.panInfo2.Controls.Add(this.lblInfo2Value);
            this.panInfo2.Controls.Add(this.lblInfo2);
            this.panInfo2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panInfo2.Location = new System.Drawing.Point(0, 24);
            this.panInfo2.Name = "panInfo2";
            this.panInfo2.Size = new System.Drawing.Size(291, 17);
            this.panInfo2.TabIndex = 3;
            // 
            // lblInfo2Value
            // 
            this.lblInfo2Value.AutoSize = true;
            this.lblInfo2Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo2Value.Location = new System.Drawing.Point(113, 3);
            this.lblInfo2Value.Name = "lblInfo2Value";
            this.lblInfo2Value.Size = new System.Drawing.Size(83, 13);
            this.lblInfo2Value.TabIndex = 2;
            this.lblInfo2Value.Text = "<attributename>";
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
            // panLink
            // 
            this.panLink.Controls.Add(this.panSeparator2);
            this.panLink.Controls.Add(this.linkLabel1);
            this.panLink.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panLink.Location = new System.Drawing.Point(0, 435);
            this.panLink.Name = "panLink";
            this.panLink.Size = new System.Drawing.Size(291, 28);
            this.panLink.TabIndex = 1;
            // 
            // panSeparator2
            // 
            this.panSeparator2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panSeparator2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panSeparator2.Location = new System.Drawing.Point(0, 0);
            this.panSeparator2.Name = "panSeparator2";
            this.panSeparator2.Size = new System.Drawing.Size(291, 1);
            this.panSeparator2.TabIndex = 5;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(24, 6);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(181, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "MscrmTools Metadata Browser Code";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panSeparator1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 41);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(291, 8);
            this.panel2.TabIndex = 4;
            // 
            // propMeta
            // 
            this.propMeta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propMeta.HelpVisible = false;
            this.propMeta.LineColor = System.Drawing.SystemColors.Window;
            this.propMeta.Location = new System.Drawing.Point(0, 53);
            this.propMeta.Name = "propMeta";
            this.propMeta.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propMeta.Size = new System.Drawing.Size(291, 378);
            this.propMeta.TabIndex = 0;
            this.propMeta.ToolbarVisible = false;
            this.propMeta.ViewBorderColor = System.Drawing.SystemColors.Window;
            // 
            // panSeparator1
            // 
            this.panSeparator1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panSeparator1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panSeparator1.Location = new System.Drawing.Point(0, 7);
            this.panSeparator1.Name = "panSeparator1";
            this.panSeparator1.Size = new System.Drawing.Size(291, 1);
            this.panSeparator1.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(291, 4);
            this.panel1.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Window;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 431);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(291, 4);
            this.panel3.TabIndex = 6;
            // 
            // metadataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propMeta);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panLink);
            this.Controls.Add(this.panInfo2);
            this.Controls.Add(this.panInfo1);
            this.Name = "metadataControl";
            this.Size = new System.Drawing.Size(291, 463);
            this.panInfo1.ResumeLayout(false);
            this.panInfo1.PerformLayout();
            this.panInfo2.ResumeLayout(false);
            this.panInfo2.PerformLayout();
            this.panLink.ResumeLayout(false);
            this.panLink.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panInfo1;
        private System.Windows.Forms.Label lblInfo1Value;
        private System.Windows.Forms.Label lblInfo1;
        private System.Windows.Forms.Panel panInfo2;
        private System.Windows.Forms.Label lblInfo2Value;
        private System.Windows.Forms.Label lblInfo2;
        private System.Windows.Forms.Panel panLink;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PropertyGrid propMeta;
        private System.Windows.Forms.Panel panSeparator2;
        private System.Windows.Forms.Panel panSeparator1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
    }
}
