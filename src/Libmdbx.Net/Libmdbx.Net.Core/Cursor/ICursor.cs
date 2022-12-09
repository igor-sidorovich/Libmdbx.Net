using System;
using Libmdbx.Net.Core.Transaction;

namespace Libmdbx.Net.Core.Cursor
{
    public interface ICursor : IDisposable
    {
        bool IsCompleted { get; }
        void Close();
        bool ToFirst<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool ToPrevious<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool Current<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool ToNext<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool ToLast<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool Find<TK, TV>(TK key, out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool LowerBound<TK, TV>(TK key, out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool ToCurrentFirstMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool ToCurrentNextMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool ToCurrentLastMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool ToNextFirstMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool FindMultivalue<TK, TV>(TK key, TV value, out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool LowerBoundMultivalue<TK, TV>(TK key, TV value, out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool ToCurrentPrevMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false);
        bool Seek<TK>(TK key);
        bool Eof();
        bool OnFirst();
        bool OnLast();
        void Renew(ITxn txn);
        void Insert<TK, TV>(TK key, TV value);
        bool TryInsert<TK, TV>(TK key, TV value);
        void Upsert<TK, TV>(TK key, TV value);
        void Update<TK, TV>(TK key, TV value);
        bool TryUpdate<TK, TV>(TK key, TV value);
        bool Erase<TK>(TK key, bool wholeMultivalue = true);
        bool Erase(bool wholeMultivalue = false);
        bool EraseMulti<TK, TV>(TK key, TV value);
    }
}
