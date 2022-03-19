namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    partial class SimplerBuilder
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
            this.gbColumns = new System.Windows.Forms.GroupBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.gbFilters = new System.Windows.Forms.GroupBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.gbRelateds = new System.Windows.Forms.GroupBox();
            this.gbTable = new System.Windows.Forms.GroupBox();
            this.xrmTable = new Rappen.XTB.Helpers.Controls.XRMEntityComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.gbRelateds.SuspendLayout();
            this.gbTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbColumns
            // 
            this.gbColumns.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbColumns.Location = new System.Drawing.Point(0, 100);
            this.gbColumns.Name = "gbColumns";
            this.gbColumns.Size = new System.Drawing.Size(517, 100);
            this.gbColumns.TabIndex = 0;
            this.gbColumns.TabStop = false;
            this.gbColumns.Text = "Columns";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 200);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(517, 8);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // gbFilters
            // 
            this.gbFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbFilters.Location = new System.Drawing.Point(0, 208);
            this.gbFilters.Name = "gbFilters";
            this.gbFilters.Size = new System.Drawing.Size(517, 100);
            this.gbFilters.TabIndex = 2;
            this.gbFilters.TabStop = false;
            this.gbFilters.Text = "Filters";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 308);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(517, 8);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // gbRelateds
            // 
            this.gbRelateds.Controls.Add(this.comboBox1);
            this.gbRelateds.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbRelateds.Location = new System.Drawing.Point(0, 316);
            this.gbRelateds.Name = "gbRelateds";
            this.gbRelateds.Size = new System.Drawing.Size(517, 100);
            this.gbRelateds.TabIndex = 4;
            this.gbRelateds.TabStop = false;
            this.gbRelateds.Text = "Relateds";
            // 
            // gbTable
            // 
            this.gbTable.Controls.Add(this.xrmTable);
            this.gbTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbTable.Location = new System.Drawing.Point(0, 0);
            this.gbTable.Name = "gbTable";
            this.gbTable.Size = new System.Drawing.Size(517, 100);
            this.gbTable.TabIndex = 5;
            this.gbTable.TabStop = false;
            this.gbTable.Text = "Table";
            // 
            // xrmTable
            // 
            this.xrmTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xrmTable.FormattingEnabled = true;
            this.xrmTable.Location = new System.Drawing.Point(13, 26);
            this.xrmTable.Name = "xrmTable";
            this.xrmTable.Size = new System.Drawing.Size(492, 28);
            this.xrmTable.TabIndex = 0;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(239, 26);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 28);
            this.comboBox1.TabIndex = 0;
            // 
            // SimplerBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(517, 566);
            this.Controls.Add(this.gbRelateds);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.gbFilters);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.gbColumns);
            this.Controls.Add(this.gbTable);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SimplerBuilder";
            this.Text = "SimplerBuilder";
            this.gbRelateds.ResumeLayout(false);
            this.gbTable.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbColumns;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox gbFilters;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.GroupBox gbRelateds;
        private System.Windows.Forms.GroupBox gbTable;
        private Rappen.XTB.Helpers.Controls.XRMEntityComboBox xrmTable;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}