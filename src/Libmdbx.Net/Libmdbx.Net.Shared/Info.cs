using Libmdbx.Net.Bindings;
using Libmdbx.Net.Core.Transaction;
using static Libmdbx.Net.Core.Common.Const;

namespace Libmdbx.Net
{
    public struct Info
    {
        public DbFlags flags;
        public DbiState state;

        public KeyMode KeyMode => (KeyMode)((uint)flags & (MDBX_REVERSEKEY | MDBX_INTEGERKEY));
        public ValueMode ValueMode => (ValueMode)((uint)flags & (MDBX_DUPSORT | MDBX_REVERSEDUP | MDBX_DUPFIXED | MDBX_INTEGERDUP));

        public Info(DbFlags flags,
                    DbiState state)
        {
            this.flags = flags;
            this.state = state;
        }
    }
}