using System;
using System.Collections.Generic;
using Abp.Dependency;
using Hangfire;

namespace Abp.Hangfire
{
    public class HangfireIocJobActivator : JobActivator
    {
        private readonly IIocResolver _iocResolver;

        public HangfireIocJobActivator(IIocResolver iocResolver)
        {
            if (iocResolver == null)
            {
                throw new ArgumentNullException(nameof(iocResolver));
            }

            _iocResolver = iocResolver;
        }

        public override object ActivateJob(Type jobType)
        {
            return _iocResolver.Resolve(jobType);
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            return new HangfireIocJobActivatorScope(this, _iocResolver);
        }

        class HangfireIocJobActivatorScope : JobActivatorScope
        {
            private readonly JobActivator _activator;
            private readonly IIocResolver _iocResolver;

            private readonly List<object> _resolvedObjects;

            public HangfireIocJobActivatorScope(JobActivator activator, IIocResolver iocResolver)
            {
                _activator = activator;
                _iocResolver = iocResolver;
                _resolvedObjects = new List<object>();
            }

            public override object Resolve(Type type)
            {
                var instance = _activator.ActivateJob(type);
                _resolvedObjects.Add(instance);
                return instance;
            }

            public override void DisposeScope()
            {
                _resolvedObjects.ForEach(_iocResolver.Release);
            }
        }
    }
}