namespace Libmdbx.Net.Core.Env
{
    public struct OperateOptions
    {
        /// <summary>
        /// MDBX_NOTLS
        /// </summary>
        public bool orphanReadTransactions;

        public bool nestedWriteTransactions;

        /// <summary>
        /// MDBX_EXCLUSIVE
        /// </summary>
        public bool exclusive;

        /// <summary>
        /// MDBX_NORDAHEAD
        /// </summary>
        public bool disableReadahead;

        /// <summary>
        /// MDBX_NOMEMINIT
        /// </summary>
        public bool disableClearMemory;

        public OperateOptions(bool orphanReadTransactions = false,
            bool nestedWriteTransactions = false,
            bool exclusive = false,
            bool disableReadahead = false,
            bool disableClearMemory = false)
        {
            this.orphanReadTransactions = orphanReadTransactions;
            this.nestedWriteTransactions = nestedWriteTransactions;
            this.exclusive = exclusive;
            this.disableReadahead = disableReadahead;
            this.disableClearMemory = disableClearMemory;
        }

        public OperateOptions(EnvFlags flagsT)
        {
            orphanReadTransactions = (flagsT & (EnvFlags.NoTls | EnvFlags.Exclusive)) == EnvFlags.NoTls;
            nestedWriteTransactions = (flagsT & (EnvFlags.WriteMap | EnvFlags.ReadOnly)) != 0;
            exclusive = (flagsT & EnvFlags.Exclusive) != 0;
            disableReadahead = (flagsT & EnvFlags.NordAhead) != 0;
            disableClearMemory = (flagsT & EnvFlags.NoMemInit) != 0;
        }
    }
}