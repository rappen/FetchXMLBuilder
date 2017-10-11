namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.gbEntities = new System.Windows.Forms.GroupBox();
            this.chkEntOnlyAF = new System.Windows.Forms.CheckBox();
            this.chkEntIntersect = new System.Windows.Forms.CheckBox();
            this.chkEntStandard = new System.Windows.Forms.CheckBox();
            this.chkEntCustom = new System.Windows.Forms.CheckBox();
            this.chkEntUncustomizable = new System.Windows.Forms.CheckBox();
            this.chkEntCustomizable = new System.Windows.Forms.CheckBox();
            this.chkEntUnmanaged = new System.Windows.Forms.CheckBox();
            this.chkEntManaged = new System.Windows.Forms.CheckBox();
            this.chkEntAll = new System.Windows.Forms.CheckBox();
            this.gbAttributes = new System.Windows.Forms.GroupBox();
            this.chkAttOnlyRead = new System.Windows.Forms.CheckBox();
            this.chkAttOnlyAF = new System.Windows.Forms.CheckBox();
            this.chkAttStandard = new System.Windows.Forms.CheckBox();
            this.chkAttCustom = new System.Windows.Forms.CheckBox();
            this.chkAttUncustomizable = new System.Windows.Forms.CheckBox();
            this.chkAttCustomizable = new System.Windows.Forms.CheckBox();
            this.chkAttUnmanaged = new System.Windows.Forms.CheckBox();
            this.chkAttManaged = new System.Windows.Forms.CheckBox();
            this.chkAttAll = new System.Windows.Forms.CheckBox();
            this.gbResult = new System.Windows.Forms.GroupBox();
            this.chkResAllPages = new System.Windows.Forms.CheckBox();
            this.cmbSeralizationStyle = new System.Windows.Forms.ComboBox();
            this.rbResRaw = new System.Windows.Forms.RadioButton();
            this.rbResSerialized = new System.Windows.Forms.RadioButton();
            this.rbResGrid = new System.Windows.Forms.RadioButton();
            this.bgStats = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkStatAllow = new System.Windows.Forms.CheckBox();
            this.gbAppearance = new System.Windows.Forms.GroupBox();
            this.chkAppNoSavePrompt = new System.Windows.Forms.CheckBox();
            this.chkAppSingle = new System.Windows.Forms.CheckBox();
            this.chkAppQuick = new System.Windows.Forms.CheckBox();
            this.chkAppFriendly = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbEntities.SuspendLayout();
            this.gbAttributes.SuspendLayout();
            this.gbResult.SuspendLayout();
            this.bgStats.SuspendLayout();
            this.gbAppearance.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbEntities
            // 
            this.gbEntities.Controls.Add(this.chkEntOnlyAF);
            this.gbEntities.Controls.Add(this.chkEntIntersect);
            this.gbEntities.Controls.Add(this.chkEntStandard);
            this.gbEntities.Controls.Add(this.chkEntCustom);
            this.gbEntities.Controls.Add(this.chkEntUncustomizable);
            this.gbEntities.Controls.Add(this.chkEntCustomizable);
            this.gbEntities.Controls.Add(this.chkEntUnmanaged);
            this.gbEntities.Controls.Add(this.chkEntManaged);
            this.gbEntities.Controls.Add(this.chkEntAll);
            this.gbEntities.Location = new System.Drawing.Point(12, 166);
            this.gbEntities.Name = "gbEntities";
            this.gbEntities.Size = new System.Drawing.Size(247, 258);
            this.gbEntities.TabIndex = 2;
            this.gbEntities.TabStop = false;
            this.gbEntities.Text = "Show Entities";
            // 
            // chkEntOnlyAF
            // 
            this.chkEntOnlyAF.AutoSize = true;
            this.chkEntOnlyAF.Enabled = false;
            this.chkEntOnlyAF.Location = new System.Drawing.Point(26, 214);
            this.chkEntOnlyAF.Name = "chkEntOnlyAF";
            this.chkEntOnlyAF.Size = new System.Drawing.Size(132, 17);
            this.chkEntOnlyAF.TabIndex = 8;
            this.chkEntOnlyAF.Text = "Only valid for Adv.Find";
            this.chkEntOnlyAF.UseVisualStyleBackColor = true;
            // 
            // chkEntIntersect
            // 
            this.chkEntIntersect.AutoSize = true;
            this.chkEntIntersect.Checked = true;
            this.chkEntIntersect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEntIntersect.Enabled = false;
            this.chkEntIntersect.Location = new System.Drawing.Point(26, 191);
            this.chkEntIntersect.Name = "chkEntIntersect";
            this.chkEntIntersect.Size = new System.Drawing.Size(67, 17);
            this.chkEntIntersect.TabIndex = 7;
            this.chkEntIntersect.Text = "Intersect";
            this.chkEntIntersect.UseVisualStyleBackColor = true;
            // 
            // chkEntStandard
            // 
            this.chkEntStandard.AutoSize = true;
            this.chkEntStandard.Checked = true;
            this.chkEntStandard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEntStandard.Enabled = false;
            this.chkEntStandard.Location = new System.Drawing.Point(26, 168);
            this.chkEntStandard.Name = "chkEntStandard";
            this.chkEntStandard.Size = new System.Drawing.Size(69, 17);
            this.chkEntStandard.TabIndex = 6;
            this.chkEntStandard.Text = "Standard";
            this.chkEntStandard.UseVisualStyleBackColor = true;
            this.chkEntStandard.CheckedChanged += new System.EventHandler(this.chkEntCustom_CheckedChanged);
            // 
            // chkEntCustom
            // 
            this.chkEntCustom.AutoSize = true;
            this.chkEntCustom.Checked = true;
            this.chkEntCustom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEntCustom.Enabled = false;
            this.chkEntCustom.Location = new System.Drawing.Point(26, 145);
            this.chkEntCustom.Name = "chkEntCustom";
            this.chkEntCustom.Size = new System.Drawing.Size(61, 17);
            this.chkEntCustom.TabIndex = 5;
            this.chkEntCustom.Text = "Custom";
            this.chkEntCustom.UseVisualStyleBackColor = true;
            this.chkEntCustom.CheckedChanged += new System.EventHandler(this.chkEntCustom_CheckedChanged);
            // 
            // chkEntUncustomizable
            // 
            this.chkEntUncustomizable.AutoSize = true;
            this.chkEntUncustomizable.Checked = true;
            this.chkEntUncustomizable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEntUncustomizable.Enabled = false;
            this.chkEntUncustomizable.Location = new System.Drawing.Point(26, 122);
            this.chkEntUncustomizable.Name = "chkEntUncustomizable";
            this.chkEntUncustomizable.Size = new System.Drawing.Size(101, 17);
            this.chkEntUncustomizable.TabIndex = 4;
            this.chkEntUncustomizable.Text = "Uncustomizable";
            this.chkEntUncustomizable.UseVisualStyleBackColor = true;
            this.chkEntUncustomizable.CheckedChanged += new System.EventHandler(this.chkEntCust_CheckedChanged);
            // 
            // chkEntCustomizable
            // 
            this.chkEntCustomizable.AutoSize = true;
            this.chkEntCustomizable.Checked = true;
            this.chkEntCustomizable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEntCustomizable.Enabled = false;
            this.chkEntCustomizable.Location = new System.Drawing.Point(26, 99);
            this.chkEntCustomizable.Name = "chkEntCustomizable";
            this.chkEntCustomizable.Size = new System.Drawing.Size(88, 17);
            this.chkEntCustomizable.TabIndex = 3;
            this.chkEntCustomizable.Text = "Customizable";
            this.chkEntCustomizable.UseVisualStyleBackColor = true;
            this.chkEntCustomizable.CheckedChanged += new System.EventHandler(this.chkEntCust_CheckedChanged);
            // 
            // chkEntUnmanaged
            // 
            this.chkEntUnmanaged.AutoSize = true;
            this.chkEntUnmanaged.Checked = true;
            this.chkEntUnmanaged.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEntUnmanaged.Enabled = false;
            this.chkEntUnmanaged.Location = new System.Drawing.Point(26, 76);
            this.chkEntUnmanaged.Name = "chkEntUnmanaged";
            this.chkEntUnmanaged.Size = new System.Drawing.Size(84, 17);
            this.chkEntUnmanaged.TabIndex = 2;
            this.chkEntUnmanaged.Text = "Unmanaged";
            this.chkEntUnmanaged.UseVisualStyleBackColor = true;
            this.chkEntUnmanaged.CheckedChanged += new System.EventHandler(this.chkEntMgd_CheckedChanged);
            // 
            // chkEntManaged
            // 
            this.chkEntManaged.AutoSize = true;
            this.chkEntManaged.Checked = true;
            this.chkEntManaged.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEntManaged.Enabled = false;
            this.chkEntManaged.Location = new System.Drawing.Point(26, 53);
            this.chkEntManaged.Name = "chkEntManaged";
            this.chkEntManaged.Size = new System.Drawing.Size(71, 17);
            this.chkEntManaged.TabIndex = 1;
            this.chkEntManaged.Text = "Managed";
            this.chkEntManaged.UseVisualStyleBackColor = true;
            this.chkEntManaged.CheckedChanged += new System.EventHandler(this.chkEntMgd_CheckedChanged);
            // 
            // chkEntAll
            // 
            this.chkEntAll.AutoSize = true;
            this.chkEntAll.Checked = true;
            this.chkEntAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEntAll.Location = new System.Drawing.Point(20, 30);
            this.chkEntAll.Name = "chkEntAll";
            this.chkEntAll.Size = new System.Drawing.Size(37, 17);
            this.chkEntAll.TabIndex = 0;
            this.chkEntAll.Text = "All";
            this.chkEntAll.UseVisualStyleBackColor = true;
            this.chkEntAll.CheckedChanged += new System.EventHandler(this.chkEntAll_CheckedChanged);
            // 
            // gbAttributes
            // 
            this.gbAttributes.Controls.Add(this.chkAttOnlyRead);
            this.gbAttributes.Controls.Add(this.chkAttOnlyAF);
            this.gbAttributes.Controls.Add(this.chkAttStandard);
            this.gbAttributes.Controls.Add(this.chkAttCustom);
            this.gbAttributes.Controls.Add(this.chkAttUncustomizable);
            this.gbAttributes.Controls.Add(this.chkAttCustomizable);
            this.gbAttributes.Controls.Add(this.chkAttUnmanaged);
            this.gbAttributes.Controls.Add(this.chkAttManaged);
            this.gbAttributes.Controls.Add(this.chkAttAll);
            this.gbAttributes.Location = new System.Drawing.Point(284, 166);
            this.gbAttributes.Name = "gbAttributes";
            this.gbAttributes.Size = new System.Drawing.Size(247, 258);
            this.gbAttributes.TabIndex = 3;
            this.gbAttributes.TabStop = false;
            this.gbAttributes.Text = "Show Attributes";
            // 
            // chkAttOnlyRead
            // 
            this.chkAttOnlyRead.AutoSize = true;
            this.chkAttOnlyRead.Enabled = false;
            this.chkAttOnlyRead.Location = new System.Drawing.Point(26, 214);
            this.chkAttOnlyRead.Name = "chkAttOnlyRead";
            this.chkAttOnlyRead.Size = new System.Drawing.Size(116, 17);
            this.chkAttOnlyRead.TabIndex = 9;
            this.chkAttOnlyRead.Text = "Only valid for Read";
            this.chkAttOnlyRead.UseVisualStyleBackColor = true;
            // 
            // chkAttOnlyAF
            // 
            this.chkAttOnlyAF.AutoSize = true;
            this.chkAttOnlyAF.Enabled = false;
            this.chkAttOnlyAF.Location = new System.Drawing.Point(26, 191);
            this.chkAttOnlyAF.Name = "chkAttOnlyAF";
            this.chkAttOnlyAF.Size = new System.Drawing.Size(132, 17);
            this.chkAttOnlyAF.TabIndex = 8;
            this.chkAttOnlyAF.Text = "Only valid for Adv.Find";
            this.chkAttOnlyAF.UseVisualStyleBackColor = true;
            // 
            // chkAttStandard
            // 
            this.chkAttStandard.AutoSize = true;
            this.chkAttStandard.Checked = true;
            this.chkAttStandard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAttStandard.Enabled = false;
            this.chkAttStandard.Location = new System.Drawing.Point(26, 168);
            this.chkAttStandard.Name = "chkAttStandard";
            this.chkAttStandard.Size = new System.Drawing.Size(69, 17);
            this.chkAttStandard.TabIndex = 6;
            this.chkAttStandard.Text = "Standard";
            this.chkAttStandard.UseVisualStyleBackColor = true;
            this.chkAttStandard.CheckedChanged += new System.EventHandler(this.chkAttCustom_CheckedChanged);
            // 
            // chkAttCustom
            // 
            this.chkAttCustom.AutoSize = true;
            this.chkAttCustom.Checked = true;
            this.chkAttCustom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAttCustom.Enabled = false;
            this.chkAttCustom.Location = new System.Drawing.Point(26, 145);
            this.chkAttCustom.Name = "chkAttCustom";
            this.chkAttCustom.Size = new System.Drawing.Size(61, 17);
            this.chkAttCustom.TabIndex = 5;
            this.chkAttCustom.Text = "Custom";
            this.chkAttCustom.UseVisualStyleBackColor = true;
            this.chkAttCustom.CheckedChanged += new System.EventHandler(this.chkAttCustom_CheckedChanged);
            // 
            // chkAttUncustomizable
            // 
            this.chkAttUncustomizable.AutoSize = true;
            this.chkAttUncustomizable.Checked = true;
            this.chkAttUncustomizable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAttUncustomizable.Enabled = false;
            this.chkAttUncustomizable.Location = new System.Drawing.Point(26, 122);
            this.chkAttUncustomizable.Name = "chkAttUncustomizable";
            this.chkAttUncustomizable.Size = new System.Drawing.Size(101, 17);
            this.chkAttUncustomizable.TabIndex = 4;
            this.chkAttUncustomizable.Text = "Uncustomizable";
            this.chkAttUncustomizable.UseVisualStyleBackColor = true;
            this.chkAttUncustomizable.CheckedChanged += new System.EventHandler(this.chkAttCust_CheckedChanged);
            // 
            // chkAttCustomizable
            // 
            this.chkAttCustomizable.AutoSize = true;
            this.chkAttCustomizable.Checked = true;
            this.chkAttCustomizable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAttCustomizable.Enabled = false;
            this.chkAttCustomizable.Location = new System.Drawing.Point(26, 99);
            this.chkAttCustomizable.Name = "chkAttCustomizable";
            this.chkAttCustomizable.Size = new System.Drawing.Size(88, 17);
            this.chkAttCustomizable.TabIndex = 3;
            this.chkAttCustomizable.Text = "Customizable";
            this.chkAttCustomizable.UseVisualStyleBackColor = true;
            this.chkAttCustomizable.CheckedChanged += new System.EventHandler(this.chkAttCust_CheckedChanged);
            // 
            // chkAttUnmanaged
            // 
            this.chkAttUnmanaged.AutoSize = true;
            this.chkAttUnmanaged.Checked = true;
            this.chkAttUnmanaged.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAttUnmanaged.Enabled = false;
            this.chkAttUnmanaged.Location = new System.Drawing.Point(26, 76);
            this.chkAttUnmanaged.Name = "chkAttUnmanaged";
            this.chkAttUnmanaged.Size = new System.Drawing.Size(84, 17);
            this.chkAttUnmanaged.TabIndex = 2;
            this.chkAttUnmanaged.Text = "Unmanaged";
            this.chkAttUnmanaged.UseVisualStyleBackColor = true;
            this.chkAttUnmanaged.CheckedChanged += new System.EventHandler(this.chkAttMgd_CheckedChanged);
            // 
            // chkAttManaged
            // 
            this.chkAttManaged.AutoSize = true;
            this.chkAttManaged.Checked = true;
            this.chkAttManaged.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAttManaged.Enabled = false;
            this.chkAttManaged.Location = new System.Drawing.Point(26, 53);
            this.chkAttManaged.Name = "chkAttManaged";
            this.chkAttManaged.Size = new System.Drawing.Size(71, 17);
            this.chkAttManaged.TabIndex = 1;
            this.chkAttManaged.Text = "Managed";
            this.chkAttManaged.UseVisualStyleBackColor = true;
            this.chkAttManaged.CheckedChanged += new System.EventHandler(this.chkAttMgd_CheckedChanged);
            // 
            // chkAttAll
            // 
            this.chkAttAll.AutoSize = true;
            this.chkAttAll.Checked = true;
            this.chkAttAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAttAll.Location = new System.Drawing.Point(16, 30);
            this.chkAttAll.Name = "chkAttAll";
            this.chkAttAll.Size = new System.Drawing.Size(37, 17);
            this.chkAttAll.TabIndex = 0;
            this.chkAttAll.Text = "All";
            this.chkAttAll.UseVisualStyleBackColor = true;
            this.chkAttAll.CheckedChanged += new System.EventHandler(this.chkAttAll_CheckedChanged);
            // 
            // gbResult
            // 
            this.gbResult.Controls.Add(this.chkResAllPages);
            this.gbResult.Controls.Add(this.cmbSeralizationStyle);
            this.gbResult.Controls.Add(this.rbResRaw);
            this.gbResult.Controls.Add(this.rbResSerialized);
            this.gbResult.Controls.Add(this.rbResGrid);
            this.gbResult.Location = new System.Drawing.Point(284, 12);
            this.gbResult.Name = "gbResult";
            this.gbResult.Size = new System.Drawing.Size(247, 137);
            this.gbResult.TabIndex = 1;
            this.gbResult.TabStop = false;
            this.gbResult.Text = "Result view";
            // 
            // chkResAllPages
            // 
            this.chkResAllPages.AutoSize = true;
            this.chkResAllPages.Location = new System.Drawing.Point(20, 106);
            this.chkResAllPages.Name = "chkResAllPages";
            this.chkResAllPages.Size = new System.Drawing.Size(111, 17);
            this.chkResAllPages.TabIndex = 4;
            this.chkResAllPages.Text = "Retrieve all pages";
            this.chkResAllPages.UseVisualStyleBackColor = true;
            // 
            // cmbSeralizationStyle
            // 
            this.cmbSeralizationStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSeralizationStyle.Enabled = false;
            this.cmbSeralizationStyle.FormattingEnabled = true;
            this.cmbSeralizationStyle.Items.AddRange(new object[] {
            "Explicit XML",
            "Basic XML",
            "JSON",
            "EntityCollection"});
            this.cmbSeralizationStyle.Location = new System.Drawing.Point(96, 51);
            this.cmbSeralizationStyle.Name = "cmbSeralizationStyle";
            this.cmbSeralizationStyle.Size = new System.Drawing.Size(145, 21);
            this.cmbSeralizationStyle.TabIndex = 2;
            // 
            // rbResRaw
            // 
            this.rbResRaw.AutoSize = true;
            this.rbResRaw.Location = new System.Drawing.Point(20, 75);
            this.rbResRaw.Name = "rbResRaw";
            this.rbResRaw.Size = new System.Drawing.Size(102, 17);
            this.rbResRaw.TabIndex = 3;
            this.rbResRaw.Text = "Raw fetch result";
            this.rbResRaw.UseVisualStyleBackColor = true;
            // 
            // rbResSerialized
            // 
            this.rbResSerialized.AutoSize = true;
            this.rbResSerialized.Location = new System.Drawing.Point(20, 53);
            this.rbResSerialized.Name = "rbResSerialized";
            this.rbResSerialized.Size = new System.Drawing.Size(70, 17);
            this.rbResSerialized.TabIndex = 1;
            this.rbResSerialized.Text = "Serialized";
            this.rbResSerialized.UseVisualStyleBackColor = true;
            this.rbResSerialized.CheckedChanged += new System.EventHandler(this.rbResSerialized_CheckedChanged);
            // 
            // rbResGrid
            // 
            this.rbResGrid.AutoSize = true;
            this.rbResGrid.Checked = true;
            this.rbResGrid.Location = new System.Drawing.Point(20, 30);
            this.rbResGrid.Name = "rbResGrid";
            this.rbResGrid.Size = new System.Drawing.Size(72, 17);
            this.rbResGrid.TabIndex = 0;
            this.rbResGrid.TabStop = true;
            this.rbResGrid.Text = "Table grid";
            this.rbResGrid.UseVisualStyleBackColor = true;
            // 
            // bgStats
            // 
            this.bgStats.Controls.Add(this.label1);
            this.bgStats.Controls.Add(this.chkStatAllow);
            this.bgStats.Location = new System.Drawing.Point(12, 443);
            this.bgStats.Name = "bgStats";
            this.bgStats.Size = new System.Drawing.Size(517, 130);
            this.bgStats.TabIndex = 4;
            this.bgStats.TabStop = false;
            this.bgStats.Text = "Statistics";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(504, 72);
            this.label1.TabIndex = 6;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // chkStatAllow
            // 
            this.chkStatAllow.AutoSize = true;
            this.chkStatAllow.Location = new System.Drawing.Point(26, 95);
            this.chkStatAllow.Name = "chkStatAllow";
            this.chkStatAllow.Size = new System.Drawing.Size(94, 17);
            this.chkStatAllow.TabIndex = 5;
            this.chkStatAllow.Text = "Allow statistics";
            this.chkStatAllow.UseVisualStyleBackColor = true;
            this.chkStatAllow.CheckedChanged += new System.EventHandler(this.chkStatAllow_CheckedChanged);
            // 
            // gbAppearance
            // 
            this.gbAppearance.Controls.Add(this.chkAppNoSavePrompt);
            this.gbAppearance.Controls.Add(this.chkAppSingle);
            this.gbAppearance.Controls.Add(this.chkAppQuick);
            this.gbAppearance.Controls.Add(this.chkAppFriendly);
            this.gbAppearance.Location = new System.Drawing.Point(12, 12);
            this.gbAppearance.Name = "gbAppearance";
            this.gbAppearance.Size = new System.Drawing.Size(247, 137);
            this.gbAppearance.TabIndex = 0;
            this.gbAppearance.TabStop = false;
            this.gbAppearance.Text = "Appearance";
            // 
            // chkAppNoSavePrompt
            // 
            this.chkAppNoSavePrompt.AutoSize = true;
            this.chkAppNoSavePrompt.Location = new System.Drawing.Point(20, 99);
            this.chkAppNoSavePrompt.Name = "chkAppNoSavePrompt";
            this.chkAppNoSavePrompt.Size = new System.Drawing.Size(159, 17);
            this.chkAppNoSavePrompt.TabIndex = 4;
            this.chkAppNoSavePrompt.Text = "Do not prompt to save to file";
            this.chkAppNoSavePrompt.UseVisualStyleBackColor = true;
            // 
            // chkAppSingle
            // 
            this.chkAppSingle.AutoSize = true;
            this.chkAppSingle.Location = new System.Drawing.Point(20, 76);
            this.chkAppSingle.Name = "chkAppSingle";
            this.chkAppSingle.Size = new System.Drawing.Size(203, 17);
            this.chkAppSingle.TabIndex = 3;
            this.chkAppSingle.Text = "Use single quotation in rendered XML";
            this.chkAppSingle.UseVisualStyleBackColor = true;
            // 
            // chkAppQuick
            // 
            this.chkAppQuick.AutoSize = true;
            this.chkAppQuick.Checked = true;
            this.chkAppQuick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAppQuick.Location = new System.Drawing.Point(20, 53);
            this.chkAppQuick.Name = "chkAppQuick";
            this.chkAppQuick.Size = new System.Drawing.Size(155, 17);
            this.chkAppQuick.TabIndex = 2;
            this.chkAppQuick.Text = "Show Quick Action buttons";
            this.chkAppQuick.UseVisualStyleBackColor = true;
            // 
            // chkAppFriendly
            // 
            this.chkAppFriendly.AutoSize = true;
            this.chkAppFriendly.Location = new System.Drawing.Point(20, 30);
            this.chkAppFriendly.Name = "chkAppFriendly";
            this.chkAppFriendly.Size = new System.Drawing.Size(151, 17);
            this.chkAppFriendly.TabIndex = 1;
            this.chkAppFriendly.Text = "Friendly names (CTRL+F)";
            this.chkAppFriendly.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Location = new System.Drawing.Point(12, 579);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(517, 53);
            this.panel1.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(436, 19);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(353, 19);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(543, 642);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gbAppearance);
            this.Controls.Add(this.bgStats);
            this.Controls.Add(this.gbResult);
            this.Controls.Add(this.gbAttributes);
            this.Controls.Add(this.gbEntities);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.gbEntities.ResumeLayout(false);
            this.gbEntities.PerformLayout();
            this.gbAttributes.ResumeLayout(false);
            this.gbAttributes.PerformLayout();
            this.gbResult.ResumeLayout(false);
            this.gbResult.PerformLayout();
            this.bgStats.ResumeLayout(false);
            this.bgStats.PerformLayout();
            this.gbAppearance.ResumeLayout(false);
            this.gbAppearance.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbEntities;
        private System.Windows.Forms.CheckBox chkEntAll;
        private System.Windows.Forms.CheckBox chkEntOnlyAF;
        private System.Windows.Forms.CheckBox chkEntIntersect;
        private System.Windows.Forms.CheckBox chkEntStandard;
        private System.Windows.Forms.CheckBox chkEntCustom;
        private System.Windows.Forms.CheckBox chkEntUncustomizable;
        private System.Windows.Forms.CheckBox chkEntCustomizable;
        private System.Windows.Forms.CheckBox chkEntUnmanaged;
        private System.Windows.Forms.CheckBox chkEntManaged;
        private System.Windows.Forms.GroupBox gbAttributes;
        private System.Windows.Forms.CheckBox chkAttOnlyRead;
        private System.Windows.Forms.CheckBox chkAttOnlyAF;
        private System.Windows.Forms.CheckBox chkAttStandard;
        private System.Windows.Forms.CheckBox chkAttCustom;
        private System.Windows.Forms.CheckBox chkAttUncustomizable;
        private System.Windows.Forms.CheckBox chkAttCustomizable;
        private System.Windows.Forms.CheckBox chkAttUnmanaged;
        private System.Windows.Forms.CheckBox chkAttManaged;
        private System.Windows.Forms.CheckBox chkAttAll;
        private System.Windows.Forms.GroupBox gbResult;
        private System.Windows.Forms.GroupBox bgStats;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkStatAllow;
        private System.Windows.Forms.GroupBox gbAppearance;
        private System.Windows.Forms.CheckBox chkAppSingle;
        private System.Windows.Forms.CheckBox chkAppQuick;
        private System.Windows.Forms.CheckBox chkAppFriendly;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbResGrid;
        private System.Windows.Forms.RadioButton rbResRaw;
        private System.Windows.Forms.RadioButton rbResSerialized;
        private System.Windows.Forms.ComboBox cmbSeralizationStyle;
        private System.Windows.Forms.CheckBox chkResAllPages;
        private System.Windows.Forms.CheckBox chkAppNoSavePrompt;
    }
}