using System;

namespace Libmdbx.Net
{
    [Flags]
    public enum DbiState
    {
        /** DB was written in this Txn */
        Dirty = 0x01,

        /** Named-DB record is older than txnID */
        Stale = 0x02,

        /** Named-DB handle opened in this Txn */
        Fresh = 0x04,

        /** Named-DB handle created in this Txn */
        MDBX_DBI_CREAT = 0x08,
    };
}