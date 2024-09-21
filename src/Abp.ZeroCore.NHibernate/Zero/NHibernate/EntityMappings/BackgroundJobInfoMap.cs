using Abp.BackgroundJobs;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class BackgroundJobInfoMap : EntityMap<BackgroundJobInfo, long>
{
    public BackgroundJobInfoMap()
        : base("AbpBackgroundJobs")
    {
        this.MapCreationAudited();
        Map(x => x.IsAbandoned)
            .Not.Nullable();
        Map(x => x.JobArgs)
            .Length(BackgroundJobInfo.MaxJobArgsLength)
            .Not.Nullable();
        Map(x => x.JobType)
            .Length(BackgroundJobInfo.MaxJobTypeLength)
            .Not.Nullable();
        Map(x => x.LastTryTime);
        Map(x => x.NextTryTime)
            .Not.Nullable();
        Map(x => x.Priority)
            .CustomType<BackgroundJobPriority>()
            .Not.Nullable();
        Map(x => x.TryCount)
            .Not.Nullable();
    }
}