namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    partial class ResultGrid
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ctxmenuGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRecordOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRecordCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxColumn = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxColumnOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxColumnCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRecordSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.ctxBehavior = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxColumns = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxFind = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblOptionsExpander = new System.Windows.Forms.Label();
            this.mnuOptions = new System.Windows.Forms.MenuStrip();
            this.mnuBehavior = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFriendly = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBothNames = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLocalTime = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyHeaders = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuQuickFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPagingCookie = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowElapsed = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuColumns = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIndexCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIdCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNullCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSysCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuResetLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowAllCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAutoSizeAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowLayoutXML = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPagePlus = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPage = new System.Windows.Forms.ToolStripTextBox();
            this.mnuPageMinus = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRecordsNumbers = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRetrieveTime = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.panQuickFilter = new System.Windows.Forms.Panel();
            this.txtQuickFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tmFilter = new System.Windows.Forms.Timer(this.components);
            this.txtPagingCookie = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.gbPagingCookie = new System.Windows.Forms.GroupBox();
            this.crmGridView1 = new Rappen.XTB.Helpers.Controls.XRMDataGridView();
            this.ctxmenuGrid.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.mnuOptions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panQuickFilter.SuspendLayout();
            this.panel3.SuspendLayout();
            this.gbPagingCookie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crmGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ctxmenuGrid
            // 
            this.ctxmenuGrid.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxmenuGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxRecord,
            this.ctxColumn,
            this.ctxRecordSeparator,
            this.ctxBehavior,
            this.ctxColumns,
            this.toolStripSeparator1,
            this.ctxFind});
            this.ctxmenuGrid.Name = "ctxmenuGrid";
            this.ctxmenuGrid.Size = new System.Drawing.Size(163, 126);
            this.ctxmenuGrid.Opening += new System.ComponentModel.CancelEventHandler(this.ctxmenuGrid_Opening);
            this.ctxmenuGrid.Opened += new System.EventHandler(this.ctxmenuGrid_Opened);
            // 
            // ctxRecord
            // 
            this.ctxRecord.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxRecordOpen,
            this.ctxRecordCopy});
            this.ctxRecord.Name = "ctxRecord";
            this.ctxRecord.Size = new System.Drawing.Size(162, 22);
            this.ctxRecord.Text = "Selected record";
            // 
            // ctxRecordOpen
            // 
            this.ctxRecordOpen.Name = "ctxRecordOpen";
            this.ctxRecordOpen.Size = new System.Drawing.Size(126, 22);
            this.ctxRecordOpen.Text = "Open...";
            this.ctxRecordOpen.Click += new System.EventHandler(this.ctxOpen_Click);
            // 
            // ctxRecordCopy
            // 
            this.ctxRecordCopy.Name = "ctxRecordCopy";
            this.ctxRecordCopy.Size = new System.Drawing.Size(126, 22);
            this.ctxRecordCopy.Text = "Copy URL";
            this.ctxRecordCopy.Click += new System.EventHandler(this.ctxCopy_Click);
            // 
            // ctxColumn
            // 
            this.ctxColumn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxColumnOpen,
            this.ctxColumnCopy});
            this.ctxColumn.Name = "ctxColumn";
            this.ctxColumn.Size = new System.Drawing.Size(162, 22);
            this.ctxColumn.Text = "Selected column";
            // 
            // ctxColumnOpen
            // 
            this.ctxColumnOpen.Name = "ctxColumnOpen";
            this.ctxColumnOpen.Size = new System.Drawing.Size(126, 22);
            this.ctxColumnOpen.Text = "Open...";
            this.ctxColumnOpen.Click += new System.EventHandler(this.ctxOpen_Click);
            // 
            // ctxColumnCopy
            // 
            this.ctxColumnCopy.Name = "ctxColumnCopy";
            this.ctxColumnCopy.Size = new System.Drawing.Size(126, 22);
            this.ctxColumnCopy.Text = "Copy URL";
            this.ctxColumnCopy.Click += new System.EventHandler(this.ctxCopy_Click);
            // 
            // ctxRecordSeparator
            // 
            this.ctxRecordSeparator.Name = "ctxRecordSeparator";
            this.ctxRecordSeparator.Size = new System.Drawing.Size(159, 6);
            // 
            // ctxBehavior
            // 
            this.ctxBehavior.Name = "ctxBehavior";
            this.ctxBehavior.Size = new System.Drawing.Size(162, 22);
            this.ctxBehavior.Text = "Appearance";
            // 
            // ctxColumns
            // 
            this.ctxColumns.Name = "ctxColumns";
            this.ctxColumns.Size = new System.Drawing.Size(162, 22);
            this.ctxColumns.Text = "Columns";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(159, 6);
            // 
            // ctxFind
            // 
            this.ctxFind.Name = "ctxFind";
            this.ctxFind.Size = new System.Drawing.Size(162, 22);
            this.ctxFind.Text = "Find...";
            this.ctxFind.Click += new System.EventHandler(this.ctxFind_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblOptionsExpander);
            this.groupBox1.Controls.Add(this.mnuOptions);
            this.groupBox1.Location = new System.Drawing.Point(-1, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(652, 45);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // lblOptionsExpander
            // 
            this.lblOptionsExpander.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOptionsExpander.AutoSize = true;
            this.lblOptionsExpander.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblOptionsExpander.Location = new System.Drawing.Point(631, 0);
            this.lblOptionsExpander.Name = "lblOptionsExpander";
            this.lblOptionsExpander.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblOptionsExpander.Size = new System.Drawing.Size(14, 13);
            this.lblOptionsExpander.TabIndex = 60;
            this.lblOptionsExpander.Text = "–";
            this.lblOptionsExpander.Click += new System.EventHandler(this.lblOptionsExpander_Click);
            // 
            // mnuOptions
            // 
            this.mnuOptions.BackColor = System.Drawing.SystemColors.Window;
            this.mnuOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mnuOptions.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mnuOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuBehavior,
            this.mnuColumns,
            this.mnuPagePlus,
            this.mnuPage,
            this.mnuPageMinus,
            this.mnuRecordsNumbers,
            this.mnuRetrieveTime,
            this.mnuExcel});
            this.mnuOptions.Location = new System.Drawing.Point(3, 12);
            this.mnuOptions.Name = "mnuOptions";
            this.mnuOptions.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.mnuOptions.ShowItemToolTips = true;
            this.mnuOptions.Size = new System.Drawing.Size(646, 30);
            this.mnuOptions.TabIndex = 61;
            this.mnuOptions.Text = "menuStrip1";
            // 
            // mnuBehavior
            // 
            this.mnuBehavior.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFriendly,
            this.mnuBothNames,
            this.mnuLocalTime,
            this.mnuCopyHeaders,
            this.toolStripMenuItem1,
            this.mnuQuickFilter,
            this.mnuPagingCookie,
            this.mnuShowElapsed});
            this.mnuBehavior.Name = "mnuBehavior";
            this.mnuBehavior.Size = new System.Drawing.Size(82, 28);
            this.mnuBehavior.Text = "Appearance";
            this.mnuBehavior.DropDownOpening += new System.EventHandler(this.mnuBehaviorColumns_DropDownOpening);
            // 
            // mnuFriendly
            // 
            this.mnuFriendly.CheckOnClick = true;
            this.mnuFriendly.Name = "mnuFriendly";
            this.mnuFriendly.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.mnuFriendly.Size = new System.Drawing.Size(228, 22);
            this.mnuFriendly.Text = "Friendly Names";
            this.mnuFriendly.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuBothNames
            // 
            this.mnuBothNames.CheckOnClick = true;
            this.mnuBothNames.Name = "mnuBothNames";
            this.mnuBothNames.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.B)));
            this.mnuBothNames.Size = new System.Drawing.Size(228, 22);
            this.mnuBothNames.Text = "Both Names";
            this.mnuBothNames.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuLocalTime
            // 
            this.mnuLocalTime.CheckOnClick = true;
            this.mnuLocalTime.Name = "mnuLocalTime";
            this.mnuLocalTime.Size = new System.Drawing.Size(228, 22);
            this.mnuLocalTime.Text = "Local Times";
            this.mnuLocalTime.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuCopyHeaders
            // 
            this.mnuCopyHeaders.CheckOnClick = true;
            this.mnuCopyHeaders.Name = "mnuCopyHeaders";
            this.mnuCopyHeaders.Size = new System.Drawing.Size(228, 22);
            this.mnuCopyHeaders.Text = "Copy with Headers";
            this.mnuCopyHeaders.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(225, 6);
            // 
            // mnuQuickFilter
            // 
            this.mnuQuickFilter.CheckOnClick = true;
            this.mnuQuickFilter.Name = "mnuQuickFilter";
            this.mnuQuickFilter.Size = new System.Drawing.Size(228, 22);
            this.mnuQuickFilter.Text = "Show Quick Filter";
            this.mnuQuickFilter.Click += new System.EventHandler(this.mnuQuickFilter_Click);
            // 
            // mnuPagingCookie
            // 
            this.mnuPagingCookie.CheckOnClick = true;
            this.mnuPagingCookie.Name = "mnuPagingCookie";
            this.mnuPagingCookie.Size = new System.Drawing.Size(228, 22);
            this.mnuPagingCookie.Text = "Show Result Paging Cookie";
            this.mnuPagingCookie.Click += new System.EventHandler(this.mnuPagingCookie_Click);
            // 
            // mnuShowElapsed
            // 
            this.mnuShowElapsed.CheckOnClick = true;
            this.mnuShowElapsed.Name = "mnuShowElapsed";
            this.mnuShowElapsed.Size = new System.Drawing.Size(228, 22);
            this.mnuShowElapsed.Text = "Show Elapsed Retrieve Time";
            this.mnuShowElapsed.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuColumns
            // 
            this.mnuColumns.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuIndexCol,
            this.mnuIdCol,
            this.mnuNullCol,
            this.mnuSysCol,
            this.mnuResetLayout,
            this.mnuShowAllCol,
            this.mnuAutoSizeAll,
            this.mnuShowLayoutXML});
            this.mnuColumns.Name = "mnuColumns";
            this.mnuColumns.Size = new System.Drawing.Size(67, 28);
            this.mnuColumns.Text = "Columns";
            this.mnuColumns.DropDownOpening += new System.EventHandler(this.mnuBehaviorColumns_DropDownOpening);
            // 
            // mnuIndexCol
            // 
            this.mnuIndexCol.CheckOnClick = true;
            this.mnuIndexCol.Name = "mnuIndexCol";
            this.mnuIndexCol.Size = new System.Drawing.Size(166, 22);
            this.mnuIndexCol.Text = "Index";
            this.mnuIndexCol.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuIdCol
            // 
            this.mnuIdCol.CheckOnClick = true;
            this.mnuIdCol.Name = "mnuIdCol";
            this.mnuIdCol.Size = new System.Drawing.Size(166, 22);
            this.mnuIdCol.Text = "Primary Key";
            this.mnuIdCol.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuNullCol
            // 
            this.mnuNullCol.CheckOnClick = true;
            this.mnuNullCol.Name = "mnuNullCol";
            this.mnuNullCol.Size = new System.Drawing.Size(166, 22);
            this.mnuNullCol.Text = "Without value";
            this.mnuNullCol.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuSysCol
            // 
            this.mnuSysCol.CheckOnClick = true;
            this.mnuSysCol.Name = "mnuSysCol";
            this.mnuSysCol.Size = new System.Drawing.Size(166, 22);
            this.mnuSysCol.Text = "System added";
            this.mnuSysCol.Click += new System.EventHandler(this.chkGridOptions_Click);
            // 
            // mnuResetLayout
            // 
            this.mnuResetLayout.Name = "mnuResetLayout";
            this.mnuResetLayout.Size = new System.Drawing.Size(166, 22);
            this.mnuResetLayout.Text = "Reset Layout";
            this.mnuResetLayout.Click += new System.EventHandler(this.mnuResetLayout_Click);
            // 
            // mnuShowAllCol
            // 
            this.mnuShowAllCol.Name = "mnuShowAllCol";
            this.mnuShowAllCol.Size = new System.Drawing.Size(166, 22);
            this.mnuShowAllCol.Text = "Show All";
            this.mnuShowAllCol.Click += new System.EventHandler(this.mnuShowAllCol_Click);
            // 
            // mnuAutoSizeAll
            // 
            this.mnuAutoSizeAll.Name = "mnuAutoSizeAll";
            this.mnuAutoSizeAll.Size = new System.Drawing.Size(166, 22);
            this.mnuAutoSizeAll.Text = "Auto Size All";
            this.mnuAutoSizeAll.Click += new System.EventHandler(this.mnuAutoSizeAll_Click);
            // 
            // mnuShowLayoutXML
            // 
            this.mnuShowLayoutXML.Name = "mnuShowLayoutXML";
            this.mnuShowLayoutXML.Size = new System.Drawing.Size(166, 22);
            this.mnuShowLayoutXML.Text = "Show LayoutXML";
            this.mnuShowLayoutXML.Click += new System.EventHandler(this.mnuShowLayoutXML_Click);
            // 
            // mnuPagePlus
            // 
            this.mnuPagePlus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuPagePlus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.mnuPagePlus.Name = "mnuPagePlus";
            this.mnuPagePlus.Size = new System.Drawing.Size(27, 28);
            this.mnuPagePlus.Tag = "1";
            this.mnuPagePlus.Text = ">";
            this.mnuPagePlus.ToolTipText = "Move to next page";
            this.mnuPagePlus.Visible = false;
            this.mnuPagePlus.Click += new System.EventHandler(this.mnuPagePlusMinus_Click);
            // 
            // mnuPage
            // 
            this.mnuPage.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuPage.AutoSize = false;
            this.mnuPage.BackColor = System.Drawing.SystemColors.Window;
            this.mnuPage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.mnuPage.Name = "mnuPage";
            this.mnuPage.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.mnuPage.Size = new System.Drawing.Size(62, 23);
            this.mnuPage.Text = "Page";
            this.mnuPage.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnuPage.Visible = false;
            this.mnuPage.Enter += new System.EventHandler(this.mnuPage_Enter);
            this.mnuPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mnuPage_KeyPress);
            this.mnuPage.Validating += new System.ComponentModel.CancelEventHandler(this.mnuPage_Validating);
            this.mnuPage.Validated += new System.EventHandler(this.mnuPage_Validated);
            // 
            // mnuPageMinus
            // 
            this.mnuPageMinus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuPageMinus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.mnuPageMinus.Name = "mnuPageMinus";
            this.mnuPageMinus.Size = new System.Drawing.Size(27, 28);
            this.mnuPageMinus.Tag = "-1";
            this.mnuPageMinus.Text = "<";
            this.mnuPageMinus.ToolTipText = "Move to previous page";
            this.mnuPageMinus.Visible = false;
            this.mnuPageMinus.Click += new System.EventHandler(this.mnuPagePlusMinus_Click);
            // 
            // mnuRecordsNumbers
            // 
            this.mnuRecordsNumbers.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuRecordsNumbers.BackColor = System.Drawing.SystemColors.Window;
            this.mnuRecordsNumbers.Name = "mnuRecordsNumbers";
            this.mnuRecordsNumbers.Size = new System.Drawing.Size(61, 28);
            this.mnuRecordsNumbers.Text = "Records";
            this.mnuRecordsNumbers.Visible = false;
            // 
            // mnuRetrieveTime
            // 
            this.mnuRetrieveTime.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuRetrieveTime.BackColor = System.Drawing.SystemColors.Window;
            this.mnuRetrieveTime.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_stopwatch_20;
            this.mnuRetrieveTime.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnuRetrieveTime.Name = "mnuRetrieveTime";
            this.mnuRetrieveTime.Size = new System.Drawing.Size(32, 28);
            this.mnuRetrieveTime.Visible = false;
            // 
            // mnuExcel
            // 
            this.mnuExcel.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_excel_24;
            this.mnuExcel.Name = "mnuExcel";
            this.mnuExcel.Size = new System.Drawing.Size(70, 28);
            this.mnuExcel.Text = "Excel";
            this.mnuExcel.Click += new System.EventHandler(this.mnuExcel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 226);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(650, 47);
            this.panel1.TabIndex = 5;
            // 
            // panQuickFilter
            // 
            this.panQuickFilter.Controls.Add(this.txtQuickFilter);
            this.panQuickFilter.Controls.Add(this.label1);
            this.panQuickFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panQuickFilter.Location = new System.Drawing.Point(0, 0);
            this.panQuickFilter.Name = "panQuickFilter";
            this.panQuickFilter.Size = new System.Drawing.Size(650, 31);
            this.panQuickFilter.TabIndex = 6;
            // 
            // txtQuickFilter
            // 
            this.txtQuickFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuickFilter.Location = new System.Drawing.Point(47, 6);
            this.txtQuickFilter.Name = "txtQuickFilter";
            this.txtQuickFilter.Size = new System.Drawing.Size(591, 20);
            this.txtQuickFilter.TabIndex = 1;
            this.txtQuickFilter.TextChanged += new System.EventHandler(this.txtQuickFilter_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filter";
            // 
            // tmFilter
            // 
            this.tmFilter.Interval = 300;
            this.tmFilter.Tick += new System.EventHandler(this.tmFilter_Tick);
            // 
            // txtPagingCookie
            // 
            this.txtPagingCookie.BackColor = System.Drawing.SystemColors.Window;
            this.txtPagingCookie.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPagingCookie.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPagingCookie.Location = new System.Drawing.Point(15, 19);
            this.txtPagingCookie.Name = "txtPagingCookie";
            this.txtPagingCookie.ReadOnly = true;
            this.txtPagingCookie.Size = new System.Drawing.Size(546, 13);
            this.txtPagingCookie.TabIndex = 0;
            this.txtPagingCookie.Click += new System.EventHandler(this.txtPagingCookie_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.linkLabel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(561, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(86, 19);
            this.panel3.TabIndex = 1;
            this.panel3.Visible = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(6, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(74, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Copy to Query";
            // 
            // gbPagingCookie
            // 
            this.gbPagingCookie.Controls.Add(this.txtPagingCookie);
            this.gbPagingCookie.Controls.Add(this.panel3);
            this.gbPagingCookie.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbPagingCookie.Location = new System.Drawing.Point(0, 185);
            this.gbPagingCookie.Name = "gbPagingCookie";
            this.gbPagingCookie.Padding = new System.Windows.Forms.Padding(15, 6, 3, 3);
            this.gbPagingCookie.Size = new System.Drawing.Size(650, 41);
            this.gbPagingCookie.TabIndex = 8;
            this.gbPagingCookie.TabStop = false;
            this.gbPagingCookie.Text = "Paging Cookie";
            this.gbPagingCookie.Visible = false;
            // 
            // crmGridView1
            // 
            this.crmGridView1.AllowUserToAddRows = false;
            this.crmGridView1.AllowUserToDeleteRows = false;
            this.crmGridView1.AllowUserToOrderColumns = true;
            this.crmGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.crmGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.crmGridView1.AutoRefresh = false;
            this.crmGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.crmGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.crmGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.crmGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.crmGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.crmGridView1.ColumnOrder = "";
            this.crmGridView1.ContextMenuStrip = this.ctxmenuGrid;
            this.crmGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crmGridView1.EnableHeadersVisualStyles = false;
            this.crmGridView1.EntityReferenceClickable = true;
            this.crmGridView1.FilterColumns = "";
            this.crmGridView1.LayoutXML = "";
            this.crmGridView1.Location = new System.Drawing.Point(0, 31);
            this.crmGridView1.Name = "crmGridView1";
            this.crmGridView1.ReadOnly = true;
            this.crmGridView1.RowHeadersVisible = false;
            this.crmGridView1.RowHeadersWidth = 24;
            this.crmGridView1.Service = null;
            this.crmGridView1.ShowEditingIcon = false;
            this.crmGridView1.Size = new System.Drawing.Size(650, 154);
            this.crmGridView1.TabIndex = 1;
            this.crmGridView1.RecordDoubleClick += new Rappen.XTB.Helpers.Controls.XRMRecordEventHandler(this.crmGridView1_RecordDoubleClick);
            this.crmGridView1.RecordEnter += new Rappen.XTB.Helpers.Controls.XRMRecordEventHandler(this.crmGridView1_RecordEnter);
            this.crmGridView1.ColumnDisplayIndexChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.crmGridView1_LayoutChanged);
            this.crmGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.crmGridView1_ColumnHeaderMouseClick);
            this.crmGridView1.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.crmGridView1_LayoutChanged);
            // 
            // ResultGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(650, 273);
            this.Controls.Add(this.crmGridView1);
            this.Controls.Add(this.gbPagingCookie);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panQuickFilter);
            this.HideOnClose = true;
            this.MainMenuStrip = this.mnuOptions;
            this.Name = "ResultGrid";
            this.Text = "Result View";
            this.DockStateChanged += new System.EventHandler(this.ResultGrid_DockStateChanged);
            this.ctxmenuGrid.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.mnuOptions.ResumeLayout(false);
            this.mnuOptions.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panQuickFilter.ResumeLayout(false);
            this.panQuickFilter.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.gbPagingCookie.ResumeLayout(false);
            this.gbPagingCookie.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crmGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Rappen.XTB.Helpers.Controls.XRMDataGridView crmGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblOptionsExpander;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.MenuStrip mnuOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuColumns;
        private System.Windows.Forms.ToolStripMenuItem mnuIndexCol;
        private System.Windows.Forms.ToolStripMenuItem mnuIdCol;
        private System.Windows.Forms.ToolStripMenuItem mnuNullCol;
        private System.Windows.Forms.ToolStripMenuItem mnuSysCol;
        private System.Windows.Forms.ToolStripMenuItem mnuBehavior;
        private System.Windows.Forms.ToolStripMenuItem mnuFriendly;
        private System.Windows.Forms.ToolStripMenuItem mnuLocalTime;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyHeaders;
        private System.Windows.Forms.Panel panQuickFilter;
        private System.Windows.Forms.TextBox txtQuickFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem mnuQuickFilter;
        private System.Windows.Forms.Timer tmFilter;
        private System.Windows.Forms.ContextMenuStrip ctxmenuGrid;
        private System.Windows.Forms.ToolStripMenuItem ctxBehavior;
        private System.Windows.Forms.ToolStripMenuItem ctxColumns;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ctxFind;
        private System.Windows.Forms.ToolStripMenuItem ctxRecord;
        private System.Windows.Forms.ToolStripMenuItem ctxRecordOpen;
        private System.Windows.Forms.ToolStripMenuItem ctxColumnOpen;
        private System.Windows.Forms.ToolStripSeparator ctxRecordSeparator;
        private System.Windows.Forms.ToolStripMenuItem ctxColumn;
        private System.Windows.Forms.ToolStripMenuItem ctxColumnCopy;
        private System.Windows.Forms.ToolStripMenuItem ctxRecordCopy;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtPagingCookie;
        private System.Windows.Forms.ToolStripMenuItem mnuPagingCookie;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.GroupBox gbPagingCookie;
        private System.Windows.Forms.ToolStripMenuItem mnuPagePlus;
        private System.Windows.Forms.ToolStripTextBox mnuPage;
        private System.Windows.Forms.ToolStripMenuItem mnuPageMinus;
        private System.Windows.Forms.ToolStripMenuItem mnuBothNames;
        private System.Windows.Forms.ToolStripMenuItem mnuResetLayout;
        private System.Windows.Forms.ToolStripMenuItem mnuShowAllCol;
        private System.Windows.Forms.ToolStripMenuItem mnuAutoSizeAll;
        private System.Windows.Forms.ToolStripMenuItem mnuShowLayoutXML;
        private System.Windows.Forms.ToolStripMenuItem mnuRetrieveTime;
        private System.Windows.Forms.ToolStripMenuItem mnuRecordsNumbers;
        private System.Windows.Forms.ToolStripMenuItem mnuShowElapsed;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuExcel;
    }
}
