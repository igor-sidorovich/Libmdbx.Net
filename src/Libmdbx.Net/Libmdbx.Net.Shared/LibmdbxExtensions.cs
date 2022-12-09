using System;
using System.Runtime.CompilerServices;

namespace Libmdbx.Net
{
    internal static class LibmdbxExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowException(this LibmdbxResultCodeFlag errorCode, string methodName)
        {
            if (errorCode.IsSuccess())
            {
                return;
            }
            switch (errorCode)
            {
                case LibmdbxResultCodeFlag.EINVAL:
                    throw new ArgumentException("mdbx");
                case LibmdbxResultCodeFlag.ENOMEM:
                    throw new OutOfMemoryException();
                case LibmdbxResultCodeFlag.SUCCESS:
                    throw new Exception("MDBX_SUCCESS (MDBX_RESULT_FALSE)");
                case LibmdbxResultCodeFlag.RESULT_TRUE:
                    throw new Exception("MDBX_RESULT_TRUE)");
                case LibmdbxResultCodeFlag.BAD_DBI:
                case LibmdbxResultCodeFlag.BAD_TXN:
                case LibmdbxResultCodeFlag.BAD_VALSIZE:
                case LibmdbxResultCodeFlag.CORRUPTED:
                case LibmdbxResultCodeFlag.CURSOR_FULL:
                case LibmdbxResultCodeFlag.PAGE_NOTFOUND:
                case LibmdbxResultCodeFlag.MAP_FULL:
                case LibmdbxResultCodeFlag.INVALID:
                case LibmdbxResultCodeFlag.TOO_LARGE:
                case LibmdbxResultCodeFlag.UNABLE_EXTEND_MAPSIZE:
                case LibmdbxResultCodeFlag.VERSION_MISMATCH:
                case LibmdbxResultCodeFlag.WANNA_RECOVERY:
                case LibmdbxResultCodeFlag.EBADSIGN:
                case LibmdbxResultCodeFlag.PANIC:
                case LibmdbxResultCodeFlag.INCOMPATIBLE:
                case LibmdbxResultCodeFlag.PAGE_FULL:
                case LibmdbxResultCodeFlag.PROBLEM:
                case LibmdbxResultCodeFlag.EKEYMISMATCH:
                case LibmdbxResultCodeFlag.DBS_FULL:
                case LibmdbxResultCodeFlag.READERS_FULL:
                case LibmdbxResultCodeFlag.EMULTIVAL:
                case LibmdbxResultCodeFlag.ENODATA:
                case LibmdbxResultCodeFlag.NOTFOUND:
                case LibmdbxResultCodeFlag.EPERM:
                case LibmdbxResultCodeFlag.EACCESS:
                case LibmdbxResultCodeFlag.BAD_RSLOT:
                case LibmdbxResultCodeFlag.EREMOTE:
                case LibmdbxResultCodeFlag.BUSY:
                case LibmdbxResultCodeFlag.THREAD_MISMATCH:
                case LibmdbxResultCodeFlag.TXN_FULL:
                case LibmdbxResultCodeFlag.TXN_OVERLAPPING:
                    throw new LibmdbxException(methodName, errorCode, "mdbx");
                default:
                    if (errorCode.IsMdbxError())
                    {
                        throw new LibmdbxException(methodName, errorCode);
                    }
                    throw new LibmdbxException(methodName, errorCode, "system error");
            }
        }

        /// <summary>
        /// TODO - C++
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BooleanOrThrow(this LibmdbxResultCodeFlag errorCode, string methodName)
        {
            if (errorCode.IsSuccess())
            {
                return true;
            }

            switch (errorCode)
            {
                case LibmdbxResultCodeFlag.RESULT_FALSE:
                    return false;
                case LibmdbxResultCodeFlag.RESULT_TRUE:
                    return true;
                default:
                    errorCode.ThrowException(methodName);
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuccess(this LibmdbxResultCodeFlag errorCode)
        {
            return LibmdbxResultCodeFlag.SUCCESS == errorCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMdbxError(this LibmdbxResultCodeFlag errorCode)
        {
            return (errorCode >= LibmdbxResultCodeFlag.FIRST_LMDB_ERRCODE &&
                    errorCode <= LibmdbxResultCodeFlag.LAST_LMDB_ERRCODE) ||
                   (errorCode >= LibmdbxResultCodeFlag.FIRST_ADDED_ERRCODE &&
                    errorCode <= LibmdbxResultCodeFlag.LAST_ADDED_ERRCODE);
        }
    }
}
