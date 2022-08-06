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
using Rappen.XTB.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using WeifenLuo.WinFormsUI.Docking;
using XrmToolBox;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder
{
    public partial class FetchXmlBuilder : PluginControlBase
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
        private XmlContentControl dockControlFetchXmlCs;
        private XmlContentControl dockControlFetchXmlJs;
        private ODataControl dockControlOData2;
        private ODataControl dockControlOData4;
        private FlowListControl dockControlFlowList;
        private XmlContentControl dockControlQExp;
        private XmlContentControl dockControlSQL;
        private MetadataControl dockControlMeta;
        private Entity dynml;
        private string fileName;
        private QueryRepository repository = new QueryRepository();
        private bool inSql4Cds;
        private bool bduexists;

        internal void EnableControls()
        {
            EnableControls(buttonsEnabled);
        }

        /// <summary>Enables or disables all buttons on the form</summary>
        /// <param name="enabled"></param>
        internal void EnableControls(bool enabled)
        {
            MethodInvoker mi = delegate
            {
                try
                {
                    tsbNew.Enabled = enabled;
                    tsbOpen.Enabled = enabled;
                    tsmiOpenFile.Enabled = enabled;
                    tsmiOpenView.Enabled = enabled && Service != null;
                    tsmiOpenML.Visible = enabled && Service != null && GetEntity("list") != null;
                    tsmiOpenCWP.Visible = enabled && Service != null && GetEntity("cint_feed") != null;
                    tsbReturnToCaller.Visible = CallerWantsResults();
                    tsbSave.Enabled = enabled;
                    tsmiSaveFile.Enabled = enabled && dockControlBuilder?.FetchChanged == true && !string.IsNullOrEmpty(FileName);
                    tsmiSaveFileAs.Enabled = enabled;
                    tsmiSaveView.Enabled = enabled && Service != null && View != null && View.IsCustomizable();
                    tsmiSaveViewAs.Enabled = tsmiSaveView.Enabled;
                    tsmiSaveML.Enabled = enabled && Service != null && DynML != null;
                    tsmiSaveCWP.Visible = enabled && Service != null && GetEntity("cint_feed") != null;
                    tsmiSaveCWP.Enabled = enabled && Service != null && dockControlBuilder?.FetchChanged == true && !string.IsNullOrEmpty(CWPFeed);
                    tsbView.Enabled = enabled;
                    tsbExecute.Enabled = enabled && Service != null;
                    tsmiSelect.Enabled = enabled && Service != null;
                    tsbAbort.Visible = settings.Results.RetrieveAllPages;
                    tsbBDU.Visible = bduexists && callerArgs?.SourcePlugin != "Bulk Data Updater";
                    tsbBDU.Enabled = enabled && (dockControlBuilder?.IsFetchAggregate() == false);
                    tsmiShowLayoutXML.Enabled = enabled && Service != null && (dockControlBuilder?.IsFetchAggregate() == false) && settings.Results.WorkWithLayout;
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
            if (!preventxmlupdate && dockControlFetchXml?.Visible == true)
            {
                dockControlFetchXml.UpdateXML(dockControlBuilder.GetFetchString(true, false));
            }
            if (!preventxmlupdate && dockControlLayoutXml?.Visible == true)
            {
                dockControlBuilder.LayoutXML?.MakeSureAllCellsExistForAttributes();
                dockControlLayoutXml.UpdateXML(dockControlBuilder.LayoutXML?.ToXML());
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
                dockControlFlowList.DisplayFlowList(GetOData(4));
            }
            if (dockControlQExp?.Visible == true && entities != null)
            {
                dockControlQExp.UpdateXML(GetQueryExpressionCode());
            }
            if (dockControlSQL?.Visible == true && entities != null)
            {
                var sql = GetSQLQuery(out var sql4cds);
                dockControlSQL.UpdateSQL(sql, sql4cds);
            }
            if (dockControlFetchXmlCs?.Visible == true)
            {
                dockControlFetchXmlCs.UpdateXML(GetCSharpCode());
            }
            if (dockControlFetchXmlJs?.Visible == true)
            {
                dockControlFetchXmlJs.UpdateXML(GetJavaScriptCode());
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

        internal static void HelpClick(object sender)
        {
            if (sender is Control control && control.Tag is string tag && tag.StartsWith("http"))
            {
                OpenURL(tag);
            }
        }

        internal void ShowMetadataControl()
        {
            ShowMetadataControl(ref dockControlMeta, DockState.DockRight);
        }

        private static string GetDockFileName()
        {
            return Path.Combine(Paths.SettingsPath, "Rappen.XTB.FXB_[DockPanels].xml");
        }

        private void CheckIntegrationTools()
        {
            bduexists = PluginManagerExtended.Instance.Plugins.Any(p =>
                p.Metadata.Name == "Bulk Data Updater" && new Version(p.Value.GetVersion()) > new Version(1, 2020, 12, 4));
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
            return null;
        }

        private string GetCSharpCode()
        {
            var cs = string.Empty;
            var fetch = dockControlBuilder.GetFetchString(true, false);
            try
            {
                cs = CSharpCodeGenerator.GetCSharpCode(fetch);
            }
            catch (Exception ex)
            {
                cs = "Failed to generate C# code.\n\n" + ex.Message;
            }
            return cs;
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
            Entity feed = GetCWPFeed(feedid);
            if (feed == null)
            {
                MessageBox.Show("Feed not found.", "Open CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (feed.Contains("cint_fetchxml"))
            {
                CWPFeed = feed.Contains("cint_id") ? feed["cint_id"].ToString() : feedid;
                dockControlBuilder.Init(feed["cint_fetchxml"].ToString(), null, "open CWP feed", false);
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
                dockControlBuilder.Init(fetchDoc.OuterXml, null, "open file", true);
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
                    dockControlBuilder.Init(DynML["query"].ToString(), null, "open marketing list", false);
                    LogUse("OpenML");
                }
                else
                {
                    if (MessageBox.Show("The selected marketing list does not contain any FetchXML.\nPlease select another one.", "Open Marketing List",
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
                        dockControlBuilder.Init(View["fetchxml"].ToString(), View["layoutxml"].ToString(), "open view", false);
                        attributesChecksum = dockControlBuilder.GetAttributesSignature();
                        LogUse("OpenView");
                    }
                    else
                    {
                        if (MessageBox.Show("The selected view does not contain any FetchXML.\nPlease select another one.", "Open View",
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
                    MessageBox.Show("Feed not saved.", "Save CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                CWPFeed = feedid;
            }
            Entity feed = GetCWPFeed(CWPFeed);
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
            MessageBox.Show("CWP Feed " + CWPFeed + " has been " + verb + "!", "Save CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveDockPanels()
        {
            var dockFile = GetDockFileName();
            dockContainer.SaveAsXml(dockFile);
        }

        private bool SaveFetchXML(bool prompt, bool silent)
        {
            bool result = false;
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
                    MessageBox.Show(this, "FetchXML saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                var result = MessageBox.Show("FetchXML has changed.\nSave changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
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
                    Entity newView = new Entity(DynML.LogicalName);
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
            var includelayout = saveas;
            if (!includelayout && settings.Results.WorkWithLayout)
            {
                var inclresult = MessageBox.Show("Include the layout of the view?", "Save View", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                    MessageBox.Show("Cannot save view, returned attributes must not be changed.\n\nExpected attributes:\n  " +
                        attributesChecksum.Replace("\n", "\n  ") + "\nCurrent attributes:\n  " + currentAttributes.Replace("\n", "\n  "),
                        "Cannot save view", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            var viewname = View["name"].ToString();
            var viewtype = View.LogicalName;
            if (saveas)
            {
                var typeresult = MessageBox.Show("Save the view as which type?\n\nSay Yes for System view\nSay No for Personal", "Save View As", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                    MessageBox.Show("No name for new view.", "Save View As", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (newviewname.ToLowerInvariant() == viewname.ToLowerInvariant())
                {
                    MessageBox.Show("Enter a new name for this view.", "Save View As", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                viewname = newviewname;
            }
            if (viewtype == "savedquery")
            {
                if (MessageBox.Show("This will update and publish the saved query in Dataverse.\n\nConfirm!", "Confirm",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                }
            }
            var fetch = dockControlBuilder.GetFetchString(false, false);
            var layout = includelayout ? dockControlBuilder.LayoutXML.ToXML() : View["layoutxml"].ToString();
            var entityname = View["returnedtypecode"].ToString();
            var newView = new Entity(viewtype);
            newView["fetchxml"] = fetch;
            if (includelayout)
            {
                newView["layoutxml"] = layout;
            }
            if (saveas)
            {
                newView["name"] = viewname;
                newView["returnedtypecode"] = View["returnedtypecode"];
                newView["querytype"] = 0;
            }
            else
            {
                newView.Id = View.Id;
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
                    LogUse("SaveView");
                    if (newView.LogicalName == "savedquery")
                    {
                        var pubRequest = new PublishXmlRequest();
                        pubRequest.ParameterXml = string.Format(
                            @"<importexportxml><entities><entity>{0}</entity></entities><nodes/><securityroles/><settings/><workflows/></importexportxml>",
                            View["returnedtypecode"].ToString());
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
                        if (completedargs.Result is Entity newview)
                        {
                            entityname = newview["returnedtypecode"].ToString();
                            var viewsuffix = viewtype == "savedquery" ? "S" : "U";
                            if (!views.ContainsKey(entityname + "|" + viewsuffix))
                            {
                                views.Add(entityname + "|" + viewsuffix, new List<Entity>());
                            }
                            views[entityname + "|" + viewsuffix].Add(newView);
                            View = newview;
                        }
                        dockControlBuilder.ClearChanged();
                    }
                }
            });
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
            UpdateLiveXML();
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
                MessageBox.Show("Huge result, this may take a while!\n" + content.Length.ToString() + " characters.\n\nContinue?", "Huge result",
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
    }
}