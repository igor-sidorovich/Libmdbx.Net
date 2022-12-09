using System;
using System.IO;
using System.Reflection;
using Libmdbx.Net;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Libmdbx.Net.Shared;

namespace Libmdbx.NetFramework.Samples
{
    internal class Program
    {
        private static IEnvFactory _envFactory;

        static void Main(string[] args)
        {
            _envFactory = new EnvFactory();
            InsertExample();
            Console.WriteLine("Finished");
        }

        private static void InsertExample()
        {
            var path = GetPath();

            var lowerSize = Geometry.MinimalValue;
            var upperSize = new IntPtr((int)Geometry.Size.MB * 50);

            var geometry = Geometry.make_dynamic(lowerSize, upperSize);
            var createParameters = new CreateParameters(geometry);

            uint maxMaps = 100;
            uint maxReaders = 100;

            var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

            using (IEnv env = _envFactory.Create(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    for (int i = 0; i < 10; i++)
                    {
                        trx.Insert(map, i, i);
                    }

                    trx.Commit();
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    for (int i = 0; i < 10; i++)
                    {
                        var value = trx.Get<int, int>(map, i);
                    }

                    trx.Commit();
                }
            }
        }

        private static string GetPath()
        {
            
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Databases");
            if (Directory.Exists(path))
            {
                Env.Remove(Path.Combine(path, "mdbx_test.db"));
            }
            Directory.CreateDirectory(path);

            return Path.Combine(path, "mdbx_test.db");
        }
    }
}
