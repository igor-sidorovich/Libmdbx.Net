using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net.Core.Transaction
{
    public enum ValueMode : uint
    {
        /// <summary>
        /// Usual Single value for each key. In terms of
        /// keys, they are unique.
        /// </summary>
        Single = MDBX_DB_DEFAULTS,

        /// <summary>
        /// A more than one data value could be associated with
        /// each key. Internally each key is stored once, and the
        /// corresponding data values are sorted by byte-by-byte
        /// lexicographic comparison like `std::memcmp()`.
        /// In terms of keys, they are not unique, i.e. has
        /// duplicates which are sorted by associated data values.
        /// </summary>
        Multi = MDBX_DUPSORT,

        /// <summary>
        /// A more than one data value could be associated with
        /// each key. Internally each key is stored once, and
        /// the corresponding data values are sorted by
        /// byte-by-byte lexicographic comparison in Reverse
        /// order, from the end of the keys to the beginning.
        /// In terms of keys, they are not unique, i.e. has
        /// duplicates which are sorted by associated data
        /// values.
        /// </summary>
        MultiReverse = MDBX_DUPSORT | MDBX_REVERSEDUP,

        /// <summary>
        /// A more than one data value could be associated with
        /// each key, and all data values must be same length.
        /// Internally each key is stored once, and the
        /// corresponding data values are sorted by byte-by-byte
        /// lexicographic comparison like `std::memcmp()`. In
        /// terms of keys, they are not unique, i.e. has
        /// duplicates which are sorted by associated data values.
        /// </summary>
        MultiSameLength = MDBX_DUPSORT | MDBX_DUPFIXED,

        /// <summary>
        /// A more than one data value could be associated with
        /// each key, and all data values are binary integers in
        /// native byte order, either `uint32_t` or `uint64_t`,
        /// and will be sorted as such. Internally each key is
        /// stored once, and the corresponding data values are
        /// sorted. In terms of keys, they are not unique, i.e.
        /// has duplicates which are sorted by associated data
        /// values.
        /// </summary>
        MultiOrdinal = MDBX_DUPSORT | MDBX_DUPFIXED | MDBX_INTEGERDUP,

        /// <summary>
        /// A more than one data value could be associated with
        /// each key, and all data values must be same length.
        /// Internally each key is stored once, and the
        /// corresponding data values are sorted by byte-by-byte
        /// lexicographic comparison in Reverse order, from the
        /// end of the keys to the beginning. In terms of keys,
        /// they are not unique, i.e. has duplicates which are
        /// sorted by associated data values.
        /// </summary>
        MultiReverseSameLength = MDBX_DUPSORT | MDBX_REVERSEDUP | MDBX_DUPFIXED,

        //msgpack = -1 ///< A more than one data value could be associated with each
        //             ///< key. Values are in [MessagePack](https://msgpack.org/)
        //             ///< format with appropriate comparison. Internally each key is
        //             ///< stored once, and the corresponding data values are sorted.
        //             ///< In terms of keys, they are not unique, i.e. has duplicates
        //             ///< which are sorted by associated data values.
        //             ///< \note Not yet implemented and PRs are welcome.
    };
}