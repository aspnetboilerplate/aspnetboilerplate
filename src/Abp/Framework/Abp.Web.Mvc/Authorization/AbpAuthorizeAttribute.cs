using System.Web.Mvc;

namespace Abp.Web.Mvc.Authorization
{
    public class AbpAuthorizeAttribute : AuthorizeAttribute
    {
        public string[] Features { get; set; }

        public AbpAuthorizeAttribute()
        {
            Features = new string[0];            
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                return false;
            }

            return HasAccessToOneOfFeatures();
        }

        private bool HasAccessToOneOfFeatures()
        {
            return true;
        }
    }
}
