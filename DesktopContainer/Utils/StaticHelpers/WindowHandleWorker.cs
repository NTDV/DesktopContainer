using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DesktopContainer.Utils.StaticHelpers
{
    public static class WindowHandleWorker
    {
        public static IntPtr GetWindowHandler(Window window) => new WindowInteropHelper(window).EnsureHandle();
    }
}
