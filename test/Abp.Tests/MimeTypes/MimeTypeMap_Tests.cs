using System;
using System.Collections.Generic;
using System.Linq;
using Abp.MimeTypes;
using Shouldly;
using Xunit;

namespace Abp.Tests.MimeTypes
{
    public class MimeTypeMap_Tests
    {
        private IMimeTypeMap _mimeTypeMap;

        public MimeTypeMap_Tests()
        {
            _mimeTypeMap = new MimeTypeMap();
        }

        [Theory]
        [InlineData("zip", "application/zip")]
        [InlineData("txt", "text/plain")]
        [InlineData("css", "text/css")]
        [InlineData("html", "text/html")]
        public void Should_Get_Mime_Type_Without_Dot(string key, string expected)
        {
            _mimeTypeMap.GetMimeType(key).ShouldBe(expected);

            _mimeTypeMap.TryGetMimeType(key, out var output).ShouldBe(true);
            output.ShouldBe(expected);
        }

        [Theory]
        [InlineData("test.zip", "application/zip")]
        [InlineData("test.txt", "text/plain")]
        [InlineData("test.css", "text/css")]
        [InlineData("test.html", "text/html")]
        public void Should_Get_Mime_Type_With_FileName(string key, string expected)
        {
            _mimeTypeMap.GetMimeType(key).ShouldBe(expected);

            _mimeTypeMap.TryGetMimeType(key, out var output).ShouldBe(true);
            output.ShouldBe(expected);
        }

        [Fact]
        public void Try_Get_Mime_Type_Common_Tests()
        {
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.TryGetMimeType("", out _); });

            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.TryGetMimeType(null, out _); });

            _mimeTypeMap.TryGetMimeType("test", out _).ShouldBeFalse(); //not found
        }

        [Fact]
        public void Get_Mime_Type_Common_Tests()
        {
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.GetMimeType(""); });

            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.GetMimeType(null); });

            Should.Throw<ArgumentException>(() => { _mimeTypeMap.GetMimeType("test"); }); //not found exception
            _mimeTypeMap.GetMimeType("test", throwErrorIfNotFound: false).ShouldBe(string.Empty);
        }

        [Fact]
        public void Try_Get_Extension_Common_Tests()
        {
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.TryGetExtension("", out _); });

            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.TryGetExtension(null, out _); });

            _mimeTypeMap.TryGetExtension("test", out _).ShouldBeFalse(); //not found
        }

        [Fact]
        public void Get_Extension_Common_Tests()
        {
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.GetExtension(""); });

            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.GetExtension(null); });

            Should.Throw<ArgumentException>(() => { _mimeTypeMap.GetExtension("test"); }); //not found exception
            _mimeTypeMap.GetExtension("test", throwErrorIfNotFound: false).ShouldBe(string.Empty);
        }

        [Fact]
        public void Add_Mime_Type_Tests()
        {
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddMimeType("", ""); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddMimeType(null, null); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddMimeType("", ".test"); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddMimeType(null, ".test"); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddMimeType("test", ""); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddMimeType("test", null); });

            Should.Throw<ArgumentException>(() => { _mimeTypeMap.AddMimeType(".test", ".test"); }); //MIME type should not start with dot exception
            Should.Throw<ArgumentException>(() => { _mimeTypeMap.AddMimeType("test", "test"); }); //Extension should start with dot exception


            Should.Throw<ArgumentException>(() => { _mimeTypeMap.AddMimeType("application/zip", ".zip"); }); //An item with the same mimeType has already been added exception

            var testMimeType = "test/test";
            var testExtension = ".test";
            _mimeTypeMap.GetExtension(testMimeType, false).ShouldBeNullOrEmpty();
            _mimeTypeMap.AddMimeType(testMimeType, testExtension);
            _mimeTypeMap.GetExtension(testMimeType).ShouldBe(testExtension);
        }

        [Fact]
        public void Remove_Mime_Type_Tests()
        {
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.RemoveMimeType(""); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.RemoveMimeType(null); });

            Should.Throw<ArgumentException>(() => { _mimeTypeMap.RemoveMimeType(".test"); }); //MIME type should not start with dot exception

            var testMimeType = "application/zip";
            _mimeTypeMap.GetExtension(testMimeType, false).ShouldNotBeNullOrEmpty();
            _mimeTypeMap.RemoveMimeType(testMimeType);
            _mimeTypeMap.GetExtension(testMimeType, false).ShouldBeNullOrEmpty();
        }

        [Fact]
        public void Add_Extension_Tests()
        {
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddExtension("", ""); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddExtension(null, null); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddExtension("", ".test"); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddExtension(null, ".test"); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddExtension("test", ""); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.AddExtension("test", null); });

            Should.Throw<ArgumentException>(() => { _mimeTypeMap.AddExtension(".test", ".test"); }); //MIME type should not start with dot exception
            Should.Throw<ArgumentException>(() => { _mimeTypeMap.AddExtension("test", "test"); }); //Extension should start with dot exception


            Should.Throw<ArgumentException>(() => { _mimeTypeMap.AddExtension(".zip", "application/zip"); }); //An item with the same extension has already been added exception

            var testMimeType = "test/test";
            var testExtension = ".test";
            _mimeTypeMap.GetMimeType(testExtension, false).ShouldBeNullOrEmpty();
            _mimeTypeMap.AddExtension(testExtension, testMimeType);
            _mimeTypeMap.GetMimeType(testExtension).ShouldBe(testMimeType);
        }

        [Fact]
        public void Remove_Extension_Tests()
        {
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.RemoveExtension(""); });
            Should.Throw<ArgumentNullException>(() => { _mimeTypeMap.RemoveExtension(null); });

            Should.Throw<ArgumentException>(() => { _mimeTypeMap.RemoveExtension("test"); }); //Extension should start with dot exception

            var testExtension = ".zip";
            _mimeTypeMap.GetMimeType(testExtension, false).ShouldNotBeNullOrEmpty();
            _mimeTypeMap.RemoveExtension(testExtension);
            _mimeTypeMap.GetMimeType(testExtension, false).ShouldBeNullOrEmpty();
        }
    }
}