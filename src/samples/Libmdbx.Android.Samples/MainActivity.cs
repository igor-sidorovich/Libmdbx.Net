using System;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using Android.App;
using Android.Content.PM;
using Android.Drm;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Libmdbx.Net.Shared;

namespace Libmdbx.Android.Samples
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private IEnvFactory _envFactory;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            _envFactory = new EnvFactory();
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            if (fab != null)
            {
                fab.Click += FabOnClick;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();

            InsertExample();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
