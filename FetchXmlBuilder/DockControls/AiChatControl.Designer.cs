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
            this.panOData = new System.Windows.Forms.Panel();
            this.splitAiChat = new System.Windows.Forms.SplitContainer();
            this.panAiConversation = new System.Windows.Forms.Panel();
            this.txtAiChatAsk = new System.Windows.Forms.TextBox();
            this.txtUsage = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnAiChatAsk = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panOData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitAiChat)).BeginInit();
            this.splitAiChat.Panel1.SuspendLayout();
            this.splitAiChat.Panel2.SuspendLayout();
            this.splitAiChat.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panOData
            // 
            this.panOData.BackColor = System.Drawing.SystemColors.Window;
            this.panOData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panOData.Controls.Add(this.splitAiChat);
            this.panOData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panOData.Location = new System.Drawing.Point(0, 0);
            this.panOData.Name = "panOData";
            this.panOData.Padding = new System.Windows.Forms.Padding(4);
            this.panOData.Size = new System.Drawing.Size(438, 373);
            this.panOData.TabIndex = 28;
            // 
            // splitAiChat
            // 
            this.splitAiChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitAiChat.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitAiChat.Location = new System.Drawing.Point(4, 4);
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
            this.splitAiChat.Panel2.Controls.Add(this.txtAiChatAsk);
            this.splitAiChat.Panel2.Controls.Add(this.txtUsage);
            this.splitAiChat.Panel2.Controls.Add(this.panel1);
            this.splitAiChat.Panel2MinSize = 40;
            this.splitAiChat.Size = new System.Drawing.Size(428, 363);
            this.splitAiChat.SplitterDistance = 209;
            this.splitAiChat.SplitterWidth = 8;
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
            this.panAiConversation.Name = "panAiConversation";
            this.panAiConversation.Size = new System.Drawing.Size(428, 209);
            this.panAiConversation.TabIndex = 0;
            // 
            // txtAiChatAsk
            // 
            this.txtAiChatAsk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAiChatAsk.Location = new System.Drawing.Point(0, 0);
            this.txtAiChatAsk.Multiline = true;
            this.txtAiChatAsk.Name = "txtAiChatAsk";
            this.txtAiChatAsk.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAiChatAsk.Size = new System.Drawing.Size(376, 133);
            this.txtAiChatAsk.TabIndex = 1;
            this.txtAiChatAsk.TextChanged += new System.EventHandler(this.txtAiChatAsk_TextChanged);
            this.txtAiChatAsk.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAiChatAsk_KeyDown);
            // 
            // txtUsage
            // 
            this.txtUsage.BackColor = System.Drawing.SystemColors.Window;
            this.txtUsage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtUsage.Location = new System.Drawing.Point(0, 133);
            this.txtUsage.Name = "txtUsage";
            this.txtUsage.ReadOnly = true;
            this.txtUsage.Size = new System.Drawing.Size(376, 13);
            this.txtUsage.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.btnExecute);
            this.panel1.Controls.Add(this.btnSettings);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnCopy);
            this.panel1.Controls.Add(this.btnReset);
            this.panel1.Controls.Add(this.btnYes);
            this.panel1.Controls.Add(this.btnAiChatAsk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(376, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(52, 146);
            this.panel1.TabIndex = 0;
            // 
            // btnExecute
            // 
            this.btnExecute.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_play_16;
            this.btnExecute.Location = new System.Drawing.Point(25, 51);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(22, 22);
            this.btnExecute.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnExecute, "Execute the query!");
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_settings_16;
            this.btnSettings.Location = new System.Drawing.Point(25, 95);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(22, 22);
            this.btnSettings.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnSettings, "Open Settings regarding the AI Chat");
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_save2_16;
            this.btnSave.Location = new System.Drawing.Point(25, 73);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(22, 22);
            this.btnSave.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnSave, "Save all communication with the robot");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_paste_16;
            this.btnCopy.Location = new System.Drawing.Point(3, 73);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(22, 22);
            this.btnCopy.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnCopy, "Copy all conversation with the robot");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnReset
            // 
            this.btnReset.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_reset_16;
            this.btnReset.Location = new System.Drawing.Point(3, 95);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(22, 22);
            this.btnReset.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnReset, "Reset all chat dialog");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnYes
            // 
            this.btnYes.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_yes_16;
            this.btnYes.Location = new System.Drawing.Point(3, 51);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(22, 22);
            this.btnYes.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnYes, "Yes please!\r\n<CTRL>+Y");
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnAiChatAsk
            // 
            this.btnAiChatAsk.Enabled = false;
            this.btnAiChatAsk.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_bot_32;
            this.btnAiChatAsk.Location = new System.Drawing.Point(3, 3);
            this.btnAiChatAsk.Name = "btnAiChatAsk";
            this.btnAiChatAsk.Size = new System.Drawing.Size(44, 42);
            this.btnAiChatAsk.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btnAiChatAsk, "<CTRL>+Enter\r\nSend the query to the AI");
            this.btnAiChatAsk.UseVisualStyleBackColor = true;
            this.btnAiChatAsk.Click += new System.EventHandler(this.btnAiChatAsk_Click);
            // 
            // AiChatControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 373);
            this.Controls.Add(this.panOData);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Name = "AiChatControl";
            this.TabText = "AI Chat";
            this.Text = "AI Chat";
            this.DockStateChanged += new System.EventHandler(this.AiChatControl_DockStateChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AiChatControl_FormClosing);
            this.panOData.ResumeLayout(false);
            this.splitAiChat.Panel1.ResumeLayout(false);
            this.splitAiChat.Panel1.PerformLayout();
            this.splitAiChat.Panel2.ResumeLayout(false);
            this.splitAiChat.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitAiChat)).EndInit();
            this.splitAiChat.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panOData;
        private System.Windows.Forms.SplitContainer splitAiChat;
        private System.Windows.Forms.TextBox txtAiChatAsk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAiChatAsk;
        private System.Windows.Forms.Panel panAiConversation;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtUsage;
    }
}
