namespace XamlStyler.XamlStylerVSPackage
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