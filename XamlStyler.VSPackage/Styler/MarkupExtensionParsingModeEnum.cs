namespace XamlStyler.XamlStylerVSPackage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #region Enumerations

    public enum MarkupExtensionParsingModeEnum
    {
        START,

        MARKUP_NAME,

        NAME_VALUE_PAIR,

        MARKUP_EXTENSION_VALUE,

        QUOTED_LITERAL_VALUE,

        LITERAL_VALUE,

        END,

        UNEXPECTED
    }

    #endregion Enumerations
}