namespace Rappen.XTB
{
    partial class Supporting
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Supporting));
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblCompany = new System.Windows.Forms.Label();
            this.txtCompany = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbPersonal = new System.Windows.Forms.RadioButton();
            this.rbCompany = new System.Windows.Forms.RadioButton();
            this.panCorp = new System.Windows.Forms.Panel();
            this.txtCountry = new System.Windows.Forms.TextBox();
            this.lblCountry = new System.Windows.Forms.Label();
            this.cmbSize = new System.Windows.Forms.ComboBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblInvoiceemail = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtIFirst = new System.Windows.Forms.TextBox();
            this.txtILast = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.panPersonal = new System.Windows.Forms.Panel();
            this.txtICountry = new System.Windows.Forms.TextBox();
            this.lblICountry = new System.Windows.Forms.Label();
            this.txtIEmail = new System.Windows.Forms.TextBox();
            this.lblIEmail = new System.Windows.Forms.Label();
            this.lblIName = new System.Windows.Forms.Label();
            this.panInfo = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnInfoClose = new System.Windows.Forms.Button();
            this.linkHelping = new System.Windows.Forms.LinkLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnInfo = new System.Windows.Forms.Button();
            this.lblLater = new System.Windows.Forms.Label();
            this.lblAlready = new System.Windows.Forms.Label();
            this.pics = new System.Windows.Forms.ImageList(this.components);
            this.panel1.SuspendLayout();
            this.panCorp.SuspendLayout();
            this.panPersonal.SuspendLayout();
            this.panInfo.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Berlin Sans FB", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Yellow;
            this.lblHeader.Location = new System.Drawing.Point(49, 38);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(421, 38);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "We Support Tools!";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCompany
            // 
            this.lblCompany.AutoSize = true;
            this.lblCompany.Location = new System.Drawing.Point(16, 19);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.Size = new System.Drawing.Size(72, 18);
            this.lblCompany.TabIndex = 1;
            this.lblCompany.Text = "Company";
            // 
            // txtCompany
            // 
            this.txtCompany.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtCompany.ForeColor = System.Drawing.Color.Yellow;
            this.txtCompany.Location = new System.Drawing.Point(148, 16);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.Size = new System.Drawing.Size(292, 25);
            this.txtCompany.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtCompany, "Name of your company");
            this.txtCompany.Validating += new System.ComponentModel.CancelEventHandler(this.txtCompany_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "Supporting";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbPersonal);
            this.panel1.Controls.Add(this.rbCompany);
            this.panel1.Location = new System.Drawing.Point(169, 112);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(245, 42);
            this.panel1.TabIndex = 1;
            // 
            // rbPersonal
            // 
            this.rbPersonal.AutoSize = true;
            this.rbPersonal.Location = new System.Drawing.Point(145, 12);
            this.rbPersonal.Name = "rbPersonal";
            this.rbPersonal.Size = new System.Drawing.Size(68, 22);
            this.rbPersonal.TabIndex = 1;
            this.rbPersonal.Text = "myself";
            this.rbPersonal.UseVisualStyleBackColor = true;
            this.rbPersonal.CheckedChanged += new System.EventHandler(this.rbType_CheckedChanged);
            // 
            // rbCompany
            // 
            this.rbCompany.AutoSize = true;
            this.rbCompany.Checked = true;
            this.rbCompany.Location = new System.Drawing.Point(12, 12);
            this.rbCompany.Name = "rbCompany";
            this.rbCompany.Size = new System.Drawing.Size(112, 22);
            this.rbCompany.TabIndex = 0;
            this.rbCompany.TabStop = true;
            this.rbCompany.Text = "our company";
            this.rbCompany.UseVisualStyleBackColor = true;
            this.rbCompany.CheckedChanged += new System.EventHandler(this.rbType_CheckedChanged);
            // 
            // panCorp
            // 
            this.panCorp.Controls.Add(this.txtCountry);
            this.panCorp.Controls.Add(this.lblCountry);
            this.panCorp.Controls.Add(this.cmbSize);
            this.panCorp.Controls.Add(this.lblSize);
            this.panCorp.Controls.Add(this.txtEmail);
            this.panCorp.Controls.Add(this.lblInvoiceemail);
            this.panCorp.Controls.Add(this.lblCompany);
            this.panCorp.Controls.Add(this.txtCompany);
            this.panCorp.Location = new System.Drawing.Point(30, 180);
            this.panCorp.Name = "panCorp";
            this.panCorp.Size = new System.Drawing.Size(456, 148);
            this.panCorp.TabIndex = 2;
            // 
            // txtCountry
            // 
            this.txtCountry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtCountry.ForeColor = System.Drawing.Color.Yellow;
            this.txtCountry.Location = new System.Drawing.Point(148, 78);
            this.txtCountry.Name = "txtCountry";
            this.txtCountry.Size = new System.Drawing.Size(292, 25);
            this.txtCountry.TabIndex = 3;
            this.txtCountry.Validating += new System.ComponentModel.CancelEventHandler(this.txtCountry_Validating);
            // 
            // lblCountry
            // 
            this.lblCountry.AutoSize = true;
            this.lblCountry.Location = new System.Drawing.Point(16, 81);
            this.lblCountry.Name = "lblCountry";
            this.lblCountry.Size = new System.Drawing.Size(59, 18);
            this.lblCountry.TabIndex = 7;
            this.lblCountry.Text = "Country";
            // 
            // cmbSize
            // 
            this.cmbSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.cmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSize.ForeColor = System.Drawing.Color.Yellow;
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Items.AddRange(new object[] {
            "",
            "1",
            "2-10",
            "11-50",
            "51-100",
            "101+"});
            this.cmbSize.Location = new System.Drawing.Point(148, 109);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(292, 26);
            this.cmbSize.TabIndex = 4;
            this.cmbSize.Validating += new System.ComponentModel.CancelEventHandler(this.cmbSize_Validating);
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(16, 112);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(91, 18);
            this.lblSize.TabIndex = 5;
            this.lblSize.Text = "Users of FXB";
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtEmail.ForeColor = System.Drawing.Color.Yellow;
            this.txtEmail.Location = new System.Drawing.Point(148, 47);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(292, 25);
            this.txtEmail.TabIndex = 2;
            this.txtEmail.Validating += new System.ComponentModel.CancelEventHandler(this.txtEmail_Validating);
            // 
            // lblInvoiceemail
            // 
            this.lblInvoiceemail.AutoSize = true;
            this.lblInvoiceemail.Location = new System.Drawing.Point(16, 50);
            this.lblInvoiceemail.Name = "lblInvoiceemail";
            this.lblInvoiceemail.Size = new System.Drawing.Size(45, 18);
            this.lblInvoiceemail.TabIndex = 3;
            this.lblInvoiceemail.Text = "Email";
            // 
            // txtIFirst
            // 
            this.txtIFirst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtIFirst.ForeColor = System.Drawing.Color.Yellow;
            this.txtIFirst.Location = new System.Drawing.Point(148, 16);
            this.txtIFirst.Name = "txtIFirst";
            this.txtIFirst.Size = new System.Drawing.Size(141, 25);
            this.txtIFirst.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtIFirst, "Name of your company");
            this.txtIFirst.Validating += new System.ComponentModel.CancelEventHandler(this.txtIFirst_Validating);
            // 
            // txtILast
            // 
            this.txtILast.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtILast.ForeColor = System.Drawing.Color.Yellow;
            this.txtILast.Location = new System.Drawing.Point(295, 16);
            this.txtILast.Name = "txtILast";
            this.txtILast.Size = new System.Drawing.Size(145, 25);
            this.txtILast.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtILast, "Name of your company");
            this.txtILast.Validating += new System.ComponentModel.CancelEventHandler(this.txtIFirst_Validating);
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ImageIndex = 0;
            this.btnSave.ImageList = this.pics;
            this.btnSave.Location = new System.Drawing.Point(178, 334);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(210, 68);
            this.btnSave.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnSave, "Click to forward you to supporting form on JonasR.app.\r\nInformation added here wi" +
        "ll be brought in next form.");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnCorp_Click);
            // 
            // panPersonal
            // 
            this.panPersonal.Controls.Add(this.txtILast);
            this.panPersonal.Controls.Add(this.txtICountry);
            this.panPersonal.Controls.Add(this.lblICountry);
            this.panPersonal.Controls.Add(this.txtIEmail);
            this.panPersonal.Controls.Add(this.lblIEmail);
            this.panPersonal.Controls.Add(this.lblIName);
            this.panPersonal.Controls.Add(this.txtIFirst);
            this.panPersonal.Location = new System.Drawing.Point(30, 492);
            this.panPersonal.Name = "panPersonal";
            this.panPersonal.Size = new System.Drawing.Size(456, 148);
            this.panPersonal.TabIndex = 3;
            this.panPersonal.Visible = false;
            // 
            // txtICountry
            // 
            this.txtICountry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtICountry.ForeColor = System.Drawing.Color.Yellow;
            this.txtICountry.Location = new System.Drawing.Point(148, 78);
            this.txtICountry.Name = "txtICountry";
            this.txtICountry.Size = new System.Drawing.Size(292, 25);
            this.txtICountry.TabIndex = 4;
            this.txtICountry.Validating += new System.ComponentModel.CancelEventHandler(this.txtICountry_Validating);
            // 
            // lblICountry
            // 
            this.lblICountry.AutoSize = true;
            this.lblICountry.Location = new System.Drawing.Point(16, 81);
            this.lblICountry.Name = "lblICountry";
            this.lblICountry.Size = new System.Drawing.Size(59, 18);
            this.lblICountry.TabIndex = 7;
            this.lblICountry.Text = "Country";
            // 
            // txtIEmail
            // 
            this.txtIEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtIEmail.ForeColor = System.Drawing.Color.Yellow;
            this.txtIEmail.Location = new System.Drawing.Point(148, 47);
            this.txtIEmail.Name = "txtIEmail";
            this.txtIEmail.Size = new System.Drawing.Size(292, 25);
            this.txtIEmail.TabIndex = 3;
            this.txtIEmail.Validating += new System.ComponentModel.CancelEventHandler(this.txtIEmail_Validating);
            // 
            // lblIEmail
            // 
            this.lblIEmail.AutoSize = true;
            this.lblIEmail.Location = new System.Drawing.Point(16, 50);
            this.lblIEmail.Name = "lblIEmail";
            this.lblIEmail.Size = new System.Drawing.Size(45, 18);
            this.lblIEmail.TabIndex = 3;
            this.lblIEmail.Text = "Email";
            // 
            // lblIName
            // 
            this.lblIName.AutoSize = true;
            this.lblIName.Location = new System.Drawing.Point(16, 19);
            this.lblIName.Name = "lblIName";
            this.lblIName.Size = new System.Drawing.Size(123, 18);
            this.lblIName.TabIndex = 1;
            this.lblIName.Text = "Name (first / last)";
            // 
            // panInfo
            // 
            this.panInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.panInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panInfo.Controls.Add(this.panel2);
            this.panInfo.Location = new System.Drawing.Point(33, 678);
            this.panInfo.Name = "panInfo";
            this.panInfo.Size = new System.Drawing.Size(437, 390);
            this.panInfo.TabIndex = 11;
            this.panInfo.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(66)))), ((int)(((byte)(173)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnInfoClose);
            this.panel2.Controls.Add(this.linkHelping);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(5, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(425, 378);
            this.panel2.TabIndex = 0;
            // 
            // btnInfoClose
            // 
            this.btnInfoClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfoClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInfoClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.btnInfoClose.Image = ((System.Drawing.Image)(resources.GetObject("btnInfoClose.Image")));
            this.btnInfoClose.Location = new System.Drawing.Point(390, 3);
            this.btnInfoClose.Name = "btnInfoClose";
            this.btnInfoClose.Size = new System.Drawing.Size(30, 30);
            this.btnInfoClose.TabIndex = 13;
            this.btnInfoClose.UseVisualStyleBackColor = true;
            this.btnInfoClose.Click += new System.EventHandler(this.btnInfoClose_Click);
            // 
            // linkHelping
            // 
            this.linkHelping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkHelping.AutoSize = true;
            this.linkHelping.LinkColor = System.Drawing.Color.Yellow;
            this.linkHelping.Location = new System.Drawing.Point(94, 348);
            this.linkHelping.Name = "linkHelping";
            this.linkHelping.Size = new System.Drawing.Size(179, 18);
            this.linkHelping.TabIndex = 2;
            this.linkHelping.TabStop = true;
            this.linkHelping.Tag = "https://jonasr.app/helping/";
            this.linkHelping.Text = "https://jonasr.app/helping/";
            this.linkHelping.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkHelping_LinkClicked);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(66)))), ((int)(((byte)(173)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.ForeColor = System.Drawing.Color.Yellow;
            this.textBox1.Location = new System.Drawing.Point(16, 53);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(391, 296);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Berlin Sans FB", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(411, 35);
            this.label2.TabIndex = 0;
            this.label2.Text = "How and Why do we support?";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnInfo
            // 
            this.btnInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.btnInfo.Image = ((System.Drawing.Image)(resources.GetObject("btnInfo.Image")));
            this.btnInfo.Location = new System.Drawing.Point(12, 12);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(30, 30);
            this.btnInfo.TabIndex = 12;
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // lblLater
            // 
            this.lblLater.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLater.AutoSize = true;
            this.lblLater.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLater.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.lblLater.Location = new System.Drawing.Point(461, 386);
            this.lblLater.Name = "lblLater";
            this.lblLater.Size = new System.Drawing.Size(43, 18);
            this.lblLater.TabIndex = 101;
            this.lblLater.Text = "Later";
            this.lblLater.Click += new System.EventHandler(this.lblLater_Click);
            // 
            // lblAlready
            // 
            this.lblAlready.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAlready.AutoSize = true;
            this.lblAlready.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAlready.Location = new System.Drawing.Point(9, 368);
            this.lblAlready.Name = "lblAlready";
            this.lblAlready.Size = new System.Drawing.Size(82, 36);
            this.lblAlready.TabIndex = 102;
            this.lblAlready.Text = "I\'m already\r\nsupporting!";
            this.lblAlready.Visible = false;
            this.lblAlready.Click += new System.EventHandler(this.lblAlready_Click);
            // 
            // pics
            // 
            this.pics.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("pics.ImageStream")));
            this.pics.TransparentColor = System.Drawing.Color.Transparent;
            this.pics.Images.SetKeyName(0, "Corporate Supports Tools 2 narrow 200.png");
            this.pics.Images.SetKeyName(1, "I Support Tools narrow 200.png");
            // 
            // Supporting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(66)))), ((int)(((byte)(173)))));
            this.ClientSize = new System.Drawing.Size(516, 429);
            this.ControlBox = false;
            this.Controls.Add(this.lblAlready);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.panInfo);
            this.Controls.Add(this.panPersonal);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panCorp);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.lblLater);
            this.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Yellow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Supporting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "We Support Tools";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Supporting_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panCorp.ResumeLayout(false);
            this.panCorp.PerformLayout();
            this.panPersonal.ResumeLayout(false);
            this.panPersonal.PerformLayout();
            this.panInfo.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblCompany;
        private System.Windows.Forms.TextBox txtCompany;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbPersonal;
        private System.Windows.Forms.RadioButton rbCompany;
        private System.Windows.Forms.Panel panCorp;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblInvoiceemail;
        private System.Windows.Forms.ComboBox cmbSize;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.TextBox txtCountry;
        private System.Windows.Forms.Label lblCountry;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panPersonal;
        private System.Windows.Forms.TextBox txtILast;
        private System.Windows.Forms.TextBox txtICountry;
        private System.Windows.Forms.Label lblICountry;
        private System.Windows.Forms.TextBox txtIEmail;
        private System.Windows.Forms.Label lblIEmail;
        private System.Windows.Forms.Label lblIName;
        private System.Windows.Forms.TextBox txtIFirst;
        private System.Windows.Forms.Panel panInfo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.LinkLabel linkHelping;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnInfoClose;
        private System.Windows.Forms.Label lblLater;
        private System.Windows.Forms.Label lblAlready;
        private System.Windows.Forms.ImageList pics;
    }
}