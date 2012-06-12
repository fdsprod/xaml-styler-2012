using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XamlStyler.XamlStylerVSPackage.StylerModels
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