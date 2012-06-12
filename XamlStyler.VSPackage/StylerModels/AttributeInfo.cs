using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XamlStyler.XamlStylerVSPackage.StylerModels
{
    public class AttributeInfo : IComparable
    {
        private static readonly Regex MarkupExtensionPattern = new Regex("^{(?!}).*}$");
        private readonly AttributeOrderRule _orderRule = null;

        public AttributeInfo(string name, string value, AttributeOrderRule orderRule)
        {
            this.Name = name;
            this.Value = value;
            this.IsMarkupExtension = MarkupExtensionPattern.IsMatch(value);
            this._orderRule = orderRule;
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

            if (_orderRule.AttributeTokenType != target._orderRule.AttributeTokenType)
            {
                return _orderRule.AttributeTokenType.CompareTo(target._orderRule.AttributeTokenType);
            }

            if (_orderRule.Priority != target._orderRule.Priority)
            {
                return _orderRule.Priority.CompareTo(target._orderRule.Priority);
            }

            return this.Name.CompareTo(target.Name);
        }
    }
}