namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    partial class FlowListControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowListControl));
            this.panOData = new System.Windows.Forms.Panel();
            this.lblCopied = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.linkExpand = new System.Windows.Forms.LinkLabel();
            this.linkTop = new System.Windows.Forms.LinkLabel();
            this.linkOrder = new System.Windows.Forms.LinkLabel();
            this.linkFilter = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkAggr = new System.Windows.Forms.LinkLabel();
            this.panODataLabel = new System.Windows.Forms.Panel();
            this.linkHelp = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblError = new System.Windows.Forms.Label();
            this.tm = new System.Windows.Forms.Timer(this.components);
            this.panOData.SuspendLayout();
            this.panODataLabel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panOData
            // 
            this.panOData.BackColor = System.Drawing.SystemColors.Window;
            this.panOData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panOData.Controls.Add(this.lblCopied);
            this.panOData.Controls.Add(this.label6);
            this.panOData.Controls.Add(this.linkExpand);
            this.panOData.Controls.Add(this.linkTop);
            this.panOData.Controls.Add(this.linkOrder);
            this.panOData.Controls.Add(this.linkFilter);
            this.panOData.Controls.Add(this.label5);
            this.panOData.Controls.Add(this.label4);
            this.panOData.Controls.Add(this.label3);
            this.panOData.Controls.Add(this.label2);
            this.panOData.Controls.Add(this.label1);
            this.panOData.Controls.Add(this.linkAggr);
            this.panOData.Controls.Add(this.panODataLabel);
            this.panOData.Controls.Add(this.lblError);
            this.panOData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panOData.Location = new System.Drawing.Point(0, 0);
            this.panOData.Name = "panOData";
            this.panOData.Padding = new System.Windows.Forms.Padding(4);
            this.panOData.Size = new System.Drawing.Size(519, 146);
            this.panOData.TabIndex = 28;
            // 
            // lblCopied
            // 
            this.lblCopied.AutoSize = true;
            this.lblCopied.ForeColor = System.Drawing.Color.Red;
            this.lblCopied.Location = new System.Drawing.Point(280, 112);
            this.lblCopied.Name = "lblCopied";
            this.lblCopied.Size = new System.Drawing.Size(43, 13);
            this.lblCopied.TabIndex = 14;
            this.lblCopied.Text = "Copied!";
            this.lblCopied.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(134, 112);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Click links above to copy!";
            // 
            // linkExpand
            // 
            this.linkExpand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkExpand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkExpand.Location = new System.Drawing.Point(131, 84);
            this.linkExpand.Name = "linkExpand";
            this.linkExpand.Padding = new System.Windows.Forms.Padding(2);
            this.linkExpand.Size = new System.Drawing.Size(376, 20);
            this.linkExpand.TabIndex = 12;
            this.linkExpand.TabStop = true;
            this.linkExpand.Tag = "Expand Query";
            this.linkExpand.Text = "N/A";
            this.linkExpand.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // linkTop
            // 
            this.linkTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkTop.Location = new System.Drawing.Point(131, 65);
            this.linkTop.Name = "linkTop";
            this.linkTop.Padding = new System.Windows.Forms.Padding(2);
            this.linkTop.Size = new System.Drawing.Size(376, 20);
            this.linkTop.TabIndex = 11;
            this.linkTop.TabStop = true;
            this.linkTop.Tag = "Top Count";
            this.linkTop.Text = "N/A";
            this.linkTop.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // linkOrder
            // 
            this.linkOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkOrder.Location = new System.Drawing.Point(131, 46);
            this.linkOrder.Name = "linkOrder";
            this.linkOrder.Padding = new System.Windows.Forms.Padding(2);
            this.linkOrder.Size = new System.Drawing.Size(376, 20);
            this.linkOrder.TabIndex = 10;
            this.linkOrder.TabStop = true;
            this.linkOrder.Tag = "Order By";
            this.linkOrder.Text = "N/A";
            this.linkOrder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // linkFilter
            // 
            this.linkFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkFilter.Location = new System.Drawing.Point(131, 27);
            this.linkFilter.Name = "linkFilter";
            this.linkFilter.Padding = new System.Windows.Forms.Padding(2);
            this.linkFilter.Size = new System.Drawing.Size(376, 20);
            this.linkFilter.TabIndex = 9;
            this.linkFilter.TabStop = true;
            this.linkFilter.Tag = "Filter Query";
            this.linkFilter.Text = "N/A";
            this.linkFilter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 86);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Expand Query";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 67);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Top Count";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 48);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Order By";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(51, 29);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "Filter query";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Aggr.transf.";
            // 
            // linkAggr
            // 
            this.linkAggr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkAggr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkAggr.Location = new System.Drawing.Point(131, 8);
            this.linkAggr.Name = "linkAggr";
            this.linkAggr.Padding = new System.Windows.Forms.Padding(2);
            this.linkAggr.Size = new System.Drawing.Size(376, 20);
            this.linkAggr.TabIndex = 3;
            this.linkAggr.TabStop = true;
            this.linkAggr.Tag = "Aggregate transformation";
            this.linkAggr.Text = "N/A";
            this.linkAggr.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // panODataLabel
            // 
            this.panODataLabel.Controls.Add(this.linkHelp);
            this.panODataLabel.Controls.Add(this.pictureBox1);
            this.panODataLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.panODataLabel.Location = new System.Drawing.Point(4, 4);
            this.panODataLabel.Name = "panODataLabel";
            this.panODataLabel.Size = new System.Drawing.Size(42, 136);
            this.panODataLabel.TabIndex = 2;
            // 
            // linkHelp
            // 
            this.linkHelp.AutoSize = true;
            this.linkHelp.Location = new System.Drawing.Point(3, 44);
            this.linkHelp.Name = "linkHelp";
            this.linkHelp.Size = new System.Drawing.Size(32, 13);
            this.linkHelp.TabIndex = 1;
            this.linkHelp.TabStop = true;
            this.linkHelp.Text = "Help!";
            this.linkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkHelp_LinkClicked);
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
            // lblError
            // 
            this.lblError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(131, 8);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(376, 96);
            this.lblError.TabIndex = 0;
            this.lblError.Text = "Error";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tm
            // 
            this.tm.Interval = 2000;
            this.tm.Tick += new System.EventHandler(this.Tm_Tick);
            // 
            // FlowListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(519, 146);
            this.Controls.Add(this.panOData);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Name = "FlowListControl";
            this.TabText = "Power Automate List Records Parameters";
            this.Text = "Power Automate List Records Parameters";
            this.DockStateChanged += new System.EventHandler(this.FlowListControl_DockStateChanged);
            this.panOData.ResumeLayout(false);
            this.panOData.PerformLayout();
            this.panODataLabel.ResumeLayout(false);
            this.panODataLabel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panOData;
        private System.Windows.Forms.LinkLabel linkAggr;
        private System.Windows.Forms.Panel panODataLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel linkExpand;
        private System.Windows.Forms.LinkLabel linkTop;
        private System.Windows.Forms.LinkLabel linkOrder;
        private System.Windows.Forms.LinkLabel linkFilter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCopied;
        private System.Windows.Forms.Timer tm;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.LinkLabel linkHelp;
    }
}
