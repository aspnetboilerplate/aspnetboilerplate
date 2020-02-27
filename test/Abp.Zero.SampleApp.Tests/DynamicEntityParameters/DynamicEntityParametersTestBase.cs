using System;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.DynamicEntityParameters;
using Abp.Localization;
using Abp.Modules;
using Abp.Threading;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityParameters
{
    public class DynamicEntityParametersTestBase : DynamicEntityParametersTestBase<DynamicEntityParametersTestModule>
    {
    }

    public class DynamicEntityParametersTestBase<TModule> : SampleAppTestBase<TModule>
        where TModule : AbpModule
    {
        public const string TestPermission = "Abp.Zero.TestPermission";
        public const string TestEntityFullName = "Abp.Zero.TestEntity";

        protected readonly IDynamicParameterStore DynamicParameterStore;
        protected readonly IEntityDynamicParameterStore EntityDynamicParameterStore;

        public DynamicEntityParametersTestBase()
        {
            DynamicParameterStore = Resolve<IDynamicParameterStore>();
            EntityDynamicParameterStore = Resolve<IEntityDynamicParameterStore>();

            AbpSession.UserId = 1;
            AbpSession.TenantId = 1;

            var user = AsyncHelper.RunSync(() => UserManager.FindByIdAsync(AbpSession.UserId.Value));
            AsyncHelper.RunSync(() => GrantPermissionAsync(user, TestPermission));
        }

        protected void RunAndCheckIfPermissionControlled(Action function, string requiredPermission = TestPermission)
        {
            var user = AsyncHelper.RunSync(() => UserManager.FindByIdAsync(AbpSession.UserId.Value));
            AsyncHelper.RunSync(() => ProhibitPermissionAsync(user, requiredPermission));

            bool isExceptionThrown = false;
            try
            {
                WithUnitOfWork(function.Invoke);
            }
            catch (Exception e)
            {
                if (e.Message.Contains(requiredPermission) || e.InnerException != null && e.InnerException.Message.Contains(requiredPermission))
                {
                    isExceptionThrown = true;
                }
                else
                {
                    throw;
                }
            }

            if (!isExceptionThrown)
            {
                throw new Exception("Should throw exception for unauthorized users");
            }

            AsyncHelper.RunSync(() => GrantPermissionAsync(user, requiredPermission));

            WithUnitOfWork(function.Invoke);
        }

        protected async Task RunAndCheckIfPermissionControlledAsync(Func<Task> function, string requiredPermission = TestPermission)
        {
            var user = await UserManager.FindByIdAsync(AbpSession.UserId.Value);

            await ProhibitPermissionAsync(user, requiredPermission);

            bool isExceptionThrown = false;
            try
            {
                await WithUnitOfWorkAsync(function.Invoke);
            }
            catch (Exception e)
            {
                if (e.Message.Contains(requiredPermission) || e.InnerException != null && e.InnerException.Message.Contains(requiredPermission))
                {
                    isExceptionThrown = true;
                }
                else
                {
                    throw;
                }
            }

            if (!isExceptionThrown)
            {
                throw new Exception("Should throw exception for unauthorized users");
            }

            await GrantPermissionAsync(user, requiredPermission);

            await WithUnitOfWorkAsync(function.Invoke);
        }


        protected DynamicParameter CreateAndGetDynamicParameterWithTestPermission()
        {
            var dynamicParameter = new DynamicParameter()
            {
                InputType = "string",
                ParameterName = "City",
                Permission = TestPermission,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                DynamicParameterStore.Add(dynamicParameter);
            });

            return dynamicParameter;
        }

        protected EntityDynamicParameter CreateAndGetEntityDynamicParameter()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                EntityDynamicParameterStore.Add(entityDynamicParameter);
            });
            return entityDynamicParameter;
        }

        protected T RegisterFake<T>() where T : class
        {
            var substitute = Substitute.For<T>();
            LocalIocManager.IocContainer.Register(Component.For<T>().Instance(substitute).IsDefault());
            return substitute;
        }
    }

    public class DynamicEntityParametersTestAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(DynamicEntityParametersTestBase.TestPermission,
                new FixedLocalizableString(DynamicEntityParametersTestBase.TestPermission));
        }
    }

    [DependsOn(typeof(SampleAppTestModule))]
    public class DynamicEntityParametersTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<DynamicEntityParametersTestAuthorizationProvider>();
            IocManager.RegisterAssemblyByConvention(typeof(DynamicEntityParametersTestModule).Assembly);
        }
    }
}
