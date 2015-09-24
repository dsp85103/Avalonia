﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Perspex.Controls;
using Perspex.Controls.Platform;
using Perspex.Platform;
using Perspex.Win32.Interop;

namespace Perspex.Win32
{
    class CommonDialogImpl : ICommonDialogImpl
    {
        public unsafe Task<string[]> ShowAsync(CommonDialog dialog, IWindowImpl parent)
        {
            var hWnd = parent?.Handle?.Handle ?? IntPtr.Zero;
            return Task.Factory.StartNew(() =>
            {
                var filters = new StringBuilder();
                foreach (var filter in dialog.Filters)
                {
                    var extMask = string.Join(";", filter.Extensions.Select(e => "*." + e));
                    filters.Append(filter.Name);
                    filters.Append(" (");
                    filters.Append(extMask);
                    filters.Append(")");
                    filters.Append('\0');
                    filters.Append(extMask);
                    filters.Append('\0');
                }
                if (filters.Length == 0)
                    filters.Append("All files\0*.*\0");
                filters.Append('\0');

                var filterBuffer = new char[filters.Length];
                filters.CopyTo(0, filterBuffer, 0, filterBuffer.Length);


                var buffer = new char[256];
                fixed (char* pBuffer = buffer)
                fixed (char* pFilterBuffer = filterBuffer)
                fixed (char* pDefExt = dialog.DefaultExtension)
                fixed (char* pInitDir = dialog.InitialDirectory)
                fixed (char* pTitle = dialog.Title)
                {

                    var ofn = new UnmanagedMethods.OpenFileName()
                    {
                        hwndOwner = hWnd,
                        hInstance = IntPtr.Zero,
                        lCustData = IntPtr.Zero,
                        nFilterIndex = 0,
                        Flags =
                            UnmanagedMethods.OpenFileNameFlags.OFN_EXPLORER |
                            UnmanagedMethods.OpenFileNameFlags.OFN_HIDEREADONLY,
                        nMaxCustFilter = 0,
                        nMaxFile = buffer.Length - 1,
                        nMaxFileTitle = 0,
                        lpTemplateName = IntPtr.Zero,
                        lpfnHook = IntPtr.Zero,
                        lpstrCustomFilter = IntPtr.Zero,
                        lpstrDefExt = new IntPtr(pDefExt),
                        lpstrFile = new IntPtr(pBuffer),
                        lpstrFileTitle = IntPtr.Zero,
                        lpstrFilter = new IntPtr(pFilterBuffer),
                        lpstrInitialDir = new IntPtr(pInitDir),
                        lpstrTitle = new IntPtr(pTitle),

                    };
                    ofn.lStructSize = Marshal.SizeOf(ofn);
                    if (dialog.AllowMultiple && dialog.Action == CommonDialogAction.OpenFile)
                        ofn.Flags |= UnmanagedMethods.OpenFileNameFlags.OFN_ALLOWMULTISELECT;

                    if (dialog.Action == CommonDialogAction.SaveFile)
                        ofn.Flags |= UnmanagedMethods.OpenFileNameFlags.OFN_NOREADONLYRETURN |
                                     UnmanagedMethods.OpenFileNameFlags.OFN_OVERWRITEPROMPT;

                    var pofn = &ofn;

                    var res = dialog.Action == CommonDialogAction.OpenFile
                        ? UnmanagedMethods.GetOpenFileName(new IntPtr(pofn))
                        : UnmanagedMethods.GetSaveFileName(new IntPtr(pofn));
                    if (!res)
                        return null;

                }
                var cStart = 0;
                string dir = null;
                var files = new List<string>();
                for (var c = 0; c < buffer.Length; c++)
                {
                    if (buffer[c] == 0)
                    {
                        //Encountered double zero char
                        if (cStart == c)
                            break;

                        var s = new string(buffer, cStart, c - cStart);
                        if (dir == null)
                            dir = s;
                        else
                            files.Add(s);
                        cStart = c + 1;
                    }
                }
                if (files.Count == 0)
                    return new[] {dir};

                return files.Select(f => Path.Combine(dir, f)).ToArray();
            });
        }
    }
}
