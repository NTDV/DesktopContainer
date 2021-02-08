//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    internal class ShellFolderItems : IEnumerator<ShellObject>
    {
        #region Internal Constructor

        internal ShellFolderItems(ShellContainer nativeShellFolder)
        {
            this.nativeShellFolder = nativeShellFolder;

            var hr = nativeShellFolder.NativeShellFolder.EnumObjects(
                IntPtr.Zero,
                ShellNativeMethods.ShellFolderEnumerationOptions.Folders |
                ShellNativeMethods.ShellFolderEnumerationOptions.NonFolders,
                out nativeEnumIdList);


            if (!CoreErrorHelper.Succeeded(hr))
            {
                if (hr == HResult.Canceled)
                    throw new FileNotFoundException();
                throw new ShellException(hr);
            }
        }

        #endregion

        #region IEnumerator<ShellObject> Members

        public ShellObject Current { get; private set; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (nativeEnumIdList != null)
            {
                Marshal.ReleaseComObject(nativeEnumIdList);
                nativeEnumIdList = null;
            }
        }

        #endregion

        #region Private Fields

        private IEnumIDList nativeEnumIdList;
        private readonly ShellContainer nativeShellFolder;

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current => Current;

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (nativeEnumIdList == null) return false;

            IntPtr item;
            uint numItemsReturned;
            uint itemsRequested = 1;
            var hr = nativeEnumIdList.Next(itemsRequested, out item, out numItemsReturned);

            if (numItemsReturned < itemsRequested || hr != HResult.Ok) return false;

            Current = ShellObjectFactory.Create(item, nativeShellFolder);

            return true;
        }

        /// <summary>
        /// </summary>
        public void Reset()
        {
            if (nativeEnumIdList != null) nativeEnumIdList.Reset();
        }

        #endregion
    }
}