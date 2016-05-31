using System;

namespace Abp.NHibernate.Tests.Entities
{
    public class Location
    {
        public virtual DateTime CreationDate { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime? ModificationDate { get; set; }
    }
}