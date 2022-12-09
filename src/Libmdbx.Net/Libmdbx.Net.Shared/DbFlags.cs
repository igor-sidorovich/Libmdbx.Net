using System;
using Libmdbx.Net.Bindings;
using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net
{
    [Flags]
    public enum DbFlags : uint
    {
        Defaults = MDBX_DB_DEFAULTS,

        /// <summary>
        /// Use Reverse string comparison for keys
        /// </summary>
        ReverseKey = MDBX_REVERSEKEY,

        /// <summary>
        /// Use sorted duplicates, i.e. allow Multi-values for a keys
        /// </summary>
        Dupsort = MDBX_DUPSORT,

        /// <summary>
        /// Numeric keys in native byte order either uint32_t or uint64_t
        /// (must be one of uint32_t or uint64_t, other integer types, for example,
        /// signed integer or uint16_t will not work).
        /// The keys must all be of the same Size and must be aligned while passing as
        /// arguments
        /// </summary>
        IntegerKey = MDBX_INTEGERKEY,

        /// <summary>
        /// With MDBX_DUPSORT; sorted dup items have fixed Size. The data values
        /// must all be of the same Size
        /// </summary>
        Dupfixed = MDBX_DUPFIXED,

        /// <summary>
        /// With MDBX_DUPSORT and with MDBX_DUPFIXED; dups are fixed Size
        /// like MDBX_INTEGERKEY -style integers. The data values must all be of
        /// the same Size and must be aligned while passing as arguments
        /// </summary>
        IntegerDup = MDBX_INTEGERDUP,

        /// <summary>
        /// With MDBX_DUPSORT; use Reverse string comparison for data values
        /// </summary>
        ReverseDup = MDBX_REVERSEDUP,

        /// <summary>
        /// Create DB if not already existing
        /// </summary>
        Сreate = MDBX_CREATE,

        /// <summary>
        /// Opens an existing sub-database created with unknown Flags.
        /// The `MDBX_DB_ACCEDE` flag is intend to open a existing sub-database which
        /// was created with unknown Flags (MDBX_REVERSEKEY, MDBX_DUPSORT,
        /// MDBX_INTEGERKEY, MDBX_DUPFIXED, MDBX_INTEGERDUP and
        /// MDBX_REVERSEDUP).
        /// 
        /// In such cases, instead of returning the MDBX_INCOMPATIBLE error, the
        /// sub-database will be opened with Flags which it was created, and then an
        /// application could determine the actual Flags by mdbx_dbi_flags()
        /// </summary>
        Accede = MDBX_ACCEDE
    };
}