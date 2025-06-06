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
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.picBtnSave = new System.Windows.Forms.PictureBox();
            this.picBtnReset = new System.Windows.Forms.PictureBox();
            this.picBtnCopy = new System.Windows.Forms.PictureBox();
            this.picBtnSettings = new System.Windows.Forms.PictureBox();
            this.picBtnYes = new System.Windows.Forms.PictureBox();
            this.btnAiChatAsk = new System.Windows.Forms.Button();
            this.panOData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitAiChat)).BeginInit();
            this.splitAiChat.Panel1.SuspendLayout();
            this.splitAiChat.Panel2.SuspendLayout();
            this.splitAiChat.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBtnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBtnReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBtnCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBtnSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBtnYes)).BeginInit();
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
            this.panOData.Size = new System.Drawing.Size(507, 480);
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
            this.splitAiChat.Panel2.Controls.Add(this.panel1);
            this.splitAiChat.Panel2MinSize = 40;
            this.splitAiChat.Size = new System.Drawing.Size(497, 470);
            this.splitAiChat.SplitterDistance = 335;
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
            this.panAiConversation.Size = new System.Drawing.Size(497, 335);
            this.panAiConversation.TabIndex = 0;
            // 
            // txtAiChatAsk
            // 
            this.txtAiChatAsk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAiChatAsk.Location = new System.Drawing.Point(0, 0);
            this.txtAiChatAsk.Multiline = true;
            this.txtAiChatAsk.Name = "txtAiChatAsk";
            this.txtAiChatAsk.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAiChatAsk.Size = new System.Drawing.Size(451, 127);
            this.txtAiChatAsk.TabIndex = 1;
            this.txtAiChatAsk.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAiChatAsk_KeyDown);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.picBtnSave);
            this.panel1.Controls.Add(this.picBtnReset);
            this.panel1.Controls.Add(this.picBtnCopy);
            this.panel1.Controls.Add(this.picBtnSettings);
            this.panel1.Controls.Add(this.picBtnYes);
            this.panel1.Controls.Add(this.btnAiChatAsk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(451, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(46, 127);
            this.panel1.TabIndex = 0;
            // 
            // picBtnSave
            // 
            this.picBtnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picBtnSave.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_save;
            this.picBtnSave.Location = new System.Drawing.Point(25, 71);
            this.picBtnSave.Name = "picBtnSave";
            this.picBtnSave.Size = new System.Drawing.Size(16, 16);
            this.picBtnSave.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBtnSave.TabIndex = 6;
            this.picBtnSave.TabStop = false;
            this.toolTip1.SetToolTip(this.picBtnSave, "Save all our chat dialog to a text file!");
            this.picBtnSave.Click += new System.EventHandler(this.picBtnSave_Click);
            // 
            // picBtnReset
            // 
            this.picBtnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picBtnReset.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_reset_16;
            this.picBtnReset.Location = new System.Drawing.Point(25, 51);
            this.picBtnReset.Name = "picBtnReset";
            this.picBtnReset.Size = new System.Drawing.Size(16, 16);
            this.picBtnReset.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBtnReset.TabIndex = 5;
            this.picBtnReset.TabStop = false;
            this.toolTip1.SetToolTip(this.picBtnReset, "Reset our chat, start from Hello again!");
            this.picBtnReset.Click += new System.EventHandler(this.picBtnReset_Click);
            // 
            // picBtnCopy
            // 
            this.picBtnCopy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picBtnCopy.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_paste_16;
            this.picBtnCopy.Location = new System.Drawing.Point(6, 71);
            this.picBtnCopy.Name = "picBtnCopy";
            this.picBtnCopy.Size = new System.Drawing.Size(16, 16);
            this.picBtnCopy.TabIndex = 4;
            this.picBtnCopy.TabStop = false;
            this.toolTip1.SetToolTip(this.picBtnCopy, "Copy all our chat dialog!");
            this.picBtnCopy.Click += new System.EventHandler(this.picBtnCopy_Click);
            // 
            // picBtnSettings
            // 
            this.picBtnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picBtnSettings.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_settings_16;
            this.picBtnSettings.Location = new System.Drawing.Point(25, 90);
            this.picBtnSettings.Name = "picBtnSettings";
            this.picBtnSettings.Size = new System.Drawing.Size(16, 16);
            this.picBtnSettings.TabIndex = 3;
            this.picBtnSettings.TabStop = false;
            this.toolTip1.SetToolTip(this.picBtnSettings, "Open Settings about AI Chat.");
            this.picBtnSettings.Click += new System.EventHandler(this.picBtnSettings_Click);
            // 
            // picBtnYes
            // 
            this.picBtnYes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picBtnYes.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_execute;
            this.picBtnYes.Location = new System.Drawing.Point(6, 51);
            this.picBtnYes.Name = "picBtnYes";
            this.picBtnYes.Size = new System.Drawing.Size(16, 16);
            this.picBtnYes.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBtnYes.TabIndex = 1;
            this.picBtnYes.TabStop = false;
            this.toolTip1.SetToolTip(this.picBtnYes, "Please execute! <CTRL>+Y");
            this.picBtnYes.Click += new System.EventHandler(this.picBtnYes_Click);
            // 
            // btnAiChatAsk
            // 
            this.btnAiChatAsk.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_bot_32;
            this.btnAiChatAsk.Location = new System.Drawing.Point(3, 3);
            this.btnAiChatAsk.Name = "btnAiChatAsk";
            this.btnAiChatAsk.Size = new System.Drawing.Size(40, 42);
            this.btnAiChatAsk.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btnAiChatAsk, "<CTRL>+Enter\r\nSend the query to the AI");
            this.btnAiChatAsk.UseVisualStyleBackColor = true;
            this.btnAiChatAsk.Click += new System.EventHandler(this.btnAiChatAsk_Click);
            // 
            // AiChatControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 480);
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
            ((System.ComponentModel.ISupportInitialize)(this.picBtnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBtnReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBtnCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBtnSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBtnYes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panOData;
        private System.Windows.Forms.SplitContainer splitAiChat;
        private System.Windows.Forms.TextBox txtAiChatAsk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAiChatAsk;
        private System.Windows.Forms.Panel panAiConversation;
        private System.Windows.Forms.PictureBox picBtnYes;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox picBtnSettings;
        private System.Windows.Forms.PictureBox picBtnCopy;
        private System.Windows.Forms.PictureBox picBtnReset;
        private System.Windows.Forms.PictureBox picBtnSave;
    }
}
