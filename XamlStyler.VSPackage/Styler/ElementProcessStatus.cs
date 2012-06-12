using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamlStyler.XamlStylerVSPackage
{
    public class ElementProcessStatus
    {
        public ContentTypeEnum ContentType
        {
            get;
            set;
        }

        public bool IsMultlineStartTag
        {
            get;
            set;
        }

        public bool IsSelfClosingElement
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}