using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class CommentMap : EntityMap<Comment>
    {
        public CommentMap() : base("Posts")
        {
            Map(c=> c.Content);
            HasOne(c => c.Post);
        }
    }
}