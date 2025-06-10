using System;
using System.Globalization;
using Abp.Application.Services;

namespace AbpAspNetCoreDemo.Core.Application;

public class ModelBindingAppService : ApplicationService
{
    public CalculatePriceOutput CalculatePrice(CalculatePriceDto input)
    {
        var culture = CultureInfo.CurrentCulture.Name;
        return new CalculatePriceOutput
        {
            Culture = culture,
            Price = input.Price.ToString(),
            OrderDate = input.OrderDate.ToString(),
        };
    }

    public string TestDate(TestDateDto input)
    {
        return input.TestDate.ToString();
    }

    public string NullableTestDate(NullableTestDateDto input)
    {
        if (!input.TestDate.HasValue)
        {
            return string.Empty;
        }

        return input.TestDate.ToString();
    }

    public string DateOnlyTestDate(DateOnlyTestDateDateDto input)
    {
        return input.TestDate.ToString();
    }
}

public class CalculatePriceDto
{
    public double Price { get; set; }
    public DateOnly OrderDate { get; set; }
}

public class CalculatePriceOutput
{
    public string Price { get; set; }

    public string OrderDate { get; set; }

    public string Culture { get; set; }
}

public class TestDateDto
{
    public DateTime TestDate { get; set; }
}

public class NullableTestDateDto
{
    public DateTime? TestDate { get; set; }
}

public class DateOnlyTestDateDateDto
{
    public DateOnly TestDate { get; set; }
}