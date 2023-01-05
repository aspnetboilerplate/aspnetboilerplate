using System;
using System.Threading.Tasks;
using Abp.Timing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Abp.AspNetCore.Mvc.ModelBinding
{
    public class AbpDateTimeModelBinder : IModelBinder
    {
        private readonly Type _type;
        private readonly SimpleTypeModelBinder _simpleTypeModelBinder;
        private readonly IClock _clock;
        
        public AbpDateTimeModelBinder(Type type, IClock clock)
        {
            _type = type;
            _clock = clock;
            _simpleTypeModelBinder = new SimpleTypeModelBinder(type, NullLoggerFactory.Instance);
        }
        
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await _simpleTypeModelBinder.BindModelAsync(bindingContext);

            if (!bindingContext.Result.IsModelSet)
            {
                return;
            }

            if (_type == typeof(DateTime))
            {
                var dateTime = (DateTime)bindingContext.Result.Model;
                bindingContext.Result = ModelBindingResult.Success(_clock.Normalize(dateTime));
            }
            else
            {
                var dateTime = (DateTime?)bindingContext.Result.Model;
                if (dateTime != null)
                {
                    bindingContext.Result = ModelBindingResult.Success(_clock.Normalize(dateTime.Value));
                }
            }
        }
    }
}