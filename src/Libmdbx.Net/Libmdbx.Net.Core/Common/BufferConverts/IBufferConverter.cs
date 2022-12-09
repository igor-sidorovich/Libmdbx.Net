﻿namespace Libmdbx.Net.Core.Common.BufferConverts
{
    public interface IBufferConverter<T>
    {
        byte[] ConvertToBuffer(T t);
        T ConvertFromBuffer(byte[] buffer);
    }
}