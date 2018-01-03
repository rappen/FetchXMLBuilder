using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System;
using System.Collections.Generic;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class FXBSettings
    {
        private static string filename = Assembly.GetExecutingAssembly().GetName().Name + ".Settings.xml";
        private bool _useFriendlyNames;

        public bool useFriendlyNames { get { return _useFriendlyNames; } set { _useFriendlyNames = value; FetchXmlBuilder.friendlyNames = value; } }
        public bool showEntitiesAll { get; set; }
        public bool showEntitiesManaged { get; set; }
        public bool showEntitiesUnmanaged { get; set; }
        public bool showEntitiesCustomizable { get; set; }
        public bool showEntitiesUncustomizable { get; set; }
        public bool showEntitiesCustom { get; set; }
        public bool showEntitiesStandard { get; set; }
        public bool showEntitiesIntersect { get; set; }
        public bool showEntitiesOnlyValidAF { get; set; }
        public bool showAttributesAll { get; set; }
        public bool showAttributesManaged { get; set; }
        public bool showAttributesUnmanaged { get; set; }
        public bool showAttributesCustomizable { get; set; }
        public bool showAttributesUncustomizable { get; set; }
        public bool showAttributesCustom { get; set; }
        public bool showAttributesStandard { get; set; }
        public bool showAttributesOnlyValidAF { get; set; }
        public bool showAttributesOnlyValidRead { get; set; }
        public int resultOption { get; set; }
        public int resultSerializeStyle { get; set; }
        public bool retrieveAllPages { get; set; } = false;
        public bool gridFriendly { get; set; }
        public bool gridId { get; set; }
        public bool gridIndex { get; set; }
        public bool gridCopyHeaders { get; set; } = true;
        public string fetchxml { get; set; }
        public bool? logUsage { get; set; }
        public string currentVersion { get; set; }
        public bool showQuickActions { get; set; }
        public bool useSingleQuotation { get; set; }
        public string lastOpenedViewEntity { get; set; }
        public Guid lastOpenedViewId { get; set; }
        public bool doNotPromptToSave { get; set; } = false;
        public bool resultsAlwaysNewWindow { get; set; } = false;
        public int treeHeight { get; set; } = -1;
        public DockStates dockStates { get; set; } = new DockStates();
        public ContentWindows ContentWindows { get; set; } = new ContentWindows();

        public FXBSettings()
        {
            showEntitiesAll = true;
            showAttributesAll = true;
            showQuickActions = true;
        }
    }

    public class DockStates
    {
        public DockState ResultView { get; set; } = DockState.Document;
        public DockState FetchXML { get; set; } = DockState.Document;
        public DockState FetchXMLCs { get; set; } = DockState.DockRight;
        public DockState FetchXMLJs { get; set; } = DockState.DockRight;
        public DockState QueryExpression { get; set; } = DockState.DockRight;
        public DockState SQLQuery { get; set; } = DockState.DockRight;
    }

    public class ContentWindow
    {
        public bool LiveUpdate { get; set; } = false;
        public bool FormatExpanded { get; set; } = true;
        public bool ActionExpanded { get; set; } = true;
    }

    public class ContentWindows
    {
        public ContentWindow FetchXmlWindow { get; set; } = new ContentWindow();
        public ContentWindow SQLWindow { get; set; } = new ContentWindow();
        public ContentWindow FetchXmlCsWindow { get; set; } = new ContentWindow();
        public ContentWindow FetchXmlJsWindow { get; set; } = new ContentWindow();
        public ContentWindow QueryExpressionWindow { get; set; } = new ContentWindow();

        internal ContentWindow GetContentWindow(ContentType type)
        {
            switch (type)
            {
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
}
