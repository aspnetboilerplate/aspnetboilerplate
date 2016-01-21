using System.Collections.Generic;
using Abp.Dependency;
using Abp.Reflection;
using Castle.DynamicProxy;

namespace Abp.Application.Features
{
    /// <summary>
    /// Intercepts methods to apply <see cref="RequiresFeatureAttribute"/>.
    /// </summary>
    public class FeatureInterceptor : IInterceptor
    {
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInterceptor"/> class.
        /// </summary>
        /// <param name="iocResolver">The ioc resolver.</param>
        public FeatureInterceptor(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public void Intercept(IInvocation invocation)
        {
            var featureAttributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType<RequiresFeatureAttribute>(
                    invocation.MethodInvocationTarget
                    );

            if (featureAttributes.Count <= 0)
            {
                invocation.Proceed();
                return;
            }

            CheckFeatures(featureAttributes);

            invocation.Proceed();
        }

        private void CheckFeatures(IEnumerable<RequiresFeatureAttribute> featureAttributes)
        {
            _iocResolver.Using<IFeatureChecker>(featureChecker =>
            {
                foreach (var featureAttribute in featureAttributes)
                {
                    featureChecker.CheckEnabled(featureAttribute.RequiresAll, featureAttribute.Features);
                }
            });
        }
    }
}
