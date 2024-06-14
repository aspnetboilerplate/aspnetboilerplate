using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.TPH.EFCore
{
    public class CitizenshipInformation : Entity
    {
        public string CitizenShipId { get; set; }

        [Required] public Student Student { get; set; }
    }
}