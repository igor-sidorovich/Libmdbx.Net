using System.Diagnostics;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Libmdbx.Net.Shared;

BenchmarkRunner.Run<LibMdbxBenchmarking>();

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByMethod)]
[SimpleJob(RunStrategy.Monitoring, iterationCount: 10, invocationCount: 1)]
public class LibMdbxBenchmarking
{
    private string dbFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "mdbx");
    private string dbFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "mdbx", "mdbx-test.db");
    private string mapName = "test";
    readonly EnvFactory _envFactory = new EnvFactory();
    private string key = "builtin/features/popups/shared/duplicate_collection_item_convert/test/{0}/key";

    #region Benchmark

    [Benchmark]
    [Arguments(1000)]
    [Arguments(2000)]
    [Arguments(3000)]
    [Arguments(4000)]
    [Arguments(5000)]
    public void LibMdbx_Insert_ValuesCount_DbSize_Dynamic(int valuesCount)
    {
        var lowerSize = Geometry.MinimalValue;
        var upperSize = new IntPtr((int)Geometry.Size.MB * 5);

        var geometry = Geometry.make_dynamic(lowerSize, upperSize);
        var createParameters = new CreateParameters(geometry);

        uint maxMaps = 1;
        uint maxReaders = 1;

        var operateParameters = new OperateParameters(maxMaps, maxReaders,
            reclaiming: new ReclaimingOptions(lifo: true),
            options: new OperateOptions(orphanReadTransactions: true, exclusive: true));

        using (IEnv env = _envFactory.Create(dbFilePath, createParameters, operateParameters))
        {
            using (ITxn trx = env.StartWrite())
            {
                var map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

                for (int i = 0; i < valuesCount; i++)
                {
                    trx.Insert(map, string.Format(key, i), i);
                }

                trx.Commit();

                //Close a database handle. Normally unnecessary.
                //Closing a database handle is not necessary, but lets mdbx_dbi_open() reuse the handle value. Usually it's better to set a bigger mdbx_env_set_maxdbs(), unless that value would be large.
                //env.CloseMap(map);
            }
        }
    }

    [Benchmark]
    [Arguments(1000, 5)]
    [Arguments(5000, 5)]
    [Arguments(10000, 5)]
    [Arguments(15000, 15)]
    [Arguments(20000, 15)]
    [Arguments(25000, 15)]
    public void LibMdbx_Insert_ValuesCount_DbSize_DbSizeValue(int valuesCount, long dbSizeValue)
    {
        var fixedSize = new IntPtr((int)Geometry.Size.MB * dbSizeValue);

        var geometry = Geometry.make_fixed(fixedSize);
        var createParameters = new CreateParameters(geometry);

        uint maxMaps = 1;
        uint maxReaders = 1;

        var operateParameters = new OperateParameters(maxMaps, maxReaders,
            reclaiming: new ReclaimingOptions(lifo: true),
            options: new OperateOptions(orphanReadTransactions: true, exclusive: true));

        using (IEnv env = _envFactory.Create(dbFilePath, createParameters, operateParameters))
        {
            using (ITxn trx = env.StartWrite())
            {
                var map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

                for (int i = 0; i < valuesCount; i++)
                {
                    trx.Insert(map, string.Format(key, i), i);
                }

                trx.Commit();

                //Close a database handle. Normally unnecessary.
                //Closing a database handle is not necessary, but lets mdbx_dbi_open() reuse the handle value. Usually it's better to set a bigger mdbx_env_set_maxdbs(), unless that value would be large.
                //env.CloseMap(map);
            }
        }
    }

    [Benchmark]
    [Arguments(1000)]
    [Arguments(2000)]
    [Arguments(3000)]
    [Arguments(4000)]
    [Arguments(5000)]
    public void LibMdbx_Insert_Read_ValuesCount_DbSize_Dynamic(int valuesCount)
    {
        var lowerSize = Geometry.MinimalValue;
        var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

        var geometry = Geometry.make_dynamic(lowerSize, upperSize);
        var createParameters = new CreateParameters(geometry);

        uint maxMaps = 1;
        uint maxReaders = 1;

        var operateParameters = new OperateParameters(maxMaps, maxReaders,
            reclaiming: new ReclaimingOptions(lifo: true),
            options: new OperateOptions(orphanReadTransactions: true, exclusive: true));

        using (IEnv env = _envFactory.Create(dbFilePath, createParameters, operateParameters))
        {
            using (ITxn trx = env.StartWrite())
            {
                var map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

                for (int i = 0; i < valuesCount; i++)
                {
                    trx.Insert(map, string.Format(key, i), i);
                }

                trx.Commit();

                //Close a database handle. Normally unnecessary.
                //Closing a database handle is not necessary, but lets mdbx_dbi_open() reuse the handle value. Usually it's better to set a bigger mdbx_env_set_maxdbs(), unless that value would be large.
                //env.CloseMap(map);
            }

            using (ITxn trx = env.StartRead())
            {
                var openMap = trx.OpenMap(mapName, KeyMode.Usual, ValueMode.Single);

                for (int i = 0; i < valuesCount; i++)
                {
                    var value = trx.Get<string, int>(openMap, string.Format(key, i));
                    if (value != i)
                    {
                        throw new Exception($"Db value={value} should to be equals {i}");
                    }
                }

                trx.Commit();
            }
        }
    }

    [Benchmark]
    [Arguments(1000, 5)]
    [Arguments(5000, 5)]
    [Arguments(10000, 5)]
    [Arguments(15000, 15)]
    [Arguments(20000, 15)]
    [Arguments(25000, 15)]
    public void LibMdbx_Insert_Read_ValuesCount_DbSize_DbSizeValue(int valuesCount, long dbSizeValue)
    {
        var fixedSize = new IntPtr((int)Geometry.Size.MB * dbSizeValue);

        var geometry = Geometry.make_fixed(fixedSize);
        var createParameters = new CreateParameters(geometry);

        uint maxMaps = 1;
        uint maxReaders = 1;

        var operateParameters = new OperateParameters(maxMaps, maxReaders,
            reclaiming: new ReclaimingOptions(lifo: true),
            options: new OperateOptions(orphanReadTransactions: true, exclusive: true));

        using (IEnv env = _envFactory.Create(dbFilePath, createParameters, operateParameters))
        {
            using (ITxn trx = env.StartWrite())
            {
                var map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

                for (int i = 0; i < valuesCount; i++)
                {
                    trx.Insert(map, string.Format(key, i), i);
                }

                trx.Commit();

                //Close a database handle. Normally unnecessary.
                //Closing a database handle is not necessary, but lets mdbx_dbi_open() reuse the handle value. Usually it's better to set a bigger mdbx_env_set_maxdbs(), unless that value would be large.
                //env.CloseMap(map);
            }

            using (ITxn trx = env.StartRead())
            {
                var openMap = trx.OpenMap(mapName, KeyMode.Usual, ValueMode.Single);

                for (int i = 0; i < valuesCount; i++)
                {
                    var value = trx.Get<string, int>(openMap, string.Format(key, i));
                    if (value != i)
                    {
                        throw new Exception($"Db value={value} should to be equals {i}");
                    }
                }

                trx.Commit();
            }
        }
    }

    ///// <summary>
    ///// We use one map, first time we insert - then returns false each time.
    ///// </summary>
    //[Benchmark]
    //public void TryInsert()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i++)
    //            {
    //                trx.TryInsert(map, i, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, first time we insert - then update.
    ///// </summary>
    //[Benchmark]
    //public void Upsert()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i++)
    //            {
    //                trx.Upsert(map, i, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, in global setup we insert values, then update each inserted value.
    ///// </summary>
    //[Benchmark]
    //public void Update()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.OpenMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i++)
    //            {
    //                trx.Update(map, i, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, in global setup we insert values, then try update not exists values.
    ///// </summary>
    //[Benchmark]
    //public void TryUpdate()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.OpenMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = iterationCount; i < iterationCount + iterationCount; i++)
    //            {
    //                trx.TryUpdate(map, i, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, then try update not exists values.
    ///// </summary>
    //[Benchmark]
    //public void TryUpdateEmptyMap()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i++)
    //            {
    //                trx.TryUpdate(map, i, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, in global setup we insert values, then erase values.
    ///// </summary>
    //[Benchmark]
    //public void EraseByKey()
    //{
    //    Console.WriteLine($"Method {nameof(EraseByKey)}");

    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.OpenMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i++)
    //            {
    //                trx.Erase(map, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, in global setup we insert values, then erase values.
    ///// </summary>
    //[Benchmark]
    //public void EraseByKeyValue()
    //{
    //    Console.WriteLine($"Method {nameof(EraseByKeyValue)}");

    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.OpenMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i++)
    //            {
    //                trx.Erase(map, i, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, in global setup we insert values, then erase values.
    ///// </summary>
    //[Benchmark]
    //public void Replace()
    //{
    //    Console.WriteLine($"Method {nameof(EraseByKey)}");

    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.OpenMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i++)
    //            {
    //                trx.Replace(map, i, i, i + iterationCount);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, we insert new values each time using cursor.
    ///// </summary>
    //[Benchmark]
    //public void CursorInsert()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            using (var cursor = trx.OpenCursor(map))
    //            {
    //                for (int i = 0; i < iterationCount; i++)
    //                {
    //                    cursor.Insert(i, i);
    //                }
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, first time we insert - then returns false each time.
    ///// </summary>
    //[Benchmark]
    //public void CursorTryInsert()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            using (var cursor = trx.OpenCursor(map))
    //            {
    //                for (int i = 0; i < iterationCount; i++)
    //                {
    //                    cursor.TryInsert(i, i);
    //                }
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, first time we insert - then update each time.
    ///// </summary>
    //[Benchmark]
    //public void CursorUpsert()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            using (var cursor = trx.OpenCursor(map))
    //            {
    //                for (int i = 0; i < iterationCount; i++)
    //                {
    //                    cursor.Upsert(i, i);
    //                }
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, in global setup we insert values, then update each inserted value.
    ///// </summary>
    //[Benchmark]
    //public void CursorUpdate()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            using (var cursor = trx.OpenCursor(map))
    //            {
    //                while (cursor.ToNext<int, int>(out var cursorResult))
    //                {
    //                    cursor.Update(cursorResult.Key, 5);
    //                }
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, in global setup we insert values, then update each inserted value.
    ///// </summary>
    //[Benchmark]
    //public void CursorTryUpdate()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            using (var cursor = trx.OpenCursor(map))
    //            {
    //                while (cursor.ToNext<int, int>(out var cursorResult))
    //                {
    //                    cursor.TryUpdate(cursorResult.Key, 15);
    //                }
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, in global setup we insert values, then erase values.
    ///// </summary>
    //[Benchmark]
    //public void CursorErase()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            using (var cursor = trx.OpenCursor(map))
    //            {
    //                for (int i = 0; i < iterationCount; i++)
    //                {
    //                    cursor.Erase(i);
    //                }
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, update each iteration step value.
    ///// </summary>
    //[Benchmark]
    //public void UpdateEachIterationStepValue()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.OpenMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i += iterationStep)
    //            {
    //                trx.Update(map, i, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// We use one map, update each iteration step value.
    ///// </summary>
    //[Benchmark]
    //public void CursorUpdateEachIterationStepValue()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            using (var cursor = trx.OpenCursor(map))
    //            {
    //                while (cursor.ToNext<int, int>(out var cursorResult))
    //                {
    //                    if ((cursorResult.Key % iterationStep) == 0)
    //                    {
    //                        cursor.Update(cursorResult.Key, 5);
    //                    }
    //                }
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// Update one value.
    ///// </summary>
    //[Benchmark]
    //public void UpdateOneValue()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.OpenMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            trx.Update(map, 1000, 1000);

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    ///// <summary>
    ///// Cursor update one value.
    ///// </summary>
    //[Benchmark]
    //public void CursorUpdateOneValue()
    //{
    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            using (var cursor = trx.OpenCursor(map))
    //            {
    //                while (cursor.ToNext<int, int>(out var cursorResult))
    //                {
    //                    if (cursorResult.Key == 1000)
    //                    {
    //                        cursor.Update(cursorResult.Key, 5);
    //                    }
    //                }
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    #endregion

    //#region Erase Iteration Setup

    //[IterationSetup(Targets = new[] { nameof(EraseByKey), nameof(EraseByKeyValue), nameof(Replace), nameof(CursorErase) })]
    //public void EraseReplaceIterationSetup()
    //{
    //    Directory.CreateDirectory(dbFolderPath);

    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i++)
    //            {
    //                trx.Insert(map, i, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    //[IterationCleanup(Targets = new[] { nameof(EraseByKey), nameof(EraseByKeyValue), nameof(Replace), nameof(CursorErase) })]
    //public void EraseReplaceIterationCleanup()
    //{
    //    Env.Remove(dbFilePath);
    //}

    //#endregion

    //#region Update, TryInsert Global Setup

    //[GlobalSetup(Targets = new[]{ nameof(Update), nameof(TryUpdate),
    //    nameof(CursorUpdate), nameof(CursorTryUpdate), nameof(UpdateEachIterationStepValue),
    //    nameof(UpdateOneValue), nameof(CursorUpdateOneValue), nameof(CursorUpdateEachIterationStepValue) })]
    //public void UpdateGlobalSetup()
    //{
    //    Directory.CreateDirectory(dbFolderPath);

    //    var lowerSize = Geometry.MinimalValue;
    //    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    //    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    //    var createParameters = new CreateParameters(geometry);

    //    uint maxMaps = 5;
    //    uint maxReaders = 100;

    //    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //    //we should always insert data from the same thread
    //    using (Env env = new Env(dbFilePath, createParameters, operateParameters))
    //    {
    //        MapHandle map;

    //        using (ITxn trx = env.StartWrite())
    //        {
    //            map = trx.CreateMap(mapName, KeyMode.Usual, ValueMode.Single);

    //            for (int i = 0; i < iterationCount; i++)
    //            {
    //                trx.Insert(map, i, i);
    //            }

    //            trx.Commit();
    //        }

    //        env.CloseMap(map);
    //    }
    //}

    //[GlobalCleanup(Targets = new[] { nameof(Update), nameof(TryUpdate), nameof(CursorUpdate),
    //    nameof(CursorTryUpdate), nameof(UpdateEachIterationStepValue), nameof(UpdateOneValue), nameof(CursorUpdateOneValue), nameof(CursorUpdateEachIterationStepValue) })]
    //public void UpdateGlobalCleanup()
    //{
    //    Env.Remove(dbFilePath);
    //}

    //#endregion

    #region Iteration Insert Setup

    [IterationSetup]
    public void InsertIterationSetup()
    {
        Directory.CreateDirectory(dbFolderPath);
    }

    [IterationCleanup]
    public void InsertIterationCleanup()
    {
        Directory.Delete(dbFolderPath, true);
    }

    #endregion

    #region Global Setup

    [GlobalSetup]
    public void GlobalSetup()
    {
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
    }

    #endregion
}
