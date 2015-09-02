using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.TestBase.SampleApplication
{
    [Table("Messages")]
    public class Message : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public string Text { get; set; }
    }
}
