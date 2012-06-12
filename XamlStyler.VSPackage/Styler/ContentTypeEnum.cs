using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamlStyler.XamlStylerVSPackage
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