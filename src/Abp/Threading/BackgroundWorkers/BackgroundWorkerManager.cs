using System;
using System.Collections.Generic;
using Abp.Dependency;

namespace Abp.Threading.BackgroundWorkers
{
    /// <summary>
    ///     Implements <see cref="IBackgroundWorkerManager" />.
    /// </summary>
    public class BackgroundWorkerManager : RunnableBase, IBackgroundWorkerManager, ISingletonDependency, IDisposable
    {
        private readonly List<IBackgroundWorker> _backgroundJobs;
        private readonly IIocResolver _iocResolver;

        private bool _isDisposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BackgroundWorkerManager" /> class.
        /// </summary>
        public BackgroundWorkerManager(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
            _backgroundJobs = new List<IBackgroundWorker>();
        }

        public override void Start()
        {
            base.Start();

            _backgroundJobs.ForEach(job => job.Start());
        }

        public override void Stop()
        {
            _backgroundJobs.ForEach(job => job.Stop());

            base.Stop();
        }

        public override void WaitToStop()
        {
            _backgroundJobs.ForEach(job => job.WaitToStop());

            base.WaitToStop();
        }

        public void Add(IBackgroundWorker worker)
        {
            _backgroundJobs.Add(worker);

            if (IsRunning)
            {
                worker.Start();
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            _backgroundJobs.ForEach(_iocResolver.Release);
            _backgroundJobs.Clear();
        }
    }
}