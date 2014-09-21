using System;
using System.Web.Mvc;
using Abp.Application.Authorization;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Logging;
using Castle.Core.Logging;

namespace Abp.Web.Mvc.Authorization
{
    /// <summary>
    /// This attribute is used on an action of an MVC <see cref="Controller"/>
    /// to make that action usable only by authorized users. 
    /// </summary>
    public class AbpAuthorizeAttribute : AuthorizeAttribute, IAbpAuthorizeAttribute
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

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
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
