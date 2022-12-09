using System;
using System.Collections.Generic;

namespace Libmdbx.Net.Core.Common.BufferConverts
{
    public static class BufferConverterFactory
    {
        private static readonly Dictionary<Type, object> Converters = new Dictionary<Type, object>()
        {
            { typeof(string), new StringBufferConverter() },
            { typeof(int), new IntBufferConverter() },
            { typeof(long), new LongBufferConverter() },
            { typeof(uint), new UIntBufferConverter() }
        };


        public static void Register<T>(IBufferConverter<T> converter)
        {
            Converters[typeof(T)] = converter;
        }

        public static IBufferConverter<T> Get<T>()
        {
            Converters.TryGetValue(typeof(T), out var obj);
            IBufferConverter<T> bufferConverter = obj as IBufferConverter<T>;
            if (bufferConverter == null)
            {
                throw new KeyNotFoundException($"Unable to find converter of {typeof(T).Name}, please use `BufferConverter.Register` to register.");
            }
            return bufferConverter;
        }
    }
}
