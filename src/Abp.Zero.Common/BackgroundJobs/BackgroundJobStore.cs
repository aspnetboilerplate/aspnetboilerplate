using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Timing;

namespace Abp.BackgroundJobs
{
    /// <summary>
    /// Implements <see cref="IBackgroundJobStore"/> using repositories.
    /// </summary>
    public class BackgroundJobStore : IBackgroundJobStore, ITransientDependency
    {
        private readonly IRepository<BackgroundJobInfo, long> _backgroundJobRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public BackgroundJobStore(
            IRepository<BackgroundJobInfo, long> backgroundJobRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _backgroundJobRepository = backgroundJobRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public Task<BackgroundJobInfo> GetAsync(long jobId)
        {
            return _backgroundJobRepository.GetAsync(jobId);
        }

        public BackgroundJobInfo Get(long jobId)
        {
            return _backgroundJobRepository.Get(jobId);
        }

        public Task InsertAsync(BackgroundJobInfo jobInfo)
        {
            return _backgroundJobRepository.InsertAsync(jobInfo);
        }

        public void Insert(BackgroundJobInfo jobInfo)
        {
            _backgroundJobRepository.Insert(jobInfo);
        }

        public virtual async Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount)
        {
            List<BackgroundJobInfo> result;

            using (var uow = _unitOfWorkManager.Begin())
            {
                var waitingJobs = _backgroundJobRepository.GetAll()
                    .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                    .OrderByDescending(t => t.Priority)
                    .ThenBy(t => t.TryCount)
                    .ThenBy(t => t.NextTryTime)
                    .Take(maxResultCount)
                    .ToList();

                result = waitingJobs;
                await uow.CompleteAsync();
            }

            return result;
        }

        public virtual List<BackgroundJobInfo> GetWaitingJobs(int maxResultCount)
        {
            List<BackgroundJobInfo> result;

            using (var uow = _unitOfWorkManager.Begin())
            {
                var waitingJobs = _backgroundJobRepository.GetAll()
                    .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                    .OrderByDescending(t => t.Priority)
                    .ThenBy(t => t.TryCount)
                    .ThenBy(t => t.NextTryTime)
                    .Take(maxResultCount)
                    .ToList();

                result = waitingJobs;
                uow.Complete();
            }

            return result;
        }

        public Task DeleteAsync(BackgroundJobInfo jobInfo)
        {
            return _backgroundJobRepository.DeleteAsync(jobInfo);
        }

        public void Delete(BackgroundJobInfo jobInfo)
        {
            _backgroundJobRepository.Delete(jobInfo);
        }

        public Task UpdateAsync(BackgroundJobInfo jobInfo)
        {
            return _backgroundJobRepository.UpdateAsync(jobInfo);
        }

        public void Update(BackgroundJobInfo jobInfo)
        {
            _backgroundJobRepository.Update(jobInfo);
        }
    }
}
