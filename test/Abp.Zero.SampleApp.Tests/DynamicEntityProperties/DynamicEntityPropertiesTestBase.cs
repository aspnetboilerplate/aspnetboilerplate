using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.DynamicEntityProperties;
using Abp.Localization;
using Abp.Modules;
using Abp.Threading;
using Abp.UI.Inputs;
using Abp.Zero.SampleApp.EntityHistory;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityProperties
{
    public class DynamicEntityPropertiesTestBase : SampleAppTestBase<DynamicEntityPropertiesTestModule>
    {
        public const string TestPermission = "Abp.Zero.TestPermission";
        public string TestEntityFullName => typeof(Country).FullName;

        protected readonly IDynamicPropertyStore DynamicPropertyStore;
        protected readonly IDynamicEntityPropertyStore DynamicEntityPropertyStore;

        public DynamicEntityPropertiesTestBase()
        {
            DynamicPropertyStore = Resolve<IDynamicPropertyStore>();
            DynamicEntityPropertyStore = Resolve<IDynamicEntityPropertyStore>();

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
                if (e.Message.Contains(requiredPermission) ||
                    e.InnerException != null && e.InnerException.Message.Contains(requiredPermission))
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

        protected async Task RunAndCheckIfPermissionControlledAsync(Func<Task> function,
            string requiredPermission = TestPermission)
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
                if (e.Message.Contains(requiredPermission) ||
                    e.InnerException != null && e.InnerException.Message.Contains(requiredPermission))
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

        protected string GetRandomAllowedInputType()
        {
            return Resolve<IDynamicEntityPropertyDefinitionManager>().GetAllAllowedInputTypeNames().First();
        }

        protected DynamicProperty CreateAndGetDynamicPropertyWithTestPermission()
        {
            var dynamicProperty = new DynamicProperty()
            {
                InputType = GetRandomAllowedInputType(),
                PropertyName = "City" + Guid.NewGuid().ToString().Substring(0, 5),
                Permission = TestPermission,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { DynamicPropertyStore.Add(dynamicProperty); });

            return dynamicProperty;
        }

        protected DynamicEntityProperty CreateAndGetDynamicEntityProperty()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { DynamicEntityPropertyStore.Add(dynamicEntityProperty); });
            return dynamicEntityProperty;
        }

        protected T RegisterFake<T>() where T : class
        {
            var substitute = Substitute.For<T>();
            LocalIocManager.IocContainer.Register(Component.For<T>().Instance(substitute).IsDefault());
            return substitute;
        }
    }

    public class MyDynamicEntityPropertyDefinitionProvider : DynamicEntityPropertyDefinitionProvider
    {
        public override void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context)
        {
            context.Manager.AddAllowedInputType<SingleLineStringInputType>();
            context.Manager.AddAllowedInputType<CheckboxInputType>();
            context.Manager.AddAllowedInputType<ComboboxInputType>();

            context.Manager.AddEntity<Country, int>();
        }
    }

    public class DynamicEntityPropertiesTestAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(DynamicEntityPropertiesTestBase.TestPermission,
                new FixedLocalizableString(DynamicEntityPropertiesTestBase.TestPermission));
        }
    }

    [DependsOn(typeof(SampleAppTestModule))]
    public class DynamicEntityPropertiesTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DynamicEntityPropertiesTestModule).Assembly);

            Configuration.Authorization.Providers.Add<DynamicEntityPropertiesTestAuthorizationProvider>();
            Configuration.DynamicEntityProperties.Providers.Add<MyDynamicEntityPropertyDefinitionProvider>();
        }
    }
}