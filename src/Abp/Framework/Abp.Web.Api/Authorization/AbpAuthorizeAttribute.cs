using System.Web.Http;
using Abp.Application.Authorization;
using Abp.Application.Authorization.Permissions;

namespace Abp.WebApi.Authorization
{
    /// <summary>
    /// This attribute is used on a method of an <see cref="ApiController"/>
    /// to make that method usable only by authorized users.
    /// TODO: This class is not implemented yet.
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
