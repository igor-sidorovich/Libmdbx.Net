using System;
using System.Runtime.InteropServices;
using System.Security;
using static Libmdbx.Net.Bindings.Const;

namespace Libmdbx.Net.Bindings
{
    internal static class MdbxDb
    {
        /// <summary>
        /// int mdbx_dbi_close(MDBX_env* Env, MDBX_dbi dbi)
        /// 
        /// Close a database handle. Normally unnecessary.
        /// Closing a database handle is not necessary, but lets mdbx_dbi_open() reuse the handle value.
        /// Usually it's better to set a bigger mdbx_env_set_maxdbs(), unless that value would be large.
        ///
        /// Note
        ///     Use with care. This call is synchronized via mutex with mdbx_dbi_close(), but NOT with other transactions running by other threads.
        ///     The "Next" version of libmdbx (MithrilDB) will solve this issue.
        /// 
        /// Handles should only be closed if no other threads are going to reference the database handle or one of its cursors any further.
        /// Do not Close a handle if an existing transaction has modified its database.
        /// Doing so can cause misbehavior from database corruption to errors like MDBX_BAD_DBI (since the DB name is gone).
        /// </summary>
        /// <param name="env">An environment handle returned by mdbx_env_create().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <returns>A non-zero error value on failure and 0 on success.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_dbi_close")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_dbi_close(IntPtr env,
                                                                 [In, MarshalAs(UnmanagedType.U4)] uint dbi);

        /// <summary>
        /// int mdbx_dbi_open(MDBX_txn* Txn, const char* name, DbFlags Flags, MDBX_dbi* dbi)
        /// 
        /// Open or Create a database in the environment.
        /// A database handle denotes the name and parameters of a database, independently of whether such a database exists.
        /// The database handle may be discarded by calling mdbx_dbi_close().
        /// The old database handle is returned if the database was already open. The handle may only be closed once.
        ///
        /// Note
        ///     A notable difference between MDBX and LMDB is that MDBX make handles opened for existing databases immediately available for other transactions, regardless this transaction will be aborted or reset.
        ///     The REASON for this is to avoiding the requirement for multiple opening a same handles in concurrent read transactions, and tracking of such open but hidden handles until the completion of read transactions which opened them.
        ///
        /// Nevertheless, the handle for the NEWLY CREATED database will be invisible for other transactions until the this write transaction is successfully committed.
        /// If the write transaction is aborted the handle will be closed automatically. After a successful Commit the such handle will reside in the shared environment, and may be used by other transactions.
        /// In contrast to LMDB, the MDBX allow this function to be called from multiple concurrent transactions or threads in the same process.
        /// To use named database(with name != NULL), mdbx_env_set_maxdbs() must be called before opening the environment.
        /// Table names are keys in the internal unnamed database, and may be read but not written.
        /// </summary>
        /// <param name="txn">transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="name">The name of the database to open. If only a Single database is needed in the environment, this value may be NULL.</param>
        /// <param name="flags">Special options for this database. This parameter must be set to 0 or by bitwise OR'ing together one or more of the values described here:
        ///     MDBX_REVERSEKEY Keys are strings to be compared in Reverse order, from the end of the strings to the beginning. By default, Keys are treated as strings and compared from beginning to end.
        ///     MDBX_INTEGERKEY Keys are binary integers in native byte order, either uint32_t or uint64_t, and will be sorted as such.The keys must all be of the same Size and must be aligned while passing as arguments.
        ///     MDBX_DUPSORT Duplicate keys may be used in the database.Or, from another point of view, keys may have multiple data items, stored in sorted order. By default keys must be unique and may have only a Single data item.
        ///     MDBX_DUPFIXED This flag may only be used in combination with MDBX_DUPSORT.This option tells the library that the data items for this database are all the same Size, which allows further optimizations in storage and retrieval.When all data items are the same Size, the MDBX_GET_MULTIPLE, MDBX_NEXT_MULTIPLE and MDBX_PREV_MULTIPLE cursor operations may be used to retrieve multiple items at once.
        ///     MDBX_INTEGERDUP This option specifies that duplicate data items are binary integers, similar to MDBX_INTEGERKEY keys. The data values must all be of the same Size and must be aligned while passing as arguments.
        ///     MDBX_REVERSEDUP This option specifies that duplicate data items should be compared as strings in Reverse order (the comparison is performed in the direction from the Last byte to the First).
        ///     MDBX_CREATE Create the named database if it doesn't exist. This option is not allowed in a read-only transaction or a read-only environment.
        /// </param>
        /// <param name="dbi">Address where the new MDBX_dbi handle will be stored</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// 
        /// Return values
        ///     MDBX_NOTFOUND The specified database doesn't exist in the environment and MDBX_CREATE was not specified.
        ///     MDBX_DBS_FULL Too many databases have been opened.
        ///
        /// See also
        ///     mdbx_env_set_maxdbs()
        ///
        /// Return values
        ///     MDBX_INCOMPATIBLE MdbxDb is incompatible with given Flags, i.e.the passed Flags is different with which the database was created, or the database was already opened with a different comparison function(s).
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_dbi_open")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_dbi_open(IntPtr txn,
                                               [In, MarshalAs(UnmanagedType.LPStr)] string name,
                                               [In, MarshalAs(UnmanagedType.U4)] DatabaseOpenFlags flags,
                                               [Out, MarshalAs(UnmanagedType.U4)] out uint dbi);

        /// <summary>
        /// int mdbx_put(MDBX_txn* Txn, MDBX_dbi dbi, const MDBX_val* key, MDBX_val* data, MDBX_put_flags_t Flags)
        /// 
        /// Store items into a database.
        ///This function stores key/data pairs in the database.
        /// The default behavior is to enter the new key/data pair, replacing any previously existing key if duplicates are disallowed, or adding a duplicate data item if duplicates are allowed(see MDBX_DUPSORT).	
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="key">The key to store in the database.</param>
        /// <param name="value">The data to store.</param>
        /// <param name="flags">
        /// Special options for this operation. This parameter must be set to 0 or by bitwise OR'ing together one or more of the values described here:
        ///     - MDBX_NODUPDATA Enter the new key-value pair only if it does not already appear in the database. This flag may only be specified if the database was opened with MDBX_DUPSORT. The function will return MDBX_KEYEXIST if the key/data pair already appears in the database.
        ///     - MDBX_NOOVERWRITE Enter the new key/data pair only if the key does not already appear in the database. The function will return MDBX_KEYEXIST if the key already appears in the database, even if the database supports duplicates (see MDBX_DUPSORT). The data parameter will be set to point to the existing item.
        ///     - MDBX_CURRENT Update an Single existing entry, but not add new ones.The function will return MDBX_NOTFOUND if the given key not exist in the database.In case Multi-values for the given key, with combination of the MDBX_ALLDUPS will Replace all Multi-values, otherwise return the MDBX_EMULTIVAL.
        ///     - MDBX_RESERVE Reserve space for data of the given Size, but don't Copy the given data. Instead, return a pointer to the reserved space, which the caller can fill in later - before the Next Update operation or the transaction ends. This saves an extra memcpy if the data is being generated later. MDBX does nothing else with this memory, the caller is expected to modify all of the space requested. This flag must not be specified if the database was opened with MDBX_DUPSORT.
        ///     - MDBX_APPEND Append the given key/data pair to the end of the database. This option allows fast bulk loading when keys are already known to be in the correct order.Loading unsorted keys with this flag will cause a MDBX_EKEYMISMATCH error.
        ///     - MDBX_APPENDDUP As above, but for sorted dup data.
        ///     - MDBX_MULTIPLE Store multiple contiguous data elements in a Single request.This flag may only be specified if the database was opened with MDBX_DUPFIXED. With combination the MDBX_ALLDUPS will Replace all Multi-values.The data argument must be an array of two MDBX_val. The iov_len of the First MDBX_val must be the Size of a Single data element.The iov_base of the First MDBX_val must point to the beginning of the array of contiguous data elements which must be properly aligned in case of database with MDBX_INTEGERDUP flag.The iov_len of the second MDBX_val must be the count of the number of data elements to store. On return this field will be set to the count of the number of elements actually written. The iov_base of the second MDBX_val is unused.
        /// </param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_KEYEXIST The key/value pair already exists in the database.
        ///     MDBX_MAP_FULL The database is full, see mdbx_env_set_mapsize().
        ///     MDBX_TXN_FULL The transaction has too many dirty pages.
        ///     MDBX_EACCES An attempt was made to write in a read-only transaction.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_put")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_put(IntPtr txn,
                                          [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                          ref MdbxDbVal key, 
                                          ref MdbxDbVal value,
                                          [In, MarshalAs(UnmanagedType.U4)] DatabasePutFlags flags);


        /// <summary>
        /// int mdbx_get(MDBX_txn* Txn, MDBX_dbi dbi, const MDBX_val* key, MDBX_val* data)
        /// 
        /// Get items from a database.
        /// This function retrieves key/data pairs from the database.
        /// The address and length of the data associated with the specified key are returned in the structure to which data refers.
        /// If the database supports duplicate keys (MDBX_DUPSORT) then the First data item for the key will be returned.
        /// Retrieval of other items requires the use of mdbx_cursor_get().
        ///
        /// Note
        ///     The memory pointed to by the returned values is owned by the database.The caller need not dispose of the memory, and may not modify it in any way.
        ///     For values returned in a read-only transaction any modification attempts will cause a SIGSEGV.
        ///     Values returned from the database are valid only until a subsequent Update operation, or the end of the transaction.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="key">The key to search for in the database.</param>
        /// <param name="value">The data corresponding to the key.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_NOTFOUND The key was not in the database.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_get")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_get(IntPtr txn,
                                          [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                          ref MdbxDbVal key,
                                          ref MdbxDbVal value);

        /// <summary>
        /// int mdbx_get_ex(MDBX_txn* Txn, MDBX_dbi dbi, MDBX_val* key, MDBX_val* data, size_t* values_count)
        ///
        /// Get items from a database and optionally number of data items for a given key.
        /// Briefly this function does the same as mdbx_get() with a few differences:
        ///     If values_count is NOT NULL, then returns the count of Multi-values/duplicates for a given key.
        ///     Updates BOTH the key and the data for pointing to the actual key-value pair inside the database.	
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="key">The key to search for in the database.</param>
        /// <param name="value">The data corresponding to the key.</param>
        /// <param name="values_count">The optional address to return number of values associated with given key: = 0 - in case MDBX_NOTFOUND error; = 1 - exactly for databases WITHOUT MDBX_DUPSORT; >= 1 for databases WITH MDBX_DUPSORT.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_NOTFOUND The key was not in the database.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_get_ex")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_get_ex(IntPtr txn,
                                            [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                            ref MdbxDbVal key,
                                            ref MdbxDbVal value,
                                            ref UIntPtr values_count);

        /// <summary>
        /// int mdbx_replace(MDBX_txn* Txn, MDBX_dbi dbi, const MDBX_val* key, MDBX_val* new_data, MDBX_val* old_data, MDBX_put_flags_t Flags)
        /// 
        /// Replace items in a database.
        /// This function allows to Update or delete an existing value at the same time as the Previous value is retrieved.
        /// If the argument new_data equal is NULL zero, the removal is performed, otherwise the Update/Insert.
        /// The current value may be in an already changed(aka dirty) page.In this case, the page will be overwritten during the Update, and the old value will be lost.
        /// Therefore, an additional buffer must be passed via old_data argument initially to Copy the old value.
        /// If the buffer passed in is too small, the function will return MDBX_RESULT_TRUE by setting iov_len field pointed by old_data argument to the appropriate value, without performing any changes.
        /// For databases with non-unique keys (i.e.with MDBX_DUPSORT flag), another use case is also possible, when by old_data argument selects a specific item from Multi-value/duplicates with the same key for deletion or Update.
        /// To select this scenario in Flags should simultaneously specify MDBX_CURRENT and MDBX_NOOVERWRITE.
        /// This combination is chosen because it makes no sense, and thus allows you to identify the request of such a scenario.	
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="key">The key to store in the database.</param>
        /// <param name="new_value">The data to store, if NULL then deletion will be performed.</param>
        /// <param name="old_value">The buffer for retrieve Previous value as describe above.</param>
        /// <param name="flags">Special options for this operation. This parameter must be set to 0 or by bitwise OR'ing together one or more of the values described in mdbx_put() description above, and additionally (MDBX_CURRENT | MDBX_NOOVERWRITE) combination for selection particular item from Multi-value/duplicates.</param>
        /// <returns>A non-zero error value on failure and 0 on success.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_replace")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_replace(IntPtr txn,
                                                                [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                                                ref MdbxDbVal key,
                                                                ref MdbxDbVal new_value,
                                                                ref MdbxDbVal old_value,
                                                                [In, MarshalAs(UnmanagedType.U4)] DatabasePutFlags flags);

        /// <summary>
        /// int mdbx_get_equal_or_great(MDBX_txn* txnMDBX_dbi dbi, MDBX_val* key, MDBX_val* data)
        /// 
        /// Get equal or great item from a database.
        /// 
        /// Briefly this function does the same as mdbx_get() with a few differences:
        ///     - Return equal or great (due comparison function) key-value pair, but not only exactly matching with the key.
        ///     - On success return MDBX_SUCCESS if key found exactly, and MDBX_RESULT_TRUE otherwise. Moreover, for databases with MDBX_DUPSORT flag the data argument also will be used to match over Multi-value/duplicates, and MDBX_SUCCESS will be returned only when BOTH the key and the data match exactly.
        ///     - Updates BOTH the key and the data for pointing to the actual key-value pair inside the database.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="key">The key to search for in the database.</param>
        /// <param name="value">The data corresponding to the key.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and MDBX_RESULT_FALSE or MDBX_RESULT_TRUE on success(as described above). Some possible errors are:
        /// Return values
        ///     MDBX_THREAD_MISMATCH Given transaction is not owned by current thread.
        ///     MDBX_NOTFOUND The key was not in the database.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_get_equal_or_great")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_get_equal_or_great(IntPtr txn,
                                                        [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                                        ref MdbxDbVal key,
                                                        out MdbxDbVal value);

        /// <summary>
        /// int mdbx_drop(MDBX_txn* Txn, MDBX_dbi dbi, bool del)
        ///
        /// Empty or delete and Close a database.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="del">false to empty the DB, true to delete it from the environment and Close the DB handle.</param>
        /// <returns>A non-zero error value on failure and 0 on success.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_drop")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_drop(IntPtr txn,
                                          [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                          bool del);

        /// <summary>
        /// int mdbx_del(MDBX_txn* Txn, MDBX_dbi dbi, const MDBX_val* key, const MDBX_val* data)
        ///
        /// Delete items from a database.
        ///
        /// This function removes key/data pairs from the database.
        ///
        /// Note
        ///     The data parameter is NOT ignored regardless the database does support sorted duplicate data items or not.
        ///     If the data parameter is non-NULL only the matching data item will be deleted.Otherwise, if data parameter is NULL, any/all value(s) for specified key will be deleted.
        ///
        ///     This function will return MDBX_NOTFOUND if the specified key/data pair is not in the database.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="key">The key to delete from the database.</param>
        /// <param name="value">The data to delete.</param>
        /// <returns>
        /// Returns
        ///     A non-zero error value on failure and 0 on success, some possible errors are:
        /// Return values
        ///     MDBX_EACCES An attempt was made to write in a read-only transaction.
        ///     MDBX_EINVAL An invalid parameter was specified.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_del")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_del(IntPtr txn,
                                          [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                          ref MdbxDbVal key,
                                          IntPtr value);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_del")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_del(IntPtr txn,
                                          [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                          ref MdbxDbVal key,
                                          ref MdbxDbVal value);

        /// <summary>
        /// int mdbx_dcmp(const MDBX_txn* Txn, MDBX_dbi dbi, const MDBX_val* a, const MDBX_val* b)
        /// 
        /// Compare two data items according to a particular database.
        ///
        /// See also
        ///     MDBX_cmp_func
        /// 
        /// This returns a comparison as if the two items were data items of the specified database.
        ///
        /// Warning
        ///     There ss a Undefined behavior if one of arguments is invalid.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="a">The First item to compare.</param>
        /// <param name="b">The second item to compare.</param>
        /// <returns>< 0 if a < b; 0 if a == b; > 0 if a > b</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_dcmp")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int mdbx_dcmp(IntPtr txn,
                                          [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                          ref MdbxDbVal a,
                                          ref MdbxDbVal b);

        /// <summary>
        /// int mdbx_cmp(const MDBX_txn* Txn, MDBX_dbi dbi, const MDBX_val* a, const MDBX_val* b)
        ///
        /// Compare two keys according to a particular database.
        ///
        /// See also
        ///     MDBX_cmp_func
        /// 
        ///     This returns a comparison as if the two data items were keys in the specified database.
        ///
        /// Warning
        ///    There ss a Undefined behavior if one of arguments is invalid.
        /// </summary>
        /// <param name="txn">A transaction handle returned by mdbx_txn_begin().</param>
        /// <param name="dbi">A database handle returned by mdbx_dbi_open().</param>
        /// <param name="a">The First item to compare.</param>
        /// <param name="b">The second item to compare.</param>
        /// <returns>< 0 if a < b, 0 if a == b, > 0 if a > b</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_cmp")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int mdbx_cmp(IntPtr txn,
                                          [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                          ref MdbxDbVal a,
                                          ref MdbxDbVal b);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(LibMdbxName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mdbx_replace_ex")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern LibmdbxResultCodeFlag mdbx_replace_ex(IntPtr txn,
                                                                   [In, MarshalAs(UnmanagedType.U4)] uint dbi,
                                                                   ref MdbxDbVal key,
                                                                   ref MdbxDbVal new_data,
                                                                   ref MdbxDbVal old_data,
                                                                   [In, MarshalAs(UnmanagedType.U4)] DatabasePutFlags flags,
                                                                   IntPtr preserver,
                                                                   IntPtr preserver_context);
    }
}
