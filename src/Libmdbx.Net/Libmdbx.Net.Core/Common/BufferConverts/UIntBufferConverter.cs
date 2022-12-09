using System;

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
    }
}
