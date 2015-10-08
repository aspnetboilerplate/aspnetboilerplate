using Abp.NHibernate.EntityMappings;
using MyProject.News;

namespace MyProject.NHibernate.EntityMappings
{

    public class NewsMap : EntityMap<News.News,long>
    {
        public NewsMap()
            : base("News")
        {
            Map(x => x.Title);
            Map(x=>x.Intro);
            Map(x=>x.Content);
        }
    }
}
