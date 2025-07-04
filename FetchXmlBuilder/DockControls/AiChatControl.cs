using Microsoft.Extensions.AI;
using Rappen.AI.WinForm;
using Rappen.XTB.FXB.Settings;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    public partial class AiChatControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder fxb;
        private ChatMessageHistory chatHistory;
        private AiSupplier supplier;
        private AiModel model;
        private string lastquery;

        #region Public Constructor

        public AiChatControl(FetchXmlBuilder fetchXmlBuilder)
        {
            fxb = fetchXmlBuilder;
            InitializeComponent();
            Initialize();
        }

        #endregion Public Constructor

        #region Internal Methods

        internal void Initialize()
        {
            ChatMessageHistory.UserTextColor = OnlineSettings.Instance.Colors.Bright;
            ChatMessageHistory.UserBackgroundColor = OnlineSettings.Instance.Colors.Medium;
            ChatMessageHistory.AssistansTextColor = OnlineSettings.Instance.Colors.Dark;
            ChatMessageHistory.AssistansBackgroundColor = OnlineSettings.Instance.Colors.Bright;
            ChatMessageHistory.WaitingBackColor = Color.FromArgb(240, 240, 240);
            chatHistory?.Save(Paths.LogsPath, "FXB");
            supplier = OnlineSettings.Instance.AiSuppliers.Supplier(fxb.settings.AiSettings.Supplier);
            model = supplier.Model(fxb.settings.AiSettings.Model);
            chatHistory = new ChatMessageHistory(panAiConversation, supplier?.Name, model?.Name, fxb.settings.AiSettings.CallMe);
            SetTitle();
            EnableButtons();
        }

        #endregion Internal Methods

        #region Private Methods

        private void SetTitle()
        {
            Text = $"AI Chat - {fxb.settings.AiSettings.Supplier}";
            TabText = Text;
        }

        private void SendChatToAI(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBoxEx.Show(fxb, "Please enter a question or request.", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (supplier == null)
            {
                if (MessageBoxEx.Show(fxb, "No AI supplier found.\nAdd it in the setting!", "AI Chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    fxb.ShowSettings("tabAiChat");
                }
                return;
            }
            if (model == null)
            {
                if (MessageBoxEx.Show(fxb, "No AI model found", "AI Chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    fxb.ShowSettings("tabAiChat");
                }
                return;
            }
            if (string.IsNullOrWhiteSpace(fxb.settings.AiSettings.ApiKey))
            {
                if (MessageBoxEx.Show(fxb, "No API Key found.\nAdd it in the setting!", "AI Chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    fxb.ShowSettings("tabAiChat");
                }
                return;
            }

            if (!chatHistory.Initialized)
            {
                chatHistory.Initialize(supplier.SystemPrompt.Replace("{fetchxml}", fxb.dockControlBuilder?.GetFetchString(true, false)) + Environment.NewLine + supplier.GetCallMe(fxb.settings.AiSettings.CallMe).Trim());
            }
            else
            {
                var newfetch = fxb.dockControlBuilder?.GetFetchString(true, false);
                if (newfetch != lastquery)
                {
                    chatHistory.Add(ChatRole.User, supplier.UpdatePrompt.Replace("{fetchxml}", newfetch), true);
                }
            }
            chatHistory.IsRunning = true;
            EnableButtons();
            AiCommunication.CallingAI(
                text,
                supplier.Name,
                model.Name,
                fxb.settings.AiSettings.ApiKey,
                chatHistory,
                fxb,
                HandlingResponseFromAi,
                ExecuteFetchXMLQuery,
                UpdateCurrentFetchXmlQuery
                );
            txtAiChat.Clear();
        }

        private void EnableButtons()
        {
            btnAiChatAsk.Enabled = !string.IsNullOrWhiteSpace(txtAiChat.Text);
            btnYes.Enabled = chatHistory.HasDialog;
            btnCopy.Enabled = chatHistory.HasDialog;
            btnSave.Enabled = chatHistory.HasDialog;
            btnReset.Enabled = chatHistory.HasDialog;
            splitAiChat.Panel2.Enabled = !chatHistory.IsRunning;
            txtAiChat.BackColor = chatHistory.IsRunning ? ChatMessageHistory.WaitingBackColor : ChatMessageHistory.BackColor;
            txtAiChat.Enabled = !chatHistory.IsRunning;
        }

        [Description("Executes FetchXML Query")]
        private string ExecuteFetchXMLQuery([Description("The FetchXML Query to be Executed. This is the current FetchXML, as specified in the conversation with the assistant.")] string fetchXml)
        {
            try
            {
                SetQueryFromAi(fetchXml);
                var result = fxb.RetrieveMultipleSync(fetchXml, null, null);
                fxb.HandleRetrieveMultipleResult(result);
                return "Query executed successfully";
            }
            catch (Exception ex)
            {
                return $"Error executing query: {ex.Message}";
            }
        }

        [Description("Updates the current FetchXML Query that we are working on. The assistant should call this tool every time the assistant makes a change to the FetchXml query.")]
        private string UpdateCurrentFetchXmlQuery([Description("A well formed FetchXml query that is the current query that has been updated by the assistant.")] string fetchXml)
        {
            try
            {
                // Informs the assistant that a change has been made to the current FetchXml.
                chatHistory.Add(ChatRole.User, supplier.UpdatePrompt.Replace("{fetchxml}", fetchXml), true);

                // Sets the current query, so that the query is updated in the FXB GUI.
                SetQueryFromAi(fetchXml);

                return "Current query updated successfully";
            }
            catch (Exception ex)
            {
                return $"Error updating current query: {ex.Message}";
            }
        }

        private void HandlingResponseFromAi(ChatResponse response)
        {
            txtAiChat.Clear();
            txtUsage.Text = chatHistory.Responses.UsageToString();
            EnableButtons();
            txtAiChat.Focus();
        }

        private void SetQueryFromAi(string fetch)
        {
            MethodInvoker mi = () =>
            {
                var currentfetch = Regex.Replace(fxb.dockControlBuilder?.GetFetchString(true, false), @"\s+", " ");
                var newfetch = Regex.Replace(fetch, @"\s+", " ");
                if (!currentfetch.Equals(newfetch))
                {
                    fxb.dockControlBuilder.Init(fetch, null, false, "Query from AI", true);
                }
            };
            if (InvokeRequired)
            {
                Invoke(mi);
            }
            else
            {
                mi();
            }
        }

        #endregion Private Methods

        #region Private Event Handlers

        private void AiChatControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            chatHistory.Save(Paths.LogsPath, "FXB");
        }

        private void AiChatControl_DockStateChanged(object sender, EventArgs e)
        {
            if (!IsHidden)
            {
                Height = 400;
            }
            if (DockState != WeifenLuo.WinFormsUI.Docking.DockState.Unknown &&
                DockState != WeifenLuo.WinFormsUI.Docking.DockState.Hidden)
            {
                fxb.settings.DockStates.AiChat = DockState;
            }
        }

        private void txtAiChatAsk_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void txtAiChatAsk_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control && !string.IsNullOrWhiteSpace(txtAiChat.Text))
            {
                e.Handled = true;
                SendChatToAI(txtAiChat.Text);
            }
            else if (e.KeyCode == Keys.Y && e.Control)
            {
                e.Handled = true;
                btnYes_Click();
            }
        }

        private void btnAiChatAsk_Click(object sender, EventArgs e)
        {
            SendChatToAI(txtAiChat.Text);
        }

        private void btnYes_Click(object sender = null, EventArgs e = null)
        {
            SendChatToAI("Yes please!");
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            SendChatToAI("Please execute the FetchXML query!");
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var chat = chatHistory.ToString();
            Clipboard.SetText(chat);
            fxb.WorkAsync(new WorkAsyncInfo { Message = "Copying!", Work = (w, a) => { Thread.Sleep(500); } });
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = "Select a location to save your dialog",
                Filter = "Text file (*.txt)|*.txt",
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                chatHistory.Save(sfd.FileName);
                MessageBoxEx.Show(fxb, $"Chat history saved to {sfd.FileName}", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx.Show(fxb, "Are you sure you want to clear the AI chat history?", "Reset AI Chat", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Initialize();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            fxb.ShowSettings("tabAiChat");
        }

        #endregion Private Event Handlers
    }
}