using System;
using System.Text;

namespace Libmdbx.Net.Core.Common.BufferConverts
{
    public class StringBufferConverter : IBufferConverter<string>
    {
        public byte[] ConvertToBuffer(string t)
        {
            if (t == default)
            {
                return Array.Empty<byte>();
            }

            return Encoding.UTF8.GetBytes(t);
        }

        public string ConvertFromBuffer(byte[] buffer)
        {
            if (buffer == default || buffer.Length == 0)
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
