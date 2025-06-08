using Newtonsoft.Json;
using Rappen.AI.WinForm;
using Rappen.XTB.FXB.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public AiChatControl(FetchXmlBuilder fetchXmlBuilder)
        {
            fxb = fetchXmlBuilder;
            InitializeComponent();
            TabText = Text;
            ChatMessageHistory.UserTextColor = OnlineSettings.Instance.Colors.Bright;
            ChatMessageHistory.UserBackgroundColor = OnlineSettings.Instance.Colors.Medium;
            ChatMessageHistory.AssistansTextColor = OnlineSettings.Instance.Colors.Dark;
            ChatMessageHistory.AssistansBackgroundColor = OnlineSettings.Instance.Colors.Bright;
            chatHistory = new ChatMessageHistory(panAiConversation);
            EnableButtons();
        }

        internal void Reset()
        {
            chatHistory.Save(Paths.LogsPath, "FXB");
            chatHistory.Restart();
            EnableButtons();
        }

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

        private void SendChatToAI(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBoxEx.Show(this, "Please enter a question or request.", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var supplier = OnlineSettings.Instance.AiSupported.Supplier(fxb.settings.AiSettings.Supplier);
            if (supplier == null)
            {
                MessageBoxEx.Show(this, "No AI supplier found", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var model = supplier.Model(fxb.settings.AiSettings.Model);
            if (model == null)
            {
                MessageBoxEx.Show(this, "No AI model found", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var currentFetchXml = fxb.dockControlBuilder?.GetFetchString(true, false);
            var intro = "You are an agent that helps the user interact with Dataverse using FetchXML queries. The user describes the query he want to do in natural language, and you create a FetchXML query based on the users's description. Your answers are short and to the point. When asked to explain a query, you summarize the meaning of the query in a short text, don't talk about fields and operators. Don't execute the Executes FetchXML Query tool before asking the user if he wants to execute it. When the Executes FetchXML Query tool is executed, the result will be show in the UI if there were no errors. The current FetchXML we are working with is:\n" + currentFetchXml;

            AiCommunication.CallingAI(text, intro, supplier, model, fxb.settings.AiSettings.ApiKey, chatHistory, fxb, ExecuteFetchXMLQuery, SetQueryFromAi);

            txtAiChatAsk.Clear();
            EnableButtons();
        }

        private void EnableButtons()
        {
            btnAiChatAsk.Enabled = !string.IsNullOrWhiteSpace(txtAiChatAsk.Text);
            btnYes.Enabled = chatHistory.Messages.Count > 0;
            btnCopy.Enabled = chatHistory.Messages.Count > 0;
            btnSave.Enabled = chatHistory.Messages.Count > 0;
            btnReset.Enabled = chatHistory.Messages.Count > 0;
        }

        [Description("Executes FetchXML Query")]
        private string ExecuteFetchXMLQuery([Description("The FetchXML Query to be Executed. This is the current FetchXML, as specified by the system prompt.")] string fetchXml)
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

        private void SetQueryFromAi(string response)
        {
            MethodInvoker mi = () =>
            {
                txtAiChatAsk.Clear();
                var pattern = @"<fetch\b.*?</fetch>";
                if (Regex.Matches(response, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase) is MatchCollection matches && matches.Count > 0)
                {
                    fxb.dockControlBuilder.Init(matches[0].Value, null, false, "Query from AI", true);
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

        private void txtAiChatAsk_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void txtAiChatAsk_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control && !string.IsNullOrWhiteSpace(txtAiChatAsk.Text))
            {
                e.Handled = true;
                SendChatToAI(txtAiChatAsk.Text);
            }
            else if (e.KeyCode == Keys.Y && e.Control)
            {
                e.Handled = true;
                btnYes_Click();
            }
        }

        private void btnAiChatAsk_Click(object sender, EventArgs e)
        {
            SendChatToAI(txtAiChatAsk.Text);
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
                MessageBoxEx.Show(this, $"Chat history saved to {sfd.FileName}", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx.Show(this, "Are you sure you want to clear the AI chat history?\nIt won't know your name anymore...", "Reset AI Chat", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                chatHistory.Save(Paths.LogsPath, "FXB");
                chatHistory.Restart();
                EnableButtons();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            fxb.ShowSettings("tabAiChat");
        }
    }

    public class InputSchema
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, SchemaProperty> Properties { get; set; }

        [JsonProperty("required")]
        public List<string> Required { get; set; }

        public InputSchema()
        {
            Properties = new Dictionary<string, SchemaProperty>();
            Required = new List<string>();
        }
    }

    public class SchemaProperty
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}