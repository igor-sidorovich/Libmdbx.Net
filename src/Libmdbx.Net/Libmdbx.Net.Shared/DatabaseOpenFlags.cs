using System;
using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net
{
    [Flags]
    public enum DatabaseOpenFlags : uint
    {
        Defaults = MDBX_DB_DEFAULTS,

        ReverseKey = MDBX_REVERSEKEY,

        DupSort = MDBX_DUPSORT,

        IntegerKey = MDBX_INTEGERKEY,

        DupFixed = MDBX_DUPFIXED,

        IntegerDuplicates = MDBX_INTEGERDUP,

        ReverseDuplicates = MDBX_REVERSEDUP,

        Create = MDBX_CREATE,

        DbAccede = MDBX_DB_ACCEDE
    }
}
