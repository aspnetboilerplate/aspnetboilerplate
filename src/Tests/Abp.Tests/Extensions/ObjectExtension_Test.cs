using System.Globalization;
using Abp.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Extensions
{
    public class ObjectExtensions_Tests
    {
        [Fact]
        public void As_Test()
        {
            var obj = (object)new ObjectExtensions_Tests();
            obj.As<ObjectExtensions_Tests>().ShouldNotBe(null);

            obj = null;
            obj.As<ObjectExtensions_Tests>().ShouldBe(null);
        }

        [Fact]
        public void IsIn_Test()
        {
            5.IsIn(1, 3, 5, 7).ShouldBe(true);
            6.IsIn(1, 3, 5, 7).ShouldBe(false);

            int? number = null;
            number.IsIn(2, 3, 5).ShouldBe(false);

            var str = "a";
            str.IsIn("a", "b", "c").ShouldBe(true);

            str = null;
            str.IsIn("a", "b", "c").ShouldBe(false);
        }
    }

    public class StringExtensions_Test
    {
        [Fact]
        public void ToPascalCase_Test()
        {
            "helloWorld".ToPascalCase().ShouldBe("HelloWorld");
            "istanbul".ToPascalCase().ShouldBe("Istanbul");
            "istanbul".ToPascalCase(new CultureInfo("tr-TR")).ShouldBe("İstanbul");
        }

        [Fact]
        public void ToCamelCase_Test()
        {
            "HelloWorld".ToCamelCase().ShouldBe("helloWorld");
            "Istanbul".ToCamelCase().ShouldBe("istanbul");
            "Istanbul".ToCamelCase(new CultureInfo("tr-TR")).ShouldBe("ıstanbul");
            "İstanbul".ToCamelCase(new CultureInfo("tr-TR")).ShouldBe("istanbul");
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
