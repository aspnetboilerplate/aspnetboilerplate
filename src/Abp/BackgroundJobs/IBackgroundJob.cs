namespace Abp.BackgroundJobs
{
    public interface IBackgroundJob
    {
        void Execute(object state);
    }
}