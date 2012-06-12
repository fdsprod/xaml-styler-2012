using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamlStyler.Core
{
    public class AttributeOrderRules
    {
        private readonly IDictionary<string, AttributeOrderRule> internalDictionary;

        public AttributeOrderRules(IStylerOptions options)
        {
            internalDictionary = new Dictionary<string, AttributeOrderRule>();

            this.Populate(options.AttributeOrderWpfNamespace, AttributeTokenInfoEnum.WpfNamespace)
                .Populate(options.AttributeOrderClass, AttributeTokenInfoEnum.Class)
                .Populate(options.AttributeOrderKey, AttributeTokenInfoEnum.Key)
                .Populate(options.AttributeOrderName, AttributeTokenInfoEnum.Name)
                .Populate(options.AttributeOrderAttachedLayout, AttributeTokenInfoEnum.AttachedLayout)
                .Populate(options.AttributeOrderCoreLayout, AttributeTokenInfoEnum.CoreLayout)
                .Populate(options.AttributeOrderAlignmentLayout, AttributeTokenInfoEnum.AlignmentLayout)
                .Populate(options.AttributeOrderOthers, AttributeTokenInfoEnum.Other)
                .Populate(options.AttributeOrderBlendRelated, AttributeTokenInfoEnum.BlendRelated);
        }

        public bool ContainsRuleFor(string name)
        {
            return internalDictionary.Keys.Contains(name);
        }

        public AttributeOrderRule GetRuleFor(string attributeName)
        {
            AttributeOrderRule result = null;

            if (internalDictionary.Keys.Contains(attributeName))
            {
                result = internalDictionary[attributeName];
            }
            else
            {
                AttributeTokenInfoEnum tempAttributeTokenType = AttributeTokenInfoEnum.Other;

                if (attributeName.StartsWith("xmlns"))
                {
                    tempAttributeTokenType = AttributeTokenInfoEnum.OtherNamespace;
                }
                else
                {
                    tempAttributeTokenType = AttributeTokenInfoEnum.Other;
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
                    internalDictionary[attributeName] = new AttributeOrderRule()
                        {
                            AttributeTokenType = tokenType,
                            Priority = priority
                        };

                    priority++;
                }
            }

            return this;
        }
    }
}