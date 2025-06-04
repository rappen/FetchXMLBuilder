using Microsoft.Extensions.AI;
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
        }

        public void SetExecuteResponse(Exception ex)
        {
            chatHistory.Add(ChatRole.User, $"An error occured, please solve it:{Environment.NewLine}{ex.Message}", false);
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

        private void btnAiChatAsk_Click(object sender, EventArgs e)
        {
            SendChatToAI(txtAiChatAsk.Text);
        }

        private async void SendChatToAI(string text)
        {
            // Get the current FetchXml query.
            string currentFetchXml = fxb.dockControlBuilder?.GetFetchString(true, false);
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

            var intro = "You are an agent that helps the user interact with Dataverse using FetchXml queries. The user describes the query he want to do in natural language, and you create a FetchXml query based on the users's description. Your answers are short and to the point. When asked to explain a query, you summarize the meaning of the query in a short text, don't talk about fields and operators. Don't execute the ExecuteFetchXmlRequest tool before asking the user if he wants to execute it. The current FetchXml we are working with is " + currentFetchXml;

            AiCommunication.CallingAI(text, intro, supplier, model, fxb.settings.AiSettings.ApiKey, chatHistory, fxb, ExecuteFetchXmlRequest, SetQueryFromAi);

            txtAiChatAsk.Clear();
        }

        [Description("Executes a FetchXmlRequest")]
        private string ExecuteFetchXmlRequest([Description("The FetchXmlRequest To Execute. This is the current FetchXml, as specified by the system prompt.")] string fetchXml)
        {
            MethodInvoker mi = delegate
            {
                try
                {
                    SetQueryFromAi(fetchXml);
                    fxb.FetchResults(fetchXml, true);
                }
                catch
                {
                    // Now what?
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

            return "Query executed successfully";
        }

        private void SetQueryFromAi(string response)
        {
            txtAiChatAsk.Clear();
            var pattern = @"<fetch\b.*?</fetch>";
            if (Regex.Matches(response, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase) is MatchCollection matches && matches.Count > 0)
            {
                fxb.dockControlBuilder.Init(matches[0].Value, null, false, "Query from AI", true);
            }
        }

        private void picBtnYes_Click(object sender, EventArgs e)
        {
            SendChatToAI("Yes please!");
        }

        private void picBtnNo_Click(object sender, EventArgs e)
        {
            SendChatToAI("Thanks, but no.");
        }

        private void picBtnSettings_Click(object sender, EventArgs e)
        {
            fxb.ShowSettings("tabAiChat");
        }

        private void txtAiChatAsk_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control)
            {
                SendChatToAI(txtAiChatAsk.Text);
            }
            else if (e.KeyCode == Keys.Y && e.Control)
            {
                SendChatToAI("Yes please!");
            }
        }

        private void picBtnCopy_Click(object sender, EventArgs e)
        {
            var chat = chatHistory.ToString();
            Clipboard.SetText(chat);
            fxb.WorkAsync(new WorkAsyncInfo { Message = "Copying!", Work = (w, a) => { Thread.Sleep(500); } });
        }

        private void AiChatControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            chatHistory.Save(Paths.LogsPath, "FXB");
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