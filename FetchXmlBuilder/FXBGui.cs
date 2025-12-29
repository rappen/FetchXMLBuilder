using Cinteros.Xrm.FetchXmlBuilder.Properties;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Converters;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Forms;
using Rappen.XTB.FetchXmlBuilder.Settings;
using Rappen.XTB.FetchXmlBuilder.Views;
using Rappen.XTB.FXB.Settings;
using Rappen.XTB.Helpers.RappXTB;
using Rappen.XTB.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using WeifenLuo.WinFormsUI.Docking;
using XrmToolBox;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder
{
    public partial class FetchXmlBuilder : RappPluginControlBase
    {
        internal static bool friendlyNames = false;
        internal TreeBuilderControl dockControlBuilder;
        internal XmlContentControl dockControlLayoutXml;
        internal ResultGrid dockControlGrid;
        internal HistoryManager historyMgr = new HistoryManager();
        internal bool historyisavailable = true;
        private string cwpfeed;
        private XmlContentControl dockControlFetchResult;
        private XmlContentControl dockControlFetchXml;
        private XmlContentControl dockControlFetchXmlJs;
        private XmlContentControl dockControlPowerPlatformCLI;
        private ODataControl dockControlOData2;
        private ODataControl dockControlOData4;
        private FlowListControl dockControlFlowList;
        private XmlContentControl dockControlCSharp;
        private XmlContentControl dockControlSQL;
        private MetadataControl dockControlMeta;
        private AiChatControl dockControlAiChat;
        private Entity dynml;
        private string fileName;
        private QueryRepository repository = new QueryRepository();
        private bool inSql4Cds;

        internal void EnableControls()
        {
            EnableControls(buttonsEnabled);
        }

        /// <summary>Enables or disables all buttons on the form</summary>
        /// <param name="enabled"></param>
        internal void EnableControls(bool enabled, bool toolstripmenuenabled = false)
        {
            MethodInvoker mi = delegate
            {
                try
                {
                    // Menus
                    toolStripMain.Enabled = enabled || toolstripmenuenabled;

                    // Main menu items
                    tsbNew.Enabled = enabled;
                    tsbOpen.Enabled = enabled;
                    tsbSave.Enabled = enabled;
                    tsbRepo.Enabled = enabled;
                    tsbExecute.Enabled = enabled && Service != null;
                    tsbAbort.Visible = settings.ExecuteOptions.AllPages;
                    tsbSend.Enabled = enabled;
                    tsbConvert.Enabled = enabled;
                    tsbView.Enabled = enabled;
                    tsbOptions.Enabled = enabled;

                    // Sub menu Open items
                    tsmiOpenView.Enabled = enabled && Service != null;
                    tsmiOpenML.Visible = enabled && Service != null && GetEntity("list") != null;
                    tsmiOpenCWP.Visible = enabled && Service != null && GetEntity("cint_feed") != null;

                    // Sub menu Save items
                    tsmiSaveFile.Enabled = enabled && dockControlBuilder?.FetchChanged == true && !string.IsNullOrEmpty(FileName);
                    tsmiSaveView.Enabled = enabled && Service != null && View != null && View.IsCustomizable();
                    tsmiSaveViewAs.Enabled = enabled && Service != null && (tsmiSaveView.Enabled || settings.Layout.Enabled);
                    tsmiSaveML.Enabled = enabled && Service != null && DynML != null;
                    tsmiSaveCWP.Visible = enabled && Service != null && GetEntity("cint_feed") != null;
                    tsmiSaveCWP.Enabled = enabled && Service != null && dockControlBuilder?.FetchChanged == true && !string.IsNullOrEmpty(CWPFeed);

                    // Sub menu Options items
                    tsmiSelect.Enabled = enabled && Service != null;

                    // Sub menu Convert items
                    tsmiShowLayoutXML.Enabled = enabled && Service != null && (dockControlBuilder?.IsFetchAggregate() == false) && settings.Layout.Enabled;
                    tsmiShowMetadata.Enabled = enabled && Service != null;
                    tsmiShowFlow.Enabled = enabled && Service != null;
                    tsmiShowOData.Enabled = enabled && Service != null;
                    tsmiShowOData4.Enabled = enabled && Service != null;
                    tsmiShowCSharpCode.Enabled = enabled && Service != null && (dockControlBuilder?.IsFetchAggregate() == false);

                    // Enable local menus/buttons/etc
                    dockControlBuilder?.EnableControls(enabled);

                    buttonsEnabled = enabled;
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
        }

        internal void EnableDisableHistoryButtons()
        {
            if (historyisavailable)
            {
                historyMgr.SetupUndoButton(tsbUndo);
                historyMgr.SetupRedoButton(tsbRedo);
            }
            else
            {
                tsbUndo.Enabled = false;
                tsbRedo.Enabled = false;
            }
        }

        internal void UpdateLiveXML(bool preventxmlupdate = false)
        {
            if (!preventxmlupdate)
            {
                dockControlBuilder?.LayoutXML?.MakeSureAllCellsExistForAttributes();
                if (dockControlFetchXml?.Visible == true)
                {
                    dockControlFetchXml.UpdateXML(dockControlBuilder.GetFetchString(true, false));
                }
                if (dockControlLayoutXml?.Visible == true)
                {
                    if (settings.Layout.Enabled)
                    {
                        dockControlLayoutXml.UpdateXML(dockControlBuilder.LayoutXML?.ToXMLString());
                    }
                    else
                    {
                        dockControlLayoutXml.Close();
                        dockControlBuilder.LayoutXML = null;
                    }
                }
            }
            if (dockControlGrid?.Visible == true)
            {
                dockControlGrid.SetQueryIfChangesDesign();
            }
            if (dockControlOData2?.Visible == true && entities != null)
            {
                dockControlOData2.DisplayOData(GetOData(2));
            }
            if (dockControlOData4?.Visible == true && entities != null)
            {
                dockControlOData4.DisplayOData(GetOData(4));
            }
            if (dockControlFlowList?.Visible == true && entities != null)
            {
                dockControlFlowList.DisplayFlowList(dockControlBuilder.GetFetchString(true, false));
            }
            if (dockControlCSharp?.Visible == true && entities != null)
            {
                dockControlCSharp.UpdateXML(GetCSharpCode());
            }
            if (dockControlSQL?.Visible == true)
            {
                var sql = GetSQLQuery(out var sql4cds);
                dockControlSQL.UpdateSQL(sql, sql4cds);
            }
            if (dockControlFetchXmlJs?.Visible == true)
            {
                dockControlFetchXmlJs.UpdateXML(GetJavaScriptCode());
            }
            if (dockControlPowerPlatformCLI?.Visible == true)
            {
                dockControlPowerPlatformCLI.UpdateXML(GetPowerPlatformCLIFetch());
            }
            if (dockControlMeta?.Visible == true)
            {
                dockControlMeta.UpdateMeta(dockControlBuilder.SelectedMetadata());
            }
        }

        internal void ShowMetadata(MetadataBase meta)
        {
            if (dockControlMeta?.Visible == true)
            {
                dockControlMeta.UpdateMeta(meta);
            }
        }

        internal void HelpClick(object sender, EventArgs e) => OpenUrl(sender);

        internal void ShowMetadataControl()
        {
            ShowMetadataControl(ref dockControlMeta, DockState.DockRight);
        }

        private static string GetDockFileName()
        {
            return Path.Combine(Paths.SettingsPath, "Rappen.XTB.FXB_[DockPanels].xml");
        }

        private void CreateRepoMenuItem(QueryDefinition query)
        {
            ToolStripDropDownItem folder = tsbRepo;
            var nameparts = query.Name.Split('\\');
            for (var i = 0; i < nameparts.Length - 1; i++)
            {
                var foldername = nameparts[i];
                folder = GetMenuFolder(folder, foldername);
            }
            var name = nameparts[nameparts.Length - 1];
            var menu = new ToolStripMenuItem(name) { Tag = query };
            menu.Click += tsmiRepoOpen_Click;
            folder.DropDownItems.Add(menu);
        }

        private ToolStripMenuItem GetMenuFolder(ToolStripDropDownItem parent, string label)
        {
            var result = parent.DropDownItems.Cast<ToolStripItem>().FirstOrDefault(m => m.Text == label && m.Tag as string == "folder") as ToolStripMenuItem;
            if (result == null)
            {
                result = new ToolStripMenuItem(label) { Tag = "folder" };
                parent.DropDownItems.Add(result);
            }
            return result;
        }

        private IDockContent dockDeSerialization(string persistString)
        {
            if (persistString == typeof(TreeBuilderControl).ToString() && dockControlBuilder?.IsDisposed != false)
            {
                dockControlBuilder = new TreeBuilderControl(this);
                return dockControlBuilder;
            }
            else if (persistString == typeof(ResultGrid).ToString() && dockControlGrid?.IsDisposed != false)
            {
                dockControlGrid = new ResultGrid(this);
                return dockControlGrid;
            }
            else if ((persistString == XmlContentControl.GetPersistString(ContentType.FetchXML_Result) ||
                      persistString == XmlContentControl.GetPersistString(ContentType.Serialized_Result_JSON) ||
                      persistString == XmlContentControl.GetPersistString(ContentType.Serialized_Result_XML)) &&
                      dockControlFetchResult?.IsDisposed != false)
            {
                dockControlFetchResult = new XmlContentControl(ContentType.FetchXML_Result, SaveFormat.XML, this);
                return dockControlFetchResult;
            }
            else if (persistString == XmlContentControl.GetPersistString(ContentType.FetchXML) && dockControlFetchXml?.IsDisposed != false)
            {
                dockControlFetchXml = new XmlContentControl(this);
                return dockControlFetchXml;
            }
            else if (persistString == XmlContentControl.GetPersistString(ContentType.LayoutXML) && dockControlLayoutXml?.IsDisposed != false)
            {
                dockControlLayoutXml = new XmlContentControl(ContentType.LayoutXML, SaveFormat.None, this);
                return dockControlLayoutXml;
            }
            else if (persistString == typeof(AiChatControl).ToString() && dockControlAiChat?.IsDisposed != false)
            {
                dockControlAiChat = new AiChatControl(this, true);
                return dockControlAiChat;
            }
            return null;
        }

        private string GetJavaScriptCode()
        {
            var js = string.Empty;
            var fetch = dockControlBuilder.GetFetchString(true, false);
            try
            {
                js = JavascriptCodeGenerator.GetJavascriptCode(fetch);
            }
            catch (Exception ex)
            {
                js = "Failed to generate JavaScript code.\n\n" + ex.Message;
            }
            return js;
        }

        private string GetPowerPlatformCLIFetch()
        {
            var fxl = dockControlBuilder.GetFetchString(false, false);
            fxl = XmlContentControl.GetFetchMini(fxl);
            return $"pac org fetch --xml \"{fxl}\"";
        }

        private void OpenCWPFeed()
        {
            if (!SaveIfChanged())
            {
                return;
            }
            var feedid = Prompt.ShowDialog("Enter CWP Feed ID", "Open CWP Feed");
            if (string.IsNullOrWhiteSpace(feedid))
            {
                return;
            }
            var feed = GetCWPFeed(feedid);
            if (feed == null)
            {
                MessageBoxEx.Show(this, "Feed not found.", "Open CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (feed.Contains("cint_fetchxml"))
            {
                CWPFeed = feed.Contains("cint_id") ? feed["cint_id"].ToString() : feedid;
                dockControlBuilder.Init(feed["cint_fetchxml"].ToString(), null, false, "open CWP feed", false);
                LogUse("OpenCWP");
            }
            EnableControls(true);
        }

        private void OpenFile()
        {
            if (!SaveIfChanged())
            {
                return;
            }
            var ofd = new OpenFileDialog
            {
                Title = "Select an XML file containing FetchXML",
                Filter = "XML file (*.xml)|*.xml"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                EnableControls(false);
                FileName = ofd.FileName;
                var fetchDoc = new XmlDocument();
                fetchDoc.Load(ofd.FileName);
                switch (fetchDoc.FirstChild?.Name)
                {
                    case "fetch":
                        dockControlBuilder.Init(fetchDoc.OuterXml, null, false, "open file", true);
                        break;

                    case "view":
                        var fetch = fetchDoc.SelectSingleNode("//fetch")?.InnerText;
                        var layout = fetchDoc.SelectSingleNode("//grid")?.InnerText;
                        dockControlBuilder.Init(fetch, layout, true, "open file", true);
                        break;

                    default:
                        MessageBoxEx.Show(this, "The selected file does not contain FetchXML.\nPlease select another one.", "Open File",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;
                }
                LogUse("OpenFile");
            }
            EnableControls(true);
        }

        private void OpenML()
        {
            if (!SaveIfChanged())
            {
                return;
            }
            var mlselector = new SelectMLDialog(this);
            mlselector.StartPosition = FormStartPosition.CenterParent;
            if (mlselector.ShowDialog() == DialogResult.OK)
            {
                if (mlselector.View.Contains("query") && !string.IsNullOrEmpty(mlselector.View["query"].ToString()))
                {
                    DynML = mlselector.View;
                    dockControlBuilder.Init(DynML["query"].ToString(), null, false, "open marketing list", false);
                    LogUse("OpenML");
                }
                else
                {
                    if (MessageBoxEx.Show(this, "The selected marketing list does not contain any FetchXML.\nPlease select another one.", "Open Marketing List",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        OpenML();
                    }
                }
            }
            EnableControls(true);
        }

        private void OpenView()
        {
            if (!SaveIfChanged())
            {
                return;
            }
            if (views == null || views.Count == 0)
            {
                LoadViews(OpenView);
            }
            else
            {
                var viewselector = new SelectViewDialog(this);
                viewselector.StartPosition = FormStartPosition.CenterParent;
                if (viewselector.ShowDialog() == DialogResult.OK)
                {
                    if (viewselector.View.Contains("fetchxml") && !string.IsNullOrEmpty(viewselector.View["fetchxml"].ToString()))
                    {
                        View = viewselector.View;
                        dockControlBuilder.Init(View["fetchxml"].ToString(), View["layoutxml"].ToString(), true, "open view", false);
                        attributesChecksum = dockControlBuilder.GetAttributesSignature();
                        LogUse($"OpenView-{(View.LogicalName == "savedquery" ? "S" : "P")}");
                    }
                    else
                    {
                        if (MessageBoxEx.Show(this, "The selected view does not contain any FetchXML.\nPlease select another one.", "Open View",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                        {
                            OpenView();
                        }
                    }
                }
            }
            EnableControls(true);
        }

        private void RebuildRepositoryMenu(QueryDefinition selectedquery)
        {
            tsbRepo.Tag = selectedquery;
            dockControlBuilder.SetFetchName(selectedquery != null ? $"Repo: {selectedquery.Name}" : null);
            var oldqueries = tsbRepo.DropDownItems.Cast<ToolStripItem>().Where(m => m.Tag is QueryDefinition || m.Tag?.ToString() == "folder").ToList();
            foreach (var oldmenu in oldqueries)
            {
                tsbRepo.DropDownItems.Remove(oldmenu);
            }
            foreach (var query in repository.Queries)
            {
                CreateRepoMenuItem(query);
            }
        }

        private void ResetDockLayout()
        {
            var i = 0;
            while (i < dockContainer.Contents.Count)
            {
                if (dockContainer.Contents[i] == dockControlBuilder)
                {
                    i++;
                }
                else
                {
                    dockContainer.Contents[i].DockHandler.Close();
                }
            }
            settings.DockStates = new DockStates();
            if (dockControlBuilder?.IsDisposed != false)
            {
                dockControlBuilder = new TreeBuilderControl(this);
            }
            dockControlBuilder.Show(dockContainer, DockState.DockLeft);
        }

        private void ResetSourcePointers()
        {
            FileName = null;
            CWPFeed = null;
            View = null;
            DynML = null;
        }

        private void SaveCWPFeed()
        {
            if (string.IsNullOrWhiteSpace(CWPFeed))
            {
                var feedid = Prompt.ShowDialog("Enter CWP Feed ID (enter existing ID to update feed)", "Save CWP Feed");
                if (feedid == null)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(feedid))
                {
                    MessageBoxEx.Show(this, "Feed not saved.", "Save CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                CWPFeed = feedid;
            }
            var feed = GetCWPFeed(CWPFeed);
            if (feed == null)
            {
                feed = new Entity("cint_feed");
            }
            if (feed.Contains("cint_fetchxml"))
            {
                feed.Attributes.Remove("cint_fetchxml");
            }
            feed.Attributes.Add("cint_fetchxml", dockControlBuilder.GetFetchString(true, false));
            var verb = feed.Id.Equals(Guid.Empty) ? "created" : "updated";
            if (feed.Id.Equals(Guid.Empty))
            {
                feed.Attributes.Add("cint_id", CWPFeed);
                feed.Attributes.Add("cint_description", "Created by FetchXML Builder for XrmToolBox");
                Service.Create(feed);
            }
            else
            {
                Service.Update(feed);
            }
            LogUse("SaveCWPFeed");
            MessageBoxEx.Show(this, "CWP Feed " + CWPFeed + " has been " + verb + "!", "Save CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveDockPanels()
        {
            var dockFile = GetDockFileName();
            dockContainer.SaveAsXml(dockFile);
        }

        private bool SaveFetchXML(bool prompt, bool silent)
        {
            var result = false;
            var newfile = prompt ? "" : FileName;
            if (prompt || string.IsNullOrEmpty(FileName))
            {
                var sfd = new SaveFileDialog
                {
                    Title = "Select a location to save the FetchXML",
                    Filter = "Xml file (*.xml)|*.xml",
                    FileName = System.IO.Path.GetFileName(FileName)
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    newfile = sfd.FileName;
                }
            }
            if (!string.IsNullOrEmpty(newfile))
            {
                EnableControls(false);
                FileName = newfile;
                dockControlBuilder.Save(FileName);
                LogUse("SaveFile");
                if (!silent)
                {
                    MessageBoxEx.Show(this, "FetchXML saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                result = true;
                EnableControls(true);
            }
            return result;
        }

        /// <summary>Enables buttons relevant for currently selected node</summary>
        private bool SaveIfChanged()
        {
            var ok = true;
            if (!settings.DoNotPromptToSave && dockControlBuilder?.FetchChanged == true)
            {
                var result = MessageBoxEx.Show(this, "FetchXML has changed.\nSave changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Cancel)
                {
                    ok = false;
                }
                if (result == DialogResult.Yes)
                {
                    if (!SaveFetchXML(false, true))
                    {
                        ok = false;
                    }
                }
            }
            return ok;
        }

        private void SaveML()
        {
            var msg = "Saving {0}...";
            WorkAsync(new WorkAsyncInfo(string.Format(msg, DynML["listname"]),
                (eventargs) =>
                {
                    var xml = dockControlBuilder.GetFetchString(false, false);
                    var newView = new Entity(DynML.LogicalName);
                    newView.Id = DynML.Id;
                    newView.Attributes.Add("query", xml);
                    Service.Update(newView);
                    LogUse("SaveML");
                    DynML["query"] = xml;
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        ShowErrorDialog(completedargs.Error, "Save Marketing List");
                    }
                    else
                    {
                        dockControlBuilder.ClearChanged();
                    }
                }
            });
        }

        private void SaveRepository()
        {
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), repository, "[QueryRepository]");
        }

        private void SaveView(bool saveas)
        {
            var entityname = View?["returnedtypecode"].ToString() ?? dockControlBuilder.RootEntityName;
            if (GetEntity(entityname) == null)
            {
                MessageBoxEx.Show(this, $"Can't set the correct ReturnedTypeCode from the metadata: {entityname}", "Save View",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0,
                    "https://docs.microsoft.com/power-apps/developer/model-driven-apps/customize-entity-views?WT.mc_id=DX-MVP-5002475#create-views");
                return;
            }
            if (dockControlBuilder.PrimaryIdNode == null)
            {
                if (MessageBoxEx.Show(this, $"Views should really include the primary id.\nYou should add attribute {dockControlBuilder.PrimaryIdName}.\n\nYes - I will fix it\nNo - I don't care.",
                                    "Save View", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.No)
                {
                    return;
                }
            }
            var includelayout = saveas;
            if (!includelayout && settings.Layout.Enabled)
            {
                var inclresult = MessageBoxEx.Show(this, "Include the layout of the view?", "Save View",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 0,
                    "https://jonasr.app/fxb-layout/#howto");
                switch (inclresult)
                {
                    case DialogResult.Yes:
                        includelayout = true;
                        break;

                    case DialogResult.No:
                        includelayout = false;
                        break;

                    default:
                        return;
                }
            }
            if (!includelayout)
            {
                var currentAttributes = dockControlBuilder.GetAttributesSignature();
                if (currentAttributes != attributesChecksum)
                {
                    MessageBoxEx.Show(this, "Cannot save view, returned attributes must not be changed.\n\nExpected attributes:\n  " +
                        attributesChecksum.Replace("\n", "\n  ") + "\nCurrent attributes:\n  " + currentAttributes.Replace("\n", "\n  "),
                        "Cannot save view", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            var viewname = View?.GetAttributeValue<string>("name");
            var viewtype = View?.LogicalName;
            if (saveas)
            {
                var typeresult = MessageBoxEx.Show(this, "Save as a System View?\n\nYes - creating a new System View\nNo - creating a new Personal View", "Save View As",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 0,
                    "https://docs.microsoft.com/power-apps/maker/model-driven-apps/create-edit-views?WT.mc_id=DX-MVP-5002475#types-of-views");
                switch (typeresult)
                {
                    case DialogResult.Yes:
                        viewtype = "savedquery";
                        break;

                    case DialogResult.No:
                        viewtype = "userquery";
                        break;

                    default:
                        return;
                }
                var newviewname = Prompt.ShowDialog("Enter name for the new view", "Save View As", viewname);
                if (string.IsNullOrEmpty(newviewname))
                {
                    MessageBoxEx.Show(this, "No name for new view.", "Save View As", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (newviewname.ToLowerInvariant() == viewname?.ToLowerInvariant())
                {
                    MessageBoxEx.Show(this, "Enter a new name for this view.", "Save View As", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                viewname = newviewname;
            }
            if (viewtype == "savedquery")
            {
                if (MessageBoxEx.Show(this, "This will update and publish the saved query in Dataverse.\n\nConfirm!", "Confirm",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                }
            }
            var fetch = dockControlBuilder.GetFetchString(false, false);
            var layout = includelayout ? dockControlBuilder.LayoutXML.ToXMLString() : View?["layoutxml"].ToString();
            var newView = new Entity(viewtype);
            newView["fetchxml"] = fetch;
            newView["returnedtypecode"] = entityname;
            if (includelayout && !string.IsNullOrWhiteSpace(layout))
            {
                newView["layoutxml"] = layout;
            }
            if (saveas)
            {
                newView["name"] = viewname;
                newView["querytype"] = 0;
            }
            else
            {
                newView.Id = View?.Id ?? Guid.Empty;
                if (newView.Id.Equals(Guid.Empty))
                {
                    MessageBoxEx.Show(this, "Somehow missing the Id for the existing view.\nSorry. Try again, or reboot, or anything else...", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            var msg = newView.LogicalName == "savedquery" ? "Saving and publishing {0}..." : "Saving {0}...";
            WorkAsync(new WorkAsyncInfo(string.Format(msg, viewname),
                (worker, eventargs) =>
                {
                    if (newView.Id.Equals(Guid.Empty))
                    {
                        newView.Id = Service.Create(newView);
                        eventargs.Result = newView;
                    }
                    else
                    {
                        Service.Update(newView);
                    }
                    LogUse($"SaveView{(newView.Contains("layoutxml") ? "Layout" : "")}-{(newView.LogicalName == "savedquery" ? "S" : "P")}");
                    if (newView.LogicalName == "savedquery")
                    {
                        var pubRequest = new PublishXmlRequest();
                        pubRequest.ParameterXml = string.Format(
                            @"<importexportxml><entities><entity>{0}</entity></entities><nodes/><securityroles/><settings/><workflows/></importexportxml>",
                            newView["returnedtypecode"].ToString());
                        Execute(pubRequest);
                    }
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        ShowErrorDialog(completedargs.Error, "Save view");
                    }
                    else
                    {
                        if (completedargs.Result is Entity newview && saveas)
                        {
                            if (views != null)
                            {
                                entityname = newview["returnedtypecode"].ToString();
                                var viewsuffix = newview.LogicalName == "savedquery" ? "S" : "U";
                                if (!views.ContainsKey(entityname + "|" + viewsuffix))
                                {
                                    views.Add(entityname + "|" + viewsuffix, new List<Entity>());
                                }
                                views[entityname + "|" + viewsuffix].Add(newView);
                            }
                            View = newview;
                            if (newView.LogicalName == "savedquery")
                            {
                                MessageBoxEx.Show(this, $"{viewname} is created in the system.\n\nNote that it is now only in the Default solution!\nMake sure you go and add it to your own solutions.",
                                    "New View", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        dockControlBuilder.ClearChanged();
                    }
                }
            });
        }

        private void SetIntegrationsTo()
        {
            var pos = 2;
            foreach (var integrationto in OnlineSettings.Instance.IntegratedToTools)
            {
                var name = integrationto;
                Image image = Resources.icon_send;
                var tool = PluginManagerExtended.Instance.PluginsExt.Where(t => t.Metadata.Name == integrationto).FirstOrDefault();
                var enabled = tool != null;
                if (enabled)
                {
                    var bytes = Convert.FromBase64String(tool.Metadata.SmallImageBase64);
                    using var ms = new MemoryStream(bytes);
                    image = Image.FromStream(ms);
                }
                else
                {
                    name += " (not installed)";
                }
                var menu = new ToolStripMenuItem(name)
                {
                    Tag = integrationto,
                    Image = image,
                    Enabled = enabled
                };
                menu.Click += SendToTool_Click;
                tsbSend.DropDownItems.Insert(pos++, menu);
            }
        }

        private void SetupDockControls()
        {
            var dockFile = GetDockFileName();
            if (File.Exists(dockFile))
            {
                try
                {
                    dockContainer.LoadFromXml(dockFile, dockDeSerialization);
                }
                catch
                {
                }
            }
            if (dockControlBuilder == null ||
                dockControlBuilder.DockState == DockState.Hidden ||
                dockControlBuilder.DockState == DockState.Unknown)
            {   // Something fishy, treecontrol should always be visible
                ResetDockLayout();
            }
        }

        private void ShowContentControl(ref XmlContentControl control, ContentType contenttype, SaveFormat save, DockState state)
        {
            LogUse($"Show-{contenttype}");
            if (control?.IsDisposed != false)
            {
                control = new XmlContentControl(contenttype, save, this);
                control.Show(dockContainer, state);
            }
            else
            {
                control.EnsureVisible(dockContainer, state);
            }
        }

        private void ShowMetadataControl(ref MetadataControl control, DockState defaultstate)
        {
            LogUse($"Show-Metadata");
            if (control?.IsDisposed != false)
            {
                control = new MetadataControl(this);
                control.Show(dockContainer, defaultstate);
            }
            else
            {
                control.EnsureVisible(dockContainer, defaultstate);
            }
            UpdateLiveXML();
        }

        internal void ShowSelectSettings()
        {
            var result = ShowMetadataOptions.Show(this, ApplyFilteringSetting);
        }

        private void ApplyFilteringSetting(bool result)
        {
            solutionentities = null;
            solutionattributes = null;
            if (result)
            {
                LogUse("SaveSelect");
                SaveSetting();
                ApplySettings(false);
                dockControlBuilder.ApplyCurrentSettings();
                dockControlFetchXml?.ApplyCurrentSettings();
                EnableControls();
            }
        }

        private void ShowODataControl(ref ODataControl control, int version)
        {
            LogUse($"Show-OData{version}.0");
            if (control?.IsDisposed != false)
            {
                control = new ODataControl(this, version);
                control.Show(dockContainer, DockState.DockBottom);
            }
            else
            {
                control.EnsureVisible(dockContainer, DockState.DockBottom);
            }
            UpdateLiveXML();
        }

        private void ShowFlowListControl(ref FlowListControl control, DockState defaultstate)
        {
            LogUse($"Show-FlowList");
            if (control?.IsDisposed != false)
            {
                control = new FlowListControl(this);
                var defaultfloatsize = dockContainer.DefaultFloatWindowSize;
                dockContainer.DefaultFloatWindowSize = dockControlFlowList.Size;
                control.Show(dockContainer, defaultstate);
                dockContainer.DefaultFloatWindowSize = defaultfloatsize;
            }
            else
            {
                control.EnsureVisible(dockContainer, defaultstate);
            }
            UpdateLiveXML();
        }

        private void ShowResultControl(string content, ContentType contenttype, SaveFormat save, DockState defaultstate)
        {
            if (content.Length > 100000 &&
                MessageBoxEx.Show(this, "Huge result, this may take a while!\n" + content.Length.ToString() + " characters.\n\nContinue?", "Huge result",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }
            LogUse($"Show-{contenttype}");
            var resultControl = dockControlFetchResult;
            if (settings.Results.AlwaysNewWindow)
            {
                resultControl = new XmlContentControl(contenttype, save, this);
                resultpanecount++;
            }
            else if (resultControl?.IsDisposed != false)
            {
                resultControl = new XmlContentControl(contenttype, save, this);
                resultControl.Show(dockContainer, defaultstate);
                dockControlFetchResult = resultControl;
            }
            resultControl.SetContentType(contenttype);
            resultControl.SetFormat(save);
            if (settings.Results.AlwaysNewWindow)
            {
                resultControl.Text += $" ({resultpanecount})";
                resultControl.TabText = resultControl.Text;
            }
            resultControl.EnsureVisible(dockContainer, defaultstate);
            resultControl.UpdateXML(content);
        }

        internal void ShowLayoutXML()
        {
            ShowContentControl(ref dockControlLayoutXml, ContentType.LayoutXML, SaveFormat.None, settings.DockStates.LayoutXML);
        }

        internal void ShowAiChatControl(bool dontask = false)
        {
            if ((string.IsNullOrEmpty(settings.AiSettings?.Provider) ||
                 string.IsNullOrEmpty(settings.AiSettings?.Model)) &&
                 !dontask)
            {
                if (MessageBoxEx.Show((IWin32Window)this, "The AI Chat feature is missing some settings, please add them now.", "AI Chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    ShowSettings("tabAiChat");
                    ShowAiChatControl(true);
                }
            }
            if (!string.IsNullOrEmpty(settings.AiSettings?.Provider) &&
                !string.IsNullOrEmpty(settings.AiSettings?.Model))
            {
                LogUse("Show-AiChat");
                if (dockControlAiChat?.IsDisposed != false)
                {
                    dockControlAiChat = new AiChatControl(this, false);
                    dockControlAiChat.Show(dockContainer, DockState.DockRight);
                }
                else
                {
                    dockControlAiChat.EnsureVisible(dockContainer, DockState.DockRight);
                }
            }
        }

        internal static ResultOutput ResultItemToSettingResult(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 1: return ResultOutput.XML;
                case 2: return ResultOutput.JSON;
                case 3: return ResultOutput.JSONWebAPI;
                case 4: return ResultOutput.Raw;
                default: return ResultOutput.Grid;
            }
        }

        private void SetResultTypeMenu(ResultOutput resultOutput)
        {
            tsmiResultXml.Checked = resultOutput == ResultOutput.XML;
            tsmiResultJson.Checked = resultOutput == ResultOutput.JSON;
            tsmiResultWebApi.Checked = resultOutput == ResultOutput.JSONWebAPI;
            tsmiResultRaw.Checked = resultOutput == ResultOutput.Raw;
            tsmiResultGridView.Checked = !tsmiResultXml.Checked && !tsmiResultJson.Checked && !tsmiResultWebApi.Checked && !tsmiResultRaw.Checked;
            tsmiDisplayFriendly.Enabled = tsmiResultGridView.Checked;
            tsmiDisplayRaw.Enabled = tsmiResultGridView.Checked;
            tsmiDisplayBoth.Enabled = tsmiResultGridView.Checked;
        }

        private void tsmiResultType_Click(object sender = null, EventArgs e = null)
        {
            var result =
                sender is ToolStripMenuItem tsmi &&
                tsmi.Tag is string strtag &&
                int.TryParse(strtag, out var inttag) ? ResultItemToSettingResult(inttag) : ResultOutput.Grid;
            SetResultTypeMenu(result);
            if (settings.ExecuteOptions.ResultOutput != result)
            {
                settings.ExecuteOptions.ResultOutput = result;
                FetchResults();
            }
        }

        private void SetDisplayFriendlyRaw()
        {
            tsmiDisplayFriendly.Checked = settings.Results.Friendly;
            tsmiDisplayRaw.Checked = !tsmiDisplayFriendly.Checked;
            tsmiDisplayBoth.Checked = settings.Results.BothNames;
            dockControlGrid?.ApplySettingsToGrid();
        }

        private void tsmiDisplayType_Click(object sender = null, EventArgs e = null)
        {
            settings.Results.Friendly = sender == tsmiDisplayFriendly;
            SetDisplayFriendlyRaw();
        }

        private void tsmiDisplayBoth_Click(object sender, EventArgs e)
        {
            settings.Results.BothNames = !tsmiDisplayBoth.Checked;
            SetDisplayFriendlyRaw();
        }
    }
}