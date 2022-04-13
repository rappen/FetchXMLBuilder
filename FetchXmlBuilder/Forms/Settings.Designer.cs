using Cinteros.Xrm.FetchXmlBuilder.AppCode;

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
            Cinteros.Xrm.FetchXmlBuilder.AppCode.XmlColors xmlColors1 = new Cinteros.Xrm.FetchXmlBuilder.AppCode.XmlColors();
            this.gbResult = new System.Windows.Forms.GroupBox();
            this.chkResAllPages = new System.Windows.Forms.CheckBox();
            this.chkAppResultsNewWindow = new System.Windows.Forms.CheckBox();
            this.gbAppearance = new System.Windows.Forms.GroupBox();
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
            this.gbDefaultQuery = new System.Windows.Forms.GroupBox();
            this.txtFetch = new ScintillaNET.Scintilla();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnDefaultQuery = new System.Windows.Forms.Button();
            this.btnFormatQuery = new System.Windows.Forms.Button();
            this.gbXml = new System.Windows.Forms.GroupBox();
            this.btnResetXmlColors = new System.Windows.Forms.Button();
            this.gbBehavior = new System.Windows.Forms.GroupBox();
            this.chkShowValidationInfo = new System.Windows.Forms.CheckBox();
            this.chkShowValidation = new System.Windows.Forms.CheckBox();
            this.chkAddConditionToFilter = new System.Windows.Forms.CheckBox();
            this.chkShowRepository = new System.Windows.Forms.CheckBox();
            this.gbLayout = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.chkShowAllAttributes = new System.Windows.Forms.CheckBox();
            this.cmbResult = new System.Windows.Forms.ComboBox();
            this.chkClickableLinks = new System.Windows.Forms.CheckBox();
            this.propXmlColors = new System.Windows.Forms.PropertyGrid();
            this.gbResult.SuspendLayout();
            this.gbAppearance.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbDefaultQuery.SuspendLayout();
            this.panel2.SuspendLayout();
            this.gbXml.SuspendLayout();
            this.gbBehavior.SuspendLayout();
            this.gbLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbResult
            // 
            this.gbResult.Controls.Add(this.chkClickableLinks);
            this.gbResult.Controls.Add(this.cmbResult);
            this.gbResult.Controls.Add(this.chkResAllPages);
            this.gbResult.Controls.Add(this.chkAppResultsNewWindow);
            this.gbResult.Location = new System.Drawing.Point(484, 12);
            this.gbResult.Name = "gbResult";
            this.gbResult.Size = new System.Drawing.Size(222, 130);
            this.gbResult.TabIndex = 40;
            this.gbResult.TabStop = false;
            this.gbResult.Text = "Result";
            // 
            // chkResAllPages
            // 
            this.chkResAllPages.AutoSize = true;
            this.chkResAllPages.Location = new System.Drawing.Point(16, 60);
            this.chkResAllPages.Name = "chkResAllPages";
            this.chkResAllPages.Size = new System.Drawing.Size(111, 17);
            this.chkResAllPages.TabIndex = 10;
            this.chkResAllPages.Text = "Retrieve all pages";
            this.chkResAllPages.UseVisualStyleBackColor = true;
            // 
            // chkAppResultsNewWindow
            // 
            this.chkAppResultsNewWindow.AutoSize = true;
            this.chkAppResultsNewWindow.Location = new System.Drawing.Point(16, 80);
            this.chkAppResultsNewWindow.Name = "chkAppResultsNewWindow";
            this.chkAppResultsNewWindow.Size = new System.Drawing.Size(192, 17);
            this.chkAppResultsNewWindow.TabIndex = 12;
            this.chkAppResultsNewWindow.Text = "Always open results in new window";
            this.chkAppResultsNewWindow.UseVisualStyleBackColor = true;
            // 
            // gbAppearance
            // 
            this.gbAppearance.Controls.Add(this.chkUseLookup);
            this.gbAppearance.Controls.Add(this.chkAppSingle);
            this.gbAppearance.Controls.Add(this.chkAppFriendly);
            this.gbAppearance.Location = new System.Drawing.Point(12, 12);
            this.gbAppearance.Name = "gbAppearance";
            this.gbAppearance.Size = new System.Drawing.Size(222, 87);
            this.gbAppearance.TabIndex = 10;
            this.gbAppearance.TabStop = false;
            this.gbAppearance.Text = "Appearance";
            // 
            // chkUseLookup
            // 
            this.chkUseLookup.AutoSize = true;
            this.chkUseLookup.Location = new System.Drawing.Point(16, 60);
            this.chkUseLookup.Name = "chkUseLookup";
            this.chkUseLookup.Size = new System.Drawing.Size(193, 17);
            this.chkUseLookup.TabIndex = 8;
            this.chkUseLookup.Text = "Use Lookup control instead of Guid";
            this.chkUseLookup.UseVisualStyleBackColor = true;
            // 
            // chkAppSingle
            // 
            this.chkAppSingle.AutoSize = true;
            this.chkAppSingle.Location = new System.Drawing.Point(16, 40);
            this.chkAppSingle.Name = "chkAppSingle";
            this.chkAppSingle.Size = new System.Drawing.Size(203, 17);
            this.chkAppSingle.TabIndex = 3;
            this.chkAppSingle.Text = "Use single quotation in rendered XML";
            this.chkAppSingle.UseVisualStyleBackColor = true;
            // 
            // chkAppFriendly
            // 
            this.chkAppFriendly.AutoSize = true;
            this.chkAppFriendly.Location = new System.Drawing.Point(16, 20);
            this.chkAppFriendly.Name = "chkAppFriendly";
            this.chkAppFriendly.Size = new System.Drawing.Size(179, 17);
            this.chkAppFriendly.TabIndex = 1;
            this.chkAppFriendly.Text = "Friendly names i query (CTRL+F)";
            this.chkAppFriendly.UseVisualStyleBackColor = true;
            // 
            // chkShowButtonTexts
            // 
            this.chkShowButtonTexts.AutoSize = true;
            this.chkShowButtonTexts.Location = new System.Drawing.Point(16, 20);
            this.chkShowButtonTexts.Name = "chkShowButtonTexts";
            this.chkShowButtonTexts.Size = new System.Drawing.Size(111, 17);
            this.chkShowButtonTexts.TabIndex = 1;
            this.chkShowButtonTexts.Text = "Show button texts";
            this.chkShowButtonTexts.UseVisualStyleBackColor = true;
            // 
            // chkShowNodeTypes
            // 
            this.chkShowNodeTypes.AutoSize = true;
            this.chkShowNodeTypes.Location = new System.Drawing.Point(16, 60);
            this.chkShowNodeTypes.Name = "chkShowNodeTypes";
            this.chkShowNodeTypes.Size = new System.Drawing.Size(156, 17);
            this.chkShowNodeTypes.TabIndex = 3;
            this.chkShowNodeTypes.Text = "Show treeview nodes types";
            this.chkShowNodeTypes.UseVisualStyleBackColor = true;
            // 
            // chkShowHelp
            // 
            this.chkShowHelp.AutoSize = true;
            this.chkShowHelp.Location = new System.Drawing.Point(16, 40);
            this.chkShowHelp.Name = "chkShowHelp";
            this.chkShowHelp.Size = new System.Drawing.Size(100, 17);
            this.chkShowHelp.TabIndex = 2;
            this.chkShowHelp.Text = "Show help links";
            this.chkShowHelp.UseVisualStyleBackColor = true;
            // 
            // chkAppAllowUncustViews
            // 
            this.chkAppAllowUncustViews.AutoSize = true;
            this.chkAppAllowUncustViews.Location = new System.Drawing.Point(16, 80);
            this.chkAppAllowUncustViews.Name = "chkAppAllowUncustViews";
            this.chkAppAllowUncustViews.Size = new System.Drawing.Size(198, 17);
            this.chkAppAllowUncustViews.TabIndex = 6;
            this.chkAppAllowUncustViews.Text = "Allow opening uncustomizable views";
            this.chkAppAllowUncustViews.UseVisualStyleBackColor = true;
            // 
            // chkUseSQL4CDS
            // 
            this.chkUseSQL4CDS.AutoSize = true;
            this.chkUseSQL4CDS.Location = new System.Drawing.Point(16, 60);
            this.chkUseSQL4CDS.Name = "chkUseSQL4CDS";
            this.chkUseSQL4CDS.Size = new System.Drawing.Size(197, 17);
            this.chkUseSQL4CDS.TabIndex = 5;
            this.chkUseSQL4CDS.Text = "Use SQL 4 CDS for SQL conversion";
            this.chkUseSQL4CDS.UseVisualStyleBackColor = true;
            // 
            // chkAppNoSavePrompt
            // 
            this.chkAppNoSavePrompt.AutoSize = true;
            this.chkAppNoSavePrompt.Location = new System.Drawing.Point(16, 20);
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
            this.panel1.Location = new System.Drawing.Point(12, 369);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(691, 53);
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
            this.btnCancel.Location = new System.Drawing.Point(612, 19);
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
            this.btnOK.Location = new System.Drawing.Point(531, 19);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbDefaultQuery
            // 
            this.gbDefaultQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbDefaultQuery.Controls.Add(this.txtFetch);
            this.gbDefaultQuery.Controls.Add(this.panel2);
            this.gbDefaultQuery.Location = new System.Drawing.Point(12, 234);
            this.gbDefaultQuery.Name = "gbDefaultQuery";
            this.gbDefaultQuery.Size = new System.Drawing.Size(458, 132);
            this.gbDefaultQuery.TabIndex = 80;
            this.gbDefaultQuery.TabStop = false;
            this.gbDefaultQuery.Text = "Default New Query";
            // 
            // txtFetch
            // 
            this.txtFetch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFetch.Location = new System.Drawing.Point(3, 16);
            this.txtFetch.Name = "txtFetch";
            this.txtFetch.Size = new System.Drawing.Size(371, 113);
            this.txtFetch.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnDefaultQuery);
            this.panel2.Controls.Add(this.btnFormatQuery);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(374, 16);
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
            this.btnDefaultQuery.Text = "Default";
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
            // gbXml
            // 
            this.gbXml.Controls.Add(this.btnResetXmlColors);
            this.gbXml.Controls.Add(this.propXmlColors);
            this.gbXml.Location = new System.Drawing.Point(484, 148);
            this.gbXml.Name = "gbXml";
            this.gbXml.Size = new System.Drawing.Size(222, 132);
            this.gbXml.TabIndex = 50;
            this.gbXml.TabStop = false;
            this.gbXml.Text = "XML color scheme";
            // 
            // btnResetXmlColors
            // 
            this.btnResetXmlColors.Image = ((System.Drawing.Image)(resources.GetObject("btnResetXmlColors.Image")));
            this.btnResetXmlColors.Location = new System.Drawing.Point(181, 100);
            this.btnResetXmlColors.Name = "btnResetXmlColors";
            this.btnResetXmlColors.Size = new System.Drawing.Size(30, 23);
            this.btnResetXmlColors.TabIndex = 8;
            this.btnResetXmlColors.UseVisualStyleBackColor = true;
            this.btnResetXmlColors.Click += new System.EventHandler(this.btnResetXmlColors_Click);
            // 
            // gbBehavior
            // 
            this.gbBehavior.Controls.Add(this.chkShowValidationInfo);
            this.gbBehavior.Controls.Add(this.chkShowValidation);
            this.gbBehavior.Controls.Add(this.chkAddConditionToFilter);
            this.gbBehavior.Controls.Add(this.chkUseSQL4CDS);
            this.gbBehavior.Controls.Add(this.chkAppNoSavePrompt);
            this.gbBehavior.Controls.Add(this.chkAppAllowUncustViews);
            this.gbBehavior.Location = new System.Drawing.Point(248, 12);
            this.gbBehavior.Name = "gbBehavior";
            this.gbBehavior.Size = new System.Drawing.Size(222, 150);
            this.gbBehavior.TabIndex = 30;
            this.gbBehavior.TabStop = false;
            this.gbBehavior.Text = "Behavior";
            // 
            // chkShowValidationInfo
            // 
            this.chkShowValidationInfo.AutoSize = true;
            this.chkShowValidationInfo.Location = new System.Drawing.Point(16, 120);
            this.chkShowValidationInfo.Name = "chkShowValidationInfo";
            this.chkShowValidationInfo.Size = new System.Drawing.Size(131, 17);
            this.chkShowValidationInfo.TabIndex = 8;
            this.chkShowValidationInfo.Text = "Show Information Tips";
            this.chkShowValidationInfo.UseVisualStyleBackColor = true;
            // 
            // chkShowValidation
            // 
            this.chkShowValidation.AutoSize = true;
            this.chkShowValidation.Location = new System.Drawing.Point(16, 100);
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
            this.chkAddConditionToFilter.Location = new System.Drawing.Point(16, 40);
            this.chkAddConditionToFilter.Name = "chkAddConditionToFilter";
            this.chkAddConditionToFilter.Size = new System.Drawing.Size(193, 17);
            this.chkAddConditionToFilter.TabIndex = 4;
            this.chkAddConditionToFilter.Text = "Add Condition to Filter automatically";
            this.chkAddConditionToFilter.UseVisualStyleBackColor = true;
            // 
            // chkShowRepository
            // 
            this.chkShowRepository.AutoSize = true;
            this.chkShowRepository.Location = new System.Drawing.Point(16, 80);
            this.chkShowRepository.Name = "chkShowRepository";
            this.chkShowRepository.Size = new System.Drawing.Size(135, 17);
            this.chkShowRepository.TabIndex = 4;
            this.chkShowRepository.Text = "Show Repository menu";
            this.chkShowRepository.UseVisualStyleBackColor = true;
            // 
            // gbLayout
            // 
            this.gbLayout.Controls.Add(this.chkShowAllAttributes);
            this.gbLayout.Controls.Add(this.chkShowRepository);
            this.gbLayout.Controls.Add(this.chkShowButtonTexts);
            this.gbLayout.Controls.Add(this.chkShowNodeTypes);
            this.gbLayout.Controls.Add(this.chkShowHelp);
            this.gbLayout.Location = new System.Drawing.Point(12, 105);
            this.gbLayout.Name = "gbLayout";
            this.gbLayout.Size = new System.Drawing.Size(222, 125);
            this.gbLayout.TabIndex = 20;
            this.gbLayout.TabStop = false;
            this.gbLayout.Text = "Layout";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(50, 15);
            this.linkLabel1.Location = new System.Drawing.Point(500, 300);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(181, 42);
            this.linkLabel1.TabIndex = 102;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Missing the Entities and Attributes\r\nselections?\r\nClick here! -->";
            this.linkLabel1.UseCompatibleTextRendering = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // chkShowAllAttributes
            // 
            this.chkShowAllAttributes.AutoSize = true;
            this.chkShowAllAttributes.Location = new System.Drawing.Point(16, 100);
            this.chkShowAllAttributes.Name = "chkShowAllAttributes";
            this.chkShowAllAttributes.Size = new System.Drawing.Size(169, 17);
            this.chkShowAllAttributes.TabIndex = 5;
            this.chkShowAllAttributes.Text = "Show \"all-attributes\" (bad use)";
            this.chkShowAllAttributes.UseVisualStyleBackColor = true;
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
            this.cmbResult.Location = new System.Drawing.Point(16, 20);
            this.cmbResult.Name = "cmbResult";
            this.cmbResult.Size = new System.Drawing.Size(192, 21);
            this.cmbResult.TabIndex = 13;
            // 
            // chkClickableLinks
            // 
            this.chkClickableLinks.AutoSize = true;
            this.chkClickableLinks.Location = new System.Drawing.Point(16, 100);
            this.chkClickableLinks.Name = "chkClickableLinks";
            this.chkClickableLinks.Size = new System.Drawing.Size(157, 17);
            this.chkClickableLinks.TabIndex = 14;
            this.chkClickableLinks.Text = "Double-click links on results";
            this.chkClickableLinks.UseVisualStyleBackColor = true;
            // 
            // propXmlColors
            // 
            this.propXmlColors.CanShowVisualStyleGlyphs = false;
            this.propXmlColors.HelpVisible = false;
            this.propXmlColors.LineColor = System.Drawing.SystemColors.Window;
            this.propXmlColors.Location = new System.Drawing.Point(1, 19);
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
            this.propXmlColors.Size = new System.Drawing.Size(218, 104);
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
            this.ClientSize = new System.Drawing.Size(717, 432);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.gbLayout);
            this.Controls.Add(this.gbBehavior);
            this.Controls.Add(this.gbXml);
            this.Controls.Add(this.gbDefaultQuery);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gbAppearance);
            this.Controls.Add(this.gbResult);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FetchXML Builder - Settings";
            this.gbResult.ResumeLayout(false);
            this.gbResult.PerformLayout();
            this.gbAppearance.ResumeLayout(false);
            this.gbAppearance.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbDefaultQuery.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.gbXml.ResumeLayout(false);
            this.gbBehavior.ResumeLayout(false);
            this.gbBehavior.PerformLayout();
            this.gbLayout.ResumeLayout(false);
            this.gbLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbResult;
        private System.Windows.Forms.GroupBox gbAppearance;
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
        private System.Windows.Forms.GroupBox gbDefaultQuery;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnFormatQuery;
        private System.Windows.Forms.Button btnDefaultQuery;
        private System.Windows.Forms.CheckBox chkUseSQL4CDS;
        private System.Windows.Forms.CheckBox chkUseLookup;
        private System.Windows.Forms.PropertyGrid propXmlColors;
        private System.Windows.Forms.GroupBox gbXml;
        private System.Windows.Forms.GroupBox gbBehavior;
        private System.Windows.Forms.Button btnResetXmlColors;
        private System.Windows.Forms.CheckBox chkShowHelp;
        private System.Windows.Forms.CheckBox chkShowNodeTypes;
        private System.Windows.Forms.CheckBox chkAddConditionToFilter;
        private System.Windows.Forms.CheckBox chkShowButtonTexts;
        private ScintillaNET.Scintilla txtFetch;
        private System.Windows.Forms.CheckBox chkShowValidation;
        private System.Windows.Forms.CheckBox chkShowRepository;
        private System.Windows.Forms.GroupBox gbLayout;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox chkShowValidationInfo;
        private System.Windows.Forms.CheckBox chkClickableLinks;
        private System.Windows.Forms.ComboBox cmbResult;
        private System.Windows.Forms.CheckBox chkShowAllAttributes;
    }
}