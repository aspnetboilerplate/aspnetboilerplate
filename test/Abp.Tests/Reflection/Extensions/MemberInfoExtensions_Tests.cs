using System;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Reflection.Extensions
{
    public class MemberInfoExtensions_Tests
    {
        [Theory]
        [InlineData(typeof(MyClass))]
        [InlineData(typeof(MyBaseClass))]
        public void GetSingleAttributeOfTypeOrBaseTypesOrNull_Test(Type type)
        {
            var attr = type.GetSingleAttributeOfTypeOrBaseTypesOrNull<MultiTenancySideAttribute>();
            attr.ShouldNotBeNull();
            attr.Side.ShouldBe(MultiTenancySides.Host);
        }

        private class MyClass : MyBaseClass
        {
            
        }

        [MultiTenancySide(MultiTenancySides.Host)]
        private abstract class MyBaseClass
        {

        }
    }
}
