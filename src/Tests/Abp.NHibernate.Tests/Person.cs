using Abp.Domain.Entities;
using System;

namespace Abp.NHibernate.Tests
{
    public class Person : Entity<Guid>
    {
        public const int MaxNameLength = 64;

        public virtual string Name { get; set; }
    }
}