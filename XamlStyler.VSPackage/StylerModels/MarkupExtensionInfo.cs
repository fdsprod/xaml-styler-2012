﻿namespace XamlStyler.XamlStylerVSPackage.StylerModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class MarkupExtensionInfo
    {
        #region Constructors

        public MarkupExtensionInfo()
        {
            ValueOnlyProperties = new List<object>();
            KeyValueProperties = new List<KeyValuePair<string, object>>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Value could be string or MarkupExtensionInfo
        /// </summary>
        public IList<KeyValuePair<string, object>> KeyValueProperties
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Value could be string or MarkupExtensionInfo
        /// </summary>
        public IList<object> ValueOnlyProperties
        {
            get; set;
        }

        #endregion Properties
    }
}