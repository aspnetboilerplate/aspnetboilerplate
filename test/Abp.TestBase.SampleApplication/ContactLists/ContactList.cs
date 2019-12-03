using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.TestBase.SampleApplication.People;

namespace Abp.TestBase.SampleApplication.ContactLists
{
    [Table("ContactLists")]
    public class ContactList : Entity, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }

        public virtual string Name { get; set; }

        [ForeignKey("ContactListId")]
        public virtual ICollection<Person> People { get; set; }
    }
}
