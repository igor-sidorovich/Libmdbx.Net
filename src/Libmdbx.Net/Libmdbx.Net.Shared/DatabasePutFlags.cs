using System;
using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net
{
    [Flags]
    public enum DatabasePutFlags : uint
    {
        Upsert = MDBX_UPSERT,

        NoOverwrite = MDBX_NOOVERWRITE,

        NoDupData = MDBX_NODUPDATA,

        Current = MDBX_CURRENT,

        AllDups = MDBX_ALLDUPS,

        Reserve = MDBX_RESERVE,

        Append = MDBX_APPEND,

        AppendDup = MDBX_APPENDDUP,

        Multiple = MDBX_MULTIPLE
    }
}
