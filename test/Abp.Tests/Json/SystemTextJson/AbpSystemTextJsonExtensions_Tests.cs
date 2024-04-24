using System;
using Abp.Json;
using Abp.Timing;
using Shouldly;
using Xunit;

namespace Abp.Tests.Json.SystemTextJson
{
    [Collection("Clock.Provider")]
    public class AbpSystemTextJsonExtensionsTests
    {
        [Fact]
        public void Serialize_Deserialize_With_Boolean()
        {
            var json = "{\"Name\":\"abp\",\"IsDeleted\":\"fAlSe\"}";
            var file = json.FromJsonString<FileWithBoolean>();
            file.Name.ShouldBe("abp");
            file.IsDeleted.ShouldBeFalse();

            file.IsDeleted = false;
            var newJson = file.ToJsonString();
            newJson.ShouldBe("{\"Name\":\"abp\",\"IsDeleted\":false}");
        }

        [Fact]
        public void Serialize_Deserialize_With_Nullable_Boolean()
        {
            var json = "{\"Name\":\"abp\",\"IsDeleted\":null}";
            var file = json.FromJsonString<FileWithNullableBoolean>();
            file.Name.ShouldBe("abp");
            file.IsDeleted.ShouldBeNull();

            var newJson = file.ToJsonString();
            newJson.ShouldBe("{\"Name\":\"abp\",\"IsDeleted\":null}");

            json = "{\"Name\":\"abp\",\"IsDeleted\":\"true\"}";
            file = json.FromJsonString<FileWithNullableBoolean>();
            file.IsDeleted.ShouldNotBeNull();
            file.IsDeleted.Value.ShouldBeTrue();

            newJson = file.ToJsonString();
            newJson.ShouldBe("{\"Name\":\"abp\",\"IsDeleted\":true}");
        }

        [Fact]
        public void Serialize_Deserialize_With_Enum()
        {
            var json = "{\"Name\":\"abp\",\"Type\":\"Exe\"}";
            var file = json.FromJsonString<FileWithEnum>();
            file.Name.ShouldBe("abp");
            file.Type.ShouldBe(FileType.Exe);

            var newJson = file.ToJsonString();
            newJson.ShouldBe("{\"Name\":\"abp\",\"Type\":2}");
        }

        [Fact]
        public void Serialize_Deserialize_With_Nullable_Enum()
        {
            var json = "{\"Name\":\"abp\",\"Type\":null}";
            var file = json.FromJsonString<FileWithNullableEnum>();
            file.Name.ShouldBe("abp");
            file.Type.ShouldBeNull();

            var newJson = file.ToJsonString();
            newJson.ShouldBe("{\"Name\":\"abp\",\"Type\":null}");

            json = "{\"Name\":\"abp\",\"Type\":\"Exe\"}";
            file = json.FromJsonString<FileWithNullableEnum>();
            file.Type.ShouldNotBeNull();
            file.Type.ShouldBe(FileType.Exe);

            newJson = file.ToJsonString();
            newJson.ShouldBe("{\"Name\":\"abp\",\"Type\":2}");
        }

        [Fact]
        public void Serialize_Deserialize_With_Datetime()
        {
            Clock.Provider = ClockProviders.Unspecified;
            var json = "{\"Name\":\"abp\",\"CreationTime\":\"2020-11-20T00:00:00\"}";
            var file = json.FromJsonString<FileWithDatetime>();
            file.CreationTime.Year.ShouldBe(2020);
            file.CreationTime.Month.ShouldBe(11);
            file.CreationTime.Day.ShouldBe(20);

            var newJson = file.ToJsonString();
            newJson.ShouldBe(json);
        }

        [Fact]
        public void Serialize_Deserialize_With_Nullable_Datetime()
        {
            var json = "{\"Name\":\"abp\",\"CreationTime\":null}";
            var file = json.FromJsonString<FileWithNullableDatetime>();
            file.CreationTime.ShouldBeNull();

            json = "{\"Name\":\"abp\"}";
            file = json.FromJsonString<FileWithNullableDatetime>();
            file.CreationTime.ShouldBeNull();

            json = "{\"Name\":\"abp\",\"CreationTime\":\"2020-11-20T00:00:00\"}";
            file = json.FromJsonString<FileWithNullableDatetime>();
            file.CreationTime.ShouldNotBeNull();

            file.CreationTime.Value.Year.ShouldBe(2020);
            file.CreationTime.Value.Month.ShouldBe(11);
            file.CreationTime.Value.Day.ShouldBe(20);

            var newJson = file.ToJsonString();
            newJson.ShouldBe(json);
        }

        public class FileWithBoolean
        {
            public string Name { get; set; }

            public bool IsDeleted { get; set; }
        }

        public class FileWithNullableBoolean
        {
            public string Name { get; set; }

            public bool? IsDeleted { get; set; }
        }

        public class FileWithEnum
        {
            public string Name { get; set; }

            public FileType Type { get; set; }
        }

        public class FileWithNullableEnum
        {
            public string Name { get; set; }

            public FileType? Type { get; set; }
        }

        public enum FileType
        {
            Zip = 0,
            Exe = 2
        }

        public class FileWithDatetime
        {
            public string Name { get; set; }

            public DateTime CreationTime { get; set; }
        }

        public class FileWithNullableDatetime
        {
            public string Name { get; set; }

            public DateTime? CreationTime { get; set; }
        }
    }

    [Collection("Clock.Provider")]
    public abstract class AbpSystemTextJsonExtensions_Datetime_Kind_Tests
    {
        protected DateTimeKind Kind { get; set; } = DateTimeKind.Unspecified;

        [Fact]
        public void Serialize_Deserialize()
        {
            var json = "{\"Name\":\"abp\",\"CreationTime\":\"2020-11-20T00:00:00\"}";
            var file = json.FromJsonString<AbpSystemTextJsonExtensionsTests.FileWithDatetime>();
            file.CreationTime.Kind.ShouldBe(Kind);
        }
    }

    public class AbpSystemTextJsonExtensionsDatetimeKindUtcTests : AbpSystemTextJsonExtensions_Datetime_Kind_Tests
    {
        public AbpSystemTextJsonExtensionsDatetimeKindUtcTests()
        {
            Kind = DateTimeKind.Utc;
            Clock.Provider = ClockProviders.Utc;
        }
    }

    public class AbpSystemTextJsonExtensionsDatetimeKindLocalTests : AbpSystemTextJsonExtensions_Datetime_Kind_Tests
    {
        public AbpSystemTextJsonExtensionsDatetimeKindLocalTests()
        {
            Kind = DateTimeKind.Local;
            Clock.Provider = ClockProviders.Local;;
        }
    }

    public class AbpSystemTextJsonExtensionsDatetimeKindUnspecifiedTests : AbpSystemTextJsonExtensions_Datetime_Kind_Tests
    {
        public AbpSystemTextJsonExtensionsDatetimeKindUnspecifiedTests()
        {
            Kind = DateTimeKind.Unspecified;
            Clock.Provider = ClockProviders.Unspecified;;
        }
    }
}
