using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static Libmdbx.Net.Bindings.MdbxError;

namespace Libmdbx.Net
{
    internal static class LibmdbxResultCode
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ResultCodeToString(LibmdbxResultCodeFlag err)
        {
            int capacity = 400;
            StringBuilder buffer = new StringBuilder(capacity);
            IntPtr intPtr = IntPtr.Add(IntPtr.Zero, capacity);
            var ptr = mdbx_strerror_r((int)err, ref buffer, intPtr);
            return Marshal.PtrToStringAnsi(ptr);
        }
    }
}
