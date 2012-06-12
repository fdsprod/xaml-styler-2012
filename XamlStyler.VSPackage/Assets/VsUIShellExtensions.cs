using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;

namespace XamlStyler.VSPackage
{
    public static class VsUIShellExtensions
    {
        public static void ShowMessageBox(this IVsUIShell source, string caption, string message)
        {
            var guid = Guid.Empty;
            int result;

            source.ShowMessageBox(0, ref guid, caption, message, string.Empty, 0,
                                  OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                  OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                                  OLEMSGICON.OLEMSGICON_INFO,
                                  0, out result);

        }
    }
}
