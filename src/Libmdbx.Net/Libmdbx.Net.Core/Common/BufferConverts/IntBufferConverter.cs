using System;

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
    }
}
