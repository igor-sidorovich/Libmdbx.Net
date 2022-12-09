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
            MdbxDbVal dbValue = new MdbxDbVal(IntPtr.Zero, 0);

            return Move(MoveOperation.FindKey, ref key, ref dbValue, false);
        }

        protected LibmdbxResultCodeFlag Put(ref MdbxDbVal key, ref MdbxDbVal value, DatabasePutFlags flags)
        {
            return mdbx_cursor_put(_cursorPtr, ref key, ref value, flags);
        }

        protected bool Move<TK>(MoveOperation op, TK key, bool throwNotFound)
        {
            IntPtr keyPtr = IntPtr.Zero;

            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);

            try
            {
                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);
                MdbxDbVal dbValue = new MdbxDbVal(IntPtr.Zero, 0);

                return Move(op, ref dbKey, ref dbValue, throwNotFound);
            }
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }
            }
        }

        protected bool Move<TK, TV>(MoveOperation op, out CursorResult<TK, TV> cursorResult, bool throwNotFound)
        {
            cursorResult = default;

            MdbxDbVal dbKey = new MdbxDbVal(IntPtr.Zero, 0);
            MdbxDbVal dbValue = new MdbxDbVal(IntPtr.Zero, 0);

            var moveResult = Move(op, ref dbKey, ref dbValue, throwNotFound);

            if (moveResult)
            {
                byte[] keyBuffer = default;
                byte[] valueBuffer = default;

                if (dbKey.addr != IntPtr.Zero)
                {
                    keyBuffer = new byte[dbKey.Length];
                    Marshal.Copy(dbKey.addr, keyBuffer, 0, keyBuffer.Length);
                }

                if (dbValue.addr != IntPtr.Zero)
                {
                    valueBuffer = new byte[dbValue.Length];
                    Marshal.Copy(dbValue.addr, valueBuffer, 0, valueBuffer.Length);
                }

                TK key = keyBuffer != default ? BufferConverterFactory.Get<TK>().ConvertFromBuffer(keyBuffer) : default;
                TV value = valueBuffer != default ? BufferConverterFactory.Get<TV>().ConvertFromBuffer(valueBuffer) : default;

                cursorResult = new CursorResult<TK, TV>(key, value);
            }

            return moveResult;
        }

        protected bool Move<TK, TV>(MoveOperation op, out CursorResult<TK, TV> cursorResult, TK key, bool throwNotFound)
        {
            cursorResult = default;
            bool moveResult;
            IntPtr keyPtr = IntPtr.Zero;

            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);

            try
            {
                byte[] valueBuffer = default;

                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);
                MdbxDbVal dbValue = new MdbxDbVal(IntPtr.Zero, 0);

                moveResult = Move(op, ref dbKey, ref dbValue, throwNotFound);
                if (moveResult)
                {
                    if (dbKey.addr != IntPtr.Zero)
                    {
                        if (keyBuffer == default || keyBuffer.Length != dbKey.Length)
                        {
                            keyBuffer = new byte[dbKey.Length];
                        }

                        Marshal.Copy(dbKey.addr, keyBuffer, 0, keyBuffer.Length);
                    }
                    else
                    {
                        keyBuffer = default;
                    }

                    if (dbValue.addr != IntPtr.Zero)
                    {
                        valueBuffer = new byte[dbValue.Length];
                        Marshal.Copy(dbValue.addr, valueBuffer, 0, valueBuffer.Length);
                    }

                    TK cursorKey = keyBuffer != default ? BufferConverterFactory.Get<TK>().ConvertFromBuffer(keyBuffer) : default;
                    TV cursorValue = valueBuffer != default ? BufferConverterFactory.Get<TV>().ConvertFromBuffer(valueBuffer) : default;

                    cursorResult = new CursorResult<TK, TV>(cursorKey, cursorValue);
                }
            }
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }
            }

            return moveResult;
        }

        protected bool Move<TK, TV>(MoveOperation op, out CursorResult<TK, TV> cursorResult, TK key, TV value, bool throwNotFound)
        {
            cursorResult = default;
            bool moveResult;

            IntPtr keyPtr = IntPtr.Zero;
            IntPtr valuePtr = IntPtr.Zero;

            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            try
            {
                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                if (valueBuffer != default && valueBuffer.Length > 0)
                {
                    valuePtr = Marshal.AllocHGlobal(valueBuffer.Length);
                    Marshal.Copy(valueBuffer, 0, valuePtr, valueBuffer.Length);
                }

                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);
                MdbxDbVal dbValue = new MdbxDbVal(valuePtr, valueBuffer?.Length ?? 0);

                moveResult = Move(op, ref dbKey, ref dbValue, throwNotFound);
                if (moveResult)
                {
                    if (dbKey.addr != IntPtr.Zero)
                    {
                        if (keyBuffer == default || keyBuffer.Length != dbKey.Length)
                        {
                            keyBuffer = new byte[dbKey.Length];
                        }

                        Marshal.Copy(dbKey.addr, keyBuffer, 0, keyBuffer.Length);
                    }
                    else
                    {
                        keyBuffer = default;
                    }

                    if (dbValue.addr != IntPtr.Zero)
                    {
                        if (valueBuffer == default || valueBuffer.Length != dbValue.Length)
                        {
                            valueBuffer = new byte[dbValue.Length];
                        }

                        Marshal.Copy(dbValue.addr, valueBuffer, 0, valueBuffer.Length);
                    }
                    else
                    {
                        valueBuffer = default;
                    }

                    TK cursorKey = keyBuffer != default ? BufferConverterFactory.Get<TK>().ConvertFromBuffer(keyBuffer) : default;
                    TV cursorValue = valueBuffer != default ? BufferConverterFactory.Get<TV>().ConvertFromBuffer(valueBuffer) : default;

                    cursorResult = new CursorResult<TK, TV>(cursorKey, cursorValue);
                }
            }
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }
            }

            return moveResult;
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
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            IntPtr keyPtr = IntPtr.Zero;
            IntPtr valuePtr = IntPtr.Zero;

            try
            {
                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                if (valueBuffer != default && valueBuffer.Length > 0)
                {
                    valuePtr = Marshal.AllocHGlobal(valueBuffer.Length);
                    Marshal.Copy(valueBuffer, 0, valuePtr, valueBuffer.Length);
                }

                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);
                MdbxDbVal dbValue = new MdbxDbVal(valuePtr, valueBuffer?.Length ?? 0);

                Put(ref dbKey, ref dbValue, (DatabasePutFlags)PutMode.InsertUnique).ThrowException(nameof(Insert));
            }
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }

                if (valuePtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(valuePtr);
                }
            }
        }

        public bool TryInsert<TK, TV>(TK key, TV value)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            IntPtr keyPtr = IntPtr.Zero;
            IntPtr valuePtr = IntPtr.Zero;

            try
            {
                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                if (valueBuffer != default && valueBuffer.Length > 0)
                {
                    valuePtr = Marshal.AllocHGlobal(valueBuffer.Length);
                    Marshal.Copy(valueBuffer, 0, valuePtr, valueBuffer.Length);
                }

                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);
                MdbxDbVal dbValue = new MdbxDbVal(valuePtr, valueBuffer?.Length ?? 0);

                var err = Put(ref dbKey, ref dbValue, (DatabasePutFlags)PutMode.InsertUnique);

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
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }

                if (valuePtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(valuePtr);
                }
            }
        }

        #endregion

        #region Upsert

        public void Upsert<TK, TV>(TK key, TV value)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            IntPtr keyPtr = IntPtr.Zero;
            IntPtr valuePtr = IntPtr.Zero;

            try
            {
                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                if (valueBuffer != default && valueBuffer.Length > 0)
                {
                    valuePtr = Marshal.AllocHGlobal(valueBuffer.Length);
                    Marshal.Copy(valueBuffer, 0, valuePtr, valueBuffer.Length);
                }

                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);
                MdbxDbVal dbValue = new MdbxDbVal(valuePtr, valueBuffer?.Length ?? 0);

                Put(ref dbKey, ref dbValue, (DatabasePutFlags)PutMode.Upsert).ThrowException(nameof(Upsert));
            }
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }

                if (valuePtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(valuePtr);
                }
            }
        }

        #endregion

        #region Update

        public void Update<TK, TV>(TK key, TV value)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            IntPtr keyPtr = IntPtr.Zero;
            IntPtr valuePtr = IntPtr.Zero;

            try
            {
                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                if (valueBuffer != default && valueBuffer.Length > 0)
                {
                    valuePtr = Marshal.AllocHGlobal(valueBuffer.Length);
                    Marshal.Copy(valueBuffer, 0, valuePtr, valueBuffer.Length);
                }

                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);
                MdbxDbVal dbValue = new MdbxDbVal(valuePtr, valueBuffer?.Length ?? 0);

                Put(ref dbKey, ref dbValue, (DatabasePutFlags)PutMode.Update).ThrowException(nameof(Update));
            }
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }

                if (valuePtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(valuePtr);
                }
            }
        }

        public bool TryUpdate<TK, TV>(TK key, TV value)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            IntPtr keyPtr = IntPtr.Zero;
            IntPtr valuePtr = IntPtr.Zero;

            try
            {
                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                if (valueBuffer != default && valueBuffer.Length > 0)
                {
                    valuePtr = Marshal.AllocHGlobal(valueBuffer.Length);
                    Marshal.Copy(valueBuffer, 0, valuePtr, valueBuffer.Length);
                }

                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);
                MdbxDbVal dbValue = new MdbxDbVal(valuePtr, valueBuffer?.Length ?? 0);

                var err = Put(ref dbKey, ref dbValue, (DatabasePutFlags)PutMode.Update);

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
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }

                if (valuePtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(valuePtr);
                }
            }
        }

        #endregion

        #region EraseMulti

        public bool Erase<TK>(TK key, bool wholeMultivalue = true)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);

            IntPtr keyPtr = IntPtr.Zero;

            try
            {
                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);

                return Erase(ref dbKey, wholeMultivalue);
            }
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }
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
