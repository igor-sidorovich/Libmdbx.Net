using System;
using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net.Core.Transaction
{
    [Flags]
    public enum TxnFlags : uint
    {
        ReadWrite = MDBX_TXN_READWRITE,
        ReadOnly = MDBX_RDONLY,
        ReadOnlyPrepare = MDBX_TXN_RDONLY_PREPARE,
        Try = MDBX_TXN_TRY,
        NoMetaAsync = MDBX_TXN_NOMETASYNC,
        NoSync = MDBX_TXN_NOSYNC
    }
}
