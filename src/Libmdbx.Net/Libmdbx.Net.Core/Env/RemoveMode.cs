using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net.Core.Env
{
    public enum RemoveMode
    {
        /// Just delete the environment's files and directory if any.
        /// On POSIX systems, processes already working with the database will
        /// continue to work without interference until it Close the environment.
        /// On Windows, the behavior of `JustRemove` is different
        /// because the system does not support deleting files that are currently
        /// memory mapped.
        JustRemove = MDBX_ENV_JUST_DELETE,

        /// Make sure that the environment is not being used by other
        /// processes, or return an error otherwise.
        EnsureUnused = MDBX_ENV_ENSURE_UNUSED,

        /// Wait until other processes closes the environment before
        /// deletion.
        WaitForUnused = MDBX_ENV_WAIT_FOR_UNUSED
    };
}