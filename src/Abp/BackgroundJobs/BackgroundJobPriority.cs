namespace Abp.BackgroundJobs
{
    public enum BackgroundJobPriority : byte
    {
        Low = 5,
        BelowNormal = 10,
        Normal = 15,
        AboveNormal = 20,
        High = 25
    }
}