using System;
using Abp.Reflection;
using Shouldly;
using Xunit;

namespace Abp.Tests.Reflection
{
    public class TypeHelper_Tests
    {
        [Fact]
        public void Test_IsFunc()
        {
            TypeHelper.IsFunc(new Func<object>(() => 42)).ShouldBe(true);
            TypeHelper.IsFunc(new Func<int>(() => 42)).ShouldBe(true);
            TypeHelper.IsFunc(new Func<string>(() => "42")).ShouldBe(true);

            TypeHelper.IsFunc("42").ShouldBe(false);
        }

        [Fact]
        public void Test_IsFuncOfTReturn()
        {
            TypeHelper.IsFunc<object>(new Func<object>(() => 42)).ShouldBe(true);
            TypeHelper.IsFunc<object>(new Func<int>(() => 42)).ShouldBe(false);
            TypeHelper.IsFunc<string>(new Func<string>(() => "42")).ShouldBe(true);

            TypeHelper.IsFunc("42").ShouldBe(false);
        }

        [Fact]
        public void Test_IsPrimitiveExtendedIncludingNullable()
        {
            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(int)).ShouldBe(true);
            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(int?)).ShouldBe(true);

            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(Guid)).ShouldBe(true);
            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(Guid?)).ShouldBe(true);

            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(string)).ShouldBe(true);

            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(TypeHelper_Tests)).ShouldBe(false);
        }
    }
}
