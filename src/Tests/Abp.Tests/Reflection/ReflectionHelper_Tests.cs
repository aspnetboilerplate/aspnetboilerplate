using System;
using System.Collections.Generic;
using Abp.Reflection;
using Shouldly;
using Xunit;

namespace Abp.Tests.Reflection
{
    public class ReflectionHelper_Tests
    {
        [My(3)]
        public class MyList : List<int>
        {
        }

        [My(2)]
        public class MyDerivedList : MyList
        {
            [My(1)]
            public void DoIt()
            {
            }
        }

        public class MyAttribute : Attribute
        {
            public MyAttribute(int number)
            {
                Number = number;
            }

            public int Number { get; set; }
        }

        [Fact]
        public static void Should_Find_Attributes()
        {
            var attributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType<MyAttribute>(
                    typeof(MyDerivedList).GetMethod("DoIt"));
            attributes.Count.ShouldBe(2); //TODO: Why not find MyList's attribute?
            attributes[0].Number.ShouldBe(1);
            attributes[1].Number.ShouldBe(2);
            //attributes[2].Number.ShouldBe(3);
        }

        [Fact]
        public static void Should_Find_GenericType()
        {
            ReflectionHelper.IsAssignableToGenericType(typeof(List<string>), typeof(List<>)).ShouldBe(true);
            ReflectionHelper.IsAssignableToGenericType(new List<string>().GetType(), typeof(List<>)).ShouldBe(true);

            ReflectionHelper.IsAssignableToGenericType(typeof(MyList), typeof(List<>)).ShouldBe(true);
            ReflectionHelper.IsAssignableToGenericType(new MyList().GetType(), typeof(List<>)).ShouldBe(true);
        }
    }
}