using Microsoft.Extensions.AI;
using Rappen.AI.WinForm;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FXB.AppCode;
using Rappen.XTB.FXB.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
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
        private Stopwatch callingstopwatch;
        private Dictionary<string, List<SimpleAiMeta>> metaAttributes = new Dictionary<string, List<SimpleAiMeta>>();
        private string logname = "AI";

        #region Public Constructor

        public AiChatControl(FetchXmlBuilder fetchXmlBuilder)
        {
            fxb = fetchXmlBuilder;
            InitializeComponent();
            Initialize();
            EnableButtons();
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
            supplier = OnlineSettings.Instance.AiSupport.Supplier(fxb.settings.AiSettings.Supplier);

            if (supplier == null)
            {
                MessageBoxEx.Show(fxb, "The AI supplier is not available (yet).\nGo check the setting!", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fxb.ShowSettings("tabAiChat");
                return;
            }
            logname = $"AI-{supplier.Name}";
            model = supplier.Model(fxb.settings.AiSettings.Model);
            if (model == null)
            {
                MessageBoxEx.Show(fxb, "The AI model is not available (yet).\nGo check the setting!", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fxb.ShowSettings("tabAiChat");
                return;
            }
            chatHistory = new ChatMessageHistory(panAiConversation, supplier?.Name, model?.Name, fxb.settings.AiSettings.MyName);
            metaAttributes.Clear();
            SetTitle();
            EnableButtons();
        }

        #endregion Internal Methods

        #region Private Methods

        private string PromptSystem => model?.Prompts?.System ?? supplier?.Prompts?.System ?? OnlineSettings.Instance.AiSupport.Prompts.System;
        private string PromptMyName => model?.Prompts?.CallMe ?? supplier?.Prompts?.CallMe ?? OnlineSettings.Instance.AiSupport.Prompts.CallMe;
        private string PromptUpdate => model?.Prompts?.Update ?? supplier?.Prompts?.Update ?? OnlineSettings.Instance.AiSupport.Prompts.Update;
        private string PromptEntityMeta => model?.Prompts?.EntityMeta ?? supplier?.Prompts?.EntityMeta ?? OnlineSettings.Instance.AiSupport.Prompts.EntityMeta;
        private string PromptAttributeMeta => model?.Prompts?.AttributeMeta ?? supplier?.Prompts?.AttributeMeta ?? OnlineSettings.Instance.AiSupport.Prompts.AttributeMeta;

        private void SetTitle()
        {
            Text = $"AI Chat - {supplier}";
            TabText = Text;
        }

        private void SendChatToAI(object sender, EventArgs e = null)
        {
            var text = string.Empty;
            var action = "Ask";
            switch (sender)
            {
                case Button btn when btn == btnAiChatAsk:
                    text = txtAiChat.Text;
                    break;

                case Button btn when btn == btnYes:
                    text = "Yes please!";
                    action = "Yes!";
                    break;

                case Button btn when btn == btnExecute:
                    text = "Please execute the FetchXML query!";
                    action = "Execute!";
                    break;
            }
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
            if (chatHistory == null)
            {
                MessageBoxEx.Show(fxb, "Chat history is not initialized. Please try to close and open the AI chat again.", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var manualquery = fxb.dockControlBuilder?.GetFetchString(true, false);
            if (string.IsNullOrEmpty(lastquery))
            {
                lastquery = manualquery;
            }
            if (!chatHistory.Initialized)
            {
                var intro = PromptSystem.Replace("{fetchxml}", fxb.dockControlBuilder?.GetFetchString(true, false));
                if (!string.IsNullOrEmpty(fxb.settings.AiSettings.MyName))
                {
                    intro += Environment.NewLine + PromptMyName.Replace("{callme}", fxb.settings.AiSettings.MyName).Trim();
                }
                chatHistory.Initialize(intro);
                fxb.LogUse($"{logname}-Init", count: intro.Length, ai2: true, ai1: false);
            }
            else if (!manualquery.EqualXml(lastquery))
            {
                lastquery = manualquery;
                chatHistory.Add(ChatRole.User, PromptUpdate.Replace("{fetchxml}", manualquery), true);
            }

            chatHistory.IsRunning = true;
            EnableButtons();
            fxb.LogUse($"{logname}-{action}", count: text.Length, ai2: true, ai1: false);
            callingstopwatch = Stopwatch.StartNew();
            try
            {
                AiCommunication.CallingAI(
                    text,
                    supplier.Name,
                    model.Name,
                    fxb.settings.AiSettings.ApiKey,
                    chatHistory,
                    fxb,
                    HandlingResponseFromAi,
                    ExecuteFetchXMLQuery,
                    UpdateCurrentFetchXmlQuery,
                    GetMetadataForUnknownEntity,
                    GetMetadataForUnknownAttribute);
            }
            catch (Exception ex)
            {
                fxb.LogError($"communicating with {supplier.Name}\n{ex.ExceptionDetails()}\n{ex.StackTrace}");
                fxb.ShowErrorDialog(ex, "AI Chat", "An error occurred while trying to communicate with the AI.");
            }
            txtAiChat.Clear();
        }

        private void EnableButtons()
        {
            btnAiChatAsk.Enabled = chatHistory != null && !string.IsNullOrWhiteSpace(txtAiChat.Text);
            btnYes.Enabled = chatHistory?.HasDialog == true;
            btnExecute.Enabled = chatHistory != null;
            btnCopy.Enabled = btnYes.Enabled;
            btnSave.Enabled = btnYes.Enabled;
            btnReset.Enabled = btnYes.Enabled;
            splitAiChat.Panel2.Enabled = chatHistory?.IsRunning != true;
            txtAiChat.BackColor = chatHistory?.IsRunning == true ? ChatMessageHistory.WaitingBackColor : ChatMessageHistory.BackColor;
            txtAiChat.Enabled = chatHistory?.IsRunning != true;
        }

        [Description("Executes FetchXML Query")]
        private string ExecuteFetchXMLQuery([Description("The FetchXML Query to be Executed. This is the current FetchXML, as specified in the conversation with the assistant.")] string fetchXml)
        {
            try
            {
                SetQueryFromAi(fetchXml);
                var sw = Stopwatch.StartNew();
                var result = fxb.RetrieveMultipleSync(fetchXml, null, null);
                sw.Stop();
                fxb.LogUse($"{logname}-Query-Execute", count: result is QueryInfo qi ? qi.Results.Entities.Count : 0, duration: sw.ElapsedMilliseconds, ai2: true, ai1: false);
                fxb.HandleRetrieveMultipleResult(result);
                //fxb.dockControlGrid?.ResetLayout();   Commented it out since it exploded, but it might be good to do this after each new query execute
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
                if (fetchXml.EqualXml(fxb.dockControlBuilder?.GetFetchString(true, false)))
                {
                    return "No changes made to the current query.";
                }
                // Informs the assistant that a change has been made to the current FetchXml.
                //chatHistory.Add(ChatRole.User, PromptUpdate.Replace("{fetchxml}", fetchXml), true);
                // Sets the current query, so that the query is updated in the FXB GUI.
                SetQueryFromAi(fetchXml);
                return "Current query updated successfully";
            }
            catch (Exception ex)
            {
                return $"Error updating current query: {ex.Message}";
            }
        }

        [Description("Retrieves the logical name and display name of tables/entity that matches a description. The result is returned in a JSON list with entries of the format {\"LN\":\"[logical name of entity]\",\"DN\":\"[display name of entity]\"}. There may be many results, if a unique table cannot be found.")]
        private string GetMetadataForUnknownEntity([Description("The name/description of a table.")] string tableDescription)
        {
            var entities = fxb.EntitiesToAi();
            var json = System.Text.Json.JsonSerializer.Serialize(entities, new System.Text.Json.JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

            chatHistory.Add(ChatRole.User, $"The tool GetMetadataForUnknownAttribute was called: retrieve a table that matches the description '{tableDescription}'", true);

            var sw = Stopwatch.StartNew();
            var result = AiCommunication.SamplingAI(PromptEntityMeta.Replace("{metadata}", json),
                $"Please find entries that match the description {tableDescription}", supplier.Name, model.Name, fxb.settings.AiSettings.ApiKey);
            sw.Stop();
            fxb.LogUse($"{logname}-Meta-Entity-{tableDescription}", count: entities.Count, duration: sw.ElapsedMilliseconds, ai2: true, ai1: false);

            chatHistory.Add(result, true);
            return result.Text;
        }

        [Description("Returns attributes of a table/entity that matches a description. Information about attributes is returned in a JSON list with entries of the format {\"LN\":\"[logical name of attribute]\",\"DN\":\"[display name of attribute]\"}. There may be many results, if a unique attribute cannot be found.")]
        private string GetMetadataForUnknownAttribute([Description("The logical name of the entity and a name/description of an attribute, separated by '@@'. Example: 'logical name of table@@a description of an attribute'")] string entityNameAndAttributeDescription)
        {
            string[] parts = entityNameAndAttributeDescription.Split(new[] { "@@" }, 2, StringSplitOptions.None);

            var entityName = parts[0];
            var attributeDescription = parts[1];

            if (!metaAttributes.ContainsKey(entityName))
            {
                try
                {
                    var aimeta = fxb.AttributesToAi(entityName, true);

                    if (aimeta.Count == 0) return $"There is no table called '{entityName}'. Call the GetMetadataForUnknownEntity tool first to get the correct table name.";

                    metaAttributes[entityName] = aimeta;
                }
                catch (Exception ex)
                {
                    return $"Error retrieving attribute metadata: {ex.Message}";
                }
            }
            var attributes = metaAttributes[entityName];
            var json = JsonSerializer.Serialize(attributes, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

            chatHistory.Add(ChatRole.User, $"The tool GetMetadataForUnknownAttribute was called: retrieve attributes for table '{entityName}' that matches the description '{attributeDescription}'", true);

            var sw = Stopwatch.StartNew();
            var result = AiCommunication.SamplingAI(PromptAttributeMeta.Replace("{metadata}", json),
                $"Please find attributes that match the description {attributeDescription}", supplier.Name, model.Name, fxb.settings.AiSettings.ApiKey);
            sw.Stop();
            fxb.LogUse($"{logname}-Meta-Attribute-{entityName}", count: attributes.Count, duration: sw.ElapsedMilliseconds, ai2: true, ai1: false);

            chatHistory.Add(result, true);

            return result.Text;
        }

        private void HandlingResponseFromAi(ChatResponse response)
        {
            callingstopwatch?.Stop();
            fxb.LogUse($"{logname}-Response", count: response?.ToString()?.Length, duration: callingstopwatch?.ElapsedMilliseconds, ai2: true, ai1: false);
            txtAiChat.Clear();
            txtUsage.Text = chatHistory.Responses.UsageToString();
            EnableButtons();
            txtAiChat.Focus();
        }

        private void SetQueryFromAi(string fetch)
        {
            if (lastquery.EqualXml(fetch))
            {
                return;
            }
            lastquery = fetch;
            MethodInvoker mi = () => { fxb.dockControlBuilder.Init(fetch, null, false, "Query from AI", true); };
            if (InvokeRequired) Invoke(mi); else mi();
            fxb.LogUse($"{logname}-Query-Change", ai2: true, ai1: false);
        }

        #endregion Private Methods

        #region Private Event Handlers

        private void AiChatControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            chatHistory?.Save(Paths.LogsPath, "FXB");
            if (chatHistory?.Initialized == true)
            {
                fxb.LogUse($"{logname}-Closing", count: chatHistory.Responses?.Count, ai2: true, ai1: false);
            }
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
                SendChatToAI(btnAiChatAsk);
            }
            else if (e.KeyCode == Keys.Y && e.Control)
            {
                e.Handled = true;
                SendChatToAI(btnYes);
            }
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