using System;
using System.Reflection;
using System.Collections.Generic;
using Abp.Reflection;
using Shouldly;
using Xunit;

namespace Abp.Tests.Reflection
{
    public class ReflectionHelper_Tests
    {
        [Fact]
        public static void Should_Find_GenericType()
        {
            ReflectionHelper.IsAssignableToGenericType(typeof(List<string>), typeof(List<>)).ShouldBe(true);
            ReflectionHelper.IsAssignableToGenericType(new List<string>().GetType(), typeof(List<>)).ShouldBe(true);
            
            ReflectionHelper.IsAssignableToGenericType(typeof(MyList), typeof(List<>)).ShouldBe(true);
            ReflectionHelper.IsAssignableToGenericType(new MyList().GetType(), typeof(List<>)).ShouldBe(true);
        }

        [Fact]
        public static void Should_Find_Attributes()
        {
            var attributes = ReflectionHelper.GetAttributesOfMemberAndDeclaringType<MyAttribute>(typeof(MyDerivedList).GetTypeInfo().GetMethod("DoIt"));
            attributes.Count.ShouldBe(2); //TODO: Why not find MyList's attribute?
            attributes[0].Number.ShouldBe(1);
            attributes[1].Number.ShouldBe(2);
            //attributes[2].Number.ShouldBe(3);
        }

        [Fact]
        public static void GetSingleAttributeOfMemberOrDeclaringTypeOrDefault_Test()
        {
            var attr1 = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<IMyAttribute>(
                typeof(MyDerivedList).GetTypeInfo().GetMethod("DoIt")
            );

            attr1.ShouldNotBeNull();
            attr1.Number.ShouldBe(1);

            var attr2 = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<IMyAttribute>(
                typeof(MyDerivedList2).GetTypeInfo().GetMethod("DoIt")
            );

            attr2.ShouldNotBeNull();
            attr2.Number.ShouldBe(2);
        }

        [MyAttribute(3)]
        public class MyList : List<int>
        {

        }

        [MyAttribute(2)]
        public class MyDerivedList : MyList
        {
            [MyAttribute(1)]
            public void DoIt()
            {

            }
        }

        [MyAttribute(2)]
        public class MyDerivedList2 : MyList
        {
            public void DoIt()
            {

            }
        }

        public class MyAttribute : Attribute, IMyAttribute
        {
            public int Number { get; set; }

            public MyAttribute(int number)
            {
                Number = number;
            }
        }

        public interface IMyAttribute
        {
            int Number { get; set; }
        }
    }
}
