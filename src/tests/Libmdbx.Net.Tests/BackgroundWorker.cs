using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Libmdbx.Tests
{
    public class BackgroundWorker : IDisposable
    {
        public readonly AutoSetSynchronizationContext SynchronizationContext;
        private Task _task;
        private CancellationTokenSource _tokenSource;

        public BackgroundWorker(AutoSetSynchronizationContext context)
        {
            SynchronizationContext = context;
        }

        public Action ThreadStartedCallback;
        public Action ThreadStoppedCallback;
        public Action ThreadCallback;

        public void Start()
        {
            if (_task == null)
            {
                using (AutoResetEvent ev = new AutoResetEvent(false))
                {
                    _tokenSource = new CancellationTokenSource();
                    _task = Task.Factory.StartNew(() => Run(_tokenSource.Token, ev), CancellationToken.None,
                        TaskCreationOptions.LongRunning |
                        TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                    ev.WaitOne();
                }
            }
        }

        public void Stop()
        {
            if (_task != null)
            {
                _tokenSource.Cancel();
                SynchronizationContext.Set();
                _tokenSource.Dispose();
                _task = null;
            }
        }

        private void Run(CancellationToken token, AutoResetEvent ev)
        {
            System.Threading.SynchronizationContext.SetSynchronizationContext(SynchronizationContext);

            SynchronizationContext.AssignThread();

            if (ThreadStartedCallback != null)
                ThreadStartedCallback();

            ev.Set();

            while (!token.IsCancellationRequested)
            {
                SynchronizationContext.Wait();
                try
                {
                    if (!token.IsCancellationRequested)
                    {
                        SynchronizationContext.Process();

                        if (ThreadCallback != null)
                            ThreadCallback();
                    }
                }
                catch (Exception e)
                {
                    ExceptionDispatchInfo info = ExceptionDispatchInfo.Capture(e);
                }
            }

            if (ThreadStoppedCallback != null)
                ThreadStoppedCallback();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
