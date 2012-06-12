using System;

namespace XamlStyler.Core
{
    public class AttributeOrderRule
    {
        public AttributeOrderRule()
        {
            AttributeTokenType = AttributeTokenInfoEnum.Other;
            Priority = 0;
        }

        public AttributeTokenInfoEnum AttributeTokenType
        {
            get;
            set;
        }

        public int Priority
        {
            get;
            set;
        }
    }
}