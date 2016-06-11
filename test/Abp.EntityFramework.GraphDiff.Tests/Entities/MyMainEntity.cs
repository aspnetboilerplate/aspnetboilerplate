using System.Collections.Generic;
using Abp.Domain.Entities;

namespace Abp.EntityFramework.GraphDIff.Tests.Entities
{
    public class MyMainEntity : Entity
    {
        public virtual ICollection<MyDependentEntity> MyDependentEntities { get; set; }
    }
}