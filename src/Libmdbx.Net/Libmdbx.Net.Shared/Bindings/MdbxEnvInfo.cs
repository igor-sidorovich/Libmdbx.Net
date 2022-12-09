using System.Runtime.InteropServices;

namespace Libmdbx.Net.Bindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MdbxEnvInfo
    {
        public EnvInfoGeo mi_geo;

        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_mapsize;             /**< Size of the data memory map */

        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_last_pgno;           /**< Number of the Last used page */

        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_recent_txnid;        /**< ID of the Last committed transaction */

        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_latter_reader_txnid; /**< ID of the Last reader transaction */

        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_self_latter_reader_txnid; /**< ID of the Last reader transaction of caller process */

        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_meta0_txnid, mi_meta0_sign;

        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_meta1_txnid, mi_meta1_sign;

        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_meta2_txnid, mi_meta2_sign;

        [MarshalAs(UnmanagedType.U4)]
        public uint mi_maxreaders;   /**< Total reader slots in the environment */

        [MarshalAs(UnmanagedType.U4)]
        public uint mi_numreaders;   /**< Max reader slots used in the environment */

        [MarshalAs(UnmanagedType.U4)]
        public uint mi_dxb_pagesize; /**< MdbxDb pageSize */

        [MarshalAs(UnmanagedType.U4)]
        public uint mi_sys_pagesize; /**< System pageSize */

        /** brief A mostly unique ID that is regenerated on each boot.
        As such it can be used to identify the local machine's current boot. MDBX
        uses such when open the database to determine whether rollback required to
        the Last steady sync point or not. I.e. if current bootid is differ from the
        value within a database then the system was rebooted and all changes since
        Last steady sync must be reverted for data integrity. Zeros mean that no
        relevant information is available from the system. */
        private EnvBoots mi_bootid;

        /** Bytes not explicitly synchronized to disk */
        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_unsync_volume;

        /** Current auto-sync threshold, see \ref mdbx_env_set_syncbytes(). */
        [MarshalAs(UnmanagedType.U8)]
        public ulong mi_autosync_threshold;

        /** Time since the Last steady sync in 1/65536 of second */
        [MarshalAs(UnmanagedType.U4)]
        public uint mi_since_sync_seconds16dot16;

        /** Current auto-sync period in 1/65536 of second,
         * see \ref mdbx_env_set_syncperiod(). */
        [MarshalAs(UnmanagedType.U4)]
        public uint mi_autosync_period_seconds16dot16;

        /** Time since the Last readers check in 1/65536 of second,
         * see \ref mdbx_reader_check(). */
        [MarshalAs(UnmanagedType.U4)]
        public uint mi_since_reader_check_seconds16dot16;

        /** Current environment Mode.
         * The same as \ref mdbx_env_get_flags() returns. */
        [MarshalAs(UnmanagedType.U4)]
        public uint mi_mode;

        /** Statistics of page operations.
        * \details Overall statistics of page operations of all (running, completed
        * and aborted) transactions in the current Multi-process session (since the
        * First process opened the database after everyone had previously closed it).
        */
        public EnvPageOperations mi_pgop_stat;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EnvInfoGeo
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong lower;

        [MarshalAs(UnmanagedType.U8)]
        public ulong upper;

        [MarshalAs(UnmanagedType.U8)]
        public ulong current;

        [MarshalAs(UnmanagedType.U8)]
        public ulong shrink;

        [MarshalAs(UnmanagedType.U8)]
        public ulong grow;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EnvPageOperations
    {
        [MarshalAs(UnmanagedType.U8)] 
        public ulong newly;

        [MarshalAs(UnmanagedType.U8)] 
        public ulong cow;

        [MarshalAs(UnmanagedType.U8)] 
        public ulong clone;

        [MarshalAs(UnmanagedType.U8)] 
        public ulong split;

        [MarshalAs(UnmanagedType.U8)] 
        public ulong merge;

        [MarshalAs(UnmanagedType.U8)] 
        public ulong spill;

        [MarshalAs(UnmanagedType.U8)] 
        public ulong unspill;

        [MarshalAs(UnmanagedType.U8)] 
        public ulong wops;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EnvBootItem
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong x;

        [MarshalAs(UnmanagedType.U8)]
        public ulong y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EnvBoots
    {
        public EnvBootItem current;

        public EnvBootItem meta0;

        public EnvBootItem meta1;

        public EnvBootItem meta2;
    }
}