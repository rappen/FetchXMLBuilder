using Cinteros.Xrm.FetchXmlBuilder.Converters;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XRM.Helpers.Serialization;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using Rappen.XTB.FetchXmlBuilder.Settings;
using Rappen.XTB.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;

namespace Rappen.XTB.FetchXmlBuilder
{
    public partial class FetchXmlBuilder
    {
        private string attributesChecksum = "";

        internal EntityCollection RetrieveMultiple(QueryBase query, bool allrecords = true)
        {
            EntityCollection result = null;
            var start = DateTime.Now;
            try
            {
                result = allrecords ? Service.RetrieveMultipleAll(query) : Service.RetrieveMultiple(query);
            }
            finally
            {
                var stop = DateTime.Now;
                var duration = stop - start;
                var qinfo = "";
                if (query is QueryExpression qe)
                {
                    qinfo = qe.EntityName;
                }
                else if (query is FetchExpression fe)
                {
                    qinfo = "fetchxml";
                }
                LogInfo($"Retrieving {qinfo}: {duration}");
            }
            return result;
        }

        internal OrganizationResponse Execute(OrganizationRequest request)
        {
            OrganizationResponse result = null;
            var start = DateTime.Now;
            try
            {
                result = Service.Execute(request);
            }
            finally
            {
                var stop = DateTime.Now;
                var duration = stop - start;
                LogInfo($"Execute {request.ToString().Replace("Microsoft.Crm.Sdk.Messages.", "")}: {duration}");
            }
            return result;
        }

        internal void FetchResults(string fetch = "")
        {
            if (!tsbExecute.Enabled)
            {
                return;
            }
            if (working)
            {
                MessageBox.Show("Busy doing something...\n\nPlease wait until current transaction is done.\n\nCoffee break perhaps?\nFor me too.\nClick the button 'Help' to join me!", "Waiting...", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0, "https://www.buymeacoffee.com/rappen");
                return;
            }
            if (string.IsNullOrEmpty(fetch))
            {
                fetch = dockControlBuilder.GetFetchString(true, true);
            }
            if (string.IsNullOrEmpty(fetch))
            {
                return;
            }
            switch (settings.Results.ResultOutput)
            {
                case ResultOutput.Grid:
                case ResultOutput.XML:
                case ResultOutput.JSON:
                case ResultOutput.JSONWebAPI:
                    RetrieveMultiple(fetch);
                    break;

                case ResultOutput.Raw:
                    ExecuteFetch(fetch);
                    break;

                default:
                    MessageBox.Show("Select valid output type under Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        internal void LoadViews(Action viewsLoaded)
        {
            WorkAsync(new WorkAsyncInfo("Loading views...",
            (eventargs) =>
            {
                EnableControls(false);
                if (views == null || views.Count == 0)
                {
                    if (Service == null)
                    {
                        throw new Exception("Need a connection to load views.");
                    }
                    var qexs = new QueryExpression("savedquery");
                    qexs.ColumnSet = new ColumnSet("name", "returnedtypecode", "fetchxml", "layoutxml", "iscustomizable");
                    qexs.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                    qexs.Criteria.AddCondition("fetchxml", ConditionOperator.NotNull);
                    if (!settings.OpenUncustomizableViews)
                    {
                        qexs.Criteria.AddCondition("iscustomizable", ConditionOperator.Equal, true);
                    }
                    qexs.AddOrder("name", OrderType.Ascending);
                    var sysviews = RetrieveMultiple(qexs);
                    foreach (var view in sysviews.Entities)
                    {
                        var entityname = view["returnedtypecode"].ToString();
                        if (!string.IsNullOrWhiteSpace(entityname) && GetEntity(entityname) != null)
                        {
                            if (views == null)
                            {
                                views = new Dictionary<string, List<Entity>>();
                            }
                            if (!views.ContainsKey(entityname + "|S"))
                            {
                                views.Add(entityname + "|S", new List<Entity>());
                            }
                            views[entityname + "|S"].Add(view);
                        }
                    }
                    var qexu = new QueryExpression("userquery");
                    qexu.ColumnSet = new ColumnSet("name", "returnedtypecode", "fetchxml", "layoutxml");
                    qexu.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                    qexu.AddOrder("name", OrderType.Ascending);
                    var userviews = RetrieveMultiple(qexu);
                    foreach (var view in userviews.Entities)
                    {
                        var entityname = view["returnedtypecode"].ToString();
                        if (!string.IsNullOrWhiteSpace(entityname) && GetEntity(entityname) != null)
                        {
                            if (views == null)
                            {
                                views = new Dictionary<string, List<Entity>>();
                            }
                            if (!views.ContainsKey(entityname + "|U"))
                            {
                                views.Add(entityname + "|U", new List<Entity>());
                            }
                            views[entityname + "|U"].Add(view);
                        }
                    }
                }
            })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    EnableControls(true);
                    if (completedargs.Error != null)
                    {
                        ShowErrorDialog(completedargs.Error, "Loading Views");
                    }
                    else
                    {
                        viewsLoaded();
                    }
                }
            });
        }

        internal Entity GetViewById(Guid id)
        {
            var cols = new ColumnSet("name", "returnedtypecode", "fetchxml", "layoutxml");
            try
            {
                return Service.Retrieve("savedquery", id, cols);
            }
            catch (FaultException<OrganizationServiceFault>)
            {
                try
                {
                    return Service.Retrieve("userquery", id, cols);
                }
                catch (FaultException<OrganizationServiceFault>)
                {
                    MessageBox.Show($"Cannot find either a system's or a personal's view with id {id}.", "Loading View", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    ShowErrorDialog(ex);
                }
            }
            return null;
        }

        internal void StringQueryExpressionToFetchXml(string query, QExStyleEnum style)
        {
            working = true;
            WorkAsync(new WorkAsyncInfo
            {
                Message = $"Translating {style} to FetchXML text...",
                Work = (worker, eventargs) =>
                {
                    var start = DateTime.Now;
                    var fetchXml = QExParse.GetFetchXmlFromCSharpQueryExpression(query, style, Service);
                    var duration = DateTime.Now - start;
                    LogUse($"{style}ToFetchXml", false, null, duration.TotalMilliseconds);
                    SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Execution time: {duration}"));
                    eventargs.Result = fetchXml;
                },
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        ShowErrorDialog(completedargs.Error, $"Parse {style}");
                    }
                    else if (completedargs.Result is string)
                    {
                        dockControlBuilder.Init(completedargs.Result.ToString().ToXml().OuterXml, null, false, $"parse {style}", true);
                    }
                    working = false;
                }
            });
        }

        private void ExecuteFetch(string fetch)
        {
            working = true;
            LogUse("ExecuteFetch");
            WorkAsync(new WorkAsyncInfo("Executing FetchXML...",
                (eventargs) =>
                {
                    //var fetchxml = GetFetchDocument().OuterXml;
                    var start = DateTime.Now;
                    var resp = (ExecuteFetchResponse)Execute(new ExecuteFetchRequest() { FetchXml = fetch });
                    var stop = DateTime.Now;
                    var duration = stop - start;
                    LogUse("ExecuteFetch", false, null, duration.TotalMilliseconds);
                    SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Execution time: {duration}"));
                    eventargs.Result = resp.FetchXmlResult;
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    working = false;
                    if (completedargs.Error != null)
                    {
                        ShowErrorDialog(completedargs.Error, "Execute Fetch", fetch);
                    }
                    else if (completedargs.Result is string)
                    {
                        ShowResultControl(completedargs.Result.ToString().ToXml().OuterXml, ContentType.FetchXML_Result, SaveFormat.XML, settings.DockStates.FetchResult);
                    }
                }
            });
        }

        private Entity GetCWPFeed(string feedid)
        {
            var qeFeed = new QueryExpression("cint_feed");
            qeFeed.ColumnSet.AddColumns("cint_id", "cint_fetchxml");
            qeFeed.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            qeFeed.Criteria.AddCondition("cint_id", ConditionOperator.Equal, feedid);
            var feeds = RetrieveMultiple(qeFeed);
            Entity feed = feeds.Entities.Count > 0 ? feeds.Entities[0] : null;
            return feed;
        }

        private void RetrieveMultiple(string fetch)
        {
            working = true;
            SendMessageToStatusBar(this, new StatusBarMessageEventArgs("Retrieving..."));
            tsbExecute.Enabled = false;
            tsbAbort.Enabled = true;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Executing FetchXML...",
                IsCancelable = true,
                Work = (worker, eventargs) =>
                {
                    QueryBase query = new FetchExpression(fetch);
                    var querysignature = dockControlBuilder.GetTreeChecksum(null);
                    var attributessignature = dockControlBuilder.GetAttributesSignature();
                    var start = DateTime.Now;
                    var resultCollection = settings.Results.RetrieveAllPages ?
                        Service.RetrieveMultipleAll(query, worker, eventargs, "Executing FetchXML...\nRecords: {retrieving}\nPage: {page}\nTime: {time}", false) :
                        Service.RetrieveMultiple(query);
                    LogUse($"RetrieveMultiple-{settings.Results.ResultOutput}", false, resultCollection?.Entities?.Count, (DateTime.Now - start).TotalMilliseconds);
                    if (settings.Results.ResultOutput == ResultOutput.JSON)
                    {
                        var json = EntityCollectionSerializer.ToJSONComplex(resultCollection, Formatting.Indented);
                        eventargs.Result = json;
                    }
                    else if (settings.Results.ResultOutput == ResultOutput.JSONWebAPI)
                    {
                        var json = EntityCollectionSerializer.ToJSONSimple(resultCollection, Formatting.Indented);
                        eventargs.Result = json;
                    }
                    else
                    {
                        eventargs.Result = new QueryInfo
                        {
                            Query = query,
                            QuerySignature = querysignature,
                            AttributesSignature = attributessignature,
                            Results = resultCollection
                        };
                    }
                },
                ProgressChanged = (changeargs) =>
                {
                    SetWorkingMessage(changeargs.UserState.ToString());
                },
                PostWorkCallBack = (completedargs) =>
                {
                    working = false;
                    tsbAbort.Enabled = false;
                    tsbExecute.Enabled = true;
                    Enabled = true;
                    Cursor = Cursors.Default;
                    if (completedargs.Error != null)
                    {
                        LogError("RetrieveMultiple error: {0}", completedargs.Error);
                        ShowErrorDialog(completedargs.Error, "Executing FetchXML", fetch);
                    }
                    else if (completedargs.Cancelled)
                    {
                        MessageBox.Show($"Manual abort.", "Execute", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (completedargs.Result is QueryInfo queryinfo)
                    {
                        switch (settings.Results.ResultOutput)
                        {
                            case ResultOutput.Grid:
                                if (settings.Results.AlwaysNewWindow)
                                {
                                    var newresults = new ResultGrid(this);
                                    resultpanecount++;
                                    newresults.Text = $"Results ({resultpanecount})";
                                    newresults.Show(dockContainer, settings.DockStates.ResultView);
                                    newresults.SetData(queryinfo);
                                }
                                else
                                {
                                    if (dockControlGrid?.IsDisposed != false)
                                    {
                                        dockControlGrid = new ResultGrid(this);
                                        dockControlGrid.Show(dockContainer, settings.DockStates.ResultView);
                                    }
                                    dockControlGrid.SetData(queryinfo);
                                    dockControlGrid.Activate();
                                }
                                break;

                            case ResultOutput.XML:
                                var serialized = EntityCollectionSerializer.Serialize(queryinfo.Results, SerializationStyle.Explicit);
                                ShowResultControl(serialized.OuterXml, ContentType.Serialized_Result_XML, SaveFormat.XML, settings.DockStates.FetchResult);
                                break;
                        }
                        queryinfo.Results.Entities.WarnIf50kReturned(queryinfo.Query);
                    }
                    else if ((settings.Results.ResultOutput == ResultOutput.JSON || settings.Results.ResultOutput == ResultOutput.JSONWebAPI) && completedargs.Result is string json)
                    {
                        ShowResultControl(json, ContentType.Serialized_Result_JSON, SaveFormat.JSON, settings.DockStates.FetchResult);
                    }
                }
            });
        }
    }
}