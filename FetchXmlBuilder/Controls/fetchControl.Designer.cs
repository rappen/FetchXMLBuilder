namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    partial class fetchControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fetchControl));
            this.label4 = new System.Windows.Forms.Label();
            this.textPageSize = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.textTop = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.textPage = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textPagingCookie = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.dataSourceComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOptions = new System.Windows.Forms.TextBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbOptionsAdd = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Count";
            this.toolTip1.SetToolTip(this.label4, "Max number of records to return per page.\r\nhttps://learn.microsoft.com/power-apps" +
        "/developer/data-platform/fetchxml/page-results\r\n");
            // 
            // textPageSize
            // 
            this.textPageSize.Location = new System.Drawing.Point(6, 30);
            this.textPageSize.Name = "textPageSize";
            this.textPageSize.Size = new System.Drawing.Size(106, 20);
            this.textPageSize.TabIndex = 7;
            this.textPageSize.Tag = "count";
            this.toolTip1.SetToolTip(this.textPageSize, "Max number of records to return per page.\r\nhttps://learn.microsoft.com/power-apps" +
        "/developer/data-platform/fetchxml/page-results\r\n");
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(8, 64);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(75, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Tag = "aggregate";
            this.checkBox1.Text = "Aggregate";
            this.toolTip1.SetToolTip(this.checkBox1, "Aggregate will be a different type of query. Read more!\r\nhttps://learn.microsoft." +
        "com/power-apps/developer/data-platform/fetchxml/aggregate-data\r\n");
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(8, 42);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(61, 17);
            this.checkBox2.TabIndex = 2;
            this.checkBox2.Tag = "distinct";
            this.checkBox2.Text = "Distinct";
            this.toolTip1.SetToolTip(this.checkBox2, "Choose if only one record will be returned, if they are identical.\r\nhttps://learn" +
        ".microsoft.com/power-apps/developer/data-platform/fetchxml/overview#return-disti" +
        "nct-results\r\n");
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // textTop
            // 
            this.textTop.Location = new System.Drawing.Point(7, 16);
            this.textTop.Name = "textTop";
            this.textTop.Size = new System.Drawing.Size(106, 20);
            this.textTop.TabIndex = 1;
            this.textTop.Tag = "top";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 2);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "Top";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(125, 42);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(113, 17);
            this.checkBox4.TabIndex = 4;
            this.checkBox4.Tag = "returntotalrecordcount";
            this.checkBox4.Text = "Total record count";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // textPage
            // 
            this.textPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textPage.Location = new System.Drawing.Point(118, 30);
            this.textPage.Name = "textPage";
            this.textPage.Size = new System.Drawing.Size(182, 20);
            this.textPage.TabIndex = 8;
            this.textPage.Tag = "page";
            this.toolTip1.SetToolTip(this.textPage, "Shows which page to return.\r\nhttps://learn.microsoft.com/power-apps/developer/dat" +
        "a-platform/fetchxml/page-results");
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(115, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Page";
            this.toolTip1.SetToolTip(this.label10, "Shows which page to return.\r\nhttps://learn.microsoft.com/power-apps/developer/dat" +
        "a-platform/fetchxml/page-results\r\n");
            // 
            // textPagingCookie
            // 
            this.textPagingCookie.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textPagingCookie.Location = new System.Drawing.Point(6, 69);
            this.textPagingCookie.Name = "textPagingCookie";
            this.textPagingCookie.Size = new System.Drawing.Size(294, 20);
            this.textPagingCookie.TabIndex = 9;
            this.textPagingCookie.Tag = "paging-cookie";
            this.toolTip1.SetToolTip(this.textPagingCookie, "Paging Cookie can detect which records to return.\r\nhttps://learn.microsoft.com/po" +
        "wer-apps/developer/data-platform/fetchxml/page-results#paging-cookies\r\n");
            this.textPagingCookie.Leave += new System.EventHandler(this.textPagingCookie_Leave);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 55);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 13);
            this.label11.TabIndex = 37;
            this.label11.Text = "Paging Cookie";
            this.toolTip1.SetToolTip(this.label11, "Paging Cookie can detect which records to return.\r\nhttps://learn.microsoft.com/po" +
        "wer-apps/developer/data-platform/fetchxml/page-results#paging-cookies\r\n");
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(6, 19);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(97, 17);
            this.checkBox5.TabIndex = 6;
            this.checkBox5.Tag = "latematerialize";
            this.checkBox5.Text = "LateMaterialize";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(99, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(14, 14);
            this.pictureBox1.TabIndex = 38;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/optimize-" +
    "performance#late-materialize-query";
            this.toolTip1.SetToolTip(this.pictureBox1, "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/optimize-" +
        "performance#late-materialize-query");
            this.pictureBox1.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(77, 66);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(14, 14);
            this.pictureBox2.TabIndex = 39;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/aggregate" +
    "-data";
            this.toolTip1.SetToolTip(this.pictureBox2, "Aggregate will be a different type of query. Read more!\r\nhttps://learn.microsoft." +
        "com/power-apps/developer/data-platform/fetchxml/aggregate-data");
            this.pictureBox2.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(78, 55);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(14, 14);
            this.pictureBox3.TabIndex = 40;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Tag = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/fetchxml/pag" +
    "e-results#paging-cookies";
            this.toolTip1.SetToolTip(this.pictureBox3, "Paging Cookie can detect which records to return.\r\nhttps://learn.microsoft.com/po" +
        "wer-apps/developer/data-platform/fetchxml/page-results#paging-cookies");
            this.pictureBox3.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(68, 17);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(14, 14);
            this.pictureBox4.TabIndex = 42;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Tag = "https://learn.microsoft.com/power-apps/maker/data-platform/data-retention-overvie" +
    "w";
            this.toolTip1.SetToolTip(this.pictureBox4, "Read more about Dataverse Long Term Data Retention!\r\nhttps://learn.microsoft.com/" +
        "en-us/power-apps/maker/data-platform/data-retention-overview");
            this.pictureBox4.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // dataSourceComboBox
            // 
            this.dataSourceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataSourceComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.dataSourceComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.dataSourceComboBox.FormattingEnabled = true;
            this.dataSourceComboBox.Location = new System.Drawing.Point(6, 32);
            this.dataSourceComboBox.Name = "dataSourceComboBox";
            this.dataSourceComboBox.Size = new System.Drawing.Size(294, 21);
            this.dataSourceComboBox.TabIndex = 10;
            this.dataSourceComboBox.Tag = "datasource";
            this.toolTip1.SetToolTip(this.dataSourceComboBox, "Read more about Dataverse Long Term Data Retention!\r\nhttps://learn.microsoft.com/" +
        "en-us/power-apps/maker/data-platform/data-retention-overview\r\n");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "Data source";
            this.toolTip1.SetToolTip(this.label1, "Read more about Dataverse Long Term Data Retention!\r\nhttps://learn.microsoft.com/" +
        "en-us/power-apps/maker/data-platform/data-retention-overview\r\n");
            // 
            // pictureBox5
            // 
            this.pictureBox5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(38, 16);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(14, 14);
            this.pictureBox5.TabIndex = 45;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.Tag = "https://learn.microsoft.com/en-us/power-apps/developer/data-platform/fetchxml/pag" +
    "e-results?tabs=sdk#paging-models";
            this.toolTip1.SetToolTip(this.pictureBox5, "Max number of records to return per page.\r\nhttps://learn.microsoft.com/power-apps" +
        "/developer/data-platform/fetchxml/page-results");
            this.pictureBox5.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox6
            // 
            this.pictureBox6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
            this.pictureBox6.Location = new System.Drawing.Point(146, 16);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(14, 14);
            this.pictureBox6.TabIndex = 46;
            this.pictureBox6.TabStop = false;
            this.pictureBox6.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/page-resu" +
    "lts";
            this.toolTip1.SetToolTip(this.pictureBox6, "Shows which page to return.\r\nhttps://learn.microsoft.com/power-apps/developer/dat" +
        "a-platform/fetchxml/page-results\r\n");
            this.pictureBox6.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox7
            // 
            this.pictureBox7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.Location = new System.Drawing.Point(64, 44);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(14, 14);
            this.pictureBox7.TabIndex = 47;
            this.pictureBox7.TabStop = false;
            this.pictureBox7.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/overview#" +
    "return-distinct-results";
            this.toolTip1.SetToolTip(this.pictureBox7, "Choose if only one record will be returned, if they are identical.\r\nhttps://learn" +
        ".microsoft.com/power-apps/developer/data-platform/fetchxml/overview#return-disti" +
        "nct-results");
            this.pictureBox7.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox8
            // 
            this.pictureBox8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox8.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox8.Image")));
            this.pictureBox8.Location = new System.Drawing.Point(233, 44);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(14, 14);
            this.pictureBox8.TabIndex = 48;
            this.pictureBox8.TabStop = false;
            this.pictureBox8.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/count-row" +
    "s";
            this.toolTip1.SetToolTip(this.pictureBox8, "Counting rows.\r\nhttps://learn.microsoft.com/power-apps/developer/data-platform/fe" +
        "tchxml/count-rows");
            this.pictureBox8.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox9
            // 
            this.pictureBox9.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox9.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox9.Image")));
            this.pictureBox9.Location = new System.Drawing.Point(31, 2);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(14, 14);
            this.pictureBox9.TabIndex = 49;
            this.pictureBox9.TabStop = false;
            this.pictureBox9.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/overview#" +
    "limit-the-number-of-rows";
            this.toolTip1.SetToolTip(this.pictureBox9, "Max number of record to return.\r\nhttps://learn.microsoft.com/power-apps/developer" +
        "/data-platform/fetchxml/overview#limit-the-number-of-rows");
            this.pictureBox9.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // pictureBox10
            // 
            this.pictureBox10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox10.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox10.Image")));
            this.pictureBox10.Location = new System.Drawing.Point(240, 67);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(14, 14);
            this.pictureBox10.TabIndex = 51;
            this.pictureBox10.TabStop = false;
            this.pictureBox10.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/reference" +
    "/fetch#attributes";
            this.toolTip1.SetToolTip(this.pictureBox10, "Check this to sort by values of Choices, instead of by Display name.\r\nhttps://lea" +
        "rn.microsoft.com/power-apps/developer/data-platform/fetchxml/reference/fetch#att" +
        "ributes");
            this.pictureBox10.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "Options";
            this.toolTip1.SetToolTip(this.label2, "Options are pretty advance... Know what you do!\r\nhttps://learn.microsoft.com/powe" +
        "r-apps/developer/data-platform/fetchxml/reference/fetch#options\r\n");
            // 
            // txtOptions
            // 
            this.txtOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOptions.Location = new System.Drawing.Point(6, 53);
            this.txtOptions.Name = "txtOptions";
            this.txtOptions.Size = new System.Drawing.Size(294, 20);
            this.txtOptions.TabIndex = 41;
            this.txtOptions.Tag = "options";
            this.toolTip1.SetToolTip(this.txtOptions, "Options are pretty advance... Know what you do, please!\r\nhttps://learn.microsoft." +
        "com/power-apps/developer/data-platform/fetchxml/reference/fetch#options\r\n");
            // 
            // pictureBox11
            // 
            this.pictureBox11.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox11.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox11.Image")));
            this.pictureBox11.Location = new System.Drawing.Point(46, 39);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(14, 14);
            this.pictureBox11.TabIndex = 43;
            this.pictureBox11.TabStop = false;
            this.pictureBox11.Tag = "https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/reference" +
    "/fetch#options";
            this.toolTip1.SetToolTip(this.pictureBox11, "Options are pretty advance... Know what you do!\r\nhttps://learn.microsoft.com/powe" +
        "r-apps/developer/data-platform/fetchxml/reference/fetch#options");
            this.pictureBox11.Click += new System.EventHandler(this.helpIcon_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(125, 65);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(120, 17);
            this.checkBox3.TabIndex = 50;
            this.checkBox3.Tag = "useraworderby";
            this.checkBox3.Text = "Order by Raw value";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cmbOptionsAdd);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtOptions);
            this.groupBox1.Controls.Add(this.pictureBox11);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.checkBox5);
            this.groupBox1.Location = new System.Drawing.Point(7, 189);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 105);
            this.groupBox1.TabIndex = 52;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Optimize performance";
            // 
            // cmbOptionsAdd
            // 
            this.cmbOptionsAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbOptionsAdd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOptionsAdd.FormattingEnabled = true;
            this.cmbOptionsAdd.Items.AddRange(new object[] {
            "",
            "OptimizeForUnknown",
            "ForceOrder",
            "DisableRowGoal",
            "EnableOptimizerHotfixes",
            "LoopJoin",
            "MergeJoin",
            "HashJoin",
            "NO_PERFORMANCE_SPOOL",
            "ENABLE_HIST_AMENDMENT_FOR_ASC_KEYS"});
            this.cmbOptionsAdd.Location = new System.Drawing.Point(68, 75);
            this.cmbOptionsAdd.Name = "cmbOptionsAdd";
            this.cmbOptionsAdd.Size = new System.Drawing.Size(232, 21);
            this.cmbOptionsAdd.TabIndex = 45;
            this.cmbOptionsAdd.SelectedIndexChanged += new System.EventHandler(this.cmbOptionsAdd_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Add Option";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textPageSize);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.textPage);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.textPagingCookie);
            this.groupBox2.Controls.Add(this.pictureBox3);
            this.groupBox2.Controls.Add(this.pictureBox6);
            this.groupBox2.Controls.Add(this.pictureBox5);
            this.groupBox2.Location = new System.Drawing.Point(7, 87);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(306, 96);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Paging";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.pictureBox4);
            this.groupBox3.Controls.Add(this.dataSourceComboBox);
            this.groupBox3.Location = new System.Drawing.Point(7, 300);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(306, 64);
            this.groupBox3.TabIndex = 54;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dataverse long term data retention";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.pictureBox8);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox7);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.checkBox2);
            this.panel1.Controls.Add(this.textTop);
            this.panel1.Controls.Add(this.pictureBox10);
            this.panel1.Controls.Add(this.checkBox4);
            this.panel1.Controls.Add(this.checkBox3);
            this.panel1.Controls.Add(this.pictureBox9);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(316, 84);
            this.panel1.TabIndex = 1;
            // 
            // fetchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "fetchControl";
            this.Size = new System.Drawing.Size(316, 410);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textPageSize;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.TextBox textTop;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.TextBox textPage;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textPagingCookie;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.ComboBox dataSourceComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOptions;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbOptionsAdd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
    }
}
