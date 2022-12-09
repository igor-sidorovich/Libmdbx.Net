using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using static Libmdbx.Net.Bindings.MdbxEnv;
using static Libmdbx.Net.Bindings.MdbxDb;
using static Libmdbx.Net.Bindings.MdbxTran;
using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net
{
    public class Env : IEnv
    {
        #region Fields

        protected IntPtr envPtr;

        #endregion

        #region Properties

        public IntPtr EnvPtr => envPtr;

        public bool Closed => envPtr == IntPtr.Zero;

        #endregion

        #region Constructor

        public Env(IntPtr envPtr)
        {
            this.envPtr = envPtr;
        }

        public Env(string pathName, OperateParameters op, bool accede = true) : this(CreateEnv())
        {
            Setup(op.maxMaps, op.maxReaders);

            mdbx_env_open(envPtr, pathName, op.MakeFlags(accede), 0).ThrowException(nameof(mdbx_env_open));

            if (op.options.nestedWriteTransactions &&
                !GetOptions().nestedWriteTransactions)
            {
                LibmdbxResultCodeFlag.INCOMPATIBLE.ThrowException(nameof(mdbx_env_open));
            }
        }

        public Env(string pathName, CreateParameters cp, OperateParameters op, bool accede = true) : this(CreateEnv())
        {
            Setup(op.maxMaps, op.maxReaders);

            SetGeometry(cp.geometry);

            mdbx_env_open(envPtr, pathName, op.MakeFlags(accede, cp.useSubDirectory), cp.fileModeBits).ThrowException(nameof(mdbx_env_open));

            if (op.options.nestedWriteTransactions && !GetOptions().nestedWriteTransactions)
            {
                LibmdbxResultCodeFlag.INCOMPATIBLE.ThrowException(nameof(mdbx_env_open));
            }
        }

        public Env(string pathName, CreateParameters cp, OperateParameters op) : this(pathName, cp, op, true)
        {
        }

        public Env(string pathName, OperateParameters op) : this(pathName, op, true)
        {
        }

        #endregion

        #region Methods

        public OperateOptions GetOptions()
        {
            return OperateParameters.OptionsFromFlags(GetFlags());
        }

        public void CloseMap(MapHandle handle) 
        {
            mdbx_dbi_close(envPtr, handle.dbi).ThrowException(nameof(mdbx_dbi_close));
        }

        public UIntPtr GetPageSize()
        {
            return new UIntPtr(GetStat().ms_psize);
        }

        public MdbxDbStat GetStat()
        {
            var bytes = UIntPtr.Add(UIntPtr.Zero, Marshal.SizeOf<MdbxDbStat>());
            mdbx_env_stat_ex(envPtr, IntPtr.Zero, out var stat, bytes).ThrowException(nameof(mdbx_env_stat_ex));
            return stat;
        }

        public IntPtr LimitsDbsizeMax(UIntPtr pagesize) 
        {
            var dbSizeMax = mdbx_limits_dbsize_max(pagesize);
            if (IntPtr.Size == 4)
            {
                var result = dbSizeMax.ToInt32();
                if (result < 0)
                {
                    LibmdbxResultCodeFlag.EINVAL.ThrowException(nameof(mdbx_limits_dbsize_max));
                }
            }
            else if(IntPtr.Size == 8)
            {
                var result = dbSizeMax.ToInt64();
                if (result < 0)
                {
                    LibmdbxResultCodeFlag.EINVAL.ThrowException(nameof(mdbx_limits_dbsize_max));
                }
            }

            return dbSizeMax;
        }

        public static bool Remove(string pathname, RemoveMode mode = RemoveMode.JustRemove)
        {
            var result = mdbx_env_delete(pathname, mode);
            return result.BooleanOrThrow(nameof(mdbx_env_delete));
        }

        bool IEnv.Remove(string pathname, RemoveMode mode = RemoveMode.JustRemove)
        {
            return Remove(pathname, mode);
        }

        public bool IsEmpty()
        {
            return GetStat().ms_leaf_pages == 0;
        }

        public IntPtr LimitsDbsizeMin(UIntPtr pagesize) 
        {
            var dbSizeMin = mdbx_limits_dbsize_min(pagesize);
            if (IntPtr.Size == 4)
            {
                var result = dbSizeMin.ToInt32();
                if (result < 0)
                {
                    LibmdbxResultCodeFlag.EINVAL.ThrowException(nameof(mdbx_limits_dbsize_max));
                }
            }
            else if (IntPtr.Size == 8)
            {
                var result = dbSizeMin.ToInt64();
                if (result < 0)
                {
                    LibmdbxResultCodeFlag.EINVAL.ThrowException(nameof(mdbx_limits_dbsize_max));
                }
            }

            return dbSizeMin;
        }

        public IntPtr DbsizeMax()
        {
            return LimitsDbsizeMax(GetPageSize());
        }

        public IntPtr DbsizeMin()
        {
            return LimitsDbsizeMin(GetPageSize());
        }

        public IEnv SetGeometry(Geometry geo)
        {
            mdbx_env_set_geometry(envPtr, 
                geo.sizeLower, 
                geo.sizeNow, 
                geo.sizeUpper, 
                geo.growthStep, 
                geo.shrinkThreshold, 
                geo.pageSize).ThrowException(nameof(mdbx_env_set_geometry));

            return this;
        }

        public static IntPtr DefaultPageSize()
        {
            return mdbx_default_pagesize();
        }

        IntPtr IEnv.DefaultPageSize()
        {
            return DefaultPageSize();
        }

        public string GetPath()
        {
            mdbx_env_get_path(envPtr, out IntPtr ptr).ThrowException(nameof(mdbx_env_get_path));
            return Marshal.PtrToStringAnsi(ptr);
        }

        public uint MaxMaps()
        {
            mdbx_env_get_maxdbs(envPtr, out uint r).ThrowException(nameof(mdbx_env_get_maxdbs));
            return r;
        }

        public uint MaxReaders()
        {
            var code = mdbx_env_get_maxreaders(envPtr, out uint r);
            code.ThrowException(nameof(mdbx_env_get_maxreaders));
            return r;
        }

        public IEnv Copy(string destination, bool compactify, bool force_dynamic_size) 
        {
            mdbx_env_copy(envPtr, destination, (compactify ? MDBX_CP_COMPACT : MDBX_CP_DEFAULTS) | (force_dynamic_size ? MDBX_CP_FORCE_DYNAMIC_SIZE : MDBX_CP_DEFAULTS)).ThrowException(nameof(mdbx_env_copy));
            return this;
        }

        public bool SyncToDisk(bool force = true, bool nonblock = false)
        {
            var err = mdbx_env_sync_ex(envPtr, force, nonblock);
            switch (err)
            {
                case LibmdbxResultCodeFlag.SUCCESS /* flush done */:
                case LibmdbxResultCodeFlag.RESULT_TRUE /* no data pending for flush to disk */:
                    return true;
                case LibmdbxResultCodeFlag.BUSY /* the environment is used by other thread */:
                    return false;
                default:
                    err.ThrowException(nameof(mdbx_env_sync_ex));
                    return false;
            }
        }

        public EnvFlags GetFlags()
        {
            mdbx_env_get_flags(envPtr, out var flags).ThrowException(nameof(mdbx_env_get_flags));
            return (EnvFlags)flags;
        }

        public ITxn StartWrite(bool dontWait = false) 
        {
            mdbx_txn_begin(envPtr, IntPtr.Zero, dontWait ? TxnFlags.Try : TxnFlags.ReadWrite, out var ptr).ThrowException(nameof(StartWrite));
            Debug.Assert(ptr != IntPtr.Zero);
            return new Txn(ptr);
        }

        public ITxn TryStartWrite()
        {
            return StartWrite(true);
        }

        public ITxn StartRead()
        {
            mdbx_txn_begin(envPtr, IntPtr.Zero, TxnFlags.ReadOnly, out var ptr).ThrowException(nameof(StartRead));
            Debug.Assert(ptr != IntPtr.Zero);
            return new Txn(ptr);
        }

        public void Close(bool dontSync = false)
        {
            var errorCode = mdbx_env_close_ex(envPtr, dontSync);

            switch (errorCode)
            {
                case LibmdbxResultCodeFlag.SUCCESS:
                    envPtr = IntPtr.Zero;
                    break;
                case LibmdbxResultCodeFlag.EBADSIGN:
                    envPtr = IntPtr.Zero;
                    errorCode.ThrowException(nameof(mdbx_env_close_ex));
                    break;
                default:
                    errorCode.ThrowException(nameof(mdbx_env_close_ex));
                    break;
            }
        }

        private void Setup(uint maxMaps, uint maxReaders)
        {
            if (maxReaders > 0)
            {
                mdbx_env_set_maxreaders(envPtr, maxReaders).ThrowException(nameof(mdbx_env_set_maxreaders));
            }

            if (maxMaps > 0)
            {
                mdbx_env_set_maxdbs(envPtr, maxMaps).ThrowException(nameof(mdbx_env_set_maxdbs));
            }
        }

        private static IntPtr CreateEnv()
        {
            mdbx_env_create(out var ptr).ThrowException(nameof(mdbx_env_create));
            return ptr;
        }

        #endregion

        #region Dispose

        private void ReleaseUnmanagedResources()
        {
            if (Closed)
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
        ~Env()
        {
            Dispose(false);
        }

        #endregion
    }
}
