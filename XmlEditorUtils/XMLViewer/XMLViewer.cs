/****************************** Module Header ******************************\
* Module Name:  XMLViewer.cs
* Project:	    CSRichTextBoxSyntaxHighlighting 
* Copyright (c) Microsoft Corporation.
* 
* This XMLViewer class inherits System.Windows.Forms.RichTextBox and it is used 
* to display an Xml in a specified format. 
* 
* RichTextBox uses the Rtf format to show the test. The XMLViewer will 
* convert the Xml to Rtf with some formats specified in the XMLViewerSettings,
* and then set the Rtf property to the value.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Text;
using System.Drawing;
using System.Xml;

namespace CSRichTextBoxSyntaxHighlighting
{
    public class XMLViewer : System.Windows.Forms.RichTextBox
    {
        private XMLViewerSettings settings;
        /// <summary>
        /// The format settings.
        /// </summary>
        public XMLViewerSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new XMLViewerSettings
                    {
                        AttributeKey = Color.Red,
                        AttributeValue = Color.Blue,
                        Comment = Color.Green,
                        Tag = Color.Blue,
                        Element = Color.DarkRed,
                        Value = Color.Black,
                    };
                }
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        /// <summary>
        /// Convert the Xml to Rtf with some formats specified in the XMLViewerSettings,
        /// and then set the Rtf property to the value.
        /// </summary>
        /// <param name="includeDeclaration">
        /// Specify whether include the declaration.
        /// </param>
        public void Process()
        {
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                return;
            }
            try
            {
                // The Rtf contains 2 parts, header and content. The colortbl is a part of
                // the header, and the {1} will be replaced with the content.
                string rtfFormat = @"{{\rtf1\ansi\ansicpg1252\deff0\deflang1033\deflangfe2052
{{\fonttbl{{\f0\fnil Courier New;}}}}
{{\colortbl ;{0}}}
\viewkind4\uc1\pard\lang1033\f0\fs18 
{1}}}";

                // Get the XDocument from the Text property.
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(this.Text);

                StringBuilder xmlRtfContent = new StringBuilder();

                // Get the Rtf of the root element.
                string rootRtfContent = ProcessElement(xmlDoc.DocumentElement, 0);

                xmlRtfContent.Append(rootRtfContent);

                // Construct the completed Rtf, and set the Rtf property to this value.
                this.Rtf = string.Format(rtfFormat, Settings.ToRtfFormatString(),
                    xmlRtfContent.ToString());

            }
            catch (System.Xml.XmlException xmlException)
            {
                throw new ApplicationException(
                    "Please check the input Xml. Error:" + xmlException.Message,
                    xmlException);
            }
            catch
            {
                throw;
            }
        }

        // Get the Rtf of the xml element.
        private string ProcessElement(XmlNode element, int level)
        {
            string elementRtfFormat = string.Empty;
            StringBuilder childElementsRtfContent = new StringBuilder();
            StringBuilder attributesRtfContent = new StringBuilder();

            // Construct the indent.
            string indent = new string(' ', 2 * level);

            // If the element has child elements or value, then add the element to the 
            // Rtf. {{0}} will be replaced with the attributes and {{1}} will be replaced
            // with the child elements or value.
            if (element.ChildNodes.Count > 0 && !(element.ChildNodes.Count==1 && element.ChildNodes[0] is XmlText))
            {
                elementRtfFormat = string.Format(@"
{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} >\par
{{1}}
{0}\cf{1} </\cf{2} {3}\cf{1} >\par",
                    indent,
                    XMLViewerSettings.TagID,
                    XMLViewerSettings.ElementID,
                    element.Name);

                // Construct the Rtf of child elements.
                foreach (XmlNode childElement in element.ChildNodes)
                {
                    string childElementRtfContent =
                        ProcessElement(childElement, level + 1);
                    childElementsRtfContent.Append(childElementRtfContent);
                }
            }

            else if (element is XmlComment)
            {
                elementRtfFormat = string.Format(@"
{0}\cf{1} <!--
{{1}}
\cf{1} -->\par",
                    indent,
                    XMLViewerSettings.TagID);

                childElementsRtfContent.AppendFormat(@"{0}\cf{1} {2}",
                    new string(' ', 2 * (0 /*level + 1*/)),
                    XMLViewerSettings.CommentID,
                    element.Value.Replace("\r\n", "\n").Replace("\n", "\\line "));
            }

            // If !string.IsNullOrWhiteSpace(element.Value), then construct the Rtf 
            // of the value.
            else if (element.ChildNodes.Count == 1 && element.ChildNodes[0] is XmlText)
            {
                elementRtfFormat = string.Format(@"
{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} >
{{1}}
\cf{1} </\cf{2} {3}\cf{1} >\par",
                    indent,
                    XMLViewerSettings.TagID,
                    XMLViewerSettings.ElementID,
                    element.Name);

                childElementsRtfContent.AppendFormat(@"{0}\cf{1} {2}",
                    new string(' ', 2 * (0 /*level + 1*/)),
                    XMLViewerSettings.ValueID,
                    CharacterEncoder.Encode(((XmlText)element.ChildNodes[0]).Value.Trim()));
            }

            // If !string.IsNullOrWhiteSpace(element.Value), then construct the Rtf 
            // of the value.
            else if (!string.IsNullOrWhiteSpace(element.Value))
            {
                elementRtfFormat = string.Format(@"
{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} >
{{1}}
\cf{1} </\cf{2} {3}\cf{1} >\par",
                    indent,
                    XMLViewerSettings.TagID,
                    XMLViewerSettings.ElementID,
                    element.Name);

                childElementsRtfContent.AppendFormat(@"{0}\cf{1} {2}",
                    new string(' ', 2 * (0 /*level + 1*/)),
                    XMLViewerSettings.ValueID,
                    CharacterEncoder.Encode(element.Value.Trim()));
            }

            // This element only has attributes. {{0}} will be replaced with the attributes.
            else
            {
                elementRtfFormat =
                    string.Format(@"
{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} />\par",
                    indent,
                    XMLViewerSettings.TagID,
                    XMLViewerSettings.ElementID,
                    element.Name);
            }

            // Construct the Rtf of the attributes.
            if (element.Attributes != null && element.Attributes.Count > 0)
            {
                foreach (XmlAttribute attribute in element.Attributes)
                {
                    string attributeRtfContent = string.Format(
                        @" \cf{0} {3}\cf{1} =\cf0 ""\cf{2} {4}\cf0 """,
                        XMLViewerSettings.AttributeKeyID,
                        XMLViewerSettings.TagID,
                        XMLViewerSettings.AttributeValueID,
                        attribute.Name,
                       CharacterEncoder.Encode(attribute.Value));
                    attributesRtfContent.Append(attributeRtfContent);
                }
                attributesRtfContent.Append(" ");
            }

            return string.Format(elementRtfFormat, attributesRtfContent,
                childElementsRtfContent);
        }
    }
}
