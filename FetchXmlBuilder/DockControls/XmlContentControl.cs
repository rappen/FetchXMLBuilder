using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.XmlEditorUtils;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class XmlContentControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private string findtext = "";
        private FetchXmlBuilder fxb;
        private ContentType contenttype;
        private SaveFormat format;

        internal XmlContentControl(FetchXmlBuilder caller) : this(ContentType.FetchXML, SaveFormat.XML, caller)
        {
        }

        internal XmlContentControl(ContentType contentType, SaveFormat saveFormat, FetchXmlBuilder caller)
        {
            InitializeComponent();
            this.PrepareGroupBoxExpanders();
            fxb = caller;
            if (contentType == ContentType.FetchXML)
            {
                txtXML.KeyUp += fxb.LiveXML_KeyUp;
                InitIntellisense();
            }
            SetContentType(contentType);
            SetFormat(saveFormat);
            UpdateButtons();
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
            var allowedit = contenttype == ContentType.FetchXML;
            var allowparse = contenttype == ContentType.QueryExpression;
            var allowsql = contenttype == ContentType.SQL_Query;
            chkLiveUpdate.Checked = allowedit && windowSettings.LiveUpdate;
            lblFormatExpander.GroupBoxSetState(tt, windowSettings.FormatExpanded);
            lblActionsExpander.GroupBoxSetState(tt, windowSettings.ActionExpanded);
            panLiveUpdate.Visible = allowedit;
            panOk.Visible = allowedit;
            panFormatting.Visible = allowedit;
            panExecute.Visible = allowedit;
            panParseQE.Visible = allowparse;
            panSQL4CDS.Visible = allowsql;
            panSQL4CDSInfo.Visible = allowsql;

            switch (contentType)
            {
                case ContentType.FetchXML:
                case ContentType.FetchXML_Result:
                case ContentType.Serialized_Result_XML:
                    txtXML.ConfigureForXml(fxb.settings);
                    break;

                case ContentType.SQL_Query:
                    txtXML.ConfigureForSQL();
                    break;

                case ContentType.CSharp_Query:
                case ContentType.QueryExpression:
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
            fxb.dockControlBuilder.Init(txtXML.Text, "manual edit", true);
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
            if (txtXML.Lexer == ScintillaNET.Lexer.Xml)
                FormatXML(xmlString, true);
            else
                txtXML.Text = xmlString;
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
            if (!FetchIsPlain() && !FetchIsMini())
            {
                if (FetchIsHtml())
                {
                    txtXML.Text = HttpUtility.HtmlDecode(txtXML.Text.Trim());
                }
                else if (FetchIsEscaped())
                {
                    txtXML.Text = Uri.UnescapeDataString(txtXML.Text.Trim());
                }
                else
                {
                    if (MessageBox.Show("Unrecognized encoding, unsure what to do with it.\n" +
                        "Currently FXB can handle htmlencoded and urlescaped strings.\n\n" +
                        "Would you like to submit an issue to FetchXML Builder to be able to handle this?",
                        "Decode FetchXML", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
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
            if (!FetchIsPlain())
            {
                FormatAsXML();
            }
            var xml = response == DialogResult.Yes ? GetCompactXml() : txtXML.Text;
            txtXML.Text = HttpUtility.HtmlEncode(xml);
        }

        private void FormatAsEsc()
        {
            if (!FetchIsPlain())
            {
                FormatAsXML();
            }
            txtXML.Text = Uri.EscapeDataString(GetCompactXml());
        }

        private void FormatAsMini()
        {
            if (!FetchIsPlain() && !FetchIsMini())
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
            if (!FetchIsPlain())
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
            if (FetchIsMini())
            {
                return XmlStyle.Mini;
            }
            if (FetchIsHtml())
            {
                return XmlStyle.Html;
            }
            if (FetchIsEscaped())
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

        private bool FetchIsPlain()
        {
            var lines = txtXML.Text.Trim().Split('\n').Select(l => l.Trim()).ToList();
            return lines.Count > 1 && lines[0].StartsWith("<fetch");
        }

        private bool FetchIsMini()
        {
            var lines = txtXML.Text.Trim().Split('\n').Select(l => l.Trim()).ToList();
            return lines.Count == 1 && lines[0].StartsWith("<fetch");
        }

        private bool FetchIsHtml()
        {
            return txtXML.Text.Trim().ToLowerInvariant().StartsWith("&lt;fetch");
        }

        private bool FetchIsEscaped()
        {
            return txtXML.Text.Trim().ToLowerInvariant().StartsWith("%3cfetch");
        }

        private void txtXML_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var plain = FetchIsPlain();
            rbFormatEsc.Checked = FetchIsEscaped();
            rbFormatHTML.Checked = FetchIsHtml();
            rbFormatMini.Checked = FetchIsMini();
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
                    case ContentType.CSharp_Query:
                        fxb.settings.DockStates.FetchXMLCs = DockState;
                        break;
                    case ContentType.JavaScript_Query:
                        fxb.settings.DockStates.FetchXMLJs = DockState;
                        break;
                    case ContentType.QueryExpression:
                        fxb.settings.DockStates.QueryExpression = DockState;
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
            txtXML.CharAdded += txtXML_CharAdded;
        }

        private void txtXML_CharAdded(object sender, CharAddedEventArgs e)
        {
            var stack = new Stack<object>();
            var text = txtXML.Text.Substring(0, txtXML.CurrentPosition);

            var rootType = typeof(FetchType);
            var rootElementName = ((XmlRootAttribute)Attribute.GetCustomAttribute(rootType, typeof(XmlRootAttribute))).ElementName;

            var childElements = new Dictionary<Type, Dictionary<string, Tuple<MemberInfo, Type>>>();

            var root = default(object);
            var inComment = false;
            var inElement = false;
            var inEndElement = false;
            var elementNameStart = -1;
            var attributeNameStart = -1;
            var attributeName = default(string);
            var attributeValueStart = -1;
            var valueQuoteChar = '\0';
            var textStart = -1;

            for (var i = 0; i < text.Length; i++)
            {
                var ch = text[i];

                if (ch == '<' && !inElement)
                {
                    if (i < text.Length - 3 && text[i + 1] == '!' && text[i + 2] == '-' && text[i + 3] == '-')
                    {
                        inComment = true;
                    }
                    else
                    {
                        inElement = true;
                        elementNameStart = i + 1;

                        if (textStart != -1 && textStart < i && root != null)
                        {
                            var textValue = text.Substring(textStart, i - textStart);

                            var parent = stack.Peek();
                            var textMember = parent
                                .GetType()
                                .GetMembers()
                                .SingleOrDefault(member => member.GetCustomAttribute(typeof(XmlTextAttribute)) != null);

                            if (textMember is PropertyInfo prop)
                                prop.SetValue(parent, textValue);
                            else if (textMember is FieldInfo field)
                                field.SetValue(parent, textValue);

                            textStart = -1;
                        }
                    }
                }
                else if (ch == '/' && inElement && i == elementNameStart)
                {
                    inEndElement = true;
                }
                else if (inComment && ch == '>' && text[i - 1] == '-' && text[i - 2] == '-')
                {
                    inComment = false;
                }
                else if (elementNameStart != -1 && (ch == '>' || ch == ' ' || ch == '/') && !inEndElement)
                {
                    var elementName = text.Substring(elementNameStart, i - elementNameStart);

                    if (root == null)
                    {
                        if (elementName != rootElementName)
                            return;

                        root = Activator.CreateInstance(rootType);
                        stack.Push(root);
                    }
                    else
                    {
                        var parent = stack.Peek();

                        if (!childElements.TryGetValue(parent.GetType(), out var knownChildElements))
                        {
                            knownChildElements = parent
                                .GetType()
                                .GetMembers()
                                .SelectMany(member => member
                                    .GetCustomAttributes(typeof(XmlElementAttribute))
                                    .Cast<XmlElementAttribute>()
                                    .Select(attr => new { Member = member, Attribute = attr })
                                    )
                                .ToDictionary(
                                    tuple => tuple.Attribute.ElementName,
                                    tuple =>
                                    {
                                        if (tuple.Attribute.Type != null)
                                            return new Tuple<MemberInfo, Type>(tuple.Member, tuple.Attribute.Type);

                                        var p = tuple.Member as PropertyInfo;
                                        var f = tuple.Member as FieldInfo;
                                        var targetType = p?.PropertyType ?? f.FieldType;

                                        if (targetType.IsArray)
                                            targetType = targetType.GetElementType();

                                        return new Tuple<MemberInfo, Type>(tuple.Member, targetType);
                                    }
                                );

                            childElements[parent.GetType()] = knownChildElements;
                        }

                        if (!knownChildElements.TryGetValue(elementName, out var elementDetails))
                            return;

                        var element = Activator.CreateInstance(elementDetails.Item2);
                        stack.Push(element);

                        if (elementDetails.Item1 is PropertyInfo prop)
                        {
                            if (prop.PropertyType.IsArray)
                            {
                                var array = (Array)prop.GetValue(parent);
                                if (array == null)
                                {
                                    array = Array.CreateInstance(prop.PropertyType.GetElementType(), 1);
                                }
                                else
                                {
                                    var existing = array;
                                    array = Array.CreateInstance(prop.PropertyType.GetElementType(), array.Length + 1);
                                    existing.CopyTo(array, 0);
                                }

                                array.SetValue(element, array.Length - 1);
                                prop.SetValue(parent, array);
                            }
                            else
                            {
                                prop.SetValue(parent, element);
                            }
                        }
                        else if (elementDetails.Item1 is FieldInfo field)
                        {
                            if (field.FieldType.IsArray)
                            {
                                var array = (Array) field.GetValue(parent);
                                if (array == null)
                                {
                                    array = Array.CreateInstance(field.FieldType.GetElementType(), 1);
                                }
                                else
                                {
                                    var existing = array;
                                    array = Array.CreateInstance(field.FieldType.GetElementType(), array.Length + 1);
                                    existing.CopyTo(array, 0);
                                }

                                array.SetValue(element, array.Length - 1);
                                field.SetValue(parent, array);
                            }
                            else
                            {
                                field.SetValue(parent, element);
                            }
                        }
                    }

                    elementNameStart = -1;

                    if (ch == '>')
                    {
                        inElement = false;
                        textStart = i + 1;
                    }
                }
                else if (inElement && elementNameStart == -1 && attributeNameStart == -1 && attributeName == null && ch != ' ' && ch != '/' && ch != '>')
                {
                    attributeNameStart = i;
                }
                else if (attributeNameStart != -1 && ch == '=')
                {
                    attributeName = text.Substring(attributeNameStart, i - attributeNameStart).ToLowerInvariant();
                    attributeNameStart = -1;
                }
                else if (attributeName != null && attributeValueStart == -1 && (ch == '\'' || ch == '"'))
                {
                    valueQuoteChar = ch;
                    attributeValueStart = i + 1;
                }
                else if (attributeValueStart != -1 && ch == valueQuoteChar)
                {
                    var attributeValue = text.Substring(attributeValueStart, i - attributeValueStart);
                    attributeValueStart = -1;
                    valueQuoteChar = '\0';

                    var obj = stack.Peek();
                    var member = obj
                        .GetType()
                        .GetMembers()
                        .SingleOrDefault(p =>
                        {
                            var attrs = Attribute.GetCustomAttributes(p, typeof(XmlAttributeAttribute));
                            return attrs
                                .Cast<XmlAttributeAttribute>()
                                .Any(a => a.AttributeName == attributeName || (String.IsNullOrEmpty(a.AttributeName) && p.Name == attributeName));
                        });

                    if (member != null)
                    {
                        if (member is PropertyInfo prop)
                            prop.SetValue(obj, ChangeType(prop.PropertyType, attributeValue));
                        else if (member is FieldInfo field)
                            field.SetValue(obj, ChangeType(field.FieldType, attributeValue));
                    }

                    attributeName = null;
                }
                else if (ch == '>' && inElement)
                {
                    if (text[i - 1] == '/' || inEndElement)
                    {
                        if (stack.Count == 0)
                            return;

                        stack.Pop();
                    }
                    else
                    {
                        textStart = i + 1;
                    }

                    inElement = false;
                    inEndElement = false;
                    elementNameStart = -1;
                }
            }

            // Create suggestions based on the current parser state
            if (inComment)
            {
                // We're entering a comment, nothing useful to suggest
            }
            else if (elementNameStart != -1)
            {
                // We're entering an element name
            }
            else if (attributeNameStart != -1)
            {
                // We're entering an attribute name
            }
            else if (attributeName != null)
            {
                // We're entering an attribute value

                if (attributeValueStart == -1)
                {
                    // We need to suggest the whole value, including quotes
                }
                else
                {
                    // We've already got a partial value to complete
                }
            }
            else
            {
                // We're entering a text value
            }
        }

        private object ChangeType(Type type, string value)
        {
            if (type.IsEnum)
            {
                // Check if there are custom XmlEnumAttributes defined
                var xmlValue = type
                    .GetFields()
                    .Select(field => new { Field = field, XmlEnum = field.GetCustomAttribute<XmlEnumAttribute>() })
                    .SingleOrDefault(field => field.XmlEnum?.Name == value);

                if (xmlValue != null)
                    return xmlValue.Field.GetValue(null);

                return Enum.Parse(type, value);
            }

            return Convert.ChangeType(value, type);
        }
    }

    public enum ContentType
    {
        FetchXML,
        FetchXML_Result,
        Serialized_Result_XML,
        Serialized_Result_JSON,
        QueryExpression,
        SQL_Query,
        JavaScript_Query,
        CSharp_Query
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
}