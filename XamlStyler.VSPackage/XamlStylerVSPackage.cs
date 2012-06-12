using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using XamlStyler.Core;

namespace XamlStyler.VSPackage
{
    /// <summary>
    /// The XAML Styler VS Package.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(StylerOptions), "XAML Styler", "General", 101, 106, true)]
    [ProvideProfileAttribute(typeof(StylerOptions), "XAML Styler", "XAML Styler Settings", 106, 107, true, DescriptionResourceID = 108)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.SolutionExists)]
    [Guid(GuidList.GUID_XAML_STYLER_VS_PACKAGE_PKG_STRING)]
    public sealed class XamlStylerVSPackage : VSPackageBase
    {
        /// <summary>
        /// Gets the 'File > Save Selected Items' <see cref="CommandEvents"/> object.
        /// </summary>
        private CommandEvents FileSaveSelectedItems
        {
            get
            {
                return Events.CommandEvents["{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 331];
            }
        }

        /// <summary>
        /// Gets the 'File > Save All' <see cref="CommandEvents"/> object.
        /// </summary>
        private CommandEvents FileSaveAll
        {
            get
            {
                return Events.CommandEvents["{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 224];
            }
        }

        /// <summary>
        /// Gets the IDE XAML Styler Options.
        /// </summary>
        private IStylerOptions StylerOptions
        {
            get
            {
                return GetDialogPage(typeof(StylerOptions)) as IStylerOptions;
            }
        }

        /// <summary>
        /// Called when the VSPackage is loaded by Visual Studio.
        /// </summary>
        protected override void Initialize()
        {
            FileSaveSelectedItems.BeforeExecute += delegate { OnFileSaveSelectedItemsBeforeExecute(); };
            FileSaveAll.BeforeExecute += delegate { OnFileSaveAllBeforeExecute(); };

            var menuCommandService = GetService<IMenuCommandService, OleMenuCommandService>();
            if (menuCommandService != null)
            {
                var menuCommandId = new CommandID(GuidList.GUID_XAML_STYLER_VS_PACKAGE_CMD_SET, (int)PkgCmdIdList.CMDID_BEAUTIFY_XAML);

                var menuItem = new MenuCommand(OnMenuItemSelected, menuCommandId);
                menuCommandService.AddCommand(menuItem);
            }

            base.Initialize();
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="document"/> is formattable.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private bool IsFormattableDocument(Document document)
        {
            return !document.ReadOnly && document.Language == "XAML";
        }

        /// <summary>
        /// Beautifies a document.
        /// </summary>
        /// <param name="document">The document to beautify.</param>
        private void BeautifyDocument(Document document)
        {
            if (!IsFormattableDocument(document))
            {
                return;
            }

            var xamlEditorProps = DTE.get_Properties("TextEditor", "XAML");
            var insertTabs = (bool)xamlEditorProps.Item("InsertTabs").Value;

            var styler = new Styler()
            {
                IndentCharacter = insertTabs ? '\t' : ' ',
                IndentSize = Int32.Parse(xamlEditorProps.Item("IndentSize").Value.ToString()),
                Options = StylerOptions
            };

            var textDocument = document.Object("TextDocument") as TextDocument;

            var currentPoint = textDocument.Selection.ActivePoint;
            int originalLine = currentPoint.Line;
            int originalOffset = currentPoint.LineCharOffset;

            var startPoint = textDocument.StartPoint.CreateEditPoint();
            var endPoint = textDocument.EndPoint.CreateEditPoint();

            var xamlSource = styler.Format(startPoint.GetText(endPoint));

            startPoint.ReplaceText(endPoint, xamlSource, 0);

            if (originalLine <= textDocument.EndPoint.Line)
                textDocument.Selection.MoveToLineAndOffset(originalLine, originalOffset, false);
            else
                textDocument.Selection.GotoLine(textDocument.EndPoint.Line);
        }

        /// <summary>
        /// Occours before the 'File > Save Selected Items' command is executed in Visual Studio.
        /// </summary>
        private void OnFileSaveSelectedItemsBeforeExecute()
        {
            if (StylerOptions.BeautifyOnSave && IsFormattableDocument(DTE.ActiveDocument))
            {
                BeautifyDocument(DTE.ActiveDocument);
            }
        }

        /// <summary>
        /// Occours before the 'File > Save All' command is executed in Visual Studio.
        /// </summary>
        private void OnFileSaveAllBeforeExecute()
        {
            if (StylerOptions.BeautifyOnSave)
            {
                DTE.Documents.Cast<Document>().Where(IsFormattableDocument).ForEach(BeautifyDocument);
            }
        }

        /// <summary>
        /// Occours when the user selects the menu item or toolbar button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> that contains no event data.</param>
        private void OnMenuItemSelected(object sender, EventArgs e)
        {
#if !DEBUG
            try
            {
#endif
            VsUIShell.SetWaitCursor();

            var document = DTE.ActiveDocument;
            if (IsFormattableDocument(document))
            {
                this.BeautifyDocument(document);
            }
#if !DEBUG
            }
            catch (Exception ex)
            {
                var caption = string.Format("Error in {0}:", this.GetType().Name);
                var message = string.Format(CultureInfo.CurrentCulture, "{0}\r\n\r\nIf this deems a malfunctioning of styler, please kindly submit an issue at https://github.com/Windcape/xaml-styler-2012/issues.", ex.Message);

                VsUIShell.ShowMessageBox(caption, message);
            }
#endif
        }
    }
}