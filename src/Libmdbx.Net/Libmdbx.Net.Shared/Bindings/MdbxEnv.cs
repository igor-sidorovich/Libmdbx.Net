using System;
using System.Runtime.InteropServices;
using System.Security;
using Libmdbx.Net.Core.Env;
using static Libmdbx.Net.Bindings.Const;

namespace Libmdbx.Net.Bindings
{
    internal static class MdbxEnv
    {
        /// <summary>
        /// int mdbx_env_create(MDBX_env **penv)
        /// 
        /// Create an MDBX environment instance.
        /// This function allocates memory for a MDBX_env structure.To release the allocated memory and discard the handle, call mdbx_env_close().
        /// Before the handle may be used, it must be opened using mdbx_env_open().
        /// Various other options may also need to be set before opening the handle, e.g.mdbx_env_set_geometry(), mdbx_env_set_maxreaders(), mdbx_env_set_maxdbs(), depending on usage requirements.
        /// </summary>
        /// <param name="env">The address where the new handle will be stored.</param>
        /// <returns>a non-zero error value on failure and 0 on success.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_create")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_env_create(out IntPtr env);

        /// <summary>
        /// int mdbx_env_open(MDBX_env* Env, const char* pathname, MDBX_env_flags_t flagsT, mdbx_mode_t Mode)
        /// 
        /// Open an environment instance.
        /// Indifferently this function will fails or not, the mdbx_env_close() must be called later to discard the MDBX_env handle and release associated resources.
        /// Flags set by mdbx_env_set_flags() are also used:
        /// MDBX_NOSUBDIR, MDBX_RDONLY, MDBX_EXCLUSIVE, MDBX_WRITEMAP, MDBX_NOTLS, MDBX_NORDAHEAD, MDBX_NOMEMINIT, MDBX_COALESCE, MDBX_LIFORECLAIM.See env_flags section.
        /// MDBX_NOMETASYNC, MDBX_SAFE_NOSYNC, MDBX_UTTERLY_NOSYNC.See SYNC MODES section.
        /// Note
        /// MDB_NOLOCK flag don't supported by MDBX, try use MDBX_EXCLUSIVE as a replacement.
        /// MDBX don't allow to mix processes with different MDBX_SAFE_NOSYNC flagsT on the same environment. In such case MDBX_INCOMPATIBLE will be returned.
        /// If the database is already exist and parameters specified early by mdbx_env_set_geometry() are incompatible (i.e. for instance, different page Size) then mdbx_env_open() will return MDBX_INCOMPATIBLE error.
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create()</param>
        /// <param name="path">The pathname for the database or the directory in which the database files reside. In the case of directory it must already exist and be writable.</param>
        /// <param name="flagsT">Special options for this environment. This parameter must be set to 0 or by bitwise OR'ing together one or more of the values described above in the env_flags and SYNC MODES sections.</param>
        /// <param name="mode">Mode	The UNIX permissions to set on created files. Zero value means to open existing, but do not Сreate.</param>
        /// <returns>A non-zero error value on failure and 0 on success, some possible errors are:
        ///  MDBX_VERSION_MISMATCH The version of the MDBX library doesn't match the version that created the database environment.
        ///  MDBX_INVALID The environment file headers are corrupted.
        ///  MDBX_ENOENT The directory specified by the path parameter doesn't exist.
        ///  MDBX_EACCES The user didn't have permission to access the environment files.
        ///  MDBX_EAGAIN The environment was locked by another process.
        ///  MDBX_BUSY   The MDBX_EXCLUSIVE flag was specified and the environment is in use by another process, or the current process tries to open environment more than once.
        ///  MDBX_INCOMPATIBLE Env is already opened by another process, but with different set of MDBX_SAFE_NOSYNC, MDBX_UTTERLY_NOSYNC flagsT. Or if the database is already exist and parameters specified early by mdbx_env_set_geometry() are incompatible (i.e.different pageSize, etc).
        ///  MDBX_WANNA_RECOVERY The MDBX_RDONLY flag was specified but read-write access is required to rollback inconsistent state after a system crash.
        ///  MDBX_TOO_LARGE MdbxDb is too large for this process, i.e. 32-bit process tries to open >4Gb database.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_open")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_open(IntPtr env,
            [In, MarshalAs(UnmanagedType.LPStr)] string path,
            [In, MarshalAs(UnmanagedType.U4)] EnvFlags flagsT,
            [In, MarshalAs(UnmanagedType.U2)] ushort mode);

        /// <summary>
        /// int mdbx_env_close(MDBX_env* Env)
        /// 
        /// The shortcut to calling mdbx_env_close_ex() with the dont_sync=false argument.
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create()</param>
        /// <returns></returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_close")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_close(IntPtr env);

        /// <summary>
        /// int mdbx_env_close_ex(MDBX_env * Env, bool dont_sync)
        ///
        /// Close the environment and release the memory map.
        /// Only a Single thread may call this function.
        /// All transactions, databases, and cursors must already be closed before calling this function.
        /// Attempts to use any such handles after calling this function will cause a SIGSEGV.
        /// The environment handle will be freed and must not be used again after this call.
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create()</param>
        /// <param name="dont_sync">A dont'sync flag, if non-zero the Last checkpoint will be kept "as is" and may be still "weak" in the MDBX_SAFE_NOSYNC or MDBX_UTTERLY_NOSYNC modes. Such "weak" checkpoint will be ignored on opening Next time, and transactions since the Last non-weak checkpoint (meta-page Update) will rolledback for consistency guarantee.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_BUSY The write transaction is running by other thread, in such case MDBX_env instance has NOT be destroyed not released!
        /// Note
        ///    If any OTHER error code was returned then given MDBX_env instance has been destroyed and released.
        /// Return values
        ///    MDBX_EBADSIGN Env handle already closed or not valid, i.e.mdbx_env_close() was already called for the Env or was not created by mdbx_env_create().
        ///    MDBX_PANIC If mdbx_env_close_ex() was called in the child process after fork(). In this case MDBX_PANIC is expected, i.e.MDBX_env instance was freed in proper manner.
        ///    MDBX_EIO An error occurred during synchronization.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_close_ex")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_close_ex(IntPtr env,
            bool dont_sync);

        /// <summary>
        /// int mdbx_env_info_ex(const MDBX_env* Env, const MDBX_txn* Txn, MDBX_envinfo* Info, size_t bytes)
        ///
        /// Return information about the MDBX environment.
        ///
        /// At least one of Env or Txn argument must be non-null. If Txn is passed non-null then stat will be filled accordingly to the given transaction.
        /// Otherwise, if Txn is null, then stat will be populated by a snapshot from the Last committed write transaction, and at Next time, other information can be returned.
        ///
        /// Legacy mdbx_env_info() correspond to calling mdbx_env_info_ex() with the null Txn argument.
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create()</param>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin()</param>
        /// <param name="info">The address of an MDBX_envinfo structure where the information will be copied</param>
        /// <param name="bytes">The Size of MDBX_envinfo</param>
        /// <returns>A non-zero error value on failure and 0 on success.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_info_ex")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_info_ex(IntPtr env,
            IntPtr txn,
            ref MdbxEnvInfo info,
            UIntPtr bytes);


        /// <summary>
        ///int mdbx_env_stat_ex(const MDBX_env* Env, const MDBX_txn* Txn, MDBX_stat* stat, size_t bytes)	
        /// 
        /// Return statistics about the MDBX environment.
        ///
        /// At least one of Env or Txn argument must be non-null.
        /// If Txn is passed non-null then stat will be filled accordingly to the given transaction.
        /// Otherwise, if Txn is null, then stat will be populated by a snapshot from the Last committed write transaction, and at Next time, other information can be returned.
        ///
        /// Legacy mdbx_env_stat() correspond to calling mdbx_env_stat_ex() with the null Txn argument.
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create()</param>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin()</param>
        /// <param name="stat">The address of an MDBX_stat structure where the statistics will be copied</param>
        /// <param name="bytes">The Size of MDBX_stat</param>
        /// <returns>A non-zero error value on failure and 0 on success.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_stat_ex")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_stat_ex(IntPtr env,
            IntPtr txn,
            out MdbxDbStat stat,
            UIntPtr bytes);

        /// <summary>
        /// int mdbx_env_set_maxreaders(MDBX_env * 	Env, unsigned readers)		
        /// 
        /// Set the maximum number of threads/reader slots for all processes interacts with the database.
        ///
        /// This defines the number of slots in the lock table that is used to track readers in the the environment.
        /// The default is about 100 for 4K system page Size.
        /// Starting a read-only transaction normally ties a lock table slot to the current thread until the environment closes or the thread exits.
        /// If MDBX_NOTLS is in use, mdbx_txn_begin() instead ties the slot to the MDBX_txn object until it or the MDBX_env object is destroyed.
        /// This function may only be called after mdbx_env_create() and before mdbx_env_open(), and has an effect only when the database is opened by the First process interacts with the database.
        /// 
        /// See also
        ///     mdbx_env_get_maxreaders()
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create()</param>
        /// <param name="readers">The maximum number of reader lock table slots</param>
        ///
        /// 
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// 
        /// Return values
        ///    MDBX_EINVAL An invalid parameter was specified.
        ///    MDBX_EPERM The environment is already open.
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_set_maxreaders")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_set_maxreaders(IntPtr env,
            [In, MarshalAs(UnmanagedType.U4)] uint readers);

        /// <summary>
        /// int mdbx_env_set_maxdbs(MDBX_env* Env, MDBX_dbi dbs)	
        /// 
        /// Set the maximum number of named databases for the environment.
        /// This function is only needed if multiple databases will be used in the environment.
        /// Simpler applications that use the environment as a Single unnamed database can ignore this option.
        /// This function may only be called after mdbx_env_create() and before mdbx_env_open().
        /// Currently a moderate number of slots are cheap but a huge number gets expensive:
        /// 7-120 words per transaction, and every mdbx_dbi_open() does a linear search of the opened slots.
        /// 
        /// See also
        ///     mdbx_env_get_maxdbs()
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create().</param>
        /// <param name="dbs">The maximum number of databases.</param>
        /// <returns>A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_EINVAL	An invalid parameter was specified.
        ///     MDBX_EPERM	The environment is already open.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_set_maxdbs")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_set_maxdbs(IntPtr env,
            [In, MarshalAs(UnmanagedType.U4)] uint dbs);


        /// <summary>
        /// int mdbx_env_get_maxreaders	(const MDBX_env* Env, unsigned* readers)
        ///
        /// Get the maximum number of threads/reader slots for the environment.
        /// See also
        ///     mdbx_env_set_maxreaders()
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create().</param>
        /// <param name="readers">Address of an integer to store the number of readers.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_get_maxreaders")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_get_maxreaders(IntPtr env,
            [Out, MarshalAs(UnmanagedType.U4)] out uint readers);


        /// <summary>
        /// int mdbx_env_get_maxdbs	(const MDBX_env* Env,MDBX_dbi* dbs)	
        /// </summary>
        /// Get the maximum number of named databases for the environment.

        /// See also
        ///     mdbx_env_set_maxdbs()
        /// <param name="env">An environment handle returned by mdbx_env_create()</param>
        /// <param name="dbs">Address to store the maximum number of databases</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// 
        /// Return values
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_get_maxdbs")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_get_maxdbs(IntPtr env,
            [Out, MarshalAs(UnmanagedType.U4)] out uint dbs);


        /// <summary>
        /// int mdbx_env_get_flags(const MDBX_env* Env, unsigned* Flags)
        ///
        /// Get environment Flags.
        /// 
        /// See also
        ///     mdbx_env_set_flags()
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create().</param>
        /// <param name="flags">The address of an integer to store the Flags.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_EINVAL	An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_get_flags")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_get_flags(IntPtr env,
            [Out, MarshalAs(UnmanagedType.U4)] out uint flags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_set_geometry")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_set_geometry(IntPtr env,
            IntPtr size_lower,
            IntPtr size_now,
            IntPtr size_upper,
            IntPtr growth_step,
            IntPtr shrink_threshold,
            IntPtr pagesize);

        /// <summary>
        /// Return the path that was used in mdbx_env_open().
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create()</param>
        /// <param name="dest">Address of a string pointer to contain the path. This is the actual string in the environment, not a Copy. It should not be altered in any way.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_EINVAL	An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_get_path")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern LibmdbxResultCodeFlag mdbx_env_get_path(IntPtr env,
            out IntPtr dest);

        /// <summary>
        /// Default Size of database page depends on the Size of the system page and usually exactly match it.
        /// </summary>
        /// <returns>Returns the default Size of database page for the current system.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_default_pagesize")]
        internal static extern IntPtr mdbx_default_pagesize();

        /// <summary>
        /// Returns maximal database Size in bytes for given page Size, or -1 if pageSize is invalid.
        /// </summary>
        /// <returns></returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_limits_dbsize_max")]
        internal static extern IntPtr mdbx_limits_dbsize_max(UIntPtr pagesize);

        /// <summary>
        /// Returns minimal database Size in bytes for given page Size, or -1 if pageSize is invalid.
        /// </summary>
        /// <returns></returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_limits_dbsize_min")]
        internal static extern IntPtr mdbx_limits_dbsize_min(UIntPtr pagesize);

        /// <summary>
        /// Delete the environment's files in a proper and multiprocess-safe way.
        /// </summary>
        /// <param name="pathname">The pathname for the database or the directory in which the database files reside.</param>
        /// <param name="mode">Special deletion Mode for the environment. This parameter must be set to one of the values described above in the MDBX_env_delete_mode_t section.</param>
        /// <returns>
        /// Note
        ///     The MDBX_ENV_JUST_DELETE don't supported on Windows since system unable to delete a memory-mapped files.
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_RESULT_TRUE	No corresponding files or directories were found, so no deletion was performed.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_delete")]
        internal static extern LibmdbxResultCodeFlag mdbx_env_delete(
            [In, MarshalAs(UnmanagedType.LPStr)] string pathname,
            [In, MarshalAs(UnmanagedType.I4)] RemoveMode mode);

        /// <summary>
        /// Copy an MDBX environment to the specified path, with options.
        /// This function may be used to make a backup of an existing environment. No lockfile is created, since it gets recreated at need.
        /// 
        /// Note
        ///     This call can trigger significant file Size growth if run in parallel with write transactions, because it employs a read-only transaction. See long-lived transactions under Restrictions &amp; Caveats section.
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create(). It must have already been opened successfully.</param>
        /// <param name="dest">The pathname of a file in which the Copy will reside. This file must not be already exist, but parent directory must be writable.</param>
        /// <param name="flags">Special options for this operation. This parameter must be set to 0 or by bitwise OR'ing together one or more of the values described here:
        ///                     MDBX_CP_COMPACT Perform compaction while copying: omit free pages and sequentially renumber all pages in output. This option consumes little bit more CPU for processing, but may running quickly than the default, on account skipping free pages.
        ///                     MDBX_CP_FORCE_DYNAMIC_SIZE Force to make resizeable Copy, i.e. dynamic Size instead of fixed.
        /// </param>
        /// <returns>A non-zero error value on failure and 0 on success.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_copy")]
        internal static extern LibmdbxResultCodeFlag mdbx_env_copy(IntPtr env,
            [In, MarshalAs(UnmanagedType.LPStr)] string dest,
            [In, MarshalAs(UnmanagedType.U4)] uint flags);


        /// <summary>
        /// Flush the environment data buffers to disk.
        /// 
        /// Unless the environment was opened with no-sync Flags (MDBX_NOMETASYNC, MDBX_SAFE_NOSYNC and MDBX_UTTERLY_NOSYNC), then data is always written an flushed to disk when mdbx_txn_commit() is called. Otherwise mdbx_env_sync() may be called to manually write and flush unsynced data to disk.
        /// 
        /// Besides, mdbx_env_sync_ex() with argument force=false may be used to provide polling Mode for lazy/asynchronous sync in conjunction with mdbx_env_set_syncbytes() and/or mdbx_env_set_syncperiod().
        /// 
        /// Note
        ///     This call is not valid if the environment was opened with MDBX_RDONLY.
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create()</param>
        /// <param name="force">If non-zero, force a flush. Otherwise, If force is zero, then will run in polling Mode, i.e. it will check the thresholds that were set mdbx_env_set_syncbytes() and/or mdbx_env_set_syncperiod()
        ///                     and perform flush if at least one of the thresholds is reached.</param>
        /// <param name="nonblock">Don't wait if write transaction is running by other thread.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and MDBX_RESULT_TRUE or 0 on success. The MDBX_RESULT_TRUE means no data pending for flush to disk, and 0 otherwise. Some possible errors are:
        /// Return values
        ///     MDBX_EACCES	the environment is read-only.
        ///     MDBX_BUSY	the environment is used by other thread and nonblock=true.
        ///     MDBX_EINVAL	an invalid parameter was specified.
        ///     MDBX_EIO	an error occurred during synchronization.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_env_sync_ex")]
        internal static extern LibmdbxResultCodeFlag mdbx_env_sync_ex(IntPtr env, 
            bool force, 
            bool nonblock);
    }

}
