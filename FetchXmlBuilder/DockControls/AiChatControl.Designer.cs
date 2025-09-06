namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    partial class AiChatControl
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
            this.panAiChat = new System.Windows.Forms.Panel();
            this.splitAiChat = new System.Windows.Forms.SplitContainer();
            this.panAiConversation = new System.Windows.Forms.Panel();
            this.txtAiChat = new System.Windows.Forms.TextBox();
            this.txtUsage = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMenu = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuDocs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFree = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSupporting = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnAiChatAsk = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panAiChat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitAiChat)).BeginInit();
            this.splitAiChat.Panel1.SuspendLayout();
            this.splitAiChat.Panel2.SuspendLayout();
            this.splitAiChat.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panAiChat
            // 
            this.panAiChat.BackColor = System.Drawing.SystemColors.Window;
            this.panAiChat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panAiChat.Controls.Add(this.splitAiChat);
            this.panAiChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panAiChat.Location = new System.Drawing.Point(0, 0);
            this.panAiChat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panAiChat.Name = "panAiChat";
            this.panAiChat.Padding = new System.Windows.Forms.Padding(5);
            this.panAiChat.Size = new System.Drawing.Size(511, 430);
            this.panAiChat.TabIndex = 28;
            // 
            // splitAiChat
            // 
            this.splitAiChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitAiChat.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitAiChat.Location = new System.Drawing.Point(5, 5);
            this.splitAiChat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitAiChat.Name = "splitAiChat";
            this.splitAiChat.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitAiChat.Panel1
            // 
            this.splitAiChat.Panel1.AutoScroll = true;
            this.splitAiChat.Panel1.BackColor = System.Drawing.SystemColors.Info;
            this.splitAiChat.Panel1.Controls.Add(this.panAiConversation);
            // 
            // splitAiChat.Panel2
            // 
            this.splitAiChat.Panel2.AutoScroll = true;
            this.splitAiChat.Panel2.Controls.Add(this.txtAiChat);
            this.splitAiChat.Panel2.Controls.Add(this.txtUsage);
            this.splitAiChat.Panel2.Controls.Add(this.panel1);
            this.splitAiChat.Panel2MinSize = 40;
            this.splitAiChat.Size = new System.Drawing.Size(499, 418);
            this.splitAiChat.SplitterDistance = 304;
            this.splitAiChat.SplitterWidth = 9;
            this.splitAiChat.TabIndex = 13;
            // 
            // panAiConversation
            // 
            this.panAiConversation.AutoScroll = true;
            this.panAiConversation.AutoScrollMinSize = new System.Drawing.Size(16, 0);
            this.panAiConversation.AutoSize = true;
            this.panAiConversation.BackColor = System.Drawing.SystemColors.Window;
            this.panAiConversation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panAiConversation.Location = new System.Drawing.Point(0, 0);
            this.panAiConversation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panAiConversation.Name = "panAiConversation";
            this.panAiConversation.Size = new System.Drawing.Size(499, 304);
            this.panAiConversation.TabIndex = 0;
            // 
            // txtAiChat
            // 
            this.txtAiChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAiChat.Location = new System.Drawing.Point(0, 0);
            this.txtAiChat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtAiChat.Multiline = true;
            this.txtAiChat.Name = "txtAiChat";
            this.txtAiChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAiChat.Size = new System.Drawing.Size(438, 92);
            this.txtAiChat.TabIndex = 1;
            this.txtAiChat.TextChanged += new System.EventHandler(this.txtAiChatAsk_TextChanged);
            this.txtAiChat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAiChatAsk_KeyDown);
            // 
            // txtUsage
            // 
            this.txtUsage.BackColor = System.Drawing.SystemColors.Window;
            this.txtUsage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtUsage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsage.Location = new System.Drawing.Point(0, 92);
            this.txtUsage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtUsage.Name = "txtUsage";
            this.txtUsage.ReadOnly = true;
            this.txtUsage.Size = new System.Drawing.Size(438, 13);
            this.txtUsage.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.btnMenu);
            this.panel1.Controls.Add(this.btnExecute);
            this.panel1.Controls.Add(this.btnReset);
            this.panel1.Controls.Add(this.btnYes);
            this.panel1.Controls.Add(this.btnAiChatAsk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(438, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(61, 105);
            this.panel1.TabIndex = 0;
            // 
            // btnMenu
            // 
            this.btnMenu.ContextMenuStrip = this.contextMenuStrip1;
            this.btnMenu.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_more_14;
            this.btnMenu.Location = new System.Drawing.Point(29, 75);
            this.btnMenu.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(26, 25);
            this.btnMenu.TabIndex = 7;
            this.toolTip1.SetToolTip(this.btnMenu, "Open menu to get more options\r\nfor the AI Chat, including\r\ndocumentation!");
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCopy,
            this.mnuSave,
            this.toolStripMenuItem1,
            this.mnuSettings,
            this.toolStripMenuItem2,
            this.mnuDocs,
            this.mnuFree,
            this.toolStripMenuItem3,
            this.mnuSupporting});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(189, 154);
            // 
            // mnuCopy
            // 
            this.mnuCopy.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_paste_16;
            this.mnuCopy.Name = "mnuCopy";
            this.mnuCopy.Size = new System.Drawing.Size(188, 22);
            this.mnuCopy.Text = "Copy dialog";
            this.mnuCopy.Click += new System.EventHandler(this.mnuCopy_Click);
            // 
            // mnuSave
            // 
            this.mnuSave.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_save2_16;
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.Size = new System.Drawing.Size(188, 22);
            this.mnuSave.Text = "Save dialog...";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(185, 6);
            // 
            // mnuSettings
            // 
            this.mnuSettings.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_settings_16;
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(188, 22);
            this.mnuSettings.Text = "Settings...";
            this.mnuSettings.Click += new System.EventHandler(this.mnuSettings_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(185, 6);
            // 
            // mnuDocs
            // 
            this.mnuDocs.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_info_16;
            this.mnuDocs.Name = "mnuDocs";
            this.mnuDocs.Size = new System.Drawing.Size(188, 22);
            this.mnuDocs.Text = "Documentation...";
            this.mnuDocs.Click += new System.EventHandler(this.mnuDocs_Click);
            // 
            // mnuFree
            // 
            this.mnuFree.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_heart_16;
            this.mnuFree.Name = "mnuFree";
            this.mnuFree.Size = new System.Drawing.Size(188, 22);
            this.mnuFree.Text = "Ask for Free AI...";
            this.mnuFree.Click += new System.EventHandler(this.mnuFree_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(185, 6);
            // 
            // mnuSupporting
            // 
            this.mnuSupporting.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.Supporting_bg_16;
            this.mnuSupporting.Name = "mnuSupporting";
            this.mnuSupporting.Size = new System.Drawing.Size(188, 22);
            this.mnuSupporting.Text = "Supporting this tool...";
            this.mnuSupporting.Click += new System.EventHandler(this.mnuSupporting_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_play_16;
            this.btnExecute.Location = new System.Drawing.Point(29, 50);
            this.btnExecute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(26, 25);
            this.btnExecute.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnExecute, "Execute the query!");
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.SendChatToAI);
            // 
            // btnReset
            // 
            this.btnReset.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_reset_16;
            this.btnReset.Location = new System.Drawing.Point(4, 75);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(26, 25);
            this.btnReset.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnReset, "Reset all chat dialog");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnYes
            // 
            this.btnYes.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_yes_16;
            this.btnYes.Location = new System.Drawing.Point(4, 50);
            this.btnYes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(26, 25);
            this.btnYes.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnYes, "Yes please!\r\n<CTRL>+Y");
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.SendChatToAI);
            // 
            // btnAiChatAsk
            // 
            this.btnAiChatAsk.Enabled = false;
            this.btnAiChatAsk.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_bot_32;
            this.btnAiChatAsk.Location = new System.Drawing.Point(4, 0);
            this.btnAiChatAsk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAiChatAsk.Name = "btnAiChatAsk";
            this.btnAiChatAsk.Size = new System.Drawing.Size(51, 48);
            this.btnAiChatAsk.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btnAiChatAsk, "<CTRL>+Enter\r\nSend the query to the AI");
            this.btnAiChatAsk.UseVisualStyleBackColor = true;
            this.btnAiChatAsk.Click += new System.EventHandler(this.SendChatToAI);
            // 
            // AiChatControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 430);
            this.Controls.Add(this.panAiChat);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "AiChatControl";
            this.TabText = "AI Chat";
            this.Text = "AI Chat";
            this.DockStateChanged += new System.EventHandler(this.AiChatControl_DockStateChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AiChatControl_FormClosing);
            this.panAiChat.ResumeLayout(false);
            this.splitAiChat.Panel1.ResumeLayout(false);
            this.splitAiChat.Panel1.PerformLayout();
            this.splitAiChat.Panel2.ResumeLayout(false);
            this.splitAiChat.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitAiChat)).EndInit();
            this.splitAiChat.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panAiChat;
        private System.Windows.Forms.SplitContainer splitAiChat;
        private System.Windows.Forms.TextBox txtAiChat;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAiChatAsk;
        private System.Windows.Forms.Panel panAiConversation;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtUsage;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuSupporting;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mnuDocs;
        private System.Windows.Forms.ToolStripMenuItem mnuFree;
    }
}
