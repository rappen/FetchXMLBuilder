using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;
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
        public bool AddConditionToFilter { get; set; } = true;
        public bool UseSQL4CDS { get; set; }
        public bool UseLookup { get; set; } = true;
        public bool ShowHelpLinks { get; set; } = true;
        public bool ShowButtonTexts { get; set; } = true;
        public bool LinkEntityIdAttributesOnly { get; set; } = true;
        public XmlColors XmlColors { get; set; } = new XmlColors();
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
        public ResultOutput ResultOutput { get; set; }
        public bool RetrieveAllPages { get; set; } = false;
        public bool AlwaysNewWindow { get; set; } = false;
        public bool QuickFilter { get; set; } = false;
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

    public class XmlColors
    {
        [Browsable(false)]
        public string ElementColor { get { return Element.Name; } set { Element = Color.FromName(value); } }

        [Browsable(false)]
        public string ValueColor { get { return Value.Name; } set { Value = Color.FromName(value); } }

        [Browsable(false)]
        public string AttributeKeyColor { get { return AttributeKey.Name; } set { AttributeKey = Color.FromName(value); } }

        [Browsable(false)]
        public string AttributeValueColor { get { return AttributeValue.Name; } set { AttributeValue = Color.FromName(value); } }

        [Browsable(false)]
        public string CommentColor { get { return Comment.Name; } set { Comment = Color.FromName(value); } }

        [Browsable(false)]
        public string TagColor { get { return Tag.Name; } set { Tag = Color.FromName(value); } }

        [XmlIgnore()]
        public Color Element { get; set; } = Color.DarkRed;

        [XmlIgnore()]
        public Color Value { get; set; } = Color.Black;

        [XmlIgnore()]
        public Color AttributeKey { get; set; } = Color.Red;

        [XmlIgnore()]
        public Color AttributeValue { get; set; } = Color.Blue;

        [XmlIgnore()]
        public Color Comment { get; set; } = Color.Gray;

        [XmlIgnore()]
        [Browsable(false)]
        public Color Tag { get; set; } = Color.Blue;

        public void ApplyToControl(Scintilla viewer)
        {
            viewer.Styles[Style.Default].ForeColor = Value;
            viewer.Styles[Style.Xml.Attribute].ForeColor = AttributeKey;
            viewer.Styles[Style.Xml.DoubleString].ForeColor = AttributeValue;
            viewer.Styles[Style.Xml.SingleString].ForeColor = AttributeValue;
            viewer.Styles[Style.Xml.Comment].ForeColor = Comment;
            viewer.Styles[Style.Xml.Entity].ForeColor = Value;
            viewer.Styles[Style.Xml.Tag].ForeColor = Element;
            viewer.Styles[Style.Xml.TagEnd].ForeColor = Element;
        }
    }

    public enum ResultOutput
    {
        Grid = 0,
        XML = 1,
        JSON = 2,
        Raw = 3,
        JSONWebAPI = 4
    }
}