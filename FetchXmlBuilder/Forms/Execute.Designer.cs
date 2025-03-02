namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    partial class Execute
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
            this.linkBypassPlugins = new System.Windows.Forms.LinkLabel();
            this.chkOldBypassCustom = new System.Windows.Forms.CheckBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.gbOldBypassCustom = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.gbBypassLogic = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.chkBypassSync = new System.Windows.Forms.CheckBox();
            this.txtBypassSteps = new System.Windows.Forms.TextBox();
            this.chkBypassAsync = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panButtons = new System.Windows.Forms.Panel();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbPages = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rbAllPages = new System.Windows.Forms.RadioButton();
            this.rbPageByPage = new System.Windows.Forms.RadioButton();
            this.grpResult = new System.Windows.Forms.GroupBox();
            this.rbResultGrid = new System.Windows.Forms.RadioButton();
            this.rbResultJSON = new System.Windows.Forms.RadioButton();
            this.rbResultXML = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.gbOldBypassCustom.SuspendLayout();
            this.gbBypassLogic.SuspendLayout();
            this.panButtons.SuspendLayout();
            this.gbPages.SuspendLayout();
            this.grpResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkBypassPlugins
            // 
            this.linkBypassPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkBypassPlugins.AutoSize = true;
            this.linkBypassPlugins.Location = new System.Drawing.Point(242, 26);
            this.linkBypassPlugins.Name = "linkBypassPlugins";
            this.linkBypassPlugins.Size = new System.Drawing.Size(131, 13);
            this.linkBypassPlugins.TabIndex = 112;
            this.linkBypassPlugins.TabStop = true;
            this.linkBypassPlugins.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/bypass-custom-busi" +
    "ness-logic#bypasscustompluginexecution";
            this.linkBypassPlugins.Text = "MS Learn: Bypass Custom";
            this.toolTip1.SetToolTip(this.linkBypassPlugins, "https://learn.microsoft.com/power-apps/developer/data-platform/bypass-custom-busi" +
        "ness-logic#bypasscustompluginexecution");
            this.linkBypassPlugins.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_Click);
            // 
            // chkOldBypassCustom
            // 
            this.chkOldBypassCustom.AutoSize = true;
            this.chkOldBypassCustom.Location = new System.Drawing.Point(81, 25);
            this.chkOldBypassCustom.Name = "chkOldBypassCustom";
            this.chkOldBypassCustom.Size = new System.Drawing.Size(135, 17);
            this.chkOldBypassCustom.TabIndex = 111;
            this.chkOldBypassCustom.Text = "Custom Business Logic";
            this.chkOldBypassCustom.UseVisualStyleBackColor = true;
            this.chkOldBypassCustom.CheckedChanged += new System.EventHandler(this.validate_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExecute.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecute.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_execute;
            this.btnExecute.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExecute.Location = new System.Drawing.Point(245, 22);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnExecute.Size = new System.Drawing.Size(128, 38);
            this.btnExecute.TabIndex = 115;
            this.btnExecute.Text = "Execute";
            this.btnExecute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.btnExecute, "This will execute the action.\r\nThere is NOT undo.");
            this.btnExecute.UseVisualStyleBackColor = true;
            // 
            // gbOldBypassCustom
            // 
            this.gbOldBypassCustom.Controls.Add(this.label4);
            this.gbOldBypassCustom.Controls.Add(this.chkOldBypassCustom);
            this.gbOldBypassCustom.Controls.Add(this.linkBypassPlugins);
            this.gbOldBypassCustom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbOldBypassCustom.Location = new System.Drawing.Point(6, 246);
            this.gbOldBypassCustom.Name = "gbOldBypassCustom";
            this.gbOldBypassCustom.Size = new System.Drawing.Size(379, 57);
            this.gbOldBypassCustom.TabIndex = 4;
            this.gbOldBypassCustom.TabStop = false;
            this.gbOldBypassCustom.Text = "Old: Bypass Custom";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 115;
            this.label4.Text = "Bypass";
            // 
            // linkLabel3
            // 
            this.linkLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(167, 10);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(207, 13);
            this.linkLabel3.TabIndex = 1;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/bypass-custom-busi" +
    "ness-logic";
            this.linkLabel3.Text = "MS Learn: Bypass custom Dataverse logic";
            this.toolTip1.SetToolTip(this.linkLabel3, "https://learn.microsoft.com/power-apps/developer/data-platform/bypass-custom-busi" +
        "ness-logic");
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_Click);
            // 
            // gbBypassLogic
            // 
            this.gbBypassLogic.Controls.Add(this.label1);
            this.gbBypassLogic.Controls.Add(this.label2);
            this.gbBypassLogic.Controls.Add(this.linkLabel3);
            this.gbBypassLogic.Controls.Add(this.linkLabel2);
            this.gbBypassLogic.Controls.Add(this.chkBypassSync);
            this.gbBypassLogic.Controls.Add(this.txtBypassSteps);
            this.gbBypassLogic.Controls.Add(this.chkBypassAsync);
            this.gbBypassLogic.Controls.Add(this.linkLabel1);
            this.gbBypassLogic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbBypassLogic.Location = new System.Drawing.Point(6, 117);
            this.gbBypassLogic.Name = "gbBypassLogic";
            this.gbBypassLogic.Size = new System.Drawing.Size(379, 129);
            this.gbBypassLogic.TabIndex = 3;
            this.gbBypassLogic.TabStop = false;
            this.gbBypassLogic.Text = "Bypass Logic";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 117;
            this.label1.Text = "Modes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 118;
            this.label2.Text = "Plugin Step Ids";
            // 
            // linkLabel2
            // 
            this.linkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(276, 55);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(98, 13);
            this.linkLabel2.TabIndex = 5;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/bypass-custom-busi" +
    "ness-logic#bypassbusinesslogicexecutionstepids";
            this.linkLabel2.Text = "MS Learn: Step Ids";
            this.toolTip1.SetToolTip(this.linkLabel2, "https://learn.microsoft.com/power-apps/developer/data-platform/bypass-custom-busi" +
        "ness-logic#bypassbusinesslogicexecutionstepids");
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_Click);
            // 
            // chkBypassSync
            // 
            this.chkBypassSync.AutoSize = true;
            this.chkBypassSync.Location = new System.Drawing.Point(81, 32);
            this.chkBypassSync.Name = "chkBypassSync";
            this.chkBypassSync.Size = new System.Drawing.Size(50, 17);
            this.chkBypassSync.TabIndex = 2;
            this.chkBypassSync.Text = "Sync";
            this.chkBypassSync.UseVisualStyleBackColor = true;
            this.chkBypassSync.CheckedChanged += new System.EventHandler(this.validate_Click);
            // 
            // txtBypassSteps
            // 
            this.txtBypassSteps.AcceptsReturn = true;
            this.txtBypassSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBypassSteps.Location = new System.Drawing.Point(15, 71);
            this.txtBypassSteps.Multiline = true;
            this.txtBypassSteps.Name = "txtBypassSteps";
            this.txtBypassSteps.Size = new System.Drawing.Size(358, 52);
            this.txtBypassSteps.TabIndex = 6;
            this.toolTip1.SetToolTip(this.txtBypassSteps, "Add Guids separated by comma.\r\nGuids are found on Plugin Steps in Plugin Registra" +
        "ting Tool.");
            this.txtBypassSteps.TextChanged += new System.EventHandler(this.validate_Click);
            // 
            // chkBypassAsync
            // 
            this.chkBypassAsync.AutoSize = true;
            this.chkBypassAsync.Location = new System.Drawing.Point(137, 32);
            this.chkBypassAsync.Name = "chkBypassAsync";
            this.chkBypassAsync.Size = new System.Drawing.Size(55, 17);
            this.chkBypassAsync.TabIndex = 3;
            this.chkBypassAsync.Text = "Async";
            this.chkBypassAsync.UseVisualStyleBackColor = true;
            this.chkBypassAsync.CheckedChanged += new System.EventHandler(this.validate_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(268, 33);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(106, 13);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/bypass-custom-busi" +
    "ness-logic#bypassbusinesslogicexecution";
            this.linkLabel1.Text = "MS Learn: Execution";
            this.toolTip1.SetToolTip(this.linkLabel1, "https://learn.microsoft.com/power-apps/developer/data-platform/bypass-custom-busi" +
        "ness-logic#bypassbusinesslogicexecution");
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_Click);
            // 
            // panButtons
            // 
            this.panButtons.Controls.Add(this.lblInfo);
            this.panButtons.Controls.Add(this.btnCancel);
            this.panButtons.Controls.Add(this.btnExecute);
            this.panButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panButtons.Location = new System.Drawing.Point(6, 303);
            this.panButtons.Name = "panButtons";
            this.panButtons.Size = new System.Drawing.Size(379, 78);
            this.panButtons.TabIndex = 5;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(12, 60);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(95, 13);
            this.lblInfo.TabIndex = 117;
            this.lblInfo.Text = "Execute will start...";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(15, 30);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 116;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbPages
            // 
            this.gbPages.Controls.Add(this.label3);
            this.gbPages.Controls.Add(this.rbAllPages);
            this.gbPages.Controls.Add(this.rbPageByPage);
            this.gbPages.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbPages.Location = new System.Drawing.Point(6, 60);
            this.gbPages.Name = "gbPages";
            this.gbPages.Size = new System.Drawing.Size(379, 57);
            this.gbPages.TabIndex = 2;
            this.gbPages.TabStop = false;
            this.gbPages.Text = "Pages";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Retrieving";
            // 
            // rbAllPages
            // 
            this.rbAllPages.AutoSize = true;
            this.rbAllPages.Location = new System.Drawing.Point(195, 25);
            this.rbAllPages.Name = "rbAllPages";
            this.rbAllPages.Size = new System.Drawing.Size(69, 17);
            this.rbAllPages.TabIndex = 1;
            this.rbAllPages.TabStop = true;
            this.rbAllPages.Text = "All Pages";
            this.rbAllPages.UseVisualStyleBackColor = true;
            // 
            // rbPageByPage
            // 
            this.rbPageByPage.AutoSize = true;
            this.rbPageByPage.Location = new System.Drawing.Point(81, 25);
            this.rbPageByPage.Name = "rbPageByPage";
            this.rbPageByPage.Size = new System.Drawing.Size(92, 17);
            this.rbPageByPage.TabIndex = 0;
            this.rbPageByPage.TabStop = true;
            this.rbPageByPage.Text = "Page by Page";
            this.rbPageByPage.UseVisualStyleBackColor = true;
            // 
            // grpResult
            // 
            this.grpResult.Controls.Add(this.label5);
            this.grpResult.Controls.Add(this.rbResultXML);
            this.grpResult.Controls.Add(this.rbResultJSON);
            this.grpResult.Controls.Add(this.rbResultGrid);
            this.grpResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpResult.Location = new System.Drawing.Point(6, 6);
            this.grpResult.Name = "grpResult";
            this.grpResult.Size = new System.Drawing.Size(379, 54);
            this.grpResult.TabIndex = 5;
            this.grpResult.TabStop = false;
            this.grpResult.Text = "Retrieve && Format";
            // 
            // rbResultGrid
            // 
            this.rbResultGrid.AutoSize = true;
            this.rbResultGrid.Location = new System.Drawing.Point(81, 25);
            this.rbResultGrid.Name = "rbResultGrid";
            this.rbResultGrid.Size = new System.Drawing.Size(70, 17);
            this.rbResultGrid.TabIndex = 0;
            this.rbResultGrid.TabStop = true;
            this.rbResultGrid.Text = "Grid View";
            this.rbResultGrid.UseVisualStyleBackColor = true;
            // 
            // rbResultJSON
            // 
            this.rbResultJSON.AutoSize = true;
            this.rbResultJSON.Location = new System.Drawing.Point(157, 25);
            this.rbResultJSON.Name = "rbResultJSON";
            this.rbResultJSON.Size = new System.Drawing.Size(96, 17);
            this.rbResultJSON.TabIndex = 1;
            this.rbResultJSON.TabStop = true;
            this.rbResultJSON.Text = "JSON WebAPI";
            this.rbResultJSON.UseVisualStyleBackColor = true;
            // 
            // rbResultXML
            // 
            this.rbResultXML.AutoSize = true;
            this.rbResultXML.Location = new System.Drawing.Point(259, 25);
            this.rbResultXML.Name = "rbResultXML";
            this.rbResultXML.Size = new System.Drawing.Size(110, 17);
            this.rbResultXML.TabIndex = 2;
            this.rbResultXML.TabStop = true;
            this.rbResultXML.Text = "XML (deprecated)";
            this.rbResultXML.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Result";
            // 
            // Execute
            // 
            this.AcceptButton = this.btnExecute;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(391, 387);
            this.Controls.Add(this.gbBypassLogic);
            this.Controls.Add(this.gbOldBypassCustom);
            this.Controls.Add(this.gbPages);
            this.Controls.Add(this.panButtons);
            this.Controls.Add(this.grpResult);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(380, 340);
            this.Name = "Execute";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FetchXML Builder - Execute";
            this.gbOldBypassCustom.ResumeLayout(false);
            this.gbOldBypassCustom.PerformLayout();
            this.gbBypassLogic.ResumeLayout(false);
            this.gbBypassLogic.PerformLayout();
            this.panButtons.ResumeLayout(false);
            this.panButtons.PerformLayout();
            this.gbPages.ResumeLayout(false);
            this.gbPages.PerformLayout();
            this.grpResult.ResumeLayout(false);
            this.grpResult.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.LinkLabel linkBypassPlugins;
        private System.Windows.Forms.CheckBox chkOldBypassCustom;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox gbOldBypassCustom;
        private System.Windows.Forms.CheckBox chkBypassSync;
        private System.Windows.Forms.CheckBox chkBypassAsync;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel panButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBypassSteps;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.GroupBox gbBypassLogic;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbPages;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbAllPages;
        private System.Windows.Forms.RadioButton rbPageByPage;
        private System.Windows.Forms.GroupBox grpResult;
        private System.Windows.Forms.RadioButton rbResultJSON;
        private System.Windows.Forms.RadioButton rbResultGrid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbResultXML;
    }
}