using System;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Cursor;

namespace Libmdbx.Net.Core.Transaction
{
    public interface ITxn : IDisposable
    {
        IntPtr Env { get; }
        IntPtr TxnPtr { get; }
        bool Completed { get; }
        TxnFlags Flags { get;}
        ulong Id { get; }
        bool IsReadOnly { get; }
        bool IsReadWrite { get; }
        ulong SizeCurrent { get; }
        void Abort();
        void Commit();
        bool IsDirty(IntPtr ptr);
        MdbxTxnInfo GetInfo(bool scanReaderLockTable = false);
        MapHandle CreateMap(string name, KeyMode keyMode, ValueMode valueMode);
        MapHandle OpenMap(string name, KeyMode keyMode, ValueMode valueMode);
        void DropMap(MapHandle map);
        bool DropMap(string name, bool throwIfAbsent = false);
        void ClearMap(MapHandle map);
        bool ClearMap(string name, bool throwIfAbsent = false);
        ICursor OpenCursor(MapHandle map);
        TV Get<TK, TV>(MapHandle map, TK key);
        TV Get<TK, TV>(MapHandle map, TK key, out uint valuesCount);
        bool TryGet<TK, TV>(MapHandle map, TK key, out TV value);
        void Insert<TK, TV>(MapHandle map, TK key, TV value);
        bool TryInsert<TK, TV>(MapHandle map, TK key, TV value);
        void Upsert<TK, TV>(MapHandle map, TK key, TV value);
        void Update<TK, TV>(MapHandle map, TK key, TV value);
        bool TryUpdate<TK, TV>(MapHandle map, TK key, TV value);
        bool Erase<TK, TV>(MapHandle map, TK key, TV value);
        bool Erase<TK>(MapHandle map, TK key);
        void Replace<TK, TV>(MapHandle map, TK key, TV oldValue, TV newValue, bool multiple = false);
    }
}
