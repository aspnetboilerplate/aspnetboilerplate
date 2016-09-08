using System;
using System.Web.Mvc;
using Abp.Timing;

namespace Abp.Web.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Binds any browser request datetime to utc datetime
    /// </summary>
    public class AbpMvcDateTimeBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var date = base.BindModel(controllerContext, bindingContext) as DateTime?;
            if (date.HasValue)
            {
                return Clock.Normalize(date.Value);
            }

            return null;
        }
    }
}
