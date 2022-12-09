using System.Runtime.InteropServices;

namespace Libmdbx.Net.Core.Transaction
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MdbxTxnInfo
    {
        /** The ID of the transaction. For a READ-ONLY transaction, this corresponds
      to the snapshot being read. */
        [MarshalAs(UnmanagedType.U8)]
        public ulong txn_id;

        /** For READ-ONLY transaction: the lag from a recent MVCC-snapshot, i.e. the
           number of committed transaction since read transaction started.
           For WRITE transaction (provided if `scan_rlt=true`): the lag of the oldest
           reader from current transaction (i.e. at least 1 if any reader running). */
        [MarshalAs(UnmanagedType.U8)]
        public ulong txn_reader_lag;

        /** Used space by this transaction, i.e. corresponding to the Last used
         * database page. */
        [MarshalAs(UnmanagedType.U8)]
        public ulong txn_space_used;

        /** Current Size of database file. */
        [MarshalAs(UnmanagedType.U8)]
        public ulong txn_space_limit_soft;

        /** Upper bound for Size the database file, i.e. the value `sizeUpper`
           argument of the appropriate call of \ref mdbx_env_set_geometry(). */
        [MarshalAs(UnmanagedType.U8)]
        public ulong txn_space_limit_hard;

        /** For READ-ONLY transaction: The total Size of the database pages that were
           retired by committed write transactions after the reader's MVCC-snapshot,
           i.e. the space which would be freed after the Reader releases the
           MVCC-snapshot for reuse by completion read transaction.
           For WRITE transaction: The summarized Size of the database pages that were
           retired for now due Copy-On-Write during this transaction. */
        [MarshalAs(UnmanagedType.U8)]
        public ulong txn_space_retired;

        /** For READ-ONLY transaction: the space available for writer(s) and that
           must be exhausted for reason to call the Handle-Slow-Readers callback for
           this read transaction.
           For WRITE transaction: the space inside transaction
           that left to `MDBX_TXN_FULL` error. */
        [MarshalAs(UnmanagedType.U8)]
        public ulong txn_space_leftover;

        /** For READ-ONLY transaction (provided if `scan_rlt=true`): The space that
           actually become available for reuse when only this transaction will be
           finished.
           For WRITE transaction: The summarized Size of the dirty database
           pages that generated during this transaction. */
        [MarshalAs(UnmanagedType.U8)]
        public ulong txn_space_dirty;
    }
}
