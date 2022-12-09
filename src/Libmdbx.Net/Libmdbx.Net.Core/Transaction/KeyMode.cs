using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net.Core.Transaction
{
    public enum KeyMode : uint
    {
        /// <summary>
        /// Usual variable length keys with byte-by-byte
        /// lexicographic comparison like `std::memcmp()`.
        /// </summary>
        Usual = MDBX_DB_DEFAULTS,

        /// <summary>
        /// Variable length keys with byte-by-byte
        /// lexicographic comparison in Reverse order,
        /// from the end of the keys to the beginning.
        /// </summary>
        Reverse = MDBX_REVERSEKEY,

        /// <summary>
        /// Keys are binary integers in native byte order,
        /// either `uint32_t` or `uint64_t`, and will be
        /// sorted as such. The keys must all be of the
        /// same Size and must be aligned while passing
        /// as arguments.
        /// </summary>
        Ordinal = MDBX_INTEGERKEY

        /// <summary>
        /// Keys are in [MessagePack](https://msgpack.org/)
        /// format with appropriate comparison.
        /// note Not yet implemented and PRs are welcome.
        /// </summary>
        ///msgpack = -1
    };
}