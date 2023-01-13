using Rappen.XTB.FetchXmlBuilder.Settings;

namespace Rappen.XTB.FetchXmlBuilder.Forms
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
            Rappen.XTB.FetchXmlBuilder.Settings.XmlColors xmlColors1 = new Rappen.XTB.FetchXmlBuilder.Settings.XmlColors();
            this.cmbResult = new System.Windows.Forms.ComboBox();
            this.chkResAllPages = new System.Windows.Forms.CheckBox();
            this.chkAppResultsNewWindow = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numMaxColumnWidth = new System.Windows.Forms.NumericUpDown();
            this.chkClickableLinks = new System.Windows.Forms.CheckBox();
            this.chkAlwaysShowAggregateProperties = new System.Windows.Forms.CheckBox();
            this.chkAppFriendlyResults = new System.Windows.Forms.CheckBox();
            this.chkUseLookup = new System.Windows.Forms.CheckBox();
            this.chkAppSingle = new System.Windows.Forms.CheckBox();
            this.chkAppFriendly = new System.Windows.Forms.CheckBox();
            this.chkShowButtonTexts = new System.Windows.Forms.CheckBox();
            this.chkShowNodeTypes = new System.Windows.Forms.CheckBox();
            this.chkShowHelp = new System.Windows.Forms.CheckBox();
            this.chkAppAllowUncustViews = new System.Windows.Forms.CheckBox();
            this.chkUseSQL4CDS = new System.Windows.Forms.CheckBox();
            this.chkAppNoSavePrompt = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.llShowWelcome = new System.Windows.Forms.LinkLabel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtFetch = new ScintillaNET.Scintilla();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnDefaultQuery = new System.Windows.Forms.Button();
            this.btnFormatQuery = new System.Windows.Forms.Button();
            this.btnResetXmlColors = new System.Windows.Forms.Button();
            this.chkShowValidationInfo = new System.Windows.Forms.CheckBox();
            this.chkShowValidation = new System.Windows.Forms.CheckBox();
            this.chkAddConditionToFilter = new System.Windows.Forms.CheckBox();
            this.chkShowRepository = new System.Windows.Forms.CheckBox();
            this.chkShowAllAttributes = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.chkWaitUntilMetadataLoaded = new System.Windows.Forms.CheckBox();
            this.chkTryMetadataCache = new System.Windows.Forms.CheckBox();
            this.linkLayout = new System.Windows.Forms.LinkLabel();
            this.chkWorkWithLayout = new System.Windows.Forms.CheckBox();
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabAppearance = new System.Windows.Forms.TabPage();
            this.tabLayout = new System.Windows.Forms.TabPage();
            this.tabBehavior = new System.Windows.Forms.TabPage();
            this.tabResults = new System.Windows.Forms.TabPage();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.tabDefaultQuery = new System.Windows.Forms.TabPage();
            this.tabXmlScheme = new System.Windows.Forms.TabPage();
            this.panResultView = new System.Windows.Forms.Panel();
            this.propXmlColors = new System.Windows.Forms.PropertyGrid();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxColumnWidth)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabAppearance.SuspendLayout();
            this.tabLayout.SuspendLayout();
            this.tabBehavior.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.tabAdvanced.SuspendLayout();
            this.tabDefaultQuery.SuspendLayout();
            this.tabXmlScheme.SuspendLayout();
            this.panResultView.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbResult
            // 
            this.cmbResult.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResult.FormattingEnabled = true;
            this.cmbResult.Items.AddRange(new object[] {
            "View",
            "XML (custom format)",
            "JSON (custom format)",
            "JSON (web api format)",
            "Raw result"});
            this.cmbResult.Location = new System.Drawing.Point(23, 36);
            this.cmbResult.Name = "cmbResult";
            this.cmbResult.Size = new System.Drawing.Size(192, 21);
            this.cmbResult.TabIndex = 1;
            this.cmbResult.SelectedIndexChanged += new System.EventHandler(this.cmbResult_SelectedIndexChanged);
            // 
            // chkResAllPages
            // 
            this.chkResAllPages.AutoSize = true;
            this.chkResAllPages.Location = new System.Drawing.Point(23, 74);
            this.chkResAllPages.Name = "chkResAllPages";
            this.chkResAllPages.Size = new System.Drawing.Size(111, 17);
            this.chkResAllPages.TabIndex = 10;
            this.chkResAllPages.Text = "Retrieve all pages";
            this.chkResAllPages.UseVisualStyleBackColor = true;
            // 
            // chkAppResultsNewWindow
            // 
            this.chkAppResultsNewWindow.AutoSize = true;
            this.chkAppResultsNewWindow.Location = new System.Drawing.Point(23, 97);
            this.chkAppResultsNewWindow.Name = "chkAppResultsNewWindow";
            this.chkAppResultsNewWindow.Size = new System.Drawing.Size(192, 17);
            this.chkAppResultsNewWindow.TabIndex = 12;
            this.chkAppResultsNewWindow.Text = "Always open results in new window";
            this.chkAppResultsNewWindow.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Max columns width:";
            // 
            // numMaxColumnWidth
            // 
            this.numMaxColumnWidth.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMaxColumnWidth.Location = new System.Drawing.Point(128, 37);
            this.numMaxColumnWidth.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numMaxColumnWidth.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numMaxColumnWidth.Name = "numMaxColumnWidth";
            this.numMaxColumnWidth.Size = new System.Drawing.Size(75, 20);
            this.numMaxColumnWidth.TabIndex = 15;
            this.numMaxColumnWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numMaxColumnWidth.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.numMaxColumnWidth.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // chkClickableLinks
            // 
            this.chkClickableLinks.AutoSize = true;
            this.chkClickableLinks.Location = new System.Drawing.Point(8, 15);
            this.chkClickableLinks.Name = "chkClickableLinks";
            this.chkClickableLinks.Size = new System.Drawing.Size(157, 17);
            this.chkClickableLinks.TabIndex = 14;
            this.chkClickableLinks.Text = "Double-click links on results";
            this.chkClickableLinks.UseVisualStyleBackColor = true;
            // 
            // chkAlwaysShowAggregateProperties
            // 
            this.chkAlwaysShowAggregateProperties.AutoSize = true;
            this.chkAlwaysShowAggregateProperties.Location = new System.Drawing.Point(20, 43);
            this.chkAlwaysShowAggregateProperties.Name = "chkAlwaysShowAggregateProperties";
            this.chkAlwaysShowAggregateProperties.Size = new System.Drawing.Size(195, 17);
            this.chkAlwaysShowAggregateProperties.TabIndex = 9;
            this.chkAlwaysShowAggregateProperties.Text = "Always show aggregating properties";
            this.chkAlwaysShowAggregateProperties.UseVisualStyleBackColor = true;
            // 
            // chkAppFriendlyResults
            // 
            this.chkAppFriendlyResults.AutoSize = true;
            this.chkAppFriendlyResults.Location = new System.Drawing.Point(20, 43);
            this.chkAppFriendlyResults.Name = "chkAppFriendlyResults";
            this.chkAppFriendlyResults.Size = new System.Drawing.Size(181, 17);
            this.chkAppFriendlyResults.TabIndex = 2;
            this.chkAppFriendlyResults.Text = "Friendly results (CTRL+SHIFT+F)";
            this.chkAppFriendlyResults.UseVisualStyleBackColor = true;
            // 
            // chkUseLookup
            // 
            this.chkUseLookup.AutoSize = true;
            this.chkUseLookup.Location = new System.Drawing.Point(20, 89);
            this.chkUseLookup.Name = "chkUseLookup";
            this.chkUseLookup.Size = new System.Drawing.Size(193, 17);
            this.chkUseLookup.TabIndex = 8;
            this.chkUseLookup.Text = "Use Lookup control instead of Guid";
            this.chkUseLookup.UseVisualStyleBackColor = true;
            // 
            // chkAppSingle
            // 
            this.chkAppSingle.AutoSize = true;
            this.chkAppSingle.Location = new System.Drawing.Point(20, 66);
            this.chkAppSingle.Name = "chkAppSingle";
            this.chkAppSingle.Size = new System.Drawing.Size(203, 17);
            this.chkAppSingle.TabIndex = 3;
            this.chkAppSingle.Text = "Use single quotation in rendered XML";
            this.chkAppSingle.UseVisualStyleBackColor = true;
            // 
            // chkAppFriendly
            // 
            this.chkAppFriendly.AutoSize = true;
            this.chkAppFriendly.Location = new System.Drawing.Point(20, 20);
            this.chkAppFriendly.Name = "chkAppFriendly";
            this.chkAppFriendly.Size = new System.Drawing.Size(179, 17);
            this.chkAppFriendly.TabIndex = 1;
            this.chkAppFriendly.Text = "Friendly names i query (CTRL+F)";
            this.chkAppFriendly.UseVisualStyleBackColor = true;
            // 
            // chkShowButtonTexts
            // 
            this.chkShowButtonTexts.AutoSize = true;
            this.chkShowButtonTexts.Location = new System.Drawing.Point(20, 20);
            this.chkShowButtonTexts.Name = "chkShowButtonTexts";
            this.chkShowButtonTexts.Size = new System.Drawing.Size(111, 17);
            this.chkShowButtonTexts.TabIndex = 1;
            this.chkShowButtonTexts.Text = "Show button texts";
            this.chkShowButtonTexts.UseVisualStyleBackColor = true;
            // 
            // chkShowNodeTypes
            // 
            this.chkShowNodeTypes.AutoSize = true;
            this.chkShowNodeTypes.Location = new System.Drawing.Point(20, 66);
            this.chkShowNodeTypes.Name = "chkShowNodeTypes";
            this.chkShowNodeTypes.Size = new System.Drawing.Size(156, 17);
            this.chkShowNodeTypes.TabIndex = 3;
            this.chkShowNodeTypes.Text = "Show treeview nodes types";
            this.chkShowNodeTypes.UseVisualStyleBackColor = true;
            // 
            // chkShowHelp
            // 
            this.chkShowHelp.AutoSize = true;
            this.chkShowHelp.Location = new System.Drawing.Point(20, 43);
            this.chkShowHelp.Name = "chkShowHelp";
            this.chkShowHelp.Size = new System.Drawing.Size(100, 17);
            this.chkShowHelp.TabIndex = 2;
            this.chkShowHelp.Text = "Show help links";
            this.chkShowHelp.UseVisualStyleBackColor = true;
            // 
            // chkAppAllowUncustViews
            // 
            this.chkAppAllowUncustViews.AutoSize = true;
            this.chkAppAllowUncustViews.Location = new System.Drawing.Point(20, 20);
            this.chkAppAllowUncustViews.Name = "chkAppAllowUncustViews";
            this.chkAppAllowUncustViews.Size = new System.Drawing.Size(198, 17);
            this.chkAppAllowUncustViews.TabIndex = 6;
            this.chkAppAllowUncustViews.Text = "Allow opening uncustomizable views";
            this.chkAppAllowUncustViews.UseVisualStyleBackColor = true;
            // 
            // chkUseSQL4CDS
            // 
            this.chkUseSQL4CDS.AutoSize = true;
            this.chkUseSQL4CDS.Location = new System.Drawing.Point(20, 66);
            this.chkUseSQL4CDS.Name = "chkUseSQL4CDS";
            this.chkUseSQL4CDS.Size = new System.Drawing.Size(197, 17);
            this.chkUseSQL4CDS.TabIndex = 5;
            this.chkUseSQL4CDS.Text = "Use SQL 4 CDS for SQL conversion";
            this.chkUseSQL4CDS.UseVisualStyleBackColor = true;
            // 
            // chkAppNoSavePrompt
            // 
            this.chkAppNoSavePrompt.AutoSize = true;
            this.chkAppNoSavePrompt.Location = new System.Drawing.Point(20, 20);
            this.chkAppNoSavePrompt.Name = "chkAppNoSavePrompt";
            this.chkAppNoSavePrompt.Size = new System.Drawing.Size(159, 17);
            this.chkAppNoSavePrompt.TabIndex = 2;
            this.chkAppNoSavePrompt.Text = "Do not prompt to save to file";
            this.chkAppNoSavePrompt.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.llShowWelcome);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Location = new System.Drawing.Point(12, 224);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(509, 53);
            this.panel1.TabIndex = 100;
            // 
            // llShowWelcome
            // 
            this.llShowWelcome.AutoSize = true;
            this.llShowWelcome.Location = new System.Drawing.Point(7, 24);
            this.llShowWelcome.Name = "llShowWelcome";
            this.llShowWelcome.Size = new System.Drawing.Size(107, 13);
            this.llShowWelcome.TabIndex = 2;
            this.llShowWelcome.TabStop = true;
            this.llShowWelcome.Text = "Show Release Notes";
            this.llShowWelcome.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llShowWelcome_LinkClicked);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(430, 19);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(349, 19);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtFetch
            // 
            this.txtFetch.Location = new System.Drawing.Point(23, 36);
            this.txtFetch.Name = "txtFetch";
            this.txtFetch.Size = new System.Drawing.Size(371, 110);
            this.txtFetch.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnDefaultQuery);
            this.panel2.Controls.Add(this.btnFormatQuery);
            this.panel2.Location = new System.Drawing.Point(400, 36);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(81, 113);
            this.panel2.TabIndex = 5;
            // 
            // btnDefaultQuery
            // 
            this.btnDefaultQuery.Location = new System.Drawing.Point(3, 3);
            this.btnDefaultQuery.Name = "btnDefaultQuery";
            this.btnDefaultQuery.Size = new System.Drawing.Size(75, 23);
            this.btnDefaultQuery.TabIndex = 1;
            this.btnDefaultQuery.Text = "Reset";
            this.btnDefaultQuery.UseVisualStyleBackColor = true;
            this.btnDefaultQuery.Click += new System.EventHandler(this.btnDefaultQuery_Click);
            // 
            // btnFormatQuery
            // 
            this.btnFormatQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFormatQuery.Location = new System.Drawing.Point(3, 87);
            this.btnFormatQuery.Name = "btnFormatQuery";
            this.btnFormatQuery.Size = new System.Drawing.Size(75, 23);
            this.btnFormatQuery.TabIndex = 2;
            this.btnFormatQuery.Text = "Format";
            this.btnFormatQuery.UseVisualStyleBackColor = true;
            this.btnFormatQuery.Click += new System.EventHandler(this.btnFormatQuery_Click);
            // 
            // btnResetXmlColors
            // 
            this.btnResetXmlColors.Image = ((System.Drawing.Image)(resources.GetObject("btnResetXmlColors.Image")));
            this.btnResetXmlColors.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnResetXmlColors.Location = new System.Drawing.Point(241, 79);
            this.btnResetXmlColors.Name = "btnResetXmlColors";
            this.btnResetXmlColors.Size = new System.Drawing.Size(92, 23);
            this.btnResetXmlColors.TabIndex = 8;
            this.btnResetXmlColors.Text = "Reset";
            this.btnResetXmlColors.UseVisualStyleBackColor = true;
            this.btnResetXmlColors.Click += new System.EventHandler(this.btnResetXmlColors_Click);
            // 
            // chkShowValidationInfo
            // 
            this.chkShowValidationInfo.AutoSize = true;
            this.chkShowValidationInfo.Location = new System.Drawing.Point(20, 112);
            this.chkShowValidationInfo.Name = "chkShowValidationInfo";
            this.chkShowValidationInfo.Size = new System.Drawing.Size(131, 17);
            this.chkShowValidationInfo.TabIndex = 8;
            this.chkShowValidationInfo.Text = "Show Information Tips";
            this.chkShowValidationInfo.UseVisualStyleBackColor = true;
            // 
            // chkShowValidation
            // 
            this.chkShowValidation.AutoSize = true;
            this.chkShowValidation.Location = new System.Drawing.Point(20, 89);
            this.chkShowValidation.Name = "chkShowValidation";
            this.chkShowValidation.Size = new System.Drawing.Size(146, 17);
            this.chkShowValidation.TabIndex = 7;
            this.chkShowValidation.Text = "Show Error/Warning Tips";
            this.chkShowValidation.UseVisualStyleBackColor = true;
            this.chkShowValidation.CheckedChanged += new System.EventHandler(this.chkShowValidation_CheckedChanged);
            // 
            // chkAddConditionToFilter
            // 
            this.chkAddConditionToFilter.AutoSize = true;
            this.chkAddConditionToFilter.Location = new System.Drawing.Point(20, 43);
            this.chkAddConditionToFilter.Name = "chkAddConditionToFilter";
            this.chkAddConditionToFilter.Size = new System.Drawing.Size(193, 17);
            this.chkAddConditionToFilter.TabIndex = 4;
            this.chkAddConditionToFilter.Text = "Add Condition to Filter automatically";
            this.chkAddConditionToFilter.UseVisualStyleBackColor = true;
            // 
            // chkShowRepository
            // 
            this.chkShowRepository.AutoSize = true;
            this.chkShowRepository.Location = new System.Drawing.Point(20, 89);
            this.chkShowRepository.Name = "chkShowRepository";
            this.chkShowRepository.Size = new System.Drawing.Size(135, 17);
            this.chkShowRepository.TabIndex = 4;
            this.chkShowRepository.Text = "Show Repository menu";
            this.chkShowRepository.UseVisualStyleBackColor = true;
            // 
            // chkShowAllAttributes
            // 
            this.chkShowAllAttributes.AutoSize = true;
            this.chkShowAllAttributes.Location = new System.Drawing.Point(20, 66);
            this.chkShowAllAttributes.Name = "chkShowAllAttributes";
            this.chkShowAllAttributes.Size = new System.Drawing.Size(169, 17);
            this.chkShowAllAttributes.TabIndex = 5;
            this.chkShowAllAttributes.Text = "Show \"all-attributes\" (bad use)";
            this.chkShowAllAttributes.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(48, 15);
            this.linkLabel1.Location = new System.Drawing.Point(216, 208);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(305, 17);
            this.linkLabel1.TabIndex = 102;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Missing the Entities and Attributes selections? Click here -->";
            this.linkLabel1.UseCompatibleTextRendering = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // chkWaitUntilMetadataLoaded
            // 
            this.chkWaitUntilMetadataLoaded.AutoSize = true;
            this.chkWaitUntilMetadataLoaded.Location = new System.Drawing.Point(260, 43);
            this.chkWaitUntilMetadataLoaded.Name = "chkWaitUntilMetadataLoaded";
            this.chkWaitUntilMetadataLoaded.Size = new System.Drawing.Size(172, 17);
            this.chkWaitUntilMetadataLoaded.TabIndex = 1;
            this.chkWaitUntilMetadataLoaded.Text = "Wait until all entities are loaded";
            this.chkWaitUntilMetadataLoaded.UseVisualStyleBackColor = true;
            // 
            // chkTryMetadataCache
            // 
            this.chkTryMetadataCache.AutoSize = true;
            this.chkTryMetadataCache.Location = new System.Drawing.Point(260, 20);
            this.chkTryMetadataCache.Name = "chkTryMetadataCache";
            this.chkTryMetadataCache.Size = new System.Drawing.Size(202, 17);
            this.chkTryMetadataCache.TabIndex = 0;
            this.chkTryMetadataCache.Text = "Use cached metadata in XrmToolBox";
            this.chkTryMetadataCache.UseVisualStyleBackColor = true;
            this.chkTryMetadataCache.CheckedChanged += new System.EventHandler(this.chkTryMetadataCache_CheckedChanged);
            // 
            // linkLayout
            // 
            this.linkLayout.AutoSize = true;
            this.linkLayout.Location = new System.Drawing.Point(144, 75);
            this.linkLayout.Name = "linkLayout";
            this.linkLayout.Size = new System.Drawing.Size(62, 13);
            this.linkLayout.TabIndex = 18;
            this.linkLayout.TabStop = true;
            this.linkLayout.Text = "Read docs!";
            this.linkLayout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLayout_LinkClicked);
            // 
            // chkWorkWithLayout
            // 
            this.chkWorkWithLayout.AutoSize = true;
            this.chkWorkWithLayout.Location = new System.Drawing.Point(8, 74);
            this.chkWorkWithLayout.Name = "chkWorkWithLayout";
            this.chkWorkWithLayout.Size = new System.Drawing.Size(135, 17);
            this.chkWorkWithLayout.TabIndex = 17;
            this.chkWorkWithLayout.Text = "Work with View Layout";
            this.chkWorkWithLayout.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabAppearance);
            this.tabSettings.Controls.Add(this.tabLayout);
            this.tabSettings.Controls.Add(this.tabBehavior);
            this.tabSettings.Controls.Add(this.tabResults);
            this.tabSettings.Controls.Add(this.tabAdvanced);
            this.tabSettings.Controls.Add(this.tabDefaultQuery);
            this.tabSettings.Controls.Add(this.tabXmlScheme);
            this.tabSettings.Location = new System.Drawing.Point(12, 12);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(513, 179);
            this.tabSettings.TabIndex = 103;
            // 
            // tabAppearance
            // 
            this.tabAppearance.BackColor = System.Drawing.SystemColors.Window;
            this.tabAppearance.Controls.Add(this.chkAppFriendly);
            this.tabAppearance.Controls.Add(this.chkAppFriendlyResults);
            this.tabAppearance.Controls.Add(this.chkAppSingle);
            this.tabAppearance.Controls.Add(this.chkUseLookup);
            this.tabAppearance.Location = new System.Drawing.Point(4, 22);
            this.tabAppearance.Name = "tabAppearance";
            this.tabAppearance.Padding = new System.Windows.Forms.Padding(3);
            this.tabAppearance.Size = new System.Drawing.Size(505, 153);
            this.tabAppearance.TabIndex = 0;
            this.tabAppearance.Text = "Appearance";
            // 
            // tabLayout
            // 
            this.tabLayout.BackColor = System.Drawing.SystemColors.Window;
            this.tabLayout.Controls.Add(this.chkShowButtonTexts);
            this.tabLayout.Controls.Add(this.chkShowRepository);
            this.tabLayout.Controls.Add(this.chkShowHelp);
            this.tabLayout.Controls.Add(this.chkShowNodeTypes);
            this.tabLayout.Location = new System.Drawing.Point(4, 22);
            this.tabLayout.Name = "tabLayout";
            this.tabLayout.Padding = new System.Windows.Forms.Padding(3);
            this.tabLayout.Size = new System.Drawing.Size(505, 153);
            this.tabLayout.TabIndex = 1;
            this.tabLayout.Text = "Layout";
            // 
            // tabBehavior
            // 
            this.tabBehavior.BackColor = System.Drawing.SystemColors.Window;
            this.tabBehavior.Controls.Add(this.chkShowValidationInfo);
            this.tabBehavior.Controls.Add(this.chkAppNoSavePrompt);
            this.tabBehavior.Controls.Add(this.chkShowValidation);
            this.tabBehavior.Controls.Add(this.chkAddConditionToFilter);
            this.tabBehavior.Controls.Add(this.chkUseSQL4CDS);
            this.tabBehavior.Location = new System.Drawing.Point(4, 22);
            this.tabBehavior.Name = "tabBehavior";
            this.tabBehavior.Size = new System.Drawing.Size(505, 153);
            this.tabBehavior.TabIndex = 2;
            this.tabBehavior.Text = "Behavior";
            // 
            // tabResults
            // 
            this.tabResults.BackColor = System.Drawing.SystemColors.Window;
            this.tabResults.Controls.Add(this.label2);
            this.tabResults.Controls.Add(this.panResultView);
            this.tabResults.Controls.Add(this.cmbResult);
            this.tabResults.Controls.Add(this.chkResAllPages);
            this.tabResults.Controls.Add(this.chkAppResultsNewWindow);
            this.tabResults.Location = new System.Drawing.Point(4, 22);
            this.tabResults.Name = "tabResults";
            this.tabResults.Size = new System.Drawing.Size(505, 153);
            this.tabResults.TabIndex = 3;
            this.tabResults.Text = "Results";
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.BackColor = System.Drawing.SystemColors.Window;
            this.tabAdvanced.Controls.Add(this.chkShowAllAttributes);
            this.tabAdvanced.Controls.Add(this.chkAlwaysShowAggregateProperties);
            this.tabAdvanced.Controls.Add(this.chkWaitUntilMetadataLoaded);
            this.tabAdvanced.Controls.Add(this.chkAppAllowUncustViews);
            this.tabAdvanced.Controls.Add(this.chkTryMetadataCache);
            this.tabAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Size = new System.Drawing.Size(505, 153);
            this.tabAdvanced.TabIndex = 4;
            this.tabAdvanced.Text = "Advanced";
            // 
            // tabDefaultQuery
            // 
            this.tabDefaultQuery.BackColor = System.Drawing.SystemColors.Window;
            this.tabDefaultQuery.Controls.Add(this.label3);
            this.tabDefaultQuery.Controls.Add(this.panel2);
            this.tabDefaultQuery.Controls.Add(this.txtFetch);
            this.tabDefaultQuery.Location = new System.Drawing.Point(4, 22);
            this.tabDefaultQuery.Name = "tabDefaultQuery";
            this.tabDefaultQuery.Size = new System.Drawing.Size(505, 153);
            this.tabDefaultQuery.TabIndex = 5;
            this.tabDefaultQuery.Text = "Default Query";
            // 
            // tabXmlScheme
            // 
            this.tabXmlScheme.BackColor = System.Drawing.SystemColors.Window;
            this.tabXmlScheme.Controls.Add(this.btnResetXmlColors);
            this.tabXmlScheme.Controls.Add(this.propXmlColors);
            this.tabXmlScheme.Location = new System.Drawing.Point(4, 22);
            this.tabXmlScheme.Name = "tabXmlScheme";
            this.tabXmlScheme.Size = new System.Drawing.Size(505, 153);
            this.tabXmlScheme.TabIndex = 6;
            this.tabXmlScheme.Text = "XML Scheme";
            // 
            // panResultView
            // 
            this.panResultView.Controls.Add(this.chkClickableLinks);
            this.panResultView.Controls.Add(this.linkLayout);
            this.panResultView.Controls.Add(this.numMaxColumnWidth);
            this.panResultView.Controls.Add(this.label1);
            this.panResultView.Controls.Add(this.chkWorkWithLayout);
            this.panResultView.Location = new System.Drawing.Point(260, 23);
            this.panResultView.Name = "panResultView";
            this.panResultView.Size = new System.Drawing.Size(224, 100);
            this.panResultView.TabIndex = 19;
            // 
            // propXmlColors
            // 
            this.propXmlColors.CanShowVisualStyleGlyphs = false;
            this.propXmlColors.HelpVisible = false;
            this.propXmlColors.LineColor = System.Drawing.SystemColors.Window;
            this.propXmlColors.Location = new System.Drawing.Point(6, 18);
            this.propXmlColors.Name = "propXmlColors";
            this.propXmlColors.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            xmlColors1.AttributeKey = System.Drawing.Color.Red;
            xmlColors1.AttributeKeyColor = "Red";
            xmlColors1.AttributeValue = System.Drawing.Color.Blue;
            xmlColors1.AttributeValueColor = "Blue";
            xmlColors1.Comment = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            xmlColors1.CommentColor = "0";
            xmlColors1.Element = System.Drawing.Color.DarkRed;
            xmlColors1.ElementColor = "DarkRed";
            xmlColors1.Tag = System.Drawing.Color.Blue;
            xmlColors1.TagColor = "Blue";
            xmlColors1.Value = System.Drawing.Color.Black;
            xmlColors1.ValueColor = "Black";
            this.propXmlColors.SelectedObject = xmlColors1;
            this.propXmlColors.Size = new System.Drawing.Size(252, 97);
            this.propXmlColors.TabIndex = 7;
            this.propXmlColors.ToolbarVisible = false;
            this.propXmlColors.ViewBorderColor = System.Drawing.SystemColors.Window;
            this.propXmlColors.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propXmlColors_PropertyValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Result Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Default Query FetchXML";
            // 
            // Settings
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(535, 289);
            this.Controls.Add(this.tabSettings);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.linkLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FetchXML Builder - Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numMaxColumnWidth)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabAppearance.ResumeLayout(false);
            this.tabAppearance.PerformLayout();
            this.tabLayout.ResumeLayout(false);
            this.tabLayout.PerformLayout();
            this.tabBehavior.ResumeLayout(false);
            this.tabBehavior.PerformLayout();
            this.tabResults.ResumeLayout(false);
            this.tabResults.PerformLayout();
            this.tabAdvanced.ResumeLayout(false);
            this.tabAdvanced.PerformLayout();
            this.tabDefaultQuery.ResumeLayout(false);
            this.tabDefaultQuery.PerformLayout();
            this.tabXmlScheme.ResumeLayout(false);
            this.panResultView.ResumeLayout(false);
            this.panResultView.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkAppSingle;
        private System.Windows.Forms.CheckBox chkAppFriendly;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkResAllPages;
        private System.Windows.Forms.CheckBox chkAppNoSavePrompt;
        private System.Windows.Forms.CheckBox chkAppResultsNewWindow;
        private System.Windows.Forms.LinkLabel llShowWelcome;
        private System.Windows.Forms.CheckBox chkAppAllowUncustViews;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnFormatQuery;
        private System.Windows.Forms.Button btnDefaultQuery;
        private System.Windows.Forms.CheckBox chkUseSQL4CDS;
        private System.Windows.Forms.CheckBox chkUseLookup;
        private System.Windows.Forms.PropertyGrid propXmlColors;
        private System.Windows.Forms.Button btnResetXmlColors;
        private System.Windows.Forms.CheckBox chkShowHelp;
        private System.Windows.Forms.CheckBox chkShowNodeTypes;
        private System.Windows.Forms.CheckBox chkAddConditionToFilter;
        private System.Windows.Forms.CheckBox chkShowButtonTexts;
        private ScintillaNET.Scintilla txtFetch;
        private System.Windows.Forms.CheckBox chkShowValidation;
        private System.Windows.Forms.CheckBox chkShowRepository;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox chkShowValidationInfo;
        private System.Windows.Forms.CheckBox chkClickableLinks;
        private System.Windows.Forms.ComboBox cmbResult;
        private System.Windows.Forms.CheckBox chkShowAllAttributes;
        private System.Windows.Forms.CheckBox chkTryMetadataCache;
        private System.Windows.Forms.CheckBox chkWaitUntilMetadataLoaded;
        private System.Windows.Forms.CheckBox chkAppFriendlyResults;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numMaxColumnWidth;
        private System.Windows.Forms.CheckBox chkWorkWithLayout;
        private System.Windows.Forms.LinkLabel linkLayout;
        private System.Windows.Forms.CheckBox chkAlwaysShowAggregateProperties;
        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabAppearance;
        private System.Windows.Forms.TabPage tabLayout;
        private System.Windows.Forms.TabPage tabBehavior;
        private System.Windows.Forms.TabPage tabResults;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.TabPage tabDefaultQuery;
        private System.Windows.Forms.TabPage tabXmlScheme;
        private System.Windows.Forms.Panel panResultView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}