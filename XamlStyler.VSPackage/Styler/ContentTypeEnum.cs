﻿namespace XamlStyler.XamlStylerVSPackage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #region Enumerations

    [Flags]
    public enum ContentTypeEnum
    {
        NONE = 0,

        SINGLE_LINE_TEXT_ONLY = 1,

        MULTI_LINE_TEXT_ONLY = 2,

        MIXED = 4
    }

    #endregion Enumerations
}