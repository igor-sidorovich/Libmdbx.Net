namespace Libmdbx.Net.Core.Common
{
    public struct MapHandle
    {
        public uint dbi;

        public MapHandle(uint dbi)
        {
            this.dbi = dbi;
        }
    }
}