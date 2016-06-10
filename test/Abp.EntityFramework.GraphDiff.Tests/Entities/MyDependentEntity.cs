using Abp.Domain.Entities;

namespace Abp.EntityFramework.GraphDIff.Tests.Entities
{
    public class MyDependentEntity : Entity
    {
        public virtual MyMainEntity MyMainEntity { get; set; }
    }
}
