using System;

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
    }
}
