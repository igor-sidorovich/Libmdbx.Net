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

        public TV Get<TK, TV>(MapHandle map, TK key)
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

                mdbx_get(txnPtr, map.dbi, ref dbKey, ref dbValue).ThrowException(nameof(mdbx_get));

                if (dbValue.addr != IntPtr.Zero)
                {
                    var valueBuffer = new byte[dbValue.Length];
                    Marshal.Copy(dbValue.addr, valueBuffer, 0, valueBuffer.Length);

                    return BufferConverterFactory.Get<TV>().ConvertFromBuffer(valueBuffer);
                }
                else
                {
                    return default;
                }
            }
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }
            }
        }

        public TV Get<TK, TV>(MapHandle map, TK key, out uint valuesCount)
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

                var ptr = UIntPtr.Add(UIntPtr.Zero, Marshal.SizeOf<uint>());

                mdbx_get_ex(txnPtr, map.dbi, ref dbKey, ref dbValue, ref ptr).ThrowException(nameof(mdbx_get_ex));

                if (dbValue.addr != IntPtr.Zero)
                {
                    valuesCount = ptr.ToUInt32();

                    var valueBuffer = new byte[dbValue.Length];
                    Marshal.Copy(dbValue.addr, valueBuffer, 0, valueBuffer.Length);

                    return BufferConverterFactory.Get<TV>().ConvertFromBuffer(valueBuffer);
                }
                else
                {
                    valuesCount = default;
                    return default;
                }
            }
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }
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

        protected LibmdbxResultCodeFlag Put<TK, TV>(MapHandle map, TK key, TV value, PutMode mode)
        {
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

                return mdbx_put(txnPtr, map.dbi, ref dbKey, ref dbValue, (DatabasePutFlags)mode);
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

        public bool Erase<TK, TV>(MapHandle map, TK key, TV value)
        {
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

                var code = mdbx_del(txnPtr, map.dbi, ref dbKey, ref dbValue);

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

        public bool Erase<TK>(MapHandle map, TK key)
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

                var code = mdbx_del(txnPtr, map.dbi, ref dbKey, IntPtr.Zero);

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
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }
            }
        }

        #endregion

        #region Replace

        public void Replace<TK, TV>(MapHandle map, TK key, TV oldValue, TV newValue, bool multiple = false)
        {
            IntPtr keyPtr = IntPtr.Zero;
            IntPtr oldValuePtr = IntPtr.Zero;
            IntPtr newValuePtr = IntPtr.Zero;

            var keyBuffer = BufferConverterFactory.Get<TK>().ConvertToBuffer(key);
            var oldValueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(oldValue);
            var newValueBuffer = BufferConverterFactory.Get<TV>().ConvertToBuffer(newValue);

            try
            {
                if (keyBuffer != default && keyBuffer.Length > 0)
                {
                    keyPtr = Marshal.AllocHGlobal(keyBuffer.Length);
                    Marshal.Copy(keyBuffer, 0, keyPtr, keyBuffer.Length);
                }

                if (oldValueBuffer != default && oldValueBuffer.Length > 0)
                {
                    oldValuePtr = Marshal.AllocHGlobal(oldValueBuffer.Length);
                    Marshal.Copy(oldValueBuffer, 0, oldValuePtr, oldValueBuffer.Length);
                }

                if (newValueBuffer != default && newValueBuffer.Length > 0)
                {
                    newValuePtr = Marshal.AllocHGlobal(newValueBuffer.Length);
                    Marshal.Copy(newValueBuffer, 0, newValuePtr, newValueBuffer.Length);
                }

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
            finally
            {
                if (keyPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(keyPtr);
                }

                if (oldValuePtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(oldValuePtr);
                }

                if (newValuePtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(newValuePtr);
                }
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