using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Linq.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Linq
{
    public class LinqExtension_Tests
    {
        private readonly IQueryable<Person> _myList;

        public LinqExtension_Tests()
        {
            _myList = new List<Person>
                         {
                             new Person("Hagi", 33),
                             new Person("Halil", 31),
                             new Person("Atilla", 42),
                             new Person("Atilla", 1),
                             new Person("Yunus", 19)
                         }.AsQueryable();
        }

        [Fact]
        public void Should_PageBy()
        {
            var pagedList = _myList.PageBy(1, 2).ToList();
            pagedList.Count.ShouldBe(2);
            pagedList[0].Name.ShouldBe("Halil");
            pagedList[1].Name.ShouldBe("Atilla");
        }

        [Fact]
        public void Should_Not_Order_For_Empty_String()
        {
            //TODO: Test with nested sorting!

            var notSorted = _myList.MultipleOrderBy("").ToList();
            notSorted[0].Name.ShouldBe("Hagi");
            notSorted[1].Name.ShouldBe("Halil");
            notSorted[2].Name.ShouldBe("Atilla");
            notSorted[3].Name.ShouldBe("Atilla");
            notSorted[4].Name.ShouldBe("Yunus");
        }

        [Fact]
        public void Should_Order_For_Single_Direction()
        {
            var sortedByName = _myList.MultipleOrderBy("Name").ToList();
            sortedByName[0].Name.ShouldBe("Atilla");
            sortedByName[1].Name.ShouldBe("Atilla");
            sortedByName[2].Name.ShouldBe("Hagi");
            sortedByName[3].Name.ShouldBe("Halil");
            sortedByName[4].Name.ShouldBe("Yunus");
        }

        [Fact]
        public void Should_Order_For_Single_Direction_ASC()
        {
            var sortedByAge = _myList.MultipleOrderBy("Age ASC").ToList();
            sortedByAge[0].Name.ShouldBe("Atilla");
            sortedByAge[1].Name.ShouldBe("Yunus");
            sortedByAge[2].Name.ShouldBe("Halil");
            sortedByAge[3].Name.ShouldBe("Hagi");
            sortedByAge[4].Name.ShouldBe("Atilla");
        }

        [Fact]
        public void Should_Order_For_Single_Direction_DESC()
        {
            var sortedByAgeDesc = _myList.MultipleOrderBy("Age DESC").ToList();
            sortedByAgeDesc[0].Name.ShouldBe("Atilla");
            sortedByAgeDesc[1].Name.ShouldBe("Hagi");
            sortedByAgeDesc[2].Name.ShouldBe("Halil");
            sortedByAgeDesc[3].Name.ShouldBe("Yunus");
            sortedByAgeDesc[4].Name.ShouldBe("Atilla");
        }

        [Fact]
        public void Should_Order_For_Multiple_Direction()
        {
            var sortedByNameAscAgeDesc = _myList.MultipleOrderBy("Name ASC, Age DESC").ToList();
            sortedByNameAscAgeDesc[0].Name.ShouldBe("Atilla");
            sortedByNameAscAgeDesc[0].Age.ShouldBe(42);
            sortedByNameAscAgeDesc[1].Name.ShouldBe("Atilla");
            sortedByNameAscAgeDesc[1].Age.ShouldBe(1);
            sortedByNameAscAgeDesc[2].Name.ShouldBe("Hagi");
            sortedByNameAscAgeDesc[3].Name.ShouldBe("Halil");
            sortedByNameAscAgeDesc[4].Name.ShouldBe("Yunus");
        }

        [Fact]
        public void Should_Throw_Exception_If_Sorting_Prop_Is_Invalid()
        {
            Assert.Throws<ArgumentException>(() => _myList.MultipleOrderBy("Surname ASC"));
        }

        private class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }
        }
    }
}
