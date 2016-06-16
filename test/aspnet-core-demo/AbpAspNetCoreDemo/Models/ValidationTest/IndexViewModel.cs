using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Extensions;
using Abp.Runtime.Validation;

namespace AbpAspNetCoreDemo.Models.ValidationTest
{
    public class IndexViewModel : ICustomValidate, IShouldNormalize
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Name { get; set; }

        [Range(18, 99)]
        public int Age { get; set; }

        public string ValueToBeNormalized { get; set; }

        public void AddValidationErrors(List<ValidationResult> results)
        {
            if (ValueToBeNormalized == "exvalue")
            {
                results.Add(new ValidationResult("ValueToBeNormalized can not be 'exvalue'!", new[] { "ValueToBeNormalized" }));
            }
        }

        public void Normalize()
        {
            if (ValueToBeNormalized.IsNullOrEmpty())
            {
                ValueToBeNormalized = "normalized value";
            }
        }
    }
}
