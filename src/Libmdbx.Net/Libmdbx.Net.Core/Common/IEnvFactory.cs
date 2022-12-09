using System;
using Libmdbx.Net.Core.Env;

namespace Libmdbx.Net.Core.Common
{
    public interface IEnvFactory
    {
        IEnv Create(string path);
        IEnv Create(string path, CreateParameters createParameters, OperateParameters operateParameters);
        bool Remove(string pathname, RemoveMode mode = RemoveMode.JustRemove);
        IntPtr DefaultPageSize();
    }
}
