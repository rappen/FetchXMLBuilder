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
            this.rbSafeLink = new System.Windows.Forms.RadioButton();
            this.rbAlmostReadable = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkConnection
            // 
            this.chkConnection.AutoSize = true;
            this.chkConnection.Location = new System.Drawing.Point(24, 87);
            this.chkConnection.Name = "chkConnection";
            this.chkConnection.Size = new System.Drawing.Size(117, 17);
            this.chkConnection.TabIndex = 0;
            this.chkConnection.Text = "Include connection";
            this.chkConnection.UseVisualStyleBackColor = true;
            this.chkConnection.CheckedChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // txtLink
            // 
            this.txtLink.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLink.BackColor = System.Drawing.SystemColors.Window;
            this.txtLink.Location = new System.Drawing.Point(24, 110);
            this.txtLink.Multiline = true;
            this.txtLink.Name = "txtLink";
            this.txtLink.Size = new System.Drawing.Size(512, 127);
            this.txtLink.TabIndex = 1;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCopy.Location = new System.Drawing.Point(380, 278);
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
            this.label1.Location = new System.Drawing.Point(21, 283);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Read details:";
            // 
            // linkInfo
            // 
            this.linkInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkInfo.AutoSize = true;
            this.linkInfo.Location = new System.Drawing.Point(96, 283);
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
            this.txtLinkName.Enabled = false;
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
            this.rbUrl.AutoSize = true;
            this.rbUrl.Checked = true;
            this.rbUrl.Location = new System.Drawing.Point(24, 64);
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
            this.rbHtml.AutoSize = true;
            this.rbHtml.Location = new System.Drawing.Point(77, 64);
            this.rbHtml.Name = "rbHtml";
            this.rbHtml.Size = new System.Drawing.Size(55, 17);
            this.rbHtml.TabIndex = 8;
            this.rbHtml.Text = "HTML";
            this.rbHtml.UseVisualStyleBackColor = true;
            this.rbHtml.CheckedChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // rbMarkdown
            // 
            this.rbMarkdown.AutoSize = true;
            this.rbMarkdown.Location = new System.Drawing.Point(138, 64);
            this.rbMarkdown.Name = "rbMarkdown";
            this.rbMarkdown.Size = new System.Drawing.Size(75, 17);
            this.rbMarkdown.TabIndex = 9;
            this.rbMarkdown.Text = "Markdown";
            this.rbMarkdown.UseVisualStyleBackColor = true;
            this.rbMarkdown.CheckedChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // rbSafeLink
            // 
            this.rbSafeLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbSafeLink.AutoSize = true;
            this.rbSafeLink.Checked = true;
            this.rbSafeLink.Location = new System.Drawing.Point(12, 3);
            this.rbSafeLink.Name = "rbSafeLink";
            this.rbSafeLink.Size = new System.Drawing.Size(70, 17);
            this.rbSafeLink.TabIndex = 10;
            this.rbSafeLink.TabStop = true;
            this.rbSafeLink.Text = "Safe Link";
            this.rbSafeLink.UseVisualStyleBackColor = true;
            this.rbSafeLink.CheckedChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // rbAlmostReadable
            // 
            this.rbAlmostReadable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbAlmostReadable.AutoSize = true;
            this.rbAlmostReadable.Location = new System.Drawing.Point(88, 3);
            this.rbAlmostReadable.Name = "rbAlmostReadable";
            this.rbAlmostReadable.Size = new System.Drawing.Size(128, 17);
            this.rbAlmostReadable.TabIndex = 11;
            this.rbAlmostReadable.Text = "Almost Readable Link";
            this.rbAlmostReadable.UseVisualStyleBackColor = true;
            this.rbAlmostReadable.CheckedChanged += new System.EventHandler(this.settings_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.rbAlmostReadable);
            this.panel1.Controls.Add(this.rbSafeLink);
            this.panel1.Location = new System.Drawing.Point(12, 240);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(235, 24);
            this.panel1.TabIndex = 12;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(299, 278);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 13;
            this.btnTest.Text = "Test run";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(461, 278);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // ShareLink
            // 
            this.AcceptButton = this.btnCopy;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(558, 313);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.panel1);
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
            this.Text = "Share Query Link via XrmToolBox";
            this.Load += new System.EventHandler(this.ShareLink_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.RadioButton rbSafeLink;
        private System.Windows.Forms.RadioButton rbAlmostReadable;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnClose;
    }
}