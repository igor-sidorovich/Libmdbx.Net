using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Libmdbx.Net.Core.Common.BufferConverts
{
    public class IntBufferConverter : IBufferConverter<int>
    {
        public byte[] ConvertToBuffer(int t)
        {
            return BitConverter.GetBytes(t);
        }

        public int ConvertFromBuffer(byte[] buffer)
        {
            if (buffer == default || buffer.Length == 0)
            {
                return default;
            }

            return BitConverter.ToInt32(buffer, 0);
        }

        public int ConvertFromBuffer(ReadOnlySpan<byte> buffer)
        {
            if (buffer == default || buffer.Length == 0)
            {
                return default;
            }

            if (buffer.Length < sizeof(int))
                throw new ArgumentOutOfRangeException(nameof(buffer));

            return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(buffer));
        }
    }
}
