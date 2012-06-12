using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XamlStyler.Core
{
    public class AttributeInfo : IComparable
    {
        private static readonly Regex MarkupExtensionPattern = new Regex("^{(?!}).*}$");
        
        private readonly AttributeOrderRule attributeOrderRule = null;

        public AttributeInfo(string name, string value, AttributeOrderRule orderRule)
        {
            this.Name = name;
            this.Value = value;
            this.IsMarkupExtension = MarkupExtensionPattern.IsMatch(value);
            this.attributeOrderRule = orderRule;
        }

        public bool IsMarkupExtension
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public int CompareTo(object obj)
        {
            AttributeInfo target = obj as AttributeInfo;

            if (target == null)
            {
                return 0;
            }

            if (attributeOrderRule.AttributeTokenType != target.attributeOrderRule.AttributeTokenType)
            {
                return attributeOrderRule.AttributeTokenType.CompareTo(target.attributeOrderRule.AttributeTokenType);
            }

            if (attributeOrderRule.Priority != target.attributeOrderRule.Priority)
            {
                return attributeOrderRule.Priority.CompareTo(target.attributeOrderRule.Priority);
            }

            return this.Name.CompareTo(target.Name);
        }
    }
}