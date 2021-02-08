//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    ///     The base class for all Shell objects in Shell Namespace.
    /// </summary>
    public abstract class ShellObject : IDisposable, IEquatable<ShellObject>
    {
        #region Internal Fields

        /// <summary>
        ///     Internal member to keep track of the native IShellItem2
        /// </summary>
        internal IShellItem2 nativeShellItem;

        #endregion

        #region Public Static Methods

        /// <summary>
        ///     Creates a ShellObject subclass given a parsing name.
        ///     For file system items, this method will only accept absolute paths.
        /// </summary>
        /// <param name="parsingName">The parsing name of the object.</param>
        /// <returns>A newly constructed ShellObject object.</returns>
        public static ShellObject FromParsingName(string parsingName)
        {
            return ShellObjectFactory.Create(parsingName);
        }

        #endregion

        #region Constructors

        internal ShellObject()
        {
            
        }

        internal ShellObject(IShellItem2 shellItem)
        {
            nativeShellItem = shellItem;
        }

        #endregion

        #region Protected Fields

        /// <summary>
        ///     Parsing name for this Object e.g. c:\Windows\file.txt,
        ///     or ::{Some Guid}
        /// </summary>
        private string _internalParsingName;

        /// <summary>
        ///     A friendly name for this object that' suitable for display
        /// </summary>
        private string _internalName;

        /// <summary>
        ///     PID List (PIDL) for this object
        /// </summary>
        private IntPtr _internalPIDL = IntPtr.Zero;

        #endregion

        #region Internal Properties

        /// <summary>
        ///     Return the native ShellFolder object as newer IShellItem2
        /// </summary>
        /// <exception cref="System.Runtime.InteropServices.ExternalException">
        ///     If the native object cannot be created.
        ///     The ErrorCode member will contain the external error code.
        /// </exception>
        internal virtual IShellItem2 NativeShellItem2
        {
            get
            {
                if (nativeShellItem == null && ParsingName != null)
                {
                    var guid = new Guid(ShellIIDGuid.IShellItem2);
                    var retCode = ShellNativeMethods.SHCreateItemFromParsingName(ParsingName, IntPtr.Zero, ref guid,
                        out nativeShellItem);

                    if (nativeShellItem == null || !CoreErrorHelper.Succeeded(retCode))
                        throw new ShellException("ShellObjectCreationFailed", Marshal.GetExceptionForHR(retCode));
                }

                return nativeShellItem;
            }
        }

        /// <summary>
        ///     Return the native ShellFolder object
        /// </summary>
        internal virtual IShellItem NativeShellItem => NativeShellItem2;

        /// <summary>
        ///     Gets access to the native IPropertyStore (if one is already
        ///     created for this item and still valid. This is usually done by the
        ///     ShellPropertyWriter class. The reference will be set to null
        ///     when the writer has been closed/commited).
        /// </summary>
        internal IPropertyStore NativePropertyStore { get; set; }

        #endregion

        #region Public Properties

        private ShellProperties _properties;

        /// <summary>
        ///     Gets an object that allows the manipulation of ShellProperties for this shell item.
        /// </summary>
        public ShellProperties Properties => _properties ?? (_properties = new ShellProperties(this));

        /// <summary>
        ///     Gets the parsing name for this ShellItem.
        /// </summary>
        public virtual string ParsingName
        {
            get
            {
                if (_internalParsingName == null && nativeShellItem != null)
                    _internalParsingName = ShellHelper.GetParsingName(nativeShellItem);
                return _internalParsingName ?? string.Empty;
            }
            protected set => _internalParsingName = value;
        }

        /// <summary>
        ///     Gets the normal display for this ShellItem.
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (_internalName == null && NativeShellItem != null)
                {
                    var pszString = IntPtr.Zero;
                    var hr = NativeShellItem.GetDisplayName(ShellNativeMethods.ShellItemDesignNameOptions.Normal,
                        out pszString);
                    if (hr == HResult.Ok && pszString != IntPtr.Zero)
                    {
                        _internalName = Marshal.PtrToStringAuto(pszString);

                        // Free the string
                        Marshal.FreeCoTaskMem(pszString);
                    }
                }

                return _internalName;
            }

            protected set => _internalName = value;
        }

        /// <summary>
        ///     Gets the PID List (PIDL) for this ShellItem.
        /// </summary>
        internal virtual IntPtr PIDL
        {
            get
            {
                // Get teh PIDL for the ShellItem
                if (_internalPIDL == IntPtr.Zero && NativeShellItem != null)
                    _internalPIDL = ShellHelper.PidlFromShellItem(NativeShellItem);

                return _internalPIDL;
            }
            set => _internalPIDL = value;
        }

        /// <summary>
        ///     Overrides object.ToString()
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return Name;
        }

        private ShellThumbnail _thumbnail;

        /// <summary>
        ///     Gets the thumbnail of the ShellObject.
        /// </summary>
        public ShellThumbnail Thumbnail => _thumbnail ?? (_thumbnail = new ShellThumbnail(this));

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Release the native and managed objects
        /// </summary>
        /// <param name="disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _internalName = null;
                _internalParsingName = null;
                _properties = null;
                _thumbnail = null;
            }

            _properties?.Dispose();

            if (_internalPIDL != IntPtr.Zero)
            {
                ShellNativeMethods.ILFree(_internalPIDL);
                _internalPIDL = IntPtr.Zero;
            }

            if (nativeShellItem != null)
            {
                Marshal.ReleaseComObject(nativeShellItem);
                nativeShellItem = null;
            }

            if (NativePropertyStore != null)
            {
                Marshal.ReleaseComObject(NativePropertyStore);
                NativePropertyStore = null;
            }
        }

        /// <summary>
        ///     Release the native objects.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Implement the finalizer.
        /// </summary>
        ~ShellObject()
        {
            Dispose(false);
        }

        #endregion

        #region equality and hashing

        /// <summary>
        ///     Returns the hash code of the object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (!hashValue.HasValue)
            {
                var size = ShellNativeMethods.ILGetSize(PIDL);
                if (size != 0)
                {
                    var pidlData = new byte[size];
                    Marshal.Copy(PIDL, pidlData, 0, (int) size);
                    var hashData = hashProvider.ComputeHash(pidlData);
                    hashValue = BitConverter.ToInt32(hashData, 0);
                }
                else
                {
                    hashValue = 0;
                }
            }

            return hashValue.Value;
        }

        private static readonly MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
        private int? hashValue;

        /// <summary>
        ///     Determines if two ShellObjects are identical.
        /// </summary>
        /// <param name="other">The ShellObject to comare this one to.</param>
        /// <returns>True if the ShellObjects are equal, false otherwise.</returns>
        public bool Equals(ShellObject other)
        {
            var areEqual = false;

            if (other != null)
            {
                var ifirst = NativeShellItem;
                var isecond = other.NativeShellItem;
                if (ifirst != null && isecond != null)
                {
                    var result = 0;
                    var hr = ifirst.Compare(
                        isecond, SICHINTF.SICHINT_ALLFIELDS, out result);

                    areEqual = hr == HResult.Ok && result == 0;
                }
            }

            return areEqual;
        }

        /// <summary>
        ///     Returns whether this object is equal to another.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ShellObject);
        }

        /// <summary>
        ///     Implements the == (equality) operator.
        /// </summary>
        /// <param name="leftShellObject">First object to compare.</param>
        /// <param name="rightShellObject">Second object to compare.</param>
        /// <returns>True if leftShellObject equals rightShellObject; false otherwise.</returns>
        public static bool operator ==(ShellObject leftShellObject, ShellObject rightShellObject)
        {
            if ((object) leftShellObject == null) return (object) rightShellObject == null;
            return leftShellObject.Equals(rightShellObject);
        }

        /// <summary>
        ///     Implements the != (inequality) operator.
        /// </summary>
        /// <param name="leftShellObject">First object to compare.</param>
        /// <param name="rightShellObject">Second object to compare.</param>
        /// <returns>True if leftShellObject does not equal leftShellObject; false otherwise.</returns>
        public static bool operator !=(ShellObject leftShellObject, ShellObject rightShellObject)
        {
            return !(leftShellObject == rightShellObject);
        }

        #endregion
    }
}