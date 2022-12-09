using System;
using System.Runtime.InteropServices;
using System.Security;
using static Libmdbx.Net.Bindings.Const;

namespace Libmdbx.Net.Bindings
{
    internal static class MdbxCursor
    {
        /// <summary>
        /// MDBX_cursor* mdbx_cursor_create(void* context)
        /// 
        /// Create a cursor handle but not bind it to transaction nor DBI handle.
        ///
        /// An capable of operation cursor is associated with a specific transaction and database.
        /// A cursor cannot be used when its database handle is closed.Nor when its transaction has ended, except with mdbx_cursor_bind() and mdbx_cursor_renew().
        /// Also it can be discarded with mdbx_cursor_close().
        ///
        /// A cursor must be closed explicitly always, before or after its transaction ends.It can be reused with mdbx_cursor_bind() or mdbx_cursor_renew() before finally closing it.
        ///
        /// Note
        ///     In contrast to LMDB, the MDBX required that any opened cursors can be reused and must be freed explicitly, regardless ones was opened in a read-only or write transaction. The REASON for this is eliminates ambiguity which helps to avoid errors such as: use-after-free, double-free, i.e.memory corruption and segfaults.
        /// </summary>
        /// <param name="context">A pointer to application context to be associated with created cursor and could be retrieved by mdbx_cursor_get_userctx() until cursor closed.</param>
        /// <returns>Created cursor handle or NULL in case out of memory.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_create")]
        public static extern IntPtr mdbx_cursor_create(IntPtr context);

        /// <summary>
        /// int mdbx_cursor_eof(const MDBX_cursor* cursor)
        ///
        /// Determines whether the cursor is pointed to a key-value pair or not, i.e. was not positioned or points to the end of data.
        /// </summary>
        /// <param name="cursor">A cursor handle returned by mdbx_cursor_open().</param>
        /// <returns>
        /// Returns
        ///     A MDBX_RESULT_TRUE or MDBX_RESULT_FALSE value, otherwise the error code:
        /// Return values
        ///     MDBX_RESULT_TRUE No more data available or cursor not positioned
        ///     MDBX_RESULT_FALSE   A data is available
        ///     Otherwise   the error code
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_eof")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_cursor_eof(IntPtr cursor);

        /// <summary>
        /// void mdbx_cursor_close(MDBX_cursor* cursor)	
        /// 
        /// Close a cursor handle.
        ///
        /// The cursor handle will be freed and must not be used again after this call, but its transaction may still be live.
        ///
        /// Note
        ///     In contrast to LMDB, the MDBX required that any opened cursors can be reused and must be freed explicitly, regardless ones was opened in a read-only or write transaction.
        ///     The REASON for this is eliminates ambiguity which helps to avoid errors such as: use-after-free, double-free, i.e.memory corruption and segfaults.
        /// </summary>
        /// <param name="cursor">A cursor handle returned by mdbx_cursor_open() or mdbx_cursor_create().</param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_close")]
        public static extern void mdbx_cursor_close(IntPtr cursor);

        /// <summary>
        /// int mdbx_cursor_open(MDBX_txn* Txn, MDBX_dbi dbi, MDBX_cursor** cursor)
        /// 
        /// Create a cursor handle for the specified transaction and DBI handle.
        ///
        /// Using of the mdbx_cursor_open() is equivalent to calling mdbx_cursor_create() and then mdbx_cursor_bind() functions.
        ///
        /// An capable of operation cursor is associated with a specific transaction and database.
        /// A cursor cannot be used when its database handle is closed.Nor when its transaction has ended, except with mdbx_cursor_bind() and mdbx_cursor_renew().
        /// Also it can be discarded with mdbx_cursor_close().
        ///
        /// A cursor must be closed explicitly always, before or after its transaction ends.It can be reused with mdbx_cursor_bind() or mdbx_cursor_renew() before finally closing it.
        ///
        /// Note
        ///     In contrast to LMDB, the MDBX required that any opened cursors can be reused and must be freed explicitly, regardless ones was opened in a read-only or write transaction.
        ///     The REASON for this is eliminates ambiguity which helps to avoid errors such as: use-after-free, double-free, i.e.memory corruption and segfaults.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="cursor">Address where the new MDBX_cursor handle will be stored.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_open")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_cursor_open(IntPtr txn,
                                                  [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                                  out IntPtr cursor);

        /// <summary>
        /// int mdbx_cursor_get(MDBX_cursor* cursor, MDBX_val* key, MDBX_val* data, MDBX_cursor_op op)
        ///
        /// Retrieve by cursor.
        /// This function retrieves key/data pairs from the database.
        /// The address and length of the key are returned in the object to which key refers (except for the case of the MDBX_SET option, in which the key object is unchanged), and the address and length of the data are returned in the object to which data refers.	
        /// </summary>
        /// <param name="cursor">A cursor handle returned by mdbx_cursor_open().</param>
        /// <param name="key">The key for a retrieved item.</param>
        /// <param name="value">The data of a retrieved item.</param>
        /// <param name="op">A cursor operation MDBX_cursor_op.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_NOTFOUND No matching key found.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_get")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_cursor_get(IntPtr cursor,
                                                 ref MdbxDbVal key,
                                                 ref MdbxDbVal value,
                                                 [In, MarshalAs(UnmanagedType.U4)] CursorFlags op);

        /// <summary>
        /// int mdbx_cursor_put(MDBX_cursor* cursor, const MDBX_val* key, MDBX_val* data, MDBX_put_flags_t Flags)
        ///
        /// Store by cursor.
        ///
        /// This function stores key/data pairs into the database. The cursor is positioned at the new item, or on failure usually near it.
        ///
        /// Note
        ///    - MDBX allows(unlike LMDB) you to change the Size of the data and automatically handles reordering for sorted duplicates(see MDBX_DUPSORT).
        ///    - MDBX_NODUPDATA Enter the new key-value pair only if it does not already appear in the database.This flag may only be specified if the database was opened with MDBX_DUPSORT. The function will return MDBX_KEYEXIST if the key/data pair already appears in the database.
        ///    - MDBX_NOOVERWRITE Enter the new key/data pair only if the key does not already appear in the database.The function will return MDBX_KEYEXIST if the key already appears in the database, even if the database supports duplicates (MDBX_DUPSORT).
        ///    - MDBX_RESERVE Reserve space for data of the given Size, but don't Copy the given data. Instead, return a pointer to the reserved space, which the caller can fill in later - before the Next Update operation or the transaction ends. This saves an extra memcpy if the data is being generated later. This flag must not be specified if the database was opened with MDBX_DUPSORT.
        ///    - MDBX_APPEND Append the given key/data pair to the end of the database. No key comparisons are performed.This option allows fast bulk loading when keys are already known to be in the correct order.Loading unsorted keys with this flag will cause a MDBX_KEYEXIST error.
        ///    - MDBX_APPENDDUP As above, but for sorted dup data.
        ///    - MDBX_MULTIPLE Store multiple contiguous data elements in a Single request.This flag may only be specified if the database was opened with MDBX_DUPFIXED. With combination the MDBX_ALLDUPS will Replace all Multi-values.The data argument must be an array of two MDBX_val. The iov_len of the First MDBX_val must be the Size of a Single data element.The iov_base of the First MDBX_val must point to the beginning of the array of contiguous data elements which must be properly aligned in case of database with MDBX_INTEGERDUP flag.The iov_len of the second MDBX_val must be the count of the number of data elements to store. On return this field will be set to the count of the number of elements actually written. The iov_base of the second MDBX_val is unused.
        /// </summary>
        /// <param name="cursor">A cursor handle returned by mdbx_cursor_open().</param>
        /// <param name="key">he key operated on.</param>
        /// <param name="value">The data operated on.</param>
        /// <param name="flags">Options for this operation. This parameter must be set to 0 or by bitwise OR'ing together one or more of the values described here:
        /// MDBX_CURRENT Replace the item at the current cursor position.
        /// The key parameter must still be provided, and must match it, otherwise the function return MDBX_EKEYMISMATCH.
        /// With combination the MDBX_ALLDUPS will Replace all Multi-values.
        /// </param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_EKEYMISMATCH The given key value is mismatched to the current cursor position
        ///     MDBX_MAP_FULL The database is full, see mdbx_env_set_mapsize().
        ///     MDBX_TXN_FULL The transaction has too many dirty pages.
        ///     MDBX_EACCES An attempt was made to write in a read-only transaction.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_put")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_cursor_put(IntPtr cursor,
                                                 ref MdbxDbVal key,
                                                 ref MdbxDbVal value,
                                                 [In, MarshalAs(UnmanagedType.U4)] DatabasePutFlags flags);

        /// <summary>
        /// int mdbx_cursor_del(MDBX_cursor* cursor, MDBX_put_flags_t Flags)
        ///
        /// Delete current key/data pair.
        ///
        /// This function deletes the key/data pair to which the cursor refers. This does not invalidate the cursor, so operations such as MDBX_NEXT can still be used on it.
        /// Both MDBX_NEXT and MDBX_GET_CURRENT will return the same record after this operation.
        /// </summary>
        /// <param name="cursor">A cursor handle returned by mdbx_cursor_open().</param>
        /// <param name="flags">Options for this operation. This parameter must be set to one of the values described here.
        ///     - MDBX_CURRENT Delete only Single entry at current cursor position.
        ///     - MDBX_ALLDUPS or MDBX_NODUPDATA (supported for compatibility) Delete all of the data items for the current key. This flag has effect only for database(s) was created with MDBX_DUPSORT.
        /// </param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_MAP_FULL The database is full, see mdbx_env_set_mapsize().
        ///     MDBX_TXN_FULL The transaction has too many dirty pages.
        ///     MDBX_EACCES An attempt was made to write in a read-only transaction.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_del")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_cursor_del(IntPtr cursor,
                                                [In, MarshalAs(UnmanagedType.U4)] DatabasePutFlags flags);

        /// <summary>
        /// int mdbx_cursor_count(const MDBX_cursor* cursor, size_t* pcount)
        ///
        /// Return count of duplicates for current key.
        ///
        ///This call is valid for all databases, but reasonable only for that support sorted duplicate data items MDBX_DUPSORT.
        /// </summary>
        /// <param name="cursor">A cursor handle returned by mdbx_cursor_open().</param>
        /// <param name="pcount">Address where the count will be stored.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_EINVAL Cursor is not initialized, or an invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_count")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_cursor_count(IntPtr cursor,
                                                   out IntPtr pcount);

        /// <summary>
        /// int mdbx_cursor_renew(MDBX_txn* Txn, MDBX_cursor* cursor)
        ///
        /// Renew a cursor handle.
        ///
        /// An capable of operation cursor is associated with a specific transaction and database.
        /// The cursor may be associated with a new transaction, and referencing a new or the same database handle as it was created with.
        /// This may be done whether the Previous transaction is live or dead.
        ///
        /// Using of the mdbx_cursor_renew() is equivalent to calling mdbx_cursor_bind() with the DBI handle that previously the cursor was used with.
        ///
        ///
        /// Note
        ///   In contrast to LMDB, the MDBX allow any cursor to be re-used by using mdbx_cursor_renew(), to avoid unnecessary malloc/free overhead until it freed by mdbx_cursor_close().
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="cursor">A cursor handle returned by mdbx_cursor_open().</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_renew")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_cursor_renew(IntPtr txn,
                                                   IntPtr cursor);

        /// <summary>
        /// int mdbx_cursor_on_last	(const MDBX_cursor* cursor)	
        /// 
        /// Determines whether the cursor is pointed to the Last key-value pair or not.
        /// </summary>
        /// <param name="cursor">A cursor handle returned by mdbx_cursor_open().</param>
        /// <returns>
        /// Returns
        ///     A MDBX_RESULT_TRUE or MDBX_RESULT_FALSE value, otherwise the error code:
        /// Return values
        ///     MDBX_RESULT_TRUE Cursor positioned to the Last key-value pair
        ///     MDBX_RESULT_FALSE Cursor NOT positioned to the Last key-value pair
        ///     Otherwise the error code
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_on_last")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_cursor_on_last(IntPtr cursor);

        /// <summary>
        /// int mdbx_cursor_on_first(const MDBX_cursor* cursor)	
        /// 
        /// Determines whether the cursor is pointed to the First key-value pair or not.
        /// </summary>
        /// <param name="cursor">A cursor handle returned by mdbx_cursor_open().</param>
        /// <returns>
        /// Returns
        ///     A MDBX_RESULT_TRUE or MDBX_RESULT_FALSE value, otherwise the error code:
        /// Return values
        ///     MDBX_RESULT_TRUE Cursor positioned to the First key-value pair
        ///     MDBX_RESULT_FALSE Cursor NOT positioned to the First key-value pair
        ///     Otherwise the error code
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cursor_on_first")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_cursor_on_first(IntPtr cursor);
    }
}
