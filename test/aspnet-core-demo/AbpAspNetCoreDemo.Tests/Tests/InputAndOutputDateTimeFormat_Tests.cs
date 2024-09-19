using System;
using System.Collections.Generic;
using System.Text.Json;
using Abp.Json;
using Abp.Json.SystemTextJson;
using Abp.Timing;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using AbpDateTimeConverter = Abp.Json.SystemTextJson.AbpDateTimeConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests;

[Collection("Clock.Provider")]
public class InputAndOutputDateTimeFormat_Tests
{
    [Fact]
    public void InputAndOutputDateTimeFormat_Test()
    {
        Clock.Provider = ClockProviders.Utc;
        var resultDate = new DateTime(2016, 04, 13, 08, 58, 10, DateTimeKind.Utc);

        var inputDatetimeFormat = new List<string>()
            {
                "yyyy*MM*dd",
                "yyyy-MM-dd HH:mm:ss"
            };
        var outputDatetimeFormat = "yyyy*MM-dd HH:mm:ss";

        var options = new JsonSerializerOptions();
        options.Converters.Add(new AbpDateTimeConverter(inputDatetimeFormat, outputDatetimeFormat));
        options.Converters.Add(new AbpNullableDateTimeConverter(inputDatetimeFormat, outputDatetimeFormat));

        var json = JsonSerializer.Serialize(new DateTimeDto()
        {
            DateTime1 = resultDate,
            DateTime2 = resultDate
        }, options);
        json.ShouldContain("\"DateTime1\":\"2016*04-13 08:58:10\"");
        json.ShouldContain("\"DateTime2\":\"2016*04-13 08:58:10\"");

        json = JsonConvert.SerializeObject(new DateTimeDto()
        {
            DateTime1 = resultDate,
            DateTime2 = resultDate
        }, new JsonSerializerSettings
        {
            ContractResolver = new AbpMvcContractResolver(inputDatetimeFormat, outputDatetimeFormat)
        });
        json.ShouldContain("\"DateTime1\":\"2016*04-13 08:58:10\"");
        json.ShouldContain("\"DateTime2\":\"2016*04-13 08:58:10\"");

        var dto = JsonSerializer.Deserialize<DateTimeDto>(
            "{\"DateTime1\":\"" + resultDate.ToString("yyyy*MM*dd") + "\",\"DateTime2\":\"" +
            resultDate.ToString("yyyy-MM-dd HH:mm:ss") + "\"}", options);

        dto.DateTime1.ShouldBe(resultDate.Date);
        dto.DateTime1.Kind.ShouldBe(DateTimeKind.Utc);
        dto.DateTime2.ShouldBe(resultDate);
        dto.DateTime2.Value.Kind.ShouldBe(DateTimeKind.Utc);

        dto = JsonConvert.DeserializeObject<DateTimeDto>(
            "{\"DateTime1\":\"" + resultDate.ToString("yyyy*MM*dd") + "\",\"DateTime2\":\"" +
            resultDate.ToString("yyyy-MM-dd HH:mm:ss") + "\"}", new JsonSerializerSettings
            {
                ContractResolver = new AbpMvcContractResolver(inputDatetimeFormat, outputDatetimeFormat)
            });

        dto.DateTime1.ShouldBe(resultDate.Date);
        dto.DateTime1.Kind.ShouldBe(DateTimeKind.Utc);
        dto.DateTime2.ShouldBe(resultDate);
        dto.DateTime2.Value.Kind.ShouldBe(DateTimeKind.Utc);
    }

    public class DateTimeDto
    {
        public DateTime DateTime1 { get; set; }

        public DateTime? DateTime2 { get; set; }
    }

}