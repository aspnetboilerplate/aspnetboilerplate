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

        public async Task<BackgroundJobInfo> GetAsync(long jobId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _backgroundJobRepository.GetAsync(jobId)
            );
        }

        public BackgroundJobInfo Get(long jobId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
                _backgroundJobRepository.Get(jobId)
            );
        }

        public async Task InsertAsync(BackgroundJobInfo jobInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _backgroundJobRepository.InsertAsync(jobInfo)
            );
        }

        public void Insert(BackgroundJobInfo jobInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _backgroundJobRepository.Insert(jobInfo);
            });
        }

        public virtual async Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount)
        {
            var waitingJobs = _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _backgroundJobRepository.GetAll()
                    .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                    .OrderByDescending(t => t.Priority)
                    .ThenBy(t => t.TryCount)
                    .ThenBy(t => t.NextTryTime)
                    .Take(maxResultCount)
                    .ToList();
            });

            return await Task.FromResult(waitingJobs);
        }

        public virtual List<BackgroundJobInfo> GetWaitingJobs(int maxResultCount)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _backgroundJobRepository.GetAll()
                    .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                    .OrderByDescending(t => t.Priority)
                    .ThenBy(t => t.TryCount)
                    .ThenBy(t => t.NextTryTime)
                    .Take(maxResultCount)
                    .ToList();
            });
        }

        public async Task DeleteAsync(BackgroundJobInfo jobInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _backgroundJobRepository.DeleteAsync(jobInfo)
            );
        }

        public void Delete(BackgroundJobInfo jobInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _backgroundJobRepository.Delete(jobInfo);
            });
        }

        public async Task UpdateAsync(BackgroundJobInfo jobInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _backgroundJobRepository.UpdateAsync(jobInfo)
            );
        }

        public void Update(BackgroundJobInfo jobInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _backgroundJobRepository.Update(jobInfo);
            });
        }
    }
}
