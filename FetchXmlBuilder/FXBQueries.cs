using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Converters;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using Rappen.XTB.FetchXmlBuilder.Settings;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XRM.Helpers.Serialization;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;

namespace Rappen.XTB.FetchXmlBuilder
{
    public partial class FetchXmlBuilder
    {
        private string attributesChecksum = "";

        internal EntityCollection RetrieveMultiple(QueryBase query)
        {
            EntityCollection result = null;
            var start = DateTime.Now;
            try
            {
                result = Service.RetrieveMultiple(query);
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
                MessageBox.Show("Busy doing something...\n\nPlease wait until current transaction is done.");
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

        internal void QueryExpressionToFetchXml(string query)
        {
            working = true;
            LogUse("QueryExpressionToFetchXml");
            WorkAsync(new WorkAsyncInfo("Translating QueryExpression to FetchXML...",
                (eventargs) =>
                {
                    var start = DateTime.Now;
                    string fetchXml = QueryExpressionCodeGenerator.GetFetchXmlFromCSharpQueryExpression(query, Service);
                    var stop = DateTime.Now;
                    var duration = stop - start;
                    LogUse("QueryExpressionToFetchXml", false, null, duration.TotalMilliseconds);
                    SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Execution time: {duration}"));
                    eventargs.Result = fetchXml;
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        ShowErrorDialog(completedargs.Error, "Parse QueryExpression");
                    }
                    else if (completedargs.Result is string)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(completedargs.Result.ToString());
                        dockControlBuilder.Init(doc.OuterXml, "parse QueryExpression", true);
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
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(completedargs.Result.ToString());
                        ShowResultControl(doc.OuterXml, ContentType.FetchXML_Result, SaveFormat.XML, settings.DockStates.FetchResult);
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
            LogUse("RetrieveMultiple-" + settings.Results.ResultOutput.ToString());
            SendMessageToStatusBar(this, new StatusBarMessageEventArgs("Retrieving..."));
            tsbAbort.Enabled = true;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Executing FetchXML...",
                IsCancelable = true,
                Work = (worker, eventargs) =>
                {
                    QueryBase query = new FetchExpression(fetch);
                    var attributessignature = dockControlBuilder.GetAttributesSignature(null);
                    var start = DateTime.Now;
                    EntityCollection resultCollection = null;
                    EntityCollection tmpResult = null;
                    var page = 0;
                    do
                    {
                        if (worker.CancellationPending)
                        {
                            eventargs.Cancel = true;
                            break;
                        }
                        tmpResult = RetrieveMultiple(query);
                        if (resultCollection == null)
                        {
                            resultCollection = tmpResult;
                        }
                        else
                        {
                            resultCollection.Entities.AddRange(tmpResult.Entities);
                            resultCollection.MoreRecords = tmpResult.MoreRecords;
                            resultCollection.PagingCookie = tmpResult.PagingCookie;
                            resultCollection.TotalRecordCount = tmpResult.TotalRecordCount;
                            resultCollection.TotalRecordCountLimitExceeded = tmpResult.TotalRecordCountLimitExceeded;
                        }
                        if (settings.Results.RetrieveAllPages && tmpResult.MoreRecords)
                        {
                            if (query is QueryExpression qex)
                            {
                                qex.PageInfo.PageNumber++;
                                qex.PageInfo.PagingCookie = tmpResult.PagingCookie;
                            }
                            else if (query is FetchExpression fex && fex.Query is string pagefetch)
                            {
                                var pagedoc = new XmlDocument();
                                pagedoc.LoadXml(pagefetch);
                                if (pagedoc.SelectSingleNode("fetch") is XmlElement fetchnode)
                                {
                                    if (!int.TryParse(fetchnode.GetAttribute("page"), out int pageno))
                                    {
                                        pageno = 1;
                                    }
                                    pageno++;
                                    fetchnode.SetAttribute("page", pageno.ToString());
                                    fetchnode.SetAttribute("pagingcookie", tmpResult.PagingCookie);
                                    query = new FetchExpression(pagedoc.OuterXml);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Unable to retrieve more pages, unexpected query.", "Retrieve all pages", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                        page++;
                        var duration = DateTime.Now - start;
                        var pageinfo = page == 1 ? "first page" : $"{page} pages";
                        worker.ReportProgress(0, $"Retrieved {pageinfo} in {duration.TotalSeconds:F2} sec");
                        SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Retrieved {resultCollection.Entities.Count} records on {pageinfo} in {duration.TotalSeconds:F2} seconds"));
                    }
                    while (!eventargs.Cancel && settings.Results.RetrieveAllPages && (query is QueryExpression || query is FetchExpression) && tmpResult.MoreRecords);
                    LogUse("RetrieveMultiple", false, resultCollection?.Entities?.Count, (DateTime.Now - start).TotalMilliseconds);
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