using System;
using Libmdbx.Net.Core.Common;
using Libmdbx.Net.Core.Transaction;

namespace Libmdbx.Net.Core.Env
{
    public interface IEnv : IDisposable
    {
        IntPtr EnvPtr { get; }
        bool Closed { get; }
        OperateOptions GetOptions();
        void CloseMap(MapHandle handle);
        UIntPtr GetPageSize();
        MdbxDbStat GetStat();
        IntPtr LimitsDbsizeMax(UIntPtr pagesize);
        bool Remove(string pathname, RemoveMode mode = RemoveMode.JustRemove);
        bool IsEmpty();
        IntPtr LimitsDbsizeMin(UIntPtr pagesize);
        IntPtr DbsizeMax();
        IntPtr DbsizeMin();
        IntPtr DefaultPageSize();
        string GetPath();
        uint MaxMaps();
        uint MaxReaders();
        IEnv Copy(string destination, bool compactify, bool force_dynamic_size);
        IEnv SetGeometry(Geometry geo);
        bool SyncToDisk(bool force = true, bool nonblock = false);
        EnvFlags GetFlags();
        ITxn StartWrite(bool dontWait = false);
        ITxn TryStartWrite();
        ITxn StartRead();
        void Close(bool dontSync = false);
    }
}
