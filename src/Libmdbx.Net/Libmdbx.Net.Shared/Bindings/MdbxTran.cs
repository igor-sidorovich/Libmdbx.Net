using System;
using System.Runtime.InteropServices;
using System.Security;
using Libmdbx.Net.Core.Transaction;
using static Libmdbx.Net.Bindings.Const;

namespace Libmdbx.Net.Bindings
{
    internal static class MdbxTran
    {
        /// <summary>
        /// int mdbx_txn_begin(MDBX_env* Env, MDBX_txn* parent, MDBX_txn_flags_t flagsT, MDBX_txn** Txn)
        /// 
        /// Create a transaction for use with the environment.
        /// The transaction handle may be discarded using mdbx_txn_abort() or mdbx_txn_commit().
        /// Note
        /// A transaction and its cursors must only be used by a Single thread, and a thread may only have a Single transaction at a time.
        /// If MDBX_NOTLS is in use, this does not apply to read-only transactions.
        /// Cursors may not span transactions.
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create().</param>
        /// <param name="parent">If this parameter is non-NULL, the new transaction will be a nested transaction, with the transaction indicated by parent as its parent. Transactions may be nested to any level.
        ///                      A parent transaction and its cursors may not issue any other operations than mdbx_txn_commit and mdbx_txn_abort() while it has active child transactions.
        /// </param>
        /// <param name="flagsT">Special options for this transaction. This parameter must be set to 0 or by bitwise OR'ing together one or more of the values described here:
        ///                     MDBX_RDONLY This transaction will not perform any write operations.
        ///                     MDBX_TXN_TRY Do not block when starting a write transaction.
        ///                     MDBX_SAFE_NOSYNC, MDBX_NOMETASYNC. Do not sync data to disk corresponding to MDBX_NOMETASYNC or MDBX_SAFE_NOSYNC description.
        /// </param>
        /// <param name="txn">Address where the new MDBX_txn handle will be stored.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_PANIC A fatal error occurred earlier and the environment must be shut down.
        ///     MDBX_UNABLE_EXTEND_MAPSIZE Another process wrote data beyond this MDBX_env's mapsize and this environment map must be resized as well. See mdbx_env_set_mapsize().
        ///     MDBX_READERS_FULL A read-only transaction was requested and the reader lock table is full.See mdbx_env_set_maxreaders().
        ///     MDBX_ENOMEM Out of memory.
        ///     MDBX_BUSY   The write transaction is already started by the current thread.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_begin")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_txn_begin(IntPtr env,
                                                                  IntPtr parent, 
                                                                  [In, MarshalAs(UnmanagedType.U4)] TxnFlags flags, 
                                                                  out IntPtr txn);

        /// <summary>
        /// int mdbx_txn_begin(MDBX_env* Env, MDBX_txn* parent, MDBX_txn_flags_t flagsT, MDBX_txn** Txn, void* 	context)	
        /// </summary>
        /// <param name="context">A pointer to application context to be associated with created transaction and could be retrieved by mdbx_txn_get_userctx() until transaction finished.</param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_begin_ex")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_txn_begin_ex(IntPtr env,
                                                                     IntPtr parent,
                                                                     [In, MarshalAs(UnmanagedType.U4)] TxnFlags flags,
                                                                     out IntPtr txn,
                                                                     IntPtr context);

        /// <summary>
        /// int mdbx_txn_abort(MDBX_txn* Txn)
        /// 
        /// Abandon all the operations of the transaction instead of saving them.
        /// The transaction handle is freed.It and its cursors must not be used again after this call, except with mdbx_cursor_renew() and mdbx_cursor_close().
        /// If the current thread is not eligible to manage the transaction then the MDBX_THREAD_MISMATCH error will returned.
        /// Otherwise the transaction will be aborted and its handle is freed.Thus, a result other than MDBX_THREAD_MISMATCH means that the transaction is terminated:
        ///     - Resources are released;
        ///     - Tran handle is invalid;
        ///     - Cursor(s) associated with transaction must not be used, except with mdbx_cursor_renew() and mdbx_cursor_close(). Such cursor(s) must be closed explicitly by mdbx_cursor_close() before or after transaction Abort, either can be reused with mdbx_cursor_renew() until it will be explicitly closed by mdbx_cursor_close().
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin()</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_PANIC A fatal error occurred earlier and the environment must be shut down.
        ///     MDBX_BAD_TXN Tran is already finished or never began.
        ///     MDBX_EBADSIGN Tran object has invalid signature, e.g.transaction was already terminated or memory was corrupted.
        ///     MDBX_THREAD_MISMATCH    Given transaction is not owned by current thread.
        ///     MDBX_EINVAL Tran handle is NULL
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_abort")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_txn_abort(IntPtr txn);

        /// <summary>
        /// int mdbx_txn_break(MDBX_txn* Txn)
        /// 
        /// Marks transaction as broken.
        /// Function keeps the transaction handle and corresponding locks, but makes impossible to perform any operations within a broken transaction.
        /// Broken transaction must then be aborted explicitly later.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin()</param>
        /// <returns>A non-zero error value on failure and 0 on success</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_break")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_txn_break(IntPtr txn);

        /// <summary>
        /// int mdbx_txn_commit(MDBX_txn* Txn)
        /// 
        /// Commit all the operations of a transaction into the database.
        /// If the current thread is not eligible to manage the transaction then the MDBX_THREAD_MISMATCH error will returned.
        /// Otherwise the transaction will be committed and its handle is freed.If the transaction cannot be committed, it will be aborted with the corresponding error returned.
        /// Thus, a result other than MDBX_THREAD_MISMATCH means that the transaction is terminated:
        ///     - Resources are released;
        ///     - Tran handle is invalid;
        ///     - Cursor(s) associated with transaction must not be used, except with mdbx_cursor_renew() and mdbx_cursor_close(). Such cursor(s) must be closed explicitly by mdbx_cursor_close() before or after transaction Commit, either can be reused with mdbx_cursor_renew() until it will be explicitly closed by mdbx_cursor_close().
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_RESULT_TRUE Tran was aborted since it should be aborted due to Previous errors.
        ///     MDBX_PANIC A fatal error occurred earlier and the environment must be shut down.
        ///     MDBX_BAD_TXN Tran is already finished or never began.
        ///     MDBX_EBADSIGN Tran object has invalid signature, e.g.transaction was already terminated or memory was corrupted.
        ///     MDBX_THREAD_MISMATCH    Given transaction is not owned by current thread.
        ///     MDBX_EINVAL Tran handle is NULL.
        ///     MDBX_ENOSPC No more disk space.
        ///     MDBX_EIO A system-level I/O error occurred.
        ///     MDBX_ENOMEM Out of memory.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_commit")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_txn_commit(IntPtr txn);

        /// <summary>
        /// void* mdbx_txn_get_userctx (const MDBX_txn* Txn)
        /// 
        /// Returns an application information (a context pointer) associated with the transaction.
        /// </summary>
        /// <param name="txn">An transaction handle returned by mdbx_txn_begin_ex() or mdbx_txn_begin()</param>
        /// <returns>The pointer which was passed via the context parameter of mdbx_txn_begin_ex() or set by mdbx_txn_set_userctx(), or NULL if something wrong.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_get_userctx")]
        public static extern IntPtr mdbx_txn_get_userctx(IntPtr txn);

        /// <summary>
        /// MDBX_env* mdbx_txn_env(const MDBX_txn* Txn)
        /// 
        /// Returns the transaction's MDBX_env.	
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin()</param>
        /// <returns>Returns the transaction's MDBX_env.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_env")]
        public static extern IntPtr mdbx_txn_env(IntPtr txn);

        /// <summary>
        /// int mdbx_txn_flags(const MDBX_txn* Txn)
        /// 
        /// Return the transaction's flagsT.
        /// This returns the flagsT associated with this transaction.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <returns>A transaction flagsT, valid if input is an valid transaction, otherwise -1.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_flags")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int mdbx_txn_flags(IntPtr txn);

        /// <summary>
        /// int mdbx_txn_renew(MDBX_txn* Txn)
        /// 
        /// Renew a read-only transaction.
        /// This acquires a new reader lock for a transaction handle that had been released by mdbx_txn_reset(). It must be called before a reset transaction may be used again.	
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_PANIC A fatal error occurred earlier and the environment must be shut down.
        ///     MDBX_BAD_TXN Tran is already finished or never began.
        ///     MDBX_EBADSIGN Tran object has invalid signature, e.g.transaction was already terminated or memory was corrupted.
        ///     MDBX_THREAD_MISMATCH    Given transaction is not owned by current thread.
        ///     MDBX_EINVAL Tran handle is NULL.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_renew")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_txn_renew(IntPtr txn);

        /// <summary>
        /// int mdbx_txn_reset(MDBX_txn* Txn)
        /// 
        /// Reset a read-only transaction.
        /// Abort the read-only transaction like mdbx_txn_abort(), but keep the transaction handle.
        /// Therefore mdbx_txn_renew() may reuse the handle.This saves allocation overhead if the process will start a new read-only transaction soon, and also locking overhead if MDBX_NOTLS is in use.
        /// The reader table lock is released, but the table slot stays tied to its thread or MDBX_txn.Use mdbx_txn_abort() to discard a reset handle, and to free its lock table slot if MDBX_NOTLS is in use.
        /// Cursors opened within the transaction must not be used again after this call, except with mdbx_cursor_renew() and mdbx_cursor_close().
        /// Reader locks generally don't interfere with writers, but they keep old versions of database pages allocated.
        /// Thus they prevent the old pages from being reused when writers Commit new data, and so under heavy load the database Size may grow much more rapidly than otherwise.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        ///Return values
        ///     MDBX_PANIC A fatal error occurred earlier and the environment must be shut down.
        ///     MDBX_BAD_TXN Tran is already finished or never began.
        ///     MDBX_EBADSIGN Tran object has invalid signature, e.g.transaction was already terminated or memory was corrupted.
        ///     MDBX_THREAD_MISMATCH    Given transaction is not owned by current thread.
        ///     MDBX_EINVAL Tran handle is NULL.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_reset")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_txn_reset(IntPtr txn);

        /// <summary>
        /// int mdbx_txn_set_userctx(MDBX_txn* Txn, void* ctx)
        /// 
        /// Sets application information associated (a context pointer) with the transaction.
        /// </summary>
        /// <param name="txn">An transaction handle returned by mdbx_txn_begin_ex() or mdbx_txn_begin().</param>
        /// <param name="ctx">An arbitrary pointer for whatever the application needs.</param>
        /// <returns>A non-zero error value on failure and 0 on success.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_set_userctx")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_txn_set_userctx(IntPtr txn,
                                                                        IntPtr ctx);

        /// <summary>
        /// uint64_t mdbx_txn_id(const MDBX_txn* Txn)	
        /// 
        /// Return the transaction's ID.
        /// 
        /// This returns the identifier associated with this transaction.
        /// For a read-only transaction, this corresponds to the snapshot being read; concurrent readers will frequently have the same transaction ID.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <returns>A transaction ID, valid if input is an active transaction, otherwise 0.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_id")]
        [return: MarshalAs(UnmanagedType.U8)]
        public static extern ulong mdbx_txn_id(IntPtr txn);

        /// <summary>
        /// Determines whether the given address is on a dirty database page of the transaction or not.
        ///
        /// Ultimately, this allows to avoid Copy data from non-dirty pages.
        ///
        /// "Dirty" pages are those that have already been changed during a write transaction.
        /// Accordingly, any further changes may result in such pages being overwritten.
        /// Therefore, all functions libmdbx performing changes inside the database as arguments should NOT Get pointers to data in those pages.
        /// In turn, "not dirty" pages before modification will be copied.
        ///
        /// In other words, data from dirty pages must either be copied before being passed as arguments for further processing or rejected at the argument validation stage.
        /// Thus, mdbx_is_dirty() allows you to Get rid of unnecessary copying, and perform a more complete check of the arguments.
        ///
        /// Note
        /// The address passed must point to the beginning of the data.
        /// This is the only way to ensure that the actual page header is physically located in the same memory page, including for Multi-pages with long data.
        /// In rare cases the function may return a false positive answer (MDBX_RESULT_TRUE when data is NOT on a dirty page), but never a false negative if the arguments are correct.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="ptr">The address of data to check.</param>
        /// <returns>
        /// Returns
        ///     A MDBX_RESULT_TRUE or MDBX_RESULT_FALSE value, otherwise the error code:
        /// 
        /// Return values
        ///     MDBX_RESULT_TRUE	Given address is on the dirty page.
        ///     MDBX_RESULT_FALSE	Given address is NOT on the dirty page.
        ///     Otherwise	the error code.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_is_dirty")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_is_dirty(IntPtr txn,
                                                                 IntPtr ptr);

        /// <summary>
        /// int mdbx_txn_info(const MDBX_txn* Txn, MDBX_txn_info* Info, bool scan_rlt)	
        /// 
        /// Return information about the MDBX transaction.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin()</param>
        /// <param name="info">The address of an MDBX_txn_info structure where the information will be copied.</param>
        /// <param name="scan_rlt">The boolean flag controls the scan of the read lock table to provide complete information.
        ///                        Such scan is relatively expensive and you can avoid it if corresponding fields are not needed. See description of MDBX_txn_info.
        /// </param>
        /// <returns>A non-zero error value on failure and 0 on success.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_txn_info")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_txn_info(IntPtr txn,
                                                                 ref MdbxTxnInfo info,
                                                                 [In, MarshalAs(UnmanagedType.I4)] bool scan_rlt);
    }
}
