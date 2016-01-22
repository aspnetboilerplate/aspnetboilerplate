using System;
using Abp.Dependency;
using Hangfire;

namespace Abp.Hangfire
{
    public class HangfireWindsorJobActivator : JobActivator
    {
        readonly IIocResolver _iocResolver;

        /// <summary>
        /// Initializes new instance of WindsorJobActivator with a Windsor Kernel
        /// </summary>
        public HangfireWindsorJobActivator(IIocResolver iocResolver)
        {
            if (iocResolver == null)
            {
                throw new ArgumentNullException("iocResolver");
            }

            _iocResolver = iocResolver;
        }

        /// <summary>
        /// Activates a job of a given type using the Windsor Kernel
        /// </summary>
        /// <param name="jobType">Type of job to activate</param>
        /// <returns></returns>
        public override object ActivateJob(Type jobType)
        {
            return _iocResolver.Resolve(jobType);
        }
    }
}