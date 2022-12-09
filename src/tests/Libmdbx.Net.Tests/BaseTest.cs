using System.IO;
using System.Reflection;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Shared;

namespace Libmdbx.Tests
{
    public class BaseTest
    {
        protected readonly string path;
        protected readonly IEnvFactory _envFactory;

        public BaseTest(string dbName)
        {
            _envFactory = new EnvFactory();
            path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "mdbx");
            if (Directory.Exists(path))
            {
                _envFactory.Remove(Path.Combine(path, dbName));
            }
            Directory.CreateDirectory(path);
            path = Path.Combine(path, dbName);
        }
    }
}
