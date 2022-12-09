namespace Libmdbx.Net.Core.Cursor
{
    public struct CursorResult<TKey, TValue>
    {
        public CursorResult(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Initialize = true;
        }

        public TKey Key { get;  }

        public TValue Value { get; }

        public bool Initialize { get; }
    }
}
