using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Libmdbx.Net;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Xunit;

namespace Libmdbx.Tests
{
    public class TrxTests : BaseTest
    {
        public TrxTests() : base("trx_db_name")
        {
        }

        [Fact]
        public void CreateMap_MaxMapsZero_Exception()
        {
            uint max_maps = 0;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    var exception = Assert.Throws<LibmdbxException>(() => trx.CreateMap("test", KeyMode.Usual, ValueMode.Single));
                    Assert.Equal(LibmdbxResultCodeFlag.DBS_FULL, exception.ErrorCode);
                }
            }
        }

        [Fact]
        public void CreateTwoMaps_MaxMapsEqualsOne_Exception()
        {
            uint max_maps = 1;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    var exception = Assert.Throws<LibmdbxException>(() => trx.CreateMap("test1", KeyMode.Usual, ValueMode.Single));
                    Assert.Equal(LibmdbxResultCodeFlag.MAP_FULL | LibmdbxResultCodeFlag.EPERM, exception.ErrorCode);
                }
            }
        }

        [Fact]
        public void CreateTwoMaps_MaxMapsEqualsFive_MapsCreated()
        {
            uint max_maps = 5;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    var map1 = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.NotEqual((uint)0, map1.dbi);
                    var map2 = trx.CreateMap("test1", KeyMode.Usual, ValueMode.Single);
                    Assert.NotEqual((uint)0, map2.dbi);
                    Assert.NotEqual(map1.dbi, map2.dbi);

                    trx.Commit();

                    env.CloseMap(map1);
                    env.CloseMap(map2);
                }
            }
        }

        [Fact]
        public void CreateTransaction_EnvCreated_EnvEquals()
        {
            uint max_maps = 5;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    var map1 = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.NotEqual((uint)0, map1.dbi);
                    var map2 = trx.CreateMap("test1", KeyMode.Usual, ValueMode.Single);
                    Assert.NotEqual((uint)0, map2.dbi);
                    Assert.NotEqual(map1.dbi, map2.dbi);

                    trx.Commit();

                    env.CloseMap(map1);
                    env.CloseMap(map2);
                }
            }
        }

        [Fact]
        public void CreateMap_MaxReadersZero_Exception()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.True(map.dbi != 0);
                }
            }
        }

        [Fact]
        public void TrxInsert_InsertDuplicatedValues_Exception()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    MapHandle map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    var key = 1;
                    var value = 1;

                    trx.Insert(map, key, value);

                    var exception = Assert.Throws<LibmdbxException>(() => trx.Insert(map, key, value));
                    Assert.Equal(LibmdbxResultCodeFlag.KEY_EXIST, exception.ErrorCode);
                }
            }
        }

        [Fact]
        public void TrxInsert_TwoDifferentMaps_DataAdded()
        {
            uint max_maps = 2;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    int key = 1;
                    int value = 1;

                    MapHandle map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    MapHandle map1 = trx.CreateMap("test1", KeyMode.Usual, ValueMode.Single);

                    trx.Insert(map, key, value);
                    trx.Insert(map1, key, value);

                    var getResult = trx.TryGet(map, key, out int testValue);
                    var getResult1 = trx.TryGet(map1, key, out int testValue1);

                    Assert.True(getResult);
                    Assert.Equal(value, testValue);
                    Assert.True(getResult1);
                    Assert.Equal(value, testValue1);

                    trx.Commit();

                    env.CloseMap(map);
                    env.CloseMap(map1);
                }
            }
        }

        [Fact]
        public void OpenMap_MapNotExists_Exception()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    var exception = Assert.Throws<LibmdbxException>(() => trx.OpenMap("test", KeyMode.Usual, ValueMode.Single));
                    Assert.Equal(LibmdbxResultCodeFlag.NOTFOUND, exception.ErrorCode);
                }
            }
        }

        [Fact]
        public void OpenMap_MapExists_DbiIsNotZero()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    MapHandle createMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.True(createMap.dbi != 0);

                    MapHandle openMap = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.True(openMap.dbi != 0);

                    Assert.True(createMap.dbi == openMap.dbi);
                }
            }
        }

        [Fact]
        public void OpenMap_MapDropped_Exception()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    MapHandle createMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.True(createMap.dbi != 0);

                    trx.DropMap(createMap);

                    var exception = Assert.Throws<LibmdbxException>(() => trx.OpenMap("test", KeyMode.Usual, ValueMode.Single));
                    Assert.Equal(LibmdbxResultCodeFlag.NOTFOUND, exception.ErrorCode);
                }
            }
        }

        [Fact]
        public void DropMap_MapIsNotExists_ReturnsFalse()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    Assert.False(trx.DropMap("test"));
                }
            }
        }

        [Fact]
        public void CloseMap_MapDropped_Exception()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    MapHandle createMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.True(createMap.dbi != 0);

                    trx.DropMap(createMap);

                    var exception = Assert.Throws<LibmdbxException>(() => env.CloseMap(createMap));
                    Assert.Equal(LibmdbxResultCodeFlag.BAD_DBI, exception.ErrorCode);
                }
            }
        }

        [Fact]
        public void DropMap_MapExists_ReturnsTrue()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    MapHandle createMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.True(createMap.dbi != 0);

                    Assert.True(trx.DropMap("test"));
                }
            }
        }

        [Fact]
        public void ClearMap_MapExists_MapCleared()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    var key = 1;
                    var value = 1;

                    MapHandle createMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.True(createMap.dbi != 0);

                    trx.Insert(createMap, key, value);
                    var getResult = trx.TryGet(createMap, key, out int testValue);

                    Assert.True(getResult);
                    Assert.Equal(value, testValue);
  
                    trx.ClearMap(createMap);

                    var getResult1 = trx.TryGet(createMap, key, out int testValue1);

                    Assert.False(getResult1);

                    trx.Commit();

                    env.CloseMap(createMap);
                }
            }
        }

        [Fact]
        public void ClearMap_MapNotExists_ReturnFalse()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    Assert.False(trx.ClearMap("test"));
                }
            }
        }

        [Fact]
        public void ClearMap_MapNotExists_Exception()
        {
            uint max_maps = 10;
            uint max_readers = 0;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    var exception = Assert.Throws<LibmdbxException>(() => trx.ClearMap("test", true));
                    Assert.Equal(LibmdbxResultCodeFlag.NOTFOUND, exception.ErrorCode);
                }
            }
        }

        [Fact]
        public async Task Commit_CommitFromDifferentThread_Exception()
        {
            using (AutoSetSynchronizationContext context = new AutoSetSynchronizationContext())
            {
                using (BackgroundWorker worker = new BackgroundWorker(context))
                {
                    SynchronizationContext.SetSynchronizationContext(context);
                    worker.Start();

                    //switch to inner context thread
                    await Task.Yield();

                    uint max_maps = 10;
                    uint max_readers = 0;

                    var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
                    CreateParameters createParameters = new CreateParameters(geometry);
                    OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

                    using (IEnv env = new Env(path, createParameters, operateParameters))
                    {
                        using (ITxn trx = env.StartWrite())
                        {
                            var exception = await Assert.ThrowsAsync<LibmdbxException>(() => Task.Factory.StartNew(() => trx.Commit()));
                            Assert.Equal(LibmdbxResultCodeFlag.THREAD_MISMATCH, exception.ErrorCode);
                        }
                    }
                }
            }
        }

        [Fact]
        public void UpsertSupportMultipleValues_AddedMultipleValues_ReturnedMultipleValuesCount()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                IList<string> values = new List<string>(10);

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    for (int i = 0; i < 10; i++)
                    {
                        var value = $"value - {i}";

                        values.Add(value);
                        trx.Upsert(map, key, value);
                    }

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    var result = trx.Get<int, string>(map, key, out uint valuesCount);

                    Assert.Equal((uint)values.Count, valuesCount);
                    Assert.Equal(values[0], result);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void UpsertDoNotSupportMultipleValues_AddedMultipleValues_Exception()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                int count = 10;
                IList<string> values = new List<string>(10);

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    for (int i = 0; i < count; i++)
                    {
                        var value = $"value - {i}";

                        values.Add(value);
                        trx.Upsert(map, key, value);
                    }

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    var result = trx.Get<int, string>(map, key, out uint valuesCount);

                    Assert.Equal((uint)1, valuesCount);
                    Assert.Equal(values[count-1], result);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void Update_ValueNotExists_Exception()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                IList<string> values = new List<string>(10);

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    for (int i = 0; i < 10; i++)
                    {
                        var value = $"value - {i}";

                        values.Add(value);
                        var exception = Assert.Throws<LibmdbxException>(() => trx.Update(map, key, value));
                        Assert.Equal(LibmdbxResultCodeFlag.NOTFOUND, exception.ErrorCode);
                    }

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void TryUpdate_ValueNotExists_ReturnsFalse()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    Assert.False(trx.TryUpdate(map, key, "value"));

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void UpdateMultiValues_ValueExists_Exception()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                IList<string> values = new List<string>(10);

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    for (int i = 0; i < 10; i++)
                    {
                        var value = $"value - {i}";

                        values.Add(value);
                        trx.Upsert(map, key, value);
                    }

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Multi);

                    var value = "value updated";

                    var result = Assert.Throws<LibmdbxException>(() => trx.Update(map, key, value));
                    Assert.Equal(LibmdbxResultCodeFlag.EMULTIVAL, result.ErrorCode);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void Update_ValueExists_ValueUpdated()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers,
                reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    var value = "value";

                    trx.Upsert(map, key, value);

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    var value = "value updated";

                    trx.Update(map, key, value);

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    var result = trx.Get<int, string>(map, key, out uint valuesCount);

                    Assert.Equal((uint)1, valuesCount);
                    Assert.Equal("value updated", result);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void TryUpdate_ValueExists_ValueUpdated()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    var value = "value";

                    trx.Insert(map, key, value);

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    var value = "value updated";

                    Assert.True(trx.TryUpdate(map, key, value));

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    var result = trx.Get<int, string>(map, key);

                    Assert.Equal("value updated", result);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void Erase_KeyNotExists_ReturnsFalse()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                var value = "value";

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    Assert.False(trx.Erase(map, key, value));

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void Erase_KeyExists_ReturnsTrue()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                var value = "value";

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    trx.Insert(map, key, value);

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    Assert.True(trx.Erase(map, key, value));

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void Erase_ExistsMultipleValues_ReturnsTrue()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                var value = "value1";

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    trx.Upsert(map, key, "value1");
                    trx.Upsert(map, key, "value2");
                    trx.Upsert(map, key, "value3");

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Multi);

                    Assert.True(trx.Erase(map, key, value));

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    var result = trx.Get<int, string>(map, key, out uint valuesCount);

                    Assert.Equal((uint)2, valuesCount);
                    Assert.Equal("value2", result);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void EraseOneValueFromMultipleValues_ExistsMultipleValues_ReturnsTrue()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    trx.Upsert(map, key, "value1");
                    trx.Upsert(map, key, "value2");
                    trx.Upsert(map, key, "value3");

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    Assert.True(trx.Erase(map, key, "value2"));

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Multi);

                    var result = trx.TryGet(map, key, out string val);

                    Assert.True(result);
                    Assert.Equal("value1", val);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void EraseAllMultipleValues_ExistsMultipleValues_ReturnsTrue()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    trx.Upsert(map, key, "value1");
                    trx.Upsert(map, key, "value2");
                    trx.Upsert(map, key, "value3");

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    Assert.True(trx.Erase(map, key));

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Multi);

                    var result = trx.TryGet(map, key, out string val);

                    Assert.False(result);
                    Assert.Equal(default, val);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void Erase_DataExists_DataErased()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                string value = "value1";

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    trx.Insert(map, key, value);

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    Assert.True(trx.Erase(map, key));

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    var result = trx.TryGet(map, key, out string val);

                    Assert.False(result);
                    Assert.Equal(default, val);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void Replace_DataExists_DataReplaced()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                string value = "value1";
                string replaceValue = "value_replace";

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    trx.Insert(map, key, value);

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    trx.Replace(map, key, value, replaceValue);

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                    var result = trx.TryGet(map, key, out string val);

                    Assert.True(result);
                    Assert.Equal(replaceValue, val);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void Replace_DataNotExists_DataReplaced()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                string value = "value1";
                string replaceValue = "value_replace";

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    var exception = Assert.Throws<LibmdbxException>(() => trx.Replace(map, key, value, replaceValue));
                    Assert.Equal(LibmdbxResultCodeFlag.NOTFOUND, exception.ErrorCode);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void Replace_ExistsMultipleValues_DataReplaced()
        {
            uint max_maps = 10;
            uint max_readers = 10;

            var geometry = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(geometry);
            OperateParameters operateParameters = new OperateParameters(max_maps, max_readers, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int key = 1;
                string value = "value1";
                string replaceValue = "value_replace";

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    trx.Upsert(map, key, value);

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Multi);

                    trx.Replace(map, key, value, replaceValue, true);

                    trx.Commit();
                    env.CloseMap(map);
                }

                using (ITxn trx = env.StartRead())
                {
                    var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Multi);

                    var result = trx.TryGet(map, key, out string val);

                    Assert.True(result);
                    Assert.Equal(replaceValue, val);

                    trx.Commit();
                    env.CloseMap(map);
                }
            }
        }
    }
}
