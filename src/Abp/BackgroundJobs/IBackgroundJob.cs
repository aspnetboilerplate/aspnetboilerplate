namespace Abp.BackgroundJobs
{
    /// <summary>
    /// Defines interface of a background job.
    /// </summary>
    public interface IBackgroundJob
    {
        /// <summary>
        /// Executes the job with the <see cref="state"/>.
        /// </summary>
        /// <param name="state">The job state.</param>
        void Execute(object state);
    }
}