using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Abp.Dapper.Tests.Entities
{
    [Table("Goods")]
    public class Good : FullAuditedEntity
    {
        public string Name { get; set; }

        public int ParentId { get; set; }
    }
}