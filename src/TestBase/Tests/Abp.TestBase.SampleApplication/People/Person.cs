using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.TestBase.SampleApplication.People
{
    [Table("People")]
    public class Person : Entity, ISoftDelete
    {
        public const int MaxNameLength = 64;

        [Required]
        [MaxLength(MaxNameLength)]
        public virtual string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}
