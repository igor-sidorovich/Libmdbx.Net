using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Libmdbx.Net.Bindings.MdbxError;

namespace Libmdbx.Net
{
    internal static class LibmdbxResultCode
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string? ResultCodeToString(LibmdbxResultCodeFlag err)
        {
            var ptr = mdbx_strerror((int)err);
            return Marshal.PtrToStringAnsi(ptr);
        }
    }
}
