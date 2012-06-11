namespace XamlStyler.XamlStylerVSPackage.StylerModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AttributeOrderRule
    {
        #region Fields

        public AttributeTokenInfoEnum AttributeTokenType = AttributeTokenInfoEnum.OTHER;
        public int Priority = 0;

        #endregion Fields
    }
}