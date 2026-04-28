using Microsoft.Extensions.AI;
using Rappen.AI.WinForm;
using Rappen.XRM.Helpers;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FXB.Settings;
using Rappen.XTB.Helpers;
using Rappen.XTB.Helpers.RappXTB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using XrmToolBox.AppCode.AppInsights;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    public partial class AiChatControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private const string GeneralSettingsURL = "https://rappen.github.io/Tools/";
        private const string AiUsersFileName = "Rappen.XTB.AI.Users.xml";
        private static readonly string NewSectionMd = Environment.NewLine + Environment.NewLine + "---" + Environment.NewLine;
        private static readonly string localFolder = Path.Combine(Paths.SettingsPath, "FXB");
        private const int MaxMetadataMatchesToShow = 10;

        private FetchXmlBuilder fxb;
        private AIAppInsights ai;
        private ChatMessageHistory chatHistory;
        private AiProvider provider;
        private string model;
        private string lastquery;
        private Stopwatch sessionstopwatch;
        private Stopwatch callingstopwatch;
        private Dictionary<string, List<MetadataForAIAttribute>> metaAttributes = new Dictionary<string, List<MetadataForAIAttribute>>();
        private Dictionary<string, List<MetadataForAIRelationship>> metaRelationships = new Dictionary<string, List<MetadataForAIRelationship>>();
        private string logname = "AI";
        private bool logconversation = false;
        private int manualcalls = 0; // Counts the number of calls made by the user in this session
        private static List<AiUser> freeusers;
        private bool metadataavailable;
        private List<string> askhistory = new List<string>();
        private int currentaskhistory = -1;
        private AiChatTexts texts;

        #region Public Constructor

        public AiChatControl(FetchXmlBuilder fetchXmlBuilder, bool neverprompt)
        {
            fxb = fetchXmlBuilder;
            InitializeComponent();

            ChatMessageHistory.UserTextColor = OnlineSettings.Instance.Colors.Bright;
            ChatMessageHistory.UserBackgroundColor = OnlineSettings.Instance.Colors.Medium;
            ChatMessageHistory.AssistansTextColor = OnlineSettings.Instance.Colors.Dark;
            ChatMessageHistory.AssistansBackgroundColor = OnlineSettings.Instance.Colors.Bright;
            ChatMessageHistory.WaitingBackColor = Color.FromArgb(240, 240, 240);
            ChatMessageHistory.Font = panAiConversation.Font;

            Initialize(neverprompt);
            AllMetadataLoadedChanged(fxb.AllMetadataLoaded);
            EnableButtons();
        }

        #endregion Public Constructor

        #region Internal Methods

        internal void Initialize(bool neverprompt = false)
        {
            freeusers = null;
            ClosingSession();
            mnuSendWithEnter.Checked = fxb.settings.AiSettings.SendWithEnter;
            txtAiChat.Text = string.Empty;

            provider = OnlineSettings.Instance.AiSupport.Provider(fxb.settings.AiSettings.Provider);
            if (provider == null)
            {
                if (neverprompt)
                {
                    txtAiChat.Text = $"Missing AI Provider.{Environment.NewLine}Please open the Setting for AI Chat.";
                }
                else
                {
                    MessageBoxEx.Show(fxb, "The AI provider is not available (yet).\nGo check the setting!", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fxb.ShowSettings("tabAiChat");
                    if (!string.IsNullOrWhiteSpace(fxb.settings.AiSettings.Provider))
                    {
                        Initialize();
                    }
                }
                return;
            }
            logname = $"AI-{provider.Name}";
            model = fxb.settings.AiSettings.Model;
            var knownmodel = provider.Model(model);
            if (string.IsNullOrWhiteSpace(model))
            {
                if (neverprompt)
                {
                    txtAiChat.Text = $"Missing AI Model.{Environment.NewLine}Please open the Setting for AI Chat.";
                }
                else
                {
                    MessageBoxEx.Show(fxb, "The AI model is not available (yet).\nGo check the setting!", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fxb.ShowSettings("tabAiChat");
                    if (!string.IsNullOrWhiteSpace(fxb.settings.AiSettings.Model))
                    {
                        Initialize();
                    }
                }
                return;
            }
            var endpoint = provider.EndpointFixed ? knownmodel?.Endpoint : fxb.settings.AiSettings.Endpoint;
            if (!provider.EndpointFixed && string.IsNullOrWhiteSpace(endpoint))
            {
                if (neverprompt)
                {
                    txtAiChat.Text = $"Missing AI Endpoint.{Environment.NewLine}Please open the Setting for AI Chat.";
                }
                else
                {
                    MessageBoxEx.Show(fxb, "The AI endpoint is not set.\nGo check the setting!", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fxb.ShowSettings("tabAiChat");
                    if (!string.IsNullOrWhiteSpace(fxb.settings.AiSettings.Endpoint))
                    {
                        Initialize();
                    }
                }
                return;
            }
            var apikey = "";
            if (provider.Free)
            {
                logname = "AI-Free";
                if (IsFreeAiUser(fxb))
                {
                    apikey = provider.ApiKeyDecrypted;
                    if (string.IsNullOrWhiteSpace(apikey))
                    {
                        if (neverprompt)
                        {
                            txtAiChat.Text = "The Free AI provider is currently not available. Sorry...";
                        }
                        else
                        {
                            MessageBoxEx.Show(fxb, "The AI model is unfortunately not available right now.", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fxb.ShowSettings("tabAiChat");
                            if (!string.IsNullOrWhiteSpace(fxb.settings.AiSettings.Model))
                            {
                                Initialize();
                            }
                        }
                        return;
                    }
                }
            }
            else
            {
                apikey = fxb.settings.AiSettings.ApiKey;
                if (string.IsNullOrWhiteSpace(apikey))
                {
                    if (neverprompt)
                    {
                        txtAiChat.Text = $"Missing API Key.{Environment.NewLine}Please open the Setting for AI Chat.";
                    }
                    else
                    {
                        MessageBoxEx.Show(fxb, "The AI API Key is not entered.\nGo check the setting!", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fxb.ShowSettings("tabAiChat");
                        if (!string.IsNullOrWhiteSpace(fxb.settings.AiSettings.Model))
                        {
                            Initialize();
                        }
                    }
                    return;
                }
            }
            logconversation = knownmodel?.LogConversation ?? fxb.settings.AiSettings.LogConversation;
            chatHistory = new ChatMessageHistory(panAiConversation, provider.Name, model, endpoint, apikey, fxb.settings.AiSettings.MyName, OnlineSettings.Instance.AiSupport.OnlyInfoName, provider.ToString());
            metaAttributes.Clear();
            metaRelationships.Clear();
            SetTitle();
            if (provider.Free && !IsFreeAiUser(fxb) && !string.IsNullOrWhiteSpace(OnlineSettings.Instance.AiSupport.TextToRequestFreeAi))
            {
                chatHistory.Add(ChatRole.Assistant, OnlineSettings.Instance.AiSupport.TextToRequestFreeAi);
            }
            mnuFree.Text = IsFreeAiUser(fxb) ? "Using AI for Free!" : "Request for Free AI...";
            EnableButtons();
            if (txtAiChat.Enabled)
            {
                txtAiChat.Focus();
            }
            texts = new AiChatTexts(provider, knownmodel, localFolder, fxb.settings.AiSettings.Strictness);
        }

        internal static bool IsFreeAiUser(PluginControlBase tool)
        {
            if (freeusers == null)
            {
                freeusers = XmlAtomicStore.DownloadXml<List<AiUser>>(GeneralSettingsURL, AiUsersFileName, Paths.SettingsPath);
            }
            return freeusers?.Any(u =>
                u.ToolName == tool.ToolName &&
                u.Type == "Free" &&
                u.InstallationId.Equals(InstallationInfo.Instance.InstallationId)) == true;
        }

        internal static void PromptToUseForFree(FetchXmlBuilder tool)
        {
            var install = Installation.Load();
            var url = OnlineSettings.Instance.AiSupport.UrlToUseForFree;
            var wpf = OnlineSettings.Instance.AiSupport.WpfToUseForFree;
            var installid = InstallationInfo.Instance.InstallationId;
            var fullurl = $"{url}?" +
                $"wpf{wpf}_1_first={install?.PersonalFirstName}&" +
                $"wpf{wpf}_1_last={install?.PersonalLastName}&" +
                $"wpf{wpf}_3={install?.PersonalCountry}&" +
                $"wpf{wpf}_4={install?.PersonalEmail}&" +
                $"wpf{wpf}_31={tool.ToolName}&wpf{wpf}_32={tool.Version}&wpf{wpf}_33={installid}";
            Process.Start(fullurl);
        }

        internal void AllMetadataLoadedChanged(bool allLoaded)
        {
            metadataavailable = allLoaded;
            txtAiChat.Text = allLoaded ? string.Empty : $"Please wait until all metadata is loaded before asking the AI chat.{Environment.NewLine}We need to be able to provide some metadata (NO data!) to solve the issue more correctly.";
            EnableButtons();
        }

        #endregion Internal Methods

        #region Private Methods

        private void SetTitle()
        {
            Text = $"AI Chat - {provider?.ToString() ?? "<no provider>"} - {model ?? "<no model>"}";
            TabText = Text;
        }

        private void EnableButtons()
        {
            var cancall = chatHistory != null && !string.IsNullOrWhiteSpace(chatHistory.ApiKey) && metadataavailable;
            btnAiChatAsk.Enabled = cancall && !string.IsNullOrWhiteSpace(txtAiChat.Text);
            btnYes.Enabled = cancall && chatHistory?.HasDialog == true;
            btnExecute.Enabled = cancall;
            btnReset.Enabled = (cancall && chatHistory?.IsRunning == false && chatHistory?.HasDialog == true) || (provider?.Free == true && !IsFreeAiUser(fxb));
            splitAiChat.Panel2.Enabled = chatHistory?.IsRunning != true;
            txtAiChat.BackColor = chatHistory?.IsRunning == true ? ChatMessageHistory.WaitingBackColor : ChatMessageHistory.BackColor;
            txtAiChat.Enabled = cancall && chatHistory?.IsRunning != true;
        }

        private void SendChatToAI(object sender, EventArgs e = null)
        {
            if (metadataavailable != true)
            {
                MessageBoxEx.Show(fxb, "Please wait until all metadata is loaded before asking the AI chat.\nWe need to be able to provide some metadata (NO data!) to solve the issue more correctly.", "AI Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var text = string.Empty;
            var action = "Prompt";
            switch (sender)
            {
                case Button btn when btn == btnAiChatAsk:
                    text = txtAiChat.Text;
                    AddHistoryIfNeeded(text);
                    break;

                case Button btn when btn == btnYes:
                    text = "Yes please!";
                    action += "-Yes";
                    break;

                case Button btn when btn == btnExecute:
                    text = "Please execute the query!";
                    action += "-Execute";
                    break;

                case int option:
                    text = option.ToString();
                    action += "-Option";
                    break;

                case string manual:
                    text = manual;
                    AddHistoryIfNeeded(text);
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
            if (string.IsNullOrWhiteSpace(chatHistory.Provider))
            {
                if (MessageBoxEx.Show(fxb, "No AI provider found.\nAdd it in the setting!", "AI Chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
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

            fxb.settings.AiSettings.Calls++;
            fxb.settings.Save();
            PopupMessageIfRelevant();
            manualcalls++;

            var manualquery = fxb.dockControlBuilder?.GetFetchString(true, false);
            if (string.IsNullOrEmpty(lastquery))
            {
                lastquery = manualquery;
            }
            if (!chatHistory.Initialized)
            {
                var intro = PopulateForAi(
                    texts.System + NewSectionMd +
                    texts.Style + NewSectionMd +
                    texts.Behavior + NewSectionMd +
                    texts.Strictness + NewSectionMd +
                    texts.Preferences + NewSectionMd);

                if (!string.IsNullOrWhiteSpace(fxb.settings.AiSettings.InstructionsFlavor))
                {
                    intro += Environment.NewLine + Environment.NewLine + PopulateForAi(texts.UserFlavors, ("userflavors", fxb.settings.AiSettings.InstructionsFlavor)) + NewSectionMd;
                }

                chatHistory.Initialize(intro);
                Log("Init", count: intro.Length, msg: intro);
                sessionstopwatch = Stopwatch.StartNew();
            }
            else if (!manualquery.EqualXml(lastquery))
            {
                lastquery = manualquery;
                chatHistory.Add(
                    ChatRole.User,
                    PopulateForAi(texts.UpdatedQuery, ("fetchxml", manualquery)),
                    true);
            }

            text = text.Trim();
            chatHistory.IsRunning = true;
            EnableButtons();
            Log(action, count: text.Length, msg: text);
            callingstopwatch = Stopwatch.StartNew();
            try
            {
                // TESTING UI
                //chatHistory.Add(ChatRole.User, text);
                //chatHistory.IsRunning = false;
                //chatHistory.Add(ChatRole.Assistant, $"Right back to ya!\r\n{text}");
                //HandlingResponseFromAi(new ChatResponse { Messages = new List<ChatMessage> { new ChatMessage(ChatRole.Assistant, $"Right back to ya!\r\n{text}") } });
                // END TESTING UI

                AiCommunication.Prompt(
                    fxb,
                    chatHistory,
                    text,
                    HandlingResponseFromAi,
                    GetInternalTools());
            }
            catch (Exception ex)
            {
                fxb.LogError($"Communicating with {provider}:\n{ex.ExceptionDetails()}\n{ex.StackTrace}");
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
            if (!fxb.IsShownAndActive())
            {
                ToastHelper.ToastIt(
                    fxb,
                    $"AiChatControl",
                    $"FetchXML Builder AI Chat has answered!",
                    $"{provider} {model} was thinking in{Environment.NewLine}{callingstopwatch.Elapsed.ToSmartStringLong()}.{Environment.NewLine}{Environment.NewLine}Click to read my thoughts!",
                    response.Text,
                    logo: "https://rappen.github.io/Tools/Images/Robot100.png");
            }
        }

        private string PopulateForAi(string text, params (string placeholder, string replacement)[] extraplaceholders)
        {
            var placeholders = new Dictionary<string, string>
            {
                ["fetchxml"] = fxb.dockControlBuilder?.GetFetchString(true, false) ?? string.Empty,
                ["callme"] = !string.IsNullOrWhiteSpace(fxb.settings.AiSettings.MyName) ? fxb.settings.AiSettings.MyName : "you",
                ["prefer"] = fxb.settings.AiSettings.PreferDisplayName ? "DisplayName" : "LogicalName",
                ["providername"] = provider?.Name ?? string.Empty,
                ["modelname"] = model,
                ["hascurrentquery"] = string.IsNullOrWhiteSpace(fxb.dockControlBuilder?.GetFetchString(true, false)) ? "no" : "yes"
            };

            if (extraplaceholders != null)
            {
                foreach (var extra in extraplaceholders)
                {
                    placeholders[extra.placeholder] = extra.replacement ?? string.Empty;
                }
            }

            var result = text.Populate(placeholders.Select(p => (p.Key, p.Value)));
            return result;
        }

        private AiInternalTool[] GetInternalTools() => new[]
        {
            new AiInternalTool(ExecuteFetchXMLQuery, "run_query",PopulateForAi(texts.ToolRunQuery)),
            new AiInternalTool(UpdateCurrentFetchXmlQuery, "update_query", PopulateForAi(texts.ToolUpdateQuery)),
            new AiInternalTool(GetMetadataForUnknownEntity, "match_table", PopulateForAi(texts.ToolMatchTable)),
            new AiInternalTool(GetMetadataForUnknownRelationship, "match_relationship", PopulateForAi(texts.ToolMatchRelationship)),
            new AiInternalTool(GetMetadataForUnknownAttribute, "match_column", PopulateForAi(texts.ToolMatchColumn))
        };

        private string ExecuteFetchXMLQuery([Description("Full FetchXML query.")] string fetchXml)
        {
            try
            {
                chatHistory.Add(ChatRole.Assistant, "Executing the FetchXML query...", false, true);
                SetQueryFromAi(fetchXml);
                var sw = Stopwatch.StartNew();
                var result = fxb.RetrieveMultipleSync(fetchXml, null, null);
                sw.Stop();
                var records = (result as QueryInfo)?.Results?.Entities?.Count ?? null;
                Log($"Query-Execute", records, sw.ElapsedMilliseconds);
                chatHistory.Add(ChatRole.Assistant, $"Retrieved {records} records in {sw.Elapsed.ToSmartStringLong()}.", false, true);
                fxb.HandleRetrieveMultipleResult(result);
                chatHistory.Add(ChatRole.User, records == 0 ? "No record returned." : $"Retrieved {records} records.", true);
                //Commented it out since it exploded, but it might be good to do this after each new query execute
                //fxb.dockControlGrid?.ResetLayout();
                return "Query executed successfully";
            }
            catch (Exception ex)
            {
                return $"The FetchXML query execution failed. Please let me know if you change the query to solve the problem. Error message: {ex.Message}";
            }
        }

        private string UpdateCurrentFetchXmlQuery([Description("Full FetchXML query.")] string fetchXml)
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

        private string GetMetadataForUnknownEntity([Description("One table name or description.")] string tableDescription)
        {
            var entities = fxb.EntitiesForAi();
            var json = JsonSerializer.Serialize(entities, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

            var sw = Stopwatch.StartNew();
            var result = AiCommunication.PromptStateless(
                chatHistory,
                texts.Strictness + NewSectionMd +
                texts.EntityMeta.Replace("{{metadata}}", json),
                $"Please find entries that match the description {tableDescription}",
                $"Asking for metadata for table '{tableDescription}'...");
            sw.Stop();
            Log($"Meta-Entity-{tableDescription}", result, sw.ElapsedMilliseconds, entities.Count);

            chatHistory.Add(result, true);
            try
            {
                var hitentities = JsonSerializer.Deserialize<List<MetadataForAIEntity>>(result.Text);
                var hits = hitentities.Count > 0 ?
                    hitentities.Count > MaxMetadataMatchesToShow ?
                        $"...found {hitentities.Count} tables." :
                        hitentities.Count > 1 ?
                            $"...found tables:{Environment.NewLine}* {string.Join(Environment.NewLine + "* ", hitentities.Select(e => e.D + " (" + e.L + ")"))}" :
                            $"...found table: {hitentities[0].D} ({hitentities[0].L})." :
                    "...no tables found matching.";
                chatHistory.Add(ChatRole.Assistant, hits, false, true);
            }
            catch
            {
                // If the result cannot be deserialized, we just return the text, which might be an error message from the AI.
            }
            return result.Text;
        }

        private string GetMetadataForUnknownAttribute([Description("Entity logical name and one column name/description, separated by '@@'. Example: 'account@@primary contact'. Use exactly one column request per call. If the user wording is plural or sounds like related records, that may indicate a related table rather than a column on this table.")] string entityNameAndAttributeDescription)
        {
            var parts = entityNameAndAttributeDescription.Split(new[] { "@@" }, 2, StringSplitOptions.None);
            if (parts.Length != 2 ||
                 string.IsNullOrWhiteSpace(parts[0]) ||
                 string.IsNullOrWhiteSpace(parts[1]))
            {
                return "Invalid input. Use 'entitylogicalname@@attribute name or attribute description'.";
            }

            var entityName = parts[0];
            var attributeName = parts[1];

            if (!metaAttributes.ContainsKey(entityName))
            {
                try
                {
                    var aimeta = fxb.AttributesForAi(entityName, true);

                    if (aimeta.Count == 0)
                    {
                        return $"There is no table called '{entityName}'. Call the GetMetadataForUnknownEntity tool first to get the correct table name.";
                    }

                    metaAttributes[entityName] = aimeta;
                }
                catch (Exception ex)
                {
                    return $"Error retrieving attribute metadata: {ex.Message}";
                }
            }
            var attributes = metaAttributes[entityName];
            var json = JsonSerializer.Serialize(attributes, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

            chatHistory.Add(ChatRole.User, $"The tool GetMetadataForUnknownAttribute was called: retrieve attributes for table '{entityName}' that matches the name '{attributeName}'", true);

            var sw = Stopwatch.StartNew();
            var result = AiCommunication.PromptStateless(
                chatHistory,
                texts.Strictness + NewSectionMd +
                texts.AttributeMeta
                    .Replace("{{entityname}}", entityName)
                    .Replace("{{metadata}}", json),
                $"Please find attributes that match the name {attributeName}",
                $"Asking for metadata for column '{attributeName}' in table '{entityName}'...");
            sw.Stop();
            Log($"Meta-Attribute-{entityName}-{attributeName}", result, sw.ElapsedMilliseconds, attributes.Count);

            chatHistory.Add(result, true);
            try
            {
                var hitattrs = JsonSerializer.Deserialize<List<MetadataForAIAttribute>>(result.Text);
                var hits = hitattrs.Count > 0 ?
                    hitattrs.Count > MaxMetadataMatchesToShow ?
                        $"...found {hitattrs.Count} attributes." :
                        hitattrs.Count > 1 ?
                            $"...found attributes:{Environment.NewLine}* {string.Join(Environment.NewLine + "* ", (hitattrs.Select(a => a.D + " (" + a.L + ")")))}." :
                            $"...found attribute {hitattrs[0].D} ({hitattrs[0].L})" :
                    "...no attributes found matching.";
                chatHistory.Add(ChatRole.Assistant, hits, false, true);
            }
            catch
            {
                // If the result cannot be deserialized, we just return the text, which might be an error message from the AI.
            }
            return result.Text;
        }

        private string GetMetadataForUnknownRelationship([Description("Entity logical name and one related table name/description, separated by '@@'. Example: 'account@@contacts'. Use exactly one related table request per call.")] string entityNameAndRelationshipDescription)
        {
            var parts = entityNameAndRelationshipDescription.Split(new[] { "@@" }, 2, StringSplitOptions.None);
            if (parts.Length != 2 ||
                string.IsNullOrWhiteSpace(parts[0]) ||
                string.IsNullOrWhiteSpace(parts[1]))
            {
                return "Invalid input. Use 'entitylogicalname@@related table name or relationship description'.";
            }

            var entityName = parts[0].Trim();
            var relationshipName = parts[1].Trim();

            if (!metaRelationships.ContainsKey(entityName))
            {
                try
                {
                    var aimeta = fxb.RelationshipsForAi(entityName);
                    if (aimeta.Count == 0)
                    {
                        return $"There is no table called '{entityName}', or it has no available relationships. Call the GetMetadataForUnknownEntity tool first to get the correct table name.";
                    }

                    metaRelationships[entityName] = aimeta;
                }
                catch (Exception ex)
                {
                    return $"Error retrieving relationship metadata: {ex.Message}";
                }
            }

            var relationships = metaRelationships[entityName];
            var json = JsonSerializer.Serialize(relationships, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

            chatHistory.Add(ChatRole.User, $"The tool GetMetadataForUnknownRelationship was called: retrieve relationships for table '{entityName}' that matches the name '{relationshipName}'", true);

            var sw = Stopwatch.StartNew();
            var result = AiCommunication.PromptStateless(
                chatHistory,
                texts.Strictness + NewSectionMd +
                texts.RelationshipMeta
                    .Replace("{{entityname}}", entityName)
                    .Replace("{{metadata}}", json),
                $"Please find relationships that match the description {relationshipName}",
                $"Asking for relationships for '{relationshipName}' from table '{entityName}'...");
            sw.Stop();
            Log($"Meta-Relationship-{entityName}-{relationshipName}", result, sw.ElapsedMilliseconds, relationships.Count);

            chatHistory.Add(result, true);
            try
            {
                var hitrels = JsonSerializer.Deserialize<List<MetadataForAIRelationship>>(result.Text);
                var hits = hitrels.Count > 0 ?
                    hitrels.Count > MaxMetadataMatchesToShow ?
                        $"...found {hitrels.Count} relationships." :
                        hitrels.Count > 1 ?
                            $"...found relationships:{Environment.NewLine}* {string.Join(Environment.NewLine + "* ", (hitrels.Select(r => r.ToRelationshipString())))}." :
                            $"...found relationship: {hitrels[0].ToRelationshipString()}." :
                    "...no relationships found matching.";
                chatHistory.Add(ChatRole.Assistant, hits, false, true);
            }
            catch
            {
            }
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
            if (InvokeRequired)
            {
                Invoke(mi);
            }
            else
            {
                mi();
            }

            Log($"Query-Change", msg: fetch);
        }

        private void PopupMessageIfRelevant()
        {
            var supporting = Supporting.IsMonetarySupporting(fxb) || Supporting.IsPending(fxb);
            if (OnlineSettings.Instance.AiSupport.PopupByCallNos
                .FirstOrDefault(p => p.TimeToPopup(fxb.settings.AiSettings.Calls, supporting, provider.Free)) is PopupByCallNo popup)
            {
                var title = string.IsNullOrWhiteSpace(popup.Title) ? "AI Chat" :
                    popup.Title
                        .Replace("{calls}", fxb.settings.AiSettings.Calls.ToString())
                        .Replace("{provider}", provider.ToString())
                        .Replace("{model}", model);
                var message = popup.Message
                    .Replace("{calls}", fxb.settings.AiSettings.Calls.ToString())
                    .Replace("{provider}", provider.ToString())
                    .Replace("{model}", model);
                Log($"Popup-{title.Replace(" ", "-")}", msg: message);
                if (popup.SuggestsSupporting)
                {
                    if (MessageBoxEx.Show(fxb, message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        Supporting.ShowIf(fxb, ShowItFrom.Button, true, false, sync: true);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(popup.HelpUrl))
                {
                    MessageBoxEx.Show(fxb, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0, popup.HelpUrl);
                }
                else
                {
                    MessageBoxEx.Show(fxb, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ClosingSession()
        {
            if (chatHistory?.Initialized == true)
            {
                chatHistory.Save(Paths.LogsPath, "FXB");
                if (manualcalls > 0)
                {
                    Log("Session-Prompts", count: manualcalls);
                }
                if (chatHistory.Responses?.Count > 0)
                {
                    Log("Session-Responses", chatHistory.Responses.Count);
                }
                Log("Session-Summary",
                    count: chatHistory.Messages?.Count ?? 0,
                    duration: sessionstopwatch?.ElapsedMilliseconds,
                    tokensout: chatHistory.TokensOut,
                    tokensin: chatHistory.TokensIn);
            }
            manualcalls = 0;
            chatHistory = null;
            ai = null;
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
            if (ai == null)
            {
                ai = new AIAppInsights(fxb, OnlineSettings.Instance.AiSupport.AppRegistrationEndpoint, OnlineSettings.Instance.AiSupport.InstrumentationKey, provider.Name, model);
            }
            ai.WriteEvent($"{action}", count ?? msg?.Length, duration, tokensout, tokensin, logconversation ? msg : null);
        }

        private void AddHistoryIfNeeded(string text)
        {
            if (currentaskhistory < 0 || currentaskhistory >= askhistory.Count || askhistory[currentaskhistory] != text)
            {
                askhistory.Add(text);
                currentaskhistory = askhistory.Count;
            }
        }

        private void CopyFromHistory(int direction)
        {
            currentaskhistory += direction;
            if (currentaskhistory < 0)
            {
                currentaskhistory = 0;
            }
            else if (currentaskhistory >= askhistory.Count)
            {
                currentaskhistory = askhistory.Count;
                txtAiChat.Text = string.Empty;
                return;
            }
            if (currentaskhistory >= 0 && currentaskhistory < askhistory.Count)
            {
                txtAiChat.Text = askhistory[currentaskhistory];
                txtAiChat.SelectionStart = txtAiChat.Text.Length;
            }
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
            if (!e.Control && !e.Shift && !e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter when fxb.settings.AiSettings.SendWithEnter && mnuSendWithEnter.Checked && btnAiChatAsk.Enabled && !string.IsNullOrWhiteSpace(txtAiChat.Text):
                        SendChatToAI(txtAiChat.Text);
                        break;

                    case Keys.Up:
                        CopyFromHistory(-1);
                        break;

                    case Keys.Down:
                        CopyFromHistory(1);
                        break;

                    default:
                        return;
                }
                e.Handled = true;
                e.SuppressKeyPress = true; // Prevents the beep sound
            }
            else if (e.Control)
            {   // Control...
                switch (e.KeyCode)
                {
                    case Keys k when k == Keys.Enter && btnAiChatAsk.Enabled && !string.IsNullOrWhiteSpace(txtAiChat.Text):
                        SendChatToAI(txtAiChat.Text);
                        break;

                    case Keys.Y:
                        SendChatToAI(btnYes);
                        break;

                    case Keys k when k >= Keys.D0 && k <= Keys.D9:
                        SendChatToAI((int)k - (int)Keys.D0);
                        break;

                    case Keys k when k >= Keys.NumPad0 && k <= Keys.NumPad9:
                        SendChatToAI((int)k - (int)Keys.NumPad0);
                        break;

                    default:
                        return;
                }
                e.Handled = true;
                e.SuppressKeyPress = true; // Prevents the beep sound
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            freeusers = null;
            if (provider?.Free == true && !IsFreeAiUser(fxb))
            {
                OnlineSettings.Reset();
                Initialize();
                return;
            }
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
            Supporting.ShowIf(fxb, ShowItFrom.Button, true, false, sync: true);
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

        private void mnuSendWithEnter_Click(object sender, EventArgs e)
        {
            fxb.settings.AiSettings.SendWithEnter = mnuSendWithEnter.Checked;
            fxb.settings.Save();
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