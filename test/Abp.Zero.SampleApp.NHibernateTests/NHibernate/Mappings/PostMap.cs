using System;
using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class PostMap : EntityMap<Post, Guid>
    {
        public PostMap() : base("Posts")
        {
            Map(p => p.BlogId);
            Map(p => p.Title);
            Map(p => p.Body);
            Map(x => x.TenantId).Nullable();
         
            HasOne(p => p.Blog).ForeignKey("BlogId");
            HasMany(p => p.Comments);
            
            this.MapIsDeleted();
            this.MapAudited();
        }
    }
}