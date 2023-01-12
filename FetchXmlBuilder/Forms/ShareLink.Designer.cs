namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    partial class ShareLink
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShareLink));
            this.chkConnection = new System.Windows.Forms.CheckBox();
            this.txtLink = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.linkInfo = new System.Windows.Forms.LinkLabel();
            this.txtLinkName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rbUrl = new System.Windows.Forms.RadioButton();
            this.rbHtml = new System.Windows.Forms.RadioButton();
            this.rbMarkdown = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // chkConnection
            // 
            this.chkConnection.AutoSize = true;
            this.chkConnection.Location = new System.Drawing.Point(24, 68);
            this.chkConnection.Name = "chkConnection";
            this.chkConnection.Size = new System.Drawing.Size(153, 17);
            this.chkConnection.TabIndex = 0;
            this.chkConnection.Text = "Include current connection";
            this.chkConnection.UseVisualStyleBackColor = true;
            this.chkConnection.CheckedChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // txtLink
            // 
            this.txtLink.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLink.BackColor = System.Drawing.SystemColors.Window;
            this.txtLink.Location = new System.Drawing.Point(24, 91);
            this.txtLink.Multiline = true;
            this.txtLink.Name = "txtLink";
            this.txtLink.ReadOnly = true;
            this.txtLink.Size = new System.Drawing.Size(512, 112);
            this.txtLink.TabIndex = 1;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(461, 244);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 2;
            this.btnCopy.Text = "Copy Link";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 249);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Read details:";
            // 
            // linkInfo
            // 
            this.linkInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkInfo.AutoSize = true;
            this.linkInfo.Location = new System.Drawing.Point(96, 249);
            this.linkInfo.Name = "linkInfo";
            this.linkInfo.Size = new System.Drawing.Size(175, 13);
            this.linkInfo.TabIndex = 4;
            this.linkInfo.TabStop = true;
            this.linkInfo.Text = "FetchXML Builder - Sharing Queries";
            this.linkInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkInfo_LinkClicked);
            // 
            // txtLinkName
            // 
            this.txtLinkName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLinkName.Location = new System.Drawing.Point(24, 38);
            this.txtLinkName.Name = "txtLinkName";
            this.txtLinkName.Size = new System.Drawing.Size(512, 20);
            this.txtLinkName.TabIndex = 5;
            this.txtLinkName.Text = "FetchXML Builder Query";
            this.txtLinkName.TextChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Link Titel";
            // 
            // rbUrl
            // 
            this.rbUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbUrl.AutoSize = true;
            this.rbUrl.Checked = true;
            this.rbUrl.Location = new System.Drawing.Point(24, 218);
            this.rbUrl.Name = "rbUrl";
            this.rbUrl.Size = new System.Drawing.Size(47, 17);
            this.rbUrl.TabIndex = 7;
            this.rbUrl.TabStop = true;
            this.rbUrl.Text = "URL";
            this.rbUrl.UseVisualStyleBackColor = true;
            this.rbUrl.CheckedChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // rbHtml
            // 
            this.rbHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbHtml.AutoSize = true;
            this.rbHtml.Location = new System.Drawing.Point(77, 218);
            this.rbHtml.Name = "rbHtml";
            this.rbHtml.Size = new System.Drawing.Size(55, 17);
            this.rbHtml.TabIndex = 8;
            this.rbHtml.Text = "HTML";
            this.rbHtml.UseVisualStyleBackColor = true;
            this.rbHtml.CheckedChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // rbMarkdown
            // 
            this.rbMarkdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbMarkdown.AutoSize = true;
            this.rbMarkdown.Location = new System.Drawing.Point(138, 218);
            this.rbMarkdown.Name = "rbMarkdown";
            this.rbMarkdown.Size = new System.Drawing.Size(75, 17);
            this.rbMarkdown.TabIndex = 9;
            this.rbMarkdown.Text = "Markdown";
            this.rbMarkdown.UseVisualStyleBackColor = true;
            this.rbMarkdown.CheckedChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // ShareLink
            // 
            this.AcceptButton = this.btnCopy;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(558, 279);
            this.Controls.Add(this.rbMarkdown);
            this.Controls.Add(this.rbHtml);
            this.Controls.Add(this.rbUrl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLinkName);
            this.Controls.Add(this.linkInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtLink);
            this.Controls.Add(this.chkConnection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShareLink";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Share Link";
            this.Load += new System.EventHandler(this.ShareLink_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkConnection;
        private System.Windows.Forms.TextBox txtLink;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkInfo;
        private System.Windows.Forms.TextBox txtLinkName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbUrl;
        private System.Windows.Forms.RadioButton rbHtml;
        private System.Windows.Forms.RadioButton rbMarkdown;
    }
}