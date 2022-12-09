﻿using System;
using static Libmdbx.Net.Bindings.Const;
using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net
{
    [Flags]
    public enum LibmdbxResultCodeFlag
    {
        SUCCESS = MDBX_SUCCESS,
        RESULT_FALSE = MDBX_RESULT_FALSE,
        RESULT_TRUE = MDBX_RESULT_TRUE,
        KEY_EXIST = MDBX_KEYEXIST,
        FIRST_LMDB_ERRCODE = MDBX_FIRST_LMDB_ERRCODE,
        NOTFOUND = MDBX_NOTFOUND,
        PAGE_NOTFOUND = MDBX_PAGE_NOTFOUND,
        CORRUPTED = MDBX_CORRUPTED,
        PANIC = MDBX_PANIC,
        VERSION_MISMATCH = MDBX_VERSION_MISMATCH,
        INVALID = MDBX_INVALID,
        MAP_FULL = MDBX_MAP_FULL,
        DBS_FULL = MDBX_DBS_FULL,
        READERS_FULL = MDBX_READERS_FULL,
        TXN_FULL = MDBX_TXN_FULL,
        CURSOR_FULL = MDBX_CURSOR_FULL,
        PAGE_FULL = MDBX_PAGE_FULL,
        UNABLE_EXTEND_MAPSIZE = MDBX_UNABLE_EXTEND_MAPSIZE,
        INCOMPATIBLE = MDBX_INCOMPATIBLE,
        BAD_RSLOT = MDBX_BAD_RSLOT,
        BAD_TXN = MDBX_BAD_TXN,
        BAD_VALSIZE = MDBX_BAD_VALSIZE,
        BAD_DBI = MDBX_BAD_DBI,
        PROBLEM = MDBX_PROBLEM,
        LAST_LMDB_ERRCODE =  MDBX_LAST_LMDB_ERRCODE,
        BUSY = MDBX_BUSY,
        FIRST_ADDED_ERRCODE = MDBX_FIRST_ADDED_ERRCODE,
        EMULTIVAL = MDBX_EMULTIVAL,
        EBADSIGN = MDBX_EBADSIGN,
        WANNA_RECOVERY = MDBX_WANNA_RECOVERY,
        EKEYMISMATCH = MDBX_EKEYMISMATCH,
        TOO_LARGE = MDBX_TOO_LARGE,
        THREAD_MISMATCH = MDBX_THREAD_MISMATCH,
        TXN_OVERLAPPING = MDBX_TXN_OVERLAPPING,
        LAST_ADDED_ERRCODE = MDBX_LAST_ADDED_ERRCODE,
        ENODATA = MDBX_ENODATA,
        EINVAL = MDBX_EINVAL,
        EACCESS = MDBX_EACCESS,
        ENOMEM = MDBX_ENOMEM,
        EROFS = MDBX_EROFS,
        ENOSYS = MDBX_ENOSYS,
        EIO = MDBX_EIO,
        EPERM = MDBX_EPERM,
        EINTR = MDBX_EINTR,
        ENOFILE = MDBX_ENOFILE,
        EREMOTE = MDBX_EREMOTE
}
}
