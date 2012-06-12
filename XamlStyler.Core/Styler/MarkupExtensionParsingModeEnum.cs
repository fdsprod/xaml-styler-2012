using System;

namespace XamlStyler.Core
{
    public enum MarkupExtensionParsingModeEnum
    {
        Start,
        MarkupName,
        NameValuePair,
        MarkupExtensionValue,
        QuotedLiteralValue,
        LiteralValue,
        End,
        Unexpected
    }
}