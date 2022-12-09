using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Libmdbx.Tests
{
    public class ThreadSyncContext : SynchronizationContext
    {
        protected interface IWorkItem
        {
            void Execute();
        }

        protected sealed class WorkItem : IWorkItem
        {
            private readonly SendOrPostCallback _callback;
            private readonly object _state;

            public WorkItem(SendOrPostCallback callback, object state)
            {
                _callback = callback ?? throw new ArgumentNullException(nameof(callback));
                _state = state;
            }

            public void Execute()
            {
                _callback(_state);
            }
        }

        protected class WaitableWorkItem : IWorkItem, IDisposable
        {
            private readonly SendOrPostCallback _callback;
            private readonly object _state;
            private readonly ManualResetEventSlim _reset;

            private ExceptionDispatchInfo _exceptionDispatchInfo;

            public WaitableWorkItem(SendOrPostCallback callback, object state)
            {
                _callback = callback ?? throw new ArgumentNullException(nameof(callback));
                _state = state;
                _reset = new ManualResetEventSlim();
            }

            public void Execute()
            {
                try
                {
                    _callback(_state);
                }
                catch (Exception e)
                {
                    _exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
                }
                _reset.Set();
            }

            public void Wait()
            {
                _reset.Wait();
                if (_exceptionDispatchInfo != null)
                    _exceptionDispatchInfo.Throw();
            }

            public void Dispose()
            {
                _reset.Dispose();
            }
        }

        private readonly Queue<IWorkItem> _executedItems = new Queue<IWorkItem>(10);
        private readonly ConcurrentQueue<IWorkItem> _workItems = new ConcurrentQueue<IWorkItem>();
        private int _executingThreadId;

        public ThreadSyncContext()
        {
            AssignThread();
        }

        public void AssignThread()
        {
            _executingThreadId = Environment.CurrentManagedThreadId;
        }

        private void VerifyThread()
        {
            if (_executingThreadId != Environment.CurrentManagedThreadId)
                throw new InvalidOperationException("Invalid thread");
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }

        private void ProcessQueue(WorkItem breakingItem)
        {
            IWorkItem executedWorkItem;
            do
            {
                executedWorkItem = Dequeue();
                if (executedWorkItem != null)
                    _executedItems.Enqueue(executedWorkItem);

            } while (executedWorkItem != breakingItem);

            while (_executedItems.Count > 0)
            {
                executedWorkItem = _executedItems.Dequeue();
                executedWorkItem.Execute();
            }
        }

        private IWorkItem Dequeue()
        {
            IWorkItem currentItem;
            _workItems.TryDequeue(out currentItem);
            return currentItem;
        }

        protected virtual void Enqueue(IWorkItem workItem)
        {
            _workItems.Enqueue(workItem);
        }

        public void Process()
        {
            VerifyThread();
            ProcessQueue(null);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            Enqueue(new WorkItem(d, state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            if (_executingThreadId < 0)
                throw new InvalidOperationException("working thread not initialized");

            if (Environment.CurrentManagedThreadId == _executingThreadId)
            {
                WorkItem workItem = new WorkItem(d, state);
                Enqueue(workItem);
                ProcessQueue(workItem);
            }
            else
            {
                using (WaitableWorkItem workItem = new WaitableWorkItem(d, state))
                {
                    Enqueue(workItem);
                    workItem.Wait();
                }
            }
        }
    }

    public class ManualSetSynchronizationContext : ThreadSyncContext, IDisposable
    {
        private readonly AutoResetEvent _event;

        private bool _isDisposed;

        public ManualSetSynchronizationContext()
        {
            _event = new AutoResetEvent(false);
        }

        public void Set()
        {
            if (!_isDisposed)
                _event.Set();
        }

        public void Wait()
        {
            if (!_isDisposed)
                _event.WaitOne();
        }

        protected override void Enqueue(IWorkItem workItem)
        {
            if (!_isDisposed)
                base.Enqueue(workItem);
        }

        public void Dispose()
        {
            _isDisposed = true;
            _event.Dispose();
        }
    }

    public class AutoSetSynchronizationContext : ManualSetSynchronizationContext
    {
        protected override void Enqueue(IWorkItem workItem)
        {
            base.Enqueue(workItem);
            Set();
        }
    }
}
