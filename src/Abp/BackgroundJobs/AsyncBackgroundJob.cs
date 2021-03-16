using System.Threading.Tasks;

namespace Abp.BackgroundJobs
{
    public abstract class AsyncBackgroundJob<TArgs> : BackgroundJobBase<TArgs>, IAsyncBackgroundJob<TArgs>
    {
        public abstract Task ExecuteAsync(TArgs args);
    }
}