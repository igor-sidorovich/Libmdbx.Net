namespace Libmdbx.Net.Core.Common
{
    public static class Const
    {
        #region Env

        #region MDBX_copy_flags_t

        public const uint MDBX_CP_DEFAULTS = 0;

        /** Copy with compactification: Omit free space from Copy and renumber all
        * pages sequentially */
        public const uint MDBX_CP_COMPACT = MDBX_CP_DEFAULTS << 1;

        /** Force to make resizeable Copy, i.e. dynamic Size instead of fixed */
        public const uint MDBX_CP_FORCE_DYNAMIC_SIZE = MDBX_CP_COMPACT << 1;

        #endregion

        /** \brief Just delete the environment's files and directory if any.
        * \note On POSIX systems, processes already working with the database will
        * continue to work without interference until it Close the environment.
        * \note On Windows, the behavior of `MDB_ENV_JUST_DELETE` is different
        * because the system does not support deleting files that are currently
        * memory mapped. */
        public const int MDBX_ENV_JUST_DELETE = 0;

        /** \brief Make sure that the environment is not being used by other
        * processes, or return an error otherwise. */
        public const int MDBX_ENV_ENSURE_UNUSED = 1;

        /** brief Wait until other processes closes the environment before deletion.*/
        public const int MDBX_ENV_WAIT_FOR_UNUSED = 2;

        /** \brief Wait until other processes closes the environment before deletion.*/
        public const uint MDBX_ENV_DEFAULTS = 0;

        /** No environment directory.
        *
        * By default, MDBX creates its environment in a directory whose pathname is
        * given in path, and creates its data and lock files under that directory.
        * With this option, path is used as-is for the database main data file.
        * The database lock file is the path with "-lck" appended.
        *
        * - with `MDBX_NOSUBDIR` = in a filesystem we have the pair of MDBX-files
        *   which names derived from given pathname by appending predefined suffixes.
        *
        * - without `MDBX_NOSUBDIR` = in a filesystem we have the MDBX-directory with
        *   given pathname, within that a pair of MDBX-files with predefined names.
        *
        * This flag affects only at new environment creating by \ref mdbx_env_open(),
        * otherwise at opening an existing environment libmdbx will choice this
        * automatically. */
        public const uint MDBX_NOSUBDIR = 0x4000;

        /** Read only Mode.
         *
         * Open the environment in read-only Mode. No write operations will be
         * allowed. MDBX will still modify the lock file - except on read-only
         * filesystems, where MDBX does not use locks.
         *
         * - with `MDBX_RDONLY` = open environment in read-only Mode.
         *   MDBX supports pure read-only Mode (i.e. without opening LCK-file) only
         *   when environment directory and/or both files are not writable (and the
         *   LCK-file may be missing). In such case allowing file(s) to be placed
         *   on a network read-only share.
         *
         * - without `MDBX_RDONLY` = open environment in read-write Mode.
         *
         * This flag affects only at environment opening but can't be changed after.
         */
        public const uint MDBX_RDONLY = 0x20000;

        /** Open environment in exclusive/monopolistic Mode.
        *
        * `MDBX_EXCLUSIVE` flag can be used as a replacement for `MDB_NOLOCK`,
        * which don't supported by MDBX.
        * In this way, you can Get the minimal overhead, but with the correct
        * Multi-process and Multi-thread locking.
        *
        * - with `MDBX_EXCLUSIVE` = open environment in exclusive/monopolistic Mode
        *   or return \ref MDBX_BUSY if environment already used by other process.
        *   The main feature of the exclusive Mode is the ability to open the
        *   environment placed on a network share.
        *
        * - without `MDBX_EXCLUSIVE` = open environment in cooperative Mode,
        *   i.e. for Multi-process access/interaction/cooperation.
        *   The main requirements of the cooperative Mode are:
        *
        *   1. data files MUST be placed in the LOCAL file system,
        *      but NOT on a network share.
        *   2. environment MUST be opened only by LOCAL processes,
        *      but NOT over a network.
        *   3. OS kernel (i.e. file system and memory mapping implementation) and
        *      all processes that open the given environment MUST be running
        *      in the physically Single RAM with cache-coherency. The only
        *      exception for cache-consistency requirement is Linux on MIPS
        *      architecture, but this case has not been tested for a long time).
        *
        * This flag affects only at environment opening but can't be changed after.*/
        public const uint MDBX_EXCLUSIVE = 0x400000;

        /** Using database/environment which already opened by another process(es).
         *
         * The `MDBX_ACCEDE` flag is useful to avoid \ref MDBX_INCOMPATIBLE error
         * while opening the database/environment which is already used by another
         * process(es) with unknown Mode/Flags. In such cases, if there is a
         * difference in the specified Flags (\ref MDBX_NOMETASYNC,
         * \ref MDBX_SAFE_NOSYNC, \ref MDBX_UTTERLY_NOSYNC, \ref MDBX_LIFORECLAIM,
         * \ref MDBX_COALESCE and \ref MDBX_NORDAHEAD), instead of returning an error,
         * the database will be opened in a compatibility with the already used Mode.
         *
         * `MDBX_ACCEDE` has no effect if the current process is the only one either
         * opening the DB in read-only Mode or other process(es) uses the DB in
         * read-only Mode. */
        public const uint MDBX_ACCEDE = 0x40000000;

        /** Map data into memory with write permission.
         *
         * Use a writeable memory map unless \ref MDBX_RDONLY is set. This uses fewer
         * mallocs and requires much less work for tracking database pages, but
         * loses protection from application bugs like wild pointer writes and other
         * bad updates into the database. This may be slightly faster for DBs that
         * fit entirely in RAM, but is slower for DBs larger than RAM. Also adds the
         * possibility for stray application writes thru pointers to silently
         * corrupt the database.
         *
         * - with `MDBX_WRITEMAP` = all data will be mapped into memory in the
         *   read-write Mode. This offers a significant performance benefit, since the
         *   data will be modified directly in mapped memory and then flushed to disk
         *   by Single system call, without any memory management nor copying.
         *
         * - without `MDBX_WRITEMAP` = data will be mapped into memory in the
         *   read-only Mode. This requires stocking all modified database pages in
         *   memory and then writing them to disk through file operations.
         *
         * \warning On the other hand, `MDBX_WRITEMAP` adds the possibility for stray
         * application writes thru pointers to silently corrupt the database.
         *
         * \note The `MDBX_WRITEMAP` Mode is incompatible with nested transactions,
         * since this is unreasonable. I.e. nested transactions requires mallocation
         * of database pages and more work for tracking ones, which neuters a
         * performance boost caused by the `MDBX_WRITEMAP` Mode.
         *
         * This flag affects only at environment opening but can't be changed after.
         */
        public const uint MDBX_WRITEMAP = 0x80000;

        /** Tie reader locktable slots to read-only transactions
         * instead of to threads.
         *
         * Don't use Thread-Local Storage, instead tie reader locktable slots to
         * \ref MDBX_txn objects instead of to threads. So, \ref mdbx_txn_reset()
         * keeps the slot reserved for the \ref MDBX_txn object. A thread may use
         * parallel read-only transactions. And a read-only transaction may span
         * threads if you synchronizes its use.
         *
         * Applications that multiplex many user threads over individual OS threads
         * need this option. Such an application must also serialize the write
         * transactions in an OS thread, since MDBX's write locking is unaware of
         * the user threads.
         *
         * \note Regardless to `MDBX_NOTLS` flag a write transaction entirely should
         * always be used in one thread from start to finish. MDBX checks this in a
         * reasonable manner and return the \ref MDBX_THREAD_MISMATCH error in rules
         * violation.
         *
         * This flag affects only at environment opening but can't be changed after.
         */
        public const uint MDBX_NOTLS = 0x200000;

        /** Don't do readahead.
         *
         * Turn off readahead. Most operating systems perform readahead on read
         * requests by default. This option turns it off if the OS supports it.
         * Turning it off may help random read performance when the DB is larger
         * than RAM and system RAM is full.
         *
         * By default libmdbx dynamically enables/disables readahead depending on
         * the actual database Size and currently available memory. On the other
         * hand, such automation has some limitation, i.e. could be performed only
         * when DB Size changing but can't tracks and reacts changing a free RAM
         * availability, since it changes independently and asynchronously.
         *
         * \note The mdbx_is_readahead_reasonable() function allows to quickly find
         * out whether to use readahead or not based on the Size of the data and the
         * amount of available memory.
         *
         * This flag affects only at environment opening and can't be changed after.
         */
        public const uint MDBX_NORDAHEAD = 0x800000;

        /** Don't initialize malloc'ed memory before writing to datafile.
         *
         * Don't initialize malloc'ed memory before writing to unused spaces in the
         * data file. By default, memory for pages written to the data file is
         * obtained using malloc. While these pages may be reused in subsequent
         * transactions, freshly malloc'ed pages will be initialized to zeroes before
         * use. This avoids persisting leftover data from other code (that used the
         * heap and subsequently freed the memory) into the data file.
         *
         * Note that many other system libraries may allocate and free memory from
         * the heap for arbitrary uses. E.g., stdio may use the heap for file I/O
         * buffers. This initialization step has a modest performance cost so some
         * applications may want to disable it using this flag. This option can be a
         * problem for applications which handle sensitive data like passwords, and
         * it makes memory checkers like Valgrind noisy. This flag is not needed
         * with \ref MDBX_WRITEMAP, which writes directly to the mmap instead of using
         * malloc for pages. The initialization is also skipped if \ref MDBX_RESERVE
         * is used; the caller is expected to overwrite all of the memory that was
         * reserved in that case.
         *
         * This flag may be changed at any time using `mdbx_env_set_flags()`. */
        public const uint MDBX_NOMEMINIT = 0x1000000;

        /** Aims to coalesce a Garbage Collection items.
         *
         * With `MDBX_COALESCE` flag MDBX will aims to coalesce items while recycling
         * a Garbage Collection. Technically, when possible short lists of pages
         * will be combined into longer ones, but to fit on one database page. As a
         * result, there will be fewer items in Garbage Collection and a page lists
         * are longer, which slightly increases the likelihood of returning pages to
         * Unallocated space and reducing the database file.
         *
         * This flag may be changed at any time using mdbx_env_set_flags(). */
        public const uint MDBX_COALESCE = 0x2000000;

        /** LIFO policy for recycling a Garbage Collection items.
         *
         * `MDBX_LIFORECLAIM` flag turns on LIFO policy for recycling a Garbage
         * Collection items, instead of FIFO by default. On systems with a disk
         * write-back cache, this can significantly increase write performance, up
         * to several times in a best case scenario.
         *
         * LIFO recycling policy means that for reuse pages will be taken which became
         * unused the lastest (i.e. just now or most recently). Therefore the loop of
         * database pages circulation becomes as short as possible. In other words,
         * the number of pages, that are overwritten in memory and on disk during a
         * series of write transactions, will be as small as possible. Thus creates
         * ideal conditions for the efficient operation of the disk write-back cache.
         *
         * \ref MDBX_LIFORECLAIM is compatible with all no-sync Flags, but gives NO
         * noticeable impact in combination with \ref MDBX_SAFE_NOSYNC or
         * \ref MDBX_UTTERLY_NOSYNC. Because MDBX will reused pages only before the
         * Last "steady" MVCC-snapshot, i.e. the loop length of database pages
         * circulation will be mostly defined by frequency of calling
         * \ref mdbx_env_sync() rather than LIFO and FIFO difference.
         *
         * This flag may be changed at any time using mdbx_env_set_flags(). */
        public const uint MDBX_LIFORECLAIM = 0x4000000;

        /** Debugging option, fill/perturb released pages. */
        public const uint MDBX_PAGEPERTURB = 0x8000000;

        /* SYNC MODES****************************************************************/
        /** \defgroup sync_modes SYNC MODES
         *
         * \attention Using any combination of \ref MDBX_SAFE_NOSYNC, \ref
         * MDBX_NOMETASYNC and especially \ref MDBX_UTTERLY_NOSYNC is always a deal to
         * reduce Durability for gain write performance. You must know exactly what
         * you are doing and what risks you are taking!
         *
         * \note for LMDB users: \ref MDBX_SAFE_NOSYNC is NOT similar to LMDB_NOSYNC,
         * but \ref MDBX_UTTERLY_NOSYNC is exactly match LMDB_NOSYNC. See details
         * below.
         *
         * THE SCENE:
         * - The DAT-file contains several MVCC-snapshots of B-tree at same time,
         *   each of those B-tree has its own root page.
         * - Each of meta pages at the beginning of the DAT file contains a
         *   pointer to the root page of B-tree which is the result of the particular
         *   transaction, and a number of this transaction.
         * - For data Durability, MDBX must First write all MVCC-snapshot data
         *   pages and ensure that are written to the disk, then Update a meta page
         *   with the new transaction number and a pointer to the corresponding new
         *   root page, and flush any buffers yet again.
         * - Thus during Commit a I/O buffers should be flushed to the disk twice;
         *   i.e. fdatasync(), FlushFileBuffers() or similar syscall should be
         *   called twice for each Commit. This is very expensive for performance,
         *   but guaranteed Durability even on unexpected system failure or power
         *   outage. Of course, provided that the operating system and the
         *   underlying hardware (e.g. disk) work correctly.
         *
         * TRADE-OFF:
         * By skipping some stages described above, you can significantly benefit in
         * speed, while partially or completely losing in the guarantee of data
         * Durability and/or consistency in the event of system or power failure.
         * Moreover, if for any reason disk write order is not preserved, then at
         * moment of a system crash, a meta-page with a pointer to the new B-tree may
         * be written to disk, while the itself B-tree not yet. In that case, the
         * database will be corrupted!
         *
         * \see MDBX_SYNC_DURABLE \see MDBX_NOMETASYNC \see MDBX_SAFE_NOSYNC
         * \see MDBX_UTTERLY_NOSYNC
         *
         * @{ */

        /** Default robust and durable sync Mode.
         *
         * Metadata is written and flushed to disk after a data is written and
         * flushed, which guarantees the integrity of the database in the event
         * of a crash at any time.
         *
         * \attention Please do not use other modes until you have studied all the
         * details and are sure. Otherwise, you may lose your users' data, as happens
         * in [Miranda NG](https://www.miranda-ng.org/) messenger. */
        public const uint MDBX_SYNC_DURABLE = 0;

        /** Don't sync the meta-page after Commit.
         *
         * Flush system buffers to disk only once per transaction Commit, omit the
         * metadata flush. Defer that until the system flushes files to disk,
         * or Next non-\ref MDBX_RDONLY Commit or \ref mdbx_env_sync(). Depending on
         * the platform and hardware, with \ref MDBX_NOMETASYNC you may Get a doubling
         * of write performance.
         *
         * This trade-off maintains database integrity, but a system crash may
         * undo the Last committed transaction. I.e. it preserves the ACI
         * (atomicity, consistency, isolation) but not D (Durability) database
         * property.
         *
         * `MDBX_NOMETASYNC` flag may be changed at any time using
         * \ref mdbx_env_set_flags() or by passing to \ref mdbx_txn_begin() for
         * particular write transaction. \see sync_modes */
        public const uint MDBX_NOMETASYNC = 0x40000;

        /** Don't sync anything but keep Previous steady commits.
   *
   * Like \ref MDBX_UTTERLY_NOSYNC the `MDBX_SAFE_NOSYNC` flag disable similarly
   * flush system buffers to disk when committing a transaction. But there is a
   * huge difference in how are recycled the MVCC snapshots corresponding to
   * Previous "steady" transactions (see below).
   *
   * With \ref MDBX_WRITEMAP the `MDBX_SAFE_NOSYNC` instructs MDBX to use
   * asynchronous mmap-flushes to disk. Asynchronous mmap-flushes means that
   * actually all writes will scheduled and performed by operation system on it
   * own manner, i.e. unordered. MDBX itself just notify operating system that
   * it would be nice to write data to disk, but no more.
   *
   * Depending on the platform and hardware, with `MDBX_SAFE_NOSYNC` you may Get
   * a multiple increase of write performance, even 10 times or more.
   *
   * In contrast to \ref MDBX_UTTERLY_NOSYNC Mode, with `MDBX_SAFE_NOSYNC` flag
   * MDBX will keeps untouched pages within B-tree of the Last transaction
   * "steady" which was synced to disk completely. This has big implications for
   * both data Durability and (unfortunately) performance:
   *  - a system crash can't corrupt the database, but you will lose the Last
   *    transactions; because MDBX will rollback to Last steady Commit since it
   *    kept explicitly.
   *  - the Last steady transaction makes an effect similar to "long-lived" read
   *    transaction (see above in the \ref restrictions section) since prevents
   *    reuse of pages freed by newer write transactions, thus the any data
   *    changes will be placed in newly allocated pages.
   *  - to avoid rapid database growth, the system will sync data and issue
   *    a steady Commit-point to resume reuse pages, each time there is
   *    insufficient space and before increasing the Size of the file on disk.
   *
   * In other words, with `MDBX_SAFE_NOSYNC` flag MDBX insures you from the
   * whole database corruption, at the cost increasing database Size and/or
   * number of disk IOPs. So, `MDBX_SAFE_NOSYNC` flag could be used with
   * \ref mdbx_env_sync() as alternatively for batch committing or nested
   * transaction (in some cases). As well, auto-sync feature exposed by
   * \ref mdbx_env_set_syncbytes() and \ref mdbx_env_set_syncperiod() functions
   * could be very useful with `MDBX_SAFE_NOSYNC` flag.
   *
   * The number and volume of of disk IOPs with MDBX_SAFE_NOSYNC flag will
   * exactly the as without any no-sync Flags. However, you should expect a
   * larger process's [work set](https://bit.ly/2kA2tFX) and significantly worse
   * a [locality of reference](https://bit.ly/2mbYq2J), due to the more
   * intensive allocation of previously unused pages and increase the Size of
   * the database.
   *
   * `MDBX_SAFE_NOSYNC` flag may be changed at any time using
   * \ref mdbx_env_set_flags() or by passing to \ref mdbx_txn_begin() for
   * particular write transaction. */
        public const uint MDBX_SAFE_NOSYNC = 0x10000;

        /** \deprecated Please use \ref MDBX_SAFE_NOSYNC instead of `MDBX_MAPASYNC`.
         *
         * Since version 0.9.x the `MDBX_MAPASYNC` is deprecated and has the same
         * effect as \ref MDBX_SAFE_NOSYNC with \ref MDBX_WRITEMAP. This just API
         * simplification is for convenience and clarity. */
        public const uint MDBX_MAPASYNC = MDBX_SAFE_NOSYNC;

        /** Don't sync anything and wipe Previous steady commits.
   *
   * Don't flush system buffers to disk when committing a transaction. This
   * optimization means a system crash can corrupt the database, if buffers are
   * not yet flushed to disk. Depending on the platform and hardware, with
   * `MDBX_UTTERLY_NOSYNC` you may Get a multiple increase of write performance,
   * even 100 times or more.
   *
   * If the filesystem preserves write order (which is rare and never provided
   * unless explicitly noted) and the \ref MDBX_WRITEMAP and \ref
   * MDBX_LIFORECLAIM Flags are not used, then a system crash can't corrupt the
   * database, but you can lose the Last transactions, if at least one buffer is
   * not yet flushed to disk. The risk is governed by how often the system
   * flushes dirty buffers to disk and how often \ref mdbx_env_sync() is called.
   * So, transactions exhibit ACI (atomicity, consistency, isolation) properties
   * and only lose `D` (Durability). I.e. database integrity is maintained, but
   * a system crash may undo the final transactions.
   *
   * Otherwise, if the filesystem not preserves write order (which is
   * typically) or \ref MDBX_WRITEMAP or \ref MDBX_LIFORECLAIM Flags are used,
   * you should expect the corrupted database after a system crash.
   *
   * So, most important thing about `MDBX_UTTERLY_NOSYNC`:
   *  - a system crash immediately after Commit the write transaction
   *    high likely lead to database corruption.
   *  - successful completion of mdbx_env_sync(force = true) after one or
   *    more committed transactions guarantees consistency and Durability.
   *  - BUT by committing two or more transactions you back database into
   *    a weak state, in which a system crash may lead to database corruption!
   *    In case Single transaction after mdbx_env_sync, you may lose transaction
   *    itself, but not a whole database.
   *
   * Nevertheless, `MDBX_UTTERLY_NOSYNC` provides "weak" Durability in case
   * of an application crash (but no Durability on system failure), and
   * therefore may be very useful in scenarios where data Durability is
   * not required over a system failure (e.g for short-lived data), or if you
   * can take such risk.
   *
   * `MDBX_UTTERLY_NOSYNC` flag may be changed at any time using
   * \ref mdbx_env_set_flags(), but don't has effect if passed to
   * \ref mdbx_txn_begin() for particular write transaction. \see sync_modes */
        public const uint MDBX_UTTERLY_NOSYNC = MDBX_SAFE_NOSYNC | 0x100000;

        #endregion

        #region Errors

        /** Successful result */
        public const int MDBX_SUCCESS = 0;

        /** Alias for \ref MDBX_SUCCESS */
        public const int MDBX_RESULT_FALSE = MDBX_SUCCESS;

        /** Successful result with special meaning or a flag */
        public const int MDBX_RESULT_TRUE = -1;

        /** key/data pair already exists */
        public const int MDBX_KEYEXIST = -30799;

        /** The First LMDB-compatible defined error code */
        public const int MDBX_FIRST_LMDB_ERRCODE = MDBX_KEYEXIST;

        /** key/data pair not found (EOF) */
        public const int MDBX_NOTFOUND = -30798;

        /** Requested page not found - this usually indicates corruption */
        public const int MDBX_PAGE_NOTFOUND = -30797;

        /** MdbxDb is corrupted (page was wrong type and so on) */
        public const int MDBX_CORRUPTED = -30796;

        /** Env had fatal error,
        * i.e. Update of meta page failed and so on. */
        public const int MDBX_PANIC = -30795;

        /** DB file version mismatch with libmdbx */
        public const int MDBX_VERSION_MISMATCH = -30794;

        /** File is not a valid MDBX file */
        public const int MDBX_INVALID = -30793;

        /** Env mapsize reached */
        public const int MDBX_MAP_FULL = -30792;

        /** Env maxdbs reached */
        public const int MDBX_DBS_FULL = -30791;

        /** Env maxreaders reached */
        public const int MDBX_READERS_FULL = -30790;

        /** Tran has too many dirty pages, i.e transaction too big */
        public const int MDBX_TXN_FULL = -30788;

        /** Cursor stack too deep - this usually indicates corruption,
        * i.e branch-pages loop */
        public const int MDBX_CURSOR_FULL = -30787;

        /** Page has not enough space - internal error */
        public const int MDBX_PAGE_FULL = -30786;

        /** MdbxDb engine was unable to extend mapping, e.g. since address space
        * is unavailable or busy. This can mean:
        *  - MdbxDb Size extended by other process beyond to environment mapsize
        *    and engine was unable to extend mapping while starting read
        *    transaction. Env should be reopened to continue.
        *  - Engine was unable to extend mapping during write transaction
        *    or explicit call of \ref mdbx_env_set_geometry(). */
        public const int MDBX_UNABLE_EXTEND_MAPSIZE = -30785;

        /** Env or database is not compatible with the requested operation
        * or the specified Flags. This can mean:
        *  - The operation expects an \ref MDBX_DUPSORT / \ref MDBX_DUPFIXED
        *    database.
        *  - Opening a named DB when the unnamed DB has \ref MDBX_DUPSORT /
        *    \ref MDBX_INTEGERKEY.
        *  - Accessing a data record as a database, or vice versa.
        *  - The database was dropped and recreated with different Flags. */
        public const int MDBX_INCOMPATIBLE = -30784;

        /** Invalid reuse of reader locktable slot,
        * e.g. read-transaction already run for current thread */
        public const int MDBX_BAD_RSLOT = -30783;

        /** Tran is not valid for requested operation,
        * e.g. had errored and be must aborted, has a child, or is invalid */
        public const int MDBX_BAD_TXN = -30782;

        /** Invalid Size or alignment of key or data for target database,
        * either invalid subDB name */
        public const int MDBX_BAD_VALSIZE = -30781;

        /** The specified DBI-handle is invalid
        * or changed by another thread/transaction */
        public const int MDBX_BAD_DBI = -30780;

        /** Unexpected internal error, transaction should be aborted */
        public const int MDBX_PROBLEM = -30779;

        /** The Last LMDB-compatible defined error code */
        public const int MDBX_LAST_LMDB_ERRCODE = MDBX_PROBLEM;

        /** Another write transaction is running or environment is already used while
        * opening with \ref MDBX_EXCLUSIVE flag */
        public const int MDBX_BUSY = -30778;

        /** The First of MDBX-added error codes */
        public const int MDBX_FIRST_ADDED_ERRCODE = MDBX_BUSY;

        /** The specified key has more than one associated value */
        public const int MDBX_EMULTIVAL = -30421;

        /** Bad signature of a runtime object(s), this can mean:
        *  - memory corruption or double-free;
        *  - ABI version mismatch (rare case); */
        public const int MDBX_EBADSIGN = -30420;

        /** MdbxDb should be recovered, but this could NOT be done for now
        * since it opened in read-only Mode */
        public const int MDBX_WANNA_RECOVERY = -30419;

        /** The given key value is mismatched to the current cursor position */
        public const int MDBX_EKEYMISMATCH = -30418;

        /** MdbxDb is too large for current system,
        * e.g. could NOT be mapped into RAM. */
        public const int MDBX_TOO_LARGE = -30417;

        /** A thread has attempted to use a not owned object,
        * e.g. a transaction that started by another thread. */
        public const int MDBX_THREAD_MISMATCH = -30416;

        /** Overlapping read and write transactions for the current thread */
        public const int MDBX_TXN_OVERLAPPING = -30415;

        /* The Last of MDBX-added error codes */
        public const int MDBX_LAST_ADDED_ERRCODE = MDBX_TXN_OVERLAPPING;

        #endregion

        #region Transaction Flags

        /** Start read-write transaction.
        * Only one write transaction may be active at a time. Writes are fully
        * serialized, which guarantees that writers can never deadlock.
        */
        public const uint MDBX_TXN_READWRITE = 0;

        /** Start read-only transaction.
        * There can be multiple read-only transactions simultaneously that do not
        * block each other and a write transactions.
        */
        public const uint MDBX_TXN_RDONLY = MDBX_RDONLY;

        /** Prepare but not start read-only transaction.
        * Tran will not be started immediately, but created transaction handle
        * will be ready for use with \ref mdbx_txn_renew(). This flag allows to
        * preallocate memory and assign a reader slot, thus avoiding these operations
        * at the Next start of the transaction.
        */
        public const uint MDBX_TXN_RDONLY_PREPARE = MDBX_RDONLY | MDBX_NOMEMINIT;

        /** Do not block when starting a write transaction. */
        public const uint MDBX_TXN_TRY = 0x10000000;

        /** Exactly the same as \ref MDBX_NOMETASYNC,
        * but for this transaction only
        */
        public const uint MDBX_TXN_NOMETASYNC = MDBX_NOMETASYNC;

        /** Exactly the same as \ref MDBX_SAFE_NOSYNC,
        * but for this transaction only
        */
        public const uint MDBX_TXN_NOSYNC = MDBX_SAFE_NOSYNC;

        #endregion

        #region Database Open Flags

        public const uint MDBX_DB_DEFAULTS = 0;

        /** Use Reverse string keys */
        public const uint MDBX_REVERSEKEY = 0x02;

        /** Use sorted duplicates, i.e. allow Multi-values */
        public const uint MDBX_DUPSORT = 0x04;

        /** Numeric keys in native byte order either uint32_t or uint64_t. The keys
        * must all be of the same Size and must be aligned while passing as
        * arguments. */
        public const uint MDBX_INTEGERKEY = 0x08;

        /** With \ref MDBX_DUPSORT; sorted dup items have fixed Size */
        public const uint MDBX_DUPFIXED = 0x10;

        /** With \ref MDBX_DUPSORT and with \ref MDBX_DUPFIXED; dups are fixed Size
        * \ref MDBX_INTEGERKEY -style integers. The data values must all be of the
        * same Size and must be aligned while passing as arguments. */
        public const uint MDBX_INTEGERDUP = 0x20;

        /** With \ref MDBX_DUPSORT; use Reverse string comparison */
        public const uint MDBX_REVERSEDUP = 0x40;

        /** Create DB if not already existing */
        public const uint MDBX_CREATE = 0x40000;

        /** Opens an existing sub-database created with unknown Flags.
        *
        * The `MDBX_DB_ACCEDE` flag is intend to open a existing sub-database which
        * was created with unknown Flags (\ref MDBX_REVERSEKEY, \ref MDBX_DUPSORT,
        * \ref MDBX_INTEGERKEY, \ref MDBX_DUPFIXED, \ref MDBX_INTEGERDUP and
        * \ref MDBX_REVERSEDUP).
        *
        * In such cases, instead of returning the \ref MDBX_INCOMPATIBLE error, the
        * sub-database will be opened with Flags which it was created, and then an
        * application could determine the actual Flags by \ref mdbx_dbi_flags(). */
        public const uint MDBX_DB_ACCEDE = MDBX_ACCEDE;

        #endregion

        #region Put Flags

        /** Upsertion by default (without any other Flags) */
        public const uint MDBX_UPSERT = 0;

        /** For insertion: Don't write if the key already exists. */
        public const uint MDBX_NOOVERWRITE = 0x10;

        /** Has effect only for \ref MDBX_DUPSORT databases.
        * For upsertion: don't write if the key-value pair already exist.
        * For deletion: Remove all values for key. */
        public const uint MDBX_NODUPDATA = 0x20;

        /** For upsertion: overwrite the current key/data pair.
        * MDBX allows this flag for \ref mdbx_put() for explicit overwrite/Update
        * without insertion.
        * For deletion: Remove only Single entry at the current cursor position. */
        public const uint MDBX_CURRENT = 0x40;

        /** Has effect only for \ref MDBX_DUPSORT databases.
        * For deletion: Remove all Multi-values (aka duplicates) for given key.
        * For upsertion: Replace all Multi-values for given key with a new one. */
        public const uint MDBX_ALLDUPS = 0x80;

        /** For upsertion: Just reserve space for data, don't Copy it.
        * Return a pointer to the reserved space. */
        public const uint MDBX_RESERVE = 0x10000;

        /** Data is being appended.
        * Don't split full pages, continue on a new instead. */
        public const uint MDBX_APPEND = 0x20000;

        /** Has effect only for \ref MDBX_DUPSORT databases.
        * Duplicate data is being appended.
        * Don't split full pages, continue on a new instead. */
        public const uint MDBX_APPENDDUP = 0x40000;

        /** Only for \ref MDBX_DUPFIXED.
        * Store multiple data items in one call. */
        public const uint MDBX_MULTIPLE = 0x80000;

        #endregion

        #region Cursor

        /** Position at First key/data item */
        public const uint MDBX_FIRST = 0;

        /** \ref MDBX_DUPSORT -only: Position at First data item of current key. */
        public const uint MDBX_FIRST_DUP = 1;

        /** \ref MDBX_DUPSORT -only: Position at key/data pair. */
        public const uint MDBX_GET_BOTH = 2;

        /** \ref MDBX_DUPSORT -only: Position at given key and at First data greater
         * than or equal to specified data. */
        public const uint MDBX_GET_BOTH_RANGE = 3;

        /** Return key/data at current cursor position */
        public const uint MDBX_GET_CURRENT = 4;

        /** \ref MDBX_DUPFIXED -only: Return up to a page of duplicate data items
         * from current cursor position. Move cursor to prepare
         * for \ref MDBX_NEXT_MULTIPLE. */
        public const uint MDBX_GET_MULTIPLE = 5;

        /** Position at Last key/data item */
        public const uint MDBX_LAST = 6;

        /** \ref MDBX_DUPSORT -only: Position at Last data item of current key. */
        public const uint MDBX_LAST_DUP = 7;

        /** Position at Next data item */
        public const uint MDBX_NEXT = 8;

        /** \ref MDBX_DUPSORT -only: Position at Next data item of current key. */
        public const uint MDBX_NEXT_DUP = 9;

        /** \ref MDBX_DUPFIXED -only: Return up to a page of duplicate data items
         * from Next cursor position. Move cursor to prepare
         * for `MDBX_NEXT_MULTIPLE`. */
        public const uint MDBX_NEXT_MULTIPLE = 10;

        /** Position at First data item of Next key */
        public const uint MDBX_NEXT_NODUP = 11;

        /** Position at Previous data item */
        public const uint MDBX_PREV = 12;

        /** \ref MDBX_DUPSORT -only: Position at Previous data item of current key. */
        public const uint MDBX_PREV_DUP = 13;

        /** Position at Last data item of Previous key */
        public const uint MDBX_PREV_NODUP = 14;

        /** Position at specified key */
        public const uint MDBX_SET = 15;

        /** Position at specified key, return both key and data */
        public const uint MDBX_SET_KEY = 16;

        /** Position at First key greater than or equal to specified key. */
        public const uint MDBX_SET_RANGE = 17;

        /** \ref MDBX_DUPFIXED -only: Position at Previous page and return up to
         * a page of duplicate data items. */
        public const uint MDBX_PREV_MULTIPLE = 18;

        /** Position at First key-value pair greater than or equal to specified,
         * return both key and data, and the return code depends on a exact match.
         *
         * For non DUPSORT-ed collections this work the same to \ref MDBX_SET_RANGE,
         * but returns \ref MDBX_SUCCESS if key found exactly and
         * \ref MDBX_RESULT_TRUE if greater key was found.
         *
         * For DUPSORT-ed a data value is taken into account for duplicates,
         * i.e. for a pairs/tuples of a key and an each data value of duplicates.
         * Returns \ref MDBX_SUCCESS if key-value pair found exactly and
         * \ref MDBX_RESULT_TRUE if the Next pair was returned. */
        public const uint MDBX_SET_LOWERBOUND = 19;

        #endregion
    }
}
