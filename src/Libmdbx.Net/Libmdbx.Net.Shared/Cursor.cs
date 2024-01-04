using System;
using Libmdbx.Net.Bindings;
using System.Runtime.InteropServices;
using Libmdbx.Net.Core.Common.BufferConverts;
using static Libmdbx.Net.Bindings.MdbxCursor;
using Libmdbx.Net.Core.Cursor;
using Libmdbx.Net.Core.Transaction;

namespace Libmdbx.Net
{
    public class Cursor : ICursor
    {
        #region Fields

        private IntPtr _cursorPtr;

        #endregion

        #region Properties

        public bool IsCompleted => _cursorPtr == IntPtr.Zero;

        #endregion

        #region Constructor

        public Cursor() : this(mdbx_cursor_create(IntPtr.Zero))
        {
        }

        public Cursor(IntPtr cursorPtr)
        {
            _cursorPtr = cursorPtr;
        }

        #endregion

        #region Methods

        public void Close()
        {
            if (_cursorPtr == IntPtr.Zero)
            {
                throw new LibmdbxException(nameof(Close), LibmdbxResultCodeFlag.EINVAL);
            }

            mdbx_cursor_close(_cursorPtr);
            _cursorPtr = IntPtr.Zero;
        }

        public bool ToFirst<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.First, out cursorResult, throwNotFound);
        }

        public bool ToPrevious<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.Previous, out cursorResult, throwNotFound);
        }

        public bool Current<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.GetCurrent, out cursorResult, throwNotFound);
        }

        public bool ToNext<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.Next, out cursorResult, throwNotFound);
        }

        public bool ToLast<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.Last, out cursorResult, throwNotFound);
        }

        public bool Find<TK, TV>(TK key, out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.KeyExact, out cursorResult, key, throwNotFound);
        }

        public bool LowerBound<TK, TV>(TK key, out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move<TK, TV>(MoveOperation.KeyLowerBound, out cursorResult, key, throwNotFound);
        }

        public bool ToCurrentFirstMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.MultiCurrentKeyFirstValue, out cursorResult, throwNotFound);
        }

        public bool ToCurrentPrevMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.MultiCurrentKeyPrevValue, out cursorResult, throwNotFound);
        }

        public bool ToCurrentNextMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.MultiCurrentKeyNextValue, out cursorResult, throwNotFound);
        }

        public bool ToCurrentLastMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.MultiCurrentKeyLastValue, out cursorResult, throwNotFound);
        }

        public bool ToNextFirstMulti<TK, TV>(out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.MultiNextKeyFirstValue, out cursorResult, throwNotFound);
        }

        public bool FindMultivalue<TK, TV>(TK key, TV value, out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.MultiFindPair, out cursorResult, key, value, throwNotFound);
        }

        public bool LowerBoundMultivalue<TK, TV>(TK key, TV value, out CursorResult<TK, TV> cursorResult, bool throwNotFound = false)
        {
            return Move(MoveOperation.MultiExactkeyLowerBoundValue, out cursorResult, key, value, throwNotFound);
        }

        public bool Seek<TK>(TK key)
        {
            return Move(MoveOperation.FindKey, key, false);
        }

        public bool Eof()
        {
            return mdbx_cursor_eof(_cursorPtr).BooleanOrThrow(nameof(mdbx_cursor_eof));
        }

        public bool OnFirst() 
        {
            return mdbx_cursor_on_first(_cursorPtr).BooleanOrThrow(nameof(mdbx_cursor_on_first));
        }

        public bool OnLast()
        {
            return mdbx_cursor_on_last(_cursorPtr).BooleanOrThrow(nameof(mdbx_cursor_on_last));
        }

        public void Renew(ITxn txn)
        {
            mdbx_cursor_renew(txn.TxnPtr, _cursorPtr).ThrowException(nameof(mdbx_cursor_renew));
        }

        #region Protected

        protected bool Seek(ref MdbxDbVal key)
        {
            MdbxDbVal dbValue = new MdbxDbVal();
            return Move(MoveOperation.FindKey, ref key, ref dbValue, false);
        }

        protected LibmdbxResultCodeFlag Put(ref MdbxDbVal key, ref MdbxDbVal value, DatabasePutFlags flags)
        {
            return mdbx_cursor_put(_cursorPtr, ref key, ref value, flags);
        }

        protected unsafe LibmdbxResultCodeFlag Put<TK, TV>(TK key, TV value, DatabasePutFlags flags)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            fixed (byte* keyPtr = keyBuffer)
            fixed (byte* valuePtr = valueBuffer)
            {
                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer.Length);
                MdbxDbVal dbValue = new MdbxDbVal(valuePtr, valueBuffer.Length);

                return mdbx_cursor_put(_cursorPtr, ref dbKey, ref dbValue, flags);
            }
        }

        protected unsafe bool Move<TK>(MoveOperation op, TK key, bool throwNotFound)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);

            fixed (byte* keyPtr = keyBuffer)
            {
                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer.Length);
                MdbxDbVal dbValue = new MdbxDbVal();

                return Move(op, ref dbKey, ref dbValue, throwNotFound);
            }
        }

        protected bool Move<TK, TV>(MoveOperation op, out CursorResult<TK, TV> cursorResult, bool throwNotFound)
        {
            MdbxDbVal dbKey = new MdbxDbVal();
            MdbxDbVal dbValue = new MdbxDbVal();

            return Move(op, out cursorResult, dbKey, dbValue, throwNotFound);
        }

        protected unsafe bool Move<TK, TV>(MoveOperation op, out CursorResult<TK, TV> cursorResult, TK key, bool throwNotFound)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);

            fixed (byte* keyPtr = keyBuffer)
            {
                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer.Length);
                MdbxDbVal dbValue = new MdbxDbVal();

                return Move(op, out cursorResult, dbKey, dbValue, throwNotFound);
            }
        }

        protected unsafe bool Move<TK, TV>(MoveOperation op, out CursorResult<TK, TV> cursorResult, TK key, TV value, bool throwNotFound)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            fixed (byte* keyPtr = keyBuffer)
            fixed (byte* valuePtr = valueBuffer)
            {
                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer.Length);
                MdbxDbVal dbValue = new MdbxDbVal(valuePtr, valueBuffer.Length);

                return Move(op, out cursorResult, dbKey, dbValue, throwNotFound);
            }
        }

        protected bool Move<TK, TV>(MoveOperation op, out CursorResult<TK, TV> cursorResult, MdbxDbVal key, MdbxDbVal value, bool throwNotFound)
        {
            cursorResult = default;

            if (!Move(op, ref key, ref value, throwNotFound))
            {
                return false;
            }

            TK returnKey = BufferConverterFactory.Get<TK>().ConvertFromBuffer(key.AsSpan());
            TV returnValue = BufferConverterFactory.Get<TV>().ConvertFromBuffer(value.AsSpan());

            cursorResult = new CursorResult<TK, TV>(returnKey, returnValue);

            return true;
        }

        protected bool Move(MoveOperation op, ref MdbxDbVal key, ref MdbxDbVal value, bool throwNotFound)
        {
            var err = mdbx_cursor_get(_cursorPtr, ref key, ref value, (CursorFlags)op);

            switch (err)
            {
                case LibmdbxResultCodeFlag.SUCCESS:
                    return true;
                case LibmdbxResultCodeFlag.NOTFOUND:
                    if (!throwNotFound)
                        return false;
                    err.ThrowException(nameof(mdbx_cursor_get));
                    return false;
                default:
                    err.ThrowException(nameof(mdbx_cursor_get));
                    return false;
            }
        }

        #endregion

        #region Insert

        public void Insert<TK, TV>(TK key, TV value)
        {
            Put(key, value, (DatabasePutFlags)PutMode.InsertUnique).ThrowException(nameof(Insert));
        }

        public bool TryInsert<TK, TV>(TK key, TV value)
        {
            var err = Put(key, value, (DatabasePutFlags)PutMode.InsertUnique);

            switch (err)
            {
                case LibmdbxResultCodeFlag.SUCCESS:
                    return true;
                case LibmdbxResultCodeFlag.KEY_EXIST:
                    return false;
                default:
                    err.ThrowException(nameof(TryInsert));
                    return false;
            }
        }

        #endregion

        #region Upsert

        public void Upsert<TK, TV>(TK key, TV value)
        {
            Put(key, value, (DatabasePutFlags)PutMode.Upsert).ThrowException(nameof(Upsert));
        }

        #endregion

        #region Update

        public void Update<TK, TV>(TK key, TV value)
        {
            Put(key, value, (DatabasePutFlags)PutMode.Update).ThrowException(nameof(Update));
        }

        public bool TryUpdate<TK, TV>(TK key, TV value)
        {
            var err = Put(key, value, (DatabasePutFlags)PutMode.Update);

            switch (err)
            {
                case LibmdbxResultCodeFlag.SUCCESS:
                    return true;
                case LibmdbxResultCodeFlag.NOTFOUND:
                case LibmdbxResultCodeFlag.EKEYMISMATCH:
                    return false;
                default:
                    err.ThrowException(nameof(TryUpdate));
                    return false;
            }
        }

        #endregion

        #region EraseMulti

        public unsafe bool Erase<TK>(TK key, bool wholeMultivalue = true)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);

            fixed (byte* keyPtr = keyBuffer)
            {
                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer.Length);

                return Erase(ref dbKey, wholeMultivalue);
            }
        }

        public bool Erase(bool wholeMultivalue = false)
        {
            var err = mdbx_cursor_del(_cursorPtr, wholeMultivalue ? DatabasePutFlags.AllDups : DatabasePutFlags.Current);

            switch (err)
            {
                case LibmdbxResultCodeFlag.SUCCESS:
                    return true;
                case LibmdbxResultCodeFlag.NOTFOUND:
                    return false;
                default:
                    err.ThrowException(nameof(mdbx_cursor_del));
                    return false;
            }
        }

        public bool EraseMulti<TK, TV>(TK key, TV value)
        {
            return FindMultivalue(key, value, out var result) && Erase();
        }

        protected bool Erase(ref MdbxDbVal key, bool wholeMultivalue = true)
        {
            bool found = Seek(ref key);
            return found && Erase(wholeMultivalue);
        }

        #endregion

        #region Dispose

        private void ReleaseUnmanagedResources()
        {
            if (IsCompleted)
            {
                return;
            }

            Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// TODO - С++ 
        /// </summary>
        ~Cursor()
        {
            Dispose(false);
        }

        #endregion

        #endregion
    }
}
