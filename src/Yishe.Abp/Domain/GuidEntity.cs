using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yishe.Abp.Util.Help;

namespace Yishe.Abp.Domain
{
   
    [Serializable]
    public abstract class GuidEntity : Entity<Guid>, IGuidEntity
    {
        public GuidEntity()
        {
           
            Id = StringHelper.NewCombGuid();
        }
    }
}
