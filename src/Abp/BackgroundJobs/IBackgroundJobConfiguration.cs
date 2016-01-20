namespace Abp.BackgroundJobs
{
    public interface IBackgroundJobConfiguration
    {
        /// <summary>
        /// Used to enable/disable background job execution.
        /// </summary>
        bool IsEnabled { get; set; }
    }
}