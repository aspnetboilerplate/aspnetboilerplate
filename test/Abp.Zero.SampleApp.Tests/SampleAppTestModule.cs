using System;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.Ldap;
using Abp.Zero.SampleApp.EntityFramework;
using Castle.MicroKernel.Registration;
using Microsoft.Owin.Security;
using NSubstitute;

namespace Abp.Zero.SampleApp.Tests
{
    [DependsOn(
        typeof(SampleAppEntityFrameworkModule),
        typeof(AbpZeroLdapModule),
        typeof(AbpTestBaseModule))]
    public class SampleAppTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(2);
        }

        public override void Initialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IAuthenticationManager>().Instance(Substitute.For<IAuthenticationManager>())
            );
        }
    }
}