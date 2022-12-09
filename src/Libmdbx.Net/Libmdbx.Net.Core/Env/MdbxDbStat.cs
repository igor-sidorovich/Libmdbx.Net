using System.Runtime.InteropServices;

namespace Libmdbx.Net.Core.Env
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MdbxDbStat
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint ms_psize; /**< Size of a database page. This is the same for all databases. */

        [MarshalAs(UnmanagedType.U4)]
        public uint ms_depth; /**< Depth (height) of the B-tree */

        [MarshalAs(UnmanagedType.U8)]
        public ulong ms_branch_pages;   /**< Number of internal (non-leaf) pages */

        [MarshalAs(UnmanagedType.U8)]
        public ulong ms_leaf_pages;     /**< Number of leaf pages */

        [MarshalAs(UnmanagedType.U8)]
        public ulong ms_overflow_pages; /**< Number of overflow pages */

        [MarshalAs(UnmanagedType.U8)]
        public ulong ms_entries;        /**< Number of data items */

        [MarshalAs(UnmanagedType.U8)]
        public ulong ms_mod_txnid; /**< Tran ID of committed Last modification */
    }
}
