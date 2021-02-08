using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using DesktopContainer.Utils.Win32Api.Workarounds;

namespace DesktopContainer.Utils.Win32Api
{
    public static class NativeMethods
    {
        
        /// <summary>
        /// Изменяет размер, позицию и Z-последовательность дочернего, выскакивающего или верхнего уровня окна. Дочерние, выскакивающие и верхнего уровня окна размещаются по порядку согласно их появлению на экране.
        /// </summary>
        /// <param name="hWnd">Дескриптор изменяемого окна.</param>
        /// <param name="hWndInsertAfter">Дескриптор окна, за которым размещается изменяемое окно.</param>
        /// <param name="x">Позиция по горизонтали.</param>
        /// <param name="y">Позиция по вертикали.</param>
        /// <param name="cx">Ширина.</param>
        /// <param name="cy">Высота.</param>
        /// <param name="uFlags">Флаги позиционирования окна и поведения функции.</param>
        /// <returns><see langword="true"/> если изменение положения окна произошло, <see langword="false"/> в случае неуспешного выполнения операции.</returns>
        [DllImport("user32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
    }
}
