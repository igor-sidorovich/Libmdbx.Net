using System;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;

namespace Libmdbx.Net.Shared
{
    public class EnvFactory : IEnvFactory
    {
        private const uint DefaultMaxMaps = 100;
        private const uint DefaultMaxReaders = 100;
        private const long DefaultUpperSize = (long)Geometry.Size.MB * 10;

        public IEnv Create(string path)
        {
            var lowerSize = Geometry.MinimalValue;
            var upperSize = new IntPtr(DefaultUpperSize);
            var geometry = Geometry.make_dynamic(lowerSize, upperSize);
            var createParameters = new CreateParameters(geometry);

            var operateParameters = new OperateParameters(DefaultMaxMaps, DefaultMaxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

            return new Env(path, createParameters, operateParameters);
        }

        public IEnv Create(string path, CreateParameters createParameters, OperateParameters operateParameters)
        {
            return new Env(path, createParameters, operateParameters);
        }

        public bool Remove(string pathname, RemoveMode mode = RemoveMode.JustRemove)
        {
            return Env.Remove(pathname, mode);
        }

        public IntPtr DefaultPageSize()
        {
            return Env.DefaultPageSize();
        }
    }
}
