using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Libmdbx.Net.Core.Common.BufferConverts
{
    public class LongBufferConverter : IBufferConverter<long>
    {
        public byte[] ConvertToBuffer(long t)
        {
            return BitConverter.GetBytes(t);
        }

        public long ConvertFromBuffer(byte[] buffer)
        {
            if (buffer == default || buffer.Length == 0)
            {
                return default;
            }

            return BitConverter.ToInt64(buffer, 0);
        }

        public long ConvertFromBuffer(ReadOnlySpan<byte> buffer)
        {
            if (buffer == default || buffer.Length == 0)
            {
                return default;
            }

            if (buffer.Length < sizeof(long))
                throw new ArgumentOutOfRangeException(nameof(buffer));

            return Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference(buffer));
        }
    }
}
