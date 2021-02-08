using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using DesktopContainer.Utils.StaticHelpers;
using DesktopContainer.Utils.Win32Api;
using DesktopContainer.Utils.Win32Api.Workarounds;

namespace DesktopContainer.Services
{
    /// <summary>
    /// Содержит логику изменения положения окна.
    /// </summary>
    public class WindowPositionService
    {
        /// <summary>
        /// Дескрпитор окна.
        /// </summary>
        private readonly IntPtr _windowHandle;

        /// <summary>
        /// Инициализация сервиса управления положением окна.
        /// </summary>
        /// <remarks>
        /// Вызов конструктора этого класса следует производить после инициализации элементов окна. Внутри контструктора происходит получение или создание (при отстуствии) дескриптора окна. Если получить его до инициализации окна, то это запретит окну установить некоторые свойства, изменение которых приведёт к <see cref="T:System.Xaml.XamlException">XamlException</see> в инициализаторе окна.
        /// </remarks>
        /// <param name="window">Ссылка на окна, которым требуется управлять</param>
        public WindowPositionService(Window window)
        {
            _windowHandle = WindowHandleWorker.GetWindowHandler(window);;
        }

        public WindowPositionService(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
        }
        
        /// <summary>
        /// Переместить окно ниже всех остальные, не уведомляя систему об изменениях и не активируя окно.
        /// </summary>
        /// <returns><see langword="true"/> если изменение положения окна произошло, <see langword="false"/> в случае неуспешного выполнения операции.</returns>
        public bool SetWindowUndermost() =>
            NativeMethods.SetWindowPos(_windowHandle, (IntPtr) HwndsInsertAfter.HwndBottom,
                0, 0, 0, 0,
                (uint)(UFlags.SwpNoSize | UFlags.SwpNoMove | UFlags.SwpNoActivate | UFlags.SwpAsyncWindowPos));
    }
}
