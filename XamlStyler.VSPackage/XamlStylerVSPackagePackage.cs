using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using XamlStyler.XamlStylerVSPackage.Options;
using XamlStyler.XamlStylerVSPackage.StylerModels;

namespace XamlStyler.XamlStylerVSPackage
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(StylerOptions), "Xaml Styler", "General", 101, 106, true)]
    [ProvideProfileAttribute(typeof(StylerOptions), "Xaml Styler", "Xaml Styler Settings", 106, 107, true, DescriptionResourceID = 108)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.SolutionExists)]
    [Guid(GuidList.GUID_XAML_STYLER_VS_PACKAGE_PKG_STRING)]
    public sealed class XamlStylerVSPackagePackage : Package
    {
        private DTE _dte = null;
        private EnvDTE.Events _events;
        private CommandEvents _fileSaveSelectedItems;
        private CommandEvents _fileSaveAll;
        private IVsUIShell _uiShell = null;

        public XamlStylerVSPackagePackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            _dte = this.GetService(typeof(DTE)) as DTE;

            if (_dte == null)
            {
                throw new NullReferenceException("DTE is null");
            }

            _uiShell = this.GetService(typeof(IVsUIShell)) as IVsUIShell;

            _events = _dte.Events as EnvDTE.Events;

            _fileSaveSelectedItems = _events.CommandEvents["{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 331];
            _fileSaveSelectedItems.BeforeExecute +=
                new _dispCommandEvents_BeforeExecuteEventHandler(OnFileSaveSelectedItemsBeforeExecute);

            _fileSaveAll = _events.CommandEvents["{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 224];
            _fileSaveAll.BeforeExecute +=
                new _dispCommandEvents_BeforeExecuteEventHandler(OnFileSaveAllBeforeExecute);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService menuCommandService = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (null != menuCommandService)
            {
                // Create the command for the menu item.
                CommandID menuCommandId = new CommandID(GuidList.GUID_XAML_STYLER_VS_PACKAGE_CMD_SET, (int)PkgCmdIdList.CMDID_BEAUTIFY_XAML);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
                menuCommandService.AddCommand(menuItem);
            }
        }

        private bool IsFormatableDocument(Document document)
        {
            return !document.ReadOnly && document.Language == "XAML";
        }

        private void OnFileSaveSelectedItemsBeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            Document document = _dte.ActiveDocument;

            if (IsFormatableDocument(document))
            {
                IStylerOptions options = this.GetDialogPage(typeof(StylerOptions)) as IStylerOptions;

                if (options.BeautifyOnSave)
                {
                    this.Execute(document);
                }
            }
        }

        private void OnFileSaveAllBeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            foreach (Document document in _dte.Documents)
            {
                if (IsFormatableDocument(document))
                {
                    IStylerOptions options = this.GetDialogPage(typeof(StylerOptions)) as IStylerOptions;

                    if (options.BeautifyOnSave)
                    {
                        this.Execute(document);
                    }
                }
            }
        }

        private void Execute(Document document)
        {
            if (!IsFormatableDocument(document))
            {
                return;
            }

            Properties xamlEditorProps = _dte.get_Properties("TextEditor", "XAML");
            //Properties xamlSpecificProps = _dte.get_Properties("TextEditor", "XAML Specific");

            bool insertTabs = (bool)xamlEditorProps.Item("InsertTabs").Value;
            IStylerOptions ideOptions = this.GetDialogPage(typeof(StylerOptions)) as IStylerOptions;

            Styler styler = new Styler()
            {
                IndentCharacter = insertTabs ? '\t' : ' ',
                IndentSize = Int32.Parse(xamlEditorProps.Item("IndentSize").Value.ToString()),
                //KeepFirstAttributeOnSameLine = (bool)xamlSpecificProps.Item("KeepFirstAttributeOnSameLine").Value,
                Options = ideOptions
            };

            TextDocument textDocument = (TextDocument)document.Object("TextDocument");

            TextPoint currentPoint = textDocument.Selection.ActivePoint;
            int originalLine = currentPoint.Line;
            int originalOffset = currentPoint.LineCharOffset;

            EditPoint startPoint = textDocument.StartPoint.CreateEditPoint();
            EditPoint endPoint = textDocument.EndPoint.CreateEditPoint();

            string xamlSource = startPoint.GetText(endPoint);
            xamlSource = styler.Format(xamlSource);

            startPoint.ReplaceText(endPoint, xamlSource, 0);

            if (originalLine <= textDocument.EndPoint.Line)
            {
                textDocument.Selection.MoveToLineAndOffset(originalLine, originalOffset, false);
            }
            else
            {
                textDocument.Selection.GotoLine(textDocument.EndPoint.Line);
            }

        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                _uiShell.SetWaitCursor();

                Document document = _dte.ActiveDocument;

                if (IsFormatableDocument(document))
                {
                    this.Execute(document);
                }
            }
            catch (Exception ex)
            {
                string title = string.Format("Error in {0}:", this.GetType().Name);
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}\r\n\r\nIf this deems a malfunctioning of styler, please kindly submit an issue at https://github.com/Windcape/xaml-styler-2012/issues.",
                    ex.Message);

                this.ShowMessageBox(title, message);
            }
        }

        private void ShowMessageBox(string title, string message)
        {
            Guid clsid = Guid.Empty;
            int result;

            _uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       title,
                       message,
                       String.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result);
        }
    }
}