using System.Threading.Tasks;

namespace Abp.BackgroundJobs
{
    public interface IAsyncBackgroundJob<in TArgs>: IBackgroundJobBase<TArgs>
    {
        /// <summary>
        /// Executes the job with the <paramref name="args"/>.
        /// </summary>
        /// <param name="args">Job arguments.</param>
        Task ExecuteAsync(TArgs args);
    }
}