using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System;
using System.Collections.Generic;
using System.Linq;
using WeifenLuo.WinFormsUI.Docking;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class FXBSettings
    {
        private bool _useFriendlyNames;

        public bool UseFriendlyNames { get { return _useFriendlyNames; } set { _useFriendlyNames = value; FetchXmlBuilder.friendlyNames = value; } }
        public QueryOptions QueryOptions { get; set; } = new QueryOptions();
        public MetadataOptions Entity { get; set; } = new MetadataOptions();
        public MetadataOptions Attribute { get; set; } = new MetadataOptions();
        public ResultOptions Results { get; set; } = new ResultOptions();
        public bool? LogUsage { get; set; }
        public string CurrentVersion { get; set; }
        public string LastOpenedViewEntity { get; set; }
        public Guid LastOpenedViewId { get; set; }
        public bool DoNotPromptToSave { get; set; } = false;
        public DockStates DockStates { get; set; } = new DockStates();
        public ContentWindows ContentWindows { get; set; } = new ContentWindows();
        public bool OpenUncustomizableViews { get; set; } = false;
        public bool UseSQL4CDS { get; set; }
    }

    public class QueryOptions
    {
        internal static string DefaultNewQuery = "<fetch top=\"50\"><entity name=\"\"/></fetch>";
        public bool ShowQuickActions { get; set; } = true;
        public bool UseSingleQuotation { get; set; }
        public string NewQueryTemplate { get; set; } = DefaultNewQuery;
    }

    public class MetadataOptions
    {
        public bool All { get; set; } = true;
        public bool Managed { get; set; }
        public bool Unmanaged { get; set; }
        public bool Customizable { get; set; }
        public bool Uncustomizable { get; set; }
        public bool Custom { get; set; }
        public bool Standard { get; set; }
        public bool Intersect { get; set; }
        public bool OnlyValidAF { get; set; }
        public bool OnlyValidRead { get; set; }
    }

    public class ResultOptions
    {
        public bool Friendly { get; set; }
        public bool Id { get; set; }
        public bool Index { get; set; }
        public bool NullColumns { get; set; }
        public bool SysColumns { get; set; } = true;
        public bool LocalTime { get; set; }
        public bool CopyHeaders { get; set; } = true;
        public int ResultOption { get; set; }
        public int SerializeStyle { get; set; }
        public bool RetrieveAllPages { get; set; } = false;
        public bool AlwaysNewWindow { get; set; } = false;
    }

    public class DockStates
    {
        public DockState ResultView { get; set; } = DockState.Document;
        public DockState FetchResult { get; set; } = DockState.Document;
        public DockState FetchXML { get; set; } = DockState.Document;
        public DockState FetchXMLCs { get; set; } = DockState.DockRight;
        public DockState FetchXMLJs { get; set; } = DockState.DockRight;
        public DockState QueryExpression { get; set; } = DockState.DockRight;
        public DockState SQLQuery { get; set; } = DockState.DockRight;
        public DockState FlowList { get; set; } = DockState.Float;
    }

    public class ContentWindow
    {
        public bool LiveUpdate { get; set; } = false;
        public bool FormatExpanded { get; set; } = true;
        public bool ActionExpanded { get; set; } = true;
    }

    public class ContentWindows
    {
        public ContentWindow FetchResult { get; set; } = new ContentWindow();
        public ContentWindow FetchXmlWindow { get; set; } = new ContentWindow();
        public ContentWindow SQLWindow { get; set; } = new ContentWindow();
        public ContentWindow FetchXmlCsWindow { get; set; } = new ContentWindow();
        public ContentWindow FetchXmlJsWindow { get; set; } = new ContentWindow();
        public ContentWindow QueryExpressionWindow { get; set; } = new ContentWindow();

        internal ContentWindow GetContentWindow(ContentType type)
        {
            switch (type)
            {
                case ContentType.FetchXML_Result:
                case ContentType.Serialized_Result_JSON:
                case ContentType.Serialized_Result_XML:
                    return FetchResult;
                case ContentType.FetchXML:
                    return FetchXmlWindow;
                case ContentType.SQL_Query:
                    return SQLWindow;
                case ContentType.QueryExpression:
                    return QueryExpressionWindow;
                case ContentType.CSharp_Query:
                    return FetchXmlCsWindow;
                case ContentType.JavaScript_Query:
                    return FetchXmlJsWindow;
            }
            return new ContentWindow();
        }

        internal void SetContentWindow(ContentType type, ContentWindow windowSettings)
        {
            switch (type)
            {
                case ContentType.FetchXML:
                    FetchXmlWindow = windowSettings;
                    break;
                case ContentType.SQL_Query:
                    SQLWindow = windowSettings;
                    break;
                case ContentType.QueryExpression:
                    QueryExpressionWindow = windowSettings;
                    break;
                case ContentType.CSharp_Query:
                    FetchXmlCsWindow = windowSettings;
                    break;
                case ContentType.JavaScript_Query:
                    FetchXmlJsWindow = windowSettings;
                    break;
            }
        }
    }

    public class FXBConnectionSettings
    {
        public string FetchXML { get; set; } = string.Empty;
    }

    public class QueryRepository
    {
        public List<QueryDefinition> Queries { get; set; } = new List<QueryDefinition>();

        public void SortQueries()
        {
            Queries = Queries.OrderBy(q => q.Name).ToList();
        }
    }

    public class QueryDefinition
    {
        public string Name { get; set; }
        public string Fetch { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}