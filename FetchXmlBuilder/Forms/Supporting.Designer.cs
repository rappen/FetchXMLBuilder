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
            this.txtCompanyName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbPersonal = new System.Windows.Forms.RadioButton();
            this.rbCompany = new System.Windows.Forms.RadioButton();
            this.panCorp = new System.Windows.Forms.Panel();
            this.txtCompanyCountry = new System.Windows.Forms.TextBox();
            this.lblCountry = new System.Windows.Forms.Label();
            this.cmbCompanyUsers = new System.Windows.Forms.ComboBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.txtCompanyEmail = new System.Windows.Forms.TextBox();
            this.lblInvoiceemail = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtPersonalFirst = new System.Windows.Forms.TextBox();
            this.txtPersonalLast = new System.Windows.Forms.TextBox();
            this.lblLater = new System.Windows.Forms.Label();
            this.lblAlready = new System.Windows.Forms.Label();
            this.rbPersonalContributing = new System.Windows.Forms.RadioButton();
            this.rbPersonalSupporting = new System.Windows.Forms.RadioButton();
            this.txtPersonalCountry = new System.Windows.Forms.TextBox();
            this.txtPersonalEmail = new System.Windows.Forms.TextBox();
            this.pics = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiLater = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAlready = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiNever = new System.Windows.Forms.ToolStripMenuItem();
            this.panPersonal = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblICountry = new System.Windows.Forms.Label();
            this.lblIEmail = new System.Windows.Forms.Label();
            this.lblIName = new System.Windows.Forms.Label();
            this.panInfo = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.helpText = new System.Windows.Forms.RichTextBox();
            this.helpTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panBgYellow = new System.Windows.Forms.Panel();
            this.panBgBlue = new System.Windows.Forms.Panel();
            this.btnInfoClose = new System.Windows.Forms.Button();
            this.btnWhatWhy = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panCorp.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panPersonal.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panInfo.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panBgYellow.SuspendLayout();
            this.panBgBlue.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Berlin Sans FB", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Yellow;
            this.lblHeader.Location = new System.Drawing.Point(48, 50);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(421, 38);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "[tool name]";
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
            // txtCompanyName
            // 
            this.txtCompanyName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtCompanyName.ForeColor = System.Drawing.Color.Yellow;
            this.txtCompanyName.Location = new System.Drawing.Point(148, 16);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(292, 25);
            this.txtCompanyName.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtCompanyName, "Name of your company");
            this.txtCompanyName.Validating += new System.ComponentModel.CancelEventHandler(this.ctrl_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "Supporting";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbPersonal);
            this.panel1.Controls.Add(this.rbCompany);
            this.panel1.Location = new System.Drawing.Point(169, 104);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(317, 26);
            this.panel1.TabIndex = 1;
            // 
            // rbPersonal
            // 
            this.rbPersonal.AutoSize = true;
            this.rbPersonal.ForeColor = System.Drawing.Color.Tan;
            this.rbPersonal.Location = new System.Drawing.Point(145, 2);
            this.rbPersonal.Name = "rbPersonal";
            this.rbPersonal.Size = new System.Drawing.Size(82, 22);
            this.rbPersonal.TabIndex = 1;
            this.rbPersonal.Text = "Personal";
            this.toolTip1.SetToolTip(this.rbPersonal, "My company may not understand this,\r\nbut I want to support it!");
            this.rbPersonal.UseVisualStyleBackColor = true;
            this.rbPersonal.CheckedChanged += new System.EventHandler(this.rbType_CheckedChanged);
            this.rbPersonal.MouseEnter += new System.EventHandler(this.lbl_MouseEnter);
            this.rbPersonal.MouseLeave += new System.EventHandler(this.lbl_MouseLeave);
            // 
            // rbCompany
            // 
            this.rbCompany.AutoSize = true;
            this.rbCompany.Checked = true;
            this.rbCompany.Location = new System.Drawing.Point(12, 2);
            this.rbCompany.Name = "rbCompany";
            this.rbCompany.Size = new System.Drawing.Size(92, 22);
            this.rbCompany.TabIndex = 0;
            this.rbCompany.TabStop = true;
            this.rbCompany.Text = "Corporate";
            this.toolTip1.SetToolTip(this.rbCompany, "Our company wants to support the developers,");
            this.rbCompany.UseVisualStyleBackColor = true;
            this.rbCompany.CheckedChanged += new System.EventHandler(this.rbType_CheckedChanged);
            this.rbCompany.MouseEnter += new System.EventHandler(this.lbl_MouseEnter);
            this.rbCompany.MouseLeave += new System.EventHandler(this.lbl_MouseLeave);
            // 
            // panCorp
            // 
            this.panCorp.Controls.Add(this.txtCompanyCountry);
            this.panCorp.Controls.Add(this.lblCountry);
            this.panCorp.Controls.Add(this.cmbCompanyUsers);
            this.panCorp.Controls.Add(this.lblSize);
            this.panCorp.Controls.Add(this.txtCompanyEmail);
            this.panCorp.Controls.Add(this.lblInvoiceemail);
            this.panCorp.Controls.Add(this.lblCompany);
            this.panCorp.Controls.Add(this.txtCompanyName);
            this.panCorp.Location = new System.Drawing.Point(30, 132);
            this.panCorp.Name = "panCorp";
            this.panCorp.Size = new System.Drawing.Size(456, 148);
            this.panCorp.TabIndex = 2;
            // 
            // txtCompanyCountry
            // 
            this.txtCompanyCountry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtCompanyCountry.ForeColor = System.Drawing.Color.Yellow;
            this.txtCompanyCountry.Location = new System.Drawing.Point(148, 78);
            this.txtCompanyCountry.Name = "txtCompanyCountry";
            this.txtCompanyCountry.Size = new System.Drawing.Size(292, 25);
            this.txtCompanyCountry.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtCompanyCountry, "Where is your company based?");
            this.txtCompanyCountry.Validating += new System.ComponentModel.CancelEventHandler(this.ctrl_Validating);
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
            // cmbCompanyUsers
            // 
            this.cmbCompanyUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.cmbCompanyUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCompanyUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCompanyUsers.ForeColor = System.Drawing.Color.Yellow;
            this.cmbCompanyUsers.FormattingEnabled = true;
            this.cmbCompanyUsers.Items.AddRange(new object[] {
            "",
            "1",
            "2-10",
            "11-50",
            "51-100",
            "101+"});
            this.cmbCompanyUsers.Location = new System.Drawing.Point(148, 109);
            this.cmbCompanyUsers.Name = "cmbCompanyUsers";
            this.cmbCompanyUsers.Size = new System.Drawing.Size(292, 26);
            this.cmbCompanyUsers.TabIndex = 4;
            this.toolTip1.SetToolTip(this.cmbCompanyUsers, "How many at your company are using this tool?");
            this.cmbCompanyUsers.Validating += new System.ComponentModel.CancelEventHandler(this.ctrl_Validating);
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(16, 112);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(73, 18);
            this.lblSize.TabIndex = 5;
            this.lblSize.Text = "Tool Users";
            // 
            // txtCompanyEmail
            // 
            this.txtCompanyEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtCompanyEmail.ForeColor = System.Drawing.Color.Yellow;
            this.txtCompanyEmail.Location = new System.Drawing.Point(148, 47);
            this.txtCompanyEmail.Name = "txtCompanyEmail";
            this.txtCompanyEmail.Size = new System.Drawing.Size(292, 25);
            this.txtCompanyEmail.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtCompanyEmail, "Email address for communication and where the receipt will be sent.");
            this.txtCompanyEmail.Validating += new System.ComponentModel.CancelEventHandler(this.ctrl_Validating);
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
            // txtPersonalFirst
            // 
            this.txtPersonalFirst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtPersonalFirst.ForeColor = System.Drawing.Color.Yellow;
            this.txtPersonalFirst.Location = new System.Drawing.Point(148, 16);
            this.txtPersonalFirst.Name = "txtPersonalFirst";
            this.txtPersonalFirst.Size = new System.Drawing.Size(141, 25);
            this.txtPersonalFirst.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtPersonalFirst, "First Name");
            this.txtPersonalFirst.Validating += new System.ComponentModel.CancelEventHandler(this.ctrl_Validating);
            // 
            // txtPersonalLast
            // 
            this.txtPersonalLast.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtPersonalLast.ForeColor = System.Drawing.Color.Yellow;
            this.txtPersonalLast.Location = new System.Drawing.Point(295, 16);
            this.txtPersonalLast.Name = "txtPersonalLast";
            this.txtPersonalLast.Size = new System.Drawing.Size(145, 25);
            this.txtPersonalLast.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtPersonalLast, "Last Name");
            this.txtPersonalLast.Validating += new System.ComponentModel.CancelEventHandler(this.ctrl_Validating);
            // 
            // lblLater
            // 
            this.lblLater.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLater.AutoSize = true;
            this.lblLater.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLater.ForeColor = System.Drawing.Color.Tan;
            this.lblLater.Location = new System.Drawing.Point(463, 380);
            this.lblLater.Name = "lblLater";
            this.lblLater.Size = new System.Drawing.Size(41, 18);
            this.lblLater.TabIndex = 101;
            this.lblLater.Text = "Close";
            this.lblLater.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblLater, "Close this window.\r\nYou will get a new chance to support later!");
            this.lblLater.Click += new System.EventHandler(this.lblLater_Click);
            this.lblLater.MouseEnter += new System.EventHandler(this.lbl_MouseEnter);
            this.lblLater.MouseLeave += new System.EventHandler(this.lbl_MouseLeave);
            // 
            // lblAlready
            // 
            this.lblAlready.AutoSize = true;
            this.lblAlready.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAlready.ForeColor = System.Drawing.Color.Tan;
            this.lblAlready.Location = new System.Drawing.Point(9, 344);
            this.lblAlready.Name = "lblAlready";
            this.lblAlready.Size = new System.Drawing.Size(82, 54);
            this.lblAlready.TabIndex = 102;
            this.lblAlready.Text = "I\'m already\r\nsupporting\r\n{tool}!";
            this.toolTip1.SetToolTip(this.lblAlready, "I have already supported this tool in one way or another!");
            this.lblAlready.Visible = false;
            this.lblAlready.Click += new System.EventHandler(this.lblAlready_Click);
            this.lblAlready.MouseEnter += new System.EventHandler(this.lbl_MouseEnter);
            this.lblAlready.MouseLeave += new System.EventHandler(this.lbl_MouseLeave);
            // 
            // rbPersonalContributing
            // 
            this.rbPersonalContributing.AutoSize = true;
            this.rbPersonalContributing.ForeColor = System.Drawing.Color.Tan;
            this.rbPersonalContributing.Location = new System.Drawing.Point(145, 3);
            this.rbPersonalContributing.Name = "rbPersonalContributing";
            this.rbPersonalContributing.Size = new System.Drawing.Size(105, 22);
            this.rbPersonalContributing.TabIndex = 1;
            this.rbPersonalContributing.Text = "Contribution";
            this.toolTip1.SetToolTip(this.rbPersonalContributing, "If you don\'t want to do any proper support, you can help Jonas\r\nwith development," +
        " fixing bugs, having new ideas, documentation, etc.");
            this.rbPersonalContributing.UseVisualStyleBackColor = true;
            this.rbPersonalContributing.MouseEnter += new System.EventHandler(this.lbl_MouseEnter);
            this.rbPersonalContributing.MouseLeave += new System.EventHandler(this.lbl_MouseLeave);
            // 
            // rbPersonalSupporting
            // 
            this.rbPersonalSupporting.AutoSize = true;
            this.rbPersonalSupporting.Checked = true;
            this.rbPersonalSupporting.Location = new System.Drawing.Point(12, 3);
            this.rbPersonalSupporting.Name = "rbPersonalSupporting";
            this.rbPersonalSupporting.Size = new System.Drawing.Size(96, 22);
            this.rbPersonalSupporting.TabIndex = 0;
            this.rbPersonalSupporting.TabStop = true;
            this.rbPersonalSupporting.Text = "Supporting";
            this.toolTip1.SetToolTip(this.rbPersonalSupporting, "Sharing is Caring ❤️\r\nHere, you can pay it forward!");
            this.rbPersonalSupporting.UseVisualStyleBackColor = true;
            this.rbPersonalSupporting.CheckedChanged += new System.EventHandler(this.rbPersonalMonetary_CheckedChanged);
            this.rbPersonalSupporting.MouseEnter += new System.EventHandler(this.lbl_MouseEnter);
            this.rbPersonalSupporting.MouseLeave += new System.EventHandler(this.lbl_MouseLeave);
            // 
            // txtPersonalCountry
            // 
            this.txtPersonalCountry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtPersonalCountry.ForeColor = System.Drawing.Color.Yellow;
            this.txtPersonalCountry.Location = new System.Drawing.Point(148, 78);
            this.txtPersonalCountry.Name = "txtPersonalCountry";
            this.txtPersonalCountry.Size = new System.Drawing.Size(292, 25);
            this.txtPersonalCountry.TabIndex = 4;
            this.toolTip1.SetToolTip(this.txtPersonalCountry, "Where are you from? Country name or country code.");
            this.txtPersonalCountry.Validating += new System.ComponentModel.CancelEventHandler(this.ctrl_Validating);
            // 
            // txtPersonalEmail
            // 
            this.txtPersonalEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.txtPersonalEmail.ForeColor = System.Drawing.Color.Yellow;
            this.txtPersonalEmail.Location = new System.Drawing.Point(148, 47);
            this.txtPersonalEmail.Name = "txtPersonalEmail";
            this.txtPersonalEmail.Size = new System.Drawing.Size(292, 25);
            this.txtPersonalEmail.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtPersonalEmail, "Email for any contact");
            this.txtPersonalEmail.Validating += new System.ComponentModel.CancelEventHandler(this.ctrl_Validating);
            // 
            // pics
            // 
            this.pics.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("pics.ImageStream")));
            this.pics.TransparentColor = System.Drawing.Color.Transparent;
            this.pics.Images.SetKeyName(0, "Corporate Supports Tools 2 narrow 200.png");
            this.pics.Images.SetKeyName(1, "I Support Tools narrow 200.png");
            this.pics.Images.SetKeyName(2, "I Contribute Tools narrow 200.png");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLater,
            this.tsmiAlready,
            this.toolStripMenuItem1,
            this.tsmiNever});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(250, 76);
            // 
            // tsmiLater
            // 
            this.tsmiLater.Name = "tsmiLater";
            this.tsmiLater.Size = new System.Drawing.Size(249, 22);
            this.tsmiLater.Text = "Not now, try later";
            this.tsmiLater.Click += new System.EventHandler(this.tsmiLater_Click);
            // 
            // tsmiAlready
            // 
            this.tsmiAlready.Name = "tsmiAlready";
            this.tsmiAlready.Size = new System.Drawing.Size(249, 22);
            this.tsmiAlready.Text = "I have already supported this tool";
            this.tsmiAlready.Click += new System.EventHandler(this.tsmiAlready_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(246, 6);
            // 
            // tsmiNever
            // 
            this.tsmiNever.Name = "tsmiNever";
            this.tsmiNever.Size = new System.Drawing.Size(249, 22);
            this.tsmiNever.Text = "I will never support this tool";
            this.tsmiNever.Click += new System.EventHandler(this.tsmiNever_Click);
            // 
            // panPersonal
            // 
            this.panPersonal.Controls.Add(this.label4);
            this.panPersonal.Controls.Add(this.panel3);
            this.panPersonal.Controls.Add(this.txtPersonalLast);
            this.panPersonal.Controls.Add(this.txtPersonalCountry);
            this.panPersonal.Controls.Add(this.lblICountry);
            this.panPersonal.Controls.Add(this.txtPersonalEmail);
            this.panPersonal.Controls.Add(this.lblIEmail);
            this.panPersonal.Controls.Add(this.lblIName);
            this.panPersonal.Controls.Add(this.txtPersonalFirst);
            this.panPersonal.Location = new System.Drawing.Point(30, 492);
            this.panPersonal.Name = "panPersonal";
            this.panPersonal.Size = new System.Drawing.Size(456, 148);
            this.panPersonal.TabIndex = 3;
            this.panPersonal.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 18);
            this.label4.TabIndex = 9;
            this.label4.Text = "Type";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbPersonalContributing);
            this.panel3.Controls.Add(this.rbPersonalSupporting);
            this.panel3.Location = new System.Drawing.Point(139, 107);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(301, 26);
            this.panel3.TabIndex = 8;
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
            this.panInfo.BackColor = System.Drawing.Color.Yellow;
            this.panInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panInfo.Controls.Add(this.panel2);
            this.panInfo.Location = new System.Drawing.Point(30, 663);
            this.panInfo.Name = "panInfo";
            this.panInfo.Size = new System.Drawing.Size(454, 376);
            this.panInfo.TabIndex = 11;
            this.panInfo.Visible = false;
            this.panInfo.VisibleChanged += new System.EventHandler(this.panInfo_VisibleChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(66)))), ((int)(((byte)(173)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnInfoClose);
            this.panel2.Controls.Add(this.helpText);
            this.panel2.Controls.Add(this.helpTitle);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(446, 368);
            this.panel2.TabIndex = 0;
            // 
            // helpText
            // 
            this.helpText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(66)))), ((int)(((byte)(173)))));
            this.helpText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.helpText.Font = new System.Drawing.Font("Berlin Sans FB", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpText.ForeColor = System.Drawing.Color.Yellow;
            this.helpText.Location = new System.Drawing.Point(16, 53);
            this.helpText.Name = "helpText";
            this.helpText.ReadOnly = true;
            this.helpText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.helpText.Size = new System.Drawing.Size(416, 302);
            this.helpText.TabIndex = 1;
            this.helpText.Text = resources.GetString("helpText.Text");
            this.helpText.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.helpText_LinkClicked);
            // 
            // helpTitle
            // 
            this.helpTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpTitle.Font = new System.Drawing.Font("Berlin Sans FB", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpTitle.Location = new System.Drawing.Point(12, 10);
            this.helpTitle.Name = "helpTitle";
            this.helpTitle.Size = new System.Drawing.Size(406, 35);
            this.helpTitle.TabIndex = 0;
            this.helpTitle.Text = "How and Why do we support?";
            this.helpTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Berlin Sans FB", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(175, 283);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(299, 30);
            this.label1.TabIndex = 103;
            this.label1.Text = "Click the button below to proceed your support!\r\nNote: You have to submit it at t" +
    "he next step to finalize it.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Berlin Sans FB", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point(48, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(421, 38);
            this.label2.TabIndex = 104;
            this.label2.Text = "We Support";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panBgYellow
            // 
            this.panBgYellow.BackColor = System.Drawing.Color.Yellow;
            this.panBgYellow.Controls.Add(this.panBgBlue);
            this.panBgYellow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panBgYellow.Location = new System.Drawing.Point(2, 2);
            this.panBgYellow.Name = "panBgYellow";
            this.panBgYellow.Padding = new System.Windows.Forms.Padding(4);
            this.panBgYellow.Size = new System.Drawing.Size(529, 426);
            this.panBgYellow.TabIndex = 105;
            // 
            // panBgBlue
            // 
            this.panBgBlue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(66)))), ((int)(((byte)(173)))));
            this.panBgBlue.Controls.Add(this.panInfo);
            this.panBgBlue.Controls.Add(this.btnWhatWhy);
            this.panBgBlue.Controls.Add(this.panPersonal);
            this.panBgBlue.Controls.Add(this.btnSubmit);
            this.panBgBlue.Controls.Add(this.panCorp);
            this.panBgBlue.Controls.Add(this.panel1);
            this.panBgBlue.Controls.Add(this.label3);
            this.panBgBlue.Controls.Add(this.lblHeader);
            this.panBgBlue.Controls.Add(this.lblLater);
            this.panBgBlue.Controls.Add(this.label1);
            this.panBgBlue.Controls.Add(this.label2);
            this.panBgBlue.Controls.Add(this.lblAlready);
            this.panBgBlue.Controls.Add(this.btnInfo);
            this.panBgBlue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panBgBlue.Location = new System.Drawing.Point(4, 4);
            this.panBgBlue.Name = "panBgBlue";
            this.panBgBlue.Size = new System.Drawing.Size(521, 418);
            this.panBgBlue.TabIndex = 0;
            // 
            // btnInfoClose
            // 
            this.btnInfoClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfoClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInfoClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.btnInfoClose.Image = ((System.Drawing.Image)(resources.GetObject("btnInfoClose.Image")));
            this.btnInfoClose.Location = new System.Drawing.Point(411, 3);
            this.btnInfoClose.Name = "btnInfoClose";
            this.btnInfoClose.Size = new System.Drawing.Size(30, 30);
            this.btnInfoClose.TabIndex = 13;
            this.btnInfoClose.UseVisualStyleBackColor = true;
            this.btnInfoClose.Click += new System.EventHandler(this.btnInfoClose_Click);
            // 
            // btnWhatWhy
            // 
            this.btnWhatWhy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWhatWhy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.btnWhatWhy.Image = ((System.Drawing.Image)(resources.GetObject("btnWhatWhy.Image")));
            this.btnWhatWhy.Location = new System.Drawing.Point(12, 12);
            this.btnWhatWhy.Name = "btnWhatWhy";
            this.btnWhatWhy.Size = new System.Drawing.Size(30, 30);
            this.btnWhatWhy.TabIndex = 12;
            this.toolTip1.SetToolTip(this.btnWhatWhy, "Information about WHY we should support tools - read my thoughts!");
            this.btnWhatWhy.UseVisualStyleBackColor = true;
            this.btnWhatWhy.Click += new System.EventHandler(this.btnWhatWhy_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.ImageIndex = 0;
            this.btnSubmit.ImageList = this.pics;
            this.btnSubmit.Location = new System.Drawing.Point(178, 330);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(8);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Padding = new System.Windows.Forms.Padding(8);
            this.btnSubmit.Size = new System.Drawing.Size(210, 68);
            this.btnSubmit.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnSubmit, "Click to forward you to the supporting form on JonasR.app.\r\nSubmitting will bring" +
        " the information here into the following step online.");
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.ContextMenuStrip = this.contextMenuStrip1;
            this.btnInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.btnInfo.Image = ((System.Drawing.Image)(resources.GetObject("btnInfo.Image")));
            this.btnInfo.Location = new System.Drawing.Point(475, 12);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(30, 30);
            this.btnInfo.TabIndex = 105;
            this.toolTip1.SetToolTip(this.btnInfo, "Technical information about how this works,\r\nwhat is stored, where it is stored, " +
        "how to stop\r\npromping about supporting, etc.");
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // Supporting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(533, 430);
            this.ControlBox = false;
            this.Controls.Add(this.panBgYellow);
            this.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Yellow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Supporting";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "We Support Tools";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Supporting_FormClosing);
            this.Shown += new System.EventHandler(this.Supporting_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panCorp.ResumeLayout(false);
            this.panCorp.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panPersonal.ResumeLayout(false);
            this.panPersonal.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panInfo.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panBgYellow.ResumeLayout(false);
            this.panBgBlue.ResumeLayout(false);
            this.panBgBlue.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblCompany;
        private System.Windows.Forms.TextBox txtCompanyName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbPersonal;
        private System.Windows.Forms.RadioButton rbCompany;
        private System.Windows.Forms.Panel panCorp;
        private System.Windows.Forms.TextBox txtCompanyEmail;
        private System.Windows.Forms.Label lblInvoiceemail;
        private System.Windows.Forms.ComboBox cmbCompanyUsers;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.TextBox txtCompanyCountry;
        private System.Windows.Forms.Label lblCountry;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panPersonal;
        private System.Windows.Forms.TextBox txtPersonalLast;
        private System.Windows.Forms.TextBox txtPersonalCountry;
        private System.Windows.Forms.Label lblICountry;
        private System.Windows.Forms.TextBox txtPersonalEmail;
        private System.Windows.Forms.Label lblIEmail;
        private System.Windows.Forms.Label lblIName;
        private System.Windows.Forms.TextBox txtPersonalFirst;
        private System.Windows.Forms.Panel panInfo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox helpText;
        private System.Windows.Forms.Label helpTitle;
        private System.Windows.Forms.Button btnWhatWhy;
        private System.Windows.Forms.Button btnInfoClose;
        private System.Windows.Forms.Label lblLater;
        private System.Windows.Forms.Label lblAlready;
        private System.Windows.Forms.ImageList pics;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbPersonalContributing;
        private System.Windows.Forms.RadioButton rbPersonalSupporting;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiLater;
        private System.Windows.Forms.ToolStripMenuItem tsmiNever;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panBgYellow;
        private System.Windows.Forms.Panel panBgBlue;
        private System.Windows.Forms.ToolStripMenuItem tsmiAlready;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.Button btnInfo;
    }
}