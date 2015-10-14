using Abp.NHibernate.EntityMappings;
using MyProject.News;

namespace MyProject.NHibernate.EntityMappings
{

    public class NewsMap : EntityMap<News.News,long>
    {
        public NewsMap()
            : base("News")
        {
            Map(x => x.TenantId);
            Map(x => x.Title);
            Map(x => x.Intro);
            Map(x => x.Content);
            Map(x => x.CreationTime);
            Map(x => x.CreatorUserId);
            Map(x => x.LastModificationTime);
            Map(x => x.LastModifierUserId);
            Map(x => x.IsDeleted);
            Map(x => x.DeleterUserId);
            Map(x => x.DeletionTime);
            Map(x => x.IsActive);
        }
    }
}
