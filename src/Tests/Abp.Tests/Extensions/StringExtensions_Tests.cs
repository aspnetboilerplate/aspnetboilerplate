using System;
using System.Globalization;
using Abp.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Extensions
{
    public class StringExtensions_Tests
    {
        [Fact]
        public void EnsureEndsWith_Test()
        {
            //Expected use-cases
            "Test".EnsureEndsWith('!').ShouldBe("Test!");
            "Test!".EnsureEndsWith('!').ShouldBe("Test!");
            @"C:\test\folderName".EnsureEndsWith('\\').ShouldBe(@"C:\test\folderName\");
            @"C:\test\folderName\".EnsureEndsWith('\\').ShouldBe(@"C:\test\folderName\");

            //Case differences
            "TurkeY".EnsureEndsWith('y').ShouldBe("TurkeYy");
            "TurkeY".EnsureEndsWith('y', StringComparison.InvariantCultureIgnoreCase).ShouldBe("TurkeY");
            
            //Edge cases for Turkish 'i'.
            "TAKSÝ".EnsureEndsWith('i', true, new CultureInfo("tr-TR")).ShouldBe("TAKSÝ");
            "TAKSÝ".EnsureEndsWith('i', false, new CultureInfo("tr-TR")).ShouldBe("TAKSÝi");
        }

        [Fact]
        public void EnsureStartsWith_Test()
        {
            //Expected use-cases
            "Test".EnsureStartsWith('~').ShouldBe("~Test");
            "~Test".EnsureStartsWith('~').ShouldBe("~Test");

            //Case differences
            "Turkey".EnsureStartsWith('t').ShouldBe("tTurkey");
            "Turkey".EnsureStartsWith('t', StringComparison.InvariantCultureIgnoreCase).ShouldBe("Turkey");

            //Edge cases for Turkish 'i'.
            "Ýstanbul".EnsureStartsWith('i', true, new CultureInfo("tr-TR")).ShouldBe("Ýstanbul");
            "Ýstanbul".EnsureStartsWith('i', false, new CultureInfo("tr-TR")).ShouldBe("iÝstanbul");
        }

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
        public void TruncateWithPostFix_Test()
        {
            const string str = "This is a test string";

            str.TruncateWithPostfix(3).ShouldBe("Thi...");
            str.TruncateWithPostfix(12).ShouldBe("This is a te...");
            str.TruncateWithPostfix(0).ShouldBe("...");
            str.TruncateWithPostfix(100).ShouldBe(str);

            str.TruncateWithPostfix(3, "~").ShouldBe("Thi~");
            str.TruncateWithPostfix(12, "~").ShouldBe("This is a te~");
            str.TruncateWithPostfix(0, "~").ShouldBe("~");
            str.TruncateWithPostfix(100, "~").ShouldBe(str);
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