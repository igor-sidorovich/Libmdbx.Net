using System;
using System.Collections.Generic;
using Libmdbx.Net;
using Libmdbx.Net.Core.Cursor;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Xunit;

namespace Libmdbx.Tests
{
    public class CursorTests : BaseTest
    {
        public CursorTests() : base("cursor_db_name")
        {
        }

        [Fact]
        public void OpenCursor_Cursor_CursorOpenedSuccessfully()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);
                    Assert.NotEqual((uint)0, testMap.dbi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.False(cursor.IsCompleted);
                    }
                }
            }
        }

        [Fact]
        public void InsertCursor_InsertKeyValuePair_DataInserted()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4, 5, 6, 7, 8 };
                string[] values = { "", "", "3", "4", "5", "6", "7", "8" };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int i = 0;

                        Assert.True(cursor.ToFirst<int, string>(out var firstCursorResult));
                        Assert.Equal(firstCursorResult.Key, keys[i]);
                        Assert.Equal(firstCursorResult.Value, values[i]);

                        while (cursor.ToNext<int, string>(out var nextCursorResult, false))
                        {
                            i++;
                            Assert.Equal(nextCursorResult.Key, keys[i]);
                            Assert.Equal(nextCursorResult.Value, values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorFirstPrevious_InsertKeyValuePair_PreviousReturnsFalse()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4, 5, 6, 7, 8 };
                string[] values = { "", "2", "3", "4", "5", "6", "7", "8" };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int i = 0;

                        Assert.True(cursor.ToFirst<int, string>(out var firstCursorResult));
                        Assert.True(cursor.OnFirst());
                        Assert.Equal(firstCursorResult.Key, keys[i]);
                        Assert.Equal(firstCursorResult.Value, values[i]);

                        Assert.False(cursor.ToPrevious<int, string>(out var previousCursorResult, false));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorLastPrevious_InsertKeyValuePair_PreviousReturnsValue()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4, 5, 6, 7, 8 };
                string[] values = { "", "2", "3", "4", "5", "6", "7", "8" };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.ToLast<int, string>(out var firstCursorResult));
                        Assert.True(cursor.OnLast());
                        Assert.Equal(firstCursorResult.Key, keys[^1]);
                        Assert.Equal(firstCursorResult.Value, values[^1]);

                        Assert.True(cursor.ToPrevious<int, string>(out var previousCursorResult));
                        Assert.Equal(previousCursorResult.Key, keys[^2]);
                        Assert.Equal(previousCursorResult.Value, values[^2]);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorLastFirst_InsertKeyValuePair_LastFirstHappened()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4, 5, 6, 7, 8 };
                string[] values = { "", "2", "3", "4", "5", "6", "7", "8" };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.ToLast<int, string>(out var lastCursorResult));
                        Assert.True(cursor.OnLast());
                        Assert.Equal(lastCursorResult.Key, keys[^1]);
                        Assert.Equal(lastCursorResult.Value, values[^1]);

                        Assert.True(cursor.ToFirst<int, string>(out var firstCursorResult));
                        Assert.True(cursor.OnFirst());
                        Assert.Equal(firstCursorResult.Key, keys[0]);
                        Assert.Equal(firstCursorResult.Value, values[0]);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorCurrent_InsertKeyValuePair_Exception()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4, 5, 6, 7, 8 };
                string[] values = { "", "2", "3", "4", "5", "6", "7", "8" };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        var exception = Assert.Throws<LibmdbxException>(() => cursor.Current<int, string>(out var currentCursorResult));
                        Assert.Equal(LibmdbxResultCodeFlag.ENODATA, exception.ErrorCode);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorCurrent_InsertKeyValuePair_CurrentReturnsValues()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4, 5, 6, 7, 8 };
                string[] values = { "1", "2", "3", "4", "5", "6", "7", "8" };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.ToLast<int, string>(out var lastCursorResult));
                        Assert.True(cursor.OnLast());
                        Assert.True(cursor.Eof());
                        Assert.Equal(lastCursorResult.Key, keys[^1]);
                        Assert.Equal(lastCursorResult.Value, values[^1]);

                        Assert.True(cursor.Current<int, string>(out var lastCurrentCursorResult));
                        Assert.Equal(lastCurrentCursorResult.Key, keys[^1]);
                        Assert.Equal(lastCurrentCursorResult.Value, values[^1]);

                        Assert.True(cursor.ToFirst<int, string>(out var firstCursorResult));
                        Assert.True(cursor.OnFirst());
                        Assert.Equal(firstCursorResult.Key, keys[0]);
                        Assert.Equal(firstCursorResult.Value, values[0]);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorSeek_InsertKeyValuePair_SeekOnKey()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4, 5, 6, 7, 8 };
                string[] values = { "", "2", "3", "4", "5", "6", "7", "8" };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        var findKey = 8;

                        Assert.True(cursor.Seek(findKey));

                        Assert.True(cursor.Current<int, string>(out var lastCurrentCursorResult));
                        Assert.Equal(lastCurrentCursorResult.Key, findKey);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorFind_CursorFilled_ValueFounded()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4, 5, 6, 7, 8 };
                string[] values = { "", "2", "3", "4", "5", "6", "7", "8" };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 5;

                        Assert.True(cursor.Find<int, string>(keys[index], out var cursorResult));

                        Assert.Equal(cursorResult.Key, keys[index]);
                        Assert.Equal(cursorResult.Value, values[index]);

                        Assert.True(cursor.Current<int, string>(out var currentResult));
                        Assert.Equal(currentResult.Key, keys[index]);
                        Assert.Equal(currentResult.Value, values[index]);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorLowerBound_CursorFilled_LowerBoundFounded()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4, 5, 9, 13, 15 };
                string[] values = { "", "2", "3", "4", "5", "9", "13", "15" };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 5;

                        Assert.True(cursor.LowerBound<int, string>(6, out var cursorResult));

                        Assert.Equal(cursorResult.Key, keys[index]);
                        Assert.Equal(cursorResult.Value, values[index]);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorCurrentNextMulti_CursorFilled_ReturnsFirstMulti()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };

                Dictionary<int, List<string>> keyValues = new Dictionary<int, List<string>>
                {
                    { keys[0], new List<string>{"", "11", "111", "1111", "11111"} },
                    { keys[1], new List<string>{"", "22", "222", "2222", "22222"} },
                    { keys[2], new List<string>{"", "33", "333", "3333", "33333"} },
                    { keys[3], new List<string>{"", "44", "444", "4444", "44444"} }
                };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        foreach (var keyValue in keyValues)
                        {
                            foreach (var value in keyValue.Value)
                            {
                                cursor.Upsert(keyValue.Key, value);
                            }
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;
                        CursorResult<int, string> cursorResult = default;

                        do
                        {
                            int i = 0;
                            if (index > 0)
                            {
                                Assert.Equal(cursorResult.Key, keys[index]);
                                Assert.Equal(cursorResult.Value, keyValues[keys[index]][i]);

                                i = 1;
                            }
                            while (cursor.ToCurrentNextMulti(out cursorResult, false))
                            {
                                Assert.Equal(cursorResult.Key, keys[index]);
                                Assert.Equal(cursorResult.Value, keyValues[keys[index]][i]);
                                i++;
                            }

                            index++;
                        } 
                        while (cursor.ToNext(out cursorResult, false));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorToNextFirstMultiToCurrentNextMulti_CursorFilled_ReturnsFirstMulti()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };

                Dictionary<int, List<string>> keyValues = new Dictionary<int, List<string>>
                {
                    { keys[0], new List<string>{"", "11", "111", "1111", "11111"} },
                    { keys[1], new List<string>{"", "22", "222", "2222", "22222"} },
                    { keys[2], new List<string>{"", "33", "333", "3333", "33333"} },
                    { keys[3], new List<string>{"", "44", "444", "4444", "44444"} }
                };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        foreach (var keyValue in keyValues)
                        {
                            foreach (var value in keyValue.Value)
                            {
                                cursor.Upsert(keyValue.Key, value);
                            }
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;

                        while (cursor.ToNextFirstMulti<int, string>(out var cursorResult, false))
                        {
                            int i = 0;

                            Assert.Equal(cursorResult.Key, keys[index]);
                            Assert.Equal(cursorResult.Value, keyValues[keys[index]][i]);

                            while (cursor.ToCurrentNextMulti(out cursorResult, false))
                            {
                                Assert.Equal(cursorResult.Key, keys[index]);
                                Assert.Equal(cursorResult.Value, keyValues[keys[index]][++i]);
                            }

                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorToCurrentLastFirstPrevMulti_CursorFilled_ReturnsFirstMulti()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };

                Dictionary<int, List<string>> keyValues = new Dictionary<int, List<string>>
                {
                    { keys[0], new List<string>{"", "11", "111", "1111", "11111"} },
                    { keys[1], new List<string>{"", "22", "222", "2222", "22222"} },
                    { keys[2], new List<string>{"", "33", "333", "3333", "33333"} },
                    { keys[3], new List<string>{"", "44", "444", "4444", "44444"} }
                };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        foreach (var keyValue in keyValues)
                        {
                            foreach (var value in keyValue.Value)
                            {
                                cursor.Upsert(keyValue.Key, value);
                            }
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;

                        while (cursor.ToNextFirstMulti<int, string>(out var cursorResult, false))
                        {
                            int i = 0;

                            Assert.Equal(cursorResult.Key, keys[index]);
                            Assert.Equal(cursorResult.Value, keyValues[keys[index]][i]);

                            Assert.True(cursor.ToCurrentLastMulti<int, string>(out var currentLastMultiResult, false));
                            Assert.Equal(currentLastMultiResult.Value, keyValues[keys[index]][4]);

                            Assert.True(cursor.ToCurrentPrevMulti<int, string>(out var currentPrevMultiResult, false));
                            Assert.Equal(currentPrevMultiResult.Key, keys[index]);
                            Assert.Equal(currentPrevMultiResult.Value, keyValues[keys[index]][3]);

                            Assert.True(cursor.ToCurrentFirstMulti<int, string>(out var currentFirstMultiResult, false));
                            Assert.Equal(currentFirstMultiResult.Value, keyValues[keys[index]][0]);

                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorLowerBoundMultivalue_CursorFilled_LowerBoundMultivalueExecuted()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };

                Dictionary<int, List<string>> keyValues = new Dictionary<int, List<string>>
                {
                    { keys[0], new List<string>{"", "11", "111", "1111", "11111"} },
                    { keys[1], new List<string>{"", "22", "222", "2222", "22222"} },
                    { keys[2], new List<string>{"", "33", "333", "3333", "33333"} },
                    { keys[3], new List<string>{"", "44", "444", "4444", "44444"} }
                };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        foreach (var keyValue in keyValues)
                        {
                            foreach (var value in keyValue.Value)
                            {
                                cursor.Upsert(keyValue.Key, value);
                            }
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.LowerBoundMultivalue(keys[1], "22", out var cursorResult, false));
                        Assert.Equal(cursorResult.Value, "22");
                        Assert.Equal(cursorResult.Key, keys[1]);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorTryInsert_InsertData_DataInserted()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };

                Dictionary<int, List<string>> keyValues = new Dictionary<int, List<string>>
                {
                    { keys[0], new List<string>{""} },
                    { keys[1], new List<string>{"2"} },
                    { keys[2], new List<string>{"3"} },
                    { keys[3], new List<string>{"4"} }
                };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        foreach (var keyValue in keyValues)
                        {
                            foreach (var value in keyValue.Value)
                            {
                                Assert.True(cursor.TryInsert(keyValue.Key, value));
                            }
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorTryInsert_DuplicateData_DataIsNotInserted()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };

                Dictionary<int, List<string>> keyValues = new Dictionary<int, List<string>>
                {
                    { keys[0], new List<string>{""} },
                    { keys[1], new List<string>{"2"} },
                    { keys[2], new List<string>{"3"} },
                    { keys[3], new List<string>{"4"} }
                };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.TryInsert(keys[0], 1));
                        Assert.False(cursor.TryInsert(keys[0], 2));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorUpdate_DatabaseEmpty_Exception()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };
                int[] values = { 0, 2, 3, 4 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        var exception = Assert.Throws<LibmdbxException>(() => cursor.Update(keys[0], values[0]));
                        Assert.Equal(LibmdbxResultCodeFlag.ENODATA, exception.ErrorCode);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorTryUpdate_DatabaseEmpty_ReturnFalse()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };
                int[] values = { 0, 2, 3, 4 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        var exception = Assert.Throws<LibmdbxException>(() => cursor.TryUpdate(keys[0], values[0]));
                        Assert.Equal(LibmdbxResultCodeFlag.ENODATA, exception.ErrorCode);
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorUpdate_DatabaseIsNotEmpty_DataUpdated()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };
                int[] values = { 0, 2, 3, 4 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        while (cursor.ToNext<int, int>(out var cursorResult, false))
                        {
                            cursor.Update(cursorResult.Key, 5);
                        }
                    }

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        while (cursor.ToNext<int, int>(out var cursorResult, false))
                        {
                            Assert.Equal(5, cursorResult.Value);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorTryUpdate_KeyIsNotExists_ReturnsFalse()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 1, 2, 3, 4 };
                int[] values = { 1, 2, 3, 4 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        cursor.ToNext<int, int>(out var cursorResult, false);
                        Assert.False(cursor.TryUpdate(2, 5));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorUpsertValue_DataExists_ReturnsFalse()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 1, 2, 3, 4 };
                int[] values = { 1, 2, 3, 4 };

                int[] upsertKeys = { 1, 2, 3, 4 };
                int[] upsertValues = { 10, 20, 30, 40 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    int index = 0;
                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(keys[index], cursorResult.Key);
                            Assert.Equal(values[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);


                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        cursor.ToFirst<int, int>(out var cursorResult);
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Upsert(keys[i], upsertValues[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;

                        while (cursor.ToNext(out CursorResult<int, int> cursorResult))
                        {
                            Assert.Equal(cursorResult.Key, keys[index]);
                            Assert.Equal(cursorResult.Value, upsertValues[index]);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorUpsertKeyValue_DataExists_ReturnsFalse()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 1, 2, 3, 4 };
                int[] values = { 1, 2, 3, 4 };

                int[] upsertKeys = { 10, 20, 30, 40 };
                int[] upsertValues = { 10, 20, 30, 40 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    int index = 0;
                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(keys[index], cursorResult.Key);
                            Assert.Equal(values[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        cursor.ToFirst<int, int>(out var cursorResult);
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Upsert(upsertKeys[i], upsertValues[i]);
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;

                        while (cursor.ToNext(out CursorResult<int, int> cursorResult))
                        {
                            if (index < keys.Length)
                            {
                                Assert.Equal(cursorResult.Key, keys[index]);
                                Assert.Equal(cursorResult.Value, values[index]);
                            }
                            else
                            {
                                Assert.Equal(cursorResult.Key, upsertKeys[index - keys.Length]);
                                Assert.Equal(cursorResult.Value, upsertValues[index - keys.Length]);
                            }

                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorEraseByKey_DataExists_DataErased()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 1, 2, 3, 4 };
                int[] values = { 1, 2, 3, 4 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    int index = 0;
                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(keys[index], cursorResult.Key);
                            Assert.Equal(values[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.Erase(keys[1]));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                int[] eraseKeys = { 1, 3, 4 };
                int[] eraseValues = { 1, 3, 4 };

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(eraseKeys[index], cursorResult.Key);
                            Assert.Equal(eraseValues[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorEraseByKey_DataNotExists_DataErased()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 1, 2, 3, 4 };
                int[] values = { 1, 2, 3, 4 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    int index = 0;
                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(keys[index], cursorResult.Key);
                            Assert.Equal(values[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.Erase(keys[1]));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                int[] eraseKeys = { 1, 3, 4 };
                int[] eraseValues = { 1, 3, 4 };

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(eraseKeys[index], cursorResult.Key);
                            Assert.Equal(eraseValues[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorErase_DataExists_DataErased()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 1, 2, 3, 4 };
                int[] values = { 1, 2, 3, 4 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    int index = 0;
                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(keys[index], cursorResult.Key);
                            Assert.Equal(values[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.ToFirst<int, int>(out var firstCursorResult));
                        Assert.Equal(keys[0], firstCursorResult.Key);
                        Assert.Equal(values[0], firstCursorResult.Value);
                        Assert.True(cursor.Erase());

                        Assert.True(cursor.ToLast<int, int>(out var lastCursorResult));
                        Assert.Equal(keys[3], lastCursorResult.Key);
                        Assert.Equal(values[3], lastCursorResult.Value);
                        Assert.True(cursor.Erase());
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                int[] eraseKeys = { 2, 3 };
                int[] eraseValues = { 2, 3 };

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(eraseKeys[index], cursorResult.Key);
                            Assert.Equal(eraseValues[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Fact]
        public void CursorEraseByKeyValue_DataExists_DataErased()
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 1, 2, 3, 4 };
                int[] values = { 1, 2, 3, 4 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    int index = 0;
                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(keys[index], cursorResult.Key);
                            Assert.Equal(values[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.Erase(keys[0]));
                        Assert.True(cursor.Erase(keys[3]));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                int[] eraseKeys = { 2, 3 };
                int[] eraseValues = { 2, 3 };

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(eraseKeys[index], cursorResult.Key);
                            Assert.Equal(eraseValues[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(9, 1)]
        [InlineData(8, 116)]
        public void CursorEraseByKeyValue_DataNotExists_DataIsNotErased(int eraseKey, int eraseValue)
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 1, 2, 3, 4 };
                int[] values = { 1, 2, 3, 4 };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            cursor.Insert(keys[i], values[i]);
                        }
                    }

                    int index = 0;
                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(keys[index], cursorResult.Key);
                            Assert.Equal(values[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.False(cursor.EraseMulti(eraseKey, eraseValue));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;
                        while (cursor.ToNext<int, int>(out var cursorResult))
                        {
                            Assert.Equal(keys[index], cursorResult.Key);
                            Assert.Equal(values[index], cursorResult.Value);
                            index++;
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }

        [Theory]
        [InlineData(0, "111")]
        [InlineData(0, "1111")]
        [InlineData(2, "")]
        [InlineData(2, "222")]
        [InlineData(4, "4444")]
        public void CursorEraseMultiByKeyValue_DataNotExists_DataIsNotErased(int eraseKey, string eraseValue)
        {
            uint maxMaps = 5;
            uint maxReaders = 10;

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((long)Geometry.Size.MB * 10));
            CreateParameters createParameters = new CreateParameters(var);
            OperateParameters operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                int[] keys = { 0, 2, 3, 4 };

                Dictionary<int, List<string>> keyValues = new Dictionary<int, List<string>>
                {
                    { keys[0], new List<string>{"", "11", "111", "1111", "11111"} },
                    { keys[1], new List<string>{"", "22", "222", "2222", "22222"} },
                    { keys[2], new List<string>{"", "33", "333", "3333", "33333"} },
                    { keys[3], new List<string>{"", "44", "444", "4444", "44444"} }
                };

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        foreach (var keyValue in keyValues)
                        {
                            foreach (var value in keyValue.Value)
                            {
                                cursor.Upsert(keyValue.Key, value);
                            }
                        }
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;
                        CursorResult<int, string> cursorResult = default;

                        do
                        {
                            int i = 0;
                            if (index > 0)
                            {
                                Assert.Equal(cursorResult.Key, keys[index]);
                                Assert.Equal(cursorResult.Value, keyValues[keys[index]][i]);

                                i = 1;
                            }
                            while (cursor.ToCurrentNextMulti(out cursorResult, false))
                            {
                                Assert.Equal(cursorResult.Key, keys[index]);
                                Assert.Equal(cursorResult.Value, keyValues[keys[index]][i]);
                                i++;
                            }

                            index++;
                        }
                        while (cursor.ToNext(out cursorResult, false));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                using (ITxn trx = env.StartWrite())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        Assert.True(cursor.EraseMulti(eraseKey, eraseValue));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }

                Assert.True(keyValues[eraseKey].Remove(eraseValue));

                using (ITxn trx = env.StartRead())
                {
                    var testMap = trx.CreateMap("test", KeyMode.Usual, ValueMode.Multi);

                    using (var cursor = trx.OpenCursor(testMap))
                    {
                        int index = 0;
                        CursorResult<int, string> cursorResult = default;

                        do
                        {
                            int i = 0;
                            if (index > 0)
                            {
                                Assert.Equal(cursorResult.Key, keys[index]);
                                Assert.Equal(cursorResult.Value, keyValues[keys[index]][i]);

                                i = 1;
                            }
                            while (cursor.ToCurrentNextMulti(out cursorResult))
                            {
                                Assert.Equal(cursorResult.Key, keys[index]);
                                Assert.Equal(cursorResult.Value, keyValues[keys[index]][i]);
                                i++;
                            }

                            index++;
                        }
                        while (cursor.ToNext(out cursorResult));
                    }

                    trx.Commit();
                    env.CloseMap(testMap);
                }
            }
        }
    }
}
