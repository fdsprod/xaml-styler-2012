﻿namespace XamlStyler.XamlStylerVSPackage.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using XamlStyler.XamlStylerVSPackage;
    using XamlStyler.XamlStylerVSPackage.StylerModels;

    public static class AttributeInfoExtension
    {
        #region Methods

        /// <summary>
        /// Handles markup extension value in style as:
        /// <attribute_name>="{<markup_name value1, 
        ///                                                                   value2, 
        ///                                                                   key1=value1,
        ///                                                                   key2=value2}"
        /// </summary>
        /// <param name="attrInfo"></param>
        /// <param name="baseIndentationString"></param>
        /// <returns></returns>
        public static string ToMultiLineString(this AttributeInfo attrInfo, string baseIndentationString)
        {
            #region Parameter Checks

            if (!attrInfo.IsMarkupExtension)
            {
                throw new ArgumentException("AttributeInfo shall have a markup extension value.", MethodBase.GetCurrentMethod().GetParameters()[0].Name);
            }

            #endregion

            MarkupExtensionInfo info = MarkupExtensionParser.Parse(attrInfo.Value);
            string currentIndentationString = baseIndentationString + String.Empty.PadLeft(attrInfo.Name.Length + 2, ' ');
            string value = info.ToMultiLineString(currentIndentationString);

            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat("{0}=\"{1}\"", attrInfo.Name, value);

            return buffer.ToString();
        }

        /// <summary>
        /// Single line attribute line in style as: 
        /// <attribute_name>="<attribute_value>"
        /// </summary>
        /// <param name="attrInfo"></param>
        /// <returns></returns>
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

            result = String.Format("{0}=\"{1}\"",
                    attrInfo.Name,
                    valuePart);

            return result;
        }

        #endregion Methods
    }
}