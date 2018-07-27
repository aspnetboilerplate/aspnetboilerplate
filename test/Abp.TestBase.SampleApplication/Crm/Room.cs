using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Abp.TestBase.SampleApplication.Crm
{
    [Table("Rooms")]
    public class Room : Entity, IHasCreationTime
    {
        public int HotelId { get; set; }

        public string Name { get; set; }

        public int Capacity { get; set; }

        [DisableDateTimeNormalization]
        public DateTime CreationTime { get; set; }

        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; }

        public Room()
        {
            
        }
    }
}