using System;
using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net.Core.Env
{
    [Flags]
    public enum EnvFlags : uint
    {
        Defaults = MDBX_ENV_DEFAULTS,

        //No environment directory.
        NoSubDir = MDBX_NOSUBDIR,

        //Read only Mode.
        ReadOnly = MDBX_RDONLY,

        //Open environment in exclusive/monopolistic Mode.
        Exclusive = MDBX_EXCLUSIVE,

        //Using database/environment which already opened by another process(es).
        Accede = MDBX_ACCEDE,

        //Map data into memory with write permission.
        WriteMap = MDBX_WRITEMAP,

        //Tie reader locktable slots to read-only transactions instead of to threads.
        NoTls = MDBX_NOTLS,

        //Don't do readahead.
        NordAhead = MDBX_NORDAHEAD,

        //Don't initialize malloc'ed memory before writing to datafile.
        NoMemInit = MDBX_NOMEMINIT,

        //Aims to coalesce a Garbage Collection items.
        Coalesce = MDBX_COALESCE,

        //LIFO policy for recycling a Garbage Collection items.
        LifoReclaim = MDBX_LIFORECLAIM,

        //Debugging option, fill/perturb released pages. */
        PagePerTurb = MDBX_PAGEPERTURB,

        //Default robust and durable sync Mode.
        SyncDurable = MDBX_SYNC_DURABLE,

        //Don't sync the meta-page after Commit.
        NoMetaSync = MDBX_NOMETASYNC,

        //Don't sync anything but keep Previous steady commits.
        SafeNoSync = MDBX_SAFE_NOSYNC,

        //Don't sync anything and wipe Previous steady commits.
        UtterlyNoSync = MDBX_UTTERLY_NOSYNC
    }
}
