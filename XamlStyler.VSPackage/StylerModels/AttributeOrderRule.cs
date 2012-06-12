using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamlStyler.XamlStylerVSPackage.StylerModels
{
    public class AttributeOrderRule
    {
        public AttributeTokenInfoEnum AttributeTokenType = AttributeTokenInfoEnum.Other;
        public int Priority = 0;
    }
}