using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adorable.Domain.Entities;

namespace Adorable.TestBase.SampleApplication
{
    [Table("Messages")]
    public class Message : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public string Text { get; set; }
    }
}
