using System;

namespace DesktopContainer.Utils.Win32Api.Workarounds
{
    public sealed class NativeConstants
    {

    }

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
    public enum UFlags : uint
        {
            /// <summary>
            /// Сохраняет текущий размер (игнорирует cx и cy параметры).
            /// </summary>
            SwpNoSize = 0x_0001,
            /// <summary>
            /// Сохраняет текущую позицию (игнорирует X и Y параметры).
            /// </summary>
            SwpNoMove = 0x_0002,
            /// <summary>
            /// Сохраняет текущую Z-последовательность (игнорирует параметр hWndInsertAfter).
            /// </summary>
            SwpNoZOrder = 0x_0004,
            /// <summary>
            /// Не перерисовывает изменения. Если этот флажок установлен, то не происходит никакой перерисовки любого вида. Это применяется к рабочей области, нерабочей области (включая строку заголовка и линейки прокрутки) и любую часть родительского окна, раскрытого в результате перемещения окна. Когда этот флажок установлен, прикладная программа должна явно лишить законной силы или перерисовывать любые части окна и родительского окна, которые требуют перерисовки.
            /// </summary>
            SwpNoRedraw = 0x_0008,
            /// <summary>
            /// Не активизирует окно. Если этот флажок не установлен, окно активизируется и перемещается в верхнюю часть или самой верхней, или не самой верхней группы (в зависимости от установки параметра hWndInsertAfter).
            /// </summary>
            SwpNoActivate = 0x_0010,
            /// <summary>
            /// Посылает сообщение WM_NCCALCSIZE окну, даже тогда, когда размер окна не изменяется. Если этот флажок не установлен, WM_NCCALCSIZE посылается только тогда, когда размер окна изменяется.
            /// </summary>
            SwpFramechanged = 0x_0020,
            /// <summary>
            /// Отображает окно.
            /// </summary>
            SwpShowWindow = 0x_0040,
            /// <summary>
            /// Скрывает окно.
            /// </summary>
            SwpHideWindow = 0x_0080,
            /// <summary>
            /// Сбрасывает все содержание рабочей области. Если этот флажок не установлен, допустимое содержание рабочей области сохраняется и копируется обратно в рабочую область после того, как окно установлено по размеру или переустановлено.
            /// </summary>
            SwpNoCopyBits = 0x_0100,
            /// <summary>
            /// Не изменяет позицию окна владельца в Z-последовательности.
            /// </summary>
            SwpNoOwnerZOrder = 0x_0200,
            /// <summary>
            /// Предохраняет окно от приема сообщения WM_WINDOWPOSCHANGING.
            /// </summary>
            SwpNoSendChanging = 0x_0400,
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
            SwpDeferErase = 0x_2000,
            /// <summary>
            /// Если вызывающий поток и поток, владеющий окном принадлежат разным очередям исполнения, то функция выполняется на стороне потока, владющего окном. Это позволяет избежать блокировки процесса. 
            /// </summary>
            SwpAsyncWindowPos = 0x_4000,
        }

    public enum WindowCompositionAttribute : uint
    {
        NcRenderingEnabled = 1,     //Get only atttribute
        NcRenderingPolicy,          //Enable or disable non-client rendering
        TransitionsForceDisabled,
        AllowNcPaint,
        CaptionButtonBounds,
        NonClientRtlLayout,
        ForceIconicRepresentation,
        Flip3DPolicy,
        ExtendedFrameBounds,
        HasIconicBitmap,
        DisallowPeek,
        ExcludedFromPeek,
        Cloak,
        Cloaked,
        FreezeRepresentation,
        PlaceHolder1,
        PlaceHolder2,
        PlaceHolder3,
        AccentPolicy = 19
    }

    public enum AccentState
    {
        AccentDisabled = 0,
        AccentEnableGradient = 1,
        AccentEnableTransparentgradient = 2,
        AccentEnableBlurbehind = 3,
        AccentEnableAcrylicblurbehind = 4,
        AccentInvalidState = 5
    }
}
