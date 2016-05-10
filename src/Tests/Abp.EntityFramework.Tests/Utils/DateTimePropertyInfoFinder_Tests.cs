using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Abp.EntityFramework.Utils;
using Shouldly;
using Xunit;

namespace Abp.EntityFramework.Tests.Utils
{
    public class DateTimePropertyInfoFinder_Tests
    {
        public class Hotel
        {
            public string Name { get; set; }

            public DateTime CreationDate { get; set; }

            public DateTime? ModificationDate { get; set; }

            public Location RealLocation { get; set; }

            public Location VirtualLocation { get; set; }

            public Owner Owner { get; set; }
        }

        [ComplexType]
        public class Location
        {
            public string Name { get; set; }

            public DateTime CreationDate { get; set; }

            public DateTime? Modification { get; set; }

            public Country Country { get; set; }
        }

        public class Owner
        {
            public string Name { get; set; }

            public DateTime BirthDate { get; set; }
        }

        [ComplexType]
        public class Country
        {
            public string Name { get; set; }

            public DateTime FoundingDate { get; set; }
        }

        [Fact]
        public void GetDatePropertyInfos_Test()
        {
            var dateTimePropertInfos = DateTimePropertyInfoHelper.GetDatePropertyInfos(typeof(Hotel));

            dateTimePropertInfos.DateTimePropertyInfos.Count.ShouldBe(2);
            dateTimePropertInfos.ComplexTypePropertyPaths.Count.ShouldBe(6);

            dateTimePropertInfos.ComplexTypePropertyPaths.Count(path => path.Contains("RealLocation")).ShouldBe(3);
            dateTimePropertInfos.ComplexTypePropertyPaths.Count(path => path.Contains("VirtualLocation")).ShouldBe(3);
            dateTimePropertInfos.ComplexTypePropertyPaths.Count(path => path.Contains("Country")).ShouldBe(2);
        }
    }
}