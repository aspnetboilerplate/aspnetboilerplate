using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Abp.Timing;

namespace Abp.TestBase.SampleApplication.Crm
{
    [Table("Hotels")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class Hotel: Entity, IHasCreationTime
    {
        public string Name { get; set; }

        [DisableDateTimeNormalization]
        public virtual Address BillingAddress { get; set; }

        public virtual Location Location { get; set; }

        [ForeignKey("HotelId")]
        public virtual ICollection<Room> Rooms { get; set; }

        [DisableDateTimeNormalization]
        public DateTime CreationTime { get; set; }

        public Hotel()
        {
            
        }
    }
}