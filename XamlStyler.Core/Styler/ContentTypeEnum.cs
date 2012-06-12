using System;

namespace XamlStyler.Core
{
    [Flags]
    public enum ContentTypeEnum
    {
        None = 0,
        SingleLineTextOnly = 1,
        MultiLineTextOnly = 2,
        Mixed = 4
    }
}