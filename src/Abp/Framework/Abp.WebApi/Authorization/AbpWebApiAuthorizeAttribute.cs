using System.Web.Http;

namespace Abp.WebApi.Authorization
{
    public class AbpWebApiAuthorizeAttribute : AuthorizeAttribute
    {
        public string[] Features { get; set; }

        public AbpWebApiAuthorizeAttribute()
        {
            Features = new string[0];            
        }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!base.IsAuthorized(actionContext))
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
