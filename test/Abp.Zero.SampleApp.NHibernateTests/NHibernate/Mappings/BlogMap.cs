using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory;
using NHibernate.Mapping;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class BlogMap : EntityMap<Blog>
    {
        public BlogMap() : base("BlogMaps")
        {
            Map(f => f.Name);
            Map(f => f.Url);
            HasMany(f => f.Posts);
            
            Component(x => x.More, m =>
            {
                m.Map(x => x.BloggerName);
            });
            
            this.MapCreationTime();
        }
    }
}