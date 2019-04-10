using System;
using System.Collections.Generic;
using Abp.Domain.Values;

namespace Abp.Tests.Domain.Values
{
    public class Address : ValueObject
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

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Street;
            yield return CityId;
            yield return Number;
        }
    }

    public class Address2 : ValueObject
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

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Street;
            yield return CityId;
            yield return Number;
        }
    }

    public class Address3 : ValueObject
    {
        [IgnoreOnCompare]
        public Guid? CityId { get; }

        public string Street { get; }

        public int Number { get; }

        public Address3(
            Guid? cityId,
            string street,
            int number)
        {
            CityId = cityId;
            Street = street;
            Number = number;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Street;
            yield return Number;
        }
    }
}