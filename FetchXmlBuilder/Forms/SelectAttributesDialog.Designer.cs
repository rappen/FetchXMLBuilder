namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    partial class SelectAttributesDialog
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.panOk = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.panCancel = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lvAttributes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            this.panOk.SuspendLayout();
            this.panCancel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panOk);
            this.panel2.Controls.Add(this.panCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 384);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(566, 38);
            this.panel2.TabIndex = 6;
            // 
            // panOk
            // 
            this.panOk.Controls.Add(this.btnOk);
            this.panOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.panOk.Location = new System.Drawing.Point(384, 0);
            this.panOk.Name = "panOk";
            this.panOk.Size = new System.Drawing.Size(91, 38);
            this.panOk.TabIndex = 6;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(3, 8);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // panCancel
            // 
            this.panCancel.Controls.Add(this.button2);
            this.panCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.panCancel.Location = new System.Drawing.Point(475, 0);
            this.panCancel.Name = "panCancel";
            this.panCancel.Size = new System.Drawing.Size(91, 38);
            this.panCancel.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(3, 8);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(566, 39);
            this.panel1.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lvAttributes);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 39);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(566, 345);
            this.panel3.TabIndex = 9;
            // 
            // lvAttributes
            // 
            this.lvAttributes.CheckBoxes = true;
            this.lvAttributes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAttributes.FullRowSelect = true;
            this.lvAttributes.HideSelection = false;
            this.lvAttributes.Location = new System.Drawing.Point(0, 0);
            this.lvAttributes.Name = "lvAttributes";
            this.lvAttributes.ShowGroups = false;
            this.lvAttributes.Size = new System.Drawing.Size(566, 345);
            this.lvAttributes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvAttributes.TabIndex = 8;
            this.lvAttributes.UseCompatibleStateImageBehavior = false;
            this.lvAttributes.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 166;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Logical Name";
            this.columnHeader2.Width = 169;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 114;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(13, 13);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(125, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Check / Uncheck all";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // SelectAttributesDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(566, 422);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "SelectAttributesDialog";
            this.Text = "Select Attributes";
            this.panel2.ResumeLayout(false);
            this.panOk.ResumeLayout(false);
            this.panCancel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panOk;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panCancel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView lvAttributes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;

    }
}