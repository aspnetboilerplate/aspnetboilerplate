using System.Web.Http;
using System.Web.Http.Controllers;
using Abp.Application.Authorization;
using Abp.Dependency;
using Abp.Logging;

namespace Abp.WebApi.Authorization
{
    /// <summary>
    /// This attribute is used on a method of an <see cref="ApiController"/>
    /// to make that method usable only by authorized users.
    /// TODO: This class is not implemented yet.
    /// </summary>
    public class AbpAuthorizeAttribute : AuthorizeAttribute , IAbpAuthorizeAttribute
    {
        public string[] Permissions { get; set; }

        public bool RequireAllPermissions { get; set; }
        
        /// <summary>
        /// Creates a new instance of <see cref="AbpAuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A list of permissions to authorize</param>
        public AbpAuthorizeAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (!base.IsAuthorized(actionContext))
            {
                return false;
            }

            try
            {
                using (var authorizationAttributeHelper = IocHelper.ResolveAsDisposable<AuthorizeAttributeHelper>())
                {
                    authorizationAttributeHelper.Object.Authorize(this);                    
                }

                return true;
            }
            catch (AbpAuthorizationException ex)
            {
                LogHelper.Logger.Warn(ex.Message, ex);
                return false;
            }
        }
    }
}
