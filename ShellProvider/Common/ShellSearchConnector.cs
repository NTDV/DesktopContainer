//Copyright (c) Microsoft Corporation.  All rights reserved.

using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    ///     A Serch Connector folder in the Shell Namespace
    /// </summary>
    public sealed class ShellSearchConnector : ShellSearchCollection
    {
        #region Internal Constructor

        internal ShellSearchConnector()
        {
            CoreHelpers.ThrowIfNotWin7();
        }

        internal ShellSearchConnector(IShellItem2 shellItem)
            : this()
        {
            nativeShellItem = shellItem;
        }

        #endregion
    }
}