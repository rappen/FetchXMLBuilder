namespace Cinteros.Xrm.FetchXmlBuilder.Forms
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
            CSRichTextBoxSyntaxHighlighting.XMLViewerSettings xmlViewerSettings2 = new CSRichTextBoxSyntaxHighlighting.XMLViewerSettings();
            this.panBottom = new System.Windows.Forms.Panel();
            this.panOk = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.panCancel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnFormat = new System.Windows.Forms.Button();
            this.txtXML = new CSRichTextBoxSyntaxHighlighting.XMLViewer();
            this.panBottom.SuspendLayout();
            this.panOk.SuspendLayout();
            this.panCancel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panBottom
            // 
            this.panBottom.BackColor = System.Drawing.SystemColors.Control;
            this.panBottom.Controls.Add(this.panOk);
            this.panBottom.Controls.Add(this.panCancel);
            this.panBottom.Controls.Add(this.btnFormat);
            this.panBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panBottom.Location = new System.Drawing.Point(0, 667);
            this.panBottom.Name = "panBottom";
            this.panBottom.Size = new System.Drawing.Size(809, 38);
            this.panBottom.TabIndex = 4;
            // 
            // panOk
            // 
            this.panOk.Controls.Add(this.btnOk);
            this.panOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.panOk.Location = new System.Drawing.Point(627, 0);
            this.panOk.Name = "panOk";
            this.panOk.Size = new System.Drawing.Size(91, 38);
            this.panOk.TabIndex = 4;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(3, 8);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panCancel
            // 
            this.panCancel.Controls.Add(this.btnCancel);
            this.panCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.panCancel.Location = new System.Drawing.Point(718, 0);
            this.panCancel.Name = "panCancel";
            this.panCancel.Size = new System.Drawing.Size(91, 38);
            this.panCancel.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(3, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnFormat
            // 
            this.btnFormat.Location = new System.Drawing.Point(12, 8);
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(75, 23);
            this.btnFormat.TabIndex = 2;
            this.btnFormat.Text = "Format XML";
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtXML
            // 
            this.txtXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtXML.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtXML.Location = new System.Drawing.Point(0, 0);
            this.txtXML.Name = "txtXML";
            xmlViewerSettings2.AttributeKey = System.Drawing.Color.Red;
            xmlViewerSettings2.AttributeValue = System.Drawing.Color.Blue;
            xmlViewerSettings2.Element = System.Drawing.Color.DarkRed;
            xmlViewerSettings2.Tag = System.Drawing.Color.Blue;
            xmlViewerSettings2.Value = System.Drawing.Color.Black;
            this.txtXML.Settings = xmlViewerSettings2;
            this.txtXML.Size = new System.Drawing.Size(809, 667);
            this.txtXML.TabIndex = 1;
            this.txtXML.Text = "";
            // 
            // XmlContentDisplayDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(809, 705);
            this.Controls.Add(this.txtXML);
            this.Controls.Add(this.panBottom);
            this.KeyPreview = true;
            this.Name = "XmlContentDisplayDialog";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XmlContentDisplayDialog_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XmlContentDisplayDialog_KeyDown);
            this.panBottom.ResumeLayout(false);
            this.panOk.ResumeLayout(false);
            this.panCancel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panBottom;
        private System.Windows.Forms.Panel panOk;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panCancel;
        private System.Windows.Forms.Button btnCancel;
        internal CSRichTextBoxSyntaxHighlighting.XMLViewer txtXML;
        internal System.Windows.Forms.Button btnFormat;
    }
}