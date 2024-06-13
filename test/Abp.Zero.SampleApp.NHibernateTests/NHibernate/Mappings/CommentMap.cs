using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory.Nhibernate;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class CommentMap : EntityMap<NhComment>
    {
        public CommentMap() : base("Posts")
        {
            Map(c=> c.Content);
            HasOne(c => c.Post);
        }
    }
}