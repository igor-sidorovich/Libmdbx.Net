using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using static Libmdbx.Net.Bindings.Const;

namespace Libmdbx.Net.Bindings
{
    internal static class MdbxError
    {
        /// <summary>
        /// char* mdbx_strerror_r(int errnum, char* buf, size_t buflen)
        /// Return a string describing a given error code.
        /// This function is a superset of the ANSI C X3.159-1989 (ANSI C) strerror() function.
        /// If the error code is greater than or equal to 0, then the string returned by the system function strerror() is returned.
        /// If the error code is less than 0, an error string corresponding to the MDBX library error is returned.See errors for a list of MDBX-specific error codes.
        /// mdbx_strerror_r() is thread-safe since uses user-supplied buffer where appropriate.The returned string must NOT be modified by the application, since it may be pointer to internal constant string.
        /// However, there is no restriction if the returned string points to the supplied buffer.
        /// </summary>
        /// <param name="errnum">The error code.</param>
        /// <param name="buf">Buffer to store the error message.</param>
        /// <param name="buflen">The Size of buffer to store the message.</param>
        /// <returns></returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_strerror_r")]
        internal static extern IntPtr mdbx_strerror_r([In, MarshalAs(UnmanagedType.I4)] int errnum,
                                                       [In, Out, MarshalAs(UnmanagedType.LPStr)] ref StringBuilder buf,
                                                       IntPtr buflen);

        /// <summary>
        /// char* mdbx_strerror(int errnum)	
        /// Return a string describing a given error code.
        /// This function is a superset of the ANSI C X3.159-1989 (ANSI C) strerror() function.
        /// If the error code is greater than or equal to 0, then the string returned by the system function strerror() is returned.
        /// If the error code is less than 0, an error string corresponding to the MDBX library error is returned.See errors for a list of MDBX-specific error codes.
        /// mdbx_strerror() is NOT thread-safe because may share common internal buffer for system messages.
        /// The returned string must NOT be modified by the application, but MAY be modified by a subsequent call to mdbx_strerror(), strerror() and other related functions.
        /// </summary>
        /// <param name="errnum"></param>
        /// <returns></returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_strerror")]
        internal static extern IntPtr mdbx_strerror([In, MarshalAs(UnmanagedType.I4)] int errnum);
    }
}
