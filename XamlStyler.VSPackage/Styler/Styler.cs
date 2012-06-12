using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using XamlStyler.XamlStylerVSPackage.Helpers;
using XamlStyler.XamlStylerVSPackage.Options;
using XamlStyler.XamlStylerVSPackage.StylerModels;

namespace XamlStyler.XamlStylerVSPackage
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class Styler
    {
        private Stack<ElementProcessStatus> _elementProcessStatusStack = new Stack<ElementProcessStatus>();
        private char _indentCharacter = ' ';
        private int _indentSize = 2;
        private bool _keepFirstAttributeOnSameLine = true;
        private IList<string> _noNewLineElementsList = new List<string>();
        private IStylerOptions _options = new StylerOptions();
        private AttributeOrderRules _orderRules = null;

        public Styler()
        {
            _elementProcessStatusStack.Push(new ElementProcessStatus());
        }

        public char IndentCharacter
        {
            get { return _indentCharacter; }
            set { _indentCharacter = value; }
        }

        public int IndentSize
        {
            get { return _indentSize; }
            set { _indentSize = value; }
        }

        public bool KeepFirstAttributeOnSameLine
        {
            get { return _keepFirstAttributeOnSameLine; }
            set { _keepFirstAttributeOnSameLine = value; }
        }

        public IStylerOptions Options
        {
            get
            {
                return _options;
            }

            set
            {
                _options = value;
            }
        }

        private AttributeOrderRules OrderRules
        {
            get
            {
                if (null == _orderRules)
                {
                    if (null == this.Options)
                    {
                        throw new InvalidOperationException("Styler options is not initialized.");
                    }
                    else
                    {
                        _orderRules = new AttributeOrderRules(this.Options);
                    }
                }

                return _orderRules;
            }
        }

        public string Format(string xamlSource)
        {
            string output = String.Empty;
            StringReader sourceReader = null;

            try
            {
                sourceReader = new StringReader(xamlSource);

                using (XmlReader xmlReader = XmlReader.Create(sourceReader))
                {
                    sourceReader = null;

                    xmlReader.Read();

                    while (!xmlReader.EOF)
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                this.UpdateParentElementProcessStatus(ContentTypeEnum.Mixed);

                                _elementProcessStatusStack.Push(
                                        new ElementProcessStatus()
                                        {
                                            Name = xmlReader.Name,
                                            ContentType = ContentTypeEnum.None,
                                            IsMultlineStartTag = false,
                                            IsSelfClosingElement = false
                                        }
                                    );

                                this.ProcessElement(xmlReader, ref output);

                                if (_elementProcessStatusStack.Peek().IsSelfClosingElement)
                                {
                                    _elementProcessStatusStack.Pop();
                                }

                                break;

                            case XmlNodeType.Text:
                                this.UpdateParentElementProcessStatus(ContentTypeEnum.SingleLineTextOnly);
                                this.ProcessTextNode(xmlReader, ref output);
                                break;

                            case XmlNodeType.ProcessingInstruction:
                                this.UpdateParentElementProcessStatus(ContentTypeEnum.Mixed);
                                this.ProcessInstruction(xmlReader, ref output);
                                break;

                            case XmlNodeType.Comment:
                                this.UpdateParentElementProcessStatus(ContentTypeEnum.Mixed);
                                this.ProcessComment(xmlReader, ref output);
                                break;

                            case XmlNodeType.Whitespace:
                                this.ProcessWhitespace(xmlReader, ref output);
                                break;

                            case XmlNodeType.EndElement:
                                this.ProcessEndElement(xmlReader, ref output);
                                _elementProcessStatusStack.Pop();
                                break;

                            default:
                                Trace.WriteLine(String.Format("Unprocessed NodeType: {0} Name: {1} Value: {2}", xmlReader.NodeType, xmlReader.Name, xmlReader.Value));
                                break;
                        }

                        xmlReader.Read();
                    }
                }
            }
            finally
            {
                if (null != sourceReader)
                {
                    sourceReader.Close();
                }
            }

            return output;
        }

        public string FormatFile(string filePath)
        {
            using (StreamReader reader = File.OpenText(filePath))
            {
                string xamlSource = reader.ReadToEnd();
                return this.Format(xamlSource);
            }
        }

        private string GetIndentString(int depth)
        {
            if (depth < 0)
            {
                depth = 0;
            }

            if ('\t' == this.IndentCharacter)
            {
                return new String(this.IndentCharacter, depth);
            }
            else
            {
                return new String(this.IndentCharacter, depth * this.IndentSize);
            }
        }

        private bool IsNoLineBreakElement(string elementName)
        {
            if (!String.IsNullOrEmpty(_options.NoNewLineElements))
            {
                if (null == _noNewLineElementsList || 0 == _noNewLineElementsList.Count)
                {
                    _noNewLineElementsList = this.Options.NoNewLineElements.Split(',')
                            .Where<string>(x => !String.IsNullOrWhiteSpace(x))
                            .Select<string, string>(x => x.Trim())
                            .ToList<string>();
                }
            }

            return _noNewLineElementsList.Contains<string>(elementName);
        }

        private void ProcessComment(XmlReader xmlReader, ref string output)
        {
            string currentIndentString = this.GetIndentString(xmlReader.Depth);
            string content = xmlReader.Value.Trim();

            if (!output.EndsWith("\n"))
            {
                output += Environment.NewLine;
            }

            if (content.Contains("\n"))
            {
                output += currentIndentString + "<!--";

                string contentIndentString = this.GetIndentString(xmlReader.Depth + 1);
                string[] lines = content.Split('\n');

                foreach (string line in lines)
                {
                    output += Environment.NewLine + contentIndentString + line.Trim();
                }

                output += Environment.NewLine + currentIndentString + "-->";
            }
            else
            {
                output += currentIndentString + "<!--  " + content + "  -->";
            }
        }

        private void ProcessElement(XmlReader xmlReader, ref string output)
        {
            string currentIndentString = this.GetIndentString(xmlReader.Depth);
            string elementName = xmlReader.Name;

            if ("Run".Equals(elementName))
            {
                if (output.EndsWith("\n"))
                {
                    // Shall not add extra whitespaces (including linefeeds) before <Run/>,
                    // because it will affect the rendering of <TextBlock><Run/><Run/></TextBlock>
                    output += currentIndentString + "<" + xmlReader.Name;
                }
                else
                {
                    output += "<" + xmlReader.Name;
                }
            }
            else if (output.Length == 0 || output.EndsWith("\n"))
            {
                output += currentIndentString + "<" + xmlReader.Name;
            }
            else
            {
                output += Environment.NewLine + currentIndentString + "<" + xmlReader.Name;
            }

            bool isEmptyElement = xmlReader.IsEmptyElement;
            bool hasPutEndingBracketOnNewLine = false;
            List<AttributeInfo> list = new List<AttributeInfo>(xmlReader.AttributeCount);

            if (xmlReader.HasAttributes)
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    string attributeName = xmlReader.Name;
                    string attributeValue = xmlReader.Value;
                    AttributeOrderRule orderRule = this.OrderRules.GetRuleFor(attributeName);
                    list.Add(new AttributeInfo(attributeName, attributeValue, orderRule));
                }

                list.Sort();

                currentIndentString = this.GetIndentString(xmlReader.Depth);

                // No need to break attributes
                if ((list.Count <= this.Options.AttributesTolerance)
                        || this.IsNoLineBreakElement(elementName))
                {
                    foreach (AttributeInfo attrInfo in list)
                    {
                        string pendingAppend = attrInfo.ToSingleLineString();
                        output += String.Format(" {0}", pendingAppend);
                    }

                    _elementProcessStatusStack.Peek().IsMultlineStartTag = false;
                }
                // Need to break attributes
                else
                {
                    IList<String> attributeLines = new List<String>();

                    StringBuilder currentLineBuffer = new StringBuilder();
                    int attributeCountInCurrentLineBuffer = 0;

                    for (int i = 0; i < list.Count; i++)
                    {
                        AttributeInfo attrInfo = list[i];

                        // Attributes with markup extension, always put on new line
                        if (attrInfo.IsMarkupExtension && this.Options.FormatMarkupExtension)
                        {
                            string baseIndetationString = this.GetIndentString(xmlReader.Depth - 1) + String.Empty.PadLeft(elementName.Length + 2, ' ');
                            string pendingAppend = attrInfo.ToMultiLineString(baseIndetationString);

                            if (currentLineBuffer.Length > 0)
                            {
                                attributeLines.Add(currentLineBuffer.ToString());
                                currentLineBuffer.Length = 0;
                                attributeCountInCurrentLineBuffer = 0;
                            }

                            attributeLines.Add(pendingAppend);
                        }
                        else
                        {
                            string pendingAppend = attrInfo.ToSingleLineString();

                            bool isAttributeCharLengthExceeded =
                                (attributeCountInCurrentLineBuffer > 0 && this.Options.MaxAttributeCharatersPerLine > 0
                                    && currentLineBuffer.Length + pendingAppend.Length > this.Options.MaxAttributeCharatersPerLine);

                            bool isAttributeCountExceeded =
                                (this.Options.MaxAttributesPerLine > 0 && attributeCountInCurrentLineBuffer + 1 > this.Options.MaxAttributesPerLine);

                            if (isAttributeCharLengthExceeded || isAttributeCountExceeded)
                            {
                                attributeLines.Add(currentLineBuffer.ToString());
                                currentLineBuffer.Length = 0;
                                attributeCountInCurrentLineBuffer = 0;
                            }

                            currentLineBuffer.AppendFormat("{0} ", pendingAppend);
                            attributeCountInCurrentLineBuffer++;
                        }
                    }

                    if (currentLineBuffer.Length > 0)
                    {
                        attributeLines.Add(currentLineBuffer.ToString());
                    }

                    for (int i = 0; i < attributeLines.Count; i++)
                    {
                        if (0 == i && this.KeepFirstAttributeOnSameLine)
                        {
                            output += String.Format(" {0}", attributeLines[i].Trim());
                            // Align subsequent attributes with first attribute
                            currentIndentString = this.GetIndentString(xmlReader.Depth - 1) + String.Empty.PadLeft(elementName.Length + 2, ' ');
                            continue;
                        }
                        else
                        {
                            output += Environment.NewLine + currentIndentString + attributeLines[i].Trim();
                        }
                    }

                    _elementProcessStatusStack.Peek().IsMultlineStartTag = true;
                }

                // Determine if to put ending bracket on new line
                if (this.Options.PutEndingBracketOnNewLine
                    && _elementProcessStatusStack.Peek().IsMultlineStartTag)
                {
                    output += Environment.NewLine + currentIndentString;
                    hasPutEndingBracketOnNewLine = true;
                }
            }

            if (isEmptyElement)
            {
                if (hasPutEndingBracketOnNewLine)
                {
                    output += "/>";
                }
                else
                {
                    output += " />";
                }

                _elementProcessStatusStack.Peek().IsSelfClosingElement = true;
            }
            else
            {
                output += ">";
            }
        }

        private void ProcessEndElement(XmlReader xmlReader, ref string output)
        {
            // Shrink the current element, if it has no content.
            // E.g., <Element>  </Element> => <Element />
            if (ContentTypeEnum.None == _elementProcessStatusStack.Peek().ContentType
                && Options.RemoveEndingTagOfEmptyElement)
            {
                output = output.TrimEnd(' ', '\t', '\r', '\n');

                int bracketIndex = output.LastIndexOf('>');

                if ('\t' != output[bracketIndex - 1] && ' ' != output[bracketIndex - 1])
                {
                    output = output.Insert(bracketIndex, " /");
                }
                else
                {
                    output = output.Insert(bracketIndex, "/");
                }
            }
            else if (ContentTypeEnum.SingleLineTextOnly == _elementProcessStatusStack.Peek().ContentType
                && false == _elementProcessStatusStack.Peek().IsMultlineStartTag)
            {
                int bracketIndex = output.LastIndexOf('>');

                string text = output.Substring(bracketIndex + 1, output.Length - bracketIndex - 1).Trim();

                output = output.Remove(bracketIndex + 1);
                output += text + "</" + xmlReader.Name + ">";
            }
            else
            {
                string currentIndentString = this.GetIndentString(xmlReader.Depth);

                if (!output.EndsWith("\n"))
                {
                    output += Environment.NewLine;
                }

                output += currentIndentString + "</" + xmlReader.Name + ">";
            }
        }

        private void ProcessInstruction(XmlReader xmlReader, ref string output)
        {
            string currentIndentString = this.GetIndentString(xmlReader.Depth);

            if (!output.EndsWith("\n"))
            {
                output += Environment.NewLine;
            }

            output += currentIndentString + "<?Mapping " + xmlReader.Value + " ?>";
        }

        private void ProcessTextNode(XmlReader xmlReader, ref string output)
        {
            string currentIndentString = this.GetIndentString(xmlReader.Depth);
            IEnumerable<String> textLines =
                xmlReader.Value.ToXmlEncodedString(ignoreCarrier: true).Trim().Split('\n').Where<String>(x => x.Trim().Length > 0);

            foreach (string line in textLines)
            {
                if (line.Trim().Length > 0)
                {
                    output += Environment.NewLine + currentIndentString + line.Trim();
                }
            }

            if (textLines.Count<String>() > 1)
            {
                UpdateParentElementProcessStatus(ContentTypeEnum.MultiLineTextOnly);
            }
        }

        private void ProcessWhitespace(XmlReader xmlReader, ref string output)
        {
            if (xmlReader.Value.Contains('\n'))
            {
                // For WhiteSpaces contain linefeed, trim all spaces/tab，
                // since the intent of this whitespace node is to break line,
                // and preserve the line feeds
                output += xmlReader.Value.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", Environment.NewLine);
            }
            else
            {
                // Preserve "pure" WhiteSpace between elements
                // e.g.,
                //   <TextBlock>
                //     <Run>A</Run> <Run>
                //      B
                //     </Run>
                //  </TextBlock>
                output += xmlReader.Value;
            }
        }

        private void UpdateParentElementProcessStatus(ContentTypeEnum contentType)
        {
            ElementProcessStatus parentElementProcessStatus = _elementProcessStatusStack.Peek();

            parentElementProcessStatus.ContentType |= contentType;
        }
    }
}