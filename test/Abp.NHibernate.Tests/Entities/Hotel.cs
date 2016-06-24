using System;
using Abp.Domain.Entities;

namespace Abp.NHibernate.Tests.Entities
{
    public class Hotel : Entity
    {
        public virtual string Name { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual DateTime? ModificationDate { get; set; }

        public virtual Location Headquarter { get; set; }
    }
}