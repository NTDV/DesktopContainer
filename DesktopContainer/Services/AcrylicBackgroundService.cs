using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using DesktopContainer.Utils.StaticHelpers;
using DesktopContainer.Utils.Win32Api;
using DesktopContainer.Utils.Win32Api.Workarounds;

namespace DesktopContainer.Services
{
    public class AcrylicBackgroundService
    {
        /// <summary>
        /// Дескрпитор окна.
        /// </summary>
        private readonly IntPtr _windowHandle;

        /// <summary>
        /// Инициализация сервиса управления фоном окна.
        /// </summary>
        /// <remarks>
        /// Вызов конструктора этого класса следует производить после инициализации элементов окна. Внутри контструктора происходит получение или создание (при отстуствии) дескриптора окна. Если получить его до инициализации окна, то это запретит окну установить некоторые свойства, изменение которых приведёт к <see cref="T:System.Xaml.XamlException">XamlException</see> в инициализаторе окна.
        /// </remarks>
        /// <param name="window">Ссылка на окна, которым требуется управлять</param>
        public AcrylicBackgroundService(Window window)
        {
            _windowHandle = WindowHandleWorker.GetWindowHandler(window);
        }

        public AcrylicBackgroundService(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
        }

        public static int Windows10EnableBlurBehind(IntPtr hWnd)
        {
            var accent = new AccentPolicy
            {
                AccentState = AccentState.AccentEnableBlurbehind
            };
            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.AccentPolicy,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            var ret = NativeMethods.SetWindowCompositionAttribute(hWnd, ref data);
            Marshal.FreeHGlobal(accentPtr);
            return ret;
        }

        public int Windows10EnableBlurBehind() => Windows10EnableBlurBehind(_windowHandle);
    }
}
