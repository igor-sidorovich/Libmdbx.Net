using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Libmdbx.Net;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Xunit;

using static Libmdbx.Net.Bindings.Const;

namespace Libmdbx.Tests
{
    public class EnvTests : BaseTest
    {
        public EnvTests() : base("env_db_name")
        {
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void MaxMaps_EnvCreated_ReturnsMaxMaps(uint maxMaps)
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 500));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(maxMaps, 0, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                Assert.Equal(maxMaps, env.MaxMaps());
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(100)]
        public void MaxReaders_EnvCreated_ReturnsMaxReaders(uint maxReaders)
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Assert.Equal((uint)120, env.MaxReaders());
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Assert.Equal((uint)116, env.MaxReaders());
                }
            }
        }

        [Fact]
        public void GetFlags_DefaultOperateParameters_ReturnsFlags()
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, 0, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                Assert.Equal(EnvFlags.WriteMap | EnvFlags.Accede | EnvFlags.NoSubDir, env.GetFlags());
            }
        }

        [Fact]
        public void GetFlags_OrphanReadTransactionsTrue_ReturnsFlags()
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, 0, reclaiming: new ReclaimingOptions(lifo:false), options: new OperateOptions(orphanReadTransactions:true));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                Assert.Equal(EnvFlags.WriteMap | EnvFlags.Accede | EnvFlags.NoTls | EnvFlags.NoSubDir, env.GetFlags());
            }
        }

        [Fact]
        public void DbSizeMax_OrphanReadTransactionsTrue_ReturnsSize()
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, 0, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                var dbSizeMaxPtr = env.DbsizeMax();
                dynamic? result = null;

                if (IntPtr.Size == 4)
                {
                    result = dbSizeMaxPtr.ToInt32();
                }
                else if (IntPtr.Size == 8)
                {
                    result = dbSizeMaxPtr.ToInt64();
                }

                Assert.True(result > 0);
            }
        }

        [Fact]
        public void DbSizeMin_OrphanReadTransactionsTrue_ReturnsSize()
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, 0, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                var dbSizeMinPtr = env.DbsizeMin();
                dynamic? result = null;

                if (IntPtr.Size == 4)
                {
                    result = dbSizeMinPtr.ToInt32();
                }
                else if (IntPtr.Size == 8)
                {
                    result = dbSizeMinPtr.ToInt64();
                }

                Assert.True(result > 0);
            }
        }

        [Fact]
        public void IsEmpty_WithoutData_ReturnsTrue()
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, 0, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                Assert.True(env.IsEmpty());
            }
        }

        [Fact]
        public void IsEmpty_DataAdded_ReturnsFalse()
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(1, 1, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                Assert.True(env.IsEmpty());

                using (ITxn trx = env.StartWrite())
                {
                    MapHandle map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    var value = 1;
                    var key = 1;

                    trx.Insert(map, key, value);

                    trx.Commit();

                    Assert.False(env.IsEmpty());

                    env.CloseMap(map);
                }
            }
        }

        [Fact]
        public void CloseMap_TransactionAborted_Exception()
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(1, 1, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: false));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                Assert.True(env.IsEmpty());

                using (ITxn trx = env.StartWrite())
                {
                    MapHandle map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                    var value = 1;
                    var key = 1;

                    trx.Insert(map, key, value);

                    trx.Abort();

                    Assert.True(env.IsEmpty());

                    var exception = Assert.Throws<LibmdbxException>(() => env.CloseMap(map));
                    Assert.Equal(LibmdbxResultCodeFlag.BAD_DBI, exception.ErrorCode);
                }
            }
        }

        [Fact]
        public void Remove_EnvCreated_EnvRemoved()
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, 0, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                
            }

            Assert.True(_envFactory.Remove(path));
        }

        [Fact]
        public void Remove_EnvNotClosed_EnvRemoved()
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, 0, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var exception = Assert.Throws<LibmdbxException>(() => _envFactory.Remove(path));
                    Assert.Equal(EPIPE, (int)exception.ErrorCode);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Assert.True(_envFactory.Remove(path));
                }
            }
        }

        [Theory]
        [InlineData(uint.MaxValue)]
        public void EnvManaged_MaxValueOfReaders_Exception(uint maxReaders)
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, maxReaders, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            IEnv? env = null;
            try
            {
                Assert.Throws<ArgumentException>(() => env = new Env(path, createParameters, operateParameters));
            }
            finally
            {
                env?.Close();
            }
        }

        [Theory]
        [InlineData(uint.MaxValue)]
        public void EnvManaged_MaxValueOfMaps_Exception(uint maxMaps)
        {
            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(maxMaps, 0, reclaiming: new ReclaimingOptions(false), options: new OperateOptions(false));

            IEnv? env = null;
            try
            {
                Assert.Throws<ArgumentException>(() => env = new Env(path, createParameters, operateParameters));
            }
            finally
            {
                env?.Close();
            }
        }

        [Fact]
        public void Copy_Env_EnvCopied()
        {
            string pathToCopy = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "mdbx_copy");
            if (Directory.Exists(pathToCopy))
            {
                Assert.True(_envFactory.Remove(Path.Combine(pathToCopy, "mdbx-test.db-Copy")));
            }
            Directory.CreateDirectory(pathToCopy);
            Assert.True(Directory.GetFiles(pathToCopy).Length == 0);

            var var = Geometry.make_dynamic(Geometry.MinimalValue, new IntPtr((int)Geometry.Size.MB * 1));
            var createParameters = new CreateParameters(var);
            var operateParameters = new OperateParameters(0, 0, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: true));

            using (IEnv env = new Env(path, createParameters, operateParameters))
            {
                env.Copy(Path.Combine(pathToCopy, "mdbx-test.db-Copy"), false, false);
                Assert.True(Directory.GetFiles(pathToCopy).Length > 0);
            }
        }
    }
}
