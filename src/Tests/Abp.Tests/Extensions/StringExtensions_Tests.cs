using System.Globalization;
using Abp.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Extensions
{
    public class StringExtensions_Tests
    {
        [Fact]
        public void ToPascalCase_Test()
        {
            "helloWorld".ToPascalCase().ShouldBe("HelloWorld");
            "istanbul".ToPascalCase().ShouldBe("Istanbul");
            "istanbul".ToPascalCase(new CultureInfo("tr-TR")).ShouldBe("Ýstanbul");
        }

        [Fact]
        public void ToCamelCase_Test()
        {
            "HelloWorld".ToCamelCase().ShouldBe("helloWorld");
            "Istanbul".ToCamelCase().ShouldBe("istanbul");
            "Istanbul".ToCamelCase(new CultureInfo("tr-TR")).ShouldBe("ýstanbul");
            "Ýstanbul".ToCamelCase(new CultureInfo("tr-TR")).ShouldBe("istanbul");
        }

        [Fact]
        public void Right_Test()
        {
            const string str = "This is a test string";

            str.Right(3).ShouldBe("ing");
            str.Right(0).ShouldBe("");
            str.Right(str.Length).ShouldBe(str);
        }

        [Fact]
        public void Left_Test()
        {
            const string str = "This is a test string";

            str.Left(3).ShouldBe("Thi");
            str.Left(0).ShouldBe("");
            str.Left(str.Length).ShouldBe(str);
        }

        [Fact]
        public void Truncate_Test()
        {
            const string str = "This is a test string";
            
            str.Truncate(7).ShouldBe("This is");
            str.Truncate(0).ShouldBe("");
            str.Truncate(100).ShouldBe(str);
        }

        [Fact]
        public void ToEnum_Test()
        {
            "MyValue1".ToEnum<MyEnum>().ShouldBe(MyEnum.MyValue1);
            "MyValue2".ToEnum<MyEnum>().ShouldBe(MyEnum.MyValue2);
        }

        private enum MyEnum
        {
            MyValue1,
            MyValue2
        }
    }
}