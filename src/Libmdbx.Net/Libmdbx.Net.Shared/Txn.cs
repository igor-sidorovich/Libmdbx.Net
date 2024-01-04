using System;
using Libmdbx.Net.Bindings;
using System.Runtime.InteropServices;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Common.BufferConverts;
using Libmdbx.Net.Core.Cursor;
using Libmdbx.Net.Core.Transaction;
using static Libmdbx.Net.Bindings.MdbxDb;
using static Libmdbx.Net.Bindings.MdbxTran;
using static Libmdbx.Net.Bindings.MdbxCursor;
using Libmdbx.Net.Core.Env;
using System.Reflection.Metadata;

namespace Libmdbx.Net
{
    public class Txn : ITxn
    {
        #region Fields
        
        protected IntPtr txnPtr;
        private IntPtr _env;

        #endregion

        #region Properties

        public IntPtr Env => mdbx_txn_env(txnPtr);

        public IntPtr TxnPtr => txnPtr;

        public bool Completed => txnPtr == IntPtr.Zero;

        public TxnFlags Flags
        {
            get
            {
                int bits = mdbx_txn_flags(txnPtr);
                if (bits == -1)
                {
                    LibmdbxResultCodeFlag.BAD_TXN.ThrowException(nameof(mdbx_txn_flags));
                }

                return (TxnFlags)bits;
            }
        }

        public ulong Id
        {
            get
            {
                ulong txnId = mdbx_txn_id(txnPtr);
                if (txnId == 0)
                {
                    LibmdbxResultCodeFlag.BAD_TXN.ThrowException(nameof(mdbx_txn_id));
                }

                return txnId;
            }
        }

        public bool IsReadOnly => (Flags & TxnFlags.ReadOnly) != 0;

        public bool IsReadWrite => (Flags & TxnFlags.ReadOnly) == 0;

        public ulong SizeCurrent => GetInfo().txn_space_dirty;

        #endregion

        #region Constructor

        public Txn(IntPtr txnPtr)
        {
            this.txnPtr = txnPtr;
        }

        #endregion

        #region Methods

        public void Abort()
        {
            var err = mdbx_txn_abort(txnPtr);

            if (err != LibmdbxResultCodeFlag.THREAD_MISMATCH)
            {
                txnPtr = IntPtr.Zero;
            }

            if (err != LibmdbxResultCodeFlag.SUCCESS)
            {
                err.ThrowException(nameof(Abort));
            }
        }

        public void Commit()
        {
            var err = mdbx_txn_commit(txnPtr);

            if (err != LibmdbxResultCodeFlag.THREAD_MISMATCH)
            {
                txnPtr = IntPtr.Zero;
            }

            if (err != LibmdbxResultCodeFlag.SUCCESS)
            {
                err.ThrowException(nameof(Abort));
            }
        }

        public bool IsDirty(IntPtr ptr)
        {
            return mdbx_is_dirty(txnPtr, ptr).BooleanOrThrow(nameof(mdbx_is_dirty));
        }

        public MdbxTxnInfo GetInfo(bool scanReaderLockTable = false)
        {
            MdbxTxnInfo r = new MdbxTxnInfo();
            mdbx_txn_info(txnPtr, ref r, scanReaderLockTable).ThrowException(nameof(mdbx_txn_info));
            return r;
        }

        public MapHandle CreateMap(string name, KeyMode keyMode, ValueMode valueMode) 
        {
            MapHandle map;
            mdbx_dbi_open(txnPtr, name, DatabaseOpenFlags.Create | (DatabaseOpenFlags)keyMode | (DatabaseOpenFlags)valueMode, out map.dbi).ThrowException(nameof(mdbx_dbi_open));
            return map;
        }

        public MapHandle OpenMap(string name, KeyMode keyMode, ValueMode valueMode)
        {
            MapHandle map;
            mdbx_dbi_open(txnPtr, name, (DatabaseOpenFlags)keyMode | (DatabaseOpenFlags)valueMode, out map.dbi).ThrowException(nameof(mdbx_dbi_open));
            return map;
        }

        public void DropMap(MapHandle map)
        {
            mdbx_drop(txnPtr, map.dbi, true).ThrowException(nameof(mdbx_drop));
        }

        public bool DropMap(string name, bool throwIfAbsent = false) 
        {
            MapHandle map;
            var err = mdbx_dbi_open(txnPtr, name, DatabaseOpenFlags.DbAccede, out map.dbi);

            switch (err) 
            {
                case LibmdbxResultCodeFlag.SUCCESS:
                    DropMap(map);
                    return true;
                case LibmdbxResultCodeFlag.NOTFOUND:
                case LibmdbxResultCodeFlag.BAD_DBI:
                    if (!throwIfAbsent)
                    {
                        return false;
                    }
                    err.ThrowException(nameof(mdbx_dbi_open));
                    return false;
                default:
                    err.ThrowException(nameof(mdbx_dbi_open));
                    return false;
            }
        }

        public void ClearMap(MapHandle map)
        {
            mdbx_drop(txnPtr, map.dbi, false).ThrowException(nameof(mdbx_drop));
        }

        public bool ClearMap(string name, bool throwIfAbsent = false) 
        {
            MapHandle map;
            var err = mdbx_dbi_open(txnPtr, name, DatabaseOpenFlags.DbAccede, out map.dbi);

            switch (err)
            {
                case LibmdbxResultCodeFlag.SUCCESS:
                    ClearMap(map);
                    return true;
                case LibmdbxResultCodeFlag.NOTFOUND:
                case LibmdbxResultCodeFlag.BAD_DBI:
                    if (!throwIfAbsent)
                    {
                        return false;
                    }
                    err.ThrowException(nameof(mdbx_dbi_open));
                    return false;
                default:
                    err.ThrowException(nameof(mdbx_dbi_open));
                    return false;
            }
        }

        public ICursor OpenCursor(MapHandle map)
        {
            mdbx_cursor_open(txnPtr, map.dbi, out IntPtr ptr).ThrowException(nameof(mdbx_cursor_open));
            return new Cursor(ptr);
        }

        #region Get

        public unsafe TV Get<TK, TV>(MapHandle map, TK key)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            fixed (byte* keyPtr = keyBuffer)
            {
                var dbKey = new MdbxDbVal(keyPtr, keyBuffer.Length);

                mdbx_get(txnPtr, map.dbi, ref dbKey, out var dbValue).ThrowException(nameof(mdbx_get));

                return BufferConverterFactory.Get<TV>().ConvertFromBuffer(dbValue.AsSpan());
            }
        }

        public unsafe TV Get<TK, TV>(MapHandle map, TK key, out uint valuesCount)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            fixed (byte* keyPtr = keyBuffer)
            {
                var dbKey = new MdbxDbVal(keyPtr, keyBuffer.Length);

                valuesCount = 0;

                mdbx_get_ex(txnPtr, map.dbi, ref dbKey, out var dbValue, ref valuesCount).ThrowException(nameof(mdbx_get_ex));

                return BufferConverterFactory.Get<TV>().ConvertFromBuffer(dbValue.AsSpan());
            }
        }

        public bool TryGet<TK, TV>(MapHandle map, TK key, out TV value)
        {
            try
            {
                value = Get<TK, TV>(map, key);
            }
            catch (LibmdbxException ex)
            {
                if (ex.ErrorCode == LibmdbxResultCodeFlag.NOTFOUND)
                {
                    value = default;
                    return false;
                }

                throw;
            }

            return true;
        }

        #endregion

        #region Put

        protected unsafe LibmdbxResultCodeFlag Put<TK, TV>(MapHandle map, TK key, TV value, PutMode mode)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            fixed (byte* keyPtr = keyBuffer)
            fixed (byte* valuePtr = valueBuffer)
            {
                MdbxDbVal mdbxKey = new MdbxDbVal(keyPtr, keyBuffer.Length);
                MdbxDbVal mdbxValue = new MdbxDbVal(valuePtr, valueBuffer.Length);
                return mdbx_put(txnPtr, map.dbi, ref mdbxKey, ref mdbxValue, (DatabasePutFlags)mode);
            }
        }

        #endregion

        #region Insert

        public void Insert<TK, TV>(MapHandle map, TK key, TV value)
        {
            Put(map, key, value, PutMode.InsertUnique).ThrowException(nameof(Insert));
        }

        public bool TryInsert<TK, TV>(MapHandle map, TK key, TV value)
        {
            var err = Put(map, key, value, PutMode.InsertUnique);

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

        public void Upsert<TK, TV>(MapHandle map, TK key, TV value)
        {
            Put(map, key, value, PutMode.Upsert).ThrowException(nameof(Upsert));
        }

        #endregion

        #region Update

        public void Update<TK, TV>(MapHandle map, TK key, TV value)
        {
            Put(map, key, value, PutMode.Update).ThrowException(nameof(Update));
        }

        public bool TryUpdate<TK, TV>(MapHandle map, TK key, TV value)
        {
            var err = Put(map, key, value, PutMode.Update);

            switch (err)
            {
                case LibmdbxResultCodeFlag.SUCCESS:
                    return true;
                case LibmdbxResultCodeFlag.NOTFOUND:
                    return false;
                default:
                    err.ThrowException(nameof(TryUpdate));
                    return false;
            }
        }

        #endregion

        #region EraseMulti

        public unsafe bool Erase<TK, TV>(MapHandle map, TK key, TV value)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var valueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(value);

            fixed (byte* keyPtr = keyBuffer)
            fixed (byte* valuePtr = valueBuffer)
            {
                MdbxDbVal mdbxKey = new MdbxDbVal(keyPtr, keyBuffer.Length);
                MdbxDbVal mdbxValue = new MdbxDbVal(valuePtr, valueBuffer.Length);

                var code = mdbx_del(txnPtr, map.dbi, ref mdbxKey, ref mdbxValue);

                switch (code)
                {
                    case LibmdbxResultCodeFlag.SUCCESS:
                        return true;
                    case LibmdbxResultCodeFlag.NOTFOUND:
                        return false;
                    default:
                        code.ThrowException(nameof(Erase));
                        return false;
                }
            }
        }

        public unsafe bool Erase<TK>(MapHandle map, TK key)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            fixed (byte* keyPtr = keyBuffer)
            {
                MdbxDbVal mdbxKey = new MdbxDbVal(keyPtr, keyBuffer.Length);

                var code = mdbx_del(txnPtr, map.dbi, ref mdbxKey, IntPtr.Zero);

                switch (code)
                {
                    case LibmdbxResultCodeFlag.SUCCESS:
                        return true;
                    case LibmdbxResultCodeFlag.NOTFOUND:
                        return false;
                    default:
                        code.ThrowException(nameof(Erase));
                        return false;
                }
            }
        }

        #endregion

        #region Replace

        public unsafe void Replace<TK, TV>(MapHandle map, TK key, TV oldValue, TV newValue, bool multiple = false)
        {
            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var oldValueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(oldValue);
            var newValueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(newValue);

            fixed (byte* keyPtr = keyBuffer)
            fixed (byte* oldValuePtr = oldValueBuffer)
            fixed (byte* newValuePtr = newValueBuffer)
            {
                MdbxDbVal dbKey = new MdbxDbVal(keyPtr, keyBuffer?.Length ?? 0);
                MdbxDbVal dbNewValue = new MdbxDbVal(newValuePtr, newValueBuffer?.Length ?? 0);
                MdbxDbVal dbOldValue = new MdbxDbVal(oldValuePtr, oldValueBuffer?.Length ?? 0);

                mdbx_replace(txnPtr,
                        map.dbi,
                        ref dbKey,
                        ref dbNewValue,
                        ref dbOldValue,
                        multiple ? DatabasePutFlags.Current | DatabasePutFlags.NoOverwrite : DatabasePutFlags.Current)
                    .ThrowException(nameof(mdbx_replace_ex));
            }
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// TODO - C++
        /// </summary>
        private void ReleaseUnmanagedResources()
        {
            if (Completed)
            {
                return;
            }

            Abort();
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
        ~Txn()
        {
            Dispose(false);
        }

        #endregion
    }
}