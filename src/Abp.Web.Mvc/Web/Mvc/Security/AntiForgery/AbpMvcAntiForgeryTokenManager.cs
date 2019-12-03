using System;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Mvc.Security.AntiForgery
{
    public class AbpMvcAntiForgeryManager : AbpAntiForgeryManager
    {
        private static readonly Lazy<object> AntiForgeryWorkerObject = new Lazy<object>(() =>
        {
            var antiForgeryWorkerField = typeof(System.Web.Helpers.AntiForgery).GetField("_worker", BindingFlags.NonPublic | BindingFlags.Static);
            if (antiForgeryWorkerField == null)
            {
                throw new AbpException("Can not get _worker field of System.Web.Helpers.AntiForgery class. It's internal implementation might be changed. Please create an issue on GitHub repository to solve this.");
            }

            return antiForgeryWorkerField.GetValue(null);
        });

        private static readonly Lazy<MethodInfo> GetFormInputElementMethod = new Lazy<MethodInfo>(() =>
        {
            return AntiForgeryWorkerObject.Value
                .GetType()
                .GetMethod("GetFormInputElement", BindingFlags.Public | BindingFlags.Instance);
        });

        public AbpMvcAntiForgeryManager(IAbpAntiForgeryConfiguration configuration)
            : base(configuration)
        {

        }

        public override string GenerateToken()
        {
            /* Getting Token from input element, like done in views.
             * We are using reflection because some types/methods are internal!
             */

            var tagBuilder = (TagBuilder)GetFormInputElementMethod.Value.Invoke(
                AntiForgeryWorkerObject.Value,
                new object[]
                {
                    new HttpContextWrapper(HttpContext.Current)
                }
            );

            return tagBuilder.Attributes["value"];
        }

        public override bool IsValid(string cookieValue, string tokenValue)
        {
            try
            {
                System.Web.Helpers.AntiForgery.Validate(
                    HttpContext.Current.Request.Cookies[AntiForgeryConfig.CookieName]?.Value ?? cookieValue,
                    tokenValue
                    );

                return true;
            }
            catch (HttpAntiForgeryException ex)
            {
                Logger.Warn(ex.Message);
                return false;
            }
        }
    }
}
