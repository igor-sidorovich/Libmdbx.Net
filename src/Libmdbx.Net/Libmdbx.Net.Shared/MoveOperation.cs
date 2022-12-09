using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net
{
    public enum MoveOperation : uint
    {
        First = MDBX_FIRST,
        Last = MDBX_LAST,
        Next = MDBX_NEXT,
        Previous = MDBX_PREV,
        GetCurrent = MDBX_GET_CURRENT,

        MultiPrevKeyLastValue = MDBX_PREV_NODUP,

        MultiCurrentKeyFirstValue = MDBX_FIRST_DUP,
        MultiCurrentKeyPrevValue = MDBX_PREV_DUP,
        MultiCurrentKeyNextValue = MDBX_NEXT_DUP,
        MultiCurrentKeyLastValue = MDBX_LAST_DUP,

        MultiNextKeyFirstValue = MDBX_NEXT_NODUP,

        MultiFindPair = MDBX_GET_BOTH,
        MultiExactkeyLowerBoundValue = MDBX_GET_BOTH_RANGE,

        FindKey = MDBX_SET,
        KeyExact = MDBX_SET_KEY,
        KeyLowerBound = MDBX_SET_RANGE
    };
}
