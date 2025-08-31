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
            this.components = new System.ComponentModel.Container();
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
            this.chkShowValidationInfo = new System.Windows.Forms.CheckBox();
            this.chkShowValidation = new System.Windows.Forms.CheckBox();
            this.chkAddConditionToFilter = new System.Windows.Forms.CheckBox();
            this.chkShowAllAttributes = new System.Windows.Forms.CheckBox();
            this.chkWaitUntilMetadataLoaded = new System.Windows.Forms.CheckBox();
            this.chkTryMetadataCache = new System.Windows.Forms.CheckBox();
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabAppearance = new System.Windows.Forms.TabPage();
            this.chkShowTreeviewAttributeTypes = new System.Windows.Forms.CheckBox();
            this.chkShowAttributeTypes = new System.Windows.Forms.CheckBox();
            this.chkShowNodeTypes = new System.Windows.Forms.CheckBox();
            this.chkShowButtonTexts = new System.Windows.Forms.CheckBox();
            this.chkShowHelp = new System.Windows.Forms.CheckBox();
            this.chkAppBothNamesResults = new System.Windows.Forms.CheckBox();
            this.tabBehavior = new System.Windows.Forms.TabPage();
            this.tabAiChat = new System.Windows.Forms.TabPage();
            this.txtAiCallMe = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.picAiUrl = new System.Windows.Forms.PictureBox();
            this.picAiSupplier = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbAiSupplier = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbAiModel = new System.Windows.Forms.ComboBox();
            this.txtAiApiKey = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabResults = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.linkDeprecatedExecFetchReq = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.panResultView = new System.Windows.Forms.Panel();
            this.tabLayout = new System.Windows.Forms.TabPage();
            this.panLayout = new System.Windows.Forms.Panel();
            this.chkLayoutUseFixedWidths = new System.Windows.Forms.CheckBox();
            this.linkLayout = new System.Windows.Forms.LinkLabel();
            this.chkWorkWithLayout = new System.Windows.Forms.CheckBox();
            this.tabDefaultQuery = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.tabXmlScheme = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.btnResetXmlColors = new System.Windows.Forms.Button();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.btnForceReloadMetadata = new System.Windows.Forms.Button();
            this.btnResetAll = new System.Windows.Forms.Button();
            this.chkShowOData2 = new System.Windows.Forms.CheckBox();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.propXmlColors = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxColumnWidth)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabAppearance.SuspendLayout();
            this.tabBehavior.SuspendLayout();
            this.tabAiChat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAiUrl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAiSupplier)).BeginInit();
            this.tabResults.SuspendLayout();
            this.panResultView.SuspendLayout();
            this.tabLayout.SuspendLayout();
            this.panLayout.SuspendLayout();
            this.tabDefaultQuery.SuspendLayout();
            this.tabXmlScheme.SuspendLayout();
            this.tabAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbResult
            // 
            this.cmbResult.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResult.FormattingEnabled = true;
            this.cmbResult.Items.AddRange(new object[] {
            "Grid View",
            "XML (homemade format)",
            "JSON (homemade format)",
            "JSON (web api format)",
            "Raw result (deprecated)"});
            this.cmbResult.Location = new System.Drawing.Point(23, 36);
            this.cmbResult.Name = "cmbResult";
            this.cmbResult.Size = new System.Drawing.Size(192, 21);
            this.cmbResult.TabIndex = 1;
            this.cmbResult.SelectedIndexChanged += new System.EventHandler(this.cmbResult_SelectedIndexChanged);
            // 
            // chkResAllPages
            // 
            this.chkResAllPages.AutoSize = true;
            this.chkResAllPages.Location = new System.Drawing.Point(23, 86);
            this.chkResAllPages.Name = "chkResAllPages";
            this.chkResAllPages.Size = new System.Drawing.Size(111, 17);
            this.chkResAllPages.TabIndex = 3;
            this.chkResAllPages.Text = "Retrieve all pages";
            this.tt.SetToolTip(this.chkResAllPages, "Check this to always retrieve and keep retrieving\r\nuntil all the data is now here" +
        ". But be careful...");
            this.chkResAllPages.UseVisualStyleBackColor = true;
            // 
            // chkAppResultsNewWindow
            // 
            this.chkAppResultsNewWindow.AutoSize = true;
            this.chkAppResultsNewWindow.Location = new System.Drawing.Point(23, 63);
            this.chkAppResultsNewWindow.Name = "chkAppResultsNewWindow";
            this.chkAppResultsNewWindow.Size = new System.Drawing.Size(192, 17);
            this.chkAppResultsNewWindow.TabIndex = 2;
            this.chkAppResultsNewWindow.Text = "Always open results in new window";
            this.tt.SetToolTip(this.chkAppResultsNewWindow, "To compare results it might be good to always\r\nget the result in a new window.");
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
            this.tt.SetToolTip(this.numMaxColumnWidth, "Helps to not getting gazillion width columns for crazy\r\nwide data.");
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
            this.tt.SetToolTip(this.chkClickableLinks, "I may be easy and nice to double-click to open that \r\nrecord or lookups in the re" +
        "sults.");
            this.chkClickableLinks.UseVisualStyleBackColor = true;
            // 
            // chkAlwaysShowAggregateProperties
            // 
            this.chkAlwaysShowAggregateProperties.AutoSize = true;
            this.chkAlwaysShowAggregateProperties.Location = new System.Drawing.Point(20, 43);
            this.chkAlwaysShowAggregateProperties.Name = "chkAlwaysShowAggregateProperties";
            this.chkAlwaysShowAggregateProperties.Size = new System.Drawing.Size(195, 17);
            this.chkAlwaysShowAggregateProperties.TabIndex = 2;
            this.chkAlwaysShowAggregateProperties.Text = "Always show aggregating properties";
            this.tt.SetToolTip(this.chkAlwaysShowAggregateProperties, "Aggregation properties of the attributes are only\r\nshowing if the fetch says it s" +
        "hould aggregate.\r\nIf you check this one, they will always be shown.");
            this.chkAlwaysShowAggregateProperties.UseVisualStyleBackColor = true;
            // 
            // chkAppFriendlyResults
            // 
            this.chkAppFriendlyResults.AutoSize = true;
            this.chkAppFriendlyResults.Location = new System.Drawing.Point(20, 43);
            this.chkAppFriendlyResults.Name = "chkAppFriendlyResults";
            this.chkAppFriendlyResults.Size = new System.Drawing.Size(262, 17);
            this.chkAppFriendlyResults.TabIndex = 2;
            this.chkAppFriendlyResults.Text = "Friendly names/values in results (CTRL+SHIFT+F)";
            this.tt.SetToolTip(this.chkAppFriendlyResults, "Flip between at showing values and headers\r\nwith a friendly name or a technical v" +
        "alue.\r\nNote it can easily be flipped outside these settings with <CTRL+SHIFT>+F\r" +
        "\n");
            this.chkAppFriendlyResults.UseVisualStyleBackColor = true;
            // 
            // chkUseLookup
            // 
            this.chkUseLookup.AutoSize = true;
            this.chkUseLookup.Location = new System.Drawing.Point(20, 112);
            this.chkUseLookup.Name = "chkUseLookup";
            this.chkUseLookup.Size = new System.Drawing.Size(193, 17);
            this.chkUseLookup.TabIndex = 5;
            this.chkUseLookup.Text = "Use Lookup control instead of Guid";
            this.tt.SetToolTip(this.chkUseLookup, "Check this to be able to select a record instead of only\r\nwriting the Guid. Most " +
        "used in conditional values.");
            this.chkUseLookup.UseVisualStyleBackColor = true;
            // 
            // chkAppSingle
            // 
            this.chkAppSingle.AutoSize = true;
            this.chkAppSingle.Location = new System.Drawing.Point(20, 89);
            this.chkAppSingle.Name = "chkAppSingle";
            this.chkAppSingle.Size = new System.Drawing.Size(203, 17);
            this.chkAppSingle.TabIndex = 4;
            this.chkAppSingle.Text = "Use single quotation in rendered XML";
            this.tt.SetToolTip(this.chkAppSingle, "Flip this between singe quote and double quotes in the FetchXML.");
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
            this.tt.SetToolTip(this.chkAppFriendly, "Flip between at showing entities and attributes etc names\r\nwith a friendly name o" +
        "r a technical logical_name.\r\nNote it can easily be flipped outside these setting" +
        "s with <CTRL>+F");
            this.chkAppFriendly.UseVisualStyleBackColor = true;
            // 
            // chkAppAllowUncustViews
            // 
            this.chkAppAllowUncustViews.AutoSize = true;
            this.chkAppAllowUncustViews.Location = new System.Drawing.Point(20, 20);
            this.chkAppAllowUncustViews.Name = "chkAppAllowUncustViews";
            this.chkAppAllowUncustViews.Size = new System.Drawing.Size(198, 17);
            this.chkAppAllowUncustViews.TabIndex = 1;
            this.chkAppAllowUncustViews.Text = "Allow opening uncustomizable views";
            this.tt.SetToolTip(this.chkAppAllowUncustViews, "Some views don\'t allow to change. But they can be\r\nopened. If you check this one." +
        "");
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
            this.tt.SetToolTip(this.chkUseSQL4CDS, "Converting from FetchXML to SQL can be done by\r\nthe SQL 4 CDS tool. If unchecked," +
        " I will try within FetchXML Builder.");
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
            this.tt.SetToolTip(this.chkAppNoSavePrompt, "Check this to please just close FetchXML Builder\r\nwithout asking me if it should " +
        "be save on disk.");
            this.chkAppNoSavePrompt.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.llShowWelcome);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Location = new System.Drawing.Point(12, 197);
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
            // chkShowValidationInfo
            // 
            this.chkShowValidationInfo.AutoSize = true;
            this.chkShowValidationInfo.Location = new System.Drawing.Point(20, 112);
            this.chkShowValidationInfo.Name = "chkShowValidationInfo";
            this.chkShowValidationInfo.Size = new System.Drawing.Size(131, 17);
            this.chkShowValidationInfo.TabIndex = 8;
            this.chkShowValidationInfo.Text = "Show Information Tips";
            this.tt.SetToolTip(this.chkShowValidationInfo, "It might be too much with the Information-level of\r\ntips. So it might be good to " +
        "hide it by unchecking.");
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
            this.tt.SetToolTip(this.chkShowValidation, "It is probably good to get warnings and error notes\r\nhere and there. But it can b" +
        "e disabled.");
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
            this.tt.SetToolTip(this.chkAddConditionToFilter, "Check this if automagically create a Condition also\r\nwhen creating an Filter. You" +
        " probably will always\r\nneed an Condition there...");
            this.chkAddConditionToFilter.UseVisualStyleBackColor = true;
            // 
            // chkShowAllAttributes
            // 
            this.chkShowAllAttributes.AutoSize = true;
            this.chkShowAllAttributes.Location = new System.Drawing.Point(20, 66);
            this.chkShowAllAttributes.Name = "chkShowAllAttributes";
            this.chkShowAllAttributes.Size = new System.Drawing.Size(169, 17);
            this.chkShowAllAttributes.TabIndex = 3;
            this.chkShowAllAttributes.Text = "Show \"all-attributes\" (bad use)";
            this.tt.SetToolTip(this.chkShowAllAttributes, "all-attributes is a big no-no.\r\nBut you can, if you check this one.");
            this.chkShowAllAttributes.UseVisualStyleBackColor = true;
            // 
            // chkWaitUntilMetadataLoaded
            // 
            this.chkWaitUntilMetadataLoaded.AutoSize = true;
            this.chkWaitUntilMetadataLoaded.Location = new System.Drawing.Point(260, 43);
            this.chkWaitUntilMetadataLoaded.Name = "chkWaitUntilMetadataLoaded";
            this.chkWaitUntilMetadataLoaded.Size = new System.Drawing.Size(172, 17);
            this.chkWaitUntilMetadataLoaded.TabIndex = 6;
            this.chkWaitUntilMetadataLoaded.Text = "Wait until all entities are loaded";
            this.tt.SetToolTip(this.chkWaitUntilMetadataLoaded, "Unchecked it will load metadata in the background.\r\nChecked and it will load it e" +
        "verything, them the tool\r\ncan be used.");
            this.chkWaitUntilMetadataLoaded.UseVisualStyleBackColor = true;
            // 
            // chkTryMetadataCache
            // 
            this.chkTryMetadataCache.AutoSize = true;
            this.chkTryMetadataCache.Location = new System.Drawing.Point(260, 20);
            this.chkTryMetadataCache.Name = "chkTryMetadataCache";
            this.chkTryMetadataCache.Size = new System.Drawing.Size(202, 17);
            this.chkTryMetadataCache.TabIndex = 5;
            this.chkTryMetadataCache.Text = "Use cached metadata in XrmToolBox";
            this.tt.SetToolTip(this.chkTryMetadataCache, "XrmToolBox has a general cache for Dataverse metadata.\r\nMay be smart, may not. Tr" +
        "y it!");
            this.chkTryMetadataCache.UseVisualStyleBackColor = true;
            this.chkTryMetadataCache.CheckedChanged += new System.EventHandler(this.chkTryMetadataCache_CheckedChanged);
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabAppearance);
            this.tabSettings.Controls.Add(this.tabBehavior);
            this.tabSettings.Controls.Add(this.tabAiChat);
            this.tabSettings.Controls.Add(this.tabResults);
            this.tabSettings.Controls.Add(this.tabLayout);
            this.tabSettings.Controls.Add(this.tabDefaultQuery);
            this.tabSettings.Controls.Add(this.tabXmlScheme);
            this.tabSettings.Controls.Add(this.tabAdvanced);
            this.tabSettings.Location = new System.Drawing.Point(12, 12);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(513, 179);
            this.tabSettings.TabIndex = 103;
            // 
            // tabAppearance
            // 
            this.tabAppearance.BackColor = System.Drawing.SystemColors.Window;
            this.tabAppearance.Controls.Add(this.chkShowTreeviewAttributeTypes);
            this.tabAppearance.Controls.Add(this.chkShowAttributeTypes);
            this.tabAppearance.Controls.Add(this.chkShowNodeTypes);
            this.tabAppearance.Controls.Add(this.chkShowButtonTexts);
            this.tabAppearance.Controls.Add(this.chkShowHelp);
            this.tabAppearance.Controls.Add(this.chkAppBothNamesResults);
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
            // chkShowTreeviewAttributeTypes
            // 
            this.chkShowTreeviewAttributeTypes.AutoSize = true;
            this.chkShowTreeviewAttributeTypes.Location = new System.Drawing.Point(288, 89);
            this.chkShowTreeviewAttributeTypes.Name = "chkShowTreeviewAttributeTypes";
            this.chkShowTreeviewAttributeTypes.Size = new System.Drawing.Size(171, 17);
            this.chkShowTreeviewAttributeTypes.TabIndex = 11;
            this.chkShowTreeviewAttributeTypes.Text = "Show attribute type in treeview";
            this.tt.SetToolTip(this.chkShowTreeviewAttributeTypes, "Buttons in the main menu can only be showing\r\ntheir icons, or also include their " +
        "texts.");
            this.chkShowTreeviewAttributeTypes.UseVisualStyleBackColor = true;
            // 
            // chkShowAttributeTypes
            // 
            this.chkShowAttributeTypes.AutoSize = true;
            this.chkShowAttributeTypes.Location = new System.Drawing.Point(288, 112);
            this.chkShowAttributeTypes.Name = "chkShowAttributeTypes";
            this.chkShowAttributeTypes.Size = new System.Drawing.Size(208, 17);
            this.chkShowAttributeTypes.TabIndex = 12;
            this.chkShowAttributeTypes.Text = "Show attribute type on node properties";
            this.tt.SetToolTip(this.chkShowAttributeTypes, "Buttons in the main menu can only be showing\r\ntheir icons, or also include their " +
        "texts.");
            this.chkShowAttributeTypes.UseVisualStyleBackColor = true;
            // 
            // chkShowNodeTypes
            // 
            this.chkShowNodeTypes.AutoSize = true;
            this.chkShowNodeTypes.Location = new System.Drawing.Point(288, 66);
            this.chkShowNodeTypes.Name = "chkShowNodeTypes";
            this.chkShowNodeTypes.Size = new System.Drawing.Size(161, 17);
            this.chkShowNodeTypes.TabIndex = 10;
            this.chkShowNodeTypes.Text = "Show treeview node classes";
            this.tt.SetToolTip(this.chkShowNodeTypes, "Check this to show each type of nodes in\r\nthe treeview in the Query Builder.");
            this.chkShowNodeTypes.UseVisualStyleBackColor = true;
            // 
            // chkShowButtonTexts
            // 
            this.chkShowButtonTexts.AutoSize = true;
            this.chkShowButtonTexts.Location = new System.Drawing.Point(288, 20);
            this.chkShowButtonTexts.Name = "chkShowButtonTexts";
            this.chkShowButtonTexts.Size = new System.Drawing.Size(111, 17);
            this.chkShowButtonTexts.TabIndex = 6;
            this.chkShowButtonTexts.Text = "Show button texts";
            this.tt.SetToolTip(this.chkShowButtonTexts, "Buttons in the main menu can only be showing\r\ntheir icons, or also include their " +
        "texts.");
            this.chkShowButtonTexts.UseVisualStyleBackColor = true;
            // 
            // chkShowHelp
            // 
            this.chkShowHelp.AutoSize = true;
            this.chkShowHelp.Location = new System.Drawing.Point(288, 43);
            this.chkShowHelp.Name = "chkShowHelp";
            this.chkShowHelp.Size = new System.Drawing.Size(100, 17);
            this.chkShowHelp.TabIndex = 7;
            this.chkShowHelp.Text = "Show help links";
            this.tt.SetToolTip(this.chkShowHelp, "There are a lot of help link in this tool.\r\nThey can be hidden with this setting." +
        "");
            this.chkShowHelp.UseVisualStyleBackColor = true;
            // 
            // chkAppBothNamesResults
            // 
            this.chkAppBothNamesResults.AutoSize = true;
            this.chkAppBothNamesResults.Location = new System.Drawing.Point(20, 66);
            this.chkAppBothNamesResults.Name = "chkAppBothNamesResults";
            this.chkAppBothNamesResults.Size = new System.Drawing.Size(249, 17);
            this.chkAppBothNamesResults.TabIndex = 3;
            this.chkAppBothNamesResults.Text = "Both names/values in results (CTRL+SHIFT+B)";
            this.chkAppBothNamesResults.UseVisualStyleBackColor = true;
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
            // tabAiChat
            // 
            this.tabAiChat.BackColor = System.Drawing.SystemColors.Window;
            this.tabAiChat.Controls.Add(this.txtAiCallMe);
            this.tabAiChat.Controls.Add(this.label9);
            this.tabAiChat.Controls.Add(this.picAiUrl);
            this.tabAiChat.Controls.Add(this.picAiSupplier);
            this.tabAiChat.Controls.Add(this.label6);
            this.tabAiChat.Controls.Add(this.cmbAiSupplier);
            this.tabAiChat.Controls.Add(this.label8);
            this.tabAiChat.Controls.Add(this.cmbAiModel);
            this.tabAiChat.Controls.Add(this.txtAiApiKey);
            this.tabAiChat.Controls.Add(this.label7);
            this.tabAiChat.Location = new System.Drawing.Point(4, 22);
            this.tabAiChat.Name = "tabAiChat";
            this.tabAiChat.Size = new System.Drawing.Size(505, 153);
            this.tabAiChat.TabIndex = 7;
            this.tabAiChat.Text = "AI Chat";
            // 
            // txtAiCallMe
            // 
            this.txtAiCallMe.Location = new System.Drawing.Point(171, 127);
            this.txtAiCallMe.Name = "txtAiCallMe";
            this.txtAiCallMe.Size = new System.Drawing.Size(322, 20);
            this.txtAiCallMe.TabIndex = 12;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(145, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Please always mention me as";
            // 
            // picAiUrl
            // 
            this.picAiUrl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAiUrl.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_link_16;
            this.picAiUrl.Location = new System.Drawing.Point(477, 46);
            this.picAiUrl.Name = "picAiUrl";
            this.picAiUrl.Size = new System.Drawing.Size(16, 16);
            this.picAiUrl.TabIndex = 10;
            this.picAiUrl.TabStop = false;
            this.tt.SetToolTip(this.picAiUrl, "Browse to the api endpoint");
            this.picAiUrl.Click += new System.EventHandler(this.picAiSupplier_Click);
            // 
            // picAiSupplier
            // 
            this.picAiSupplier.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAiSupplier.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_info_16;
            this.picAiSupplier.Location = new System.Drawing.Point(477, 19);
            this.picAiSupplier.Name = "picAiSupplier";
            this.picAiSupplier.Size = new System.Drawing.Size(16, 16);
            this.picAiSupplier.TabIndex = 9;
            this.picAiSupplier.TabStop = false;
            this.tt.SetToolTip(this.picAiSupplier, "Read more");
            this.picAiSupplier.Click += new System.EventHandler(this.picAiSupplier_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "AI Provider";
            // 
            // cmbAiSupplier
            // 
            this.cmbAiSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAiSupplier.FormattingEnabled = true;
            this.cmbAiSupplier.Items.AddRange(new object[] {
            "Anthropic",
            "OpenAI",
            "Azure OpenAI"});
            this.cmbAiSupplier.Location = new System.Drawing.Point(84, 17);
            this.cmbAiSupplier.Name = "cmbAiSupplier";
            this.cmbAiSupplier.Size = new System.Drawing.Size(387, 21);
            this.cmbAiSupplier.TabIndex = 1;
            this.cmbAiSupplier.SelectedIndexChanged += new System.EventHandler(this.cmbAiSupplier_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "API Key";
            // 
            // cmbAiModel
            // 
            this.cmbAiModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAiModel.FormattingEnabled = true;
            this.cmbAiModel.Items.AddRange(new object[] {
            "Anthropic",
            "ChatGTP",
            "xyz"});
            this.cmbAiModel.Location = new System.Drawing.Point(84, 44);
            this.cmbAiModel.Name = "cmbAiModel";
            this.cmbAiModel.Size = new System.Drawing.Size(387, 21);
            this.cmbAiModel.TabIndex = 4;
            this.cmbAiModel.SelectedIndexChanged += new System.EventHandler(this.cmbAiModel_SelectedIndexChanged);
            // 
            // txtAiApiKey
            // 
            this.txtAiApiKey.Location = new System.Drawing.Point(84, 71);
            this.txtAiApiKey.Multiline = true;
            this.txtAiApiKey.Name = "txtAiApiKey";
            this.txtAiApiKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAiApiKey.Size = new System.Drawing.Size(409, 50);
            this.txtAiApiKey.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Model";
            // 
            // tabResults
            // 
            this.tabResults.BackColor = System.Drawing.SystemColors.Window;
            this.tabResults.Controls.Add(this.label5);
            this.tabResults.Controls.Add(this.linkDeprecatedExecFetchReq);
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(209, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "More options by execute with <SHIFT>+F5";
            // 
            // linkDeprecatedExecFetchReq
            // 
            this.linkDeprecatedExecFetchReq.AutoSize = true;
            this.linkDeprecatedExecFetchReq.Location = new System.Drawing.Point(20, 128);
            this.linkDeprecatedExecFetchReq.Name = "linkDeprecatedExecFetchReq";
            this.linkDeprecatedExecFetchReq.Size = new System.Drawing.Size(263, 13);
            this.linkDeprecatedExecFetchReq.TabIndex = 21;
            this.linkDeprecatedExecFetchReq.TabStop = true;
            this.linkDeprecatedExecFetchReq.Text = "ExecuteFetchRequest is deprecated - read more here!";
            this.tt.SetToolTip(this.linkDeprecatedExecFetchReq, "https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.executefetchreq" +
        "uest");
            this.linkDeprecatedExecFetchReq.Visible = false;
            this.linkDeprecatedExecFetchReq.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGeneral_Click);
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
            // panResultView
            // 
            this.panResultView.Controls.Add(this.chkClickableLinks);
            this.panResultView.Controls.Add(this.numMaxColumnWidth);
            this.panResultView.Controls.Add(this.label1);
            this.panResultView.Location = new System.Drawing.Point(260, 23);
            this.panResultView.Name = "panResultView";
            this.panResultView.Size = new System.Drawing.Size(224, 100);
            this.panResultView.TabIndex = 19;
            // 
            // tabLayout
            // 
            this.tabLayout.BackColor = System.Drawing.SystemColors.Window;
            this.tabLayout.Controls.Add(this.panLayout);
            this.tabLayout.Controls.Add(this.linkLayout);
            this.tabLayout.Controls.Add(this.chkWorkWithLayout);
            this.tabLayout.Location = new System.Drawing.Point(4, 22);
            this.tabLayout.Name = "tabLayout";
            this.tabLayout.Padding = new System.Windows.Forms.Padding(3);
            this.tabLayout.Size = new System.Drawing.Size(505, 153);
            this.tabLayout.TabIndex = 1;
            this.tabLayout.Text = "Layout";
            // 
            // panLayout
            // 
            this.panLayout.Controls.Add(this.chkLayoutUseFixedWidths);
            this.panLayout.Location = new System.Drawing.Point(11, 37);
            this.panLayout.Name = "panLayout";
            this.panLayout.Size = new System.Drawing.Size(435, 85);
            this.panLayout.TabIndex = 21;
            // 
            // chkLayoutUseFixedWidths
            // 
            this.chkLayoutUseFixedWidths.AutoSize = true;
            this.chkLayoutUseFixedWidths.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkLayoutUseFixedWidths.Location = new System.Drawing.Point(9, 6);
            this.chkLayoutUseFixedWidths.Name = "chkLayoutUseFixedWidths";
            this.chkLayoutUseFixedWidths.Size = new System.Drawing.Size(200, 30);
            this.chkLayoutUseFixedWidths.TabIndex = 0;
            this.chkLayoutUseFixedWidths.Text = "Use fixed widths by old school pixels:\r\n25, 50, 75, 100, 125, 150, 200, 300";
            this.chkLayoutUseFixedWidths.UseVisualStyleBackColor = true;
            // 
            // linkLayout
            // 
            this.linkLayout.AutoSize = true;
            this.linkLayout.Location = new System.Drawing.Point(156, 21);
            this.linkLayout.Name = "linkLayout";
            this.linkLayout.Size = new System.Drawing.Size(62, 13);
            this.linkLayout.TabIndex = 20;
            this.linkLayout.TabStop = true;
            this.linkLayout.Tag = "https://fetchxmlbuilder.com/features/layouts";
            this.linkLayout.Text = "Read docs!";
            this.tt.SetToolTip(this.linkLayout, "https://fetchxmlbuilder.com/features/layouts");
            // 
            // chkWorkWithLayout
            // 
            this.chkWorkWithLayout.AutoSize = true;
            this.chkWorkWithLayout.Location = new System.Drawing.Point(20, 20);
            this.chkWorkWithLayout.Name = "chkWorkWithLayout";
            this.chkWorkWithLayout.Size = new System.Drawing.Size(135, 17);
            this.chkWorkWithLayout.TabIndex = 19;
            this.chkWorkWithLayout.Text = "Work with View Layout";
            this.tt.SetToolTip(this.chkWorkWithLayout, "FetchXML AND LayoutXML!\r\nProbably read about it... Link is here.");
            this.chkWorkWithLayout.UseVisualStyleBackColor = true;
            this.chkWorkWithLayout.CheckedChanged += new System.EventHandler(this.chkWorkWithLayout_CheckedChanged);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Default Query FetchXML";
            // 
            // tabXmlScheme
            // 
            this.tabXmlScheme.BackColor = System.Drawing.SystemColors.Window;
            this.tabXmlScheme.Controls.Add(this.label4);
            this.tabXmlScheme.Controls.Add(this.btnResetXmlColors);
            this.tabXmlScheme.Controls.Add(this.propXmlColors);
            this.tabXmlScheme.Location = new System.Drawing.Point(4, 22);
            this.tabXmlScheme.Name = "tabXmlScheme";
            this.tabXmlScheme.Size = new System.Drawing.Size(505, 153);
            this.tabXmlScheme.TabIndex = 6;
            this.tabXmlScheme.Text = "XML Scheme";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "XML colors for FetchXML";
            // 
            // btnResetXmlColors
            // 
            this.btnResetXmlColors.Image = ((System.Drawing.Image)(resources.GetObject("btnResetXmlColors.Image")));
            this.btnResetXmlColors.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnResetXmlColors.Location = new System.Drawing.Point(239, 101);
            this.btnResetXmlColors.Name = "btnResetXmlColors";
            this.btnResetXmlColors.Size = new System.Drawing.Size(92, 23);
            this.btnResetXmlColors.TabIndex = 8;
            this.btnResetXmlColors.Text = "Reset";
            this.btnResetXmlColors.UseVisualStyleBackColor = true;
            this.btnResetXmlColors.Click += new System.EventHandler(this.btnResetXmlColors_Click);
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.BackColor = System.Drawing.SystemColors.Window;
            this.tabAdvanced.Controls.Add(this.btnForceReloadMetadata);
            this.tabAdvanced.Controls.Add(this.btnResetAll);
            this.tabAdvanced.Controls.Add(this.chkShowOData2);
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
            // btnForceReloadMetadata
            // 
            this.btnForceReloadMetadata.Location = new System.Drawing.Point(280, 66);
            this.btnForceReloadMetadata.Name = "btnForceReloadMetadata";
            this.btnForceReloadMetadata.Size = new System.Drawing.Size(177, 23);
            this.btnForceReloadMetadata.TabIndex = 12;
            this.btnForceReloadMetadata.Text = "[BETA] Reload all metadata";
            this.btnForceReloadMetadata.UseVisualStyleBackColor = true;
            this.btnForceReloadMetadata.Click += new System.EventHandler(this.btnForceReloadMetadata_Click);
            // 
            // btnResetAll
            // 
            this.btnResetAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetAll.Location = new System.Drawing.Point(280, 116);
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.Size = new System.Drawing.Size(177, 23);
            this.btnResetAll.TabIndex = 20;
            this.btnResetAll.Text = "Reset all settings to default";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // chkShowOData2
            // 
            this.chkShowOData2.AutoSize = true;
            this.chkShowOData2.Location = new System.Drawing.Point(20, 89);
            this.chkShowOData2.Name = "chkShowOData2";
            this.chkShowOData2.Size = new System.Drawing.Size(172, 17);
            this.chkShowOData2.TabIndex = 4;
            this.chkShowOData2.Text = "Show deprecated \"OData 2.0\"";
            this.tt.SetToolTip(this.chkShowOData2, "OData v2.0 is deprecated and removed from the\r\nplatform at 2023-03-30.\r\nYou can s" +
        "how it anyway, if you which, but checking this one.");
            this.chkShowOData2.UseVisualStyleBackColor = true;
            // 
            // propXmlColors
            // 
            this.propXmlColors.CanShowVisualStyleGlyphs = false;
            this.propXmlColors.HelpVisible = false;
            this.propXmlColors.LineColor = System.Drawing.SystemColors.Window;
            this.propXmlColors.Location = new System.Drawing.Point(4, 40);
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
            // Settings
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(535, 262);
            this.Controls.Add(this.tabSettings);
            this.Controls.Add(this.panel1);
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
            this.tabBehavior.ResumeLayout(false);
            this.tabBehavior.PerformLayout();
            this.tabAiChat.ResumeLayout(false);
            this.tabAiChat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAiUrl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAiSupplier)).EndInit();
            this.tabResults.ResumeLayout(false);
            this.tabResults.PerformLayout();
            this.panResultView.ResumeLayout(false);
            this.panResultView.PerformLayout();
            this.tabLayout.ResumeLayout(false);
            this.tabLayout.PerformLayout();
            this.panLayout.ResumeLayout(false);
            this.panLayout.PerformLayout();
            this.tabDefaultQuery.ResumeLayout(false);
            this.tabDefaultQuery.PerformLayout();
            this.tabXmlScheme.ResumeLayout(false);
            this.tabXmlScheme.PerformLayout();
            this.tabAdvanced.ResumeLayout(false);
            this.tabAdvanced.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.CheckBox chkAddConditionToFilter;
        private ScintillaNET.Scintilla txtFetch;
        private System.Windows.Forms.CheckBox chkShowValidation;
        private System.Windows.Forms.CheckBox chkShowValidationInfo;
        private System.Windows.Forms.CheckBox chkClickableLinks;
        private System.Windows.Forms.ComboBox cmbResult;
        private System.Windows.Forms.CheckBox chkShowAllAttributes;
        private System.Windows.Forms.CheckBox chkTryMetadataCache;
        private System.Windows.Forms.CheckBox chkWaitUntilMetadataLoaded;
        private System.Windows.Forms.CheckBox chkAppFriendlyResults;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numMaxColumnWidth;
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
        private System.Windows.Forms.CheckBox chkShowOData2;
        private System.Windows.Forms.LinkLabel linkDeprecatedExecFetchReq;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnForceReloadMetadata;
        private System.Windows.Forms.CheckBox chkAppBothNamesResults;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbAiSupplier;
        private System.Windows.Forms.TextBox txtAiApiKey;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbAiModel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox picAiSupplier;
        private System.Windows.Forms.TabPage tabAiChat;
        private System.Windows.Forms.PictureBox picAiUrl;
        private System.Windows.Forms.TextBox txtAiCallMe;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkShowButtonTexts;
        private System.Windows.Forms.CheckBox chkShowHelp;
        private System.Windows.Forms.CheckBox chkShowTreeviewAttributeTypes;
        private System.Windows.Forms.CheckBox chkShowAttributeTypes;
        private System.Windows.Forms.CheckBox chkShowNodeTypes;
        private System.Windows.Forms.LinkLabel linkLayout;
        private System.Windows.Forms.CheckBox chkWorkWithLayout;
        private System.Windows.Forms.Panel panLayout;
        private System.Windows.Forms.CheckBox chkLayoutUseFixedWidths;
    }
}