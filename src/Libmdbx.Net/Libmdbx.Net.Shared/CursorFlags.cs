using System;
using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net
{
    [Flags]
    public enum CursorFlags : uint
    {
        First =  MDBX_FIRST,

        FirstDuplicated = MDBX_FIRST_DUP,

        GetBoth = MDBX_GET_BOTH,

        GetBothRange = MDBX_GET_BOTH_RANGE,

        GetCurrent = MDBX_GET_CURRENT,

        GetMultiple = MDBX_GET_MULTIPLE,

        Last = MDBX_LAST,

        LastDuplicated = MDBX_LAST_DUP,

        Next = MDBX_NEXT,

        NextDuplicated = MDBX_NEXT_DUP,

        NextMultiple = MDBX_NEXT_MULTIPLE,

        NextNoDup = MDBX_NEXT_NODUP,

        Prev = MDBX_PREV,

        PrevDuplicated = MDBX_PREV_DUP,

        PrevNoDuplicated = MDBX_PREV_NODUP,

        Set = MDBX_SET,

        SetKey = MDBX_SET_KEY,

        SetRange = MDBX_SET_RANGE,

        PrevMultiple = MDBX_PREV_MULTIPLE,

        SetLowerBound = MDBX_SET_LOWERBOUND
    }
}
