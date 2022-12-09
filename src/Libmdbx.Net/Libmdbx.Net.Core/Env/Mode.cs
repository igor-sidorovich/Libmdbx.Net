namespace Libmdbx.Net.Core.Env
{
    /// Operation Mode.
    public enum Mode
    {
        ReadonlyMode,   /// MDBX_RDONLY
        WriteFileIo,    /// don't available on OpenBSD
        WriteMappedIo   /// MDBX_WRITEMAP
    }
}