using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamlStyler.Core;

namespace XamlStyler.Tests
{
    [TestClass]
    public class FormattingTests
    {
        [TestMethod]
        [DeploymentItem(@"TestFiles\TestAttributeThresholdHandling.xaml")]
        [DeploymentItem(@"TestFiles\TestAttributeThresholdHandling_output_expected.xaml")]
        public void TestAttributeThresholdHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            Styler styler = new Styler();
            styler.Options.AttributesTolerance = 0;
            styler.Options.MaxAttributeCharatersPerLine = 80;
            styler.Options.MaxAttributesPerLine = 3;
            styler.Options.PutEndingBracketOnNewLine = true;

            DoTest(testInput, styler);
        }

        [TestMethod]
        [DeploymentItem(@"TestFiles\TestCommentHandling.xaml")]
        [DeploymentItem(@"TestFiles\TestCommentHandling_output_expected.xaml")]
        public void TestCommentHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }

        [TestMethod]
        [DeploymentItem(@"TestFiles\TestDefaultHandling.xaml")]
        [DeploymentItem(@"TestFiles\TestDefaultHandling_output_expected.xaml")]
        public void TestDefaultHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }

        [TestMethod]
        [DeploymentItem(@"TestFiles\TestAttributeSortingOptionHandling.xaml")]
        [DeploymentItem(@"TestFiles\TestAttributeSortingOptionHandling_output_expected.xaml")]
        public void TestAttributeSortingOptionHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            Styler styler = new Styler();
            styler.Options.AttributeOrderClass = "x:Class";
            styler.Options.AttributeOrderWpfNamespace = "xmlns, xmlns:x";
            styler.Options.AttributeOrderKey = "Key, x:Key";
            styler.Options.AttributeOrderName = "Name, x:Name, Title";
            styler.Options.AttributeOrderAttachedLayout = "Grid.Column, Grid.ColumnSpan, Grid.Row, Grid.RowSpan, Canvas.Right, Canvas.Bottom, Canvas.Left, Canvas.Top";
            styler.Options.AttributeOrderCoreLayout = "MinWidth, MinHeight, Width, Height, MaxWidth, MaxHeight, Margin";
            styler.Options.AttributeOrderAlignmentLayout = "Panel.ZIndex, HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment";
            styler.Options.AttributeOrderOthers = "Offset, Color, TargetName, Property, Value, StartPoint, EndPoint, PageSource, PageIndex";
            styler.Options.AttributeOrderBlendRelated = "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText";

            DoTest(testInput, styler);
        }

        [TestMethod]
        [DeploymentItem(@"TestFiles\TestMarkupExtensionHandling.xaml")]
        [DeploymentItem(@"TestFiles\TestMarkupExtensionHandling_output_expected.xaml")]
        public void TestMarkupExtensionHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            Styler styler = new Styler();
            styler.Options.FormatMarkupExtension = true;

            DoTest(testInput, styler);
        }

        [TestMethod]
        [DeploymentItem(@"TestFiles\TestNoContentElementHandling.xaml")]
        [DeploymentItem(@"TestFiles\TestNoContentElementHandling_output_expected.xaml")]
        public void TestNoContentElementHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }

        [TestMethod]
        [DeploymentItem(@"TestFiles\TestTextOnlyContentElementHandling.xaml")]
        [DeploymentItem(@"TestFiles\TestTextOnlyContentElementHandling_output_expected.xaml")]
        public void TestTextOnlyContentElementHandling()
        {
            string testInput = MethodBase.GetCurrentMethod().Name + ".xaml";

            DoTest(testInput);
        }

        private void DoTest(string testInput)
        {
            DoTest(testInput, new Styler());
        }

        private void DoTest(string testInput, Styler styler)
        {
            string actualOutputFile = testInput.Replace(".xaml", "_output.xaml");
            string expectedOutputFile = testInput.Replace(".xaml", "_output_expected.xaml");

            string output = styler.FormatFile(testInput);

            File.WriteAllText(actualOutputFile, output);

            Assert.IsTrue(this.FileCompare(actualOutputFile, expectedOutputFile));
        }

        private bool FileCompare(string file1, string file2)
        {
            int file1Byte;
            int file2Byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1Byte = fs1.ReadByte();
                file2Byte = fs2.ReadByte();
            }
            while ((file1Byte == file2Byte) && (file1Byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is
            // equal to "file2byte" at this point only if the files are
            // the same.
            return ((file1Byte - file2Byte) == 0);
        }
    }
}