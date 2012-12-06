/****************************** Module Header ******************************\ 
* Module Name:  XMLViewer.cs 
* Project:        CSRichTextBoxSyntaxHighlighting  
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
using System.Xml.Linq;
using System.Drawing;

namespace Wxv.Swg.Common
{
    internal class RtfCharacterEncoder
    {
        public static string Encode(string originalText)
        {
            if (string.IsNullOrEmpty(originalText))
            {
                return string.Empty;
            }

            StringBuilder encodedText = new StringBuilder();
            for (int i = 0; i < originalText.Length; i++)
            {
                switch (originalText[i])
                {
                    case '"':
                        encodedText.Append("&quot;");
                        break;
                    case '&':
                        encodedText.Append(@"&amp;");
                        break;
                    case '\'':
                        encodedText.Append(@"&apos;");
                        break;
                    case '<':
                        encodedText.Append(@"&lt;");
                        break;
                    case '>':
                        encodedText.Append(@"&gt;");
                        break;

                    // The character '\' should be converted to @"\\" or "\\\\"  
                    case '\\':
                        encodedText.Append(@"\\");
                        break;

                    // The character '{' should be converted to @"\{" or "\\{"  
                    case '{':
                        encodedText.Append(@"\{");
                        break;

                    // The character '}' should be converted to @"\}" or "\\}"  
                    case '}':
                        encodedText.Append(@"\}");
                        break;
                    default:
                        encodedText.Append(originalText[i]);
                        break;
                }

            }
            return encodedText.ToString();
        }
    }

    public class XMLViewerSettings
    {
        public const int ElementID = 1;
        public const int ValueID = 2;
        public const int AttributeKeyID = 3;
        public const int AttributeValueID = 4;
        public const int TagID = 5;

        /// <summary> 
        /// The color of an Xml element name. 
        /// </summary> 
        public System.Drawing.Color Element { get; set; }

        /// <summary> 
        /// The color of an Xml element value. 
        /// </summary> 
        public System.Drawing.Color Value { get; set; }

        /// <summary> 
        /// The color of an Attribute Key in Xml element. 
        /// </summary> 
        public System.Drawing.Color AttributeKey { get; set; }

        /// <summary> 
        /// The color of an Attribute Value in Xml element. 
        /// </summary> 
        public System.Drawing.Color AttributeValue { get; set; }

        /// <summary> 
        /// The color of the tags and operators like "<,/> and =". 
        /// </summary> 
        public System.Drawing.Color Tag { get; set; }

        /// <summary> 
        /// Convert the settings to Rtf color definitions. 
        /// </summary> 
        public string ToRtfFormatString()
        {
            // The Rtf color definition format. 
            string format = @"\red{0}\green{1}\blue{2};";

            StringBuilder rtfFormatString = new StringBuilder();

            rtfFormatString.AppendFormat(format, Element.R, Element.G, Element.B);
            rtfFormatString.AppendFormat(format, Value.R, Value.G, Value.B);
            rtfFormatString.AppendFormat(format,
                AttributeKey.R,
                AttributeKey.G,
                AttributeKey.B);
            rtfFormatString.AppendFormat(format, AttributeValue.R,
                AttributeValue.G, AttributeValue.B);
            rtfFormatString.AppendFormat(format, Tag.R, Tag.G, Tag.B);

            return rtfFormatString.ToString();

        }
    }

    public class XmlToRtfConverter
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
                        AttributeKey = System.Drawing.Color.Red,
                        AttributeValue = System.Drawing.Color.Blue,
                        Tag = System.Drawing.Color.Blue,
                        Element = System.Drawing.Color.DarkRed,
                        Value = System.Drawing.Color.Black,
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
        public string Process(XDocument xmlDoc, bool includeDeclaration)
        {
            try
            {
                // The Rtf contains 2 parts, header and content. The colortbl is a part of 
                // the header, and the {1} will be replaced with the content. 
                string rtfFormat = @"{{\rtf1\ansi\ansicpg1252\deff0\deflang1033\deflangfe2052 
{{\fonttbl{{\f0\fnil Courier New;}}}} 
{{\colortbl ;{0}}} 
\viewkind4\uc1\pard\lang1033\f0\fs20  
{1}}}";

                StringBuilder xmlRtfContent = new StringBuilder();

                // If includeDeclaration is true and the XDocument has declaration, 
                // then add the declaration to the content. 
                if (includeDeclaration && xmlDoc.Declaration != null)
                {

                    // The constants in XMLViewerSettings are used to specify the order  
                    // in colortbl of the Rtf. 
                    xmlRtfContent.AppendFormat(@" 
\cf{0} <?\cf{1} xml \cf{2} version\cf{0} =\cf0 ""\cf{3} {4}\cf0 ""  
\cf{2} encoding\cf{0} =\cf0 ""\cf{3} {5}\cf0 ""\cf{0} ?>\par",
                        XMLViewerSettings.TagID,
                        XMLViewerSettings.ElementID,
                        XMLViewerSettings.AttributeKeyID,
                        XMLViewerSettings.AttributeValueID,
                        xmlDoc.Declaration.Version,
                        xmlDoc.Declaration.Encoding);
                }

                // Get the Rtf of the root element. 
                string rootRtfContent = ProcessElement(xmlDoc.Root, 0);

                xmlRtfContent.Append(rootRtfContent);

                // Construct the completed Rtf, and set the Rtf property to this value. 
                return string.Format(rtfFormat, Settings.ToRtfFormatString(),
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
        private string ProcessElement(XElement element, int level)
        {

            // This viewer does not support the Xml file that has Namespace. 
            if (!string.IsNullOrEmpty(element.Name.Namespace.NamespaceName))
            {
                throw new ApplicationException(
                    "This viewer does not support the Xml file that has Namespace.");
            }

            string elementRtfFormat = string.Empty;
            StringBuilder childElementsRtfContent = new StringBuilder();
            StringBuilder attributesRtfContent = new StringBuilder();

            // Construct the indent. 
            string indent = new string(' ', 4 * level);

            // If the element has child elements or value, then add the element to the  
            // Rtf. {{0}} will be replaced with the attributes and {{1}} will be replaced 
            // with the child elements or value. 
            if (element.HasElements || !string.IsNullOrEmpty(element.Value))
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
                if (element.HasElements)
                {
                    foreach (var childElement in element.Elements())
                    {
                        string childElementRtfContent =
                            ProcessElement(childElement, level + 1);
                        childElementsRtfContent.Append(childElementRtfContent);
                    }
                }

                // If !string.IsNullOrWhiteSpace(element.Value), then construct the Rtf  
                // of the value. 
                else
                {
                    childElementsRtfContent.AppendFormat(@"{0}\cf{1} {2}\par",
                        new string(' ', 4 * (level + 1)),
                        XMLViewerSettings.ValueID,
                        RtfCharacterEncoder.Encode(element.Value.Trim()));
                }
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
            if (element.HasAttributes)
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    string attributeRtfContent = string.Format(
                        @" \cf{0} {3}\cf{1} =\cf0 ""\cf{2} {4}\cf0 """,
                        XMLViewerSettings.AttributeKeyID,
                        XMLViewerSettings.TagID,
                        XMLViewerSettings.AttributeValueID,
                        attribute.Name,
                       RtfCharacterEncoder.Encode(attribute.Value));
                    attributesRtfContent.Append(attributeRtfContent);
                }
                attributesRtfContent.Append(" ");
            }

            return string.Format(elementRtfFormat, attributesRtfContent,
                childElementsRtfContent);
        }

    }
}