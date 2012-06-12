using System;
using System.Collections.Generic;

namespace XamlStyler.Core
{
    public class MarkupExtensionInfo
    {
        public MarkupExtensionInfo()
        {
            ValueOnlyProperties = new List<object>();
            KeyValueProperties = new List<KeyValuePair<string, object>>();
        }

        public IList<KeyValuePair<string, object>> KeyValueProperties
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IList<object> ValueOnlyProperties
        {
            get;
            set;
        }
    }
}