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
            this.panOData = new System.Windows.Forms.Panel();
            this.splitAiChat = new System.Windows.Forms.SplitContainer();
            this.panAiConversation = new System.Windows.Forms.Panel();
            this.txtAiChatAsk = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAiChatAsk = new System.Windows.Forms.Button();
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
            this.splitAiChat.Panel1.BackColor = System.Drawing.SystemColors.Info;
            this.splitAiChat.Panel1.Controls.Add(this.panAiConversation);
            // 
            // splitAiChat.Panel2
            // 
            this.splitAiChat.Panel2.Controls.Add(this.txtAiChatAsk);
            this.splitAiChat.Panel2.Controls.Add(this.panel1);
            this.splitAiChat.Size = new System.Drawing.Size(497, 470);
            this.splitAiChat.SplitterDistance = 324;
            this.splitAiChat.SplitterWidth = 8;
            this.splitAiChat.TabIndex = 13;
            // 
            // panAiConversation
            // 
            this.panAiConversation.AutoScroll = true;
            this.panAiConversation.BackColor = System.Drawing.SystemColors.Window;
            this.panAiConversation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panAiConversation.Location = new System.Drawing.Point(0, 0);
            this.panAiConversation.Name = "panAiConversation";
            this.panAiConversation.Size = new System.Drawing.Size(497, 324);
            this.panAiConversation.TabIndex = 0;
            // 
            // txtAiChatAsk
            // 
            this.txtAiChatAsk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAiChatAsk.Location = new System.Drawing.Point(0, 0);
            this.txtAiChatAsk.Multiline = true;
            this.txtAiChatAsk.Name = "txtAiChatAsk";
            this.txtAiChatAsk.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAiChatAsk.Size = new System.Drawing.Size(451, 138);
            this.txtAiChatAsk.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnAiChatAsk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(451, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(46, 138);
            this.panel1.TabIndex = 0;
            // 
            // btnAiChatAsk
            // 
            this.btnAiChatAsk.Image = global::Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_execute;
            this.btnAiChatAsk.Location = new System.Drawing.Point(3, 3);
            this.btnAiChatAsk.Name = "btnAiChatAsk";
            this.btnAiChatAsk.Size = new System.Drawing.Size(40, 42);
            this.btnAiChatAsk.TabIndex = 0;
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
            this.panOData.ResumeLayout(false);
            this.splitAiChat.Panel1.ResumeLayout(false);
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
    }
}
