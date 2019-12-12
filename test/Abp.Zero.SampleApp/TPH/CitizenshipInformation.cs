using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.TPH
{
    public class CitizenshipInformation : Entity
    {
        public string CitizenShipId { get; set; }

        [Required]
        public Student Student { get; set; }
    }
}
