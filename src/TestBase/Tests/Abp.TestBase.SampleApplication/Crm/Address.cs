using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Abp.TestBase.SampleApplication.Crm
{
    [ComplexType]
    public class Address : IHasCreationTime
    {
        public string Country { get; set; }

        public string City { get; set; }

        public string FullAddress { get; set; }

        public DateTime CreationTime { get; set; }

        public Modifier LastModifier { get; set; }
    }

    [ComplexType]
    public class Modifier
    {
        public string Name { get; set; }

        public DateTime? ModificationTime { get; set; }
    }
}