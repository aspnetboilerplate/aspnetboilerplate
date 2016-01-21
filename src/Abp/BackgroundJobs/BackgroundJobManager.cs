using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Serialization;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;

namespace Abp.BackgroundJobs
{
    public class BackgroundJobManager : BackgroundWorkerBase, IBackgroundJobManager
    {
        private readonly IIocResolver _iocResolver;
        private readonly IBackgroundJobStore _store;
        private readonly AbpTimer _timer;

        public BackgroundJobManager(
            IIocResolver iocResolver,
            IBackgroundJobStore store, 
            AbpTimer timer)
        {
            _store = store;
            _timer = timer;
            _iocResolver = iocResolver;

            _timer.Period = 5000; //5 seconds - TODO: Make configurable?
            _timer.Elapsed += Timer_Elapsed;
        }

        public override void Start()
        {
            base.Start();
            _timer.Start();
        }

        public override void Stop()
        {
            _timer.Stop();
            base.Stop();
        }

        public override void WaitToStop()
        {
            _timer.WaitToStop();
            base.WaitToStop();
        }

        public async Task EnqueueAsync<TJob, TArgs>(TArgs state, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : IBackgroundJob<TArgs>
        {
            var jobInfo = new BackgroundJobInfo
            {
                JobType = typeof(TJob).AssemblyQualifiedName,
                State = BinarySerializationHelper.Serialize(state),
                Priority = priority
            };

            if (delay.HasValue)
            {
                jobInfo.NextTryTime = Clock.Now.Add(delay.Value);
            }

            await _store.InsertAsync(jobInfo);
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                var waitingJobs = AsyncHelper.RunSync(() => _store.GetWaitingJobsAsync(1000));
                foreach (var job in waitingJobs)
                {
                    TryProcessJob(job);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        private void TryProcessJob(BackgroundJobInfo jobInfo)
        {
            try
            {
                jobInfo.TryCount++;
                jobInfo.LastTryTime = DateTime.Now;

                var jobType = Type.GetType(jobInfo.JobType);
                using (var job = _iocResolver.ResolveAsDisposable(jobType))
                {
                    var stateObj = BinarySerializationHelper.DeserializeExtended(jobInfo.State);

                    try
                    {
                        job.Object.GetType().GetMethod("Execute").Invoke(job, new[] {stateObj});
                        AsyncHelper.RunSync(() => _store.DeleteAsync(jobInfo));
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex.Message, ex);

                        var nextTryTime = jobInfo.CalculateNextTryTime();
                        if (nextTryTime.HasValue)
                        {
                            jobInfo.NextTryTime = nextTryTime.Value;
                        }
                        else
                        {
                            jobInfo.IsAbandoned = true;
                        }

                        try
                        {
                            _store.UpdateAsync(jobInfo);
                        }
                        catch (Exception updateEx)
                        {
                            Logger.Warn(updateEx.ToString(), updateEx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);

                jobInfo.IsAbandoned = true;

                try
                {
                    _store.UpdateAsync(jobInfo);
                }
                catch (Exception updateEx)
                {
                    Logger.Warn(updateEx.ToString(), updateEx);
                }
            }
        }
    }
}
