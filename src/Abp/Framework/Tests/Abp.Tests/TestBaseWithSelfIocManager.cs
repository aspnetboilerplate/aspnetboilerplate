using System;
using Abp.Dependency;

namespace Abp.Tests
{
    public abstract class TestBaseWithSelfIocManager : IDisposable
    {
        protected IIocManager LocalIocManager;

        protected TestBaseWithSelfIocManager()
        {
            LocalIocManager = new IocManager();
        }

        public virtual void Dispose()
        {
            LocalIocManager.Dispose();
        }
    }
}