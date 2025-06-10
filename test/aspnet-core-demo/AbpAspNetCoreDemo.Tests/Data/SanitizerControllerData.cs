using System.Collections.Generic;
using AbpAspNetCoreDemo.Model;

namespace AbpAspNetCoreDemo.IntegrationTests.Data;

public class SanitizerControllerData
{
    public static IEnumerable<object[]> SanitizerTestData =>
        new List<object[]>
        {
            new object[] { new MyModel{HtmlInput = "<script>alert('hello')</script>hello"}, new MyModel{HtmlInput = "hello"}},
        };

    public static IEnumerable<object[]> SanitizerTestPropertyData =>
    new List<object[]>
    {
            new object[] { "<script>alert('hello')</script>hello", null, new { firstInput = "hello", secondInput = (string) null } },
            new object[] { "<script>alert('hello')</script>hello", "3", new { firstInput = "hello", secondInput = "3" } },
            new object[] { "<script>alert('hello')</script>hello", "Second text", new { firstInput = "hello", secondInput = "Second text"} },
            new object[] { null, null, new { firstInput = (string) null, secondInput = (string) null} },
            new object[] { null, "3", new { firstInput = (string) null, secondInput = "3" }},
            new object[] { "Normal text", null, new { firstInput = "Normal text", secondInput = (string) null }},
    };


    public static IEnumerable<object[]> SanitizerTestInnerModelData =>
        new List<object[]>
        {
            new object[]
            {
                new MyModel{HtmlInput = "<script>alert('hello')</script>hello",
                    MyInnerModel = new MyInnerModel{InnerHtmlInput = "<script>alert('world')</script>world",
                        MyDeepestInnerModel = new MyDeepestInnerModel{DeepestInnerHtmlInput = "<script>alert('hello')</script>hello world"},
                    }
                },
                new MyModel{HtmlInput = "hello",
                    MyInnerModel = new MyInnerModel{InnerHtmlInput = "world",
                        MyDeepestInnerModel = new MyDeepestInnerModel{DeepestInnerHtmlInput = "hello world"},
                    }
                }
            },
        };

    public static IEnumerable<object[]> SanitizerTestAttributedModelData =>
        new List<object[]>
        {
            new object[]
            {
                new MyAttributedModel{HtmlInput = "<script>alert('hello')</script>hello",
                    DontSanitizeInput = "<script>alert('hello')</script>hello",
                    KeepChildNodesInput = "<script>alert('hello')</script>hello"},
                new MyAttributedModel{HtmlInput = "hello",
                    DontSanitizeInput = "<script>alert('hello')</script>hello",
                    KeepChildNodesInput = "alert('hello')hello"}
            },
        };

    public static IEnumerable<object[]> SanitizerTestAttributedPropertyModelData =>
        new List<object[]>
        {
            new object[]
            {
                new MyAttributedPropertyModel{KeepChildNodesInput = "<script>alert('hello')</script>",
                    DontSanitizeInput = "<script>alert('hello')</script>",
                    DontKeepChildNodesInput = "<script>alert('hello')</script>"},
                new MyAttributedPropertyModel{KeepChildNodesInput = "",
                    DontSanitizeInput = "<script>alert('hello')</script>",
                    DontKeepChildNodesInput = ""}
            },
        };
}