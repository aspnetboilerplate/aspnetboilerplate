using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.TPH.NHibernate
{
    public class NhCitizenshipInformation : Entity
    {
        public virtual string CitizenShipId { get; set; }

        [Required]
        public virtual NhStudent Student { get; set; }
    }
}
