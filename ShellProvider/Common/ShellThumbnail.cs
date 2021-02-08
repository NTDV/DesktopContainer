//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using MS.WindowsAPICodePack.Internal;

using Size = System.Windows.Size;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    ///     Represents a thumbnail or an icon for a ShellObject.
    /// </summary>
    public class ShellThumbnail
    {
        #region Constructors

        /// <summary>
        ///     Internal constructor that takes in a parent ShellObject.
        /// </summary>
        /// <param name="shellObject"></param>
        internal ShellThumbnail(ShellObject shellObject)
        {
            if (shellObject == null || shellObject.NativeShellItem == null)
                throw new ArgumentNullException(nameof(shellObject));

            shellItemNative = shellObject.NativeShellItem;
        }

        #endregion

        #region Private members

        /// <summary>
        ///     Native shellItem
        /// </summary>
        private readonly IShellItem shellItemNative;

        /// <summary>
        ///     Internal member to keep track of the current size
        /// </summary>
        private Size currentSize = new Size(256, 256);

        #endregion

        #region Public properties

        /// <summary>
        ///     Gets or sets the default size of the thumbnail or icon. The default is 32x32 pixels for icons and
        ///     256x256 pixels for thumbnails.
        /// </summary>
        /// <remarks>
        ///     If the size specified is larger than the maximum size of 1024x1024 for thumbnails and 256x256 for icons,
        ///     an <see cref="System.ArgumentOutOfRangeException" /> is thrown.
        /// </remarks>
        public Size CurrentSize
        {
            get => currentSize;
            set
            {
                // Check for 0; negative number check not required as System.Windows.Size only allows positive numbers.
                if (value.Height == 0 || value.Width == 0)
                    throw new ArgumentOutOfRangeException("value", "ShellThumbnailSizeCannotBe0");

                var size = FormatOption == ShellThumbnailFormatOption.IconOnly
                    ? DefaultIconSize.Maximum
                    : DefaultThumbnailSize.Maximum;

                if (value.Height > size.Height || value.Width > size.Width)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        string.Format(CultureInfo.InvariantCulture,
                            "ShellThumbnailCurrentSizeRange", size.ToString()));

                currentSize = value;
            }
        }

        /// <summary>
        ///     Gets the thumbnail or icon image in <see cref="System.Drawing.Bitmap" /> format.
        ///     Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public Bitmap Bitmap => GetBitmap(CurrentSize);

        /// <summary>
        ///     Gets the thumbnail or icon image in <see cref="System.Windows.Media.Imaging.BitmapSource" /> format.
        ///     Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public BitmapSource BitmapSource => GetBitmapSource(CurrentSize);

        /// <summary>
        ///     Gets the thumbnail or icon image in <see cref="System.Drawing.Icon" /> format.
        ///     Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public Icon Icon => Icon.FromHandle(Bitmap.GetHicon());

        /// <summary>
        ///     Gets the thumbnail or icon in small size and <see cref="System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap SmallBitmap => GetBitmap(DefaultIconSize.Small, DefaultThumbnailSize.Small);

        /// <summary>
        ///     Gets the thumbnail or icon in small size and <see cref="System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource SmallBitmapSource => GetBitmapSource(DefaultIconSize.Small, DefaultThumbnailSize.Small);

        /// <summary>
        ///     Gets the thumbnail or icon in small size and <see cref="System.Drawing.Icon" /> format.
        /// </summary>
        public Icon SmallIcon => Icon.FromHandle(SmallBitmap.GetHicon());

        /// <summary>
        ///     Gets the thumbnail or icon in Medium size and <see cref="System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap MediumBitmap => GetBitmap(DefaultIconSize.Medium, DefaultThumbnailSize.Medium);

        /// <summary>
        ///     Gets the thumbnail or icon in medium size and <see cref="System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource MediumBitmapSource => GetBitmapSource(DefaultIconSize.Medium, DefaultThumbnailSize.Medium);

        /// <summary>
        ///     Gets the thumbnail or icon in Medium size and <see cref="System.Drawing.Icon" /> format.
        /// </summary>
        public Icon MediumIcon => Icon.FromHandle(MediumBitmap.GetHicon());

        /// <summary>
        ///     Gets the thumbnail or icon in large size and <see cref="System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap LargeBitmap => GetBitmap(DefaultIconSize.Large, DefaultThumbnailSize.Large);

        /// <summary>
        ///     Gets the thumbnail or icon in large size and <see cref="System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource LargeBitmapSource => GetBitmapSource(DefaultIconSize.Large, DefaultThumbnailSize.Large);

        /// <summary>
        ///     Gets the thumbnail or icon in Large size and <see cref="System.Drawing.Icon" /> format.
        /// </summary>
        public Icon LargeIcon => Icon.FromHandle(LargeBitmap.GetHicon());

        /// <summary>
        ///     Gets the thumbnail or icon in extra large size and <see cref="System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap ExtraLargeBitmap => GetBitmap(DefaultIconSize.ExtraLarge, DefaultThumbnailSize.ExtraLarge);

        /// <summary>
        ///     Gets the thumbnail or icon in Extra Large size and <see cref="System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource ExtraLargeBitmapSource =>
            GetBitmapSource(DefaultIconSize.ExtraLarge, DefaultThumbnailSize.ExtraLarge);

        /// <summary>
        ///     Gets the thumbnail or icon in Extra Large size and <see cref="System.Drawing.Icon" /> format.
        /// </summary>
        public Icon ExtraLargeIcon => Icon.FromHandle(ExtraLargeBitmap.GetHicon());

        /// <summary>
        ///     Gets or sets a value that determines if the current retrieval option is cache or extract, cache only, or from
        ///     memory only.
        ///     The default is cache or extract.
        /// </summary>
        public ShellThumbnailRetrievalOption RetrievalOption { get; set; }

        private ShellThumbnailFormatOption formatOption = ShellThumbnailFormatOption.Default;

        /// <summary>
        ///     Gets or sets a value that determines if the current format option is thumbnail or icon, thumbnail only, or icon
        ///     only.
        ///     The default is thumbnail or icon.
        /// </summary>
        public ShellThumbnailFormatOption FormatOption
        {
            get => formatOption;
            set
            {
                formatOption = value;

                // Do a similar check as we did in CurrentSize property setter,
                // If our mode is IconOnly, then our max is defined by DefaultIconSize.Maximum. We should make sure 
                // our CurrentSize is within this max range
                if (FormatOption == ShellThumbnailFormatOption.IconOnly
                    && (CurrentSize.Height > DefaultIconSize.Maximum.Height ||
                        CurrentSize.Width > DefaultIconSize.Maximum.Width))
                    CurrentSize = DefaultIconSize.Maximum;
            }
        }

        /// <summary>
        ///     Gets or sets a value that determines if the user can manually stretch the returned image.
        ///     The default value is false.
        /// </summary>
        /// <remarks>
        ///     For example, if the caller passes in 80x80 a 96x96 thumbnail could be returned.
        ///     This could be used as a performance optimization if the caller will need to stretch
        ///     the image themselves anyway. Note that the Shell implementation performs a GDI stretch blit.
        ///     If the caller wants a higher quality image stretch, they should pass this flag and do it themselves.
        /// </remarks>
        public bool AllowBiggerSize { get; set; }

        #endregion

        #region Private Methods

        private ShellNativeMethods.SIIGBF CalculateFlags()
        {
            ShellNativeMethods.SIIGBF flags = 0x0000;

            if (AllowBiggerSize) flags |= ShellNativeMethods.SIIGBF.BiggerSizeOk;

            if (RetrievalOption == ShellThumbnailRetrievalOption.CacheOnly)
                flags |= ShellNativeMethods.SIIGBF.InCacheOnly;
            else if (RetrievalOption == ShellThumbnailRetrievalOption.MemoryOnly)
                flags |= ShellNativeMethods.SIIGBF.MemoryOnly;

            if (FormatOption == ShellThumbnailFormatOption.IconOnly)
                flags |= ShellNativeMethods.SIIGBF.IconOnly;
            else if (FormatOption == ShellThumbnailFormatOption.ThumbnailOnly)
                flags |= ShellNativeMethods.SIIGBF.ThumbnailOnly;

            return flags;
        }

        private IntPtr GetHBitmap(Size size)
        {
            var hbitmap = IntPtr.Zero;

            // Create a size structure to pass to the native method
            var nativeSIZE = new CoreNativeMethods.Size();
            nativeSIZE.Width = Convert.ToInt32(size.Width);
            nativeSIZE.Height = Convert.ToInt32(size.Height);

            // Use IShellItemImageFactory to get an icon
            // Options passed in: Resize to fit
            var hr = ((IShellItemImageFactory) shellItemNative).GetImage(nativeSIZE, CalculateFlags(), out hbitmap);

            if (hr == HResult.Ok)
                return hbitmap;
            if ((uint) hr == 0x8004B200 && FormatOption == ShellThumbnailFormatOption.ThumbnailOnly)
                // Thumbnail was requested, but this ShellItem doesn't have a thumbnail.
                throw new InvalidOperationException("ShellThumbnailDoesNotHaveThumbnail",
                    Marshal.GetExceptionForHR((int) hr));
            if ((uint) hr == 0x80040154) // REGDB_E_CLASSNOTREG
                throw new NotSupportedException("ShellThumbnailNoHandler", Marshal.GetExceptionForHR((int) hr));

            throw new ShellException(hr);
        }

        private Bitmap GetBitmap(Size iconOnlySize, Size thumbnailSize)
        {
            return GetBitmap(FormatOption == ShellThumbnailFormatOption.IconOnly ? iconOnlySize : thumbnailSize);
        }

        private Bitmap GetBitmap(Size size)
        {
            var hBitmap = GetHBitmap(size);

            // return a System.Drawing.Bitmap from the hBitmap
            var returnValue = Image.FromHbitmap(hBitmap);

            // delete HBitmap to avoid memory leaks
            ShellNativeMethods.DeleteObject(hBitmap);

            return returnValue;
        }

        private BitmapSource GetBitmapSource(Size iconOnlySize, Size thumbnailSize)
        {
            return GetBitmapSource(FormatOption == ShellThumbnailFormatOption.IconOnly ? iconOnlySize : thumbnailSize);
        }

        private BitmapSource GetBitmapSource(Size size)
        {
            var hBitmap = GetHBitmap(size);

            // return a System.Media.Imaging.BitmapSource
            // Use interop to create a BitmapSource from hBitmap.
            var returnValue = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            // delete HBitmap to avoid memory leaks
            ShellNativeMethods.DeleteObject(hBitmap);

            return returnValue;
        }

        #endregion
    }
}