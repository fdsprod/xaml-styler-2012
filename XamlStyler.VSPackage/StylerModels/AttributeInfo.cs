namespace XamlStyler.XamlStylerVSPackage.StylerModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class AttributeInfo : IComparable
    {
        #region Fields

        // Fields
        private static readonly Regex MarkupExtensionPattern = new Regex("^{(?!}).*}$");

        private readonly AttributeOrderRule _orderRule = null;

        #endregion Fields

        #region Constructors

        public AttributeInfo(string name, string value, AttributeOrderRule orderRule)
        {
            this.Name = name;
            this.Value = value;
            this.IsMarkupExtension = MarkupExtensionPattern.IsMatch(value);
            this._orderRule = orderRule;
        }

        #endregion Constructors

        #region Properties

        public bool IsMarkupExtension
        {
            get; private set;
        }

        // Properties
        public string Name
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }

        #endregion Properties

        #region Methods

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

        #endregion Methods
    }
}