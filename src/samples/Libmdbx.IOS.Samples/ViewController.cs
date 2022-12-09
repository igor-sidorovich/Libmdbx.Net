using Foundation;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UIKit;
using Libmdbx.Net;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Libmdbx.Net.Shared;

namespace Libmdbx.IOS.Samples
{
    public partial class ViewController : UIViewController
    {
        private IEnvFactory _envFactory;

        public ViewController (IntPtr handle) : base (handle)
        {
            _envFactory = new EnvFactory();
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.

            UIButton myButton = new UIButton(UIButtonType.System);
            myButton.Frame = new CoreGraphics.CGRect(25, 25, 300, 150);
            myButton.SetTitle("Test", UIControlState.Normal);
            myButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            myButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;

            myButton.TouchUpInside += MyButton_TouchUpInside;

            View.AddSubview(myButton);
        }

        private void MyButton_TouchUpInside(object sender, EventArgs e)
        {
            InsertExample();
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
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