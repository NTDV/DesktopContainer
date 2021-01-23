using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DesktopContainer.Services
{
    /// <summary>
    /// Константные "дескрипторы" окон.
    /// </summary>
    public enum HwndsInsertAfter
        {
            /// <summary>
            /// Помещает окно наверху Z-последовательности.
            /// </summary>
            HwndTop = 0,
            /// <summary>
            /// Помещает окно внизу Z-последовательности. Если параметр hWnd идентифицирует самое верхнее окно, окно теряет своё самое верхнее состояние и помещается внизу всех других окон.
            /// </summary>
            HwndBottom = 1,
            /// <summary>
            /// Помещает окно перед не самыми верхними окнами. Окно сохраняет свою самую верхнюю позицию даже тогда, когда оно неактивное.
            /// </summary>
            HwndTopmost = -1,
            /// <summary>
            /// омещает окно перед всеми не в самыми верхними окнами (то есть позади всех самых верхних окон). Этот флажок не имеет никакого влияния, если окно - уже не самое верхнее окно.
            /// </summary>
            HwndNotopmost = -2,
        }

    /// <summary>
        /// Определяет флажки, устанавливающие размеры и позиционирование окна. Может быть комбинацией значений.
        /// </summary>
    [Flags]
    public enum UFlags
        {
            /// <summary>
            /// Сохраняет текущий размер (игнорирует cx и cy параметры).
            /// </summary>
            SwpNoSize = 0x0001,
            /// <summary>
            /// Сохраняет текущую позицию (игнорирует X и Y параметры).
            /// </summary>
            SwpNoMove = 0x0002,
            /// <summary>
            /// Сохраняет текущую Z-последовательность (игнорирует параметр hWndInsertAfter).
            /// </summary>
            SwpNoZOrder = 0x0004,
            /// <summary>
            /// Не перерисовывает изменения. Если этот флажок установлен, то не происходит никакой перерисовки любого вида. Это применяется к рабочей области, нерабочей области (включая строку заголовка и линейки прокрутки) и любую часть родительского окна, раскрытого в результате перемещения окна. Когда этот флажок установлен, прикладная программа должна явно лишить законной силы или перерисовывать любые части окна и родительского окна, которые требуют перерисовки.
            /// </summary>
            SwpNoRedraw = 0x0008,
            /// <summary>
            /// Не активизирует окно. Если этот флажок не установлен, окно активизируется и перемещается в верхнюю часть или самой верхней, или не самой верхней группы (в зависимости от установки параметра hWndInsertAfter).
            /// </summary>
            SwpNoActivate = 0x0010,
            /// <summary>
            /// Посылает сообщение WM_NCCALCSIZE окну, даже тогда, когда размер окна не изменяется. Если этот флажок не установлен, WM_NCCALCSIZE посылается только тогда, когда размер окна изменяется.
            /// </summary>
            SwpFramechanged = 0x0020,
            /// <summary>
            /// Отображает окно.
            /// </summary>
            SwpShowWindow = 0x0040,
            /// <summary>
            /// Скрывает окно.
            /// </summary>
            SwpHideWindow = 0x0080,
            /// <summary>
            /// Сбрасывает все содержание рабочей области. Если этот флажок не установлен, допустимое содержание рабочей области сохраняется и копируется обратно в рабочую область после того, как окно установлено по размеру или переустановлено.
            /// </summary>
            SwpNoCopyBits = 0x0100,
            /// <summary>
            /// Не изменяет позицию окна владельца в Z-последовательности.
            /// </summary>
            SwpNoOwnerZOrder = 0x0200,
            /// <summary>
            /// Предохраняет окно от приема сообщения WM_WINDOWPOSCHANGING.
            /// </summary>
            SwpNoSendChanging = 0x0400,
            /// <summary>
            /// Выводит рамку (определенную в описании класса окна) вокруг окна.
            /// </summary>
            SwpDrawFrame = SwpFramechanged,
            /// <summary>
            /// Не изменяет позицию окна владельца в Z-последовательности.
            /// </summary>
            SwpNoReposition = SwpNoOwnerZOrder,
            /// <summary>
            /// Исключает генерацию WM_SYNCPAINT сообщения.
            /// </summary>
            SwpDeferErase = 0x2000,
            /// <summary>
            /// Если вызывающий поток и поток, владеющий окном принадлежат разным очередям исполнения, то функция выполняется на стороне потока, владющего окном. Это позволяет избежать блокировки процесса. 
            /// </summary>
            SwpAsyncWindowPos = 0x4000,
        }

    /// <summary>
    /// Содержит логику изменения положения окна-контейнера.
    /// </summary>
    public class ContainerWindowPositionService
    {
        /// <summary>
        /// Дескрпитор окна-контейнера.
        /// </summary>
        private readonly IntPtr _windowHadle;

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
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        /// <summary>
        /// Инициализация сервиса управления положением окна-контейнера
        /// </summary>
        /// <remarks>
        /// Вызов конструктора этого класса следует производить после инициализации элементов окна. Внутри контструктора происходит получение или создание (при отстуствии) дескриптора окна. Если получить его до инициализации окна, то это запретит окну установить некоторые свойства, изменение которых приведёт к <see cref="T:System.Xaml.XamlException">XamlException</see> в инициализаторе окна.
        /// </remarks>
        /// <param name="window">Ссылка на окна, которым требуется управлять</param>
        public ContainerWindowPositionService(Window window)
        {
            _windowHadle = new WindowInteropHelper(window).EnsureHandle();
        }
        
        /// <summary>
        /// Переместить окно под все остальные, не уведомляя систему об изменениях и не активируя окно.
        /// </summary>
        /// <returns><see langword="true"/> если изменение положения окна произошло, <see langword="false"/> в случае неуспешного выполнения операции.</returns>
        public bool SetWindowUndermost() =>
            SetWindowPos(_windowHadle, (IntPtr) HwndsInsertAfter.HwndBottom,
                0, 0, 0, 0, 
                (uint) (UFlags.SwpNoSize | UFlags.SwpNoMove | UFlags.SwpNoActivate | UFlags.SwpAsyncWindowPos));
    }
}
