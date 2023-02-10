using System.Globalization;
using Abp.Application.Services;

namespace AbpAspNetCoreDemo.Core.Application
{
    public class ModelBindingAppService : ApplicationService
    {
        public CalculatePriceOutput CalculatePrice(CalculatePriceDto input)
        {
            var culture = CultureInfo.CurrentCulture.Name;
            return new CalculatePriceOutput
            {
                Culture = culture,
                Price = input.Price.ToString()
            };
        }
    }

    public class CalculatePriceDto
    {
        public double Price { get; set; }
    }

    public class CalculatePriceOutput
    {
        public string Price { get; set; }

        public string Culture { get; set; }
    }
}