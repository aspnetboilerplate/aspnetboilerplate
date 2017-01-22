using System;
using Abp.Domain.Values;

namespace Abp.Tests.Domain.Values
{
    public class Address : ValueObject<Address>
    {
        public Guid CityId { get; }

        public string Street { get; }

        public int Number { get; }

        public Address(
            Guid cityId,
            string street,
            int number)
        {
            CityId = cityId;
            Street = street;
            Number = number;
        }
    }

    public class Address2 : ValueObject<Address2>
    {
        public Guid? CityId { get; }

        public string Street { get; }

        public int Number { get; }

        public Address2(
            Guid? cityId,
            string street,
            int number)
        {
            CityId = cityId;
            Street = street;
            Number = number;
        }
    }
}