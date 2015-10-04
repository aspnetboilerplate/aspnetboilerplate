using System.Collections.Generic;
using Abp.Dependency;
using Abp.Reflection;
using Castle.DynamicProxy;

namespace Abp.Application.Features
{
    public class FeatureInterceptor : IInterceptor
    {
        private readonly IIocResolver _iocResolver;

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

        private void CheckFeatures(List<RequiresFeatureAttribute> featureAttributes)
        {
            if (_iocResolver.IsRegistered<IFeatureChecker>())
            {
                _iocResolver.Using<IFeatureChecker>(featureChecker =>
                {
                    foreach (var featureAttribute in featureAttributes)
                    {
                        featureChecker.CheckEnabled(featureAttribute.RequiresAll, featureAttribute.Features);
                    }
                });
            }
            else
            {
                foreach (var featureAttribute in featureAttributes)
                {
                    NullFeatureChecker.Instance.CheckEnabled(featureAttribute.RequiresAll, featureAttribute.Features);
                }
            }
        }
    }
}
