namespace XamlStyler.XamlStylerVSPackage.Options
{
    using System;

    public interface IStylerOptions
    {
        #region Properties

        string AttributeOrderAlignmentLayout
        {
            get; set;
        }

        string AttributeOrderAttachedLayout
        {
            get; set;
        }

        string AttributeOrderBlendRelated
        {
            get; set;
        }

        string AttributeOrderClass
        {
            get; set;
        }

        string AttributeOrderCoreLayout
        {
            get; set;
        }

        string AttributeOrderKey
        {
            get; set;
        }

        string AttributeOrderName
        {
            get; set;
        }

        string AttributeOrderOthers
        {
            get; set;
        }

        string AttributeOrderWpfNamespace
        {
            get; set;
        }

        int AttributesTolerance
        {
            get; set;
        }

        bool FormatMarkupExtension
        {
            get; set;
        }

        int MaxAttributeCharatersPerLine
        {
            get; set;
        }

        int MaxAttributesPerLine
        {
            get; set;
        }

        string NoNewLineElements
        {
            get; set;
        }

        bool PutEndingBracketOnNewLine
        {
            get; set;
        }

        bool RemoveEndingTagOfEmptyElement
        {
            get; set;
        }

        bool BeautifyOnSave { get; set; }

        #endregion Properties
    }
}