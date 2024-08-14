using Rappen.XTB.FetchXmlBuilder.DockControls;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;
using WeifenLuo.WinFormsUI.Docking;

namespace Rappen.XTB.FetchXmlBuilder.Settings
{
    public class FXBSettings
    {
        private bool _useFriendlyNames;

        public bool UseFriendlyNames
        {
            get { return _useFriendlyNames; }
            set
            {
                _useFriendlyNames = value;
                FetchXmlBuilder.friendlyNames = value;
            }
        }

        public bool UseFriendlyAndRawEntities { get; set; }

        public QueryOptions QueryOptions { get; set; } = new QueryOptions();
        public ResultOptions Results { get; set; } = new ResultOptions();
        public string CurrentVersion { get; set; }
        public string LastOpenedViewEntity { get; set; }
        public Guid LastOpenedViewId { get; set; }
        public bool DoNotPromptToSave { get; set; } = true;
        public DockStates DockStates { get; set; } = new DockStates();
        public ContentWindows ContentWindows { get; set; } = new ContentWindows();
        public bool OpenUncustomizableViews { get; set; } = false;
        public bool AddConditionToFilter { get; set; } = true;
        public bool UseSQL4CDS { get; set; }
        public bool UseDatePicker { get; set; } = true;
        public bool UseLookup { get; set; } = true;
        public bool ShowHelpLinks { get; set; } = true;
        public bool ShowButtonTexts { get; set; } = true;
        public bool LinkEntityIdAttributesOnly { get; set; } = true;
        public XmlColors XmlColors { get; set; } = new XmlColors();
        public bool ShowNodeType { get; set; } = false;
        public bool ShowValidation { get; set; } = true;
        public bool ShowValidationInfo { get; set; } = true;
        public bool ShowRepository { get; set; } = false;
        public bool ShowBDU { get; set; } = true;
        public bool ShowTreeviewAttributeTypes { get; set; } = false;
        public bool ShowAttributeTypes { get; set; } = true;
        public bool ShowOData2 { get; set; }
        public bool TryMetadataCache { get; set; } = true;
        public bool WaitUntilMetadataLoaded { get; set; } = false;
        public bool AlwaysShowAggregationProperties { get; set; } = false;
        public CodeGenerators CodeGenerators { get; set; } = new CodeGenerators();
    }

    public class ResultOptions
    {
        public bool Friendly { get; set; }
        public bool BothNames { get; set; }
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
        public bool PagingCookie { get; set; } = false;
        public bool ClickableLinks { get; set; } = true;
        public int MaxColumnWidth { get; set; } = 500;
        public bool WorkWithLayout { get; set; } = true;
    }

    public class DockStates
    {
        public DockState ResultView { get; set; } = DockState.Document;
        public DockState FetchResult { get; set; } = DockState.Document;
        public DockState FetchXML { get; set; } = DockState.Document;
        public DockState LayoutXML { get; set; } = DockState.Document;
        public DockState FetchXMLJs { get; set; } = DockState.DockRight;
        public DockState CSharp { get; set; } = DockState.DockRight;
        public DockState SQLQuery { get; set; } = DockState.DockRight;
        public DockState FlowList { get; set; } = DockState.Float;
        public DockState PowerPlatformCLI { get; set; } = DockState.DockRight;
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
        public ContentWindow LayoutXmlWindow { get; set; } = new ContentWindow();
        public ContentWindow SQLWindow { get; set; } = new ContentWindow();
        public ContentWindow FetchXmlCsWindow { get; set; } = new ContentWindow();
        public ContentWindow FetchXmlJsWindow { get; set; } = new ContentWindow();
        public ContentWindow CSharpWindow { get; set; } = new ContentWindow();
        public ContentWindow PowerPlatformCLI { get; set; } = new ContentWindow();

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

                case ContentType.LayoutXML:
                    return LayoutXmlWindow;

                case ContentType.SQL_Query:
                    return SQLWindow;

                case ContentType.CSharp_Code:
                    return CSharpWindow;

                case ContentType.JavaScript_Query:
                    return FetchXmlJsWindow;

                case ContentType.Power_Platform_CLI:
                    return PowerPlatformCLI;
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

                case ContentType.LayoutXML:
                    LayoutXmlWindow = windowSettings;
                    break;

                case ContentType.SQL_Query:
                    SQLWindow = windowSettings;
                    break;

                case ContentType.CSharp_Code:
                    CSharpWindow = windowSettings;
                    break;

                case ContentType.JavaScript_Query:
                    FetchXmlJsWindow = windowSettings;
                    break;

                case ContentType.Power_Platform_CLI:
                    PowerPlatformCLI = windowSettings;
                    break;
            }
        }
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
        public string ElementColor
        { get { return Element.Name; } set { Element = Color.FromName(value); } }

        [Browsable(false)]
        public string ValueColor
        { get { return Value.Name; } set { Value = Color.FromName(value); } }

        [Browsable(false)]
        public string AttributeKeyColor
        { get { return AttributeKey.Name; } set { AttributeKey = Color.FromName(value); } }

        [Browsable(false)]
        public string AttributeValueColor
        { get { return AttributeValue.Name; } set { AttributeValue = Color.FromName(value); } }

        [Browsable(false)]
        public string CommentColor
        { get { return Comment.Name; } set { Comment = Color.FromName(value); } }

        [Browsable(false)]
        public string TagColor
        { get { return Tag.Name; } set { Tag = Color.FromName(value); } }

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

    public class CodeGenerators
    {
        public QExStyleEnum QExStyle { get; set; } = QExStyleEnum.QueryExpression;
        public QExFlavorEnum QExFlavor { get; set; } = QExFlavorEnum.LateBound;
        public bool ObjectInitializer { get; set; } = false;
        public int Indents { get; set; } = 0;
        public bool IncludeComments { get; set; } = true;
        public bool FilterVariables { get; set; } = true;
        public string EBG_EntityLogicalNames { get; set; } = "EntityLogicalName";
        public string EBG_AttributeLogicalNameClass { get; set; } = "Fields.";
        public LCG.Settings LCG_Settings { get; set; } = new LCG.Settings();
    }

    public enum QExStyleEnum
    {
        QueryExpression,
        QueryByAttribute,
        OrganizationServiceContext,
        QueryExpressionFactory,
        FluentQueryExpression,
        FetchXML
    }

    public enum QExFlavorEnum
    {
        LateBound,
        EBGconstants,
        LCGconstants,
        EarlyBound
    }
}