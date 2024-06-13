using System;
using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory.Nhibernate;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class PostMap : EntityMap<NhPost, Guid>
    {
        public PostMap() : base("Posts")
        {
            References(x => x.Blog)
                .Column("BlogId")
                .Cascade.All();
            
            HasMany(f => f.Comments);

            Map(x => x.Title);
            Map(x => x.Body);
            Map(x => x.TenantId);
            
            
            this.MapIsDeleted();
            this.MapAudited();
        }
    }
}