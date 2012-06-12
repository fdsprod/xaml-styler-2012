using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XamlStyler.Core
{
    public static class AttributeInfoExtension
    {
        public static string ToMultiLineString(this AttributeInfo attrInfo, string baseIndentationString)
        {
            if (!attrInfo.IsMarkupExtension)
            {
                throw new ArgumentException("AttributeInfo shall have a markup extension value.", MethodBase.GetCurrentMethod().GetParameters()[0].Name);
            }

            MarkupExtensionInfo info = MarkupExtensionParser.Parse(attrInfo.Value);
            string currentIndentationString = baseIndentationString + String.Empty.PadLeft(attrInfo.Name.Length + 2, ' ');
            string value = info.ToMultiLineString(currentIndentationString);

            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat("{0}=\"{1}\"", attrInfo.Name, value);

            return buffer.ToString();
        }

        public static string ToSingleLineString(this AttributeInfo attrInfo)
        {
            string result = String.Empty;
            string valuePart = String.Empty;

            if (attrInfo.IsMarkupExtension)
            {
                MarkupExtensionInfo info = MarkupExtensionParser.Parse(attrInfo.Value);
                valuePart = info.ToSingleLineString();
            }
            else
            {
                valuePart = attrInfo.Value.ToXmlEncodedString();
            }

            result = String.Format("{0}=\"{1}\"", attrInfo.Name, valuePart);

            return result;
        }
    }
}