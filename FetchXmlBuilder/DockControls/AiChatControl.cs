using Microsoft.Extensions.AI;
using Rappen.AI.WinForm;
using Rappen.XRM.Helpers;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FXB.Settings;
using Rappen.XTB.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using XrmToolBox.AppCode.AppInsights;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    public partial class AiChatControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private FetchXmlBuilder fxb;
        private AIAppInsights ai;
        private ChatMessageHistory chatHistory;
        private AiSupplier supplier;
        private AiModel model;
        private string lastquery;
        private Stopwatch sessionstopwatch;
        private Stopwatch callingstopwatch;
        private Dictionary<string, List<MetadataForAIAttribute>> metaAttributes = new Dictionary<string, List<MetadataForAIAttribute>>();
        private string logname = "AI";
        private int manualcalls = 0; // Counts the number of calls made by the user in this session
        private static List<AiUser> freeusers;

        #region Public Constructor

        public AiChatControl(FetchXmlBuilder fetchXmlBuilder)
        {
            ChatMessageHistory.UserTextColor = OnlineSettings.Instance.Colors.Bright;
            ChatMessageHistory.UserBackgroundColor = OnlineSettings.Instance.Colors.Medium;
            ChatMessageHistory.AssistansTextColor = OnlineSettings.Instance.Colors.Dark;
            ChatMessageHistory.AssistansBackgroundColor = OnlineSettings.Instance.Colors.Bright;
            ChatMessageHistory.WaitingBackColor = Color.FromArgb(240, 240, 240);

            fxb = fetchXmlBuilder;
            InitializeComponent();
            Initialize();
            EnableButtons();
        }

        #endregion Public Constructor

        #region Internal Methods

        internal void Initialize()
        {
            freeusers = null;
            ClosingSession();

            supplier = OnlineSettings.Instance.AiSupport.Supplier(fxb.settings.AiSettings.Supplier);
            if (supplier == null)
            {
                MessageBoxEx.Show(fxb, "The AI supplier is not available (yet).\nGo check the setting!", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fxb.ShowSettings("tabAiChat");
                if (!string.IsNullOrWhiteSpace(fxb.settings.AiSettings.Supplier))
                {
                    Initialize();
                }
                return;
            }
            logname = $"AI-{supplier.Name}";
            model = supplier.Model(fxb.settings.AiSettings.Model);
            if (model == null)
            {
                MessageBoxEx.Show(fxb, "The AI model is not available (yet).\nGo check the setting!", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fxb.ShowSettings("tabAiChat");
                if (!string.IsNullOrWhiteSpace(fxb.settings.AiSettings.Model))
                {
                    Initialize();
                }
                return;
            }
            var apikey = "";
            if (supplier.Free)
            {
                logname = "AI-Free";
                if (IsFreeAiUser(fxb))
                {
                    apikey = model.ApiKeyDecrypted;
                    //model.ApiKey = apikey;
                    //OnlineSettings.Instance.Save();
                    if (string.IsNullOrWhiteSpace(apikey))
                    {
                        MessageBoxEx.Show(fxb, "The AI model is unfortunately not available right now.", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fxb.ShowSettings("tabAiChat");
                        if (!string.IsNullOrWhiteSpace(fxb.settings.AiSettings.Model))
                        {
                            Initialize();
                        }
                        return;
                    }
                }
            }
            else
            {
                apikey = fxb.settings.AiSettings.ApiKey;
            }
            chatHistory = new ChatMessageHistory(panAiConversation, supplier?.Name, model?.Endpoint, model?.Name, apikey, fxb.settings.AiSettings.MyName);
            metaAttributes.Clear();
            SetTitle();
            if (supplier.Free && !IsFreeAiUser(fxb))
            {
                chatHistory.Add(ChatRole.Assistant, @"To use the free AI provider, you have to fill in a form.
Please click the button (the three dots down-right) and select 'Ask for Free AI'.

Jonas stands for any costs. Why? Read my blog: https://jonasr.app/free-ai-by-jonas
Note that there will be a slight lag between your submission and when it is activated.", false);
            }
            mnuFree.Text = IsFreeAiUser(fxb) ? "Using AI for free!" : "Ask for Free AI...";
            EnableButtons();
        }

        internal static bool IsFreeAiUser(PluginControlBase tool)
        {
            if (freeusers == null)
            {
                freeusers = new Uri("https://rappen.github.io/Tools/Rappen.XTB.AI.Users.xml")
                    .DownloadXml(new List<AiUser>())
                    .ToList();
            }
            return freeusers?.Any(u =>
                u.ToolName == tool.ToolName &&
                u.Type == "Free" &&
                u.InstallationId.Equals(InstallationInfo.Instance.InstallationId)) == true;
        }

        internal static void PromptToUseForFree(PluginControlBase tool)
        {
            var install = Installation.Load(null);
            var url = OnlineSettings.Instance.AiSupport.UrlToUseForFree;
            var wpf = OnlineSettings.Instance.AiSupport.WpfToUseForFree;
            var installid = InstallationInfo.Instance.InstallationId;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var fullurl = $"{url}?" +
                $"wpf{wpf}_1_first={install?.PersonalFirstName}&" +
                $"wpf{wpf}_1_last={install?.PersonalLastName}&" +
                $"wpf{wpf}_3={install?.PersonalCountry}&" +
                $"wpf{wpf}_4={install?.PersonalEmail}&" +
                $"wpf{wpf}_31={tool.ToolName}&wpf{wpf}_32={version}&wpf{wpf}_33={installid}";
            Process.Start(fullurl);
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
            Text = $"AI Chat - {supplier?.ToString() ?? "<no provider>"} - {model?.Name ?? "<no model>"}";
            TabText = Text;
        }

        private void EnableButtons()
        {
            var cancall = chatHistory != null && !string.IsNullOrWhiteSpace(chatHistory.ApiKey);
            btnAiChatAsk.Enabled = cancall && !string.IsNullOrWhiteSpace(txtAiChat.Text);
            btnYes.Enabled = cancall && chatHistory?.HasDialog == true;
            btnExecute.Enabled = cancall;
            btnReset.Enabled = cancall && chatHistory?.IsRunning == false && chatHistory?.HasDialog == true;
            splitAiChat.Panel2.Enabled = chatHistory?.IsRunning != true;
            txtAiChat.BackColor = chatHistory?.IsRunning == true ? ChatMessageHistory.WaitingBackColor : ChatMessageHistory.BackColor;
            txtAiChat.Enabled = cancall && chatHistory?.IsRunning != true;
        }

        private void SendChatToAI(object sender, EventArgs e = null)
        {
            var text = string.Empty;
            var action = "Prompt";
            var countcall = false;
            switch (sender)
            {
                case Button btn when btn == btnAiChatAsk:
                    text = txtAiChat.Text;
                    countcall = true;
                    break;

                case Button btn when btn == btnYes:
                    text = "Yes please!";
                    action += "-Yes";
                    break;

                case Button btn when btn == btnExecute:
                    text = "Please execute the query!";
                    action += "-Execute";
                    break;
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBoxEx.Show(fxb, "Please enter a question or request.", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (chatHistory == null)
            {
                MessageBoxEx.Show(fxb, "Chat history is not initialized. Please try to close and open the AI chat again.", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(chatHistory.Supplier))
            {
                if (MessageBoxEx.Show(fxb, "No AI supplier found.\nAdd it in the setting!", "AI Chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    fxb.ShowSettings("tabAiChat");
                }
                return;
            }
            if (string.IsNullOrWhiteSpace(chatHistory.Model))
            {
                if (MessageBoxEx.Show(fxb, "No AI model found", "AI Chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    fxb.ShowSettings("tabAiChat");
                }
                return;
            }
            if (string.IsNullOrWhiteSpace(chatHistory.ApiKey))
            {
                if (MessageBoxEx.Show(fxb, "No API Key found.\nAdd it in the setting!", "AI Chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    fxb.ShowSettings("tabAiChat");
                }
                return;
            }

            if (countcall)
            {
                fxb.settings.AiSettings.Calls++;
                fxb.settings.Save();
                PopupMessageIfRelevant();
            }
            manualcalls++;

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
                Log("Init", count: intro.Length, msg: intro);
                sessionstopwatch = Stopwatch.StartNew();
            }
            else if (!manualquery.EqualXml(lastquery))
            {
                lastquery = manualquery;
                chatHistory.Add(ChatRole.User, PromptUpdate.Replace("{fetchxml}", manualquery), true);
            }

            chatHistory.IsRunning = true;
            EnableButtons();
            Log(action, count: text.Length, msg: text);
            callingstopwatch = Stopwatch.StartNew();
            try
            {
                AiCommunication.CallingAIAsync(
                    fxb,
                    chatHistory,
                    text,
                    HandlingResponseFromAi,
                    ExecuteFetchXMLQuery,
                    UpdateCurrentFetchXmlQuery,
                    GetMetadataForUnknownEntity,
                    GetMetadataForUnknownAttribute);
            }
            catch (Exception ex)
            {
                fxb.LogError($"Communicating with {supplier}:\n{ex.ExceptionDetails()}\n{ex.StackTrace}");
                fxb.ShowErrorDialog(ex, "AI Chat", "An error occurred while trying to communicate with the AI.");
            }
            txtAiChat.Clear();
        }

        private void HandlingResponseFromAi(ChatResponse response)
        {
            callingstopwatch?.Stop();
            Log("Response", response, callingstopwatch?.ElapsedMilliseconds);
            txtAiChat.Clear();
            txtUsage.Text = chatHistory?.Responses?.UsageToString() ?? "?";
            EnableButtons();
            txtAiChat.Focus();
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
                var records = (result as QueryInfo)?.Results?.Entities?.Count ?? null;
                Log($"Query-Execute", records, sw.ElapsedMilliseconds);
                fxb.HandleRetrieveMultipleResult(result);
                AiCommunication.SamplingAI("The FetchXML query is executed", records == 0 ? "No record returned." : $"Retrieved {records} records.", chatHistory);
                //chatHistory.Add(ChatRole.User, records == 0 ? "No record returned." : $"Retrieved {records} records.", true);
                //Commented it out since it exploded, but it might be good to do this after each new query execute
                //fxb.dockControlGrid?.ResetLayout();
                return "Query executed successfully";
            }
            catch (Exception ex)
            {
                return $"The FetchXML query execution failed. Please let me know if you change the query to solve the problem. Error message: {ex.Message}";
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
            var entities = fxb.EntitiesForAi();
            var json = JsonSerializer.Serialize(entities, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

            chatHistory.Add(ChatRole.User, $"The tool GetMetadataForUnknownAttribute was called: retrieve a table that matches the description '{tableDescription}'", true);

            var sw = Stopwatch.StartNew();
            var result = AiCommunication.SamplingAI(PromptEntityMeta.Replace("{metadata}", json),
                $"Please find entries that match the description {tableDescription}", chatHistory);
            sw.Stop();
            Log($"Meta-Entity-{tableDescription}", result, sw.ElapsedMilliseconds, entities.Count);

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
                    var aimeta = fxb.AttributesForAi(entityName, true);

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
                $"Please find attributes that match the description {attributeDescription}", chatHistory);
            sw.Stop();
            Log($"Meta-Attribute-{entityName}", result, sw.ElapsedMilliseconds, attributes.Count);

            chatHistory.Add(result, true);

            return result.Text;
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
            Log($"Query-Change", msg: fetch);
        }

        private void PopupMessageIfRelevant()
        {
            var supporting = Supporting.IsMonetarySupporting(fxb) || Supporting.IsPending(fxb);
            if (OnlineSettings.Instance.AiSupport.PopupByCallNos
                .FirstOrDefault(p => p.TimeToPopup(fxb.settings.AiSettings.Calls, supporting, supplier.Free)) is PopupByCallNo popup)
            {
                var message = popup.Message
                    .Replace("{calls}", fxb.settings.AiSettings.Calls.ToString())
                    .Replace("{provider}", supplier.ToString())
                    .Replace("{model}", model.Name);
                if (popup.SuggestsSupporting)
                {
                    if (MessageBoxEx.Show(fxb, message, "AI Chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        Supporting.ShowIf(fxb, true, false, fxb.ai2);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(popup.HelpUrl))
                {
                    MessageBoxEx.Show(fxb, message, "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0, popup.HelpUrl);
                }
                else
                {
                    MessageBoxEx.Show(fxb, message, "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ClosingSession()
        {
            if (chatHistory != null)
            {
                chatHistory.Save(Paths.LogsPath, "FXB");
                if (chatHistory.Initialized)
                {
                    Log("Session-Time", duration: sessionstopwatch?.ElapsedMilliseconds);
                    if (manualcalls > 0)
                    {
                        Log("Session-Calls-Manual", count: manualcalls);
                    }
                    Log("Session-Calls-Total", chatHistory.Messages?.Count ?? 0);
                    Log("Session-Responses", chatHistory.Responses?.Count ?? 0);
                    Log("Session-Tokens", tokensout: chatHistory.TokensOut, tokensin: chatHistory.TokensIn);
                }
                chatHistory = null;
                Log("Close");
                ai = null;
            }
        }

        private void Log(string action, ChatResponse response, double? duration, double? count = null)
        {
            var text = response?.Text;
            if (string.IsNullOrWhiteSpace(text?.Trim('[', ']', '{', '}')))
            {
                text = null;
            }
            Log(action, count ?? response?.ToString()?.Length, duration, response?.Usage?.OutputTokenCount, response?.Usage?.InputTokenCount, text);
        }

        private void Log(string action, double? count = null, double? duration = null, long? tokensout = null, long? tokensin = null, string msg = null)
        {
            if (count == 0 && duration == null)
            {   // Unnecessary to log nothing
                return;
            }
            fxb.LogInfo($"{logname}-{action}{(count != null ? $" Count: {count}" : "")}{(duration != null ? $" Duration: {duration}" : "")}{(tokensout != null ? $" TokensOut: {tokensout}" : "")}{(tokensin != null ? $" TokensIn: {tokensin}" : "")}");

            if (ai == null)
            {
                ai = new AIAppInsights(fxb, OnlineSettings.Instance.AiSupport.AppRegistrationEndpoint, OnlineSettings.Instance.AiSupport.InstrumentationKey, supplier.Name, model.Name);
            }
            ai.WriteEvent($"{action}", count ?? msg?.Length, duration, tokensout, tokensin, fxb.settings.AiSettings.LogConversation ? msg : null);
        }

        #endregion Private Methods

        #region Private Event Handlers

        private void AiChatControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClosingSession();
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
            if (e.KeyCode == Keys.Enter && e.Control && btnAiChatAsk.Enabled && !string.IsNullOrWhiteSpace(txtAiChat.Text))
            {
                e.Handled = true;
                e.SuppressKeyPress = true; // Prevents the beep sound
                SendChatToAI(btnAiChatAsk);
            }
            else if (e.KeyCode == Keys.Y && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SendChatToAI(btnYes);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx.Show(fxb, "Are you sure you want to clear the AI chat history?", "Reset AI Chat", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Initialize();
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            mnuCopy.Enabled = chatHistory?.HasDialog == true;
            mnuSave.Enabled = chatHistory?.HasDialog == true;
            contextMenuStrip1.Show(btnMenu, new Point(0, btnMenu.Height));
        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            var chat = chatHistory.ToString();
            Clipboard.SetText(chat);
            fxb.WorkAsync(new WorkAsyncInfo { Message = "Copying!", Work = (w, a) => { Thread.Sleep(500); } });
        }

        private void mnuSave_Click(object sender, EventArgs e)
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

        private void mnuSettings_Click(object sender, EventArgs e)
        {
            fxb.ShowSettings("tabAiChat");
        }

        private void mnuSupporting_Click(object sender, EventArgs e)
        {
            Supporting.ShowIf(fxb, true, false, fxb.ai2);
        }

        private void mnuDocs_Click(object sender, EventArgs e)
        {
            UrlUtils.OpenUrl("https://fetchxmlbuilder.com/features/ai/");
        }

        private void mnuFree_Click(object sender, EventArgs e)
        {
            if (!IsFreeAiUser(fxb))
            {
                PromptToUseForFree(fxb);
            }
            else
            {
                MessageBoxEx.Show(fxb, "You are already registered as a free user!", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion Private Event Handlers
    }

    public class AiUser
    {
        public Guid InstallationId;
        public string ToolName;
        public string Type;
        public DateTime Date;
        public DateTime Published;

        public override string ToString() => $"{InstallationId} {ToolName} {Date}";
    }
}