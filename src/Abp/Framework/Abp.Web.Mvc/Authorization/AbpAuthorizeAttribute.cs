using System.Web.Mvc;
using Abp.Application.Authorization;
using Abp.Application.Authorization.Permissions;

namespace Abp.Web.Mvc.Authorization
{
    /// <summary>
    /// This attribute is used on an action of an MVC <see cref="Controller"/>
    /// to make that action usable only by authorized users.
    /// </summary>
    public class AbpAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// A list of permissions to authorize.
        /// A user is authorized if any of the permissions is granted.
        /// </summary>
        public string[] Permissions { get; set; }

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

            return true;
        }
    }
}
