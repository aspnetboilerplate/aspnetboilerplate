using System;
using Abp.Application.Services;

namespace Abp.Application.Authorization
{
    /// <summary>
    /// This attribute is used on a method of an Application Service (A class that implements <see cref="IApplicationService"/>)
    /// to make that method usable only by authorized users.
    /// </summary>
    public class AbpAuthorizeAttribute : Attribute, IFeatureBasedAuthorization
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
                Features = new[]{singleFeature};
            }
            else
            {
                Features = new string[0];                
            }
        }
    }
}
