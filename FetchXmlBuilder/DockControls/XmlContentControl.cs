using MarkMpn.XmlSchemaAutocomplete.Scintilla;
using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Converters;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Settings;
using Rappen.XTB.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    public partial class XmlContentControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private string findtext = "";
        private FetchXmlBuilder fxb;
        private ContentType contenttype;
        private SaveFormat format;
        private string liveUpdateXml = "";

        private MarkMpn.XmlSchemaAutocomplete.Autocomplete<FetchType> _autocomplete;
        private bool _usedAutocomplete;

        internal XmlContentControl(FetchXmlBuilder caller) : this(ContentType.FetchXML, SaveFormat.XML, caller)
        {
        }

        internal XmlContentControl(ContentType contentType, SaveFormat saveFormat, FetchXmlBuilder caller)
        {
            InitializeComponent();
            this.PrepareGroupBoxExpanders();
            fxb = caller;
            cmbQExStyle.Items.AddRange(QExStyle.GetComboBoxItems());
            cmbQExFlavor.Items.AddRange(QExFlavor.GetComboBoxItems());
            SetContentType(contentType);
            SetFormat(saveFormat);
            UpdateButtons();
            if (contentType == ContentType.FetchXML || contentType == ContentType.LayoutXML)
            {
                InitIntellisense();
            }
        }

        protected override string GetPersistString()
        {
            return GetPersistString(contenttype);
        }

        internal static string GetPersistString(ContentType type)
        {
            return typeof(XmlContentControl).ToString() + "." + type.ToString();
        }

        internal void SetContentType(ContentType contentType)
        {
            contenttype = contentType;
            Text = contenttype.ToString().Replace("_", " ").Replace("CSharp", "C#");
            TabText = Text;
            var windowSettings = fxb.settings.ContentWindows.GetContentWindow(contenttype);
            var allowedit = contenttype == ContentType.FetchXML || contenttype == ContentType.LayoutXML;
            var allowsql = contenttype == ContentType.SQL_Query;
            chkLiveUpdate.Checked = allowedit && windowSettings.LiveUpdate;
            lblFormatExpander.GroupBoxSetState(tt, windowSettings.FormatExpanded);
            lblActionsExpander.GroupBoxSetState(tt, windowSettings.ActionExpanded);
            panActions.Visible = contenttype != ContentType.CSharp_Code;
            panLiveUpdate.Visible = allowedit;
            panOk.Visible = allowedit;
            panFormatting.Visible = allowedit;
            panExecute.Visible = allowedit && contenttype == ContentType.FetchXML;
            panQExOptions.Visible = contenttype == ContentType.CSharp_Code;
            panSQL4CDS.Visible = allowsql;
            panSQL4CDSInfo.Visible = allowsql;
            cmbQExStyle.SelectedItem = cmbQExStyle.Items.Cast<QExStyle>().FirstOrDefault(s => s.Tag == fxb.settings.CodeGenerators.QExStyle);
            cmbQExFlavor.SelectedItem = cmbQExFlavor.Items.Cast<QExFlavor>().FirstOrDefault(f => f.Tag == fxb.settings.CodeGenerators.QExFlavor);
            rbQExLineByLine.Checked = !fxb.settings.CodeGenerators.ObjectInitializer;
            rbQExObjectinitializer.Checked = fxb.settings.CodeGenerators.ObjectInitializer;
            chkQExComments.Checked = fxb.settings.CodeGenerators.IncludeComments;
            chkQExFilterVariables.Checked = fxb.settings.CodeGenerators.FilterVariables;

            switch (contentType)
            {
                case ContentType.FetchXML:
                case ContentType.LayoutXML:
                case ContentType.FetchXML_Result:
                case ContentType.Serialized_Result_XML:
                    txtXML.ConfigureForXml(fxb.settings);
                    break;

                case ContentType.SQL_Query:
                    txtXML.ConfigureForSQL();
                    break;

                case ContentType.CSharp_Code:
                    txtXML.ConfigureForCSharp();
                    break;

                case ContentType.JavaScript_Query:
                    txtXML.ConfigureForJavaScript();
                    break;

                case ContentType.Serialized_Result_JSON:
                    txtXML.ConfigureForJSON();
                    break;
            }
        }

        internal void SetFormat(SaveFormat saveFormat)
        {
            format = saveFormat;
            panSave.Visible = format != SaveFormat.None;
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            FormatXML(txtXML.Text, false);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            UpdateQueryBuild("manual edit", false);
        }

        private void FormatXML(string text, bool silent)
        {
            try
            {
                txtXML.FormatXML(text, fxb.settings);
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show(ex.Message, "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void XmlContentDisplayDialog_KeyDown(object sender, KeyEventArgs e)
        {
            findtext = FindTextHandler.HandleFindKeyPress(e, txtXML, findtext);
        }

        public void UpdateXML(string xmlString)
        {
            if (Visible)
            {
                if (txtXML.Lexer == ScintillaNET.Lexer.Xml)
                {
                    FormatXML(xmlString, true);
                }
                else
                {
                    txtXML.Text = xmlString;
                }
            }
            liveUpdateXml = xmlString;
        }

        public void UpdateSQL(string sql, bool sql4cds)
        {
            txtXML.Text = sql;
            panSQL4CDSInfo.Visible = !sql4cds;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            fxb.FetchResults(txtXML.Text);
        }

        private void btnParseQE_Click(object sender, EventArgs e)
        {
            fxb.QueryExpressionToFetchXml(txtXML.Text);
        }

        private void XmlContentDisplayDialog_Load(object sender, EventArgs e)
        {
            panActions.Visible = gbActions.Controls.Cast<Control>().Any(c => c.Visible);
            if (DialogResult == DialogResult.Cancel)
            {
                Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = $"Save {format}",
                Filter = $"{format} file (*.{format.ToString().ToLowerInvariant()})|*.{format.ToString().ToLowerInvariant()}"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, txtXML.Text);
                MessageBox.Show($"{format} saved to {sfd.FileName}");
            }
        }

        private void FormatAsXML()
        {
            if (string.IsNullOrEmpty(txtXML.Text.Trim()))
            {
                return;
            }
            if (!XMLIsPlain() && !XMLIsMini())
            {
                if (XMLIsHtml())
                {
                    txtXML.Text = HttpUtility.HtmlDecode(txtXML.Text.Trim());
                }
                else if (XMLIsEscaped())
                {
                    txtXML.Text = Uri.UnescapeDataString(txtXML.Text.Trim());
                }
                else
                {
                    if (MessageBox.Show("Unrecognized encoding, unsure what to do with it.\n" +
                        "Currently FXB can handle htmlencoded and urlescaped strings.\n\n" +
                        "Would you like to submit an issue to FetchXML Builder to be able to handle this?",
                        "Decode " + Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        FetchXmlBuilder.OpenURL("https://github.com/rappen/FetchXMLBuilder/issues/new");
                    }
                    return;
                }
            }
            FormatXML(txtXML.Text, false);
        }

        private void FormatAsHtml()
        {
            var response = MessageBox.Show("Strip spaces from encoded XML?", "Encode XML", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (response == DialogResult.Cancel)
            {
                UpdateButtons();
                return;
            }
            if (!XMLIsPlain())
            {
                FormatAsXML();
            }
            var xml = response == DialogResult.Yes ? GetCompactXml() : txtXML.Text;
            txtXML.Text = HttpUtility.HtmlEncode(xml);
        }

        private void FormatAsEsc()
        {
            if (!XMLIsPlain())
            {
                FormatAsXML();
            }
            txtXML.Text = Uri.EscapeDataString(GetCompactXml());
        }

        private void FormatAsMini()
        {
            if (!XMLIsPlain() && !XMLIsMini())
            {
                FormatAsXML();
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(txtXML.Text);
            var comments = doc.SelectNodes("//comment()");
            if (comments.Count > 0 && MessageBox.Show("Remove comments?", "Minify XML", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (XmlNode node in comments)
                {
                    node.ParentNode.RemoveChild(node);
                }
            }

            string xml;
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = new XmlFragmentWriter(stringWriter))
                {
                    xmlWriter.QuoteChar = fxb.settings.QueryOptions.UseSingleQuotation ? '\'' : '"';
                    doc.Save(xmlWriter);
                    xml = stringWriter.ToString();
                }
            }
            txtXML.Text = StripSpaces(xml);
        }

        private string GetCompactXml()
        {
            if (!XMLIsPlain())
            {
                FormatAsXML();
            }
            return StripSpaces(txtXML.Text);
        }

        private static string StripSpaces(string xml)
        {
            while (xml.Contains(" <")) xml = xml.Replace(" <", "<");
            while (xml.Contains(" >")) xml = xml.Replace(" >", ">");
            while (xml.Contains(" />")) xml = xml.Replace(" />", "/>");
            return xml.Trim();
        }

        private XmlStyle GetStyle()
        {
            if (XMLIsMini())
            {
                return XmlStyle.Mini;
            }
            if (XMLIsHtml())
            {
                return XmlStyle.Html;
            }
            if (XMLIsEscaped())
            {
                return XmlStyle.Esc;
            }
            return XmlStyle.Formatted;
        }

        private void SetStyle(XmlStyle style)
        {
            switch (style)
            {
                case XmlStyle.Formatted:
                    FormatAsXML();
                    break;

                case XmlStyle.Html:
                    FormatAsHtml();
                    break;

                case XmlStyle.Esc:
                    FormatAsEsc();
                    break;

                case XmlStyle.Mini:
                    FormatAsMini();
                    break;
            }
        }

        private bool XMLIsPlain()
        {
            var lines = txtXML.Text.Trim().Split('\n').Select(l => l.Trim()).ToList();
            return lines.Count > 1 && lines[0].StartsWith("<" + ContentTypeStart(contenttype));
        }

        private bool XMLIsMini()
        {
            var lines = txtXML.Text.Trim().Split('\n').Select(l => l.Trim()).ToList();
            return lines.Count == 1 && lines[0].StartsWith("<" + ContentTypeStart(contenttype));
        }

        private bool XMLIsHtml()
        {
            return txtXML.Text.Trim().ToLowerInvariant().StartsWith("&lt;" + ContentTypeStart(contenttype));
        }

        private bool XMLIsEscaped()
        {
            return txtXML.Text.Trim().ToLowerInvariant().StartsWith("%3c" + ContentTypeStart(contenttype));
        }

        private string ContentTypeStart(ContentType type)
        {
            switch (type)
            {
                case ContentType.FetchXML: return "fetch";
                case ContentType.LayoutXML: return "grid";
                default: return "dscxdsfdcvgfgwesdxdzsfdcbgf454";
            }
        }

        private void txtXML_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var plain = XMLIsPlain();
            rbFormatEsc.Checked = XMLIsEscaped();
            rbFormatHTML.Checked = XMLIsHtml();
            rbFormatMini.Checked = XMLIsMini();
            rbFormatXML.Checked = plain;
            btnFormat.Enabled = plain;
            btnExecute.Enabled = plain && !chkLiveUpdate.Checked;
            btnOk.Enabled = !chkLiveUpdate.Checked;
        }

        private void XmlContentDisplayDialog_DockStateChanged(object sender, EventArgs e)
        {
            if (DockState != WeifenLuo.WinFormsUI.Docking.DockState.Unknown &&
                DockState != WeifenLuo.WinFormsUI.Docking.DockState.Hidden)
            {
                switch (contenttype)
                {
                    case ContentType.FetchXML_Result:
                    case ContentType.Serialized_Result_JSON:
                    case ContentType.Serialized_Result_XML:
                        fxb.settings.DockStates.FetchResult = DockState;
                        break;

                    case ContentType.FetchXML:
                        fxb.settings.DockStates.FetchXML = DockState;
                        break;

                    case ContentType.LayoutXML:
                        fxb.settings.DockStates.LayoutXML = DockState;
                        break;

                    case ContentType.JavaScript_Query:
                        fxb.settings.DockStates.FetchXMLJs = DockState;
                        break;

                    case ContentType.CSharp_Code:
                        fxb.settings.DockStates.CSharp = DockState;
                        break;

                    case ContentType.SQL_Query:
                        fxb.settings.DockStates.SQLQuery = DockState;
                        break;
                }
            }
        }

        private void rbFormatXML_Click(object sender, EventArgs e)
        {
            FormatAsXML();
        }

        private void rbFormatHTML_Click(object sender, EventArgs e)
        {
            FormatAsHtml();
        }

        private void rbFormatEsc_Click(object sender, EventArgs e)
        {
            FormatAsEsc();
        }

        private void rbFormatMini_Click(object sender, EventArgs e)
        {
            FormatAsMini();
        }

        private void chkLiveUpdate_CheckedChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void XmlContentDisplayDialog_VisibleChanged(object sender, EventArgs e)
        {
            fxb.UpdateLiveXML();
        }

        private void llGroupBoxExpander_Clicked(object sender, EventArgs e)
        {
            (sender as Label)?.GroupBoxSetState(tt);
        }

        private void XmlContentDisplayDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            var windowSettings = new ContentWindow
            {
                LiveUpdate = chkLiveUpdate.Checked,
                FormatExpanded = gbFormatting.IsExpanded(),
                ActionExpanded = gbActions.IsExpanded()
            };
            fxb.settings.ContentWindows.SetContentWindow(contenttype, windowSettings);
        }

        private void btnSQL4CDS_Click(object sender, EventArgs e)
        {
            fxb.EditInSQL4CDS();
        }

        internal void ApplyCurrentSettings()
        {
            fxb.settings.XmlColors.ApplyToControl(txtXML);
        }

        private void txtXML_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 32)
            {
                // Prevent control characters from getting inserted into the text buffer
                e.Handled = true;
                return;
            }
        }

        private void InitIntellisense()
        {
            _autocomplete = new MarkMpn.XmlSchemaAutocomplete.Autocomplete<FetchType>();

            // Add descriptions for the elements and attributes
            _autocomplete.AddTypeDescription<FetchType>("FetchXML Query", "Defines query-level options.");
            _autocomplete.AddMemberDescription<FetchType>(nameof(FetchType.aggregate), "Aggregate Query", "Indicates if the query can use aggregate functions (group results by specific attributes and produce summary results) or not (generates a simple list of records).");
            _autocomplete.AddMemberDescription<FetchType>(nameof(FetchType.count), "Page Size", "The maximum number of records to return in each page (default 5000).");
            _autocomplete.AddMemberDescription<FetchType>(nameof(FetchType.distinct), "Distinct", "Indicates if only unique rows should be produced and any duplicates eliminated from the results.");
            _autocomplete.AddMemberDescription<FetchType>(nameof(FetchType.nolock), "No Lock", "Indicates if the query should bypass any locks from other queries. This may lead to inconsistent results.");
            _autocomplete.AddMemberDescription<FetchType>(nameof(FetchType.page), "Page Number", "Indicates which page of results to return (default 1).");
            _autocomplete.AddMemberDescription<FetchType>(nameof(FetchType.pagingcookie), "Paging Cookie", "The paging cookie returned with the previous page of results. Supplying this value makes it more efficient to retrieve the next page.");
            _autocomplete.AddMemberDescription<FetchType>(nameof(FetchType.returntotalrecordcount), "Return Total Record Count", "Indicates if the total number of possible results should be returned along with this page of results.");
            _autocomplete.AddMemberDescription<FetchType>(nameof(FetchType.top), "Top Count", "The maximum number of records to return. No further pages of data will be returned.");

            _autocomplete.AddTypeDescription<FetchEntityType>("Main Entity", "Gives the name of the entity type the query will return.");
            _autocomplete.AddMemberDescription<FetchEntityType>(nameof(FetchEntityType.enableprefiltering), "Enable Prefiltering", "If this query is being used in an SSRS report, indicates if the report can be pre-filtered by the user selecting the records to run it on.");
            _autocomplete.AddMemberDescription<FetchEntityType>(nameof(FetchEntityType.name), "Entity Name", "The name of the entity type the query will return.");
            _autocomplete.AddMemberDescription<FetchEntityType>(nameof(FetchEntityType.prefilterparametername), "Prefilter Parameter Name", "If prefiltering is enabled, this gives the name of the parameter that will be used for the filtering.");

            _autocomplete.AddTypeDescription<FetchAttributeType>("Attribute", "Includes an attribute in the query results.");
            _autocomplete.AddMemberDescription<FetchAttributeType>(nameof(FetchAttributeType.aggregate), "Aggregate Function", "The name of the aggregate function that should be applied to summarize the values in this attribute.");
            _autocomplete.AddMemberDescription<FetchAttributeType>(nameof(FetchAttributeType.alias), "Alias Name", "The alias name to apply to this attribute. Only required for aggregate queries.");
            _autocomplete.AddMemberDescription<FetchAttributeType>(nameof(FetchAttributeType.dategrouping), "Date Grouping", "The part of date/time values that should be used when grouping by this attribute.");
            _autocomplete.AddMemberDescription<FetchAttributeType>(nameof(FetchAttributeType.distinct), "Distinct Aggregates", "Indicates if duplicated values in this attribute should only be considered once when calculating aggregated results.");
            _autocomplete.AddMemberDescription<FetchAttributeType>(nameof(FetchAttributeType.groupby), "Grouping", "Indicates that the results should be grouped by this attribute. Only applies to aggregate queries.");
            _autocomplete.AddMemberDescription<FetchAttributeType>(nameof(FetchAttributeType.name), "Attribute Name", "The name of the attribute to include.");

            _autocomplete.AddTypeDescription<allattributes>("All Attributes", "Includes all attributes from the entity in the query results.");

            _autocomplete.AddTypeDescription<FetchLinkEntityType>("Link Entity", "Joins the entity to another entity");
            _autocomplete.AddMemberDescription<FetchLinkEntityType>(nameof(FetchLinkEntityType.alias), "Alias Name", "The alias name to apply to this linked entity.");
            _autocomplete.AddMemberDescription<FetchLinkEntityType>(nameof(FetchLinkEntityType.enableprefiltering), "Enable Prefiltering", "If this query is being used in an SSRS report, indicates if the report can be pre-filtered by the user selecting the records to run it on.");
            _autocomplete.AddMemberDescription<FetchLinkEntityType>(nameof(FetchLinkEntityType.from), "Join From Attribute", "The name of the attribute on this entity that should be used in the join.");
            _autocomplete.AddMemberDescription<FetchLinkEntityType>(nameof(FetchLinkEntityType.intersect), "Is Intersect", "Indicates if this linked entity is a many-to-many join intersect entity. Setting this hides the link from the Advanced Find editor.");
            _autocomplete.AddMemberDescription<FetchLinkEntityType>(nameof(FetchLinkEntityType.linktype), "Join Type", "The type of join to apply.");
            _autocomplete.AddMemberDescription<FetchLinkEntityType>(nameof(FetchLinkEntityType.name), "Entity Name", "The name of the entity to join to.");
            _autocomplete.AddMemberDescription<FetchLinkEntityType>(nameof(FetchLinkEntityType.prefilterparametername), "Prefilter Parameter Name", "If prefiltering is enabled, this gives the name of the parameter that will be used for the filtering.");
            _autocomplete.AddMemberDescription<FetchLinkEntityType>(nameof(FetchLinkEntityType.to), "Join To Attribute", "The name of the attribute in the parent entity that should be used in the join.");
            _autocomplete.AddMemberDescription<FetchLinkEntityType>(nameof(FetchLinkEntityType.visible), "Is Visible", "Indicates if this linked entity should be visible in the Advanced Find editor.");

            _autocomplete.AddTypeDescription<filter>("Filter", "Applies a filter to the query results using multiple conditions or sub-filters.");
            _autocomplete.AddMemberDescription<filter>(nameof(filter.isquickfindfields), "Is Quick Find", "If this FetchXML is being used in a Quick Find view, indicates that the quick-find filter conditions should be added to this filter.");
            _autocomplete.AddMemberDescription<filter>(nameof(filter.type), "Filter Operator", "Indicates if the conditions and sub-filters in this filter should be combined with a logical AND or OR operator.");

            _autocomplete.AddTypeDescription<condition>("Condition", "Filters the query results based on the value of a specific attribute");
            _autocomplete.AddMemberDescription<condition>(nameof(condition.alias), "Alias Name", "The name of the aliased attribute that this condition applies to.");
            _autocomplete.AddMemberDescription<condition>(nameof(condition.attribute), "Attribute Name", "The name of the attribute that this condition applies to.");
            _autocomplete.AddMemberDescription<condition>(nameof(condition.entityname), "Entity Name", "The name of the linked entity that the attribute to filter on is in.");
            _autocomplete.AddMemberDescription<condition>(nameof(condition.@operator), "Operator", "The type of filter condition to apply to this attribute.");
            _autocomplete.AddMemberDescription<condition>(nameof(condition.uihidden), "Is Hidden", "Indicates if the condition should be hidden from the Advanced Find view.");
            _autocomplete.AddMemberDescription<condition>(nameof(condition.uiname), "Lookup Value Name", "When filtering on a lookup value, gives the name of the associated record to show in the Advanced Find view.");
            _autocomplete.AddMemberDescription<condition>(nameof(condition.uitype), "Lookup Value Type", "When filtering on a lookup value, gives the type of the associated record to show in the Advanced Find view.");
            _autocomplete.AddMemberDescription<condition>(nameof(condition.value), "Value", "The value to compare the records against to check if they should be included in the results.");
            _autocomplete.AddMemberDescription<condition>(nameof(condition.valueof), "Compare To Attribute", "The name of another attribute to compare the first attribute to.");

            _autocomplete.AddTypeDescription<conditionValue>("Condition Value", "Specifies one of a list of possible values to use in an \"in\" or \"not-in\" condition");
            _autocomplete.AddMemberDescription<conditionValue>(nameof(conditionValue.uiname), "Lookup Value Name", "When filtering on a lookup value, gives the name of the associated record to show in the Advanced Find view.");
            _autocomplete.AddMemberDescription<conditionValue>(nameof(conditionValue.uitype), "Lookup Value Type", "When filtering on a lookup value, gives the type of the associated record to show in the Advanced Find view.");

            _autocomplete.AddTypeDescription<FetchOrderType>("Sort Order", "Sorts the query results based on a specific attribute");
            _autocomplete.AddMemberDescription<FetchOrderType>(nameof(FetchOrderType.alias), "Alias Name", "The name of the aliased attribute to sort the results by.");
            _autocomplete.AddMemberDescription<FetchOrderType>(nameof(FetchOrderType.attribute), "Attribute Name", "The name of the attribute to sort the results by.");
            _autocomplete.AddMemberDescription<FetchOrderType>(nameof(FetchOrderType.descending), "Descending Order", "Indicates if the results should be sorted in descending order by this attribute.");

            _autocomplete.AutocompleteAttributeValue += AutocompleteAttributeValue;

            var helper = new ScintillaXmlHelper(txtXML, _autocomplete);
            helper.Menu.ImageList = autocompleteImageList;
            helper.Menu.Opening += (sender, e) =>
            {
                if (_usedAutocomplete)
                    return;

                // Log this only once per instance, not per keypress
                fxb.LogUse("Autocomplete");
                _usedAutocomplete = true;
            };
            helper.Attach();
        }

        private void AutocompleteAttributeValue(object sender, MarkMpn.XmlSchemaAutocomplete.AutocompleteAttributeValueEventArgs e)
        {
            // Autocomplete entity names for <entity> and <link-entity> elements
            if ((e.Element.Name == "entity" || e.Element.Name == "link-entity") && e.Attribute.Name == "name")
            {
                e.Suggestions.AddRange(fxb.GetDisplayEntities().Select(entity => new EntityMetadataSuggestion(entity, fxb.settings.UseFriendlyNames)));
            }

            // Autocomplete prefiltering parameter name
            if ((e.Element.Name == "entity" || e.Element.Name == "link-entity") && e.Attribute.Name == "prefilterparametername")
            {
                if (fxb.GetEntity(e.Element.GetAttribute("name")) is EntityMetadata entity)
                {
                    e.Suggestions.Add(new MarkMpn.XmlSchemaAutocomplete.AutocompleteAttributeValueSuggestion { Value = $"CRM_{entity.ReportViewName}" });
                }
            }

            // Autocomplete attribute names for <attribute>, <condition> and <order> elements
            if (((e.Element.Name == "attribute" && e.Attribute.Name == "name") || ((e.Element.Name == "condition" || e.Element.Name == "order") && e.Attribute.Name == "attribute")) ||
                (e.Element.Name == "condition" && e.Attribute.Name == "valueof"))
            {
                var entityNode = (XmlElement)e.Element.ParentNode;
                while (entityNode != null && entityNode.Name != "entity" && entityNode.Name != "link-entity")
                    entityNode = (XmlElement)entityNode.ParentNode;

                if (entityNode != null && (entityNode.Name == "entity" || entityNode.Name == "link-entity"))
                {
                    if (TryGetAttributes(entityNode.GetAttribute("name"), out _, out var attrs))
                    {
                        var attributes = attrs.Where(attr => attr.IsValidForRead != false && attr.AttributeOf == null);

                        if (e.Element.Name == "condition" && e.Attribute.Name == "valueof")
                        {
                            // Only suggest attributes of the same type
                            var attr = attrs.SingleOrDefault(a => a.LogicalName == e.Element.GetAttribute("attribute"));
                            if (attr != null)
                                attributes = attributes.Where(a => IsCompatibleType(a, attr));
                        }

                        e.Suggestions.AddRange(attributes.OrderBy(attr => attr.LogicalName).Select(attr => new AttributeMetadataSuggestion(attr, fxb.settings.UseFriendlyNames)));
                    }
                }
            }

            // Autocomplete attribute names for <link-entity> elements
            if (e.Element.Name == "link-entity" && (e.Attribute.Name == "from" || e.Attribute.Name == "to"))
            {
                var entityNode = e.Attribute.Name == "from" ? e.Element : (XmlElement)e.Element.ParentNode;
                var otherEntityNode = e.Attribute.Name == "from" ? (XmlElement)e.Element.ParentNode : e.Element;

                if (entityNode != null && TryGetAttributes(entityNode.GetAttribute("name"), out var entity, out var attrs))
                {
                    var attributes = attrs.Where(attr => attr.IsValidForRead != false && attr.AttributeOf == null);

                    if (otherEntityNode != null && TryGetAttributes(otherEntityNode.GetAttribute("name"), out var otherEntity, out var otherAttrs))
                    {
                        // Only suggest attributes of the same type. If the other attribute isn't specified yet, offer the
                        // primary key and foreign key attributes
                        // TODO: Offer all attributes if user is entering something other than the primary or foreign keys
                        // Maybe always offer all attributes but highlight the best ones?
                        var attr = otherAttrs.SingleOrDefault(a => a.LogicalName == e.Element.GetAttribute(e.Attribute.Name == "from" ? "to" : "from"));
                        if (attr != null)
                            attributes = attributes.Where(a => IsCompatibleType(a, attr));
                        else
                            attributes = attributes.Where(a => a.LogicalName == entity.PrimaryIdAttribute || a is LookupAttributeMetadata lookup && lookup.Targets.Contains(otherEntity.LogicalName));
                    }

                    e.Suggestions.AddRange(attributes.OrderBy(attr => attr.LogicalName).Select(attr => new AttributeMetadataSuggestion(attr, fxb.settings.UseFriendlyNames)));
                }
            }

            // Autocomplete alias names for <condition> and <order> elements
            if ((e.Element.Name == "condition" || e.Element.Name == "order") && e.Attribute.Name == "alias")
            {
                var aliases = e.Element.OwnerDocument.SelectNodes("//attribute/@alias").Cast<XmlAttribute>().Select(alias => alias.Value).Distinct();
                e.Suggestions.AddRange(aliases.Select(alias => new MarkMpn.XmlSchemaAutocomplete.AutocompleteAttributeValueSuggestion { Value = alias }));
            }

            // Autocomplete link types
            if (e.Element.Name == "link-entity" && e.Attribute.Name == "link-type")
            {
                e.Suggestions.Add(new MarkMpn.XmlSchemaAutocomplete.AutocompleteAttributeValueSuggestion { Value = "inner" });
                e.Suggestions.Add(new MarkMpn.XmlSchemaAutocomplete.AutocompleteAttributeValueSuggestion { Value = "outer" });
            }

            // Autocomplete known values for <condition> elements
            if (e.Element.Name == "condition" && e.Attribute.Name == "value")
            {
                var entityNode = (XmlElement)e.Element.ParentNode;
                while (entityNode != null && entityNode.Name != "entity" && entityNode.Name != "link-entity")
                    entityNode = (XmlElement)entityNode.ParentNode;

                if (entityNode != null && (entityNode.Name == "entity" || entityNode.Name == "link-entity"))
                {
                    if (TryGetAttributes(entityNode.GetAttribute("name"), out _, out var attrs))
                    {
                        var attr = attrs.SingleOrDefault(a => a.LogicalName == e.Element.GetAttribute("attribute"));

                        if (attr is EnumAttributeMetadata enumAttr)
                        {
                            e.Suggestions.AddRange(enumAttr.OptionSet.Options.Select(opt => new MarkMpn.XmlSchemaAutocomplete.AutocompleteAttributeValueSuggestion { Value = opt.Value.ToString(), DisplayName = $"{opt.Value} - {opt.Label?.UserLocalizedLabel?.Label}" }));
                        }
                    }
                }
            }
        }

        private bool TryGetAttributes(string entityName, out EntityMetadata entity, out AttributeMetadata[] attributes)
        {
            if (fxb.Service != null)
            {
                entity = fxb.GetEntity(entityName);
                if (entity?.Attributes != null)
                {
                    attributes = fxb.GetDisplayAttributes(entityName).ToArray();
                    return true;
                }

                if (fxb.NeedToLoadEntity(entityName))
                {
                    // NOTE: Don't do:
                    // fxb.LoadEntityDetails(entityName, null);
                    // This causes a refresh of the XML from the current tree view, overwriting our changes as we type!
                    // We also don't really want the "Loading..." popup to show while we're typing, so just start the metadata loading in a background task directly
                    Task.Run(() =>
                    {
                        var resp = MetadataExtensions.LoadEntityDetails(fxb.Service, entityName, fxb.ConnectionDetail.OrganizationMajorVersion, fxb.ConnectionDetail.OrganizationMinorVersion);

                        if (resp.EntityMetadata.Count == 1)
                        {
                            if (fxb.entities == null)
                            {
                                fxb.entities = new List<EntityMetadata>();
                            }
                            else if (fxb.GetEntity(entityName) is EntityMetadata oldentity)
                            {
                                fxb.entities.Remove(oldentity);
                            }
                            fxb.entities.Add(resp.EntityMetadata[0]);
                        }
                    });
                }
            }

            entity = null;
            attributes = null;
            return false;
        }

        private bool IsCompatibleType(AttributeMetadata a, AttributeMetadata attr)
        {
            // For optionset attributes, check they are the same optionset type
            if (a is EnumAttributeMetadata optionsetX && attr is EnumAttributeMetadata optionsetY)
                return optionsetX.OptionSet.IsGlobal == true && optionsetY.OptionSet.IsGlobal == true && optionsetX.OptionSet.Name == optionsetY.OptionSet.Name;

            // For lookup/customer/owner/uniqueid attributes, check they can refer to the same entity types
            var lookupX = a as LookupAttributeMetadata;
            var lookupY = attr as LookupAttributeMetadata;

            if (a.IsPrimaryId == true)
                lookupX = new LookupAttributeMetadata { Targets = new[] { a.EntityLogicalName } };

            if (attr.IsPrimaryId == true)
                lookupY = new LookupAttributeMetadata { Targets = new[] { attr.EntityLogicalName } };

            if (lookupX != null && lookupY != null)
                return lookupX.Targets.Intersect(lookupY.Targets).Any();

            // For everything else, just check they're the same overall type
            if (a.AttributeType == attr.AttributeType)
                return true;

            return false;
        }

        private void XmlContentControl_Enter(object sender, EventArgs e)
        {
            fxb.historyisavailable = false;
            fxb.EnableDisableHistoryButtons();
        }

        private void tmLiveUpdate_Tick(object sender, EventArgs e)
        {
            tmLiveUpdate.Stop();
            if (chkLiveUpdate.Checked)
            {
                UpdateQueryBuild("live edit", true);
            }
        }

        private void UpdateQueryBuild(string action, bool live)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(txtXML.Text);
                if (doc.OuterXml != liveUpdateXml)
                {
                    switch (contenttype)
                    {
                        case ContentType.FetchXML:
                            fxb.dockControlBuilder.ParseXML(txtXML.Text, false);
                            fxb.historyMgr.RecordHistory(action, txtXML.Text);
                            break;

                        case ContentType.LayoutXML:
                            fxb.dockControlBuilder.SetLayoutFromXML(txtXML.Text);
                            fxb.dockControlGrid?.SetLayoutToGrid();
                            break;
                    }
                }
                liveUpdateXml = doc.OuterXml;
            }
            catch (Exception)
            {
                if (contenttype == ContentType.LayoutXML && !live)
                {
                    fxb.dockControlBuilder.SetLayoutFromXML(null);
                    fxb.dockControlGrid?.SetLayoutToGrid();
                }
            }
            fxb.UpdateLiveXML(live);
        }

        private void txtXML_KeyUp(object sender, KeyEventArgs e)
        {
            if (chkLiveUpdate.Checked && Visible && !e.Handled)
            {
                tmLiveUpdate.Stop();
                tmLiveUpdate.Start();
            }
        }

        private void rbQExStyle_Click(object sender, EventArgs e)
        {
            //            if (rbQExQExFactory.Checked)
            //            {
            //                MessageBox.Show(@"This feature is not yet implemented... #sorry

            //Do you like that idea?
            //Click the ""Help"" button to vote on this Issue #822 and it will be implemented, one day...!

            //More votes == released sooner.", "QueryExpressionFactory",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0,
            //                    "https://github.com/rappen/FetchXMLBuilder/issues/822");
            //                if (fxb.settings.CodeGenerators.Style == CodeGenerationStyle.EarlyBoundEBG)
            //                {
            //                    rbQExEarly.Checked = true;
            //                }
            //                else
            //                {
            //                    rbQExLate.Checked = true;
            //                }
            //                return;
            //            }
            //            fxb.settings.CodeGenerators.Style = rbQExEarly.Checked ? CodeGenerationStyle.EarlyBoundEBG : rbQExQExFactory.Checked ? CodeGenerationStyle.QueryExpressionFactory : CodeGenerationStyle.LateBound;
            //            fxb.UpdateLiveXML();
        }

        private void chkQExComments_CheckedChanged(object sender, EventArgs e)
        {
            fxb.settings.CodeGenerators.IncludeComments = chkQExComments.Checked;
            fxb.UpdateLiveXML();
        }

        private void linkEBG_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.OpenURL("https://www.xrmtoolbox.com/plugins/DLaB.Xrm.EarlyBoundGenerator/");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.OpenURL("https://github.com/rappen/FetchXMLBuilder/issues/822");
        }

        private void cmbQExStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            fxb.settings.CodeGenerators.QExStyle = (cmbQExStyle.SelectedItem is QExStyle style) ? style.Tag : QExStyleEnum.QueryExpression;
            rbQExLineByLine.Enabled = true;
            rbQExObjectinitializer.Enabled = true;
            cmbQExFlavor.Enabled = true;
            chkQExComments.Enabled = true;
            switch (fxb.settings.CodeGenerators.QExStyle)
            {
                case QExStyleEnum.QueryExpressionFactory:
                    rbQExLineByLine.Enabled = false;
                    rbQExObjectinitializer.Checked = true;
                    break;

                case QExStyleEnum.FetchXML:
                    rbQExObjectinitializer.Enabled = false;
                    cmbQExFlavor.Enabled = false;
                    chkQExComments.Enabled = false;
                    rbQExLineByLine.Checked = true;
                    cmbQExFlavor.SelectedIndex = -1;
                    chkQExComments.Checked = false;
                    break;
            }
            fxb.UpdateLiveXML();
        }

        private void cmbQExFlavor_SelectedIndexChanged(object sender, EventArgs e)
        {
            fxb.settings.CodeGenerators.QExFlavor = (cmbQExFlavor.SelectedItem is QExFlavor flavor) ? flavor.Tag : QExFlavorEnum.LateBound;
            fxb.UpdateLiveXML();
        }

        private void chkQExFilterVariables_CheckedChanged(object sender, EventArgs e)
        {
            fxb.settings.CodeGenerators.FilterVariables = chkQExFilterVariables.Checked;
            fxb.UpdateLiveXML();
        }

        private void rbQExObjectInitializer_CheckedChanged(object sender, EventArgs e)
        {
            fxb.settings.CodeGenerators.ObjectInitializer = rbQExObjectinitializer.Checked;
            fxb.UpdateLiveXML();
            return;
            if (rbQExObjectinitializer.Checked)
            {
                MessageBox.Show(@"Oooh I know you wich that option... but it is not yet implemented... #sorry

Do you like that idea?
Click the ""Help"" button to vote on this Issue #830 and it will be implemented, one day...!

More votes == released sooner.", "QueryExpressionFactory",
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0,
                    "https://github.com/rappen/FetchXMLBuilder/issues/830");

                rbQExLineByLine.Checked = true;
            }
        }
    }

    public enum ContentType
    {
        FetchXML,
        LayoutXML,
        FetchXML_Result,
        Serialized_Result_XML,
        Serialized_Result_JSON,
        CSharp_Code,
        SQL_Query,
        JavaScript_Query
    }

    internal enum SaveFormat
    {
        None = 0,
        XML = 1,
        JSON = 2,
        SQL = 3
    }

    internal enum XmlStyle
    {
        Formatted,
        Html,
        Esc,
        Mini
    }

    internal class AttributeMetadataSuggestion : MarkMpn.XmlSchemaAutocomplete.AutocompleteAttributeValueSuggestion
    {
        public AttributeMetadataSuggestion(AttributeMetadata attr, bool useDisplayName)
        {
            Value = attr.LogicalName;
            Title = attr.DisplayName?.UserLocalizedLabel?.Label;
            Description = attr.Description?.UserLocalizedLabel?.Label;
            DisplayName = useDisplayName ? $"{Title} ({Value})" : Value;
        }
    }

    internal class EntityMetadataSuggestion : MarkMpn.XmlSchemaAutocomplete.AutocompleteAttributeValueSuggestion
    {
        public EntityMetadataSuggestion(EntityMetadata entity, bool useDisplayName)
        {
            Value = entity.LogicalName;
            Title = entity.DisplayName?.UserLocalizedLabel?.Label;
            Description = entity.Description?.UserLocalizedLabel?.Label;
            DisplayName = useDisplayName ? $"{Title} ({Value})" : Value;
        }
    }
}