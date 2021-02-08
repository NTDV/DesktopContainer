using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using DesktopContainer.Utils.Win32Api;
using DesktopContainer.Utils.Win32Api.Workarounds;
using Microsoft.WindowsAPICodePack.Shell;

namespace DesktopContainer.Utils.StaticHelpers
{
    public static class FileIconWorker
    {
        public static BitmapSource GetThumbnail(string path) => ShellObject.FromParsingName(path).Thumbnail.BitmapSource;
    }
}
