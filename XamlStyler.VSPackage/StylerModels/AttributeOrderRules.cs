namespace XamlStyler.XamlStylerVSPackage.StylerModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using XamlStyler.XamlStylerVSPackage.Options;

    public class AttributeOrderRules
    {
        #region Fields

        private readonly IDictionary<string, AttributeOrderRule> _internalDictionary = null;

        #endregion Fields

        #region Constructors

        public AttributeOrderRules(IStylerOptions options)
        {
            _internalDictionary = new Dictionary<string, AttributeOrderRule>();

            this.Populate(options.AttributeOrderWpfNamespace, AttributeTokenInfoEnum.WPF_NAMESPACE)
                .Populate(options.AttributeOrderClass, AttributeTokenInfoEnum.CLASS)
                .Populate(options.AttributeOrderKey, AttributeTokenInfoEnum.KEY)
                .Populate(options.AttributeOrderName, AttributeTokenInfoEnum.NAME)
                .Populate(options.AttributeOrderAttachedLayout, AttributeTokenInfoEnum.ATTACHED_LAYOUT)
                .Populate(options.AttributeOrderCoreLayout, AttributeTokenInfoEnum.CORE_LAYOUT)
                .Populate(options.AttributeOrderAlignmentLayout, AttributeTokenInfoEnum.ALIGNMENT_LAYOUT)
                .Populate(options.AttributeOrderOthers, AttributeTokenInfoEnum.OTHER)
                .Populate(options.AttributeOrderBlendRelated, AttributeTokenInfoEnum.BLEND_RELATED);
        }

        #endregion Constructors

        #region Methods

        public bool ContainsRuleFor(string name)
        {
            return _internalDictionary.Keys.Contains(name);
        }

        public AttributeOrderRule GetRuleFor(string attributeName)
        {
            AttributeOrderRule result = null;

            if (_internalDictionary.Keys.Contains(attributeName))
            {
                result = _internalDictionary[attributeName];
            }
            else
            {
                AttributeTokenInfoEnum tempAttributeTokenType = AttributeTokenInfoEnum.OTHER;

                if (attributeName.StartsWith("xmlns"))
                {
                    tempAttributeTokenType = AttributeTokenInfoEnum.OTHER_NAMESPACE;
                }
                else
                {
                    tempAttributeTokenType = AttributeTokenInfoEnum.OTHER;
                }

                result = new AttributeOrderRule()
                {
                    AttributeTokenType = tempAttributeTokenType,
                    Priority = 0
                };
            }

            return result;
        }

        private AttributeOrderRules Populate(string option, AttributeTokenInfoEnum tokenType)
        {
            if (!String.IsNullOrWhiteSpace(option))
            {
                int priority = 1;

                string[] attributeNames = option.Split(',')
                    .Where<string>(x => !String.IsNullOrWhiteSpace(x))
                    .Select<string, string>(x => x.Trim())
                    .ToArray<string>();

                foreach (string attributeName in attributeNames)
                {
                    _internalDictionary[attributeName] = new AttributeOrderRule()
                        {
                            AttributeTokenType = tokenType,
                            Priority = priority
                        };

                    priority++;
                }
            }

            return this;
        }

        #endregion Methods
    }
}