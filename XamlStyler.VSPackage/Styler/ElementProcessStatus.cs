﻿namespace XamlStyler.XamlStylerVSPackage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ElementProcessStatus
    {
        #region Properties

        /// <summary>
        /// Gets or sets the content type of current element.
        /// E.g., 
        ///     <Element></Element> : ContentTypeEnum.NONE
        ///     <Element>asdf<OtherElements/></Element> ContentTypeEnum.TEXT_ONLY
        ///     <Element>asdf<OtherElements/></Element> ContentTypeEnum.MIXED
        /// </summary>
        public ContentTypeEnum ContentType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether the start tag of this element has been broken into multi-lines.
        /// </summary>
        public bool IsMultlineStartTag
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether the current element is self-closing.
        /// E.g., <Element/> is an self-closing element.
        /// </summary>
        public bool IsSelfClosingElement
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets Element name.
        /// </summary>
        public string Name
        {
            get; set;
        }

        #endregion Properties
    }
}