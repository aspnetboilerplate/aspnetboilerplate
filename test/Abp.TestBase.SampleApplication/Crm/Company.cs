using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.TestBase.SampleApplication.Crm
{
    [Table("Companies")]
    public class Company : Entity, IHasCreationTime
    {
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }

        public Address ShippingAddress { get; set; }

        public Address BillingAddress { get; set; }

        [ForeignKey("CompanyId")]
        public virtual ICollection<Branch> Branches { get; set; }
    }
}
