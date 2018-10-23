using Abp.Domain.Entities.Auditing;

namespace Abp.NHibernate.Tests.Entities
{
    public class Book : FullAuditedEntity
    {
        public virtual string Name { get; set; }
    }
}