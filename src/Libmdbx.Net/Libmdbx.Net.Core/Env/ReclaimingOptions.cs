namespace Libmdbx.Net.Core.Env
{
    public struct ReclaimingOptions
    {
        /// <summary>
        /// MDBX_LIFORECLAIM
        /// </summary>
        public bool lifo;

        /// <summary>
        /// MDBX_COALESCE
        /// </summary>
        public bool coalesce;

        public ReclaimingOptions(bool lifo = false,
                                 bool coalesce = false)
        {
            this.lifo = lifo;
            this.coalesce = coalesce;
        }

        public ReclaimingOptions(EnvFlags flagsT)
        {
            lifo = (flagsT & EnvFlags.LifoReclaim) != 0;
            coalesce = (flagsT & EnvFlags.Coalesce) != 0;
        }
    }
}