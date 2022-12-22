namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    partial class XmlContentControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlContentControl));
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnParseQE = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panCancel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panActions = new System.Windows.Forms.Panel();
            this.gbActions = new System.Windows.Forms.GroupBox();
            this.panSQL4CDS = new System.Windows.Forms.Panel();
            this.btnSQL4CDS = new System.Windows.Forms.Button();
            this.lblActionsExpander = new System.Windows.Forms.Label();
            this.panExecute = new System.Windows.Forms.Panel();
            this.panSave = new System.Windows.Forms.Panel();
            this.panOk = new System.Windows.Forms.Panel();
            this.panLiveUpdate = new System.Windows.Forms.Panel();
            this.chkLiveUpdate = new System.Windows.Forms.CheckBox();
            this.panParseQE = new System.Windows.Forms.Panel();
            this.chkQExComments = new System.Windows.Forms.CheckBox();
            this.panFormatting = new System.Windows.Forms.Panel();
            this.gbFormatting = new System.Windows.Forms.GroupBox();
            this.lblFormatExpander = new System.Windows.Forms.Label();
            this.rbFormatMini = new System.Windows.Forms.RadioButton();
            this.rbFormatEsc = new System.Windows.Forms.RadioButton();
            this.rbFormatHTML = new System.Windows.Forms.RadioButton();
            this.rbFormatXML = new System.Windows.Forms.RadioButton();
            this.btnFormat = new System.Windows.Forms.Button();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.panSQL4CDSInfo = new System.Windows.Forms.Panel();
            this.lblSQL4CDSInfo = new System.Windows.Forms.Label();
            this.txtXML = new ScintillaNET.Scintilla();
            this.autocompleteImageList = new System.Windows.Forms.ImageList(this.components);
            this.tmLiveUpdate = new System.Windows.Forms.Timer(this.components);
            this.panQExOptions = new System.Windows.Forms.Panel();
            this.numQExIndent = new System.Windows.Forms.NumericUpDown();
            this.rbQExObjectinitializer = new System.Windows.Forms.RadioButton();
            this.rbQExLineByLine = new System.Windows.Forms.RadioButton();
            this.chkQExFilterVariables = new System.Windows.Forms.CheckBox();
            this.cmbQExFlavor = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbQExStyle = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panCancel.SuspendLayout();
            this.panActions.SuspendLayout();
            this.gbActions.SuspendLayout();
            this.panSQL4CDS.SuspendLayout();
            this.panExecute.SuspendLayout();
            this.panSave.SuspendLayout();
            this.panOk.SuspendLayout();
            this.panLiveUpdate.SuspendLayout();
            this.panParseQE.SuspendLayout();
            this.panFormatting.SuspendLayout();
            this.gbFormatting.SuspendLayout();
            this.panSQL4CDSInfo.SuspendLayout();
            this.panQExOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQExIndent)).BeginInit();
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
            // btnParseQE
            // 
            this.btnParseQE.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnParseQE.Location = new System.Drawing.Point(4, 10);
            this.btnParseQE.Name = "btnParseQE";
            this.btnParseQE.Size = new System.Drawing.Size(75, 23);
            this.btnParseQE.TabIndex = 3;
            this.btnParseQE.Text = "Parse";
            this.btnParseQE.UseVisualStyleBackColor = true;
            this.btnParseQE.Click += new System.EventHandler(this.btnParseQE_Click);
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
            this.panCancel.Location = new System.Drawing.Point(1001, 16);
            this.panCancel.Name = "panCancel";
            this.panCancel.Size = new System.Drawing.Size(91, 28);
            this.panCancel.TabIndex = 4;
            this.panCancel.Visible = false;
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
            // panActions
            // 
            this.panActions.Controls.Add(this.gbActions);
            this.panActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panActions.Location = new System.Drawing.Point(0, 309);
            this.panActions.Name = "panActions";
            this.panActions.Size = new System.Drawing.Size(1093, 50);
            this.panActions.TabIndex = 10;
            // 
            // gbActions
            // 
            this.gbActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbActions.Controls.Add(this.panSQL4CDS);
            this.gbActions.Controls.Add(this.lblActionsExpander);
            this.gbActions.Controls.Add(this.panExecute);
            this.gbActions.Controls.Add(this.panSave);
            this.gbActions.Controls.Add(this.panOk);
            this.gbActions.Controls.Add(this.panCancel);
            this.gbActions.Controls.Add(this.panLiveUpdate);
            this.gbActions.Location = new System.Drawing.Point(-1, 4);
            this.gbActions.Name = "gbActions";
            this.gbActions.Size = new System.Drawing.Size(1095, 47);
            this.gbActions.TabIndex = 4;
            this.gbActions.TabStop = false;
            this.gbActions.Text = "Actions";
            // 
            // panSQL4CDS
            // 
            this.panSQL4CDS.Controls.Add(this.btnSQL4CDS);
            this.panSQL4CDS.Dock = System.Windows.Forms.DockStyle.Right;
            this.panSQL4CDS.Location = new System.Drawing.Point(587, 16);
            this.panSQL4CDS.Name = "panSQL4CDS";
            this.panSQL4CDS.Size = new System.Drawing.Size(118, 28);
            this.panSQL4CDS.TabIndex = 8;
            this.panSQL4CDS.Visible = false;
            // 
            // btnSQL4CDS
            // 
            this.btnSQL4CDS.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSQL4CDS.Location = new System.Drawing.Point(6, 0);
            this.btnSQL4CDS.Name = "btnSQL4CDS";
            this.btnSQL4CDS.Size = new System.Drawing.Size(106, 23);
            this.btnSQL4CDS.TabIndex = 4;
            this.btnSQL4CDS.Text = "Edit in SQL 4 CDS";
            this.btnSQL4CDS.UseVisualStyleBackColor = true;
            this.btnSQL4CDS.Click += new System.EventHandler(this.btnSQL4CDS_Click);
            // 
            // lblActionsExpander
            // 
            this.lblActionsExpander.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblActionsExpander.AutoSize = true;
            this.lblActionsExpander.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblActionsExpander.Location = new System.Drawing.Point(1073, 0);
            this.lblActionsExpander.Name = "lblActionsExpander";
            this.lblActionsExpander.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblActionsExpander.Size = new System.Drawing.Size(14, 13);
            this.lblActionsExpander.TabIndex = 7;
            this.lblActionsExpander.Text = "–";
            this.lblActionsExpander.Click += new System.EventHandler(this.llGroupBoxExpander_Clicked);
            // 
            // panExecute
            // 
            this.panExecute.Controls.Add(this.btnExecute);
            this.panExecute.Dock = System.Windows.Forms.DockStyle.Right;
            this.panExecute.Location = new System.Drawing.Point(705, 16);
            this.panExecute.Name = "panExecute";
            this.panExecute.Size = new System.Drawing.Size(114, 28);
            this.panExecute.TabIndex = 1;
            // 
            // panSave
            // 
            this.panSave.Controls.Add(this.btnSave);
            this.panSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.panSave.Location = new System.Drawing.Point(819, 16);
            this.panSave.Name = "panSave";
            this.panSave.Size = new System.Drawing.Size(91, 28);
            this.panSave.TabIndex = 2;
            // 
            // panOk
            // 
            this.panOk.Controls.Add(this.btnOk);
            this.panOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.panOk.Location = new System.Drawing.Point(910, 16);
            this.panOk.Name = "panOk";
            this.panOk.Size = new System.Drawing.Size(91, 28);
            this.panOk.TabIndex = 3;
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
            // panParseQE
            // 
            this.panParseQE.Controls.Add(this.btnParseQE);
            this.panParseQE.Dock = System.Windows.Forms.DockStyle.Right;
            this.panParseQE.Location = new System.Drawing.Point(1002, 0);
            this.panParseQE.Name = "panParseQE";
            this.panParseQE.Size = new System.Drawing.Size(91, 59);
            this.panParseQE.TabIndex = 10;
            // 
            // chkQExComments
            // 
            this.chkQExComments.AutoSize = true;
            this.chkQExComments.Location = new System.Drawing.Point(434, 37);
            this.chkQExComments.Name = "chkQExComments";
            this.chkQExComments.Size = new System.Drawing.Size(75, 17);
            this.chkQExComments.TabIndex = 4;
            this.chkQExComments.Text = "Comments";
            this.chkQExComments.UseVisualStyleBackColor = true;
            this.chkQExComments.CheckedChanged += new System.EventHandler(this.chkQExComments_CheckedChanged);
            // 
            // panFormatting
            // 
            this.panFormatting.Controls.Add(this.gbFormatting);
            this.panFormatting.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panFormatting.Location = new System.Drawing.Point(0, 265);
            this.panFormatting.MaximumSize = new System.Drawing.Size(10000, 44);
            this.panFormatting.Name = "panFormatting";
            this.panFormatting.Size = new System.Drawing.Size(1093, 44);
            this.panFormatting.TabIndex = 7;
            // 
            // gbFormatting
            // 
            this.gbFormatting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFormatting.Controls.Add(this.lblFormatExpander);
            this.gbFormatting.Controls.Add(this.rbFormatMini);
            this.gbFormatting.Controls.Add(this.rbFormatEsc);
            this.gbFormatting.Controls.Add(this.rbFormatHTML);
            this.gbFormatting.Controls.Add(this.rbFormatXML);
            this.gbFormatting.Controls.Add(this.btnFormat);
            this.gbFormatting.Location = new System.Drawing.Point(-1, 3);
            this.gbFormatting.Name = "gbFormatting";
            this.gbFormatting.Size = new System.Drawing.Size(1095, 43);
            this.gbFormatting.TabIndex = 4;
            this.gbFormatting.TabStop = false;
            this.gbFormatting.Text = "Formatting";
            // 
            // lblFormatExpander
            // 
            this.lblFormatExpander.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFormatExpander.AutoSize = true;
            this.lblFormatExpander.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblFormatExpander.Location = new System.Drawing.Point(1073, 0);
            this.lblFormatExpander.Name = "lblFormatExpander";
            this.lblFormatExpander.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblFormatExpander.Size = new System.Drawing.Size(14, 13);
            this.lblFormatExpander.TabIndex = 6;
            this.lblFormatExpander.Text = "–";
            this.lblFormatExpander.Click += new System.EventHandler(this.llGroupBoxExpander_Clicked);
            // 
            // rbFormatMini
            // 
            this.rbFormatMini.AutoSize = true;
            this.rbFormatMini.Location = new System.Drawing.Point(179, 19);
            this.rbFormatMini.Name = "rbFormatMini";
            this.rbFormatMini.Size = new System.Drawing.Size(44, 17);
            this.rbFormatMini.TabIndex = 5;
            this.rbFormatMini.TabStop = true;
            this.rbFormatMini.Text = "Mini";
            this.rbFormatMini.UseVisualStyleBackColor = true;
            this.rbFormatMini.Click += new System.EventHandler(this.rbFormatMini_Click);
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
            this.btnFormat.Location = new System.Drawing.Point(267, 16);
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(75, 23);
            this.btnFormat.TabIndex = 4;
            this.btnFormat.Text = "Format";
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // panSQL4CDSInfo
            // 
            this.panSQL4CDSInfo.BackColor = System.Drawing.SystemColors.Info;
            this.panSQL4CDSInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panSQL4CDSInfo.Controls.Add(this.lblSQL4CDSInfo);
            this.panSQL4CDSInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panSQL4CDSInfo.Location = new System.Drawing.Point(0, 229);
            this.panSQL4CDSInfo.Name = "panSQL4CDSInfo";
            this.panSQL4CDSInfo.Padding = new System.Windows.Forms.Padding(4);
            this.panSQL4CDSInfo.Size = new System.Drawing.Size(1093, 36);
            this.panSQL4CDSInfo.TabIndex = 11;
            // 
            // lblSQL4CDSInfo
            // 
            this.lblSQL4CDSInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSQL4CDSInfo.Location = new System.Drawing.Point(4, 4);
            this.lblSQL4CDSInfo.Name = "lblSQL4CDSInfo";
            this.lblSQL4CDSInfo.Size = new System.Drawing.Size(1083, 26);
            this.lblSQL4CDSInfo.TabIndex = 0;
            this.lblSQL4CDSInfo.Text = "FetchXML to SQL conversion can also be performed with when the SQL 4 CDS tool. Ge" +
    "t it from the Tool Library and enable SQL 4 CDS in FetchXML Builder Options to g" +
    "et even more accurate results.";
            // 
            // txtXML
            // 
            this.txtXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtXML.Location = new System.Drawing.Point(0, 59);
            this.txtXML.Name = "txtXML";
            this.txtXML.Size = new System.Drawing.Size(1093, 170);
            this.txtXML.TabIndex = 1;
            this.txtXML.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtXML_KeyPress);
            this.txtXML.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtXML_KeyUp);
            // 
            // autocompleteImageList
            // 
            this.autocompleteImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("autocompleteImageList.ImageStream")));
            this.autocompleteImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.autocompleteImageList.Images.SetKeyName(0, "XMLIntellisenseElement_16x.png");
            this.autocompleteImageList.Images.SetKeyName(1, "XMLIntellisenseAttribute_16x.png");
            this.autocompleteImageList.Images.SetKeyName(2, "XMLIntellisenseAttributeHighConfidence_16x.png");
            this.autocompleteImageList.Images.SetKeyName(3, "XMLIntelliSenseDescendant_16x.png");
            this.autocompleteImageList.Images.SetKeyName(4, "XMLIntellisenseElement_16x.png");
            // 
            // tmLiveUpdate
            // 
            this.tmLiveUpdate.Interval = 1000;
            this.tmLiveUpdate.Tick += new System.EventHandler(this.tmLiveUpdate_Tick);
            // 
            // panQExOptions
            // 
            this.panQExOptions.BackColor = System.Drawing.SystemColors.Window;
            this.panQExOptions.Controls.Add(this.label3);
            this.panQExOptions.Controls.Add(this.numQExIndent);
            this.panQExOptions.Controls.Add(this.rbQExObjectinitializer);
            this.panQExOptions.Controls.Add(this.rbQExLineByLine);
            this.panQExOptions.Controls.Add(this.chkQExFilterVariables);
            this.panQExOptions.Controls.Add(this.cmbQExFlavor);
            this.panQExOptions.Controls.Add(this.label2);
            this.panQExOptions.Controls.Add(this.label1);
            this.panQExOptions.Controls.Add(this.chkQExComments);
            this.panQExOptions.Controls.Add(this.panParseQE);
            this.panQExOptions.Controls.Add(this.cmbQExStyle);
            this.panQExOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panQExOptions.Location = new System.Drawing.Point(0, 0);
            this.panQExOptions.Name = "panQExOptions";
            this.panQExOptions.Size = new System.Drawing.Size(1093, 59);
            this.panQExOptions.TabIndex = 0;
            this.panQExOptions.Text = "Type of generated code";
            // 
            // numQExIndent
            // 
            this.numQExIndent.Location = new System.Drawing.Point(328, 36);
            this.numQExIndent.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numQExIndent.Name = "numQExIndent";
            this.numQExIndent.Size = new System.Drawing.Size(46, 20);
            this.numQExIndent.TabIndex = 12;
            this.numQExIndent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numQExIndent.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numQExIndent.ValueChanged += new System.EventHandler(this.numQExIndent_ValueChanged);
            // 
            // rbQExObjectinitializer
            // 
            this.rbQExObjectinitializer.AutoSize = true;
            this.rbQExObjectinitializer.Location = new System.Drawing.Point(129, 36);
            this.rbQExObjectinitializer.Name = "rbQExObjectinitializer";
            this.rbQExObjectinitializer.Size = new System.Drawing.Size(98, 17);
            this.rbQExObjectinitializer.TabIndex = 3;
            this.rbQExObjectinitializer.Text = "Object initializer";
            this.rbQExObjectinitializer.UseVisualStyleBackColor = true;
            this.rbQExObjectinitializer.CheckedChanged += new System.EventHandler(this.rbQExObjectInitializer_CheckedChanged);
            // 
            // rbQExLineByLine
            // 
            this.rbQExLineByLine.AutoSize = true;
            this.rbQExLineByLine.Checked = true;
            this.rbQExLineByLine.Location = new System.Drawing.Point(48, 36);
            this.rbQExLineByLine.Name = "rbQExLineByLine";
            this.rbQExLineByLine.Size = new System.Drawing.Size(78, 17);
            this.rbQExLineByLine.TabIndex = 2;
            this.rbQExLineByLine.TabStop = true;
            this.rbQExLineByLine.Text = "Line-by-line";
            this.rbQExLineByLine.UseVisualStyleBackColor = true;
            // 
            // chkQExFilterVariables
            // 
            this.chkQExFilterVariables.AutoSize = true;
            this.chkQExFilterVariables.Location = new System.Drawing.Point(515, 37);
            this.chkQExFilterVariables.Name = "chkQExFilterVariables";
            this.chkQExFilterVariables.Size = new System.Drawing.Size(94, 17);
            this.chkQExFilterVariables.TabIndex = 5;
            this.chkQExFilterVariables.Text = "Filter Variables";
            this.chkQExFilterVariables.UseVisualStyleBackColor = true;
            this.chkQExFilterVariables.CheckedChanged += new System.EventHandler(this.chkQExFilterVariables_CheckedChanged);
            // 
            // cmbQExFlavor
            // 
            this.cmbQExFlavor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQExFlavor.FormattingEnabled = true;
            this.cmbQExFlavor.Location = new System.Drawing.Point(434, 10);
            this.cmbQExFlavor.Name = "cmbQExFlavor";
            this.cmbQExFlavor.Size = new System.Drawing.Size(186, 21);
            this.cmbQExFlavor.TabIndex = 1;
            this.cmbQExFlavor.SelectedIndexChanged += new System.EventHandler(this.cmbQExFlavor_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(392, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Flavor";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Style";
            // 
            // cmbQExStyle
            // 
            this.cmbQExStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQExStyle.FormattingEnabled = true;
            this.cmbQExStyle.Location = new System.Drawing.Point(48, 10);
            this.cmbQExStyle.Name = "cmbQExStyle";
            this.cmbQExStyle.Size = new System.Drawing.Size(326, 21);
            this.cmbQExStyle.TabIndex = 0;
            this.cmbQExStyle.SelectedIndexChanged += new System.EventHandler(this.cmbQExStyle_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Indentation";
            // 
            // XmlContentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1093, 359);
            this.Controls.Add(this.txtXML);
            this.Controls.Add(this.panQExOptions);
            this.Controls.Add(this.panSQL4CDSInfo);
            this.Controls.Add(this.panFormatting);
            this.Controls.Add(this.panActions);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.HideOnClose = true;
            this.KeyPreview = true;
            this.Name = "XmlContentControl";
            this.DockStateChanged += new System.EventHandler(this.XmlContentDisplayDialog_DockStateChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XmlContentDisplayDialog_FormClosing);
            this.Load += new System.EventHandler(this.XmlContentDisplayDialog_Load);
            this.VisibleChanged += new System.EventHandler(this.XmlContentDisplayDialog_VisibleChanged);
            this.Enter += new System.EventHandler(this.XmlContentControl_Enter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XmlContentDisplayDialog_KeyDown);
            this.panCancel.ResumeLayout(false);
            this.panActions.ResumeLayout(false);
            this.gbActions.ResumeLayout(false);
            this.gbActions.PerformLayout();
            this.panSQL4CDS.ResumeLayout(false);
            this.panExecute.ResumeLayout(false);
            this.panSave.ResumeLayout(false);
            this.panOk.ResumeLayout(false);
            this.panLiveUpdate.ResumeLayout(false);
            this.panLiveUpdate.PerformLayout();
            this.panParseQE.ResumeLayout(false);
            this.panFormatting.ResumeLayout(false);
            this.gbFormatting.ResumeLayout(false);
            this.gbFormatting.PerformLayout();
            this.panSQL4CDSInfo.ResumeLayout(false);
            this.panQExOptions.ResumeLayout(false);
            this.panQExOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQExIndent)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panCancel;
        private System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnExecute;
        internal System.Windows.Forms.Button btnParseQE;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panActions;
        private System.Windows.Forms.GroupBox gbActions;
        private System.Windows.Forms.Panel panFormatting;
        private System.Windows.Forms.GroupBox gbFormatting;
        private System.Windows.Forms.RadioButton rbFormatHTML;
        private System.Windows.Forms.RadioButton rbFormatXML;
        private System.Windows.Forms.RadioButton rbFormatEsc;
        private System.Windows.Forms.RadioButton rbFormatMini;
        internal System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.Panel panExecute;
        private System.Windows.Forms.Panel panParseQE;
        private System.Windows.Forms.Panel panSave;
        private System.Windows.Forms.Panel panOk;
        private System.Windows.Forms.Panel panLiveUpdate;
        internal System.Windows.Forms.CheckBox chkLiveUpdate;
        private System.Windows.Forms.Label lblFormatExpander;
        private System.Windows.Forms.Label lblActionsExpander;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.Panel panSQL4CDS;
        private System.Windows.Forms.Button btnSQL4CDS;
        private System.Windows.Forms.Panel panSQL4CDSInfo;
        private System.Windows.Forms.Label lblSQL4CDSInfo;
        internal ScintillaNET.Scintilla txtXML;
        private System.Windows.Forms.ImageList autocompleteImageList;
        private System.Windows.Forms.Timer tmLiveUpdate;
        private System.Windows.Forms.CheckBox chkQExComments;
        private System.Windows.Forms.Panel panQExOptions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbQExStyle;
        private System.Windows.Forms.ComboBox cmbQExFlavor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkQExFilterVariables;
        private System.Windows.Forms.RadioButton rbQExObjectinitializer;
        private System.Windows.Forms.RadioButton rbQExLineByLine;
        private System.Windows.Forms.NumericUpDown numQExIndent;
        private System.Windows.Forms.Label label3;
    }
}