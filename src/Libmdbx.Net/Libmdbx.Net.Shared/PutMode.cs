using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net
{
    public enum PutMode : uint
    {
        /// <summary>
        /// Insert only unique keys
        /// </summary>
        InsertUnique = MDBX_NOOVERWRITE,

        /// <summary>
        /// Insert or Update
        /// </summary>
        Upsert = MDBX_UPSERT,

        /// <summary>
        /// Update existing, don't Insert new
        /// </summary>
        Update = MDBX_CURRENT
    };
}