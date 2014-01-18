using System.Web.Http;
using Abp.Application.Authorization;

namespace Abp.WebApi.Authorization
{
    /// <summary>
    /// This attribute is used on a method of an <see cref="ApiController"/>
    /// to make that method usable only by authorized users.
    /// TODO: This class is not implemented yet.
    /// </summary>
    public class AbpAuthorizeAttribute : AuthorizeAttribute, IFeatureBasedAuthorization
    {
        /// <summary>
        /// A list of features to authorize.
        /// A user is authorized if any of the features is allowed.
        /// </summary>
        public string[] Features { get; set; }

        /// <param name="singleFeature">
        /// A shortcut to create a AbpAuthorizeAttribute that has only one feature.
        /// If more than one feature is added, <see cref="Features"/> should be used.
        /// </param>
        public AbpAuthorizeAttribute(string singleFeature = null)
        {
            if (!string.IsNullOrEmpty(singleFeature))
            {
                Features = new[] { singleFeature };
            }
            else
            {
                Features = new string[0];
            }
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
