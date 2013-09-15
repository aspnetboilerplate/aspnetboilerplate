using System.Web.Mvc;

namespace Abp.Web.Mvc.Authorization
{
    public class AbpMvcAuthorizeAttribute : AuthorizeAttribute
    {
        public string[] Features { get; set; }

        public AbpMvcAuthorizeAttribute()
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
