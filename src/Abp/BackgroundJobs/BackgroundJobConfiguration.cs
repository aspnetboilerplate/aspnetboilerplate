namespace Abp.BackgroundJobs
{
    internal class BackgroundJobConfiguration : IBackgroundJobConfiguration
    {
        public bool IsEnabled { get; set; }

        public BackgroundJobConfiguration()
        {
            IsEnabled = true;
        }
    }
}