using System;
using AppKit;
using Foundation;
using System.IO;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Libmdbx.Net.Shared;

namespace Libmdbx.Mac.Samples
{
	public partial class ViewController : NSViewController
	{
        private IEnvFactory _envFactory;
        
		public ViewController (IntPtr handle) : base (handle)
		{
            _envFactory = new EnvFactory();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.
		}

		partial void TestClicked(AppKit.NSButton sender)
        {
            InsertExample();
        }

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}

        public void InsertExample()
        {
            var path = GetPath();

            var lowerSize = Geometry.MinimalValue;
            var upperSize = new IntPtr((int)Geometry.Size.MB * 10);

            var geometry = Geometry.make_dynamic(lowerSize, upperSize);
            var createParameters = new CreateParameters(geometry);

            uint maxMaps = 100;
            uint maxReaders = 100;

            var operateParameters =
                new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: false, exclusive: false));

            try
            {
                using (IEnv env = _envFactory.Create(path, createParameters, operateParameters))
                {
                    try
                    {
                        using (ITxn trx = env.StartRead())
                        {
                            var map = trx.OpenMap("test", KeyMode.Usual, ValueMode.Single);

                            for (int i = 0; i < 10; i++)
                            {
                                var value = trx.TryGet<int, int>(map, i, out int result);
                            }

                            trx.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    using (ITxn trx = env.StartWrite())
                    {
                        var map = trx.CreateMap("test", KeyMode.Usual, ValueMode.Single);

                        for (int i = 0; i < 10; i++)
                        {
                            trx.Upsert(map, i, i);
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

                using (IEnv env = _envFactory.Create(path, createParameters, operateParameters))
                {
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string GetPath()
        {
            string dbName = "mdbx_test.db";
            string dbFolder = "Databases";

            var dbDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbFolder);
            var dbFile = Path.Combine(dbDirectory, dbName);

            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }

            if (!File.Exists(Path.Combine(dbFile)))
            {
                File.Create(dbFile).Dispose();
            }

            return Path.Combine(dbFile);
        }
    }
}
