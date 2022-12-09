using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Libmdbx.Net;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Env;
using Libmdbx.Net.Core.Transaction;
using Libmdbx.Net.Shared;
using Geometry = Libmdbx.Net.Core.Env.Geometry;

namespace Libmdbx.UWP.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IEnvFactory _envFactory;

        public MainPage()
        {
            _envFactory = new EnvFactory();
            this.InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            InsertExample();
        }

        private void InsertExample()
        {
            var path = GetPath();

            var lowerSize = Geometry.MinimalValue;
            var upperSize = new IntPtr((int)Geometry.Size.MB * 100);

            var geometry = Geometry.make_dynamic(lowerSize, upperSize);
            var createParameters = new CreateParameters(geometry);

            uint maxMaps = 10;
            uint maxReaders = 10;

            var operateParameters = new OperateParameters(maxMaps, maxReaders, reclaiming: new ReclaimingOptions(lifo: false), options: new OperateOptions(orphanReadTransactions: false, exclusive: false));

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

        private string GetPath()
        {
            var personal = ApplicationData.Current.LocalFolder.Path;
            string path = Path.Combine(personal, "mdbx_db");
            if (Directory.Exists(path))
            {
                Env.Remove(Path.Combine(path, "mdbx_test.db"));
            }
            Directory.CreateDirectory(path);

            return Path.Combine(path, "mdbx_test.db");
        }
    }
}
