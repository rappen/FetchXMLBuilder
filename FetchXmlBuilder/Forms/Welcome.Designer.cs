namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    partial class Welcome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Welcome));
            this.llTwitter = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.llWeb = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.llStats = new System.Windows.Forms.LinkLabel();
            this.webRelease = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.lblLoading = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webRelease)).BeginInit();
            this.SuspendLayout();
            // 
            // llTwitter
            // 
            this.llTwitter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.llTwitter.AutoSize = true;
            this.llTwitter.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.llTwitter.Location = new System.Drawing.Point(81, 576);
            this.llTwitter.Name = "llTwitter";
            this.llTwitter.Size = new System.Drawing.Size(99, 13);
            this.llTwitter.TabIndex = 24;
            this.llTwitter.TabStop = true;
            this.llTwitter.Text = "@FetchXMLBuilder";
            this.llTwitter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llTwitter_LinkClicked);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 576);
            this.label4.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Twitter:";
            // 
            // llWeb
            // 
            this.llWeb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.llWeb.AutoSize = true;
            this.llWeb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.llWeb.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.llWeb.Location = new System.Drawing.Point(81, 592);
            this.llWeb.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.llWeb.Name = "llWeb";
            this.llWeb.Size = new System.Drawing.Size(136, 13);
            this.llWeb.TabIndex = 17;
            this.llWeb.TabStop = true;
            this.llWeb.Text = "https://fetchxmlbuilder.com";
            this.llWeb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llWeb_LinkClicked);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 592);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Web:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(407, 502);
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 608);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Statistics:";
            // 
            // llStats
            // 
            this.llStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.llStats.AutoSize = true;
            this.llStats.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.llStats.Location = new System.Drawing.Point(81, 608);
            this.llStats.Name = "llStats";
            this.llStats.Size = new System.Drawing.Size(59, 13);
            this.llStats.TabIndex = 33;
            this.llStats.TabStop = true;
            this.llStats.Text = "Information";
            this.llStats.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llStats_LinkClicked);
            // 
            // webRelease
            // 
            this.webRelease.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webRelease.CreationProperties = null;
            this.webRelease.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webRelease.Location = new System.Drawing.Point(425, 12);
            this.webRelease.Name = "webRelease";
            this.webRelease.Size = new System.Drawing.Size(738, 589);
            this.webRelease.Source = new System.Uri("https://jonasr.app/fxb/releases/1-2021-10-002/", System.UriKind.Absolute);
            this.webRelease.TabIndex = 36;
            this.webRelease.ZoomFactor = 1D;
            this.webRelease.NavigationCompleted += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs>(this.webRelease_NavigationCompleted);
            // 
            // lblLoading
            // 
            this.lblLoading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblLoading.AutoSize = true;
            this.lblLoading.Location = new System.Drawing.Point(743, 273);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(91, 13);
            this.lblLoading.TabIndex = 37;
            this.lblLoading.Text = "Loading release...";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Image = ((System.Drawing.Image)(resources.GetObject("linkLabel1.Image")));
            this.linkLabel1.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.linkLabel1.Location = new System.Drawing.Point(431, 612);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(138, 13);
            this.linkLabel1.TabIndex = 38;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Click to open in browser      ";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(1049, 607);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(114, 23);
            this.btnClose.TabIndex = 39;
            this.btnClose.Text = "Continue";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // Welcome
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1175, 638);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.webRelease);
            this.Controls.Add(this.llStats);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.llTwitter);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.llWeb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(880, 580);
            this.Name = "Welcome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Welcome to FetchXML Builder";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webRelease)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel llTwitter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel llWeb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel llStats;
        private Microsoft.Web.WebView2.WinForms.WebView2 webRelease;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button btnClose;
    }
}