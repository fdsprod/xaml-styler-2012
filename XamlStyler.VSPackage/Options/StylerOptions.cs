﻿namespace XamlStyler.XamlStylerVSPackage.Options
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    using Microsoft.VisualStudio.Shell;

    public class StylerOptions : DialogPage, IStylerOptions
    {
        #region Constructors

        public StylerOptions()
        {
            #region New Line
            
            this.AttributesTolerance = 2;
            this.MaxAttributesPerLine = 1;
            this.MaxAttributeCharatersPerLine = 0;
            this.NoNewLineElements = "RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter";
            this.PutEndingBracketOnNewLine = false;
            this.RemoveEndingTagOfEmptyElement = true; 

            #endregion
            
            #region Markup Extension
            
            this.FormatMarkupExtension = true; 
            
            #endregion

            #region Attribute Ordering Rule Related Default Options

            this.AttributeOrderClass = "x:Class";

            this.AttributeOrderWpfNamespace = "xmlns, xmlns:x";

            this.AttributeOrderKey = "Key, x:Key";
            this.AttributeOrderName = "Name, x:Name, Title";

            this.AttributeOrderAttachedLayout = "Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom";
            this.AttributeOrderCoreLayout = "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight, Margin";
            this.AttributeOrderAlignmentLayout = "HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex";

            this.AttributeOrderOthers = "PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint";

            this.AttributeOrderBlendRelated = "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText";

            #endregion

            #region Misc

            this.BeautifyOnSave = true;

            #endregion
        }

        #endregion Constructors

        #region Properties

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#1 Class definition group")]
        [Description("Defines ordering rule of class definition attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.")]
        [DefaultValue("x:Class")]
        public string AttributeOrderClass
        {
            get;
            set;
        }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#2 WPF Namespaces group")]
        [Description("Defines ordering rule of wpf namespaces.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.")]
        [DefaultValue("xmlns, xmlns:x")]
        public string AttributeOrderWpfNamespace
        {
            get;
            set;
        }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#3 Element key group")]
        [Description("Defines ordering rule of element key.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.")]
        [DefaultValue("Key, x:Key")]
        public string AttributeOrderKey
        {
            get;
            set;
        }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#4 Element name group")]
        [Description("Defines ordering rule of element name.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.")]
        [DefaultValue("Name, x:Name, Title")]
        public string AttributeOrderName
        {
            get; set;
        }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#5 Attached layout group")]
        [Description("Defines ordering rule of attached layout attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.")]
        [DefaultValue("Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom")]
        public string AttributeOrderAttachedLayout
        {
            get;
            set;
        }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#6 Core layout group")]
        [Description("Defines ordering rule of core layout attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.")]
        [DefaultValue("Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight, Margin")]
        public string AttributeOrderCoreLayout
        {
            get;
            set;
        }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#7 Alignment layout group")]
        [Description("Defines ordering rule of alignment layout attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.")]
        [DefaultValue("HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex")]
        public string AttributeOrderAlignmentLayout
        {
            get;
            set;
        }
        
        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#8 Miscellaneous attributes group")]
        [Description("Defines ordering rule of miscellaneous attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.\r\n**Attributes not listed in any ordring rule groups, are implicitly appended to this group in alphabetic order.")]
        [DefaultValue("PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint")]
        public string AttributeOrderOthers
        {
            get; set;
        }

        [Category("Attribute Ordering Rule Groups")]
        [DisplayName("#9 Blend related group")]
        [Description("Defines ordering rule of blend related attributes.\r\nUse ',' to seperate more than one attribute.\r\nAttributes listed in earlier group takes precedence than later groups.\r\nAttributes listed earlier in same group takes precedence than the ones listed later.")]
        [DefaultValue("mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText")]
        public string AttributeOrderBlendRelated
        {
            get;
            set;
        }

        [Category("New Line")]
        [DisplayName("Attribute tolerance")]
        [Description("Defines the attribute number tolerance before XamlStyler starts to break attributes into new lines. A value less than 1 meaning no tolerance.\r\ne.g., when this setting is 2\r\n\r\nBEFORE BEAUTIFY:\r\n<TextBlock x:Name=\"m_sample\"\r\n    Text=\"asdf\" />\r\n\r\nAFTER BEAUTIFY:\r\n<TextBlock x:Name=\"m_sample\" Text=\"asdf\" />\r\nDefault Value: 2")]
        [DefaultValue(2)]
        public int AttributesTolerance
        {
            get; set;
        }

        [Category("Markup Extension")]
        [DisplayName("Enable Markup Extension Formatting")]
        [Description("Defines whether to format markup extension.\r\nDefalut Value: true\r\nWhen this setting is true, attributes with markup extension will always be put on a new line, UNLESS the element is under AttributesTolerance or one of the NoNewLineElements.")]
        [DefaultValue(true)]
        public bool FormatMarkupExtension
        {
            get; set;
        }

        [Category("New Line")]
        [DisplayName("Max attribute characters per line")]
        [Description("Defines the maximum charater length (not including indentation characters) of attributes an element can have on each line after the start tag. A value less than 1 meaning no limit.\r\nDefault Value: 0")]
        [DefaultValue(0)]
        public int MaxAttributeCharatersPerLine
        {
            get; set;
        }

        [Category("New Line")]
        [DisplayName("Max attribute number per line")]
        [Description("Defines the maximum number of attributes an element can have on each line after the start tag. A value less than 1 meaning no limit.\r\nDefault Value: 1")]
        [DefaultValue(1)]
        public int MaxAttributesPerLine
        {
            get; set;
        }

        [Category("New Line")]
        [DisplayName("Elements no line break between attributes")]
        [Description("Defines a list of elements whose attributes shall not be broken into lines.\r\nDefault Value: RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Setter")]
        [DefaultValue("RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter")]
        public string NoNewLineElements
        {
            get; set;
        }

        [Category("New Line")]
        [DisplayName("Put ending bracket on new line")]
        [Description("Defines whether to put \">\" or \"/>\" on a new line.\r\nDefault Value: false")]
        [DefaultValue(false)]
        public bool PutEndingBracketOnNewLine
        {
            get; set;
        }

        [Category("New Line")]
        [DisplayName("Remove ending tag of empty element")]
        [Description("Defines whether to remove the ending tag of an empty element.\r\ne.g., when this setting is true\r\n\r\nBEFORE BEAUTIFY:\r\n\"<element>  </element>\"\r\n\r\nAFTER BEAUTIFY:\r\n\"<element />\"\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool RemoveEndingTagOfEmptyElement
        {
            get; set;
        }

        [Category("Misc")]
        [DisplayName("Beautify on saving xaml")]
        [Description("Defines whether to automatically beautify the active xaml document while saving.")]
        [DefaultValue(true)]
        public bool BeautifyOnSave
        {
            get;
            set;
        }
        
        #endregion Properties
    }
}