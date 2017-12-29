namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    partial class XmlContentDisplayDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlContentDisplayDialog));
            CSRichTextBoxSyntaxHighlighting.XMLViewerSettings xmlViewerSettings4 = new CSRichTextBoxSyntaxHighlighting.XMLViewerSettings();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panCancel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtXML = new CSRichTextBoxSyntaxHighlighting.XMLViewer();
            this.panActions = new System.Windows.Forms.Panel();
            this.gbActions = new System.Windows.Forms.GroupBox();
            this.chkLiveUpdate = new System.Windows.Forms.CheckBox();
            this.panFormatting = new System.Windows.Forms.Panel();
            this.gbFormatting = new System.Windows.Forms.GroupBox();
            this.rbFormatEsc = new System.Windows.Forms.RadioButton();
            this.rbFormatHTML = new System.Windows.Forms.RadioButton();
            this.rbFormatXML = new System.Windows.Forms.RadioButton();
            this.btnFormat = new System.Windows.Forms.Button();
            this.panOk = new System.Windows.Forms.Panel();
            this.panSave = new System.Windows.Forms.Panel();
            this.panExecute = new System.Windows.Forms.Panel();
            this.panLiveUpdate = new System.Windows.Forms.Panel();
            this.panCancel.SuspendLayout();
            this.panActions.SuspendLayout();
            this.gbActions.SuspendLayout();
            this.panFormatting.SuspendLayout();
            this.gbFormatting.SuspendLayout();
            this.panOk.SuspendLayout();
            this.panSave.SuspendLayout();
            this.panExecute.SuspendLayout();
            this.panLiveUpdate.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(6, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExecute.Image = ((System.Drawing.Image)(resources.GetObject("btnExecute.Image")));
            this.btnExecute.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExecute.Location = new System.Drawing.Point(6, 0);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(99, 23);
            this.btnExecute.TabIndex = 5;
            this.btnExecute.Text = "Execute";
            this.btnExecute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(6, 0);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panCancel
            // 
            this.panCancel.Controls.Add(this.btnCancel);
            this.panCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.panCancel.Location = new System.Drawing.Point(486, 16);
            this.panCancel.Name = "panCancel";
            this.panCancel.Size = new System.Drawing.Size(91, 28);
            this.panCancel.TabIndex = 4;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(6, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtXML
            // 
            this.txtXML.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtXML.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtXML.Location = new System.Drawing.Point(0, 0);
            this.txtXML.Name = "txtXML";
            xmlViewerSettings4.AttributeKey = System.Drawing.Color.Red;
            xmlViewerSettings4.AttributeValue = System.Drawing.Color.Blue;
            xmlViewerSettings4.Comment = System.Drawing.Color.Green;
            xmlViewerSettings4.Element = System.Drawing.Color.DarkRed;
            xmlViewerSettings4.QuoteCharacter = '\"';
            xmlViewerSettings4.Tag = System.Drawing.Color.Blue;
            xmlViewerSettings4.Value = System.Drawing.Color.Black;
            this.txtXML.Settings = xmlViewerSettings4;
            this.txtXML.Size = new System.Drawing.Size(578, 452);
            this.txtXML.TabIndex = 0;
            this.txtXML.Text = "";
            this.txtXML.TextChanged += new System.EventHandler(this.txtXML_TextChanged);
            // 
            // panActions
            // 
            this.panActions.Controls.Add(this.gbActions);
            this.panActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panActions.Location = new System.Drawing.Point(0, 496);
            this.panActions.Name = "panActions";
            this.panActions.Size = new System.Drawing.Size(578, 50);
            this.panActions.TabIndex = 10;
            // 
            // gbActions
            // 
            this.gbActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbActions.Controls.Add(this.panExecute);
            this.gbActions.Controls.Add(this.panSave);
            this.gbActions.Controls.Add(this.panOk);
            this.gbActions.Controls.Add(this.panCancel);
            this.gbActions.Controls.Add(this.panLiveUpdate);
            this.gbActions.Location = new System.Drawing.Point(-1, 4);
            this.gbActions.Name = "gbActions";
            this.gbActions.Size = new System.Drawing.Size(580, 47);
            this.gbActions.TabIndex = 4;
            this.gbActions.TabStop = false;
            this.gbActions.Text = "Actions";
            // 
            // chkLiveUpdate
            // 
            this.chkLiveUpdate.AutoSize = true;
            this.chkLiveUpdate.Location = new System.Drawing.Point(13, 4);
            this.chkLiveUpdate.Name = "chkLiveUpdate";
            this.chkLiveUpdate.Size = new System.Drawing.Size(115, 17);
            this.chkLiveUpdate.TabIndex = 0;
            this.chkLiveUpdate.Text = "Live Update Query";
            this.chkLiveUpdate.UseVisualStyleBackColor = true;
            this.chkLiveUpdate.CheckedChanged += new System.EventHandler(this.chkLiveUpdate_CheckedChanged);
            // 
            // panFormatting
            // 
            this.panFormatting.Controls.Add(this.gbFormatting);
            this.panFormatting.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panFormatting.Location = new System.Drawing.Point(0, 452);
            this.panFormatting.Name = "panFormatting";
            this.panFormatting.Size = new System.Drawing.Size(578, 44);
            this.panFormatting.TabIndex = 7;
            // 
            // gbFormatting
            // 
            this.gbFormatting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFormatting.Controls.Add(this.rbFormatEsc);
            this.gbFormatting.Controls.Add(this.rbFormatHTML);
            this.gbFormatting.Controls.Add(this.rbFormatXML);
            this.gbFormatting.Controls.Add(this.btnFormat);
            this.gbFormatting.Location = new System.Drawing.Point(-1, 3);
            this.gbFormatting.Name = "gbFormatting";
            this.gbFormatting.Size = new System.Drawing.Size(580, 43);
            this.gbFormatting.TabIndex = 4;
            this.gbFormatting.TabStop = false;
            this.gbFormatting.Text = "Formatting";
            // 
            // rbFormatEsc
            // 
            this.rbFormatEsc.AutoSize = true;
            this.rbFormatEsc.Location = new System.Drawing.Point(130, 19);
            this.rbFormatEsc.Name = "rbFormatEsc";
            this.rbFormatEsc.Size = new System.Drawing.Size(43, 17);
            this.rbFormatEsc.TabIndex = 3;
            this.rbFormatEsc.TabStop = true;
            this.rbFormatEsc.Text = "Esc";
            this.rbFormatEsc.UseVisualStyleBackColor = true;
            this.rbFormatEsc.Click += new System.EventHandler(this.rbFormatEsc_Click);
            // 
            // rbFormatHTML
            // 
            this.rbFormatHTML.AutoSize = true;
            this.rbFormatHTML.Location = new System.Drawing.Point(69, 19);
            this.rbFormatHTML.Name = "rbFormatHTML";
            this.rbFormatHTML.Size = new System.Drawing.Size(55, 17);
            this.rbFormatHTML.TabIndex = 2;
            this.rbFormatHTML.TabStop = true;
            this.rbFormatHTML.Text = "HTML";
            this.rbFormatHTML.UseVisualStyleBackColor = true;
            this.rbFormatHTML.Click += new System.EventHandler(this.rbFormatHTML_Click);
            // 
            // rbFormatXML
            // 
            this.rbFormatXML.AutoSize = true;
            this.rbFormatXML.Location = new System.Drawing.Point(16, 19);
            this.rbFormatXML.Name = "rbFormatXML";
            this.rbFormatXML.Size = new System.Drawing.Size(47, 17);
            this.rbFormatXML.TabIndex = 1;
            this.rbFormatXML.TabStop = true;
            this.rbFormatXML.Text = "XML";
            this.rbFormatXML.UseVisualStyleBackColor = true;
            this.rbFormatXML.Click += new System.EventHandler(this.rbFormatXML_Click);
            // 
            // btnFormat
            // 
            this.btnFormat.Location = new System.Drawing.Point(199, 16);
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(75, 23);
            this.btnFormat.TabIndex = 4;
            this.btnFormat.Text = "Format";
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // panOk
            // 
            this.panOk.Controls.Add(this.btnOk);
            this.panOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.panOk.Location = new System.Drawing.Point(395, 16);
            this.panOk.Name = "panOk";
            this.panOk.Size = new System.Drawing.Size(91, 28);
            this.panOk.TabIndex = 3;
            // 
            // panSave
            // 
            this.panSave.Controls.Add(this.btnSave);
            this.panSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.panSave.Location = new System.Drawing.Point(304, 16);
            this.panSave.Name = "panSave";
            this.panSave.Size = new System.Drawing.Size(91, 28);
            this.panSave.TabIndex = 2;
            // 
            // panExecute
            // 
            this.panExecute.Controls.Add(this.btnExecute);
            this.panExecute.Dock = System.Windows.Forms.DockStyle.Right;
            this.panExecute.Location = new System.Drawing.Point(190, 16);
            this.panExecute.Name = "panExecute";
            this.panExecute.Size = new System.Drawing.Size(114, 28);
            this.panExecute.TabIndex = 1;
            // 
            // panLiveUpdate
            // 
            this.panLiveUpdate.Controls.Add(this.chkLiveUpdate);
            this.panLiveUpdate.Dock = System.Windows.Forms.DockStyle.Left;
            this.panLiveUpdate.Location = new System.Drawing.Point(3, 16);
            this.panLiveUpdate.Name = "panLiveUpdate";
            this.panLiveUpdate.Size = new System.Drawing.Size(131, 28);
            this.panLiveUpdate.TabIndex = 5;
            // 
            // XmlContentDisplayDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(578, 546);
            this.Controls.Add(this.txtXML);
            this.Controls.Add(this.panFormatting);
            this.Controls.Add(this.panActions);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.KeyPreview = true;
            this.Name = "XmlContentDisplayDialog";
            this.ShowIcon = false;
            this.DockStateChanged += new System.EventHandler(this.XmlContentDisplayDialog_DockStateChanged);
            this.Load += new System.EventHandler(this.XmlContentDisplayDialog_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XmlContentDisplayDialog_KeyDown);
            this.panCancel.ResumeLayout(false);
            this.panActions.ResumeLayout(false);
            this.gbActions.ResumeLayout(false);
            this.panFormatting.ResumeLayout(false);
            this.gbFormatting.ResumeLayout(false);
            this.gbFormatting.PerformLayout();
            this.panOk.ResumeLayout(false);
            this.panSave.ResumeLayout(false);
            this.panExecute.ResumeLayout(false);
            this.panLiveUpdate.ResumeLayout(false);
            this.panLiveUpdate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panCancel;
        private System.Windows.Forms.Button btnCancel;
        internal CSRichTextBoxSyntaxHighlighting.XMLViewer txtXML;
        internal System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panActions;
        private System.Windows.Forms.GroupBox gbActions;
        private System.Windows.Forms.Panel panFormatting;
        private System.Windows.Forms.GroupBox gbFormatting;
        private System.Windows.Forms.RadioButton rbFormatHTML;
        private System.Windows.Forms.RadioButton rbFormatXML;
        private System.Windows.Forms.RadioButton rbFormatEsc;
        internal System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.Panel panExecute;
        private System.Windows.Forms.Panel panSave;
        private System.Windows.Forms.Panel panOk;
        private System.Windows.Forms.Panel panLiveUpdate;
        internal System.Windows.Forms.CheckBox chkLiveUpdate;
    }
}