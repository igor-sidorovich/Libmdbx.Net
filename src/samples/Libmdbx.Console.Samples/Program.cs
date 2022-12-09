using System.Reflection;
using Libmdbx.Net;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Libmdbx.Net.Shared;

string path;
string db_name = "mdbx-test.db";
var envFactory = new EnvFactory();

//Create example which close/dispose env until transaction in progress
//
EnvDynamicGrowthExample();
await ReadDataParallelFromDifferentThreadsUsingTheSameEnvironment();
await ReadDataParallelFromDifferentThreadsUsingDifferentEnvironmentsExclusiveMode();
await ReadDataParallelFromDifferentThreadsUsingOneEnvironmentNotExclusiveMode();//we use current approach on a bingo
await ReadDataDuringUpdateData();
await DemonstrateMvccApproach();
await WriteDataParallelFromDifferentThreadsUsingTheSameEnvironment();
await CloseEnvDuringWrite();
await CloseEnvDuringRead();

void RemoveDbFile()
{
    path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "mdbx");
    if (Directory.Exists(path))
    {
        envFactory.Remove(Path.Combine(path, db_name));
    }
    Directory.CreateDirectory(path);
    path = Path.Combine(path, db_name);
}

Dictionary<string, string> BuildKeyValues(int range)
{
    var dict = new Dictionary<string, string>();

    for (int i = 0; i < range; i++)
    {
        dict.Add($"test_key_{i}", $"test_value_{i}");
    }

    return dict;
}

/// <summary>
/// Current example shows dynamic growth of db file.
/// We insert 1000000 values in the db.
/// In current example we using MDBX_NOTLS and MDBX_EXCLUSIVE(see documentation to understand what current flags means).
/// </summary>
void EnvDynamicGrowthExample()
{
    Console.WriteLine($"Start example {nameof(EnvDynamicGrowthExample)}");

    RemoveDbFile();

    var lowerSize = Geometry.MinimalValue;
    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    var createParameters = new CreateParameters(geometry);

    uint maxMaps = 500;
    uint maxReaders = 100;

    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive:true));

    using (IEnv env = envFactory.Create(path, createParameters, operateParameters))
    {
        using (ITxn trx = env.StartWrite())
        {
            var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

            for (int i = 0; i < 1000000; i++)
            {
                trx.Insert(map, i, i);
            }

            trx.Commit();

            //Close a database handle. Normally unnecessary.
            //Closing a database handle is not necessary, but lets mdbx_dbi_open() reuse the handle value. Usually it's better to set a bigger mdbx_env_set_maxdbs(), unless that value would be large.
            env.CloseMap(map);
        }
    }

    FileInfo fileInfo = new FileInfo(path);
    var length = fileInfo.Length;

    Console.WriteLine($"Finish example {nameof(EnvDynamicGrowthExample)}");
}

///<summary>
/// Close env during read transactions.
/// Offen generates memory corruption exception.
/// </summary>
async Task CloseEnvDuringRead()
{
    RemoveDbFile();

    var lowerSize = Geometry.MinimalValue;
    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    var createParameters = new CreateParameters(geometry);

    uint maxMaps = 5;
    uint maxReaders = 100;

    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));
   
    try
    {
        Console.WriteLine($"Start example {nameof(CloseEnvDuringRead)}");

        //we should always insert data from the same thread
        List<Task> tasks = new List<Task>();

        using (IEnv env = envFactory.Create(path, createParameters, operateParameters))
        {
            using (ITxn trx = env.StartWrite())
            {
                try
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    for (int i = 0; i <= 10; i++)
                    {
                        trx.Upsert(map, i, i);
                    }

                    trx.Commit();
                }
                catch (Exception ex)
                {
                    trx.Abort();
                }
            }

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            for (int i = 1; i < 20; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    using (ITxn trx = env.StartRead())
                    {
                        var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                        for (int i = 0; i <= 10; i++)
                        {
                            var value = trx.Get<int, int>(map, i);
                        }
                        manualResetEvent.Set();
                        Thread.Sleep(1000);
                        trx.Commit();
                    }
                }));
            }

            manualResetEvent.WaitOne();
        }

        await Task.WhenAll(tasks);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

    using (IEnv env = envFactory.Create(path, createParameters, operateParameters))
    {

    }

    Console.WriteLine($"Finish example {nameof(CloseEnvDuringRead)}");
}

///<summary>
/// Close env during write transactions.
/// Will be returned MDBX_BUSY code in mdbx_env_close_ex.
/// </summary>
async Task CloseEnvDuringWrite()
{
    Console.WriteLine($"Start example {nameof(CloseEnvDuringWrite)}");

    RemoveDbFile();

    var lowerSize = Geometry.MinimalValue;
    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    var createParameters = new CreateParameters(geometry);

    uint maxMaps = 5;
    uint maxReaders = 100;

    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //we should always insert data from the same thread
    List<Task> tasks = new List<Task>();

    using (IEnv env = envFactory!.Create(path, createParameters, operateParameters))
    {
        for (int i = 1; i < 10; i++)
        {
            tasks.Add(Task.Factory.StartNew(o =>
            {
                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    for (int i = 0; i <= 10; i++)
                    {
                        trx.Upsert(map, i, (int)o);
                    }

                    trx.Commit();
                }
            }, i));
        }

        Thread.Sleep(1000);
    }

    await Task.WhenAll(tasks);

    Console.WriteLine($"Finish example {nameof(CloseEnvDuringWrite)}");
}

/// <summary>
/// In current example we are writing data in parallel from different threads.
/// </summary>
async Task WriteDataParallelFromDifferentThreadsUsingTheSameEnvironment()
{
    Console.WriteLine($"Start example {nameof(WriteDataParallelFromDifferentThreadsUsingTheSameEnvironment)}");

    RemoveDbFile();

    var lowerSize = Geometry.MinimalValue;
    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    var createParameters = new CreateParameters(geometry);

    uint maxMaps = 5;
    uint maxReaders = 100;

    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //we should always insert data from the same thread
    List<Task> tasks = new List<Task>();

    using (IEnv env = envFactory!.Create(path, createParameters, operateParameters))
    {
        for (int i = 1; i < 10; i++)
        {
            tasks.Add(Task.Factory.StartNew(o =>
            {
                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    for (int j = 0; j <= 10; j++)
                    {
                        trx.Upsert(map, j, (int)o);
                    }

                    trx.Commit();
                }
            }, i));
        }

        await Task.WhenAll(tasks);

        for (int i = 0; i < 1; i++)
        {
            using (ITxn trx = env.StartRead())
            {
                var openMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                for (int j = 0; j < 10; j++)
                {
                    try
                    {
                        var value = trx.Get<int, int>(openMap, j);
                        Console.WriteLine(value);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                trx.Commit();
            }
        }
    }

    Console.WriteLine($"Finish example {nameof(WriteDataParallelFromDifferentThreadsUsingTheSameEnvironment)}");
}

/// <summary>
/// It's example of parallel reading from different threads the same map.
/// We create test map and insert values from one thread, after that creates 10 threads and try to read inserted values from the same map.
/// We use the same environment value when reading from different threads.
/// We open db with NoTls - true; Exclusive - true
/// </summary>
async Task ReadDataParallelFromDifferentThreadsUsingTheSameEnvironment()
{
    Console.WriteLine($"Start example {nameof(ReadDataParallelFromDifferentThreadsUsingTheSameEnvironment)}");

    RemoveDbFile();

    var lowerSize = Geometry.MinimalValue;
    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    var createParameters = new CreateParameters(geometry);

    uint maxMaps = 5;
    uint maxReaders = 100;

    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: true));

    //we should always insert data from the same thread
    using (IEnv env = envFactory.Create(path, createParameters, operateParameters))
    {
        MapHandle map;

        using (ITxn trx = env.StartWrite())
        {
            map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

            for (int i = 0; i < 1000000; i++)
            {
                trx.Insert(map, i, i);
            }

            trx.Commit();
        }

        List<Task> tasks = new List<Task>();

        for (int i = 0; i < 20; i++)
        {
            tasks.Add(Task.Factory.StartNew(o =>
            {
                var localEnv = (Env)o!;
                using (ITxn trx = localEnv.StartRead())
                {
                    var openMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    for (int j = 0; j < 1000000; j++)
                    {
                        var value = trx.Get<int, int>(openMap, j);
                    }

                    trx.Commit();
                }
            }, env));
        }

        await Task.WhenAll(tasks);
    }

    Console.WriteLine($"Finish example {nameof(ReadDataParallelFromDifferentThreadsUsingTheSameEnvironment)}");
}

/// <summary>
/// Here we are trying to read data using different environemnts.
/// In current example will be exception - because we are using exclusive mode, and we are trying to open DB from the same process twice.
/// We open db with NoTls - true; Exclusive - true
/// </summary>
async Task ReadDataParallelFromDifferentThreadsUsingDifferentEnvironmentsExclusiveMode()
{
    try
    {
        Console.WriteLine($"Start example {nameof(ReadDataParallelFromDifferentThreadsUsingDifferentEnvironmentsExclusiveMode)}");

        RemoveDbFile();

        var keyValues = BuildKeyValues(100);

        var lowerSize = Geometry.MinimalValue;
        var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

        var geometry = Geometry.make_dynamic(lowerSize, upperSize);
        var createParameters = new CreateParameters(geometry);

        uint maxMaps = 5;
        uint maxReaders = 100;

        var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: true));

        //we should always insert data from the same thread
        using (IEnv env = envFactory.Create(path, createParameters, operateParameters))
        {
            using (ITxn trx = env.StartWrite())
            {
                MapHandle map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                foreach (var keyValue in keyValues)
                {
                    trx.Insert(map, keyValue.Key, keyValue.Value);
                }

                trx.Commit();
                env.CloseMap(map);
            }
        }

        List<Task> tasks = new List<Task>();

        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Factory.StartNew(() =>
            {
                using (IEnv env = envFactory.Create(path, createParameters, operateParameters))
                {
                    using (ITxn trx = env.StartRead())
                    {
                        var openMap = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                        foreach (var keyValue in keyValues)
                        {
                            var value = trx.Get<string, string>(openMap, keyValue.Key);
                            if (value != keyValue.Value)
                            {
                                Console.WriteLine("Failed");
                            }
                        }

                        trx.Commit();
                        env.CloseMap(openMap);
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    

    Console.WriteLine($"Finished example {nameof(ReadDataParallelFromDifferentThreadsUsingDifferentEnvironmentsExclusiveMode)}");
}

/// <summary>
/// Here we are trying to read data using one environment.
/// We open db with NoTls - true; Exclusive - false
/// We should not use exclusive mode.
/// </summary>
async Task ReadDataParallelFromDifferentThreadsUsingOneEnvironmentNotExclusiveMode()
{
    Console.WriteLine($"Start example {nameof(ReadDataParallelFromDifferentThreadsUsingOneEnvironmentNotExclusiveMode)}");

    RemoveDbFile();

    var keyValues = BuildKeyValues(100);

    var lowerSize = Geometry.MinimalValue;
    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    var createParameters = new CreateParameters(geometry);

    uint maxMaps = 100;
    uint maxReaders = 100;

    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    var env = envFactory.Create(path, createParameters, operateParameters);

    //we should always insert data from the same thread
    using (ITxn trx = env.StartWrite())
    {
        MapHandle map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

        foreach (var keyValue in keyValues)
        {
            var result = trx.TryInsert(map, keyValue.Key, keyValue.Value);
        }

        trx.Commit();
        env.CloseMap(map);
    }

    List<Task> tasks = new List<Task>();

    for (int i = 0; i < 40; i++)
    {
        if (i % 2 == 0)
        {
            tasks.Add(Task.Factory.StartNew(o =>
            {
                var localEnv = (IEnv)o!;

                using (ITxn trx = localEnv.StartRead())
                {
                    var openMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    foreach (var keyValue in keyValues)
                    {
                        var value = trx.Get<string, string>(openMap, keyValue.Key);
                        if (value != keyValue.Value)
                        {
                            Console.WriteLine(value);
                        }
                    }

                    trx.Commit();
                }
            }, env));
        }
        else
        {
            tasks.Add(Task.Factory.StartNew(o =>
            {
                var localEnv = (IEnv)o!;

                using (ITxn trx = localEnv.StartWrite())
                {
                    var openMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    foreach (var keyValue in keyValues)
                    {
                        trx.Upsert(openMap, keyValue.Key, $"{keyValue.Value}_print");
                    }

                    trx.Commit();
                }
            }, env));
        }

    }

    try
    {
        await Task.WhenAll(tasks);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    env.Dispose();

    Console.WriteLine($"Finish example {nameof(ReadDataParallelFromDifferentThreadsUsingOneEnvironmentNotExclusiveMode)}");
}

/// <summary>
/// In current example we are update data during read data in parallel,
/// and sometimes during the reading data you can get updated values,
/// because update transactions can be commited before read transaction starting.
/// We open db with NoTls - true; Exclusive - false
/// </summary>
async Task ReadDataDuringUpdateData()
{
    Console.WriteLine($"Start example {nameof(ReadDataDuringUpdateData)}");

    RemoveDbFile();

    var keyValues = BuildKeyValues(100);

    var lowerSize = Geometry.MinimalValue;
    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    var createParameters = new CreateParameters(geometry);

    uint maxMaps = 5;
    uint maxReaders = 100;

    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    //we should always insert data from the same thread
    using (IEnv env = envFactory!.Create(path, createParameters, operateParameters))
    {
        MapHandle map;

        using (ITxn trx = env.StartWrite())
        {
            map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

            foreach (var keyValue in keyValues)
            {
                trx.Insert(map, keyValue.Key, keyValue.Value);
            }

            trx.Commit();
        }

        var task = Task.Factory.StartNew(o =>
        {
            var localEnv = (Env)o!;

            foreach (var keyValue in keyValues)
            {
                using (ITxn trx = localEnv.StartRead())
                {
                    var openMap = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);
                    
                    Thread.Sleep(Random.Shared.Next(500));
                    var value = trx.Get<string, string>(openMap, keyValue.Key);
                    if (value != keyValue.Value)
                    {
                        Console.WriteLine($"Failed. Updated value - {value}");
                    }

                    trx.Commit();
                }
            }
        }, env);

        foreach (var keyValue in keyValues)
        {
            using (ITxn trx = env.StartWrite())
            {
                var openMap = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                Thread.Sleep(Random.Shared.Next(600));
                trx.Update(openMap, keyValue.Key, $"{keyValue.Value}_updated");

                trx.Commit();
            }
        }

        await task;

        env.CloseMap(map);
    }

    Console.WriteLine($"Finish example {nameof(ReadDataDuringUpdateData)}");
}

/// <summary>
/// Demonstrate mvcc approach for db.
/// </summary>
async Task DemonstrateMvccApproach()
{
    Console.WriteLine($"Start example {nameof(DemonstrateMvccApproach)}");

    RemoveDbFile();

    var keyValues = BuildKeyValues(100);

    var lowerSize = Geometry.MinimalValue;
    var upperSize = new IntPtr((int)Geometry.Size.MB * 500);

    var geometry = Geometry.make_dynamic(lowerSize, upperSize);
    var createParameters = new CreateParameters(geometry);

    uint maxMaps = 5;
    uint maxReaders = 100;

    var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true, exclusive: false));

    string insertKey = "insert_key";
    string insertValue = "insert_value";
    string insertValueUpdated = "insert_value_updated";

    //we should always insert data from the same thread
    using (IEnv env = envFactory!.Create(path, createParameters, operateParameters))
    {
        MapHandle map;

        using (ITxn trx = env.StartWrite())
        {
            map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

            trx.Insert(map, insertKey, insertValue);

            trx.Commit();
        }

        ManualResetEvent lockEvent = new ManualResetEvent(false);

        //current task create transaction - only after read transaction commited
        var task1 = Task.Factory.StartNew(o =>
        {
            var localEnv = (Env)o!;

            using (ITxn trx = localEnv.StartRead())
            {
                lockEvent.WaitOne();

                var openMap = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                Thread.Sleep(Random.Shared.Next(500));
                var value = trx.Get<string, string>(openMap, insertKey);
                if (value == insertValueUpdated)
                {
                    Console.WriteLine("Failed.");
                }

                trx.Commit();
            }
        }, env);

        var task2 = Task.Factory.StartNew(o =>
        {
            var localEnv = (Env)o!;

            lockEvent.WaitOne();

            using (ITxn trx = localEnv.StartRead())
            {
                var openMap = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                Thread.Sleep(Random.Shared.Next(500));
                var value = trx.Get<string, string>(openMap, insertKey);
                if (value != insertValueUpdated)
                {
                    Console.WriteLine("Failed.");
                }

                trx.Commit();
            }
        }, env);

        Thread.Sleep(5000);

        using (ITxn trx = env.StartWrite())
        {
            map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

            trx.Update(map, insertKey, insertValueUpdated);

            trx.Commit();
            lockEvent.Set();
        }

        await Task.WhenAll(task1, task2);

        env.CloseMap(map);
    }

    Console.WriteLine($"Finish example {nameof(DemonstrateMvccApproach)}");
}
