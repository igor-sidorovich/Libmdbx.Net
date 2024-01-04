using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Libmdbx.Net.Core.Common.BufferConverts
{
    public class UIntBufferConverter : IBufferConverter<uint>
    {
        public byte[] ConvertToBuffer(uint t)
        {
            return BitConverter.GetBytes(t);
        }

        public uint ConvertFromBuffer(byte[] buffer)
        {
            if (buffer == default || buffer.Length == 0)
            {
                return default;
            }

            return BitConverter.ToUInt32(buffer, 0);
        }

        public uint ConvertFromBuffer(ReadOnlySpan<byte> buffer)
        {
            if (buffer == default || buffer.Length == 0)
            {
                return default;
            }

            if (buffer.Length < sizeof(uint))
                throw new ArgumentOutOfRangeException(nameof(buffer));

            return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(buffer));
        }
    }
}
